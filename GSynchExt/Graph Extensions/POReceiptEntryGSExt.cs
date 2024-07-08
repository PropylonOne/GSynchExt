using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.SO;
using SOOrder = PX.Objects.SO.SOOrder;
using SOLine = PX.Objects.SO.SOLine;
using PX.Data.DependencyInjection;
using PX.Data.ReferentialIntegrity.Attributes;

using PX.Objects.PM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AP.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.Common.Bql;
using PX.Objects.Extensions.CostAccrual;
using PX.Objects.DR;
using PX.Objects;
using PX.Objects.PO;

namespace PX.Objects.PO
{
    public class POReceiptEntryGSExt : PXGraphExtension<POReceiptEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.orderTotal))]
        [PXUIField(DisplayName = "Total Cost", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
        public virtual void _(Events.CacheAttached<POReceipt.curyOrderTotal> e) { }
        #endregion
    }
}
