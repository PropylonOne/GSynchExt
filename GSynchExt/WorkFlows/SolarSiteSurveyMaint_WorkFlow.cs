using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CN.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GSynchExt.SolarSiteSurvey;
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.SolarSiteSurveyMaint, GSynchExt.SolarSiteSurvey>;

namespace GSynchExt.WorkFlows
{
    public class SolarSiteSurveyMaint_Workflow :
        PX.Data.PXGraphExtension<SolarSiteSurveyMaint>
    {
        #region Constants
        public static class States
        {
            public const string OnHold = Status.OnHold;
            public const string Rejected = Status.Rejected;
            public const string PendingApproval = Status.PendingApproval;
            public const string Completed = Status.Completed;
            public class completed : PX.Data.BQL.BqlString.Constant<completed>
            {
                public completed() : base(Completed) { }
            }
        
            public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval>
            {
                public pendingApproval() : base(PendingApproval) { }
            }

            public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
            {
                public onHold() : base(OnHold) { }
            }

            public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
            {
                public rejected() : base(Rejected) { }
            }

        }
        #endregion

        public override void Configure(PXScreenConfiguration config)
        {

            var context = config.GetScreenConfigurationContext<SolarSiteSurveyMaint, SolarSiteSurvey>();

            #region Categories
            var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
                category => category.DisplayName(CategoryNames.Processing));
            var approvalCategory = context.Categories.CreateNew(CategoryID.Approval,
                category => category.DisplayName(CategoryNames.Approval));
            #endregion

            #region Conditions

            Condition Bql<T>() where T : IBqlUnary, new() => context.Conditions.FromBql<T>();
            var conditions = new
            {
                IsApproved
                    = Bql<surveyApproved.IsEqual<True>>()
            }.AutoNameConditions();
            #endregion

            context.AddScreenConfigurationFor(screen =>
                     screen
                     .StateIdentifierIs<SolarSiteSurvey.siteStatus>()
                     .AddDefaultFlow(flow => flow
                     .WithFlowStates(fss =>
                     {
                         fss.Add<States.onHold>(flowState =>
                         {
                             return flowState
                             .IsInitial()
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.SubmitSurvey, a => a
                                 .IsDuplicatedInToolbar()
                                 .WithConnotation(ActionConnotation.Success));

                             });
                         });
                         fss.Add<States.completed>(flowState =>
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
                         transitions.AddGroupFrom<States.onHold>(ts =>
                         {
                             ts.Add(t => t.To<States.completed>()
                             .IsTriggeredOn(g => g.SubmitSurvey).When(conditions.IsApproved));

                         });
                         transitions.AddGroupFrom<States.completed>(ts =>
                         {
                             ts.Add(t => t.To<States.onHold>()
                             .IsTriggeredOn(g => g.Hold2));

                         });
                     }))
                     .WithCategories(categories =>
                     {
                         categories.Add(processingCategory);
                     })
                     .WithActions(actions =>
                     {
                         actions.Add(g => g.SubmitSurvey, c => c
                            .WithCategory(processingCategory));
                         actions.Add(g => g.Hold2, c => c
                             .WithCategory(processingCategory));
                         actions.Add(g => g.Cancel2, c => c
                             .WithCategory(processingCategory));

                     }));
        }
        public static class CategoryNames
        {
            public const string Processing = "Processing";
            public const string Approval = "Approval";
        }
        public static class CategoryID
        {
            public const string Processing = "Processing";
            public const string Approval = "Approval";
        }
    }
}
















