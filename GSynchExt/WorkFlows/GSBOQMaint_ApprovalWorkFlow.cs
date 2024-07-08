using System;
using System.Collections;
using GSynchExt.WorkFlows;
using PX.Common;
using PX.Data;
using static PX.Data.PXDatabase;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;

namespace GSynchExt
{
	using State = BOQWorkFlow.States;
	using Self = GSBOQMaint_ApprovalWorkflow;
	using Context = WorkflowContext<GSBOQMaint, GSBOQ>;
	using static GSBOQ;
	using static BoundedTo<GSBOQMaint, GSBOQ>;

	public class GSBOQMaint_ApprovalWorkflow : PXGraphExtension<BOQWorkFlow, GSBOQMaint>
	{
		private class GSBOQApproval : IPrefetchable
		{
			public static bool IsActive => PXDatabase.GetSlot<GSBOQApproval>(nameof(GSBOQApproval), typeof(GSBOQSetup)).RequireApproval;

			private bool RequireApproval;

			void IPrefetchable.Prefetch()
			{
				using (PXDataRecord bOQSetup = PXDatabase.SelectSingle<GSBOQSetup>(new PXDataField<GSBOQSetup.approvalMap>()))
				{
					if (bOQSetup != null)
						RequireApproval = bOQSetup.GetBoolean(0) ?? false;
				}
			}
		}

		public class Conditions : Condition.Pack
		{
			public Condition IsApproved => GetOrCreate(b => b.FromBql<approved.IsEqual<True>>());

			public Condition IsRejected => GetOrCreate(b => b.FromBql<rejected.IsEqual<True>>());
		}

		[PXWorkflowDependsOnType(typeof(GSBOQSetup))]
		public override void Configure(PXScreenConfiguration config)
		{
			if (GSBOQApproval.IsActive)
				Configure(config.GetScreenConfigurationContext<GSBOQMaint, GSBOQ>());
			else
				HideApprovalActions(config.GetScreenConfigurationContext<GSBOQMaint, GSBOQ>());
		}

		protected virtual void Configure(Context context)
		{
			var conditions = context.Conditions.GetPack<Conditions>();
			//var baseConditions = context.Conditions.GetPack<SolarSiteWorkflow.Conditions>();

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
									.IsTriggeredOn(g => g.RemoveHold).When(!conditions.IsApproved));
							});
							transitions.AddGroupFrom<State.pendingApproval>(ts =>
							{
								ts.Add(t => t
									.To<State.active>()
									.IsTriggeredOn(approve));
								ts.Add(t => t
									.To<State.rejected>()
									.IsTriggeredOn(reject));
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
							g => g.Hold2,
							a => a.WithFieldAssignments(fas =>
							{
								fas.Add<approved>(false);
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
				.PlaceAfter(g => g.RemoveHold)
				.With(it => hidden ? it.IsHiddenAlways() : it)
				.WithFieldAssignments(fa => fa.Add<approved>(true)));
			var reject = context.ActionDefinitions
				.CreateExisting<Self>(g => g.reject, a => a
				.WithCategory(approvalCategory)
				.PlaceAfter(approve)
				.With(it => hidden ? it.IsHiddenAlways() : it)
				.WithFieldAssignments(fa => fa.Add<rejected>(true)));
			return (approve, reject, approvalCategory);
		}

		public PXAction<GSBOQ> approve;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Approve", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Approve(PXAdapter adapter) => adapter.Get();

		public PXAction<GSBOQ> reject;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Reject", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Reject(PXAdapter adapter) => adapter.Get();
	}
}
