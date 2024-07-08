using System;
using System.Collections;
using PX.Data;
using PX.Data.Licensing;
using PX.Objects.CA;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.TM;
using PX.Objects.CN.Compliance.PM.GraphExtensions;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.FA.FABookSettings.midMonthType;
using static PX.Objects.TX.CSTaxCalcType;
using static PX.Objects.RQ.RQRequisitionContent.FK;
using PX.Objects.FS;
using PX.Objects.EP;
using PX.Objects.CS;

namespace GSynchExt
{
    public class ChangeOrderEntryGSExt : PXGraphExtension<ChangeOrderEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        #region Cached Extentions
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBInt]
        [PXSelector(typeof(Search5<EPCompanyTree.workGroupID,
        InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>>,
        InnerJoin<EPEmployee, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>>,
        Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>,
        Aggregate<GroupBy<EPCompanyTree.workGroupID, GroupBy<EPCompanyTree.description>>>>),
        SubstituteKey = typeof(EPCompanyTree.description))]
        [PXDefault(typeof(Search5<EPCompanyTree.workGroupID,
        InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>>,
        InnerJoin<EPEmployee, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>>,
        Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>,
        Aggregate<GroupBy<EPCompanyTree.workGroupID, GroupBy<EPCompanyTree.description>>>>), PersistingCheck = PXPersistingCheck.Nothing )]
        [PXUIField(DisplayName = "Workgroup ID", Enabled = false)]
        public virtual void _(Events.CacheAttached<PMChangeOrder.workgroupID> e) { }

        #endregion

        #region Action Overrides

        [PXOverride]
        public delegate IEnumerable ReverseDelegate(PXAdapter adapter);

        [PXUIField(DisplayName = PX.Objects.PM.Messages.Reverse)]
        [PXProcessButton]
        public IEnumerable Reverse(PXAdapter adapter, ReverseDelegate baseMethod)
        {

            foreach (PMChangeOrderCostBudget cOCostBudgetLine in this.Base.CostBudget.Select())
            {
                var interimMatRecord = RequestedProjectMaterials.PK.Find(this.Base, cOCostBudgetLine.ProjectID, cOCostBudgetLine.TaskID, cOCostBudgetLine.AccountGroupID, cOCostBudgetLine.CostCodeID, cOCostBudgetLine.InventoryID);
                var pmBudget = PMCostBudget.PK.Find(this.Base, cOCostBudgetLine.ProjectID, cOCostBudgetLine.TaskID, cOCostBudgetLine.AccountGroupID, cOCostBudgetLine.CostCodeID, cOCostBudgetLine.InventoryID);

                if (pmBudget == null && interimMatRecord == null)
                {
                    continue;
                }

                decimal orgQty = (pmBudget?.RevisedQty ?? 0) - (cOCostBudgetLine.Qty ?? 0);

                if ((interimMatRecord?.RequestedQty ?? 0) > orgQty)
                {
                    throw new PXException(Messages.ChangeOrderError1);
                }
                else 
                {
                    MTRequestDetails matDet =
                        PXSelectJoinGroupBy<MTRequestDetails,
                        InnerJoin<MaterialTransferRequest, On<MaterialTransferRequest.reqNbr, Equal<MTRequestDetails.reqNbr>>>,
                        Where<MaterialTransferRequest.status, Equal<Required<MaterialTransferRequest.status>>,
                        And<MTRequestDetails.projectID, Equal<Required<MTRequestDetails.projectID>>,
                        And<MTRequestDetails.taskID, Equal<Required<MTRequestDetails.taskID>>,
                        And<MTRequestDetails.accountGroupID, Equal<Required<MTRequestDetails.accountGroupID>>,
                        And<MTRequestDetails.costCode, Equal<Required<MTRequestDetails.costCode>>,
                        And<MTRequestDetails.inventoryID, Equal<Required<MTRequestDetails.inventoryID>>>>>>>>,
                        Aggregate<GroupBy<MTRequestDetails.projectID,
                                        GroupBy<MTRequestDetails.taskID,
                                        GroupBy<MTRequestDetails.costCode,
                                        GroupBy<MTRequestDetails.inventoryID,
                                        GroupBy<MTRequestDetails.accountGroupID, Sum<MTRequestDetails.requestedQty>>>>>>>>.Select(this.Base, FTRStatus.OnHold, cOCostBudgetLine.ProjectID, cOCostBudgetLine.TaskID, cOCostBudgetLine.AccountGroupID, cOCostBudgetLine.CostCodeID, cOCostBudgetLine.InventoryID);

                    if ((matDet?.RequestedQty ?? 0) > orgQty)
                    {
                        throw new PXException(GSynchExt.Messages.ChangeOrderError2);
                    }
                    if (matDet == null)
                    {
                        continue;
                    }
                }
            }
            this.Base.ReverseDocument();
            return new PMChangeOrder[] { this.Base.Document.Current };
        }

        #endregion

    }
}
