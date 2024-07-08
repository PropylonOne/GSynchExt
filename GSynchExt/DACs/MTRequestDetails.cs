using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.PM;
using static PX.Data.PXAccess.Branch;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("MTRequestDetails")]
  public class MTRequestDetails : PXBqlTable, IBqlTable
  {
        public class PK : PrimaryKeyOf<MTRequestDetails>.By<reqNbr, projectID, taskID, costCode, inventoryID>
        {
            public static MTRequestDetails Find(PXGraph graph, string reqNbr, int? projectID, int? taskID, int? costCode, int? inventoryID) => FindBy(graph, reqNbr, projectID, taskID, costCode, inventoryID);
        }
        public static class FK
        {
            public class MasterRec : MaterialTransferRequest.UK.ForeignKeyOf<MTRequestDetails>.By<reqNbr> { }
            public class CostCode : PMCostCode.PK.ForeignKeyOf<MTRequestDetails>.By<costCode> { }

        }

        #region Selected

        [PXBool]
      //  [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr", Enabled = false)]
        [PXDBDefault(typeof(MaterialTransferRequest.reqNbr), DefaultForUpdate = false)]
        [PXParent(typeof(FK.MasterRec))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion

    #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr")]
        [PXLineNbr(typeof(MaterialTransferRequest.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

    #region ProjectID
        [PXUIField(DisplayName = "Project ID", Enabled = false)]
        [PXSelector(typeof(Search<PMProject.contractID>), typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status), SubstituteKey = (typeof(PMProject.contractCD)))]
        [PXDBInt]
        [PXDefault]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

    #region TaskID
        [ProjectTask(typeof(MTRequestDetails.projectID))] 
        [PXUIField(DisplayName = "Project Task", Enabled = false)]
        public virtual int? TaskID { get; set; }
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
        #endregion

    #region AccountGroupID
        [PXDBInt]
        [PXUIField(DisplayName = "Account Group", Enabled = false)]
   //     [PXSelector(typeof(Search<PMTask.AccountGroupID>), SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? AccountGroupID { get; set; }
        public abstract class accountGroupID : PX.Data.BQL.BqlInt.Field<accountGroupID> { }
        #endregion

    #region CostCode
        [PXUIField(DisplayName = "Cost Code", Enabled = false)]     
        [CostCode(null, null, null, DescriptionField = typeof(PMCostCode.description))]
        [PXForeignReference(typeof(FK.CostCode))]
        public virtual int? CostCode { get; set; }
        public abstract class costCode : PX.Data.BQL.BqlInt.Field<costCode> { }
        #endregion

    #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        //  [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey =(typeof(InventoryItem.inventoryCD)))]
        [PMInventorySelector]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]

        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

    #region UoM
        [INUnit(DisplayName = "UoM", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual string UoM { get; set; }
        public abstract class uoM : PX.Data.BQL.BqlString.Field<uoM> { }
        #endregion

    #region RequestedQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Request Qty", Enabled = true)]
        public virtual Decimal? RequestedQty { get; set; }
        public abstract class requestedQty : PX.Data.BQL.BqlDecimal.Field<requestedQty> { }
   #endregion

    #region RevisedQty 
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Budgeted Qty", Enabled = false)]
        public virtual Decimal? RevisedQty{ get; set; }
        public abstract class revisedQty: PX.Data.BQL.BqlDecimal.Field<revisedQty> { }
        #endregion

    #region ActualQty 
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Actual Qty (Used in Project)", Enabled = false, Visible = false)]
        public virtual Decimal? ActualQty { get; set; }
        public abstract class actualQty : PX.Data.BQL.BqlDecimal.Field<actualQty> { }
        #endregion

    #region IssueQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Issue Qty", Enabled = false)]
        public virtual Decimal? IssueQty { get; set; }
        public abstract class issueQty : PX.Data.BQL.BqlDecimal.Field<issueQty> { }
        #endregion

        #region IssueQtyReleased
        [PXDecimal()]
        [PXUIField(DisplayName = "Issue Qty (Released)", Enabled = false)]
        public virtual Decimal? IssueQtyReleased { get; set; }
        public abstract class issueQtyReleased : PX.Data.BQL.BqlDecimal.Field<issueQtyReleased> { }
        #endregion



        #region IssueQtyUnReleased
        [PXDecimal()]
        [PXUIField(DisplayName = "Issue Qty (Unreleased)", Enabled = false)]
        public virtual Decimal? IssueQtyUnReleased { get; set; }
        public abstract class issueQtyUnReleased : PX.Data.BQL.BqlDecimal.Field<issueQtyUnReleased> { }
        #endregion

        #region TransferQtyUnReleased
        [PXDecimal()]
        [PXUIField(DisplayName = "Transfer Qty (Unreleased)", Enabled = false)]
        public virtual Decimal? TransferQtyUnReleased { get; set; }
        public abstract class transferQtyUnReleased : PX.Data.BQL.BqlDecimal.Field<transferQtyUnReleased> { }
        #endregion


        #region TransferQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Transfer Qty", Enabled = false)]
        public virtual Decimal? TransferQty { get; set; }
        public abstract class transferQty : PX.Data.BQL.BqlDecimal.Field<transferQty> { }
        #endregion

        #region TransferQtyReleased
        [PXDecimal()]
        [PXUIField(DisplayName = "Transfer Qty (Released)", Enabled = false)]
        public virtual Decimal? TransferQtyReleased { get; set; }
        public abstract class transferQtyReleased : PX.Data.BQL.BqlDecimal.Field<transferQtyReleased> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

    #region Tstamp
        [PXDBTimestamp()]
    [PXUIField(DisplayName = "Tstamp")]
    public virtual byte[] Tstamp { get; set; }
    public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
    #endregion

    #region CreatedByID
    [PXDBCreatedByID()]
    public virtual Guid? CreatedByID { get; set; }
    public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
    #endregion

    #region CreatedByScreenID
    [PXDBCreatedByScreenID()]
    public virtual string CreatedByScreenID { get; set; }
    public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
    #endregion

    #region CreatedDateTime
    [PXDBCreatedDateTime()]
    public virtual DateTime? CreatedDateTime { get; set; }
    public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
    #endregion

    #region LastModifiedByID
    [PXDBLastModifiedByID()]
    public virtual Guid? LastModifiedByID { get; set; }
    public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
    #endregion

    #region LastModifiedByScreenID
    [PXDBLastModifiedByScreenID()]
    public virtual string LastModifiedByScreenID { get; set; }
    public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
    #endregion

    #region LastModifiedDateTime
    [PXDBLastModifiedDateTime()]
    public virtual DateTime? LastModifiedDateTime { get; set; }
    public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
    #endregion
  }
}