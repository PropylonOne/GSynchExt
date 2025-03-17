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
    public class ISGAFARequestSetupMaint : PXGraph<ISGAFARequestSetupMaint>
    {
        #region Views

        public PXSave<ISGAFARequestSetup> Save;
        public PXCancel<ISGAFARequestSetup> Cancel;
        public PXSelect<ISGAFARequestSetup> Setup;
        public SelectFrom<ISGAFARequestSetupApproval>.View FARSetupApproval;
        #endregion
        #region Events
        protected virtual void _(Events.FieldUpdated<ISGAFARequestSetup, ISGAFARequestSetup.fARequestapprovalMap> e)
        {
            ISGAFARequestSetup row = e.Row;

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
