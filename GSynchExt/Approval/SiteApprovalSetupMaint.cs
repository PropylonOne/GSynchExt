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
	public class SiteApprovalSetupMaint : PXGraph<SiteApprovalSetupMaint>
	{
		#region Views

		public PXSelect<SiteSetup> Setup;
		public PXSelect<SiteSetupApproval> SetupApproval;

		#endregion

		#region Buttons

		public PXSave<SiteSetup> Save;
		public PXCancel<SiteSetup> Cancel;

        #endregion

    }

}
