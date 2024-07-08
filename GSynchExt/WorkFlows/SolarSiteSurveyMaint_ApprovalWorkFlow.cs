using System;
using System.Collections;
using GSynchExt.WorkFlows;
using PX.Common;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;

namespace GSynchExt
{
    using State = SolarSiteSurveyMaint_Workflow.States;
    using Self = SolarSiteSurveyMaint_ApprovalWorkFlow;
    using Context = WorkflowContext<SolarSiteSurveyMaint, SolarSiteSurvey>;
    using static SolarSiteSurvey;
    using static BoundedTo<SolarSiteSurveyMaint, SolarSiteSurvey>;

    public class SolarSiteSurveyMaint_ApprovalWorkFlow : PXGraphExtension<SolarSiteSurveyMaint_Workflow, SolarSiteSurveyMaint>
    {
        private class SolarSiteSurveyApproval : IPrefetchable
        {
            public static bool IsActive => PXDatabase.GetSlot<SolarSiteSurveyApproval>(nameof(SolarSiteSurveyApproval), typeof(SiteSetup)).RequireApproval;

            private bool RequireApproval;

            void IPrefetchable.Prefetch()
            {
                using (PXDataRecord surveySetup = PXDatabase.SelectSingle<SiteSetup>(new PXDataField<SiteSetup.siteApprovalMap>()))
                {
                    if (surveySetup != null)
                        RequireApproval = surveySetup.GetBoolean(0) ?? false;
                }
            }
        }

        protected virtual (ActionDefinition.IConfigured approve, ActionDefinition.IConfigured reject, ActionCategory.IConfigured approvalCategory) GetApprovalActions(Context context, bool hidden)
        {
            #region Categories
            ActionCategory.IConfigured approvalCategory = context.Categories.CreateNew(CommonActionCategories.ApprovalCategoryID,
                    category => category.DisplayName(CommonActionCategories.DisplayNames.Approval)
                    .PlaceAfter(CommonActionCategories.ProcessingCategoryID));
            #endregion

            var approve = context.ActionDefinitions
                .CreateExisting<Self>(g => g.approve, a => a
                .WithCategory(approvalCategory)
                .PlaceAfter(g => g.SubmitSurvey)
                .With(it => hidden ? it.IsHiddenAlways() : it)
                .WithFieldAssignments(fa => fa.Add<surveyApproved>(true)));
            var reject = context.ActionDefinitions
                .CreateExisting<Self>(g => g.reject, a => a
                .WithCategory(approvalCategory)
                .PlaceAfter(approve)
                .With(it => hidden ? it.IsHiddenAlways() : it)
                .WithFieldAssignments(fa => fa.Add<rejected>(true)));
            return (approve, reject, approvalCategory);
        }

        public class Conditions : Condition.Pack
        {
            public Condition IsApproved => GetOrCreate(b => b.FromBql<surveyApproved.IsEqual<True>>());

            public Condition IsRejected => GetOrCreate(b => b.FromBql<rejected.IsEqual<True>>());
        }

        [PXWorkflowDependsOnType(typeof(SiteSetup))]
        public override void Configure(PXScreenConfiguration config)
        {
            if (SolarSiteSurveyApproval.IsActive)
                Configure(config.GetScreenConfigurationContext<SolarSiteSurveyMaint, SolarSiteSurvey>());
            else
                HideApprovalActions(config.GetScreenConfigurationContext<SolarSiteSurveyMaint, SolarSiteSurvey>());
        }

        protected virtual void Configure(Context context)
        {
            var conditions = context.Conditions.GetPack<Conditions>();

            (var approve, var reject, var approvalCategory) = GetApprovalActions(context, hidden: false);


            context.UpdateScreenConfigurationFor(screen =>
            {
                return screen
                    .UpdateDefaultFlow(flow =>
                        flow
                        .WithFlowStates(states =>
                        {
                            states.Add<State.pendingApproval>(flowState =>
                            {
                                return flowState
                                    .WithActions(actions =>
                                    {
                                        actions.Add(approve, a => a.IsDuplicatedInToolbar());
                                        actions.Add(reject, a => a.IsDuplicatedInToolbar());
                                        actions.Add(g => g.Hold2, a => a.IsDuplicatedInToolbar());
                                    });
                            });
                            states.Add<State.rejected>(flowState =>
                            {
                                return flowState
                                    .WithActions(actions =>
                                    {
                                        actions.Add(g => g.Hold2, a => a.IsDuplicatedInToolbar());
                                    });
                            });
                        })
                        .WithTransitions(transitions =>
                        {
                            transitions.UpdateGroupFrom<State.onHold>(ts =>
                            {
                                ts.Add(t => t
                                    .To<State.pendingApproval>()
                                    .IsTriggeredOn(g => g.SubmitSurvey).When(!conditions.IsApproved));
                            });
                            transitions.AddGroupFrom<State.pendingApproval>(ts =>
                            {
                                ts.Add(t => t
                                    .To<State.completed>()
                                    .IsTriggeredOn(approve).When(conditions.IsApproved));
                                ts.Add(t => t
                                    .To<State.rejected>()
                                    .IsTriggeredOn(reject)
                                    .When(conditions.IsRejected));
                                ts.Add(t => t
                                  .To<State.onHold>()
                                  .IsTriggeredOn(g => g.Hold2));
                            });
                            transitions.AddGroupFrom<State.rejected>(ts =>
                            {
                                ts.Add(t => t
                                    .To<State.onHold>()
                                    .IsTriggeredOn(g => g.Hold2)); ;
                            });

                        }))
                    .WithActions(actions =>
                    {
                        actions.Add(approve);
                        actions.Add(reject);
                        actions.Update(
                            g => g.SubmitSurvey,
                            // g.SubmitSurvey (in case of a bug)
                            a => a.WithFieldAssignments(fas =>
                            {
                                fas.Add<surveyApproved>(false);
                                fas.Add<rejected>(false);
                            }));
                    })
                    .WithCategories(categories =>
                    {
                        categories.Add(approvalCategory);
                    });
            });
        }

        protected virtual void HideApprovalActions(Context context)
        {
            (var approve, var reject, _) = GetApprovalActions(context, hidden: true);

            context.UpdateScreenConfigurationFor(screen =>
            {
                return screen
                    .WithActions(actions =>
                    {
                        actions.Add(approve);
                        actions.Add(reject);
                    });
            });
        }


        public PXAction<SolarSiteSurvey> approve;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Approve", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Approve(PXAdapter adapter) => adapter.Get();

        public PXAction<SolarSiteSurvey> reject;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Reject", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Reject(PXAdapter adapter) => adapter.Get();
    }
}
