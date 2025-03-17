using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.IN
{
    public sealed class INTranGSExt : PXCacheExtension<PX.Objects.IN.INTran>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        #region UsrcreatedByMTR
        [PXDBBool]
        [PXUIField(DisplayName="createdByMTR")]

        public bool? UsrcreatedByMTR { get; set; }
        public abstract class usrcreatedByMTR : PX.Data.BQL.BqlBool.Field<usrcreatedByMTR> { }
        #endregion

        #region UsrMTRRef
        [PXDBString(30)]
        [PXUIField(DisplayName="MTRRef")]

        public string UsrMTRRef { get; set; }
        public abstract class usrMTRRef : PX.Data.BQL.BqlString.Field<usrMTRRef> { }
        #endregion

        #region UsrCreatedBySMR
        [PXDBBool]
        [PXUIField(DisplayName = "Created By SMR")]

        public bool? UsrCreatedBySMR { get; set; }
        public abstract class usrCreatedBySMR : PX.Data.BQL.BqlBool.Field<usrCreatedBySMR> { }
            #endregion

        #region UsrSMRRef
        [PXDBString(30)]
        [PXUIField(DisplayName = "Service Material Request Reference")]

        public string UsrSMRRef { get; set; }
        public abstract class usrSMRRef : PX.Data.BQL.BqlString.Field<usrSMRRef> { }

        #endregion

        #region UsrISGAEstLineCost
        [PXDecimal(4)]
        [PXUIField(DisplayName = "Estimated Cost", Enabled = false)]
        public decimal? UsrISGAEstLineCost { get; set; }
        public abstract class usrISGAEstLineCost : PX.Data.BQL.BqlDecimal.Field<usrISGAEstLineCost> { }
        #endregion
    }
}