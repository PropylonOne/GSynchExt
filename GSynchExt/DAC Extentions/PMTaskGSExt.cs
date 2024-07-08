using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CN.ProjectAccounting.Descriptor;
using PX.Objects.CN.ProjectAccounting.PM.Descriptor;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using PX.Data.BQL.Fluent;
using PX.TM;

namespace PX.Objects.PM
{
    public class PMTaskGSExt : PXCacheExtension<PX.Objects.PM.PMTask>
    {
        #region UsrLeadDays
        [PXDBInt]
        [PXUIField(DisplayName = "Lead Days")]
        public virtual int? UsrLeadDays { get; set; }
        public abstract class usrLeadDays : PX.Data.BQL.BqlInt.Field<usrLeadDays> { }
        #endregion
        //IsComplDocReq  bool default false. Null able
        #region UsrIsComplDocReq
        [PXDBBool()]
        [PXUIField(DisplayName = "Compliance Docs Required")]
     //   [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsComplDocReq { get; set; }
        public abstract class usrIsComplDocReq : PX.Data.BQL.BqlBool.Field<usrIsComplDocReq> { }
        #endregion

        #region UsrOwnerID
        [PX.TM.Owner(Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? UsrOwnerID { get; set; }
        public abstract class usrOwnerID : PX.Data.BQL.BqlInt.Field<usrOwnerID> { }
        #endregion

        #region UsrPredecessorTaskID
        [PXDBInt]
        [PXUIField(DisplayName = "Predecessor Task ID - Not Used")]
        //[PXSelector(typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<PMTask.projectID>>>>), SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? UsrPredecessorTaskID { get; set; }
        public abstract class usrPredecessorTaskID : PX.Data.BQL.BqlInt.Field<usrPredecessorTaskID> { }
        #endregion

        #region UsrPredecessorTaskCD
        [PXDBString(30)]
        [PXUIField(DisplayName = "Predecessor Task")]
        public virtual string UsrPredecessorTaskCD { get; set; }
        public abstract class usrPredecessorTaskCD : PX.Data.BQL.BqlString.Field<usrPredecessorTaskCD> { }
        #endregion

        #region UsrNotifier
        [PX.TM.Owner(Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Notifier")]
        public virtual int? UsrNotifier { get; set; }
        public abstract class usrNotifier : PX.Data.BQL.BqlInt.Field<usrNotifier> { }
        #endregion

        #region UsrNotifyWorkgroup
        public abstract class usrNotifyWorkgroup : PX.Data.BQL.BqlInt.Field<usrNotifyWorkgroup> { }
        protected int? _UsrNotifyWorkgroup;
        [PXDBInt]
        [PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]

        public virtual int? UsrNotifyWorkgroup
        {
            get;
            set;
        }
        #endregion
    }
}