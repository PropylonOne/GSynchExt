using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using GSynchExt;
using PX.Objects.PM;
using PX.Objects.CS;
using System.Linq;
using PX.Objects.AM;

using static PX.Objects.PM.AccountGroupMaint;

namespace PX.Objects.IN
{
    public class INTransferEntryGSExt : PXGraphExtension<INTransferEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.inventory>(); }
        #endregion

        public PXFilter<ProjectStockFilter> projectstockitemsfilter;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        [PXImport]
        public PXSelect<ProjectStock, Where<ProjectStock.projectID, Equal<Current<ProjectStockFilter.contractID>>,
            And<ProjectStock.siteID,
                        Equal<Current<INRegister.siteID>>>>> ProjectStockItems;
        public IEnumerable projectstockitems()
        {
            List<object> parameters = new List<object>();
            var projectItemSelect = new PXSelectGroupBy<ProjectStock,
                 Where<ProjectStock.projectID, Equal<Current<ProjectStockFilter.contractID>>,
                     And<ProjectStock.siteID, Equal<Current<INRegister.siteID>>>>,
                     Aggregate<GroupBy<ProjectStock.inventoryID,
                     GroupBy<ProjectStock.companyID,
                     GroupBy<ProjectStock.projectID,
                     GroupBy<ProjectStock.taskID,
                     GroupBy<ProjectStock.lotSerialNbr,
                     GroupBy<ProjectStock.siteID,
                     GroupBy<ProjectStock.costCodeID,
                     GroupBy<ProjectStock.locationID, Sum<ProjectStock.totalAvailableQty>>>>>>>>>>>(this.Base);
            PXDelegateResult delResult = new PXDelegateResult();
            delResult.Capacity = 202;
            delResult.IsResultFiltered = false;
            delResult.IsResultSorted = true;
            delResult.IsResultTruncated = false;

            var view = new PXView(this.Base, false, projectItemSelect.View.BqlSelect);
            var startRow = PXView.StartRow;
            int totalRows = 0;
            var resultset = view.Select(PXView.Currents, parameters.ToArray(), PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
            PXView.StartRow = 0;

            foreach (ProjectStock projectStockItem in resultset)
            {
                var filter = projectstockitemsfilter.Current;
                ProjectStock projectItem = projectStockItem;

                if (filter.IsActiveTasks == true)
                {

                    PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this.Base, projectItem.TaskID);

                    if (projectItem.TotalAvailableQty > 0 && task.Status == ProjectTaskStatus.Active)
                    {

                        projectItem.QtySelected = projectItem.TotalAvailableQty;
                        delResult.Add(projectItem);
                    }
                }
                else
                {

                    if (projectItem.TotalAvailableQty > 0)
                    {
                        projectItem.QtySelected = projectItem.TotalAvailableQty;
                        delResult.Add(projectItem);
                    }
                }

            }

            return delResult;
        }


        public PXAction<INTran> addProjectItems;
        [PXUIField(DisplayName = "Add Project Stock Items", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(DisplayOnMainToolbar = false)]
        public virtual IEnumerable AddProjectItems(PXAdapter adapter)
        {
            IEnumerable result = null;
            {
                if (ProjectStockItems.AskExt() == WebDialogResult.OK)
                {
                    result = AddSelectedProjecStockLines(adapter);
                }

                projectstockitemsfilter.Cache.Clear();
                ProjectStockItems.Cache.Clear();
                ProjectStockItems.ClearDialog();
                ProjectStockItems.View.Clear();
                ProjectStockItems.View.ClearDialog();
            }

            if (result != null)
            {
                return result;
            }

            return adapter.Get();
        }

        public PXAction<INTran> addSelectedProjecStockLines;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(DisplayOnMainToolbar = false)]
        public virtual IEnumerable AddSelectedProjecStockLines(PXAdapter adapter)
        {
            //  if (this.Base.Document.Current.Hold == true)
            {
                this.Base.transactions.Cache.ForceExceptionHandling = true;

                foreach (ProjectStock line in ProjectStockItems.Cache.Cached)
                {
                    if (line.Selected == true && line.QtySelected > 0)
                    {
                        INTran newline = PXCache<INTran>.CreateCopy(this.Base.transactions.Insert(new INTran()));
                        newline = InitTran(newline, line);
                        newline.Qty = line.QtySelected;
                        this.Base.transactions.Update(newline);
                    }
                }
            }

            return adapter.Get();
        }

        protected INTran InitTran(INTran newTran, ProjectStock siteStatus)
        {
            newTran.SiteID = siteStatus.SiteID ?? newTran.SiteID;
            newTran.InventoryID = siteStatus.InventoryID;
            newTran.UOM = siteStatus.UOM;


            newTran = PXCache<INTran>.CreateCopy(this.Base.transactions.Update(newTran));
            if (siteStatus.LocationID != null)
            {
                newTran.LocationID = siteStatus.LocationID;

                newTran = PXCache<INTran>.CreateCopy(this.Base.transactions.Update(newTran));
            }
            newTran.CostLayerType = CostLayerType.Project;
            newTran.ProjectID = siteStatus.ProjectID;
            newTran.TaskID = siteStatus.TaskID;
            newTran.CostCodeID = siteStatus.CostCodeID;
            newTran = PXCache<INTran>.CreateCopy(this.Base.transactions.Update(newTran));



            if (siteStatus.LotSerialNbr != null)
            {
                newTran.LotSerialNbr = siteStatus.LotSerialNbr;

                newTran = PXCache<INTran>.CreateCopy(this.Base.transactions.Update(newTran));
            }
            return newTran;
        }

        public virtual INRegister CreateTransfer(MaterialTransferRequest mTRequest, CopyDialogInfo info)
        {
            INRegister reg = new INRegister();
            if (mTRequest == null) return reg;
            reg.SiteID = mTRequest.FromSiteID;
            reg.ToSiteID = mTRequest.ToSiteID;
            reg.ExtRefNbr = mTRequest.ReqNbr;
            this.Base.transfer.Insert(reg);

            var hasLines = false;
            var req = PXSelect<MTRequestDetails,
                                                                      Where<MTRequestDetails.reqNbr,
                                                                      Equal<Required<MTRequestDetails.reqNbr>>,
                                                                      And<MTRequestDetails.requestedQty, Greater<decimal0>>>>.Select(this.Base, mTRequest.ReqNbr);
            string finalErrorMessage = "No line(s) to proceed. Please review the error.\n";
            foreach (var MTRequestetails in req)
            {
                MTRequestDetails lineRec = (MTRequestDetails)MTRequestetails;
                PMTask task = PMTask.PK.Find(this.Base, lineRec.ProjectID, lineRec.TaskID);
                INSite site = INSite.PK.Find(this.Base, mTRequest.FromSiteID);
                INLocation location = INLocation.PK.Find(this.Base, info.LocationID);
                InventoryItem item = InventoryItem.PK.Find(this.Base, lineRec.InventoryID);
                INLocationStatus inItem = PXSelect<INLocationStatus,
                                                Where<INLocationStatus.inventoryID, Equal<Required<INLocationStatus.inventoryID>>,
                                                And<INLocationStatus.siteID, Equal<Required<INLocationStatus.siteID>>,
                                                And<INLocationStatus.locationID, Equal<Required<INLocationStatus.locationID>>>>>>.Select(this.Base, lineRec.InventoryID, mTRequest.FromSiteID, info.LocationID);
                bool insertRecord = false;
                var currentErrorMessage = " ";
                if (lineRec.RequestedQty > lineRec.TransferQty)
                {
                    if (info.CheckAvailableQty == true)
                    {
                        if (inItem != null)
                        {
                            if (info.ActiveTask == true)
                            {
                                if (inItem.QtyAvail >= (lineRec.RequestedQty - lineRec.TransferQty) && task.Status == ProjectTaskStatus.Active)
                                {
                                    insertRecord = true;
                                }
                                else
                                {
                                    if (inItem.QtyAvail < (lineRec.RequestedQty - lineRec.TransferQty) && task.Status != ProjectTaskStatus.Active)
                                    {
                                        currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario1,
                                                                                            item.InventoryCD.Trim(), site.SiteCD.Trim(), location.LocationCD.Trim(), inItem.QtyAvail, task.TaskCD.Trim());

                                    }
                                    if (inItem.QtyAvail < (lineRec.RequestedQty - lineRec.TransferQty) && task.Status == ProjectTaskStatus.Active)
                                    {
                                        currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario2,
                                                                                             item.InventoryCD.Trim(), site.SiteCD.Trim(), location.LocationCD.Trim(), inItem.QtyAvail);

                                    }
                                    if (inItem.QtyAvail > (lineRec.RequestedQty - lineRec.TransferQty) && task.Status != ProjectTaskStatus.Active)
                                    {
                                        currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario3,
                                                                                            task.TaskCD.Trim());
                                    }
                                }
                            }
                            else
                            {
                                if (inItem.QtyAvail > (lineRec.RequestedQty - lineRec.TransferQty))
                                {
                                    insertRecord = true;
                                }
                                else
                                {
                                    currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario4,
                                                                                        item.InventoryCD.Trim(), site.SiteCD.Trim(), location.LocationCD.Trim(), inItem.QtyAvail);
                                }
                            }
                        }
                        else
                        {
                            currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario5,
                                                                                            item.InventoryCD.Trim(), site.SiteCD.Trim(), location.LocationCD.Trim());
                        }
                    }
                    else
                    { 
                        if (info.ActiveTask == true)
                        {
                            if (task.Status == ProjectTaskStatus.Active)
                            {
                                insertRecord = true;
                            }
                            else
                            {
                                currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario6,
                                                                                             task.TaskCD.Trim());
                            }
                        }
                        else
                        {
                            insertRecord = true;
                        }
                    }
                }
                else
                {
                    currentErrorMessage = PXMessages.LocalizeFormat(GSynchExt.Messages.Scenario7, mTRequest.ReqNbr.Trim());
                }
                if (insertRecord)
                {
                    var tran = this.Base.transactions.Insert(new INTran
                    {
                        LocationID = info.LocationID,
                        CostLayerType = info.CostLayerType,
                        ToLocationID = info.ToLocationID,
                        ToCostLayerType = info.ToCostLayerType,
                        InventoryID = lineRec.InventoryID,
                        ToProjectID = lineRec.ProjectID,
                        ToTaskID = lineRec.TaskID,
                        ToCostCodeID = lineRec.CostCode,
                        Qty = lineRec.RequestedQty - lineRec.TransferQty,
                        OrigQty = lineRec.RequestedQty
                    });
                    INTranGSExt tranExt = PXCache<INTran>.GetExtension<INTranGSExt>(tran);
                    tranExt.UsrcreatedByMTR = true;
                    tranExt.UsrMTRRef = mTRequest.ReqNbr;

                    hasLines = true;
                }
                else
                {
                    if (!finalErrorMessage.Contains(currentErrorMessage))
                    {
                        finalErrorMessage += currentErrorMessage;
                    }
                }
            }
            if (!hasLines)
            {
                throw new PXException(finalErrorMessage);
            }
            return reg;
        }
        public virtual INTran CreateTransferFromMTR(MaterialTransferRequest mTRequest, CopyDialogInfo info, bool redirect = false)
        {

            CreateTransfer(mTRequest, info);
            if (this.Base.transfer.Cache.IsDirty)
            {
                if (redirect)
                {
                    throw new PXRedirectRequiredException(this.Base, "");
                }
                else
                {
                    return this.Base.transactions.Current;
                }
            }
            throw new PXException();
        }
        public virtual INRegister CreateSMTransfer(ServiceMaterialRequest mTRequest, CopyDialogInfo2 info)
        {
            INRegister reg = new INRegister();
            reg.SiteID = mTRequest.FromSiteID;
            reg.ToSiteID = mTRequest.ToSiteID;
            reg.SiteID = info.FromSiteID;
            reg.ExtRefNbr = mTRequest.ReqNbr;
            this.Base.transfer.Insert(reg);

            var req = PXSelect<ServiceMaterialRequestDetails,
                       Where<ServiceMaterialRequestDetails.reqNbr,
                       Equal<Required<ServiceMaterialRequestDetails.reqNbr>>,
                       And<ServiceMaterialRequestDetails.requestedQty, Greater<decimal0>>>>.Select(this.Base, mTRequest.ReqNbr);
            foreach (var MTRequestetails in req)
            {
                ServiceMaterialRequestDetails lineRec = (ServiceMaterialRequestDetails)MTRequestetails;

                INTran tran = new INTran();
                INTranGSExt tranExt = PXCache<INTran>.GetExtension<INTranGSExt>(tran);
                tran.LocationID = info.LocationID;
                tran.CostLayerType = info.CostLayerType;
                tran.ToLocationID = info.ToLocationID;
                tran.ToCostLayerType = info.ToCostLayerType;
                tran.SiteID = info.FromSiteID;
                tran.InventoryID = lineRec.InventoryID;
                tran.Qty = lineRec.RequestedQty;
                tran.OrigQty = lineRec.RequestedQty;
                tranExt.UsrCreatedBySMR = true;
                tranExt.UsrSMRRef = mTRequest.ReqNbr;

                this.Base.transactions.Insert(tran);
            }
            if (mTRequest == null) return reg;

            return reg;
        }

        public virtual INTran CreateTransferFromSMR(ServiceMaterialRequest mTRequest, CopyDialogInfo2 info, bool redirect = false)
        {

            CreateSMTransfer(mTRequest, info);
            if (this.Base.transfer.Cache.IsDirty)
            {
                if (redirect)
                {

                    throw new PXRedirectRequiredException(this.Base, "");
                }
                else
                {
                    return this.Base.transactions.Current;

                }
            }
            throw new PXException();
        }
        protected virtual void CalcLineEstCostTotals(out decimal UsrISGAEstTotalCost)
        {
            UsrISGAEstTotalCost = 0m;

            var detailsRows = this.Base.transactions.Select();
            if (detailsRows == null) return;

            foreach (INTran tran in detailsRows)
            {
                INItemCost itemCostRec = INItemCost.PK.Find(this.Base, tran?.InventoryID, this.Base.Accessinfo.BaseCuryID);
                if (itemCostRec != null)
                {
                    decimal RoundedAvgCost = Math.Round((decimal)itemCostRec.AvgCost, 4);
                    decimal RoundedQty = Math.Round((decimal)tran.Qty, 4);
                    UsrISGAEstTotalCost += (decimal)(RoundedAvgCost * RoundedQty);

                }
            }
        }

        #region Event Handlers

        protected virtual void _(Events.RowSelected<INRegister> e)
        {
            INRegister row = e.Row;
            if (row == null) return;

            INRegisterGSExt rowExt = row.GetExtension<INRegisterGSExt>();
            decimal UsrISGAEstTotalCost = 0m;
            CalcLineEstCostTotals(out UsrISGAEstTotalCost);
            rowExt.UsrISGAEstTotalCost = UsrISGAEstTotalCost;

            bool isReleased = row.Status == INDocStatus.Released;
            addProjectItems.SetEnabled(!isReleased);

            bool notFromMTR = row.CreatedByScreenID != "GS301027";
            PXUIFieldAttribute.SetEnabled<INRegister.extRefNbr>(e.Cache, row, notFromMTR);
        }
        protected virtual void _(Events.RowPersisted<INRegister> e)
        {
            INRegister row = e.Row;
            if (row == null) return;
            //03212024 - Recalculate Qty Improvements Start
            MaterialTransferRequest mtr = MaterialTransferRequest.UK.Find(this.Base, row.ExtRefNbr);
            bool FromMTR = (row.CreatedByScreenID == "GS301027" || mtr != null);
            try
            {
                var MTRGraph = PXGraph.CreateInstance<MaterialTransferRequestEntry>();
                MTRGraph.MatlRequest.Current = mtr;
                ProjectEntry ProjEntry = PXGraph.CreateInstance<ProjectEntry>();
                var details = MTRGraph.MatlRequestDet.Select();

                if ((e.TranStatus == PXTranStatus.Completed || e.TranStatus == PXTranStatus.Open) && FromMTR)
                {
                    /// Recalculate transfer quantities for the request everytime the transfer header is saved/Deleted.

                    // MTRGraph.ReCalTranIssueQuantities(MTRGraph, mtr, INDocType.Transfer);

                    foreach (MTRequestDetails requestlines in details)
                    {
                        var budget = PMCostBudget.PK.Find(ProjEntry, requestlines.ProjectID, requestlines.TaskID, requestlines.AccountGroupID,
                                                                requestlines.CostCode, requestlines.InventoryID);

                        decimal? totTranQty = Decimal.Zero;
                        decimal? totIssueQty = Decimal.Zero;
                        MTRGraph.GetTranIssueQtyPerRequest(MTRGraph, requestlines, out totTranQty, out totIssueQty);

                        requestlines.TransferQty = totTranQty;
                        requestlines.IssueQty = totIssueQty;
                        requestlines.RevisedQty = budget?.RevisedQty;

                        MTRGraph.MatlRequestLineDet.Update(requestlines);
                        MTRGraph.Save.Press();
                    }
                    MTRGraph.Save.Press();
                }
            }
            //03212024 - Recalculate Qty Improvements End
            catch (Exception ex)
            {
                throw new PXException(ex.Message);
            }
        }
        #endregion
       
        public class CurrentCompany : PX.Data.BQL.BqlInt.Constant<CurrentCompany> { public CurrentCompany() : base(PX.Data.Update.PXInstanceHelper.CurrentCompany) { } }
        public class ProjectType : PX.Data.BQL.BqlString.Constant<ProjectType> { public ProjectType() : base("P") { } }


    }
}