using System;
using GSynchExt.Descriptor;
using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.Licensing;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR.Extensions;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.TM;
using static PX.Data.PXGenericInqGrph;
using static PX.SM.AUStepField;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("Bill of Quantities")]
    [PXPrimaryGraph(typeof(GSBOQMaint))]
    public class GSBOQ : PXBqlTable, IBqlTable, IAssign
    {

        #region Keys
        public class UK : PrimaryKeyOf<GSBOQ>.By<bOQID, revisionID>
        {
            public static GSBOQ Find(PXGraph graph, int? bOQID, string revisionID) => FindBy(graph, bOQID, revisionID);
        }
        #endregion

        #region BOQID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Project")]
        [PXDefault()]
        [PXReferentialIntegrityCheck]
        public virtual Int32? BOQID { get; set; }
        public abstract class bOQID : PX.Data.BQL.BqlInt.Field<bOQID> { }
        #endregion

        #region RevisionID
        [RevisionIDField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        [PXDefault(typeof(GSBOQSetup.revisionID))]
        [PXSelector(typeof(Search<GSBOQ.revisionID,
            Where<GSBOQ.bOQID, Equal<Current<GSBOQ.bOQID>>>,
            OrderBy<Desc<GSBOQ.revisionID>>>),
            typeof(GSBOQ.revisionID),
            typeof(GSBOQ.status),
            typeof(GSBOQ.description),
            typeof(GSBOQ.startDate),
            typeof(GSBOQ.endDate))]
        /*[PXSelector(typeof(Search<GSBOQ.revisionID,
            Where<GSBOQ.bOQID, Equal<Optional<GSBOQ.bOQID>>,
                Or<GSBOQ.bOQID, Equal<Current<GSBOQ.bOQID>>>>>),
            typeof(GSBOQ.revisionID),
            typeof(GSBOQ.status),
            typeof(GSBOQ.description),
            typeof(GSBOQ.startDate),
            typeof(GSBOQ.endDate))]*/
        public virtual string RevisionID { get; set; }
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Line Cntr")]
        public virtual int? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion

        #region TaskID
        [PXDBInt]
        [PXUIField(DisplayName = "Project Task", Visible = false)]
        public virtual int? TaskID { get; set; }
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
        #endregion

        #region EPCVendorID
        // [PXSelector(typeof(Search<PX.Objects.EP.EPEmployee.acctCD>), SubstituteKey = typeof(PX.Objects.EP.EPEmployee.acctName))]

        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "EPC Vendor")]
        public virtual Int32? EPCVendorID { get; set; }
        public abstract class ePCVendorID : PX.Data.BQL.BqlString.Field<ePCVendorID> { }

        #endregion
        #region CuryID
        public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
        protected String _CuryID;
        [PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
        [PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXDefault(typeof(Current<AccessInfo.baseCuryID>))]
        [PXSelector(typeof(Currency.curyID))]
        public virtual String CuryID
        {
            get
            {
                return this._CuryID;
            }
            set
            {
                this._CuryID = value;
            }
        }
        #endregion

        #region CuryInfoID
        public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
        protected Int64? _CuryInfoID;
        [PXDBLong]
        [CurrencyInfo]
        public virtual Int64? CuryInfoID
        {
            get
            {
                return this._CuryInfoID;
            }
            set
            {
                this._CuryInfoID = value;
            }
        }
        #endregion

        #region Description
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Creator
        [PXUIField(DisplayName = "Creator")]
        [PXSelector(typeof(Search<PX.Objects.EP.EPEmployee.acctCD>), SubstituteKey = typeof(PX.Objects.EP.EPEmployee.acctName))]
        [PXDBString]
        public virtual string Creator { get; set; }
        public abstract class creator : PX.Data.BQL.BqlString.Field<creator> { }

        #endregion

        #region Approver
        [PXUIField(DisplayName = "Approver")]
        [PXSelector(typeof(Search<PX.Objects.EP.EPEmployee.acctCD>), SubstituteKey = typeof(PX.Objects.EP.EPEmployee.acctName))]
        [PXDBString]
        public virtual string Approver { get; set; }
        public abstract class approver : PX.Data.BQL.BqlString.Field<approver> { }
        #endregion

        #region PhaseCapacity
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Phase Capacity ")]
        public virtual string PhaseCapacity { get; set; }
        public abstract class phaseCapacity : PX.Data.BQL.BqlString.Field<phaseCapacity> { }
        #endregion
        #region StartDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Start Date")]
        public virtual DateTime? StartDate { get; set; }
        public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion
        #region EndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "End Date", Enabled = false)]
        public virtual DateTime? EndDate { get; set; }
        public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
        #endregion
        #region CapacityType1
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Capacity Type 1 ")]
        public virtual string CapacityType1 { get; set; }
        public abstract class capacityType1 : PX.Data.BQL.BqlString.Field<capacityType1> { }
        #endregion
        #region CapacityType2
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Capacity Type 2 ")]
        public virtual string CapacityType2 { get; set; }
        public abstract class capacityType2 : PX.Data.BQL.BqlString.Field<capacityType2> { }
        #endregion
        #region CapacityType3
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Capacity Type 3 ")]
        public virtual string CapacityType3 { get; set; }
        public abstract class capacityType3 : PX.Data.BQL.BqlString.Field<capacityType3> { }
        #endregion
        #region CapacityType4
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Capacity Type 4 ")]
        public virtual string CapacityType4 { get; set; }
        public abstract class capacityType4 : PX.Data.BQL.BqlString.Field<capacityType4> { }
        #endregion
        #region CapacityType5
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Capacity Type 5 ")]
        public virtual string CapacityType5 { get; set; }
        public abstract class capacityType5 : PX.Data.BQL.BqlString.Field<capacityType5> { }
        #endregion

        #region Status
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXDefault(MessagesBOQ.OnHold)]
        [PXStringList(
             new string[]
             {
             GSynchExt.BOQStatus.OnHold ,
             GSynchExt.BOQStatus.PendingApproval ,
             GSynchExt.BOQStatus.Archived ,
             GSynchExt.BOQStatus.Active ,
             GSynchExt.BOQStatus.Rejected ,
             },
             new string[]
             {
             GSynchExt.MessagesBOQ.OnHold,
             GSynchExt.MessagesBOQ.PendingApproval,
             GSynchExt.MessagesBOQ.Archived,
             GSynchExt.MessagesBOQ.Active,
             GSynchExt.MessagesBOQ.Rejected,

         })]
        [PXUIField(DisplayName = "Status")]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
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
        public virtual int? WorkgroupID
        {
            get;
            set;
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
        protected int? _OwnerID;
        [PX.TM.Owner(typeof(GSBOQ.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? OwnerID
        { get; set; }
        #endregion


        [PXInt]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Approval Workgroup ID", Enabled = false)]
        public virtual int? ApprovalWorkgroupID
        {
            get;
            set;
        }

        [Owner(IsDBField = false, DisplayName = "Approver", Enabled = false)]
        public virtual int? ApprovalOwnerID
        {
            get;
            set;
        }

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
    [PXHidden]
    [Serializable]
    [PXProjection(typeof(Select4<GSBOQ,
    Aggregate<
        GroupBy<GSBOQ.bOQID>>>), Persistent = false)]
    public class GSBOQAggregate : PXBqlTable, IBqlTable
    {
        #region BOQID
        public abstract class bOQID : PX.Data.BQL.BqlInt.Field<bOQID> { }
        protected Int32? _BOQID;
        [BOQID(IsKey = true, BqlField = typeof(GSBOQ.bOQID))]
        public virtual int? BOQID
        {
            get
            {
                return this._BOQID;
            }
            set
            {
                this._BOQID = value;
            }
        }
        #endregion
        #region RevisionID
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        protected String _RevisionID;
        [RevisionIDField(IsKey = true, BqlField = typeof(GSBOQ.revisionID))]
        public virtual String RevisionID
        {
            get
            {
                return this._RevisionID;
            }
            set
            {
                this._RevisionID = value;
            }
        }
        #endregion
    }
}
namespace GSynchExt.Standalone
{
    [PXHidden]
    [Serializable]
    public partial class GSBOQ : GSynchExt.GSBOQ
    {

        public new abstract class boqID : PX.Data.BQL.BqlInt.Field<boqID> { }
        public new abstract class revisionID : PX.Data.BQL.BqlDateTime.Field<revisionID> { }
        public new abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        public new abstract class description : PX.Data.BQL.BqlString.Field<description> { }
    }
}




