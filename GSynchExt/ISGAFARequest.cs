using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PO;
using PX.Objects.EP;
using PX.Objects.CS;
using PX.Data.BQL;
using PX.TM;
using PX.Data.EP;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName(GSynchExt.Messages.FARequests)]
    [PXPrimaryGraph(typeof(ISGAFARequestEntry))]
    public class ISGAFARequest : PXBqlTable, IBqlTable, IAssign
    {
        #region Keys
        public class PK : PrimaryKeyOf<ISGAFARequest>.By<reqNbr>
        {
            public static ISGAFARequest Find(PXGraph graph, string reqNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, reqNbr);
        }
        public static class FK
        {
        }
        #endregion
        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXSelector(typeof(Search<ISGAFARequest.reqNbr>), typeof(ISGAFARequest.reqNbr), typeof(ISGAFARequest.status))]
        [AutoNumber(typeof(ISGAFARequestSetup.fARequestNumberingID), typeof(ISGAFARequest.createdDateTime))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion
        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Line Cntr")]
        public virtual int? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion
        #region LineCntrEvent
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Line Cntr Event")]
        public virtual int? LineCntrEvent { get; set; }
        public abstract class lineCntrEvent : PX.Data.BQL.BqlInt.Field<lineCntrEvent> { }
        #endregion
        #region ReqDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Req Date", Required = true)]
        public virtual DateTime? ReqDate { get; set; }
        public abstract class reqDate : PX.Data.BQL.BqlDateTime.Field<reqDate> { }
        #endregion
        #region ReqBy
        [PXDBInt]
        [PXUIField(DisplayName = "Request By", Enabled = false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        [PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        public virtual int? ReqBy { get; set; }
        public abstract class reqBy : PX.Data.BQL.BqlInt.Field<reqBy> { }
        #endregion
        #region ExtRefNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ext Ref Nbr", Enabled = false)]
        public virtual string ExtRefNbr { get; set; }
        public abstract class extRefNbr : PX.Data.BQL.BqlString.Field<extRefNbr> { }
        #endregion
        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXStringList(
                  new string[]
                  {
                 GSynchExt.FARequestStatus.OnHold ,
                 GSynchExt.FARequestStatus.PendingApproval ,
                 GSynchExt.FARequestStatus.Open ,
                 GSynchExt.FARequestStatus.Closed ,
                 GSynchExt.FARequestStatus.Rejected ,
                  },
                  new string[]
                  {
                 GSynchExt.Messages.OnHold,
                 GSynchExt.Messages.PendingApproval,
                 GSynchExt.Messages.Open,
                 GSynchExt.Messages.Closed,
                 GSynchExt.Messages.Rejected,
                  })]
        [PXUIField(DisplayName = "Status")]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion
        #region Notifier
        [PX.TM.Owner(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Notifier")]
        public virtual int? Notifier { get; set; }
        public abstract class notifier : PX.Data.BQL.BqlInt.Field<notifier> { }
        #endregion
        #region ApprovalReqDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Approval Req Date", Enabled = false)]
        public virtual DateTime? ApprovalReqDate { get; set; }
        public abstract class approvalReqDate : PX.Data.BQL.BqlDateTime.Field<approvalReqDate> { }
        #endregion
        #region ApprovedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Approved Date", Enabled = false)]
        public virtual DateTime? ApprovedDate { get; set; }
        public abstract class approvedDate : PX.Data.BQL.BqlDateTime.Field<approvedDate> { }
        #endregion
        #region RejectedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Rejected Date", Enabled = false)]
        public virtual DateTime? RejectedDate { get; set; }
        public abstract class rejectedDate : PX.Data.BQL.BqlDateTime.Field<rejectedDate> { }
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
        #region ApprovalReqBy
        [PXDBInt]
        [PXUIField(DisplayName = "Approval Req By", Enabled = false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        public virtual int? ApprovalReqBy { get; set; }
        public abstract class approvalReqBy : PX.Data.BQL.BqlInt.Field<approvalReqBy> { }
        #endregion
        #region ApprovedBy
        [PXDBInt()]
        [PXUIField(DisplayName = "Approved By", Enabled = false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        public virtual int? ApprovedBy { get; set; }
        public abstract class approvedBy : PX.Data.BQL.BqlInt.Field<approvedBy> { }
        #endregion
        #region RejectedBy
        [PXDBInt()]
        [PXUIField(DisplayName = "Rejected By", Enabled = false)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>), SubstituteKey = typeof(EPEmployee.acctName))]
        public virtual int? RejectedBy { get; set; }
        public abstract class rejectedBy : PX.Data.BQL.BqlInt.Field<rejectedBy> { }
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
        #region IssueTo
        [PX.TM.Owner(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Issue To", Required = true)]
        public virtual int? IssueTo { get; set; }
        public abstract class issueTo : PX.Data.BQL.BqlInt.Field<issueTo> { }
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
        public virtual int? WorkgroupID { get; set; }
        #endregion
        #region OwnerID
        public abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
        protected int? _OwnerID;
        [PX.TM.Owner(typeof(ISGAFARequest.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? OwnerID { get; set; }
        #endregion
        #region ApprovalWorkgroupID
        [PXInt]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Approval Workgroup ID", Enabled = false)]
        public virtual int? ApprovalWorkgroupID { get; set; }
        #endregion
        #region ApprovalOwnerID

        [Owner(IsDBField = false, DisplayName = "Approver", Enabled = false)]
        public virtual int? ApprovalOwnerID { get; set; }
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
        [PXUIField(DisplayName = "Tstamp", Enabled = false)]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
        #region IsFullyIssued
        [PXDBBool]
        [PXDefault(false)] 
        [PXUIField(DisplayName = "Fully Issued", Enabled = false)]
        public virtual bool? IsFullyIssued { get; set; } 
        public abstract class isFullyIssued : PX.Data.BQL.BqlBool.Field<isFullyIssued> { }
        #endregion
    }
}


