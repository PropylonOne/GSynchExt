using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.FS;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.RQ;
using static PX.Data.PXAccess.Branch;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("Service Material Request Details")]
  public class ServiceMaterialRequestDetails : PXBqlTable, IBqlTable
  {

        public static class FK
        {
            public class MasterRec : ServiceMaterialRequest.UK.ForeignKeyOf<ServiceMaterialRequestDetails>.By<reqNbr> { }
        }
        public class PK : PrimaryKeyOf<ServiceMaterialRequestDetails>.By<reqNbr, lineNbr>
        {
            public static ServiceMaterialRequestDetails Find(PXGraph graph, string reqNbr, int? lineNbr) => FindBy(graph, reqNbr, lineNbr);
        }

        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr", Enabled = false)]
        [PXDBDefault(typeof(ServiceMaterialRequest.reqNbr), DefaultForUpdate = false)]
        [PXParent(typeof(FK.MasterRec))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr")]
        [PXLineNbr(typeof(ServiceMaterialRequest.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
 
        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        //[PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey =(typeof(InventoryItem.inventoryCD)))]
        [PMInventorySelector]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]

        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region SiteID
        [Site(allowTransit: true, DisplayName = "Warehouse", DescriptionField = typeof(INSite.descr))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
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
        [PXUIField(DisplayName = "Budgeted Qty ", Enabled = false)]
        public virtual Decimal? RevisedQty{ get; set; }
        public abstract class revisedQty: PX.Data.BQL.BqlDecimal.Field<revisedQty> { }
        #endregion

    #region ActualQty 
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Actual Qty", Enabled = false)]
        public virtual Decimal? ActualQty { get; set; }
        public abstract class actualQty : PX.Data.BQL.BqlDecimal.Field<actualQty> { }
        #endregion

    #region IssueQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Issue Qty", Enabled = false)]
        public virtual Decimal? IssueQty { get; set; }
        public abstract class issueQty : PX.Data.BQL.BqlDecimal.Field<issueQty> { }
        #endregion

    #region TransferQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Transfer Qty", Enabled = false)]
        public virtual Decimal? TransferQty { get; set; }
        public abstract class transferQty : PX.Data.BQL.BqlDecimal.Field<transferQty> { }
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