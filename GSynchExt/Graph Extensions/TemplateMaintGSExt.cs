using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using PX.Objects;
using GSynchExt;
using PX.Objects.CS;

namespace PX.Objects.PM
{
    public class TemplateMaintGSExt : PXGraphExtension<PX.Objects.PM.TemplateMaint>
    {
        #region DAC Attributes Override
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
        #region PMCompletedPctMethod
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PMCompletedPctMethod.ByQuantity)]
        protected virtual void _(Events.CacheAttached<PMTask.completedPctMethod> e) { }
        #endregion
        #endregion
        #region Event Handlers
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
            if (!GSProjectHelper.ValidatePredecessorTask(prTask, row, true, out preTaskDate, out sucTaskkDate))
            {
                e.Cancel = true;
                throw new PXSetPropertyException<PMTaskGSExt.usrPredecessorTaskCD>(GSynchExt.Messages.InvalidPredecessor2, e.NewValue.ToString(), row.TaskCD);
            }
            if (prTask?.TaskID > 0)
            {
                e.Cache.SetValue<PMTaskGSExt.usrPredecessorTaskID>(rowExt, prTask?.TaskID);
            }
        }
        protected virtual void _(Events.RowSelected<PMTask> e)
        {
            PMTask doc = (PMTask)e.Row;
            if (doc == null) return;

            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrPredecessorTaskCD>(e.Cache, doc, doc.ProjectID > 0);
        }
        #endregion
        #region Overrided methods
        #endregion
    }
}