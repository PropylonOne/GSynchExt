using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PJ.Submittals.PJ.Graphs;
using PX.Objects.PM;
using PX.Objects.RQ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GSynchExt.GSBOQMaint;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.CA.CABankTran.FK;
using static PX.Objects.FA.FABookSettings.midMonthType;
using static PX.Objects.IN.InventoryTranSumEnqFilter;
using static PX.Objects.PM.GSProjectHelper;
using static PX.Objects.TX.CSTaxCalcType;
using BudgetKeyTuple = PX.Objects.CS.BudgetKeyTuple;

namespace GSynchExt
{
    public class RequestedProjectMaterialsEntry : PXGraph<RequestedProjectMaterialsEntry, RequestedProjectMaterials>
    {

        public SelectFrom<RequestedProjectMaterials>.View ReqProjectMaterials;
        public SelectFrom<MTRequestDetails>.View MaterialReqDetails;

        protected virtual HashSet<BudgetKeyTuple> GetExistingCostBudgets()
        {
            HashSet<BudgetKeyTuple> existing = new HashSet<BudgetKeyTuple>();
            foreach (RequestedProjectMaterials line in ReqProjectMaterials.Select())
            {

                var budget = PMCostBudget.PK.Find(this, line.ProjectID, line.TaskID, line.AccountGroupID, line.CostCode, line.InventoryID);
                existing.Add(BudgetKeyTuple.Create(budget));
            }

            return existing;
        }

        public virtual void UpdateRequestedQty(MaterialTransferRequest row)
        {
            HashSet<BudgetKeyTuple> existing = GetExistingCostBudgets();

            foreach(PMCostBudget line in (PXSelectJoin<PMCostBudget, LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<PMCostBudget.inventoryID>>>,
                Where<InventoryItem.stkItem, Equal<True>,
                And<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                And<PMCostBudget.revisedQty, Greater<decimal0>>>>>.Select(this, row.ProjectID)))
            {

                PMCostBudget item = (PMCostBudget)line;
                var budget = PMCostBudget.PK.Find(this, item.ProjectID, item.TaskID, item.AccountGroupID,
                                                        item.CostCodeID, item.InventoryID);

                // Give Relevant Budget data & Req Nbr of MR to fetch the line
                MTRequestDetails detline = PXSelect<MTRequestDetails, Where<MTRequestDetails.reqNbr,
                                            Equal<Required<MTRequestDetails.reqNbr>>, 
                                            And<MTRequestDetails.taskID,
                                            Equal<Required<MTRequestDetails.taskID>>,
                                            And<MTRequestDetails.accountGroupID,
                                            Equal<Required<MTRequestDetails.accountGroupID>>,
                                            And<MTRequestDetails.costCode,
                                            Equal<Required<MTRequestDetails.costCode>>,
                                            And<MTRequestDetails.inventoryID,
                                            Equal<Required<MTRequestDetails.inventoryID>>>>>>>>.Select(this, row.ReqNbr, budget.TaskID, budget.AccountGroupID, budget.CostCodeID, budget.InventoryID);

                if (detline != null)
                {
                    if (!existing.Contains(BudgetKeyTuple.Create(item)))
                    {
                        RequestedProjectMaterials materials = new RequestedProjectMaterials();
                        materials.ProjectID = detline.ProjectID;
                        materials.TaskID = detline.TaskID;
                        materials.AccountGroupID = detline.AccountGroupID;
                        materials.CostCode = detline.CostCode;
                        materials.InventoryID = detline.InventoryID;
                        materials.RequestedQty = budget.RevisedQty - budget.ActualQty - detline?.RequestedQty;
                        ReqProjectMaterials.Cache.Insert(materials);
                        ReqProjectMaterials.Cache.Update(materials);
                  //      ReqProjectMaterials.Insert(materials);
                        ReqProjectMaterials.Update(ReqProjectMaterials.Current) ;
                        this.Actions.PressSave();

                    }
                }
            }

        }

    }
}
