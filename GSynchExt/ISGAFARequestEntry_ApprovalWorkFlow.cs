using System;
using System.Collections;
using GSynchExt.WorkFlows;
using PX.Common;
using PX.Data;
using static PX.Data.PXDatabase;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;
using PX.Objects.SO;
using PX.Objects.PO;

namespace GSynchExt
{
	using State   = ISGAFARequestEntry_WorkFlow.States;
	using Self    = ISGAFARequestEntry_ApprovalWorkFlow;
	using Context = WorkflowContext<ISGAFARequestEntry, ISGAFARequest>;
	using static ISGAFARequest;
	using static BoundedTo<ISGAFARequestEntry, ISGAFARequest>;
    using static GSynchExt.ISGAFARequestLine.FK;
	public class ISGAFARequestEntry_ApprovalWorkFlow : PXGraphExtension<ISGAFARequestEntry_WorkFlow, ISGAFARequestEntry>
	{
		private class FAReqApproval : IPrefetchable
		{
			public static bool IsActive => PXDatabase.GetSlot<FAReqApproval>(nameof(FAReqApproval), typeof(ISGAFARequestSetup)).RequireApproval;
			private bool RequireApproval;
			void IPrefetchable.Prefetch()
			{
				using (PXDataRecord fARequestSetup = PXDatabase.SelectSingle<ISGAFARequestSetup>(new PXDataField<ISGAFARequestSetup.fARequestapprovalMap>()))
				{
					if (fARequestSetup != null)
						RequireApproval = fARequestSetup.GetBoolean(0) ?? false;
				}
			}
		}
		public class Conditions : Condition.Pack
		{
			public Condition IsApproved => GetOrCreate(b => b.FromBql<approved.IsEqual<True>>());
			public Condition IsRejected => GetOrCreate(b => b.FromBql<rejected.IsEqual<True>>());
		}
        [PXWorkflowDependsOnType(typeof(ISGAFARequestSetup), typeof(ISGAFARequestSetupApproval))]
        public override void Configure(PXScreenConfiguration config)
		{
			if (FAReqApproval.IsActive)
				Configure(config.GetScreenConfigurationContext<ISGAFARequestEntry, ISGAFARequest>());
			else
				HideApprovalActions(config.GetScreenConfigurationContext<ISGAFARequestEntry, ISGAFARequest>());
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
                                        actions.Add(g => g.hold, a => a.IsDuplicatedInToolbar());
                                    });
							});
							states.Add<State.rejected>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.hold, a => a.IsDuplicatedInToolbar());
									});
							});
						})
						.WithTransitions(transitions =>
						{
							transitions.UpdateGroupFrom<State.onHold>(ts =>
							{
								ts.Add(t => t
									.To<State.pendingApproval>()
									.IsTriggeredOn(g => g.removeHold).When(!conditions.IsApproved));
							});
							transitions.AddGroupFrom<State.pendingApproval>(ts =>
							{
								ts.Add(t => t
									.To<State.open>()
									.IsTriggeredOn(approve));
								ts.Add(t => t
									.To<State.rejected>()
									.IsTriggeredOn(reject));
                                ts.Add(t => t
                                    .To<State.onHold>()
                                    .IsTriggeredOn(g => g.hold));
                            });
                            transitions.AddGroupFrom<State.rejected>(ts =>
                            {
								ts.Add(t => t
									.To<State.onHold>()
									.IsTriggeredOn(g => g.hold)); ;
                            });
                        }))
					.WithActions(actions =>
					{
						actions.Add(approve);
						actions.Add(reject);
						actions.Update(
							g => g.hold,
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
				.PlaceAfter(g => g.removeHold)
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
		public PXAction<ISGAFARequest> approve;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Approve", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Approve(PXAdapter adapter)
		{
			if (this.Base.FARequest.Current != null)
			{
				this.Base.FARequest.Current.ApprovedDate = this.Base.Accessinfo.BusinessDate;
				this.Base.FARequest.Current.ApprovedBy = ISGAFARequestEntry.GetCurrentEmployee(this.Base)?.BAccountID;
				this.Base.Actions.PressSave();
			}
			return adapter.Get();
		}
		public PXAction<ISGAFARequest> reject;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Reject", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Reject(PXAdapter adapter)
		{
			if (this.Base.FARequest.Current != null)
			{
				this.Base.FARequest.Current.RejectedDate = this.Base.Accessinfo.BusinessDate;
				this.Base.FARequest.Current.RejectedBy = ISGAFARequestEntry.GetCurrentEmployee(this.Base)?.BAccountID;
				this.Base.Actions.PressSave();
			}
			return adapter.Get();
		}
	}
}
