using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Services.DataProviders;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PJ.DailyFieldReports.Descriptor;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;
using PX.Objects.PJ.DailyFieldReports.PJ.Graphs;
using PX.Objects.PM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.PJ.DailyFieldReports.Common.GenericGraphExtensions;
using PX.Objects.PJ.DailyFieldReports.Common.MappedCacheExtensions;
using PX.Objects.PJ.DailyFieldReports.Common.Mappings;
using PX.Objects.PJ.DailyFieldReports.PJ.GraphExtensions;
using static PX.Objects.PJ.DailyFieldReports.PJ.GraphExtensions.DailyFieldReportEntryProgressWorksheetExtension;
/// <summary>
/// This extension is a copy of DailyFieldReportEntryProgressWorksheetExtension
/// Creaed to provide the following functionality for users
/// Only allow progress tracking for Active tasks
/// Upgrade Notes: Reapply any changes made to DailyFieldReportEntryProgressWorksheetExtension
/// </summary>
namespace GSynchExt
{
    public class DailyFieldReportEntryGSExt : PXGraphExtension<DailyFieldReportEntry>, PXImportAttribute.IPXPrepareItems
	{
		#region Selects
		public PXSetup<PMProject>.Where<PMProject.contractID.IsEqual<DailyFieldReport.projectId.FromCurrent>> Project;
		public PXSetup<PMSetup> Setup;
		public SelectFrom<PMProgressWorksheetLine>
				.LeftJoin<DailyFieldReportProgressWorksheet>
				.On<PMProgressWorksheetLine.refNbr.IsEqual<DailyFieldReportProgressWorksheet.progressWorksheetId>
				.Or<Brackets<PMProgressWorksheetLine.refNbr.IsNull.And<DailyFieldReportProgressWorksheet.progressWorksheetId.IsNull>>>>
				.LeftJoin<PMProgressWorksheet>
				.On<PMProgressWorksheetLine.refNbr.IsEqual<PMProgressWorksheet.refNbr>>
				.Where<DailyFieldReportProgressWorksheet.dailyFieldReportId.IsEqual<DailyFieldReport.dailyFieldReportId.FromCurrent>>.View ProgressWorksheetLinesGS;

		[PXCopyPasteHiddenView]
		public SelectFrom<PMProgressWorksheet>
				.LeftJoin<DailyFieldReportProgressWorksheet>
				.On<PMProgressWorksheet.refNbr.IsEqual<DailyFieldReportProgressWorksheet.progressWorksheetId>>
				.Where<DailyFieldReportProgressWorksheet.dailyFieldReportId.IsEqual<DailyFieldReport.dailyFieldReportId.FromCurrent>>.View ProgressWorksheetsGS;

		[PXCopyPasteHiddenView]
		public SelectFrom<DailyFieldReportProgressWorksheet>
				.Where<DailyFieldReportProgressWorksheet.dailyFieldReportId
				.IsEqual<DailyFieldReport.dailyFieldReportId.FromCurrent>>.View DailyFieldReportProgressWorksheetsGS;
		public PXFilter<CostBudgetLineFilterGS> costBudgetfilterGS;

		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<PMCostBudget,
		LeftJoin<PMCostCode, On<PMCostBudget.costCodeID, Equal<PMCostCode.costCodeID>>,
		InnerJoin<PMTask, On<PMCostBudget.projectID, Equal<PMTask.projectID>, And<PMCostBudget.projectTaskID, Equal<PMTask.taskID>>>,
		InnerJoin<PMAccountGroup, On<PMCostBudget.accountGroupID, Equal<PMAccountGroup.groupID>>>>>,
		Where<PMCostBudget.projectID, Equal<Current<CostBudgetLineFilterGS.projectID>>,
			And<PMCostBudget.type, Equal<PX.Objects.GL.AccountType.expense>,
			And<PMCostBudget.productivityTracking, IsNotNull,
			And<PMCostBudget.productivityTracking, NotEqual<PMProductivityTrackingType.notAllowed>,
			And<PMTask.status, Equal<ProjectTaskStatus.active>,
			And<PMAccountGroup.isActive, Equal<True>>>>>>>> CostBudgetsGS;
		#endregion
		#region Overrides
		public PXAction<DailyFieldReport> ViewProgressWorksheetGS;

		[PXButton]
		[PXUIField]
		public virtual void viewProgressWorksheetGS()
		{
			if (ProgressWorksheetLinesGS.Current != null && ProgressWorksheetLinesGS.Current.RefNbr != null)
			{
				var progressWorksheetEntry = PXGraph.CreateInstance<ProgressWorksheetEntry>();
				progressWorksheetEntry.Document.Current =
					progressWorksheetEntry.Document.Search<PMProgressWorksheet.refNbr>(ProgressWorksheetLinesGS.Current.RefNbr);
				if (progressWorksheetEntry.Document.Current != null)
				{
					PXRedirectHelper.TryRedirect(progressWorksheetEntry, PXRedirectHelper.WindowMode.NewWindow);
				}
			}
		}

		[System.SerializableAttribute()]
		[PXCacheName(PX.Objects.PM.Messages.CostBudgetLineFilter)]
		public partial class CostBudgetLineFilterGS : PXBqlTable, IBqlTable
		{
			#region ProjectID
			public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

			/// <summary>
			/// The identifier of the <see cref="PMProject">project</see> associated with the Cost Budget Line Filter.
			/// </summary>
			/// <value>
			/// The value of this field corresponds to the value of the <see cref="PMProject.ContractID"/> field.
			/// </value>
			[PXInt()]
			public virtual Int32? ProjectID
			{
				get;
				set;
			}
			#endregion
			#region TaskID
			public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }

			/// <summary>
			/// The identifier of the <see cref="PMTask">task</see> associated with the cost budget line filter.
			/// </summary>
			/// <value>
			/// The value of this field corresponds to the value of the <see cref="PMTask.TaskID"/> field.
			/// </value>
			[ActiveProjectTask(typeof(CostBudgetLineFilterGS.projectID), CheckMandatoryCondition = typeof(Where<True, Equal<False>>), DisplayName = "Project Task")]
			public virtual Int32? TaskID
			{
				get;
				set;
			}
			#endregion
			#region AccountGroupID
			public abstract class accountGroupID : PX.Data.BQL.BqlInt.Field<accountGroupID> { }

			/// <summary>The identifier of the <see cref="PMAccountGroup">account group</see> associated with the Cost Budget Line Filter.</summary>
			/// <value>
			/// The value of this field corresponds to the value of the <see cref="PMAccountGroup.GroupID" /> field.
			/// </value>
			[PXUIField(DisplayName = "Account Group")]
			[PXInt()]
			[PXSelector(typeof(SelectFrom<PMAccountGroup>
				.InnerJoin<PMCostBudget>.On<PMAccountGroup.groupID.IsEqual<PMCostBudget.accountGroupID>>
				.Where<PMCostBudget.projectID.IsEqual<CostBudgetLineFilterGS.projectID.FromCurrent>.And<PMCostBudget.type.IsEqual<PX.Objects.GL.AccountType.expense>>.And<PMCostBudget.accountGroupID.IsNotNull>
					.And<PMAccountGroup.isExpense.IsEqual<True>>>
				.AggregateTo<GroupBy<PMCostBudget.accountGroupID>>
				.SearchFor<PMAccountGroup.groupID>),
				SubstituteKey = typeof(PMAccountGroup.groupCD))]
			public virtual Int32? AccountGroupID
			{
				get;
				set;
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

			/// <summary>
			/// The identifier of the <see cref="InventoryItem">inventory item</see> associated with the Cost Budget Line Filter.
			/// </summary>
			/// <value>
			/// The value of this field corresponds to the value of the <see cref="InventoryItem.InventoryID"/> field.
			/// </value>
			[PXUIField(DisplayName = "Inventory ID")]
			[PXInt()]
			[PXSelector(typeof(SelectFrom<InventoryItem>
				.InnerJoin<PMCostBudget>.On<InventoryItem.inventoryID.IsEqual<PMCostBudget.inventoryID>>
				.Where<PMCostBudget.projectID.IsEqual<CostBudgetLineFilterGS.projectID.FromCurrent>.And<PMCostBudget.type.IsEqual<PX.Objects.GL.AccountType.expense>>.And<PMCostBudget.inventoryID.IsNotNull>>
				.AggregateTo<GroupBy<PMCostBudget.inventoryID>>
				.SearchFor<InventoryItem.inventoryID>),
				SubstituteKey = typeof(InventoryItem.inventoryCD))]
			public virtual Int32? InventoryID
			{
				get;
				set;
			}
			#endregion
			#region CostCodeFrom
			public abstract class costCodeFrom : PX.Data.BQL.BqlInt.Field<costCodeFrom> { }

			/// <summary>
			/// The identifier of the <see cref="PMCostCode">cost code</see> associated with the Cost Budget Line Filter.
			/// </summary>
			[PXUIField(DisplayName = "Cost Code From", FieldClass = CostCodeAttribute.COSTCODE)]
			[PXInt()]
			[PXDimensionSelector(CostCodeAttribute.COSTCODE, typeof(SelectFrom<PMCostCode>
				.InnerJoin<PMCostBudget>.On<PMCostCode.costCodeID.IsEqual<PMCostBudget.costCodeID>>
				.Where<PMCostBudget.projectID.IsEqual<CostBudgetLineFilterGS.projectID.FromCurrent>.And<PMCostBudget.type.IsEqual<PX.Objects.GL.AccountType.expense>>.And<PMCostBudget.costCodeID.IsNotNull>>
				.AggregateTo<GroupBy<PMCostBudget.costCodeID>>
				.SearchFor<PMCostCode.costCodeID>), typeof(PMCostCode.costCodeCD))]
			public virtual Int32? CostCodeFrom
			{
				get;
				set;
			}
			#endregion
			#region CostCodeTo
			public abstract class costCodeTo : PX.Data.BQL.BqlInt.Field<costCodeTo> { }

			/// <summary>
			/// The identifier of the <see cref="PMCostCode">cost code</see> associated with the Cost Budget Line Filter.
			/// </summary>
			[PXUIField(DisplayName = "Cost Code To", FieldClass = CostCodeAttribute.COSTCODE)]
			[PXInt()]
			[PXDimensionSelector(CostCodeAttribute.COSTCODE, typeof(SelectFrom<PMCostCode>
				.InnerJoin<PMCostBudget>.On<PMCostCode.costCodeID.IsEqual<PMCostBudget.costCodeID>>
				.Where<PMCostBudget.projectID.IsEqual<CostBudgetLineFilterGS.projectID.FromCurrent>.And<PMCostBudget.type.IsEqual<PX.Objects.GL.AccountType.expense>>.And<PMCostBudget.costCodeID.IsNotNull>>
				.AggregateTo<GroupBy<PMCostBudget.costCodeID>>
				.SearchFor<PMCostCode.costCodeID>), typeof(PMCostCode.costCodeCD))]
			public virtual Int32? CostCodeTo
			{
				get;
				set;
			}
			#endregion
		}
		public PXAction<DailyFieldReport> loadTemplateGS;
		[PXUIField(DisplayName = "Load Template", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public IEnumerable LoadTemplateGS(PXAdapter adapter)
        {
			if (Base.DailyFieldReport.Current.Hold == true && Base.DailyFieldReport.Current.ProjectId != null)
			{
				ProgressWorksheetLinesGS.Cache.ForceExceptionHandling = true;

				Dictionary<BudgetKeyTuple, List<PMProgressWorksheet>> existingLines = GetExistingCostBudgetLinesGS();

				var costBudgetSelect = new PXSelectJoin<PMCostBudget,
					InnerJoin<PMTask, On<PMCostBudget.projectID, Equal<PMTask.projectID>, And<PMCostBudget.projectTaskID, Equal<PMTask.taskID>>>,
					InnerJoin<PMAccountGroup, On<PMCostBudget.accountGroupID, Equal<PMAccountGroup.groupID>>>>,
					Where<PMCostBudget.projectID, Equal<Required<PMProject.contractID>>,
						And<PMCostBudget.type, Equal<PX.Objects.GL.AccountType.expense>,
						And<PMCostBudget.productivityTracking, Equal<PMProductivityTrackingType.template>,
						And<PMTask.status, Equal<ProjectTaskStatus.active>,
						And<PMAccountGroup.isActive, Equal<True>>>>>>>(Base);

				foreach (PMCostBudget line in costBudgetSelect.Select(Base.DailyFieldReport.Current.ProjectId))
				{
					List<PMProgressWorksheet> list;
					if (!existingLines.TryGetValue(BudgetKeyTuple.Create(line), out list) || list.All(item => item != null && item.Status != ProgressWorksheetStatus.OnHold))
					{
						PMProgressWorksheetLine newline = new PMProgressWorksheetLine();
						newline.ProjectID = line.ProjectID;
						newline.TaskID = line.ProjectTaskID;
						newline.InventoryID = line.InventoryID;
						newline.AccountGroupID = line.AccountGroupID;
						newline.CostCodeID = line.CostCodeID;
						ProgressWorksheetLinesGS.Insert(newline);
					}
				}
			}

			return adapter.Get();
		}


		public IEnumerable costBudgetsGS()
		{
			List<object> parameters = new List<object>();
			var costBudgetSelect = new PXSelectJoin<PMCostBudget,
				LeftJoin<PMCostCode, On<PMCostBudget.costCodeID, Equal<PMCostCode.costCodeID>>,
				InnerJoin<PMTask, On<PMCostBudget.projectID, Equal<PMTask.projectID>, And<PMCostBudget.projectTaskID, Equal<PMTask.taskID>>>,
				InnerJoin<PMAccountGroup, On<PMCostBudget.accountGroupID, Equal<PMAccountGroup.groupID>>>>>,
				Where<PMCostBudget.projectID, Equal<Current<CostBudgetLineFilterGS.projectID>>,
					And<PMCostBudget.type, Equal<PX.Objects.GL.AccountType.expense>,
					And<PMCostBudget.productivityTracking, IsNotNull,
					And<PMCostBudget.productivityTracking, NotEqual<PMProductivityTrackingType.notAllowed>,
					And<PMTask.status, Equal<ProjectTaskStatus.active>,
					And<PMAccountGroup.isActive, Equal<True>>>>>>>>(Base);

			if (costBudgetfilterGS.Current.TaskID != null)
			{
				costBudgetSelect.WhereAnd(typeof(Where<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>>));
				parameters.Add(costBudgetfilterGS.Current.TaskID);
			}

			if (costBudgetfilterGS.Current.AccountGroupID != null)
			{
				costBudgetSelect.WhereAnd(typeof(Where<PMCostBudget.accountGroupID, Equal<Required<PMCostBudget.accountGroupID>>>));
				parameters.Add(costBudgetfilterGS.Current.AccountGroupID);
			}

			if (costBudgetfilterGS.Current.InventoryID != null)
			{
				costBudgetSelect.WhereAnd(typeof(Where<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>));
				parameters.Add(costBudgetfilterGS.Current.InventoryID);
			}

			if (costBudgetfilterGS.Current.CostCodeFrom != null)
			{
				PMCostCode costCodeFrom = (new PXSelect<PMCostCode, Where<PMCostCode.costCodeID, Equal<Current<CostBudgetLineFilterGS.costCodeFrom>>>>(Base)).SelectSingle();
				costBudgetSelect.WhereAnd(typeof(Where<PMCostCode.costCodeCD, GreaterEqual<Required<PMCostCode.costCodeCD>>>));
				parameters.Add(costCodeFrom.CostCodeCD);
			}

			if (costBudgetfilterGS.Current.CostCodeTo != null)
			{
				PMCostCode costCodeTo = (new PXSelect<PMCostCode, Where<PMCostCode.costCodeID, Equal<Current<CostBudgetLineFilterGS.costCodeTo>>>>(Base)).SelectSingle();
				costBudgetSelect.WhereAnd(typeof(Where<PMCostCode.costCodeCD, LessEqual<Required<PMCostCode.costCodeCD>>>));
				parameters.Add(costCodeTo.CostCodeCD);
			}
			parameters.AddRange(PXView.Parameters);

			PXDelegateResult delResult = new PXDelegateResult();
			delResult.Capacity = 202;
			delResult.IsResultFiltered = false;
			delResult.IsResultSorted = true;
			delResult.IsResultTruncated = false;

			var view = new PXView(Base, false, costBudgetSelect.View.BqlSelect);
			var startRow = PXView.StartRow;
			int totalRows = 0;
			var resultset = view.Select(PXView.Currents, parameters.ToArray(), PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;

			Dictionary<BudgetKeyTuple, List<PMProgressWorksheet>> existing = GetExistingCostBudgetLinesGS();

			foreach (PXResult<PMCostBudget, PMCostCode> costBudgetResult in resultset)
			{
				PMCostBudget costBudget = costBudgetResult;
				List<PMProgressWorksheet> list;
				if (!(existing.TryGetValue(BudgetKeyTuple.Create(costBudget), out list) && list.Any(item => item == null || item.Status == ProgressWorksheetStatus.OnHold)))
				{
					delResult.Add(costBudget);
				}
			}

			return delResult;
		}
		public void _(Events.RowInserting<PMProgressWorksheetLine> e)
		{
			if (e.Row != null)
			{
				if (e.Row.RefNbr == null)
				{
					PMProgressWorksheet pw = GetOnHoldProgressWorksheetGS();

					string refNbr = null;
					if (pw != null)
					{
						refNbr = pw.RefNbr;
					}
					else
					{
						Numbering numbering = PXSelect<Numbering, Where<Numbering.numberingID, Equal<Required<Numbering.numberingID>>>>.SelectSingleBound(Base, null, Setup.Current.ProgressWorksheetNumbering);
						if (numbering != null)
						{
							refNbr = numbering.NewSymbol;
						}
					}

					if (refNbr != null)
					{
						e.Row.RefNbr = refNbr;
						e.Row.LineNbr = ProgressWorksheetLinesGS.Select().RowCast<PMProgressWorksheetLine>().Max(line => line.LineNbr).GetValueOrDefault(0) + 1;
						e.Row.ProjectID = Project.Current.ContractID;
					}
				}
			}
		}

		public void _(Events.RowInserted<PMProgressWorksheetLine> e)
		{
			if (e.Row != null)
			{
				PMProgressWorksheet pw = GetOnHoldProgressWorksheetGS();
				if (pw == null)
				{
					pw = new PMProgressWorksheet();
					pw.Hidden = true;
					pw.Status = ProgressWorksheetStatus.OnHold;
					pw.ProjectID = Project.Current.ContractID;
					pw = ProgressWorksheetsGS.Insert(pw);
				}
			}
		}

		private ProgressWorksheetEntry graph;
		public PXAction<DailyFieldReport> selectBudgetLinesGS;
		[PXUIField(DisplayName = "Select Budget Lines", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable SelectBudgetLinesGS(PXAdapter adapter)
		{
			IEnumerable result = null;

			if (Base.DailyFieldReport.Current.Hold == true && Base.DailyFieldReport.Current.ProjectId != null)
			{
				costBudgetfilterGS.Current.ProjectID = Base.DailyFieldReport.Current.ProjectId;
				if (CostBudgetsGS.AskExt() == WebDialogResult.OK)
				{
					result = AddSelectedBudgetLines(adapter);
				}

				costBudgetfilterGS.Cache.Clear();
				CostBudgetsGS.Cache.Clear();
				CostBudgetsGS.ClearDialog();
				CostBudgetsGS.View.Clear();
				CostBudgetsGS.View.ClearDialog();
			}

			if (result != null)
			{
				return result;
			}

			return adapter.Get();
		}

		public PXAction<PMProgressWorksheet> addSelectedBudgetLines;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddSelectedBudgetLines(PXAdapter adapter)
		{
			if (Base.DailyFieldReport.Current.Hold == true)
			{
				ProgressWorksheetLinesGS.Cache.ForceExceptionHandling = true;

				Dictionary<BudgetKeyTuple, List<PMProgressWorksheet>> existing = GetExistingCostBudgetLinesGS();

				foreach (PMCostBudget line in CostBudgetsGS.Cache.Cached)
				{
					List<PMProgressWorksheet> list = null;
					if (line.Selected == true && (!existing.TryGetValue(BudgetKeyTuple.Create(line), out list) || !list.Any(item => item == null || item.Status == ProgressWorksheetStatus.OnHold)))
					{
						PMProgressWorksheetLine newline = new PMProgressWorksheetLine();
						newline.ProjectID = line.ProjectID;
						newline.TaskID = line.ProjectTaskID;
						newline.InventoryID = line.InventoryID;
						newline.AccountGroupID = line.AccountGroupID;
						newline.CostCodeID = line.CostCodeID;
						ProgressWorksheetLinesGS.Insert(newline);
					}
				}
			}

			return adapter.Get();
		}

		protected virtual void _(Events.RowSelecting<PMProgressWorksheetLine> e)
		{
			if (e.Row != null)
			{
				if (blockLineRowSelectingEvent == false)
				{
					CalculateQuantities(e.Cache, (PMProgressWorksheetLine)e.Row);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<PMProgressWorksheetLine.taskID> e)
		{
			if (e.Row != null)
			{
				CalculateQuantities(e.Cache, (PMProgressWorksheetLine)e.Row);
			}
		}

		protected virtual void _(Events.FieldUpdated<PMProgressWorksheetLine.accountGroupID> e)
		{
			if (e.Row != null)
			{
				CalculateQuantities(e.Cache, (PMProgressWorksheetLine)e.Row);
			}
		}

		protected virtual void _(Events.FieldUpdated<PMProgressWorksheetLine.inventoryID> e)
		{
			if (e.Row != null)
			{
				CalculateQuantities(e.Cache, (PMProgressWorksheetLine)e.Row);
			}
		}

		protected virtual void _(Events.FieldUpdated<PMProgressWorksheetLine.costCodeID> e)
		{
			if (e.Row != null)
			{
				CalculateQuantities(e.Cache, (PMProgressWorksheetLine)e.Row);
			}
		}

		protected virtual void _(Events.FieldUpdated<PMProgressWorksheetLine.qty> e)
		{
			if (e.Row != null)
			{
				CalculateQuantities(e.Cache, (PMProgressWorksheetLine)e.Row);
			}
		}

		private bool blockLineRowSelectingEvent;

		protected virtual void CalculateQuantities(PXCache cache, PMProgressWorksheetLine line)
		{
			try
			{
				blockLineRowSelectingEvent = true;

				PMProgressWorksheet pw;
				using (new PXConnectionScope())
				{
					pw = ProgressWorksheetsGS.Select().RowCast<PMProgressWorksheet>().SingleOrDefault(doc => doc.RefNbr == line.RefNbr);
				}

				bool newRefNbr;
				string status;
				if (pw == null)
				{
					newRefNbr = true;
					status = ProgressWorksheetStatus.OnHold;
				}
				else
				{
					newRefNbr = ProgressWorksheetsGS.Cache.GetStatus(pw) == PXEntryStatus.Inserted;
					status = pw.Status;
				}

				ProgressWorksheetEntry.CalculateQuantities(Base, cache, Base.DailyFieldReport.Current.Date.Value, status, line, newRefNbr, Setup.Current, Project.Current);
			}
			finally
			{
				blockLineRowSelectingEvent = false;
			}
		}

		protected virtual void _(Events.FieldDefaulting<PMProgressWorksheetLine, PMProgressWorksheetLine.projectID> e)
		{
			if (e.Row != null && e.Row.ProjectID == null && Project.Current != null)
			{
				e.NewValue = Project.Current.ContractID;
			}
		}

		protected virtual void _(Events.FieldDefaulting<PMProgressWorksheetLine, PMProgressWorksheetLine.inventoryID> e)
		{
			if (e.Row != null && e.Row.InventoryID == null && IsInventoryVisible(Project.Current) == false)
			{
				e.NewValue = PMInventorySelectorAttribute.EmptyInventoryID;
			}
		}

		protected virtual void _(Events.FieldDefaulting<PMProgressWorksheetLine, PMProgressWorksheetLine.costCodeID> e)
		{
			if (e.Row != null && e.Row.CostCodeID == null && IsCostCodeVisible(Project.Current) == false)
			{
				e.NewValue = CostCodeAttribute.GetDefaultCostCode();
			}
		}

		protected virtual void _(Events.FieldVerifying<DailyFieldReport.date> e)
		{
			if (e.Row != null)
			{
				foreach (PMProgressWorksheet pw in SelectFrom<PMProgressWorksheet>
					.LeftJoin<DailyFieldReportProgressWorksheet>
						.On<PMProgressWorksheet.refNbr.IsEqual<DailyFieldReportProgressWorksheet.progressWorksheetId>>
					.Where<DailyFieldReportProgressWorksheet.dailyFieldReportId.IsEqual<DailyFieldReport.dailyFieldReportId.FromCurrent>>.View.Select(Base))
				{
					if (pw.Status != ProgressWorksheetStatus.OnHold)
					{
						throw new PXSetPropertyException<DailyFieldReport.date>(DailyFieldReportMessages.NoChangeDateByProgressWorksheet);
					}
				}
			}
		}

		protected virtual void _(Events.FieldVerifying<PMProgressWorksheetLine.taskID> e)
		{
			if (e.Row != null)
			{
				PMTask data = (PMTask)PXSelectorAttribute.Select<PMProgressWorksheetLine.taskID>(e.Cache, e.Row, e.NewValue);
				PMProgressWorksheetLine row = (PMProgressWorksheetLine)e.Row;

				string refNbr = null;
				if (row.RefNbr == null)
				{
					PMProgressWorksheet pw = GetOnHoldProgressWorksheetGS();
					if (pw != null)
					{
						refNbr = pw.RefNbr;
					}
				}
				else
				{
					refNbr = row.RefNbr.Trim();
				}
				var lines = ProgressWorksheetLinesGS.Select().RowCast<PMProgressWorksheetLine>().Where(line => line.RefNbr == refNbr).ToList();
				ProgressWorksheetEntry.CheckDublicateLine(ProgressWorksheetLinesGS.Cache, lines, row, (int?)e.NewValue, row.AccountGroupID, row.InventoryID, row.CostCodeID, data?.TaskCD);
			}
		}

		protected virtual void _(Events.FieldVerifying<PMProgressWorksheetLine.accountGroupID> e)
		{
			if (e.Row != null)
			{
				PMAccountGroup data = (PMAccountGroup)PXSelectorAttribute.Select<PMProgressWorksheetLine.accountGroupID>(e.Cache, e.Row, e.NewValue);
				PMProgressWorksheetLine row = (PMProgressWorksheetLine)e.Row;

				string refNbr = null;
				if (row.RefNbr == null)
				{
					PMProgressWorksheet pw = GetOnHoldProgressWorksheetGS();
					if (pw != null)
					{
						refNbr = pw.RefNbr;
					}
				}
				else
				{
					refNbr = row.RefNbr.Trim();
				}
				var lines = ProgressWorksheetLinesGS.Select().RowCast<PMProgressWorksheetLine>().Where(line => line.RefNbr == refNbr).ToList();
				ProgressWorksheetEntry.CheckDublicateLine(ProgressWorksheetLinesGS.Cache, lines, row, row.TaskID, (int?)e.NewValue, row.InventoryID, row.CostCodeID, data?.GroupCD);
			}
		}

		protected virtual void _(Events.FieldVerifying<PMProgressWorksheetLine.inventoryID> e)
		{
			if (e.Row != null)
			{
				InventoryItem data = (InventoryItem)PXSelectorAttribute.Select<PMProgressWorksheetLine.inventoryID>(e.Cache, e.Row, e.NewValue);
				PMProgressWorksheetLine row = (PMProgressWorksheetLine)e.Row;

				string refNbr = null;
				if (row.RefNbr == null)
				{
					PMProgressWorksheet pw = GetOnHoldProgressWorksheetGS();
					if (pw != null)
					{
						refNbr = pw.RefNbr;
					}
				}
				else
				{
					refNbr = row.RefNbr.Trim();
				}
				var lines = ProgressWorksheetLinesGS.Select().RowCast<PMProgressWorksheetLine>().Where(line => line.RefNbr == refNbr).ToList();
				ProgressWorksheetEntry.CheckDublicateLine(ProgressWorksheetLinesGS.Cache, lines, row, row.TaskID, row.AccountGroupID, (int?)e.NewValue, row.CostCodeID, data?.InventoryCD);
			}
		}

		protected virtual void _(Events.FieldVerifying<PMProgressWorksheetLine.costCodeID> e)
		{
			if (e.Row != null)
			{
				PMCostCode data = (PMCostCode)PXSelectorAttribute.Select<PMProgressWorksheetLine.costCodeID>(e.Cache, e.Row, e.NewValue);
				PMProgressWorksheetLine row = (PMProgressWorksheetLine)e.Row;

				string refNbr = null;
				if (row.RefNbr == null)
				{
					PMProgressWorksheet pw = GetOnHoldProgressWorksheetGS();
					if (pw != null)
					{
						refNbr = pw.RefNbr;
					}
				}
				else
				{
					refNbr = row.RefNbr.Trim();
				}
				var lines = ProgressWorksheetLinesGS.Select().RowCast<PMProgressWorksheetLine>().Where(line => line.RefNbr == refNbr).ToList();
				ProgressWorksheetEntry.CheckDublicateLine(ProgressWorksheetLinesGS.Cache, lines, row, row.TaskID, row.AccountGroupID, row.InventoryID, (int?)e.NewValue, data?.CostCodeCD);
			}
		}

		public void _(Events.RowDeleting<PMProgressWorksheetLine> e)
		{
			if (e.Row != null)
			{
				if (e.Row.RefNbr != null && e.Row.RefNbr != Base.DailyFieldReport.Current.DailyFieldReportCd)
				{
					PMProgressWorksheet pw = PXSelect<PMProgressWorksheet, Where<PMProgressWorksheet.refNbr, Equal<Required<PMProgressWorksheet.refNbr>>>>.SelectSingleBound(Base, null, e.Row.RefNbr);
					if (pw != null && pw.Status != ProgressWorksheetStatus.OnHold)
					{
						throw new PXException(DailyFieldReportMessages.NoDeleteProgressWorksheetLine);
					}
				}
			}
		}


		public void _(Events.RowSelected<DailyFieldReport> e)
		{
			if (e.Row != null)
			{
				bool isTabEditable = ShouldTabsBeEditable(Base.DailyFieldReport.Current);

				loadTemplateGS.SetEnabled(isTabEditable);
				selectBudgetLinesGS.SetEnabled(isTabEditable);

				PXUIFieldAttribute.SetVisible<PMProgressWorksheetLine.inventoryID>(ProgressWorksheetLinesGS.Cache, null, IsInventoryVisible(Project.Current));
				PXUIFieldAttribute.SetVisible<PMProgressWorksheetLine.costCodeID>(ProgressWorksheetLinesGS.Cache, null, IsCostCodeVisible(Project.Current));
			}
		}

		public void _(Events.RowSelected<PMProgressWorksheetLine> e)
		{
			if (e.Row != null)
			{
				bool isTabEditable = ShouldTabsBeEditable(Base.DailyFieldReport.Current);

				bool isLineEditable = true;
				if (e.Row.RefNbr != null && e.Row.RefNbr != Base.DailyFieldReport.Current.DailyFieldReportCd)
				{
					PMProgressWorksheet pw = PXSelect<PMProgressWorksheet, Where<PMProgressWorksheet.refNbr, Equal<Required<PMProgressWorksheet.refNbr>>>>.SelectSingleBound(Base, null, e.Row.RefNbr);
					if (pw != null && pw.Status != ProgressWorksheetStatus.OnHold)
					{
						isLineEditable = false;
					}
				}

				PXUIFieldAttribute.SetEnabled(ProgressWorksheetLinesGS.Cache, e.Row, isTabEditable && isLineEditable);

				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.description>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.uOM>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.completedPercentTotalQuantity>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.currentPeriodQuantity>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.previouslyCompletedQuantity>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.priorPeriodQuantity>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.totalBudgetedQuantity>(ProgressWorksheetLinesGS.Cache, e.Row, false);
				PXUIFieldAttribute.SetEnabled<PMProgressWorksheetLine.totalCompletedQuantity>(ProgressWorksheetLinesGS.Cache, e.Row, false);
			}
		}

		protected virtual bool IsInventoryVisible(PMProject project)
		{
			return project != null && (project.CostBudgetLevel == BudgetLevels.Item || project.CostBudgetLevel == BudgetLevels.Detail);
		}

		protected virtual bool IsCostCodeVisible(PMProject project)
		{
			return project != null && (project.CostBudgetLevel == BudgetLevels.CostCode || project.CostBudgetLevel == BudgetLevels.Detail) &&
					PXAccess.FeatureInstalled<FeaturesSet.costCodes>();
		}

		bool PXImportAttribute.IPXPrepareItems.PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (viewName.Equals(ProgressWorksheetLinesGS.View.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				int? taskID = null;
				if (values.Contains(typeof(PMProgressWorksheetLine.taskID).Name))
				{
					string taskCD = (string)values[typeof(PMProgressWorksheetLine.taskID).Name];
					PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>
						.SelectSingleBound(Base, null, Project.Current.ContractID, taskCD);
					if (task != null)
					{
						taskID = task.TaskID;
					}
				}

				int? accountGroupID = null;
				if (values.Contains(typeof(PMProgressWorksheetLine.accountGroupID).Name))
				{
					string accountGroupCD = (string)values[typeof(PMProgressWorksheetLine.accountGroupID).Name];
					PMAccountGroup accountGroup = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupCD, Equal<Required<PMAccountGroup.groupCD>>>>
						.SelectSingleBound(Base, null, accountGroupCD);
					if (accountGroup != null)
					{
						accountGroupID = accountGroup.GroupID;
					}
				}

				int? inventoryID = null;
				if (values.Contains(typeof(PMProgressWorksheetLine.inventoryID).Name))
				{
					string inventoryCD = (string)values[typeof(PMProgressWorksheetLine.inventoryID).Name];
					InventoryItem inventory = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>
						.SelectSingleBound(Base, null, inventoryCD);
					if (inventory != null)
					{
						inventoryID = inventory.InventoryID;
					}
				}

				int? costCodeID = null;
				if (values.Contains(typeof(PMProgressWorksheetLine.costCodeID).Name))
				{
					string costCodeCD = (string)values[typeof(PMProgressWorksheetLine.costCodeID).Name];
					PMCostCode costCode = PXSelect<PMCostCode, Where<PMCostCode.costCodeCD, Equal<Required<PMCostCode.costCodeCD>>>>
						.SelectSingleBound(Base, null, costCodeCD);
					if (costCode != null)
					{
						costCodeID = costCode.CostCodeID;
					}
				}

				foreach (PXResult<PMProgressWorksheetLine, DailyFieldReportProgressWorksheet, PMProgressWorksheet> line in ProgressWorksheetLinesGS.Select())
				{
					PMProgressWorksheet progressWorksheet = line;
					PMProgressWorksheetLine progressWorksheetLine = line;
					if ((progressWorksheet == null || progressWorksheet.RefNbr == null || progressWorksheet.Hidden == true || progressWorksheet.Status == ProgressWorksheetStatus.OnHold) &&
						progressWorksheetLine.TaskID == taskID && progressWorksheetLine.AccountGroupID == accountGroupID &&
						(progressWorksheetLine.InventoryID == inventoryID || inventoryID == null && progressWorksheetLine.InventoryID == PMInventorySelectorAttribute.EmptyInventoryID) &&
						(progressWorksheetLine.CostCodeID == costCodeID || costCodeID == null && progressWorksheetLine.CostCodeID == CostCodeAttribute.GetDefaultCostCode()))
					{
						return false;
					}
				}
			}

			return true;
		}

		bool PXImportAttribute.IPXPrepareItems.RowImporting(string viewName, object row)
		{
			return row == null;
		}

		bool PXImportAttribute.IPXPrepareItems.RowImported(string viewName, object row, object oldRow)
		{
			return oldRow == null;
		}

		void PXImportAttribute.IPXPrepareItems.PrepareItems(string viewName, IEnumerable items)
		{
		}
		#endregion
		#region Methods
		protected virtual Dictionary<BudgetKeyTuple, List<PMProgressWorksheet>> GetExistingCostBudgetLinesGS()
		{
			Dictionary<BudgetKeyTuple, List<PMProgressWorksheet>> existing = new Dictionary<BudgetKeyTuple, List<PMProgressWorksheet>>();
			foreach (PXResult<PMProgressWorksheetLine, DailyFieldReportProgressWorksheet, PMProgressWorksheet> line in ProgressWorksheetLinesGS.Select())
			{
				PMProgressWorksheet progressWorksheet = line;
				PMProgressWorksheetLine progressWorksheetLine = line;
				BudgetKeyTuple lineKey = BudgetKeyTuple.Create(progressWorksheetLine);
				List<PMProgressWorksheet> list;
				if (!existing.TryGetValue(lineKey, out list))
				{
					list = new List<PMProgressWorksheet>();
					existing.Add(lineKey, list);
				}
				if (progressWorksheet == null || progressWorksheet.RefNbr == null)
				{
					list.Add(null);
				}
				else if (!list.Any(item => item != null && item.RefNbr == progressWorksheet.RefNbr))
				{
					list.Add(progressWorksheet);
				}
			}

			return existing;
		}
		private PMProgressWorksheet GetOnHoldProgressWorksheetGS()
		{
			foreach (PMProgressWorksheet pw in ProgressWorksheetsGS.Select())
			{
				if (pw.Status == ProgressWorksheetStatus.OnHold)
				{
					return pw;
				}
			}

			return null;
		}
		protected bool ShouldTabsBeEditable(DailyFieldReport dailyFieldReport)
		{
			return dailyFieldReport.Hold == true && dailyFieldReport.ProjectId != null;
		}
		#endregion
		#region Added for GAIA
		public virtual DailyFieldReport CreateDFR(PMProject proj)
		{
			DailyFieldReport dfrRec = this.Base.DailyFieldReport.Insert();
			if (proj == null) return dfrRec;
			dfrRec.ProjectId = proj.ContractID;
			dfrRec.ProjectManagerId = proj.OwnerID;
			SolarSite solarRec = SolarSite.UK.Find(this.Base, proj.ContractCD);
			if (solarRec != null)
            {
				dfrRec.SiteAddress = solarRec.Address;
				dfrRec.State = solarRec.Province;
            }
			dfrRec = this.Base.DailyFieldReport.Insert(dfrRec);
			return dfrRec;
		}

		public virtual DailyFieldReport CreateDFRFromProject(PMProject proj, bool redirect = false)
		{
			if (proj == null) return null;
			CreateDFR(proj);

			if (this.Base.DailyFieldReport.Cache.IsDirty)
			{
				if (redirect)
					throw new PXRedirectRequiredException(this.Base, "");
				else
					return this.Base.DailyFieldReport.Current;
			}
			throw new PXException("");
		}
		#endregion
	}
}
