using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GSynchExt;
using PX.Api;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.PO.ISGAPOScheduleEntry;

namespace PX.Objects.PO
{
    public class ISGAPOScheduleEntry : PXGraph<ISGAPOScheduleEntry>
    {
        #region Views
        public PXFilter<POOrder> Schedule;
        public PXSave<POOrder> Save;
        public PXCancel<POOrder> CancelClose;
        [PXImport]
        public PXSelect<ISGAPOSchedule,
             Where<ISGAPOSchedule.orderNbr, Equal<Current<POOrder.orderNbr>>,
             And<ISGAPOSchedule.orderType, Equal<Current<POOrder.orderType>>>>,
             OrderBy<Asc<ISGAPOSchedule.scheduleNbr, Asc<ISGAPOSchedule.pOlineNbr>>>> ScheduleDetails;
        #endregion
        #region Actions
        public PXAction<ISGAPOSchedule> addDelivery;
        [PXButton(CommitChanges = true, SpecialType = PXSpecialButtonType.Process)]
        [PXUIField(DisplayName = "Add Delivery")]
        protected virtual IEnumerable AddDelivery(PXAdapter adapter)
        {
            POOrder currentSchedule = Schedule.Current;
            if (Schedule.Current == null) return adapter.Get();

            ISGAPOSchedule scheduleLine             = ScheduleDetails.Current;
            ISGAPOScheduleDeliveryEntry graph       = PXGraph.CreateInstance<ISGAPOScheduleDeliveryEntry>();
            ISGAPOScheduleDelivery existingDelivery = graph.POScheduleDeliveryDetails.Select().Cast<ISGAPOScheduleDelivery>().Where(x => x.OrderNbr == scheduleLine.OrderNbr
                                                                                                                                   && x.OrderType   == scheduleLine.OrderType
                                                                                                                                   && x.POLineNbr   == scheduleLine.POLineNbr
                                                                                                                                   && x.ScheduleNbr == scheduleLine.ScheduleNbr).FirstOrDefault();
            if (existingDelivery != null)
            {
                throw new PXRedirectRequiredException(graph, Messages.Document);
            }
            else
            {
                if (scheduleLine == null)
                {
                    throw new PXException(GSynchExt.Messages.ShipmentSelectValidation1);
                }
                else
                {
                    PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<ISGAPOScheduleDeliveryEntry>().CreateScheduleDeliveryFromPOSchedule(scheduleLine));
                }
            }
            return adapter.Get();
        }
        public PXAction<POOrder> redirectToDelivery;
        [PXButton(CommitChanges = true, Connotation = Data.WorkflowAPI.ActionConnotation.Success)]
        [PXUIField(DisplayName = "Go to PO Delivery")]
        protected virtual IEnumerable RedirectToDelivery(PXAdapter adapter)
        {
            if (Schedule.Current == null) return adapter.Get();
            ISGAPOScheduleDeliveryEntry graph = PXGraph.CreateInstance<ISGAPOScheduleDeliveryEntry>();
            ISGAPOSchedule currentScheduleLine = this.ScheduleDetails.Current;
            ISGAPOScheduleDelivery existingDelivery = PXSelect<ISGAPOScheduleDelivery, Where<ISGAPOScheduleDelivery.orderNbr, Equal<Required<ISGAPOScheduleDelivery.orderNbr>>,
                                                        And<ISGAPOScheduleDelivery.orderType, Equal<Required<ISGAPOScheduleDelivery.orderType>>,
                                                        And<ISGAPOScheduleDelivery.scheduleNbr, Equal<Required<ISGAPOScheduleDelivery.scheduleNbr>>,
                                                        And<ISGAPOScheduleDelivery.pOlineNbr, Equal<Required<ISGAPOScheduleDelivery.pOlineNbr>>>>>>>.Select(graph, currentScheduleLine.OrderNbr, currentScheduleLine.OrderType, currentScheduleLine.ScheduleNbr, currentScheduleLine.POLineNbr);
            if (existingDelivery != null)
            {
                graph.Filter.Current.OrderNbr = currentScheduleLine.OrderNbr;
                graph.Filter.Current.OrderType = currentScheduleLine.OrderType;
                graph.Filter.Current.ScheduleNbr = currentScheduleLine.ScheduleNbr;
                graph.Filter.Current.POLineNbr = currentScheduleLine.POLineNbr;
                throw new PXRedirectRequiredException(graph, Messages.Document);
            }
            else
            {
                throw new PXException(GSynchExt.Messages.NOPOSchedule);
            }
        }
        #endregion
        #region Protected Methods
        public static decimal CalculateUnscheduledQty(PXGraph graph, string orderNbr, string orderType, int? poLineNbr, decimal? poLineQty)
        {
            POLine pOLine = PXSelect<POLine, Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>, And<POLine.orderType, Equal<Required<POLine.orderType>>,
                And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(graph, orderNbr, orderType, poLineNbr);

            decimal totalScheduledQty = PXSelect<ISGAPOSchedule,
                Where<ISGAPOSchedule.orderNbr, Equal<Required<ISGAPOSchedule.orderNbr>>,
                    And<ISGAPOSchedule.orderType, Equal<Required<ISGAPOSchedule.orderType>>,
                    And<ISGAPOSchedule.pOlineNbr, Equal<Required<ISGAPOSchedule.pOlineNbr>>>>>>
                .Select(graph, orderNbr, orderType, poLineNbr)
                .RowCast<ISGAPOSchedule>()
                .Sum(x=> x.ShipmentQty ?? 0m);
            return Math.Max(0, (pOLine?.OrderQty ?? 0m) - totalScheduledQty);
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.unscheduledQty> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            e.ReturnValue = CalculateUnscheduledQty(this, row.OrderNbr, row.OrderType, row.POLineNbr, row.POLineQty);
        }
        public ISGAPOSchedule CreatePOScheduleFromPOLine(POOrder order, bool redirect)
        {
            CreatePOSchedule(order);
            throw new PXRedirectRequiredException(this, "");
        }
        public virtual void CreatePOSchedule(POOrder schedule)
        {
            if (schedule == null) return;
            this.Schedule.Current.OrderNbr       = schedule.OrderNbr;
            this.Schedule.Current.OrderType      = schedule.OrderType;
            this.Schedule.Current.CuryOrderTotal = schedule.CuryOrderTotal;

            var lines    = PXSelect<POLine,
                                               Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                               And<POLine.orderType, Equal<Required<POLine.orderType>>>>>.Select(this, schedule.OrderNbr, schedule.OrderType);
            ISGAPOSchedule maxSchedule = PXSelectGroupBy<ISGAPOSchedule,
                                               Where<ISGAPOSchedule.orderNbr, Equal<Required<ISGAPOSchedule.orderNbr>>,
                                               And<ISGAPOSchedule.orderType, Equal<Required<ISGAPOSchedule.orderType>>>>,
                                               Aggregate<GroupBy<ISGAPOSchedule.scheduleNbr, Max<ISGAPOSchedule.scheduleNbr>>>>.Select(this, schedule.OrderNbr, schedule.OrderType);
            int? nextScheduleNbr = maxSchedule?.ScheduleNbr ?? 0 + 1;
            foreach (POLine record in lines)
            {
                ISGAPOSchedule scheduleLine = PXSelect<ISGAPOSchedule,
                                   Where<ISGAPOSchedule.orderNbr, Equal<Required<ISGAPOSchedule.orderNbr>>,
                                   And<ISGAPOSchedule.orderType, Equal<Required<ISGAPOSchedule.orderType>>,
                                   And<ISGAPOSchedule.pOlineNbr, Equal<Required<ISGAPOSchedule.pOlineNbr>>>>>>.Select(this, record.OrderNbr, record.OrderType, record.LineNbr);
                InventoryItem item = PXSelectJoin<InventoryItem, InnerJoin<POLine,
                                   On<POLine.inventoryID, Equal<InventoryItem.inventoryID>>>,
                                   Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                   And<POLine.orderType, Equal<Required<POLine.orderType>>,
                                   And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, record.OrderNbr, record.OrderType, record.POLineNbr);

                if (scheduleLine != null) continue;
                scheduleLine = new ISGAPOSchedule();
                scheduleLine.OrderNbr = record.OrderNbr;
                scheduleLine.OrderType = record.OrderType;
                scheduleLine.InventoryID = record.InventoryID;
                scheduleLine.ItemDescr = item?.Descr;
                scheduleLine.UOM = record.UOM;
                scheduleLine.POLineNbr = record.LineNbr;
                scheduleLine.POLineQty = record.OrderQty;
                scheduleLine.ShipmentQty = record.OrderQty;
                scheduleLine.BranchID = record.BranchID;
                scheduleLine.ScheduleNbr = nextScheduleNbr;
                ScheduleDetails.Insert(scheduleLine);
            }
        }
        #endregion
        #region Event Handlers
        protected virtual void _(Events.FieldUpdated<ISGAPOSchedule, ISGAPOSchedule.pOlineNbr> e)
        {
            ISGAPOSchedule schedule = e.Row;
            POLine line = POLine.PK.Find(this, schedule.OrderType, schedule.OrderNbr, (int)e.NewValue);
            schedule.InventoryID = line?.InventoryID;
            schedule.POLineQty = line?.OrderQty;
        }
        protected virtual void _(Events.RowInserting<ISGAPOSchedule> e)
        {
            ISGAPOSchedule row = e.Row;
            POOrder currentFilter = Schedule.Current;
            row.OrderNbr = currentFilter.OrderNbr;
            row.OrderType = currentFilter.OrderType;
            if (row == null) return;
            var scheduleLines = ScheduleDetails.Select().RowCast<ISGAPOSchedule>().Where(r => r.POLineNbr == row.POLineNbr);
            decimal scheduledPOLineQty = scheduleLines.Sum(r => r.ShipmentQty ?? 0);
            row.ShipmentQty = row.POLineQty - scheduledPOLineQty;
        }
        protected virtual void _(Events.RowPersisting<ISGAPOSchedule> e)
        {
            ISGAPOSchedule row = e.Row;
            POOrder currentFilter = Schedule.Current;
            var scheduleLines = ScheduleDetails.Select().RowCast<ISGAPOSchedule>().Where(r => r.POLineNbr == row.POLineNbr);
            decimal scheduledPOLineQty = scheduleLines.Sum(r => r.ShipmentQty ?? 0);
            if (scheduledPOLineQty > row.POLineQty)
            {
                e.Cache.RaiseExceptionHandling<ISGAPOSchedule.shipmentQty>(row, row.ShipmentQty, new PXSetPropertyException(row, GSynchExt.Messages.ShipmentQtyValidation));
            }
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.pOLineQty> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            POLine line = POLine.PK.Find(this, row.OrderType, row.OrderNbr, row.POLineNbr);
            e.ReturnValue = line.OrderQty;
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.inventoryID> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            POLine line = POLine.PK.Find(this, row?.OrderType, row?.OrderNbr, row?.POLineNbr);
            e.ReturnValue = line?.InventoryID;
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.uOM> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            POLine line = POLine.PK.Find(this, row?.OrderType, row?.OrderNbr, row?.POLineNbr);
            e.ReturnValue = line?.UOM;
        }
        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.itemDescr> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            InventoryItem item = PXSelectJoin<InventoryItem, InnerJoin<POLine,
                                     On<POLine.inventoryID, Equal<InventoryItem.inventoryID>>>,
                                     Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                     And<POLine.orderType, Equal<Required<POLine.orderType>>,
                                     And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(this, row.OrderNbr, row.OrderType, row.POLineNbr);
            e.ReturnValue = item?.Descr;
        }


        protected virtual void _(Events.FieldSelecting<ISGAPOSchedule, ISGAPOSchedule.scheduleQty> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            decimal totalScheduledQty = PXSelect<ISGAPOSchedule,
                Where<ISGAPOSchedule.orderNbr, Equal<Required<ISGAPOSchedule.orderNbr>>,
                    And<ISGAPOSchedule.orderType, Equal<Required<ISGAPOSchedule.orderType>>,
                    And<ISGAPOSchedule.pOlineNbr, Equal<Required<ISGAPOSchedule.pOlineNbr>>>>>>
                .Select(this, row.OrderNbr, row.OrderType, row.POLineNbr)
                .RowCast<ISGAPOSchedule>()
                .Sum(x => x.ShipmentQty ?? 0m);
            e.ReturnValue = totalScheduledQty;
        }
        protected virtual void _(Events.RowDeleting<ISGAPOSchedule> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
                ISGAPOScheduleDelivery existingDelivery = PXSelect < ISGAPOScheduleDelivery,
                Where< ISGAPOScheduleDelivery.orderNbr, Equal<Required<ISGAPOScheduleDelivery.orderNbr>>,
                    And < ISGAPOScheduleDelivery.orderType, Equal<Required<ISGAPOScheduleDelivery.orderType>>,
                    And < ISGAPOScheduleDelivery.scheduleNbr, Equal<Required<ISGAPOScheduleDelivery.scheduleNbr>>,
                    And < ISGAPOScheduleDelivery.pOlineNbr, Equal<Required<ISGAPOScheduleDelivery.pOlineNbr>>,
                    And < ISGAPOScheduleDelivery.deliveryNbr, IsNotNull >>>>>> 
                    .Select(this, row.OrderNbr, row.OrderType, row.ScheduleNbr, row.POLineNbr);
            if (existingDelivery != null &&
                this.Caches<ISGAPOScheduleDelivery>().GetStatus(existingDelivery) != PXEntryStatus.Deleted)
            {
                throw new PXException(GSynchExt.Messages.DeliveriesAssigned);
            }
        }
        protected virtual void _(Events.RowUpdating<ISGAPOSchedule> e)
        {
            ISGAPOSchedule row = e.Row;
            if (row == null) return;
            ISGAPOScheduleDelivery existingDelivery = PXSelect < ISGAPOScheduleDelivery,
                Where< ISGAPOScheduleDelivery.orderNbr, Equal<Required<ISGAPOScheduleDelivery.orderNbr>>,
                    And < ISGAPOScheduleDelivery.orderType, Equal<Required<ISGAPOScheduleDelivery.orderType>>,
                    And < ISGAPOScheduleDelivery.scheduleNbr, Equal<Required<ISGAPOScheduleDelivery.scheduleNbr>>,
                    And < ISGAPOScheduleDelivery.pOlineNbr, Equal<Required<ISGAPOScheduleDelivery.pOlineNbr>>,
                    And < ISGAPOScheduleDelivery.deliveryNbr, IsNotNull >>>>>> 
                    .Select(this, row.OrderNbr, row.OrderType, row.ScheduleNbr, row.POLineNbr);
            if (existingDelivery != null) 
            {
                throw new PXException(GSynchExt.Messages.ScheduleCannotBeModified);
            }
        }
        #endregion
    }
}