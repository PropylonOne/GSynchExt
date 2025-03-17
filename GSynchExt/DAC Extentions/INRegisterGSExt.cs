using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.IN
{
    public sealed class INRegisterGSExt : PXCacheExtension<PX.Objects.IN.INRegister>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        #region UsrISGAEstTotalCost
        [PXDecimal(4)]
        [PXUIField(DisplayName = "Estimated Total Cost", Enabled = false)]
        public decimal? UsrISGAEstTotalCost { get; set; }
        public abstract class usrISGAEstTotalCost : PX.Data.BQL.BqlDecimal.Field<usrISGAEstTotalCost> { }
        #endregion

    }
}