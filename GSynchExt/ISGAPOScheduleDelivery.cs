using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;

namespace PX.Objects.PO
{
    [PXCacheName(GSynchExt.Messages.ISGAPOScheduleDelivery)]
    public class ISGAPOScheduleDelivery : PXBqlTable, IBqlTable
    {
        #region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        [Branch(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        #endregion
        #region OrderNbr
        [PXDBString(15, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "PO Number", Enabled = false)]
        [PXSelector(typeof(Search<POOrder.orderNbr>))]
        [PXDefault(typeof(Current<ISGAPOSchedule.orderNbr>))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion
        #region OrderType
        [PXDBString(2, IsKey = true, InputMask = "")]
        [PXUIField(DisplayName = "PO Type", Enabled = false)]
        [PXSelector(typeof(Search<POOrder.orderType>))]
        [PXDefault(typeof(Current<ISGAPOSchedule.orderType>))]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion
        #region ScheduleNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Schedule Number", Enabled = false)]
        [PXSelector(typeof(Search<ISGAPOSchedule.scheduleNbr>))]
        [PXDefault(typeof(Current<ISGAPOSchedule.scheduleNbr>))]
        public virtual int? ScheduleNbr { get; set; }
        public abstract class scheduleNbr : PX.Data.BQL.BqlInt.Field<scheduleNbr> { }
        #endregion
        #region POLineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "PO Line Nbr")]
        [PXSelector(typeof(Search<POLine.lineNbr,
                 Where<POLine.orderNbr, Equal<Current<orderNbr>>,
                 And<POLine.orderType, Equal<Current<orderType>>>>>),
                 typeof(POLine.lineNbr), typeof(POLine.orderQty), typeof(POLine.inventoryID),
                 SubstituteKey = typeof(POLine.lineNbr),
                 DescriptionField = typeof(POLine.lineNbr))]
        [PXDefault(typeof(Current<ISGAPOSchedule.pOlineNbr>))]

        public virtual int? POLineNbr { get; set; }
        public abstract class pOlineNbr : PX.Data.BQL.BqlInt.Field<pOlineNbr> { }
        #endregion
        #region DeliveryNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Delivery Number")]
        public virtual int? DeliveryNbr { get; set; }
        public abstract class deliveryNbr : PX.Data.BQL.BqlInt.Field<deliveryNbr> { }
        #endregion
        #region DeliveryQty
        [PXDBDecimal(MinValue = 0)]
        [PXUIField(DisplayName = "Delivery Qty")]
        public virtual decimal? DeliveryQty { get; set; }
        public abstract class deliveryQty : PX.Data.BQL.BqlDecimal.Field<deliveryQty> { }
        #endregion
        #region ShipmentQty
        [PXDecimal(MinValue = 0)]
        [PXUIField(DisplayName = "Shipment Qty")]
        public virtual decimal? ShipmentQty { get; set; }
        public abstract class shipmentQty : PX.Data.BQL.BqlDecimal.Field<shipmentQty> { }
        #endregion
        #region SiteID
        [PXDBInt()]
        [PXDefault(typeof(Search<POLine.siteID, Where<POLine.orderNbr, Equal<Current<orderNbr>>,
                                And<POLine.orderType, Equal<Current<orderType>>,
                                And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>),
                                PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Warehouse")]
        [PXSelector(typeof(Search<INSite.siteID>), SubstituteKey = typeof(INSite.siteCD))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlBool.Field<siteID> { }
        #endregion
        #region WarehouseArrivalDate
        [PXDBDate()]
        [PXDefault]
        [PXUIField(DisplayName = "Warehouse Arrival Date")]
        public virtual DateTime? WarehouseArrivalDate { get; set; }
        public abstract class warehouseArrivalDate : PX.Data.BQL.BqlDateTime.Field<warehouseArrivalDate> { }
        #endregion
        #region Done
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Done")]
        public virtual bool? Done { get; set; }
        public abstract class done : PX.Data.BQL.BqlBool.Field<done> { }
        #endregion
        #region InventoryID
        [PXInt]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey = typeof(InventoryItem.inventoryCD))]
        [PXDependsOnFields(typeof(orderNbr), typeof(orderType), typeof(pOlineNbr))]
        [PXUnboundDefault(typeof(Search<POLine.inventoryID, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        [PXFormula(typeof(Search<POLine.inventoryID, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion
        #region UOM
        [PXString(6, IsUnicode = true)]
        [PXDependsOnFields(typeof(orderNbr), typeof(orderNbr), typeof(pOlineNbr))]
        [PXUnboundDefault(typeof(Search<POLine.uOM, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        [PXFormula(typeof(Search<POLine.uOM, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        [PXUIField(DisplayName = "UOM", Enabled = false)]
        public virtual string UOM { get; set; }
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        #endregion
        #region ItemDescr
        [PXString(256, IsUnicode = true)]
        [PXDependsOnFields(typeof(orderNbr), typeof(orderNbr), typeof(pOlineNbr))]
        [PXUIField(DisplayName = "Item Description", Enabled = false)]
        public virtual string ItemDescr { get; set; }
        public abstract class itemDescr : PX.Data.BQL.BqlString.Field<itemDescr> { }
        #endregion
        #region POLineQty
        [PXDecimal(MinValue = 0)]
        [PXDependsOnFields(typeof(orderNbr), typeof(orderType), typeof(pOlineNbr))]
        [PXUnboundDefault(typeof(Search<POLine.orderedQty, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        [PXFormula(typeof(Search<POLine.orderedQty, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        [PXUIField(DisplayName = "PO Line Quantity", Enabled = false)]
        public virtual decimal? POLineQty { get; set; }
        public abstract class pOLineQty : PX.Data.BQL.BqlDecimal.Field<pOLineQty> { }
        #endregion
        #region ScheduleQty
        [PXDecimal]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Scheduled Quantity", Enabled = false)]
        public virtual decimal? ScheduleQty { get; set; }
        public abstract class scheduleQty : PX.Data.BQL.BqlDecimal.Field<scheduleQty> { }
        #endregion
        #region UnscheduledQty
        [PXDecimal]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Unscheduled Qty", Enabled = false)]
        public virtual decimal? UnscheduledQty { get; set; }
        public abstract class unscheduledQty : PX.Data.BQL.BqlDecimal.Field<unscheduledQty> { }
        #endregion
        #region Audit Fields
        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion
        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion
        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
        #endregion
    }
}
