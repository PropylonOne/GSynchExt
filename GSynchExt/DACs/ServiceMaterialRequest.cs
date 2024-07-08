using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CR.Standalone;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.TM;
using static PX.Data.PXGenericInqGrph;
using static PX.Objects.IN.InventoryItem;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("Service Material Request")]
    [PXPrimaryGraph(typeof(ServiceMaterialRequestEntry))]

    public class ServiceMaterialRequest : PXBqlTable, IBqlTable
    {
        public class UK : PrimaryKeyOf<ServiceMaterialRequest>.By<reqNbr>
        {
           public static ServiceMaterialRequest Find(PXGraph graph, string reqNbr) => FindBy(graph, reqNbr);
        }

        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Line Cntr")]
        public virtual int? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion

        #region ReqNbr
        [PXDBString(10, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXSelector(typeof(Search<ServiceMaterialRequest.reqNbr>), typeof(ServiceMaterialRequest.reqNbr))]
        [AutoNumber(typeof(FundTransferRequestSetup.sMRequestNumberingID), typeof(ServiceMaterialRequest.createdDateTime))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion

        #region ReqBy
        [PXDBInt()]
        [PXUIField(DisplayName = "Request By", Enabled =false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        [PXDefault()]
        public virtual int? ReqBy { get; set; }
        public abstract class reqBy : PX.Data.BQL.BqlInt.Field<reqBy> { }
        #endregion

        #region Status
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXStringList(
              new string[]
              {
                 GSynchExt.FTRStatus.OnHold ,
                 GSynchExt.FTRStatus.Released ,
                 GSynchExt.FTRStatus.Cancelled ,
                 GSynchExt.FTRStatus.Closed ,
              },
              new string[]
              {
                 GSynchExt.Messages.OnHold,
                 GSynchExt.Messages.Released,
                 GSynchExt.Messages.Cancelled,
                 GSynchExt.Messages.Closed,
              })]
        [PXUIField(DisplayName = " Status", Enabled = false)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region TransferQty
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Transfer Qty", Enabled = false)]
        public virtual Decimal? TransferQty { get; set; }
        public abstract class transferQty : PX.Data.BQL.BqlDecimal.Field<transferQty> { }
        #endregion

        #region IssueQty
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Issue Qty", Enabled = false)]
        public virtual Decimal? IssueQty { get; set; }
        public abstract class issueQty : PX.Data.BQL.BqlDecimal.Field<issueQty> { }
        #endregion

        #region RequestQty
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Request Qty", Enabled = false)]
        public virtual Decimal? RequestQty { get; set; }
        public abstract class requestQty : PX.Data.BQL.BqlDecimal.Field<requestQty> { }
        #endregion

        #region ReqDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Request Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? ReqDate { get; set; }
        public abstract class reqDate : PX.Data.BQL.BqlDateTime.Field<reqDate> { }
        #endregion

        #region RequiredBy
        [PXDBDate()]
        [PXUIField(DisplayName = "Requested Date")]
        public virtual DateTime? RequiredBy { get; set; }
        public abstract class requiredBy : PX.Data.BQL.BqlDateTime.Field<requiredBy> { }
        #endregion

        #region ReqType
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ReqType", Enabled = false)]
        [PXStringList(
          new string[]
          {
                 GSynchExt.ReqType.FundTransfer,
                 GSynchExt.ReqType.MaterialRequest ,
                 GSynchExt.ReqType.MaterialRequestServices ,

          },
          new string[]
          {
                 GSynchExt.Messages.FundTransfer,
                 GSynchExt.Messages.MaterialRequest,
                 GSynchExt.Messages.MaterialRequestServices,

          })]
        [PXDefault(Messages.MaterialRequestServices)]
        public virtual string ReqType { get; set; }
        public abstract class reqType : PX.Data.BQL.BqlString.Field<reqType> { }
        #endregion

        #region Notify
        [PX.TM.Owner(Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Notify")]
        [PXDefault()]
        public virtual int? Notify { get; set; }
        public abstract class notify : PX.Data.BQL.BqlInt.Field<notify> { }
        #endregion

        #region Description
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        [PXDefault()]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region FromSiteID
        [Site(allowTransit: true, DisplayName = "From Warehouse", DescriptionField = typeof(INSite.descr))]
        public virtual int? FromSiteID { get; set; }
        public abstract class fromSiteID : PX.Data.BQL.BqlInt.Field<fromSiteID> { }
        #endregion

        #region ToSiteID
        [Site(allowTransit: true, DisplayName = "From Warehouse", DescriptionField = typeof(INSite.descr))]

      //  [PXRestrictor(typeof(Where<INSite.siteCD, Contains<containsSITE>>), "")]
        [PXUIField(DisplayName = "To")]
        public virtual int? ToSiteID { get; set; }
        public abstract class toSiteID : PX.Data.BQL.BqlInt.Field<toSiteID> { }
        #endregion

        #region CreatedDateTime
            [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
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

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }

    #region Dialogs
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [PXHidden]
    public class CopyDialogInfo2 : PXBqlTable, IBqlTable
    {

        #region FromSiteID
        [Site(allowTransit: true, DisplayName = "From Warehouse", DescriptionField = typeof(INSite.descr))]
        public virtual int? FromSiteID { get; set; }
        public abstract class fromSiteID : PX.Data.BQL.BqlInt.Field<fromSiteID> { }
        #endregion

        #region ToLocationID
        public abstract class toLocationID : PX.Data.BQL.BqlInt.Field<toLocationID> { }
        [PXSelector(typeof(Search<INLocation.locationID, Where<INLocation.siteID, Equal<Current<ServiceMaterialRequest.toSiteID>>>>), SubstituteKey = typeof(INLocation.locationCD))]
        //     [PXRestrictor(typeof(Where<INLocation.siteID, Equal<Current<MaterialTransferRequest.toSiteID>>>), "")]
        [PXUIField(DisplayName = "To Location ID")]
        public virtual Int32? ToLocationID
        {
            get;
            set;
        }
        #endregion

        #region LocationID
        public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
        [PXSelector(typeof(Search<INLocation.locationID, Where<INLocation.siteID, Equal<Current<CopyDialogInfo2.fromSiteID>>>>), SubstituteKey = typeof(INLocation.locationCD))]
        [PXUIField(DisplayName = "From Location ID")]
        public virtual Int32? LocationID
        {
            get;
            set;
        }
        #endregion

        #region ToCostLayerType
        public abstract class toCostLayerType : PX.Data.BQL.BqlString.Field<toCostLayerType> { }
        [PXDBString(1, IsFixed = true)]
        [PXDefault(PX.Objects.IN.CostLayerType.Normal)]
        [PXUIField(DisplayName = "To Cost Layer Type", FieldClass = FeaturesSet.inventory.CostLayerType)]
        [CostLayerType.List]
        public virtual string ToCostLayerType
        {
            get;
            set;
        }
        #endregion

        #region CostLayerType
        public abstract class costLayerType : PX.Data.BQL.BqlString.Field<costLayerType> { }
        [PXDBString(1, IsFixed = true)]
        [PXDefault(PX.Objects.IN.CostLayerType.Normal)]
        [PXUIField(DisplayName = "From Cost Layer Type", FieldClass = FeaturesSet.inventory.CostLayerType)]
        [CostLayerType.List]
        public virtual string CostLayerType
        {
            get;
            set;
        }
        #endregion

    }
    #endregion
    public class containsSITE : PX.Data.BQL.BqlString.Constant<containsSITE> { public containsSITE() : base("SITE") { } }

}