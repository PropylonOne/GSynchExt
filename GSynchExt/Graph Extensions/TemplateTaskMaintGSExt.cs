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
    public class TemplateTaskMaintGSExt : PXGraphExtension<PX.Objects.PM.TemplateTaskMaint>
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
        #endregion
    }
}