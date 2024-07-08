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
	public class BOQApprovalSetupMaint : PXGraph<BOQApprovalSetupMaint>
	{
		#region Views

		public PXSelect<GSBOQSetup> Setup;
		public PXSelect<BOQSetupApproval> SetupApproval;

		#endregion

		#region Buttons

		public PXSave<GSBOQSetup> Save;
		public PXCancel<GSBOQSetup> Cancel;

		#endregion

	}

}
