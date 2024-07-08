using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PM;
using PX.Objects.IN;

namespace GSynchExt
{
    public class RequestedProjectMaterials : PXBqlTable, IBqlTable
    {

        public static class FK
        {
            public class CostCode : PMCostCode.PK.ForeignKeyOf<MTRequestDetails>.By<costCode> { }

        }
        //
        // Summary:
        //     Primary Key
        public new class PK : PrimaryKeyOf<RequestedProjectMaterials>.By<projectID, taskID, accountGroupID, costCode, inventoryID>
        {
            public static RequestedProjectMaterials Find(PXGraph graph, int? projectID, int? taskID, int? accountGroupID, int? costCode, int? inventoryID)
            {
                return PrimaryKeyOf<RequestedProjectMaterials>.By<RequestedProjectMaterials.projectID, RequestedProjectMaterials.taskID, RequestedProjectMaterials.accountGroupID, RequestedProjectMaterials.costCode, RequestedProjectMaterials.inventoryID>.FindBy(graph, projectID, taskID, accountGroupID, costCode, inventoryID);
            }
        }
        public new class MK : PrimaryKeyOf<RequestedProjectMaterials>.By<projectID, taskID, costCode, inventoryID>
        {
            public static RequestedProjectMaterials Find(PXGraph graph, int? projectID, int? taskID,  int? costCode, int? inventoryID)
            {
                return PrimaryKeyOf<RequestedProjectMaterials>.By<RequestedProjectMaterials.projectID, RequestedProjectMaterials.taskID,  RequestedProjectMaterials.costCode, RequestedProjectMaterials.inventoryID>.FindBy(graph, projectID, taskID,costCode, inventoryID);
            }
        }


        #region ProjectID
        [PXUIField(DisplayName = "Project ID", Enabled = false)]
        [PXSelector(typeof(Search<PMProject.contractID>), typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status), SubstituteKey = (typeof(PMProject.contractCD)))]
        [PXDBInt(IsKey =true)]
        [PXDefault]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region TaskID
        [ProjectTask(typeof(RequestedProjectMaterials.projectID), IsKey = true)]
        [PXUIField(DisplayName = "Project Task", Enabled = false)]
        public virtual int? TaskID { get; set; }
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
        #endregion

        #region AccountGroupID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Account Group", Enabled = false)]
        //     [PXSelector(typeof(Search<PMTask.AccountGroupID>), SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? AccountGroupID { get; set; }
        public abstract class accountGroupID : PX.Data.BQL.BqlInt.Field<accountGroupID> { }
        #endregion

        #region CostCode
        [PXUIField(DisplayName = "Cost Code", Enabled = false)]
        [CostCode(null, null, null, DescriptionField = typeof(PMCostCode.description), IsKey = true)]
        [PXForeignReference(typeof(FK.CostCode))]
        public virtual int? CostCode { get; set; }
        public abstract class costCode : PX.Data.BQL.BqlInt.Field<costCode> { }
        #endregion

        #region InventoryID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        //  [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey =(typeof(InventoryItem.inventoryCD)))]
        [PMInventorySelector]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]

        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region RequestedQty
        [PXDBDecimal]
        [PXUIField(DisplayName = "Request Qty", Enabled = false)]
        public virtual Decimal? RequestedQty { get; set; }
        public abstract class requestedQty : PX.Data.BQL.BqlDecimal.Field<requestedQty> { }
        #endregion

    }
}
