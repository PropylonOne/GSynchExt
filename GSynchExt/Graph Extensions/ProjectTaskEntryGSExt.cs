using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using PX.Objects;
using PX.Objects.PM;
using PX.Objects.CN.ProjectAccounting.PM.Descriptor;
using PX.Objects.CN.Common.Helpers;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using GSynchExt;
using PX.TM;
using PX.SM;
using PX.Objects.EP;
using System.Linq;

namespace PX.Objects.PM
{
    public class ProjectTaskEntryGSExt : PXGraphExtension<PX.Objects.PM.ProjectTaskEntry>
    {
        #region DAC Attributes Override
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PMCompletedPctMethod.ByQuantity)]
        protected virtual void _(Events.CacheAttached<PMTask.completedPctMethod> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(ProjectTaskType.Cost)]
        protected virtual void _(Events.CacheAttached<PMTask.type> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PMProductivityTrackingType.Template)]
        protected virtual void _(Events.CacheAttached<PMCostBudget.productivityTracking> e) { }

        #region UsrPredecessorTaskCD
        [PXDBString(30)]
        [PXUIField(DisplayName = "Predecessor Task")]
        //Validate flase to enable copy function. Value is checked again at updating
        [PXSelector(typeof(Search<PMTask.taskCD,
                    Where<PMTask.projectID, Equal<Current<PMTask.projectID>>,
                    Or<PMTask.taskCD, Contains<PMTaskGSExt.usrPredecessorTaskCD>>>>), typeof(PMTask.taskCD), typeof(PMTaskGSExt.usrPredecessorTaskCD), ValidateValue = false)]
        protected virtual void _(Events.CacheAttached<PMTaskGSExt.usrPredecessorTaskCD> e)
        {
        }
        #endregion
        #endregion
        public delegate IEnumerable ActivateDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Activate(PXAdapter adapter, ActivateDelegate BaseMethod)
        {
            bool validateOK = true;
            var proj = PMProject.PK.Find(this.Base, this.Base.Task.Current?.ProjectID);
            ValidateTaskStatusUpdate(proj, this.Base.Task.Current, ProjectTaskStatus.Active, this.Base.Task.Current.Status, out validateOK);
            if (validateOK)
            {
                //return BaseMethod?.Invoke(adapter);
                BaseMethod?.Invoke(adapter);
            }
            return adapter.Get();
        }

        public delegate IEnumerable CompleteDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Complete(PXAdapter adapter, CompleteDelegate BaseMethod)
        {

            bool validateOK = true;
            var proj = PMProject.PK.Find(this.Base, this.Base.Task.Current?.ProjectID);
            ValidateTaskStatusUpdate(proj, this.Base.Task.Current, ProjectTaskStatus.Completed, this.Base.Task.Current.Status, out validateOK);
            if (validateOK)
            {
                /// Get template notification id
                BaseMethod.Invoke(adapter);

                Notification templ = PXSelect<Notification,
                    Where<Notification.screenID, Equal<Required<Notification.screenID>>,
                    And<Notification.subject, Contains<Required<Notification.subject>>>>>.Select(this.Base, "PM302000", "Completion of Project Task").FirstOrDefault();
                if (templ != null) GSProjectHelper.AddEmailActivity(this.Base.Task.Current, (int)templ.NotificationID);
                else throw new PXSetPropertyException(GSynchExt.Messages.NoEmailSent, PXErrorLevel.Warning);
            }
            return adapter.Get();
        }

        [PXOverride]
        public delegate IEnumerable HoldDelegate(PXAdapter adapter);
        [PXButton]
        [PXUIField]
        public IEnumerable Hold(PXAdapter adapter, HoldDelegate BaseMethod)
        {
            bool validateOK = true;
            var proj = PMProject.PK.Find(this.Base, this.Base.Task.Current?.ProjectID);
            ValidateTaskStatusUpdate(proj, this.Base.Task.Current, ProjectTaskStatus.Planned, this.Base.Task.Current.Status, out validateOK);
            if (validateOK)
            {
                BaseMethod?.Invoke(adapter);
            }
            return adapter.Get();
        }
        #region Event Handlers
        protected virtual void _(Events.FieldVerifying<PMTask, PMTask.status> e)
        {
            var row = e.Row;
            if (row == null) return;
            var proj = PMProject.PK.Find(this.Base, row.ProjectID);
            if (e.NewValue == null || row == null || proj == null) return;
            bool validateOK = true;
            ValidateTaskStatusUpdate(proj, row, e.NewValue.ToString(), e.OldValue.ToString(), out validateOK);
            if (!validateOK)
            {
                e.Cancel = true;
            }
        }
        protected virtual void _(Events.FieldVerifying<PMTask, PMTaskGSExt.usrPredecessorTaskCD> e)
        {
            var row = e.Row as PMTask;
            if (row == null) return;

            /// UsrPredecessorTaskCD is used since the task IDs are not generated at the time of new entry.
            if (e.NewValue == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();

            PMTask prTask = new PMTask();
            prTask = PMTask.UK.Find(this.Base, row.ProjectID, e.NewValue.ToString());
            if (prTask == null && row.ProjectID > 0) // Raise error only the project is saved.
            {
                throw new PXSetPropertyException(GSynchExt.Messages.PredecessorNoExist, rowExt.UsrPredecessorTaskCD);
            }
            if (prTask == null) return;
            ///Rest of the validation should be done if prTask is not null
            DateTime? preTaskDate = null;
            DateTime? sucTaskkDate = null;
            if (!GSProjectHelper.ValidatePredecessorTask(prTask, row, false, out preTaskDate, out sucTaskkDate))
            {
                e.Cancel = true;
                throw new PXSetPropertyException<PMTaskGSExt.usrPredecessorTaskCD>(GSynchExt.Messages.InvalidPredecessor, preTaskDate, sucTaskkDate, row.TaskCD);
            }
            if (prTask?.TaskID > 0)
            {
                e.Cache.SetValue<PMTaskGSExt.usrPredecessorTaskID>(rowExt, prTask?.TaskID);
            }
        }
        protected virtual void _(Events.RowSelected<PMTask> e)
        {
            var row = e.Row;
            if (row == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();

            bool IsPlanning = row.Status == ProjectTaskStatus.Planned;


            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrLeadDays>(e.Cache, row, IsPlanning);
            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrOwnerID>(e.Cache, row, IsPlanning);
            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrPredecessorTaskID>(e.Cache, row, IsPlanning);
            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrNotifier>(e.Cache, row, IsPlanning);
            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrIsComplDocReq>(e.Cache, row, IsPlanning);
            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrPredecessorTaskCD>(e.Cache, row, row.ProjectID > 0);
        }
        protected virtual void _(Events.FieldUpdating<PMTask, PMTaskGSExt.usrLeadDays> e)
        {
            var row = e.Row;
            if (row == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();
            ProjectManagementSetup pmSetup = (ProjectManagementSetup)PXSelect<ProjectManagementSetup>.Select(this.Base);
            if (e.NewValue != e.OldValue && !e.NewValue.Equals(0))
            {
                //Update Planned end dates based on the lead days.
                if (row.PlannedStartDate == null)
                {
                    return;
                }
                else
                {
                    row.PlannedEndDate = GSProjectHelper.CalculatePlannedEndDate(row, (DateTime)row.PlannedStartDate, (int)e.NewValue, pmSetup.CalendarId);
                }
            }
        }
        protected virtual void _(Events.FieldUpdated<PMTask, PMTask.status> e)
        {
            var row = e.Row;
            if (row == null) return;
            SiteSetup sitePref = PXSelect<SiteSetup>.Select(this.Base);
            if (sitePref == null) return;
            string oldval = e.OldValue.ToString();
            string newval = e.NewValue.ToString();
            var proj = PMProject.PK.Find(this.Base, row.ProjectID);
            bool validateOK = true;
            ValidateTaskStatusUpdate(proj, row, newval, oldval, out validateOK);
            if (validateOK)
            {
                try
                {
                    GSProjectHelper.ProcessTaskStatusUpdate(oldval, newval, sitePref, row, this.Base);
                }
                catch (Exception ex)
                {
                    throw new PXException(ex.Message);
                }
            }
        }
        protected virtual void _(Events.FieldUpdating<PMTask, PMTask.plannedEndDate> e)
        {
            var row = e.Row;
            if (row == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();
            ProjectManagementSetup pmSetup = (ProjectManagementSetup)PXSelect<ProjectManagementSetup>.Select(this.Base);
            if (e.NewValue != e.OldValue && !e.NewValue.Equals(null))
            {
                //Update lead days.
                if (row.PlannedStartDate == null)
                {
                    return;
                }
                else
                {
                    rowExt.UsrLeadDays = GSProjectHelper.GetLeadDays((DateTime)row.PlannedStartDate, (DateTime)row.PlannedStartDate, pmSetup.CalendarId);
                }
            }


        }
        #endregion
        protected virtual void ValidateTaskStatusUpdate(PMProject proj, PMTask task, string newStatus, string oldStatus, out bool validateOK)
        {
            validateOK = true;
            var error = GSProjectHelper.ValidateTaskStatusUpdate(this.Base, proj, task, newStatus, oldStatus);
            if (error != null)
            {
                validateOK = false;
                throw new PXException(error);
            }
        }


        protected virtual void _(Events.RowDeleting<PMTask> e)
        {
            var row = e.Row;
            if (row == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();
            var preTaskID = row.TaskID;
            var IsExists = false;

            var task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>>>.Select(this.Base, row.ProjectID);
            if (task == null) return;

            foreach (PMTask tasks in task)
            {
                var tasksExt = tasks.GetExtension<PMTaskGSExt>();
                if(tasksExt.UsrPredecessorTaskCD != null)
                {
                    if (tasksExt.UsrPredecessorTaskCD.TrimEnd() == row.TaskCD.TrimEnd())
                    {
                        IsExists = true;
                    }
                    if (IsExists)
                    {
                        throw new PXException(GSynchExt.Messages.CannotDeleteTask);
                    }
                }
            }
        }
    }
}