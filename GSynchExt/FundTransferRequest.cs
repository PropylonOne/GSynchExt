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
using PX.TM;
using static PX.Objects.IN.InventoryItem;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("Fund Transfer Request")]
    [PXPrimaryGraph(typeof(FundTransferRequestEntry))]
    public class FundTransferRequest : PXBqlTable, IBqlTable , IAssign
  {
        public class UK : PrimaryKeyOf<FundTransferRequest>.By<reqNbr>
        {
            public static FundTransferRequest Find(PXGraph graph, string reqNbr) => FindBy(graph, reqNbr);
        }



         
    #region ReqNbr
    [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Req Nbr")]
    [PXSelector(typeof(Search<FundTransferRequest.reqNbr>), typeof(FundTransferRequest.reqNbr), typeof(FundTransferRequest.description), typeof(FundTransferRequest.status))]

    [AutoNumber(typeof(FundTransferRequestSetup.fTRequestNumberingID), typeof(FundTransferRequest.createdDateTime))]
    public virtual string ReqNbr { get; set; }
    public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
    #endregion

    #region ReqBy
    [PXDBInt()]
    [PXUIField(DisplayName = "Requested By", Enabled = false)]
    [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
    public virtual int? ReqBy { get; set; }
    public abstract class reqBy : PX.Data.BQL.BqlInt.Field<reqBy> { }
        #endregion
        
    #region Status
       [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXStringList(
             new string[]
             {
             GSynchExt.FTRStatus.OnHold ,
             GSynchExt.FTRStatus.PendingApproval ,
             GSynchExt.FTRStatus.Archived ,
             GSynchExt.FTRStatus.Released ,
             GSynchExt.FTRStatus.Rejected ,
             GSynchExt.FTRStatus.Closed ,
             },
             new string[]
             {
             GSynchExt.Messages.OnHold,
             GSynchExt.Messages.PendingApproval,
             GSynchExt.Messages.Archived,
             GSynchExt.Messages.Released,
             GSynchExt.Messages.Rejected,
             GSynchExt.Messages.Closed,

         })]
        [PXUIField(DisplayName = " Status", Enabled = false)]
       public virtual string Status { get; set; }
       public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
    #endregion

    #region ReqDate
    [PXDBDate()]
    [PXUIField(DisplayName = "Requested Date")]
    [PXDefault(typeof(AccessInfo.businessDate))]
    public virtual DateTime? ReqDate { get; set; }
    public abstract class reqDate : PX.Data.BQL.BqlDateTime.Field<reqDate> { }
    #endregion

    #region ReqType
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "ReqType")]
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
        [PXDefault(Messages.FundTransfer)]
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

    #region Amount
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Amount")]
    public virtual Decimal? Amount { get; set; }
    public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
    #endregion

    #region OpenBalance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Open Balance")]
        public virtual Decimal? OpenBalance { get; set; }
        public abstract class openBalance : PX.Data.BQL.BqlDecimal.Field<openBalance> { }
        #endregion

    #region CashAccntID
        [PXUIField(DisplayName = "Petty Cash Account ID")]
    [CashAccount(typeof(ARPayment.branchID), typeof(Search<CashAccount.cashAccountID>), Visibility = PXUIVisibility.Visible)]
//    [PXRestrictor(typeof(Where<CashAccount.descr.Contains<pettyCash>>), "Invalid Cash Account")]
    public virtual int? CashAccntID { get; set; }
    public abstract class cashAccntID : PX.Data.BQL.BqlInt.Field<cashAccntID> { }
    #endregion

    #region Approved
        [PXDBBool()]
        [PXUIField(DisplayName = "Approved", Enabled = false)]
        [PXDefault(false)]
        public virtual bool? Approved { get; set; }
        public abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
        #endregion

    #region Rejected
        public abstract class rejected : PX.Data.BQL.BqlBool.Field<rejected> { }
        protected bool? _Rejected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Reject", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public bool? Rejected
        {
            get
            {
                return _Rejected;
            }
            set
            {
                _Rejected = value;
            }
        }
        #endregion
    
    #region Hold
        public abstract class hold : BqlBool.Field<hold> { }
        [PXDBBool]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        #endregion

    #region Transferred
        public abstract class transferred : BqlBool.Field<transferred> { }
        [PXDBBool]
        [PXUIField(DisplayName = "Transferred")]
        [PXDefault(false , PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? Transferred { get; set; }
        #endregion

    #region Requested
        public abstract class requested : BqlBool.Field<requested> { }
        [PXDBBool]
        [PXUIField(DisplayName = "Requested")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? Requested { get; set; }
        #endregion

    #region IsApprover
        public abstract class isApprover : BqlDecimal.Field<isApprover>
        {
        }
        [PXBool]
        public virtual bool? IsApprover { get; set; }
        #endregion

    #region WorkgroupID
        public abstract class workgroupID : PX.Data.BQL.BqlInt.Field<workgroupID> { }
        protected int? _WorkgroupID;
        [PXDBInt]
        [PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSubordinateGroupSelectorAttribute]
        public virtual int? WorkgroupID
        {
            get;
            set;
        }
        #endregion

    #region OwnerID
        public abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
        protected int? _OwnerID;
        [PX.TM.Owner(typeof(FundTransferRequest.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? OwnerID
        { get; set; }
        #endregion

    #region ApprovalWorkgroupID
        [PXInt]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Approval Workgroup ID", Enabled = false)]
        public virtual int? ApprovalWorkgroupID
        {
            get;
            set;
        }
        #endregion

    #region ApprovalOwnerID

        [Owner(IsDBField = false, DisplayName = "Approver", Enabled = false)]
        public virtual int? ApprovalOwnerID
        {
            get;
            set;
        }
        #endregion


    #region IAssign Members

        int? PX.Data.EP.IAssign.WorkgroupID
        {
            get { return WorkgroupID; }
            set { WorkgroupID = value; }
        }

        int? PX.Data.EP.IAssign.OwnerID
        {
            get { return OwnerID; }
            set { OwnerID = value; }
        }

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
    public class pettyCash : PX.Data.BQL.BqlString.Constant<pettyCash> { public pettyCash() : base("Petty Cash") { } }

}