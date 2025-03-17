using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.PJ.Common.DAC;
using PX.Objects.PJ.Submittals.PJ.DAC;
using PX.Objects.CR;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using PX.Objects.Common.Labels;
using PX.Objects;
using GSynchExt;
using PX.Objects.RQ;
using PX.Objects.PM;
using PX.Objects.CS;
using System.Security.Cryptography;

namespace PX.Objects.IN
{
    public class INAdjustmentEntryGSExt : PXGraphExtension<INAdjustmentEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.projectModule>(); }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXRestrictor(typeof(Where<INSite.branchID, Equal<Current<AccessInfo.branchID>>>), "Invalid Branch")]
        public virtual void _(Events.CacheAttached<INTran.siteID> e) { }
        #endregion
    }
}