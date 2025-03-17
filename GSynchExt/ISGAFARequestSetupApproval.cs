using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using PX.Objects.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName(GSynchExt.Messages.FARequestFilter)]
    public class ISGAFARequestSetupApproval : PXBqlTable, IBqlTable, IAssignedMap
    {
        #region Keys
        public class PK : PrimaryKeyOf<ISGAFARequestSetupApproval>.By<approvalID>
        {
            public static ISGAFARequestSetupApproval Find(PXGraph graph, int? approvalID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, approvalID, options);
        }
        public static class FK
        {
            public class ApprovalMap : EPAssignmentMap.PK.ForeignKeyOf<ISGAFARequestSetupApproval>.By<assignmentMapID> { }
            public class PendingApprovalNotification : PX.SM.Notification.PK.ForeignKeyOf<ISGAFARequestSetupApproval>.By<assignmentNotificationID> { }
        }
        #endregion
        #region ApprovalID
        [PXDBIdentity(IsKey = true)]
        public virtual int? ApprovalID { get; set; }
        public abstract class approvalID : BqlInt.Field<approvalID> { }
        #endregion
        #region AssignmentMapID
        [PXDBInt]
        [PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeFARequest>>>), SubstituteKey = typeof(EPAssignmentMap.name))]
        [PXUIField(DisplayName = "Approval Map")]
        [PXDefault]
        public virtual int? AssignmentMapID { get; set; }
        public abstract class assignmentMapID : BqlInt.Field<assignmentMapID> { }
        #endregion
        #region AssignmentNotificationID
        [PXDBInt]
        [PXSelector(typeof(PX.SM.Notification.notificationID), SubstituteKey = typeof(PX.SM.Notification.name))]
        [PXUIField(DisplayName = "Pending Approval Notification")]
        public virtual int? AssignmentNotificationID { get; set; }
        public abstract class assignmentNotificationID : BqlInt.Field<assignmentNotificationID> { }
        #endregion
        #region IsActive
        [PXDBBool]
        [PXDefault(typeof(Search<GSynchExt.ISGAFARequestSetup.fARequestapprovalMap>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
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
        public static class AssignmentMapType
        {
            public class AssignmentMapTypeFARequest : PX.Data.BQL.BqlString.Constant<AssignmentMapTypeFARequest>
            {
                public AssignmentMapTypeFARequest() : base(typeof(GSynchExt.ISGAFARequest).FullName) { }
            }

        }
    }
}
