using PX.Data;
using PX.Data.BQL;
using PX.Objects.EP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
	public class FundTransferApprovalSetupMaint : PXGraph<FundTransferApprovalSetupMaint>
	{
		#region Views

		public PXSelect<FundTransferRequestSetup> Setup;
		public PXSelect<FundTransferApproval> SetupApproval;

		#endregion

		#region Buttons

		public PXSave<FundTransferRequestSetup> Save;
		public PXCancel<FundTransferRequestSetup> Cancel;

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
        #endregion

    }

}
