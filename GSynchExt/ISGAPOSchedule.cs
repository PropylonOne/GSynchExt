using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.PO;
using static PX.Objects.PO.ISGAPOScheduleEntry;
using PX.Objects.IN;
using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.PO
{
    [PXCacheName(GSynchExt.Messages.ISGAPOSchedule)]
    public class ISGAPOSchedule : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<ISGAPOSchedule>.By<orderNbr, orderType, pOlineNbr, scheduleNbr>
        {
            public static ISGAPOSchedule Find(PXGraph graph, string orderNbr, string orderType, int? scheduleNbr, int? pOLineNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, orderNbr, orderType, pOLineNbr, scheduleNbr);
        }
        public static class FK
        {
            public class Order : POOrder.PK.ForeignKeyOf<POOrder>.By<orderNbr, orderType>{ }
        }
        #endregion
        #region OrderNbr
        [PXDBString(15, IsKey = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        [PXSelector(typeof(Search<POOrder.orderNbr>))]
        [PXDefault(typeof(Current<POOrder.orderNbr>))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion
        #region OrderType
        [PXDBString(2, IsKey = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Type")]
        [PXSelector(typeof(Search<POOrder.orderType>))]
        [PXDefault(typeof(Current<POOrder.orderType>))]

        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion
        #region BranchID
        [GL.Branch(typeof(AccessInfo.branchID), IsDetail = false, TabOrder = 0)]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion
        #region POLineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "PO Line Nbr")]
        [PXSelector(typeof(Search<POLine.lineNbr, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>>>>), typeof(POLine.lineNbr), typeof(POLine.orderQty), typeof(POLine.inventoryID))]
        public virtual int? POLineNbr { get; set; }
        public abstract class pOlineNbr : PX.Data.BQL.BqlInt.Field<pOlineNbr> { }
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
        [PXString]
        [PXUIField(DisplayName = "UOM", Enabled = false)]
        [PXDependsOnFields(typeof(orderNbr), typeof(orderNbr), typeof(pOlineNbr))]
        [PXUnboundDefault(typeof(Search<POLine.uOM, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        [PXFormula(typeof(Search<POLine.uOM, Where<POLine.orderNbr, Equal<Current<orderNbr>>, And<POLine.orderType, Equal<Current<orderType>>, And<POLine.lineNbr, Equal<Current<pOlineNbr>>>>>>))]
        public virtual string UOM { get; set; }
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        #endregion
        #region ItemDescr
        [PXString]
        [PXUIField(DisplayName = "Item Description", Enabled = false)]
        [PXDependsOnFields(typeof(orderNbr), typeof(orderNbr), typeof(pOlineNbr))]
        public virtual string ItemDescr { get; set; }
        public abstract class itemDescr : PX.Data.BQL.BqlString.Field<itemDescr> { }
        #endregion
        #region ScheduleNbr
        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Schedule Number")]
        public virtual int? ScheduleNbr { get; set; }
        public abstract class scheduleNbr : PX.Data.BQL.BqlInt.Field<scheduleNbr> { }
        #endregion
        #region ShipmentQty
        [PXDBDecimal(MinValue = 0)]
        [PXUIField(DisplayName = "Shipment Quantity")]
        public virtual decimal? ShipmentQty { get; set; }
        public abstract class shipmentQty : PX.Data.BQL.BqlDecimal.Field<shipmentQty> { }
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
        [PXDecimal(MinValue = 0)]
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
        #region FactoryDeliveryDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Factory Delivery Date")]
        public virtual DateTime? FactoryDeliveryDate { get; set; }
        public abstract class factoryDeliveryDate : PX.Data.BQL.BqlDateTime.Field<factoryDeliveryDate> { }
        #endregion
        #region FactoryDeliveryDone
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Factory Delivery Done")]
        public virtual bool? FactoryDeliveryDone { get; set; }
        public abstract class factoryDeliveryDone : PX.Data.BQL.BqlBool.Field<factoryDeliveryDone> { }
        #endregion
        #region PortArrivalDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Port Arrival Date")]
        public virtual DateTime? PortArrivalDate { get; set; }
        public abstract class portArrivalDate : PX.Data.BQL.BqlDateTime.Field<portArrivalDate> { }
        #endregion
        #region PortArrivalDone
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Port Arrival Done")]
        public virtual bool? PortArrivalDone { get; set; }
        public abstract class portArrivalDone : PX.Data.BQL.BqlBool.Field<portArrivalDone> { }
        #endregion
        #region ClearanceDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Clearance Date")]
        public virtual DateTime? ClearanceDate { get; set; }
        public abstract class clearanceDate : PX.Data.BQL.BqlDateTime.Field<clearanceDate> { }
        #endregion
        #region ClearanceDone
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Clearance Done")]
        public virtual bool? ClearanceDone { get; set; }
        public abstract class clearanceDone : PX.Data.BQL.BqlBool.Field<clearanceDone> { }
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