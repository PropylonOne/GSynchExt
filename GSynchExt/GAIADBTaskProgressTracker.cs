using System;
using PX.Data;
using PX.Objects;
using PX.Objects.PM;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("GAIADBTaskProgressTracker")]
  public class GAIADBTaskProgressTracker : PXBqlTable, IBqlTable
  {
        #region ProjectID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Project ID", Enabled = false)]
        [PXSelector(typeof(Search<PMProject.contractID>), typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status), SubstituteKey = (typeof(PMProject.contractCD)))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region NotComTaskID1
        [ProjectTask(typeof(GAIADBTaskProgressTracker.projectID), IsKey = true)]
        [PXUIField(DisplayName = "Not Completed Task ID1", Enabled = false)]
        public virtual int? NotComTaskID1 { get; set; }
        public abstract class notComTaskID1 : PX.Data.BQL.BqlInt.Field<notComTaskID1> { }
        #endregion

        #region NotComTaskID2
        [ProjectTask(typeof(GAIADBTaskProgressTracker.projectID), IsKey = true)]
        [PXUIField(DisplayName = "Not Completed Task ID2", Enabled = false)]
        public virtual int? NotComTaskID2 { get; set; }
        public abstract class notComTaskID2 : PX.Data.BQL.BqlInt.Field<notComTaskID2> { }
        #endregion

        #region ComTaskID1
        [ProjectTask(typeof(GAIADBTaskProgressTracker.projectID),IsKey = true)]
        [PXUIField(DisplayName = " Completed Task ID1", Enabled = false)]
        public virtual int? ComTaskID1 { get; set; }
        public abstract class comTaskID1 : PX.Data.BQL.BqlInt.Field<comTaskID1> { }
        #endregion

        #region ComTaskID2
        [ProjectTask(typeof(GAIADBTaskProgressTracker.projectID), IsKey = true)]
        [PXUIField(DisplayName = " Completed Task ID2", Enabled = false)]
        public virtual int? ComTaskID2 { get; set; }
        public abstract class comTaskID2 : PX.Data.BQL.BqlInt.Field<comTaskID2> { }
        #endregion



        #region NotComTaskCD1
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<PMTask.taskCD>))]
        [PXUIField(DisplayName = "Not Completed Task CD1")]
        public virtual string NotComTaskCD1 { get; set; }
        public abstract class notComTaskCD1 : PX.Data.BQL.BqlString.Field<notComTaskCD1> { }
        #endregion

        #region NotComTaskCD2
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<PMTask.taskCD>))]
        [PXUIField(DisplayName = "Not Completed Task CD2")]
        public virtual string NotComTaskCD2 { get; set; }
        public abstract class notComTaskCD2 : PX.Data.BQL.BqlString.Field<notComTaskCD2> { }
        #endregion

        #region ComTaskCD1
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<PMTask.taskCD>))]
        [PXUIField(DisplayName = "Completed Task CD1")]
        public virtual string ComTaskCD1 { get; set; }
        public abstract class comTaskCD1 : PX.Data.BQL.BqlString.Field<comTaskCD1> { }
        #endregion

        #region ComTaskCD2
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<PMTask.taskCD>))]
        [PXUIField(DisplayName = "Completed Task CD2")]
        public virtual string ComTaskCD2 { get; set; }
        public abstract class comTaskCD2 : PX.Data.BQL.BqlString.Field<comTaskCD2> { }
        #endregion
  }
}