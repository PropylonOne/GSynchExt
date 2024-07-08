using System;
using PX.Data;
using PX.Data.BQL.Fluent;

namespace GSynchExt
{
	public class GSBOQSetupMaint : PXGraph<GSBOQSetupMaint>
	{

        #region Public Selects
        public SelectFrom<GSBOQSetup>.View BOQSetup;
        public SelectFrom<BOQSetupApproval>.View SetupApproval;

        #endregion
        public PXSave<GSBOQSetup> Save;
        public PXCancel<GSBOQSetup> Cancel;

        #region Events

        protected virtual void _(Events.FieldUpdated<GSBOQSetup, GSBOQSetup.approvalMap> e)
        {
            GSBOQSetup row = e.Row;

            if (row != null)
            {
                foreach (BOQSetupApproval setup in SetupApproval.Select())
                {
                    SetupApproval.SetValueExt<BOQSetupApproval.isActive>(setup, row.ApprovalMap);
                }
            }
        }
        #endregion
    }
}