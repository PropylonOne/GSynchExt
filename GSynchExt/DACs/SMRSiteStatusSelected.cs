using PX.Common;
using PX.Data;
using System;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.SO;
using PX.Objects.Common.Bql;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.FS;

namespace GSynchExt
{
	[PXProjection(typeof(Select<InventoryItem, Where<InventoryItem.stkItem,
           Equal<True>>>), Persistent = false)]

    public partial class SMRSiteStatusSelected : PXBqlTable, IBqlTable
	{
        #region Keys
        public static class FK
        {
			public class InventoryItem : PX.Objects.IN.InventoryItem.PK.ForeignKeyOf<SMRSiteStatusSelected>.By<inventoryID> { }
			public class PriceClass : INPriceClass.PK.ForeignKeyOf<SMRSiteStatusSelected>.By<priceClassID> { }
		}
        #endregion

        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion

		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		protected Int32? _InventoryID;
		[Inventory(BqlField = typeof(InventoryItem.inventoryID), IsKey = true)]
		[PXDefault()]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion

		#region InventoryCD
		public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
		protected string _InventoryCD;
		[PXDefault()]
		[InventoryRaw(BqlField = typeof(InventoryItem.inventoryCD))]
		public virtual String InventoryCD
		{
			get
			{
				return this._InventoryCD;
			}
			set
			{
				this._InventoryCD = value;
			}
		}
		#endregion

		#region Descr
		public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

		protected string _Descr;
		[PXDBLocalizableString(PX.Objects.Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(InventoryItem.descr), IsProjection = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion

	


		#region ItemType
		public abstract class itemType : PX.Data.BQL.BqlString.Field<itemType> { }
		protected String _ItemType;

		/// <summary>
		/// The type of the Inventory Item.
		/// </summary>
		/// <value>
		/// Possible values are:
		/// <c>"F"</c> - Finished Good (Stock Items only),
		/// <c>"M"</c> - Component Part (Stock Items only),
		/// <c>"A"</c> - Subassembly (Stock Items only),
		/// <c>"N"</c> - Non-Stock Item (a general type of Non-Stock Item),
		/// <c>"L"</c> - Labor (Non-Stock Items only),
		/// <c>"S"</c> - Service (Non-Stock Items only),
		/// <c>"C"</c> - Charge (Non-Stock Items only),
		/// <c>"E"</c> - Expense (Non-Stock Items only).
		/// Defaults to the <see cref="INItemClass.ItemType">Type</see> associated with the <see cref="ItemClassID">Item Class</see>
		/// of the item if it's specified, or to Finished Good (<c>"F"</c>) otherwise.
		/// </value>
		[PXDBString(1, IsFixed = true, BqlField = typeof(InventoryItem.itemType))]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
		[INItemTypes.List()]
		public virtual String ItemType
		{
			get
			{
				return this._ItemType;
			}
			set
			{
				this._ItemType = value;
			}
		}
		#endregion

		#region PriceClassID
		public abstract class priceClassID : PX.Data.BQL.BqlString.Field<priceClassID> { }

		protected string _PriceClassID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.priceClassID))]
		[PXUIField(DisplayName = "Price Class ID", Visible = false)]
		public virtual String PriceClassID
		{
			get
			{
				return this._PriceClassID;
			}
			set
			{
				this._PriceClassID = value;
			}
		}
		#endregion


	

		#region BaseUnit
		public abstract class baseUnit : PX.Data.BQL.BqlString.Field<baseUnit> { }

		protected string _BaseUnit;
		[INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible, BqlField = typeof(InventoryItem.baseUnit))]
		public virtual String BaseUnit
		{
			get
			{
				return this._BaseUnit;
			}
			set
			{
				this._BaseUnit = value;
			}
		}
		#endregion


		#region SalesUnit
		public abstract class salesUnit : PX.Data.BQL.BqlString.Field<salesUnit> { }
		protected string _SalesUnit;
		[INUnit(typeof(SMRSiteStatusSelected.inventoryID), DisplayName = "Sales Unit", BqlField = typeof(InventoryItem.salesUnit))]
		public virtual String SalesUnit
		{
			get
			{
				return this._SalesUnit;
			}
			set
			{
				this._SalesUnit = value;
			}
		}
		#endregion

		#region QtySelected
		public abstract class qtySelected : PX.Data.BQL.BqlDecimal.Field<qtySelected> { }
		protected Decimal? _QtySelected;
		[PXQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Selected")]
		public virtual Decimal? QtySelected
		{
			get
			{
				return this._QtySelected ?? 0m;
			}
			set
			{
				if (value != null && value != 0m)
					this._Selected = true;
				this._QtySelected = value;
			}
		}
		#endregion		
	}
}

