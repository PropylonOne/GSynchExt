using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Objects;
using PX.Objects.PM;
using static PX.Objects.PM.ProgressWorksheetEntry;

namespace PX.Objects.PM
{
    /// <summary>
    /// ProgressWorksheetEntry is extended to provide the following functionality for users
    /// Only allow progress tracking for Active tasks
    /// Create a PMtransaction for Timeline tasks (Milestones) where completion is tracked by Quantity only. No costs
    /// And update CostBudget's ActualQty based on the progress
    /// </summary>
    public class ProgressWorksheetEntryGSExt : PXGraphExtension<PX.Objects.PM.ProgressWorksheetEntry>
    {

        #region DAC Attributes Override
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXRestrictor(typeof(Where<PMTask.status, Equal<ProjectTaskStatus.active>>), PM.Messages.PWProjectTaskNotActive, typeof(PMTask.taskCD), typeof(PMTask.status))]
        protected virtual void _(Events.CacheAttached<PMProgressWorksheetLine.taskID> e) { }
        #endregion

        #region Overrides
        public delegate IEnumerable LoadTemplateDelegate(PXAdapter adapter);

        [PXOverride]
        public IEnumerable LoadTemplate(PXAdapter adapter, LoadTemplateDelegate baseMethod)
        {
            var res = baseMethod.Invoke(adapter);
            var lines = this.Base.Details.Select();
            foreach (PMProgressWorksheetLine rec in lines)
            {
                var wsLines = (PMProgressWorksheetLine)rec;
                PMTask task = PMTask.PK.Find(this.Base, this.Base.Document.Current.ProjectID, wsLines.TaskID);
                if (task?.IsActive == false || task?.TaskID == null)
                {
                    this.Base.Details.Delete(wsLines);

                }
            }

            return res;
        }
        public IEnumerable costBudgets()
        {
            var sel = new PXView(Base, true, Base.CostBudgets.View.BqlSelect);

            if (true)
            {
                sel.WhereAnd(typeof(Where<PMTask.isActive, Equal<True>>));
            }

            int totalRow = 0;
            int startRow = PXView.StartRow;

            return sel.Select(PXView.Currents, PXView.Parameters,
                PXView.Searches, PXView.SortColumns, PXView.Descendings,
                PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRow);
        }


        [PXOverride]
        public virtual void ReleaseDocument(PMProgressWorksheet doc, Action<PMProgressWorksheet> method)
        {
            try
            {
                method?.Invoke(doc);
                if (doc == null)
                    return;

                if (doc.Released == true)
                {
                    UpdateCostBudgetActuals(doc);
                }
            }
            catch (Exception e)
            {
                throw new PXException(e.Message);
            }
        }
        #endregion
        #region New Methods
        public virtual void UpdateCostBudgetActuals(PMProgressWorksheet doc)
        {
            if (doc == null)
                return;
            bool bUpdated = false;
            var lines = this.Base.Details.Select().Where(x => x.Record.RefNbr == doc.RefNbr);
            var cache = this.Base.Caches[typeof(PMProgressWorksheetLine)];

            //Fetch the Project
            var prjGraph = PXGraph.CreateInstance<ProjectEntry>();
            prjGraph.Project.Current = PXSelect<PMProject,
                        Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this.Base, doc.ProjectID);
            var costCache = prjGraph.Caches[typeof(PMCostBudget)];
            var taskCache = prjGraph.Caches[typeof(PMTask)];


            List<int> tasksList = new List<int>();
            ///Initiate a Register Entry in case there are timeline transactions to record.
            var regGraph = PXGraph.CreateInstance<RegisterEntry>();


            PMAccountGroup accGrp = GSProjectHelper.GetTimelineDefaultAccntGrp(this.Base);


            foreach (PMProgressWorksheetLine res in lines)
            {
                var linesrec = (PMProgressWorksheetLine)res;
                /// Re-calculate the quantities.
                PX.Objects.PM.ProgressWorksheetEntry.CalculateQuantities(this.Base, cache, (DateTime)doc.Date, doc.Status, linesrec, false, this.Base.Setup.Current, prjGraph.Project.Current);

                if (res.Qty == 0.00m)
                    continue;
                else
                {
                    PMCostBudget prjCostTasks = PXSelect<PMCostBudget,
                        Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                        And<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>,
                        And<PMCostBudget.costCodeID, Equal<Required<PMCostBudget.costCodeID>>,
                        And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>,
                        And<PMCostBudget.accountGroupID, Equal<Required<PMCostBudget.accountGroupID>>>>>>>>.Select(this.Base, doc.ProjectID, res.TaskID, res.CostCodeID, res.InventoryID, res.AccountGroupID);
                    ///Check if its a milestone task with no cost amount
                    if (prjCostTasks.AccountGroupID == accGrp?.GroupID)
                    {
                        if (prjCostTasks?.TaskID != null && prjCostTasks.RevisedAmount == 0.00m && prjCostTasks.RevisedQty != 0 && prjCostTasks.IsProduction == true)
                        {
                            //costCache.SetValueExt<PMCostBudget.actualQty>(prjCostTasks, linesrec.TotalCompletedQuantity); This is not required as you create the project transaction line
                            costCache.SetValueExt<PMCostBudget.completedPct>(prjCostTasks, linesrec.CompletedPercentTotalQuantity);
                            costCache.SetValueExt<PMCostBudget.percentCompleted>(prjCostTasks, linesrec.CompletedPercentTotalQuantity);

                            if (prjGraph.CostBudget.Update(prjCostTasks) != null)
                            {
                                bUpdated = true;
                                //Create unique tasks list
                                if (!tasksList.Contains((int)prjCostTasks?.TaskID))
                                {
                                    tasksList.Add((int)prjCostTasks?.TaskID);
                                }

                                if (prjCostTasks.AccountGroupID == accGrp?.GroupID && linesrec.Qty != 0) //Its a Timeline Task. You need to explicitly enter the Proj Tran record to track the completed qty even after re-calculating projects
                                {
                                    if (regGraph?.Document.Current?.Description == null)
                                    {
                                        PMRegister reg = new PMRegister();
                                        reg.Module = "PM";
                                        reg.Description = "System Generated - Milestone Tasks Quantities in Worksheet " + this.Base.Document.Current.RefNbr;
                                        regGraph.Document.Current = regGraph.Document.Insert(reg);
                                    }

                                    InsertMilestoneProjTrans(regGraph, prjCostTasks, linesrec);
                                }
                            }
                        }
                    }
                }
            }
            if (bUpdated)
            {
                foreach (int res in tasksList)
                {
                    PMTask task = (PMTask)prjGraph.Tasks.Select().Where(x => x.Record.TaskID == res).FirstOrDefault();
                    var completedPercent = PMTaskCompletedAttribute.CalculateTaskCompletionPercentage(prjGraph, task);
                    taskCache.SetValue<PMTask.completedPercent>(task, completedPercent);
                    if (IsMilestoneTaskCompleted(prjGraph, task))
                    {
                        taskCache.SetValueExt<PMTask.status>(task, ProjectTaskStatus.Completed);
                    }
                    taskCache.SetStatus(task, PXEntryStatus.Updated);
                    taskCache.Update(task);

                }
                prjGraph.Actions.PressSave();

                regGraph.ReleaseDocument(regGraph?.Document.Current);
                regGraph.Actions.PressSave();
            }
        }
        protected virtual void InsertMilestoneProjTrans(RegisterEntry graph, PMCostBudget costTask, PMProgressWorksheetLine wsLine)
        {
            if (graph == null) return;
            if (costTask == null) return;
            PMTran tran = new PMTran
            {
                RefNbr = graph.Document.Current.RefNbr,
                ProjectID = costTask.ProjectID,
                TaskID = costTask.TaskID,
                CostCodeID = costTask.CostCodeID,
                AccountGroupID = costTask.AccountGroupID,
                UOM = costTask.UOM,
                Qty = wsLine.Qty,
                Billable = false,
                Description = costTask.Description
            };

            graph.Transactions.Insert(tran);
        }
        public virtual bool IsMilestoneTaskCompleted(ProjectEntry prjGraph, PMTask task)
        {
            if (task == null) return false;
            if (task.CompletedPctMethod == PMCompletedPctMethod.ByQuantity)
            {
                var costTask = PXSelect<PMCostBudget,
                    Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                    And<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>,
                    And<PMCostBudget.revisedQty, Greater<PMCostBudget.actualQty>>>>>.Select(this.Base, task.ProjectID, task.TaskID);

                if (costTask == null) return true;
            }
            if (task.CompletedPctMethod == PMCompletedPctMethod.ByAmount)
            {
                var costTask = PXSelect<PMCostBudget,
                    Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                    And<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>,
                    And<PMCostBudget.revisedAmount, Greater<PMCostBudget.actualAmount>>>>>.Select(this.Base, task.ProjectID, task.TaskID);
                if (costTask == null) return true;
            }
            return false;
        }
        #endregion
    }
}