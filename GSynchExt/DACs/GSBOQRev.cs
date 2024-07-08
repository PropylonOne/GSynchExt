using System;
using GSynchExt;
using PX.Data;
using PX.Data.BQL;

namespace GSynchExt
{
    [PXHidden]
    [Serializable]
    [PXCacheName("BOQ Revisions")]
    [PXPrimaryGraph(typeof(GSBOQMaintRev))]
    public partial class GSBOQRev : GSBOQ
    {
        #region BOQID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "BOQ ID")]
        public new virtual Int32? BOQID { get; set; }
        public new abstract class bOQID : PX.Data.BQL.BqlInt.Field<bOQID> { }
        #endregion

        #region RevisionID
        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Revision")]
        public new virtual string RevisionID { get; set; }
        public new abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Line Cntr")]
        public new virtual int? LineCntr { get; set; }
        public new abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion

        public new abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }

        #region EPCVendorID
        public new abstract class ePCVendorID : PX.Data.BQL.BqlString.Field<ePCVendorID> { }
        #endregion
        #region CuryID
        public new abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
        #endregion

        #region CuryInfoID
        public new abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
        #endregion

        #region Description
        public new abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
        #region Creator
        public new abstract class creator : PX.Data.BQL.BqlString.Field<creator> { }
        #endregion

        #region Approver
        public new abstract class approver : PX.Data.BQL.BqlString.Field<approver> { }
        #endregion

        #region PhaseCapacity
        public new abstract class phaseCapacity : PX.Data.BQL.BqlString.Field<phaseCapacity> { }
        #endregion
        #region StartDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Start Date")]
        public new virtual DateTime? StartDate { get; set; }
        public new abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion
        #region EndDate
        public new abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
        #endregion
        #region CapacityType1
        public new abstract class capacityType1 : PX.Data.BQL.BqlString.Field<capacityType1> { }
        #endregion
        #region CapacityType2
        public new abstract class capacityType2 : PX.Data.BQL.BqlString.Field<capacityType2> { }
        #endregion
        #region CapacityType3
        public new abstract class capacityType3 : PX.Data.BQL.BqlString.Field<capacityType3> { }
        #endregion
        #region CapacityType4
        public new abstract class capacityType4 : PX.Data.BQL.BqlString.Field<capacityType4> { }
        #endregion
        #region CapacityType5
        public new abstract class capacityType5 : PX.Data.BQL.BqlString.Field<capacityType5> { }
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
             },
             new string[]
             {
             GSynchExt.MessagesBOQ.OnHold,
             GSynchExt.MessagesBOQ.PendingApproval,
             GSynchExt.MessagesBOQ.Archived,
             GSynchExt.MessagesBOQ.Active,

         })]
        [PXUIField(DisplayName = "Status")]
        public new virtual string Status { get; set; }
        public new abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Approved
        public new abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
        #endregion
        #region Rejected
        public new abstract class rejected : BqlBool.Field<rejected> { }
        #endregion
        #region Hold
        public new abstract class hold : BqlBool.Field<hold> { }
        #endregion
        #region IsApprover
        public new abstract class isApprover : BqlDecimal.Field<isApprover> { }
        #endregion
        #region WorkgroupID
        public new abstract class workgroupID : PX.Data.BQL.BqlInt.Field<workgroupID> { }
        #endregion
        #region OwnerID
        public new abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public new virtual DateTime? CreatedDateTime { get; set; }
        public new abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID()]
        public new virtual Guid? CreatedByID { get; set; }
        public new abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion
        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public new virtual string CreatedByScreenID { get; set; }
        public new abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public new virtual DateTime? LastModifiedDateTime { get; set; }
        public new abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public new virtual Guid? LastModifiedByID { get; set; }
        public new abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public new virtual string LastModifiedByScreenID { get; set; }
        public new abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion
        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public new virtual byte[] Tstamp { get; set; }
        public new abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
        #region Noteid
        [PXNote()]
        public new virtual Guid? Noteid { get; set; }
        public new abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }
}





