using PX.Data;
using PX.Objects.CS;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.IN;
using System;
using System.Collections;

namespace PX.Objects.PM
{
    public class ProjectBalanceMaintGSExt : PXGraphExtension<ProjectBalanceMaint>
    {
        #region DAC Attributes Override

        #endregion

        protected virtual void _(Events.RowPersisting<PMBudget> e)
        {
            var accntGrp = GSProjectHelper.GetTimelineDefaultAccntGrp(this.Base);
            string error = GSProjectHelper.ValidateBudgetForTimelineTask(e.Row, accntGrp?.GroupID);
            if (error != null)
            {
                var task = PMTask.PK.Find(this.Base, e.Row.ProjectID, e.Row.ProjectTaskID);
                var proj = PMProject.PK.Find(this.Base, e.Row.ProjectID);
                e.Cache.RaiseExceptionHandling<PMBudget.amount>(e.Row, e.Row.Amount, new PXSetPropertyException(error, PXErrorLevel.Error, accntGrp.GroupCD, proj.ContractCD , task.TaskCD));
            }
        }
    }
}
