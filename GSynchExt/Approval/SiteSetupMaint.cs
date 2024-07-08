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
	public class SiteSetupMaint : PXGraph<SiteSetupMaint>
	{
		#region Views

		public PXSelect<SiteSetup> Setup;
		public PXSelect<SiteSetupApproval> SetupApproval;

		#endregion

		#region Buttons

		public new PXSave<SiteSetup> Save;
		public new PXCancel<SiteSetup> Cancel;

        #endregion

        #region Events

        protected virtual void _(Events.FieldUpdated<SiteSetup, SiteSetup.siteApprovalMap> e)
        {
            SiteSetup row = e.Row;

            if (row != null)
            {
                foreach (SiteSetupApproval setup in SetupApproval.Select())
                {
                    SetupApproval.SetValueExt<SiteSetupApproval.isActive>(setup, row.SiteApprovalMap);
                }
            }
        }
        #endregion
    }

}
