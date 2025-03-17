using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.PO;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName(GSynchExt.Messages.ISGAFARequestLine)]
    public class ISGAFARequestLine : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<ISGAFARequestLine>.By<reqNbr, assetID>
        {
            public static ISGAFARequestLine Find(PXGraph graph, string reqNbr, int? assetID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, reqNbr, assetID);
        }
        public static class FK
        {
            public class FARequest : ISGAFARequest.PK.ForeignKeyOf<ISGAFARequest>.By<reqNbr> { }
        }
        #endregion
        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXDBDefault(typeof(ISGAFARequest.reqNbr), DefaultForUpdate = false)]
        [PXParent(typeof(FK.FARequest))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion
        #region LineNbr
        [PXDBInt()]
        [PXLineNbr(typeof(ISGAFARequest.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
        #region AssetID
        [PXDBInt(IsKey = true)]
        [PXSelector(typeof(Search<FixedAsset.assetID>), SubstituteKey = typeof(FixedAsset.assetCD),DescriptionField = typeof(FixedAsset.description))]
        [PXUIField(DisplayName = "Asset ID")]
        public virtual int? AssetID { get; set; }
        public abstract class assetID : PX.Data.BQL.BqlInt.Field<assetID> { }
        #endregion
        #region UnAssignedQty
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Un Assigned Qty")]
        [PXDependsOnFields(typeof(requestQty), typeof(returnedQty), typeof(issuedQty))]
        [PXFormula(typeof(Sub<Add<requestQty, returnedQty>, issuedQty>))]
        public virtual Decimal? UnAssignedQty { get; set; }
        public abstract class unAssignedQty : PX.Data.BQL.BqlDecimal.Field<unAssignedQty> { }
        #endregion
        #region RequestQty
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Req. Qty")]
        [PXDefault(typeof(Search<FixedAsset.qty, Where<FixedAsset.assetID, Equal<Current<assetID>>>>))]

        public virtual Decimal? RequestQty { get; set; }
        public abstract class requestQty : PX.Data.BQL.BqlDecimal.Field<requestQty> { }
        #endregion
        #region OpenQty
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "Open Qty", Enabled = false)]
        public virtual Decimal? OpenQty { get; set; }
        public abstract class openQty : PX.Data.BQL.BqlDecimal.Field<openQty> { }
        #endregion
        #region FAQty
        [PXDecimal(2)]
        [PXUIField(DisplayName = "FA Qty", Enabled = false)]
        public virtual Decimal? FAQty { get; set; }
        public abstract class fAQty : PX.Data.BQL.BqlDecimal.Field<fAQty> { }
        #endregion
        #region IssuedQty
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "Issued Qty", Enabled = false)]
        public virtual Decimal? IssuedQty { get; set; }
        public abstract class issuedQty : PX.Data.BQL.BqlDecimal.Field<issuedQty> { }
        #endregion
        #region ReturnedQty
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "Returned Qty", Enabled = false)]
        public virtual Decimal? ReturnedQty { get; set; }
        public abstract class returnedQty : PX.Data.BQL.BqlDecimal.Field<returnedQty> { }
        #endregion
        #region Description
        [PXString(255, IsUnicode = true, InputMask = "")]
        //[PXDependsOnFields(typeof(assetID))]
        //[PXUnboundDefault(typeof(Search<FixedAsset.description, Where<FixedAsset.assetID, Equal<Current<assetID>>>>))]
        //[PXFormula(typeof(Search<FixedAsset.description, Where<FixedAsset.assetID, Equal<Current<assetID>>>>))]
        [PXUIField(DisplayName = "Description", Enabled = false)]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
        #region IssuedBy
        [PXDBInt()]
        [PXUIField(DisplayName = "Issued By", Enabled = false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        public virtual int? IssuedBy { get; set; }
        public abstract class issuedBy : PX.Data.BQL.BqlInt.Field<issuedBy> { }
        #endregion
        #region ReturnedBy
        [PXDBInt()]
        [PXUIField(DisplayName = "Returned By", Enabled = false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        public virtual int? ReturnedBy { get; set; }
        public abstract class returnedBy : PX.Data.BQL.BqlInt.Field<returnedBy> { }
        #endregion
        #region IssuedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Issued Date", Enabled = false)]
        public virtual DateTime? IssuedDate { get; set; }
        public abstract class issuedDate : PX.Data.BQL.BqlDateTime.Field<issuedDate> { }
        #endregion
        #region ReturnedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Returned Date", Enabled = false)]
        public virtual DateTime? ReturnedDate { get; set; }
        public abstract class returnedDate : PX.Data.BQL.BqlDateTime.Field<returnedDate> { }
        #endregion
        #region Completed
        [PXDBBool]
        [PXUIField(DisplayName = "Completed", Visibility = PXUIVisibility.Visible, Enabled = true)]
        [PXDefault(false)]
        public virtual bool? Completed { get; set; }
        public abstract class completed : BqlBool.Field<completed> { }
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
}