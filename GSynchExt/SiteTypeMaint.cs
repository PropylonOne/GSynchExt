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
    public class SiteTypeMaint : PXGraph<SiteTypeMaint>
    {
        #region Views

        public PXSave<SiteType> Save;
        public PXCancel<SiteType> Cancel;
        public PXSelect<SiteType> SiteTypeview;

        #endregion
    }

}
