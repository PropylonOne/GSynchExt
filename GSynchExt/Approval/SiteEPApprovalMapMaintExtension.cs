using PX.Data;
using PX.Objects.EP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
    public class SiteEPApprovalMapMaintExtension : PXGraphExtension<EPApprovalMapMaint>
    {
        #region Overrides

        public delegate IEnumerable<string> GetEntityTypeScreensDel();

        [PXOverride]
        public virtual IEnumerable<string> GetEntityTypeScreens(GetEntityTypeScreensDel del)
        {
            foreach (string s in del())
            {
                yield return s;
            }

            yield return "PM770034";
        }
        #endregion
    }
}
