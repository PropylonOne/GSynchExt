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
using PX.Objects.CT;

namespace PX.Objects.RQ
{
    public class RQRequestEntryGSExt : PXGraphExtension<RQRequestEntry>
    {
		#region filter
		[System.SerializableAttribute()]
		[PXCacheName("BOQ Items Filter")]
		public partial class BOQItemsFilter : PXBqlTable, IBqlTable
		{
			#region BOQID
			public abstract class bOQID : PX.Data.BQL.BqlInt.Field<bOQID> { }
			[PXInt()]
			[PXUIField(DisplayName = "Project")]
			[PXSelector(typeof(Search2<Contract.contractID,
			InnerJoin<GSBOQ,
				On<Contract.contractID, Equal<GSBOQ.bOQID>,
					And<GSBOQ.status, Equal<BOQStatus.active>>>>>), 
				typeof(Contract.contractCD), typeof(Contract.description), typeof(Contract.status),
				SubstituteKey = (typeof(Contract.contractCD)))]
			public virtual Int32? BOQID
			{
				get;
				set;
			}
			#endregion
			#region RevisionID
			public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
			[PXString()]
			[PXUIField(DisplayName = "Revision")]
			[PXSelector(typeof(Search<GSBOQ.revisionID, Where<GSBOQ.bOQID, Equal<Current<BOQItemsFilter.bOQID>>,
				And<GSBOQ.status, Equal<GSynchExt.BOQStatus.active>>>>),
				typeof(GSBOQ.revisionID), typeof(GSBOQ.status))]
			public virtual String RevisionID
			{
				get;
				set;
			}
			#endregion
			#region IsCapType1
			public abstract class isCapType1 : PX.Data.BQL.BqlBool.Field<isCapType1> { }
			[PXUIField(DisplayName = "Capacity Type 1")]
			[PXBool()]
			public virtual bool IsCapType1
			{
				get;
				set;
			}
			#endregion
			#region CapacityType1
			public abstract class capacityType1 : PX.Data.BQL.BqlString.Field<capacityType1> { }
			[PXUIField(DisplayName = "Capacity Type 1")]
			[PXString()]
			[PXDefault(typeof(Search<GSBOQ.capacityType1, Where<GSBOQ.bOQID, Equal<Current<BOQItemsFilter.bOQID>>,
				And<GSBOQ.revisionID, Equal<Current<BOQItemsFilter.revisionID>>>>>))]
			public virtual String CapacityType1
			{
				get;
				set;
			}
			#endregion

			#region InventoryID
			public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
			[PXUIField(DisplayName = "Inventory ID")]
			[PXInt()]
			[PXSelector(typeof(SelectFrom<InventoryItem>
				.InnerJoin<GSBOQMatl>.On<InventoryItem.inventoryID.IsEqual<GSBOQMatl.inventoryID>>
				.Where<GSBOQMatl.bOQID.IsEqual<BOQItemsFilter.bOQID.FromCurrent>.
					And<GSBOQMatl.revisionID.IsEqual<BOQItemsFilter.revisionID.FromCurrent>>.And<GSBOQMatl.inventoryID.IsNotNull>>
				.AggregateTo<GroupBy<GSBOQMatl.inventoryID>>
				.SearchFor<InventoryItem.inventoryID>),
				SubstituteKey = typeof(InventoryItem.inventoryCD))]
			public virtual Int32? InventoryID
			{
				get;
				set;
			}
			#endregion
		}

		public PXFilter<BOQItemsFilter> boqitemsfilter;

		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelect<GSBOQMatl,
				Where<GSBOQMatl.bOQID, Equal<Current<BOQItemsFilter.bOQID>>,
					And<GSBOQMatl.revisionID, Equal<BOQItemsFilter.revisionID>>>> BoqItems;

		public IEnumerable boqItems()
		{
			List<object> parameters = new List<object>();
			var boqItemSelect = new PXSelect<GSBOQMatl,
				Where<GSBOQMatl.bOQID, Equal<Current<BOQItemsFilter.bOQID>>,
					And<GSBOQMatl.revisionID, Equal<Current<BOQItemsFilter.revisionID>>>>>(this.Base);

			if (boqitemsfilter.Current.CapacityType1 != null)
			{
				
			}

			PXDelegateResult delResult = new PXDelegateResult();
			delResult.Capacity = 202;
			delResult.IsResultFiltered = false;
			delResult.IsResultSorted = true;
			delResult.IsResultTruncated = false;

			var view = new PXView(this.Base, false, boqItemSelect.View.BqlSelect);
			var startRow = PXView.StartRow;
			int totalRows = 0;
			var resultset = view.Select(PXView.Currents, parameters.ToArray(), PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;

			foreach (GSBOQMatl boqResult in resultset)
			{
				GSBOQMatl boqItems = boqResult;
			    delResult.Add(boqItems);
			}

			return delResult;
		}
		#endregion

		public PXAction<RQRequest> addBOQItems;
		[PXUIField(DisplayName = "Add BOQ Items", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddBOQItems(PXAdapter adapter)
		{
			IEnumerable result = null;

			if (this.Base.Document.Current.Hold == true && this.Base.Document.Current.ReqClassID != null)
			{
				if (BoqItems.AskExt() == WebDialogResult.OK)
				{
					result = AddSelectedBoQLines(adapter);
				}

				boqitemsfilter.Cache.Clear();
				BoqItems.Cache.Clear();
				BoqItems.ClearDialog();
				BoqItems.View.Clear();
				BoqItems.View.ClearDialog();
			}

			if (result != null)
			{
				return result;
			}

			return adapter.Get();
		}

		public PXAction<RQRequest> addSelectedBoQLines;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddSelectedBoQLines(PXAdapter adapter)
		{
			if (this.Base.Document.Current.Hold == true)
			{
				this.Base.Lines.Cache.ForceExceptionHandling = true;

				foreach (GSBOQMatl line in BoqItems.Cache.Cached)
				{
					if (line.Selected == true)
					{ 
						RQRequestLine newline = new RQRequestLine();
						newline.InventoryID = line.InventoryID;
						newline.OrderQty = line.EstQtyPhase;
						newline.InventoryID = line.InventoryID;
						newline = this.Base.Lines.Insert(newline);
						var newlineExt = newline.GetExtension<RQRequestLineGSExt>();
						newlineExt.UsrBOQID = line.BOQID;
						newlineExt.UsrRevisionID = line.RevisionID;
						this.Base.Lines.Update(newline);
					}
				}
			}

			return adapter.Get();
		}

		public PXAction<RQRequest> MassUpdateSub;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Mass Update Sub Acc", Enabled = false)]
        protected virtual IEnumerable massUpdateSub(PXAdapter adapter)
        {
            var currentRequest = Base.Document.Current;
            var currentRequestExt = currentRequest.GetExtension<RQRequestGSExt>();
            if (currentRequestExt.UsrMassSubItem == null || currentRequestExt.UsrMassSubItem == 0)
                return adapter.Get(); ;
            foreach (RQRequestLine item in Base.Lines.Select())
            {
                item.ExpenseSubID = currentRequestExt.UsrMassSubItem;
                Base.Lines.Current = item;
                Base.Lines.Update(Base.Lines.Current);

            }
            Base.Save.Press();
            return adapter.Get();
        }


        public virtual RQRequest CreateEmptyRequestFrom(GSBOQ boq)
        {
            RQRequest boqReq = this.Base.Document.Insert();

            return boqReq;
        }

        public virtual RQRequest CreateRequestFromBOQ(GSBOQ boq, bool redirect = false)
        {

            CreateEmptyRequestFrom(boq);

            if (this.Base.CurrentDocument.Cache.IsDirty)
            {
                if (redirect)
                    throw new PXRedirectRequiredException(this.Base, "");
                else
                    return this.Base.CurrentDocument.Current;
            }

            throw new PXException("");
        }

    }
}
