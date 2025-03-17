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
using GSynchExt;

namespace PX.Objects.IN
{
    public class INReceiptEntryGSExt : PXGraphExtension<INReceiptEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        protected virtual void _(Events.FieldUpdated<INTran, INTran.projectID> e)
        {
            INTran doc = (INTran)e.Row;
            INRegister reg = this.Base.receipt.Current;
            if (doc == null || reg == null) return;
            SiteSetup sitePref = PXSelect<SiteSetup>.Select(this.Base);
            if (sitePref == null) return;
            PMProject proj = PMProject.PK.Find(this.Base, doc?.ProjectID);
            if (proj == null) return;
            if (proj.NonProject == false)
            {
                INSite site = INSite.PK.Find(Base, doc?.SiteID);
                if(site == null) return;
                var sitePrefix = site.SiteCD.Substring(0, 3);
                if (site != null)
                {
                    var code = (sitePref.ReceiptReasonCodePrefix + "" + sitePrefix).ToUpper();
                    if (code != null & reg.TransferNbr == null)
                    {
                        ReasonCode reasonCode = ReasonCode.PK.Find(Base, code);
                        if (reasonCode != null)
                        {
                            doc.ReasonCode = reasonCode.ReasonCodeID;
                        }
                    }
                }
            }
        }
        protected virtual void _(Events.RowPersisting<INTran> e)
        {
            var row = e.Row as INTran;
            if (row == null) return;
            var doc = this.Base.receipt.Current;
            if(doc == null ) return;
            if(doc.TransferNbr != null) return;
            if (PXContext.GetScreenID() == "IN.30.10.00")
            {
                if (row.ReasonCode == null)
                {
                    throw new PXException("Reason Code cannot be empty.", PXErrorLevel.Error);
                }
            }           
        }
    }
}
