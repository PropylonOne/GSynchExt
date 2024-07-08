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

namespace PX.Objects.RQ
{
    public class RQRequisitionEntryGSExt : PXGraphExtension<RQRequisitionEntry>
    {

        PXSelect<AttrView> attrView;


        public PXAction<RQRequisition> MassUpdateSub;
            [PXButton(CommitChanges = true)]
            [PXUIField(DisplayName = "Mass Update Sub Acc", Enabled = false)]
        protected virtual IEnumerable massUpdateSub(PXAdapter adapter)
        {
            var currentRequisition = Base.Document.Current;
            var currentRequisitionExt = currentRequisition.GetExtension<RQRequisitionGSExt>();
            if (currentRequisitionExt.UsrMassSubItem == null || currentRequisitionExt.UsrMassSubItem == 0)
                return adapter.Get(); ;
            foreach (RQRequisitionLine item in Base.Lines.Select())
            {
                item.ExpenseSubID = currentRequisitionExt.UsrMassSubItem;
                Base.Lines.Current = item;
                Base.Lines.Update(Base.Lines.Current);

            }
            Base.Save.Press();
            return adapter.Get();
        }

        public PXAction<RQRequisition> MassUpdateWH;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Mass Update Warehouse", Enabled = false)]
        protected virtual IEnumerable massUpdateWH(PXAdapter adapter)
        {
            var currentRequisition = Base.Document.Current;
            var currentRequisitionExt = currentRequisition.GetExtension<RQRequisitionGSExt>();
            if (currentRequisitionExt.UsrSiteID == null || currentRequisitionExt.UsrSiteID == 0)
                return adapter.Get(); ;
            foreach (RQRequisitionLine item in Base.Lines.Select())
            {
                item.SiteID = currentRequisitionExt.UsrSiteID;
                Base.Lines.Current = item;
                Base.Lines.Update(Base.Lines.Current);

            }
            Base.Save.Press();
            return adapter.Get();
        }
        protected virtual void _(Events.RowSelected<RQRequisition> e)
        {
            RQRequisition doc = (RQRequisition)e.Row;
            if (doc == null) return;
           // var docExt = doc.GetExtension<RQRequisitionGSExt>();
            bool isOnHold = doc.Status == RQRequisitionStatus.Hold;
            bool hasLines = this.Base.Lines.Select().Count != 0;

            PXUIFieldAttribute.SetEnabled<RQRequisitionGSExt.usrSiteID>(e.Cache, doc, isOnHold && hasLines && !this.Base.Lines.Cache.IsDirty);
            PXUIFieldAttribute.SetEnabled<RQRequisitionGSExt.usrMassSubItem>(e.Cache, doc, isOnHold && hasLines && !this.Base.Lines.Cache.IsDirty);


            MassUpdateSub.SetVisible(isOnHold && hasLines && !e.Cache.IsDirty);
            MassUpdateWH.SetVisible(isOnHold && hasLines && !e.Cache.IsDirty);
        }
    }


    #region Partial Classes
    public partial class AttrView : PXBqlTable, IBqlTable
    {

		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        [PXUnboundDefault]
        [Site(DisplayName = "Warehouse", DescriptionField = typeof(INSite.descr))]
		public virtual int? SiteID { get; set; }
		#endregion
		
	}
	#endregion
}
