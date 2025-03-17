using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Common;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System;
using PX.Objects.GL;
using PX.Data.BQL.Fluent;
using PX.Objects.CA;
using PX.Objects.FA;

namespace GSynchExt
{
  public sealed class FixedAssetGSExt : PXCacheExtension<FixedAsset>
  {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.fixedAsset>(); }
        #endregion

        #region UsrISGARequestedQty
        [PXDBDecimal]
        [PXUIField(DisplayName = "Request Qty")]
        public Decimal? UsrISGARequestedQty { get; set; }
        public abstract class usrISGARequestedQty : PX.Data.BQL.BqlDecimal.Field<usrISGARequestedQty> { }
        #endregion

        #region UsrISGAIssuedQty
        [PXDBDecimal]
        [PXUIField(DisplayName = "In Use Qty")]
        public Decimal? UsrISGAIssuedQty { get; set; }
        public abstract class usrISGAIssuedQty : PX.Data.BQL.BqlDecimal.Field<usrISGAIssuedQty> { }
        #endregion

    }
}