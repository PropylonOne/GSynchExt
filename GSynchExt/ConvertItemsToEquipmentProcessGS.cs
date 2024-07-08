using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.CT;
using System;
using System.Collections.Generic;
using System.Linq;
using GSynchExt;

namespace PX.Objects.FS
{
    public class ConvertItemsToEquipmentProcessGS : PXGraph<ConvertItemsToEquipmentProcessGS>
    {
        public ConvertItemsToEquipmentProcessGS()
        {
            SMEquipmentMaint graphSMEquipmentMaint;

            InventoryItems.SetProcessDelegate(
                delegate (List<ProjIssuedEQInventoryItem> inventoryItemRows)
                {
                    graphSMEquipmentMaint = CreateInstance<SMEquipmentMaint>();

                    bool error = false;

                    var groupedProjList = inventoryItemRows
                                        .Select((x, i) => new { Value = x, Index = i })
                                        .GroupBy(row => new { row.Value.DocType, row.Value.RefNbr, row.Value.LineNbr, row.Value.INLineSplitNumber })
                                        .Select(grp => grp.ToList())
                                        .ToList();

                    for (int i = 0; i < groupedProjList.Count; i++)
                    {
                        ProjIssuedEQInventoryItem projInventoryItemRow = groupedProjList[i].First().Value;
                        error = false;
                        INTran inTranRow = INTran.PK.Find(graphSMEquipmentMaint, INDocType.Issue, projInventoryItemRow.RefNbr, projInventoryItemRow.LineNbr);
                        InventoryItem inventoryItemRow = SharedFunctions.GetInventoryItemRow(graphSMEquipmentMaint, projInventoryItemRow.InventoryID);
                        ProjIssuedEQInventoryItem newprojInventoryItemRow = (ProjIssuedEQInventoryItem)InventoryItems.Cache.CreateCopy(projInventoryItemRow);
                        CreateIssuedEquipment(graphSMEquipmentMaint, newprojInventoryItemRow, inTranRow, inventoryItemRow);

                        if (error == false)
                        {
                            foreach (var row in groupedProjList[i])
                            {
                                PXProcessing<ProjIssuedEQInventoryItem>.SetInfo(row.Index, TX.Messages.RECORD_PROCESSED_SUCCESSFULLY);
                            }
                        }
                    }
                });
        }
        public static FSEquipment CreateIssuedEquipment(SMEquipmentMaint graphSMEquipmentMaint, ProjIssuedEQInventoryItem projInventoryItemRow, INTran inTranRow, InventoryItem inventoryItemRow)
        {
            FSEquipment fsEquipmentRow = new FSEquipment();
            /// Sets Ownership to the Provincial Company
            var project = PMProject.PK.Find(graphSMEquipmentMaint, projInventoryItemRow.ProjectID);
            var solarsite = SolarSite.PK.Find(graphSMEquipmentMaint, projInventoryItemRow.SolarSiteID);
            string province = solarsite?.Province;
            if (solarsite?.Province == "UVA")
                province = "U";

            String custbranchCD = String.Concat("GG", province);
            int? cust = null;
            if (project != null)
            {
                cust = project.CustomerID ?? Customer.UK.Find(graphSMEquipmentMaint, custbranchCD)?.BAccountID; 
            }
            fsEquipmentRow.OwnerType = ID.OwnerType_Equipment.CUSTOMER;
            fsEquipmentRow.OwnerID = cust;
            fsEquipmentRow.RequireMaintenance = true;

            /// Sets the location
            fsEquipmentRow.LocationType = ID.LocationType.COMPANY;
            fsEquipmentRow.BranchID = GL.Branch.UK.Find(graphSMEquipmentMaint, solarsite.Province)?.BranchID;
            fsEquipmentRow.BranchLocationID = FSBranchLocation.UK.Find(graphSMEquipmentMaint, projInventoryItemRow.SolarSiteCD)?.BranchLocationID;



            fsEquipmentRow.SiteID = projInventoryItemRow.SiteID;

            if (inventoryItemRow != null)
            {
                FSxEquipmentModel fsxEquipmentModelRow = PXCache<InventoryItem>.GetExtension<FSxEquipmentModel>(inventoryItemRow);
                fsEquipmentRow.EquipmentTypeID = fsxEquipmentModelRow?.EquipmentTypeID;
            }

            //Lot/Serial Info.
            fsEquipmentRow.INSerialNumber = projInventoryItemRow.LotSerialNumber;
            fsEquipmentRow.SerialNumber = projInventoryItemRow.LotSerialNumber;

            //Source info.
            fsEquipmentRow.SourceType = ID.SourceType_Equipment.EP_EQUIPMENT;
            fsEquipmentRow.SourceDocType = INDocType.Issue;
            fsEquipmentRow.SourceRefNbr = projInventoryItemRow.RefNbr;
            fsEquipmentRow.ARTranLineNbr = projInventoryItemRow.LineNbr;
            //Installation Info

            fsEquipmentRow = graphSMEquipmentMaint.EquipmentRecords.Insert(fsEquipmentRow);
            graphSMEquipmentMaint.EquipmentRecords.SetValueExt<FSEquipment.inventoryID>(fsEquipmentRow, projInventoryItemRow.InventoryID);
            graphSMEquipmentMaint.EquipmentRecords.SetValueExt<FSEquipment.dateInstalled>(fsEquipmentRow, projInventoryItemRow.DocDate);
            //graphSMEquipmentMaint.EquipmentRecords.SetValueExt<FSEquipment.salesDate>(fsEquipmentRow, projInventoryItemRow.DocDate );
            graphSMEquipmentMaint.EquipmentRecords.SetValueExt<FSEquipment.descr>(fsEquipmentRow, (projInventoryItemRow.Descr == null) ? projInventoryItemRow.InventoryCD : projInventoryItemRow.Descr);

            if (projInventoryItemRow.Descr != null)
            {
                object inventoryItem = PXSelectorAttribute.Select<FSEquipment.inventoryID>(graphSMEquipmentMaint.EquipmentRecords.Cache, graphSMEquipmentMaint.EquipmentRecords.Current);
                PXDBLocalizableStringAttribute.CopyTranslations<InventoryItem.descr, FSEquipment.descr>(graphSMEquipmentMaint, inventoryItem, fsEquipmentRow);
            }

            //Attributes
            graphSMEquipmentMaint.Answers.CopyAllAttributes(graphSMEquipmentMaint.EquipmentRecords.Current, inventoryItemRow);

            //Image
            PXNoteAttribute.CopyNoteAndFiles(graphSMEquipmentMaint.Caches[typeof(InventoryItem)], inventoryItemRow, graphSMEquipmentMaint.Caches[typeof(FSEquipment)], graphSMEquipmentMaint.EquipmentRecords.Current, false, true);
            fsEquipmentRow.ImageUrl = inventoryItemRow.ImageUrl;

            graphSMEquipmentMaint.Save.Press();
            fsEquipmentRow = graphSMEquipmentMaint.EquipmentRecords.Current;

            if (fsEquipmentRow != null && inTranRow != null )
            {
                foreach (FSEquipmentComponent fsEquipmentComponentRow in graphSMEquipmentMaint.EquipmentWarranties.Select())
                {
                    fsEquipmentComponentRow.InvoiceRefNbr = projInventoryItemRow.RefNbr;
                    graphSMEquipmentMaint.EquipmentWarranties.Update(fsEquipmentComponentRow);
                    graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.installationDate>(fsEquipmentComponentRow, projInventoryItemRow.DocDate);
                }

                graphSMEquipmentMaint.Save.Press();
            }
            graphSMEquipmentMaint.Save.Press();

            return fsEquipmentRow;
        }

        #region DACFilter
        [Serializable]
        public partial class StockItemsFilter : PXBqlTable, IBqlTable
        {
            #region ItemClassID
            public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

            [PXInt]
            [PXUIField(DisplayName = "Item Class ID")]
            [PXSelector(typeof(
                Search<INItemClass.itemClassID,
                Where<FSxEquipmentModelTemplate.eQEnabled, Equal<True>>>), SubstituteKey = typeof(INItemClass.itemClassCD))]
            public virtual int? ItemClassID { get; set; }
            #endregion
            #region Date
            public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }

            [PXDBDate]
            [PXUIField(DisplayName = "Installed After")]
            public virtual DateTime? Date { get; set; }
            #endregion
        }
        #endregion

        #region Select
        [PXHidden]
        public PXFilter<StockItemsFilter> Filter;
        public PXCancel<StockItemsFilter> Cancel;

        [PXFilterable]
        public
        PXFilteredProcessingJoin<ProjIssuedEQInventoryItem, StockItemsFilter,
                InnerJoinSingleTable<Contract,
                    On<Contract.contractID, Equal<ProjIssuedEQInventoryItem.projectID>>>,
            Where2<
                Where<
                    CurrentValue<StockItemsFilter.itemClassID>, IsNull,
                    Or<ProjIssuedEQInventoryItem.itemClassID, Equal<CurrentValue<StockItemsFilter.itemClassID>>>>,
                And<
                    Where<CurrentValue<StockItemsFilter.date>, IsNull,
                    Or<ProjIssuedEQInventoryItem.docDate, GreaterEqual<CurrentValue<StockItemsFilter.date>>>>>>,
            OrderBy<
                Asc<ProjIssuedEQInventoryItem.inventoryCD,
                Asc<ProjIssuedEQInventoryItem.refNbr,
                Asc<ProjIssuedEQInventoryItem.lineNbr>>>>> InventoryItems;
        #endregion

        #region Actions

        #endregion

        #region Events
        protected virtual void _(Events.RowSelected<ProjIssuedEQInventoryItem> e)
        {
            if (e.Row == null)
            {
                return;
            }

            ProjIssuedEQInventoryItem inventoryItemRow = (ProjIssuedEQInventoryItem)e.Row;

            int components = PXSelect<FSModelComponent,
                             Where<
                                 FSModelComponent.modelID, Equal<Required<FSModelComponent.modelID>>>>
                             .SelectWindowed(this, 0, 1, inventoryItemRow.InventoryID).Count;

            if (components == 0)
            {
                e.Cache.RaiseExceptionHandling<ProjIssuedEQInventoryItem.inventoryCD>(inventoryItemRow,
                                                                              inventoryItemRow.InventoryCD,
                                                                              new PXSetPropertyException(TX.Warning.ITEM_WITH_NO_WARRANTIES_CONFIGURED,
                                                                                                         PXErrorLevel.RowWarning));
            }
        }
        #endregion

        #region Methods

        #endregion
    }

}
