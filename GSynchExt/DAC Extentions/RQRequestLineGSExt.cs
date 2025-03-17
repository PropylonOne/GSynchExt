using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.RQ;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using GSynchExt;
using static PX.Objects.RQ.RQRequestLineGSExt;


namespace PX.Objects.RQ
{
    public sealed class RQRequestLineGSExt : PXCacheExtension<PX.Objects.RQ.RQRequestLine>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.inventory>(); }
        #endregion

        #region UsrBOQID
        [PXDBInt]
        [PXUIField(DisplayName = "BOQ-Project")]
        public int? UsrBOQID { get; set; }
        public abstract class usrBOQID : PX.Data.BQL.BqlInt.Field<usrBOQID> { }
        #endregion

        #region UsrRevisionID
        [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
        [PXUIField(DisplayName = "BoQ Revision")]
        public string UsrRevisionID { get; set; }
        public abstract class usrRevisionID : PX.Data.BQL.BqlString.Field<usrRevisionID> { }
        #endregion

        /*#region UsrRQSiteID
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Warehouse")]
        [PXSelector(typeof(Search<INSite.siteID>), SubstituteKey = typeof(INSite.siteCD))]
        public int? UsrRQSiteID { get; set; }
        public abstract class usrRQSiteID : PX.Data.BQL.BqlBool.Field<usrRQSiteID> { }
        #endregion

        #region UsrTransQty
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Transfer Quantity", Enabled = false)]
        public decimal? UsrTransQty { get; set; }
        public abstract class usrTransQty : PX.Data.BQL.BqlDecimal.Field<usrTransQty> { }
        #endregion

        #region UsrOrderQty
        [PXDBDecimal]
        [PXUIField(DisplayName = "Order Quantity", Enabled = false)]
        [PXFormula(typeof(Sub<RQRequestLine.reqQty, RQRequestLineGSExt.usrTransQty>))]
        public decimal? UsrOrderQty { get; set; }
        public abstract class usrOrderQty : PX.Data.BQL.BqlDecimal.Field<usrOrderQty> { }

        #endregion*/
    }
}