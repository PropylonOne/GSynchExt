using PX.Data;
using PX.Data.BQL;
using PX.Objects.EP;
using PX.Objects.RQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
	public class SiteSetupApproval : PXBqlTable, IBqlTable, IAssignedMap
	{
		public abstract class siteApproval : BqlBool.Field<siteApproval>{ }
		[EPRequireApproval]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
		[PXUIField(DisplayName = " Site Approval Map")]
		public virtual bool? SiteApproval { get; set; }

		#region ApprovalID
		public abstract class approvalID : BqlInt.Field<approvalID>
		{
		}
		[PXDBIdentity(IsKey = true)]
		public virtual int? ApprovalID { get; set; }
		#endregion

		#region AssignmentMapID
		public abstract class assignmentMapID : BqlInt.Field<assignmentMapID>
		{
		}
		[PXDefault]
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeSSS>>>), SubstituteKey = typeof(EPAssignmentMap.name))]
		[PXUIField(DisplayName = "Approval Map")]
		public virtual Int32? AssignmentMapID { get; set; }
		#endregion

		#region AssignmentNotificationID
		public abstract class assignmentNotificationID : BqlInt.Field<assignmentNotificationID>
		{
		}
		[PXDBInt]
		[PXSelector(typeof(PX.SM.Notification.notificationID), SubstituteKey = typeof(PX.SM.Notification.name))]
		[PXUIField(DisplayName = "Pending Approval Notification")]
		public virtual int? AssignmentNotificationID { get; set; }
		#endregion

		#region IsActive
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }

		[PXDBBool]
        [PXDefault(typeof(Search<GSynchExt.SiteSetup.siteApprovalMap>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive
		{
			get; set;
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

		public static class AssignmentMapType
		{

            public class AssignmentMapTypeSSS : Constant<string>
            {
                public AssignmentMapTypeSSS() : base(typeof(GSynchExt.SolarSiteSurvey).FullName) { }
            }

        }
    }
}
