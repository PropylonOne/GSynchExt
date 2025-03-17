using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.PM;
using PX.SM;
using PX.Objects.IN;
using System.Linq;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.FA.FABookSettings.midMonthType;
using System.Collections;
using System;
using static PX.Objects.FS.FSContractPeriodDet;

namespace PX.Objects.PO
{
    public class ISGAPOScheduleDeliveryEntry : PXGraph<ISGAPOScheduleDeliveryEntry>
    {
        #region Views
        public PXFilter<ISGAPOSchedule> Filter;
        public PXSelect<ISGAPOScheduleDelivery,
                     Where<ISGAPOScheduleDelivery.orderNbr, Equal<Current<ISGAPOSchedule.orderNbr>>,
                     And<ISGAPOScheduleDelivery.orderType, Equal<Current<ISGAPOSchedule.orderType>>,
                     And<ISGAPOScheduleDelivery.scheduleNbr, Equal<Current<ISGAPOSchedule.scheduleNbr>>,
                     And<ISGAPOScheduleDelivery.pOlineNbr, Equal<Current<ISGAPOSchedule.pOlineNbr>>>>>>,
                     OrderBy<Asc<ISGAPOSchedule.scheduleNbr, Asc<ISGAPOScheduleDelivery.pOlineNbr>>>> POScheduleDeliveryDetails;
        public PXSave<ISGAPOSchedule> Save;
        #endregion
        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDecimal(MinValue = 0)]
        [PXDependsOnFields(typeof(ISGAPOSchedule.orderNbr), typeof(ISGAPOSchedule.orderType), typeof(ISGAPOSchedule.pOlineNbr))]
        [PXUnboundDefault(typeof(Search<POLine.orderedQty, Where<POLine.orderNbr, Equal<Current<ISGAPOSchedule.orderNbr>>, And<POLine.orderType, Equal<Current<ISGAPOSchedule.orderType>>, And<POLine.lineNbr, Equal<Current<ISGAPOSchedule.pOlineNbr>>>>>>))]
        [PXFormula(typeof(Search<POLine.orderedQty, Where<POLine.orderNbr, Equal<Current<ISGAPOSchedule.orderNbr>>, And<POLine.orderType, Equal<Current<ISGAPOSchedule.orderType>>, And<POLine.lineNbr, Equal<Current<ISGAPOSchedule.pOlineNbr>>>>>>))]
        [PXUIField(DisplayName = "PO Line Quantity", Enabled = false)]
        public virtual void _(Events.CacheAttached<ISGAPOSchedule.pOLineQty> e) { }
        #endregion
        #region Methods
        public PXAction<ISGAPOSchedule> redirectToSchedule;
        [PXButton(CommitChanges = true, Connotation = Data.WorkflowAPI.ActionConnotation.Success)]
        [PXUIField(DisplayName = "Go to PO Schedule")]
        protected virtual IEnumerable RedirectToSchedule(PXAdapter adapter)
        {
            if (Filter.Current == null) return adapter.Get();

            ISGAPOScheduleEntry graph = PXGraph.CreateInstance<ISGAPOScheduleEntry>();
            ISGAPOSchedule currentDelivery = this.Filter.Current;
            ISGAPOSchedule existingSchedule = PXSelect<ISGAPOSchedule, Where<ISGAPOSchedule.orderNbr, Equal<Required<ISGAPOSchedule.orderNbr>>,
                                                                                 And<ISGAPOSchedule.orderType, Equal<Required<ISGAPOSchedule.orderType>>>>>.Select(graph, currentDelivery.OrderNbr, currentDelivery.OrderType);
            

            if (existingSchedule != null)
            {
                graph.Schedule.Current.OrderNbr = currentDelivery.OrderNbr;
                graph.Schedule.Current.OrderType = currentDelivery.OrderType;
                throw new PXRedirectRequiredException(graph, Messages.Document);
            }
            else
            {
                throw new PXException(GSynchExt.Messages.NOPOScheduleDelivery);
            }
        }
        public ISGAPOScheduleDelivery CreateScheduleDeliveryFromPOSchedule(ISGAPOSchedule schedule)

        {
            CreateScheduleDelivery(schedule);

            throw new PXRedirectRequiredException(this, "");
        }
        public virtual void CreateScheduleDelivery(ISGAPOSchedule schedule)
        {

            if (schedule == null) return;


            Filter.Current.OrderNbr = schedule.OrderNbr;
            Filter.Current.OrderType = schedule.OrderType;
            Filter.Current.POLineNbr = schedule.POLineNbr;
            Filter.Current.ScheduleNbr = schedule.ScheduleNbr;
            Filter.Current.ShipmentQty = schedule.ShipmentQty;

            var lines = PXSelect<POLine,
                                             Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                             And<POLine.orderType, Equal<Required<POLine.orderType>>>>>.Select(this, schedule.OrderNbr, schedule.OrderType);
            ISGAPOSchedule maxSchedule = PXSelectGroupBy<ISGAPOSchedule,
                                               Where<ISGAPOSchedule.orderNbr, Equal<Required<ISGAPOSchedule.orderNbr>>,
                                               And<ISGAPOSchedule.orderType, Equal<Required<ISGAPOSchedule.orderType>>>>,
                                               Aggregate<GroupBy<ISGAPOSchedule.scheduleNbr, Max<ISGAPOSchedule.scheduleNbr>>>>.Select(this, schedule.OrderNbr, schedule.OrderType);
            ISGAPOScheduleDelivery deliveryLine = new ISGAPOScheduleDelivery();
            deliveryLine.OrderNbr = schedule.OrderNbr;
            deliveryLine.OrderType = schedule.OrderType;
            deliveryLine.POLineNbr = schedule.POLineNbr;
            deliveryLine.ScheduleNbr = schedule.ScheduleNbr;
            deliveryLine.DeliveryNbr = 1;
            deliveryLine.DeliveryQty = schedule.ShipmentQty;
            deliveryLine.UOM = schedule.UOM;
            deliveryLine.InventoryID = schedule.InventoryID;
            deliveryLine.ItemDescr = schedule.ItemDescr;
            POScheduleDeliveryDetails.Insert(deliveryLine);
            return;
        }
        public static decimal CalculateUnscheduledQty1(PXGraph graph, string orderNbr, string orderType, int? pOLineNbr, int? scheduleNbr)
        {
            decimal ScheduledQty = PXSelect<ISGAPOScheduleDelivery,
                Where<ISGAPOScheduleDelivery.orderNbr, Equal<Required<ISGAPOScheduleDelivery.orderNbr>>,
                    And<ISGAPOScheduleDelivery.orderType, Equal<Required<ISGAPOScheduleDelivery.orderType>>,
                    And<ISGAPOScheduleDelivery.pOlineNbr, Equal<Required<ISGAPOScheduleDelivery.pOlineNbr>>,
                    And<ISGAPOScheduleDelivery.scheduleNbr, Equal<Required<ISGAPOScheduleDelivery.scheduleNbr>>>>>>>
                .Select(graph, orderNbr, orderType, pOLineNbr, scheduleNbr)
                .RowCast<ISGAPOScheduleDelivery>()
                .Sum(x => x.DeliveryQty ?? 0m);
            ISGAPOSchedule schedule = ISGAPOSchedule.PK.Find(graph, orderNbr, orderType, scheduleNbr, pOLineNbr);
            return Math.Max(0, (schedule?.ShipmentQty ?? 0m) - ScheduledQty);
        }
        #endregion
        #region Event Handlers
        protected virtual void _(Events.FieldSelecting<ISGAPOScheduleDelivery, ISGAPOScheduleDelivery.unscheduledQty> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            e.ReturnValue = CalculateUnscheduledQty1(this, row.OrderNbr, row.OrderType, row.POLineNbr, row.ScheduleNbr);
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.pOLineQty> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            POLine line = POLine.PK.Find(this, row.OrderType, row.OrderNbr, row.POLineNbr);
            if (line == null) return;
            e.ReturnValue = line.OrderQty;
        }
        protected virtual void _(Events.RowInserting<ISGAPOScheduleDelivery> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            var allRows = POScheduleDeliveryDetails.Select().RowCast<ISGAPOScheduleDelivery>();
            decimal totalQty = allRows.Sum(r => r.DeliveryQty ?? 0);
            row.DeliveryQty = this.Filter.Current.ShipmentQty - totalQty;
        }
        protected virtual void _(Events.RowUpdating<ISGAPOScheduleDelivery> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            decimal shipmentQty = this.Filter.Current.ShipmentQty ?? 0m;
            var allRows = POScheduleDeliveryDetails.Cache.Inserted.RowCast<ISGAPOScheduleDelivery>();
            decimal totalQty = allRows.Sum(r => r.DeliveryQty ?? 0m);
            decimal newDeliveryQty = shipmentQty - totalQty;
            if (newDeliveryQty < 0)
            {
                newDeliveryQty = row.DeliveryQty ?? 0m;
            }
            row.DeliveryQty = newDeliveryQty;
        }
        protected virtual void _(Events.RowPersisting<ISGAPOScheduleDelivery> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            var allRows = POScheduleDeliveryDetails.Select().RowCast<ISGAPOScheduleDelivery>();
            decimal totalQty = allRows.Sum(r => r.DeliveryQty ?? 0);
            if (totalQty > this.Filter.Current.ShipmentQty)
            {
                e.Cache.RaiseExceptionHandling<ISGAPOScheduleDelivery.deliveryQty>(row, row.DeliveryQty, new PXSetPropertyException(row, GSynchExt.Messages.DeliveryQtyValidation));
            }
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOScheduleDelivery, ISGAPOScheduleDelivery.inventoryID> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            POLine line = POLine.PK.Find(this, row?.OrderType, row?.OrderNbr, row?.POLineNbr);
            e.ReturnValue = line?.InventoryID;
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOScheduleDelivery, ISGAPOScheduleDelivery.uOM> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            POLine line = POLine.PK.Find(this, row?.OrderType, row?.OrderNbr, row?.POLineNbr);

            e.ReturnValue = line?.UOM;
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOScheduleDelivery, ISGAPOScheduleDelivery.itemDescr> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;
            InventoryItem item = PXSelectJoin<InventoryItem, InnerJoin<POLine,
                          On<POLine.inventoryID, Equal<InventoryItem.inventoryID>>>,
                          Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                          And<POLine.orderType, Equal<Required<POLine.orderType>>,
                          And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.OrderNbr, row.OrderType, row.POLineNbr);
            e.ReturnValue = item?.Descr;
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOScheduleDelivery, ISGAPOScheduleDelivery.scheduleQty> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;

            decimal ScheduledQty = PXSelect<ISGAPOScheduleDelivery,
                Where<ISGAPOScheduleDelivery.orderNbr, Equal<Required<ISGAPOScheduleDelivery.orderNbr>>,
                    And<ISGAPOScheduleDelivery.orderType, Equal<Required<ISGAPOScheduleDelivery.orderType>>,
                    And<ISGAPOScheduleDelivery.pOlineNbr, Equal<Required<ISGAPOScheduleDelivery.pOlineNbr>>,
                    And<ISGAPOScheduleDelivery.scheduleNbr, Equal<Required<ISGAPOScheduleDelivery.scheduleNbr>>>>>>>
                .Select(this, row.OrderNbr, row.OrderType,row.POLineNbr, row.ScheduleNbr)
                .RowCast<ISGAPOScheduleDelivery>()
                .Sum(x => x.DeliveryQty ?? 0m);

            e.ReturnValue = ScheduledQty;
        }
        protected virtual void _(Events.RowDeleting<ISGAPOScheduleDelivery> e)
        {
            ISGAPOScheduleDelivery row = e.Row;
            if (row == null) return;

            bool remainingDeliveries = PXSelect<ISGAPOScheduleDelivery,
                Where<ISGAPOScheduleDelivery.scheduleNbr, Equal<Required<ISGAPOScheduleDelivery.scheduleNbr>>,
                And<ISGAPOScheduleDelivery.deliveryNbr, NotEqual<Required<ISGAPOScheduleDelivery.deliveryNbr>>>>>
                .Select(this, row.ScheduleNbr, row.DeliveryNbr).Any();
            if (!remainingDeliveries)
            {
                ISGAPOSchedule schedule = PXSelect<ISGAPOSchedule,
                    Where<ISGAPOSchedule.scheduleNbr, Equal<Required<ISGAPOSchedule.scheduleNbr>>>>
                    .Select(this, row.ScheduleNbr);

                if (schedule != null)
                {
                    this.Caches<ISGAPOSchedule>().Update(schedule);
                }
            }
        }
        #endregion
    }
}