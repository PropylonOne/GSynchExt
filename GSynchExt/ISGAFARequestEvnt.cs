using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.PO;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName(GSynchExt.Messages.RequestEvent)]
    public class ISGAFARequestEvnt : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<ISGAFARequestEvnt>.By<reqNbr, lineNbr>
        {
            public static ISGAFARequestEvnt Find(PXGraph graph, string reqNbr, int? lineNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, reqNbr, lineNbr);
        }
        public static class FK
        {
            public class FARequest : ISGAFARequest.PK.ForeignKeyOf<ISGAFARequest>.By<reqNbr> { }
        }
        #endregion
        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr", Enabled = false)]
        [PXDBDefault(typeof(ISGAFARequest.reqNbr))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion
        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(ISGAFARequest.lineCntrEvent))]
        [PXUIField(DisplayName = "Line Nbr", Enabled = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
        #region AssetID
        [PXDBInt]
        [PXSelector(typeof(Search<FixedAsset.assetID>), SubstituteKey = typeof(FixedAsset.assetCD))]
        [PXUIField(DisplayName = "Asset ID", Enabled = false)]
        public virtual int? AssetID { get; set; }
        public abstract class assetID : PX.Data.BQL.BqlInt.Field<assetID> { }
        #endregion
        #region Description
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
        #region ActionType
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXStringList(
                    new string[]
                    {
                GSynchExt.FARequestActionType.Issue ,
                GSynchExt.FARequestActionType.Return ,
                    },
                    new string[]
                    {
                GSynchExt.Messages.IssueActionType,
                GSynchExt.Messages.ReturnActionType,
                    })]
        [PXUIField(DisplayName = "Action Type", Enabled = false)]
        public virtual string ActionType { get; set; }
        public abstract class actionType : PX.Data.BQL.BqlString.Field<actionType> { }
        #endregion
        #region RequestQty
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Req. Qty", Enabled = false)]
        public virtual Decimal? RequestQty { get; set; }
        public abstract class requestQty : PX.Data.BQL.BqlDecimal.Field<requestQty> { }
        #endregion
        #region OpenQty
        [PXDBDecimal(2)]
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
        [PXUIField(DisplayName = "Issued Qty", Enabled = false)]
        public virtual Decimal? IssuedQty { get; set; }
        public abstract class issuedQty : PX.Data.BQL.BqlDecimal.Field<issuedQty> { }
        #endregion
        #region ReturnedQty
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Returned Qty", Enabled = false)]
        public virtual Decimal? ReturnedQty { get; set; }
        public abstract class returnedQty : PX.Data.BQL.BqlDecimal.Field<returnedQty> { }
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
    }
}