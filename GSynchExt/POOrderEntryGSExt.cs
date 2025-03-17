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
using PX.Objects.PJ.Submittals.PJ.Graphs;
using static PX.Objects.TX.CSTaxCalcType;

namespace PX.Objects.PO
{
    public class POOrderEntryGSExt : PXGraphExtension<POOrderEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.distributionModule>(); }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXUIField(DisplayName = "Line Nbr", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
        public virtual void _(Events.CacheAttached<POLine.lineNbr> e) { }
        #endregion

        #region Actions
        public PXAction<POOrder> MassUpdateSub;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Mass Update Sub Acc", Enabled = false)]
        protected virtual IEnumerable massUpdateSub(PXAdapter adapter)
        {
            var currentPO = Base.Document.Current;
            var currentPOExt = currentPO.GetExtension<POOrderGSExt>();
            if (currentPOExt.UsrMassSubItem == null || currentPOExt.UsrMassSubItem == 0)
                return adapter.Get(); ;
            foreach (POLine item in Base.Transactions.Select())
            {
                item.POAccrualSubID = currentPOExt.UsrMassSubItem;
                Base.Transactions.Current = item;
                Base.Transactions.Update(Base.Transactions.Current);

            }
            Base.Save.Press();
            return adapter.Get();
        }

        public PXAction<POOrder> addSchedule;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Shipment Schedule",
            MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable AddSchedule(PXAdapter adapter)
        {
            if (this.Base.Transactions.Current == null) return adapter.Get();

            POOrder scheduleRec = this.Base.Document.Current;
            ISGAPOScheduleEntry graph = PXGraph.CreateInstance<ISGAPOScheduleEntry>();
            graph.Schedule.Current = scheduleRec;

            if (scheduleRec != null)
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<ISGAPOScheduleEntry>().CreatePOScheduleFromPOLine(scheduleRec, redirect: true));
            }
            return adapter.Get();
        }
        #endregion

        #region Events
        protected virtual void POOrder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            POOrder doc = e.Row as POOrder;
            if (doc == null)
            {
                return;
            }
            bool hasLines = this.Base.Transactions.Select().Count() > 0;
            addSchedule.SetEnabled(hasLines);
        }
        #endregion

    }
}