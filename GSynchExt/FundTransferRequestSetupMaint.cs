using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.EP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
	public class FundTransferRequestSetupMaint : PXGraph<FundTransferRequestSetupMaint>
	{
		#region Views

		public PXSave<FundTransferRequestSetup> Save;
		public PXCancel<FundTransferRequestSetup> Cancel;
        public PXSelect<FundTransferRequestSetup> Setup;
        public SelectFrom<FundTransferApproval>.View SetupApproval;
        public SelectFrom<ISGAFARequestSetupApproval>.View FARSetupApproval;

        #endregion

        #region Events

        protected virtual void _(Events.FieldUpdated<FundTransferRequestSetup, FundTransferRequestSetup.approvalMap> e)
        {
            FundTransferRequestSetup row = e.Row;

            if (row != null)
            {
                foreach (FundTransferApproval setup in SetupApproval.Select())
                {
                    SetupApproval.SetValueExt<FundTransferApproval.isActive>(setup, row.ApprovalMap);
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<FundTransferRequestSetup, FundTransferRequestSetup.fARequestapprovalMap> e)
        {
            FundTransferRequestSetup row = e.Row;

            if (row != null)
            {
                foreach (ISGAFARequestSetupApproval setup in FARSetupApproval.Select())
                {
                    FARSetupApproval.SetValueExt<ISGAFARequestSetupApproval.isActive>(setup, row.FARequestapprovalMap);
                }
            }
        }

        #endregion
    }
}
