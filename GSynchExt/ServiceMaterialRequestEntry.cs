using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Data.PXGenericInqGrph;
using static PX.Objects.CA.CABankTran.FK;
using static PX.Objects.FA.FABookSettings.midMonthType;
using static PX.Objects.IN.InventoryTranSumEnqFilter;
using static PX.Objects.PM.GSProjectHelper;
using static PX.Objects.TX.CSTaxCalcType;
using BudgetKeyTuple = PX.Objects.CS.BudgetKeyTuple;
using PX.Objects.AP;
using PX.Objects.FS;
using PX.Objects.SO;

namespace GSynchExt
{
    public class ServiceMaterialRequestEntry : PXGraph<ServiceMaterialRequestEntry, ServiceMaterialRequest>
    {
        public PXSelect<ServiceMaterialRequest> MatlRequest;
        public PXSelect<ServiceMaterialRequestDetails, Where<ServiceMaterialRequestDetails.reqNbr, 
            Equal<Current<ServiceMaterialRequest.reqNbr>>>> MatlRequestDet;
        public PXSetup<FundTransferRequestSetup> AutoNumSetup;
        public PXFilter<CopyDialogInfo2> CopyDialog;
        public PXSelect<FundTransferRequestSetup> Setup;
      //  public PXFilter<SMRSiteStatusFilter> sitestatusfilter;
      //     public SMRSiteStatusLookup<SMRSiteStatusSelected, SMRSiteStatusFilter> smrSitestatus;
             public PXSelect<SMRSiteStatusSelected> smrSitestatus;
        public PXSelect<InventoryItem, Where<InventoryItem.stkItem,
           Equal<True>>> MatlRequestInv;

        public PXSelectGroupBy<ServiceMaterialRequestDetails, Where<ServiceMaterialRequestDetails.reqNbr, Equal<Current<ServiceMaterialRequest.reqNbr>>>,
        Aggregate<GroupBy<ServiceMaterialRequestDetails.reqNbr,
            Sum<ServiceMaterialRequestDetails.requestedQty,
            Sum<ServiceMaterialRequestDetails.transferQty>>>>> SMatlRequestTotals;

        public PXAction<ServiceMaterialRequest> CreateSMTransfer;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Transfer", Enabled = true)]
        protected virtual IEnumerable createSMTransfer(PXAdapter adapter)
        {
            if (MatlRequest.Current == null)
                return adapter.Get();

            this.Save.Press();

            CopyDialogInfo2 info = CopyDialog.Current;

            if (CopyDialog.View.Answer == WebDialogResult.None)
            {
                CopyDialog.Cache.Clear();
                CopyDialogInfo2 filterdata = CopyDialog.Cache.Insert() as CopyDialogInfo2;
            }
            if (CopyDialog.AskExt() != WebDialogResult.OK  /* || string.IsNullOrEmpty(CopyDialog.Current.NewRevisionID)*/)
            {
                return adapter.Get();
            }
            ServiceMaterialRequest request = (ServiceMaterialRequest)this.MatlRequest.Current;
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<INTransferEntry>().GetExtension<INTransferEntryGSExt>().CreateTransferFromSMR(request, info, redirect: true));
            }

            return adapter.Get();
        }    
        protected virtual void _(Events.FieldDefaulting<ServiceMaterialRequest.reqBy> e)
        {
            if (e.Row == null) return;

            var userID = Accessinfo.UserID;

            ServiceMaterialRequest req = (ServiceMaterialRequest)e.Row;

            EPEmployee emp = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>.
                                Select(this, userID);
            e.Cache.SetValueExt<ServiceMaterialRequest.reqBy>(e.Row, emp?.BAccountID);
        }


        #region WorkFlow Actions

        public PXAction<ServiceMaterialRequest> RemoveHold;
        [PXButton(), PXUIField(DisplayName = "Remove Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable removeHold(PXAdapter adapter)
        {
          
            return adapter.Get();
        }

        public PXAction<ServiceMaterialRequest> Hold2;
        [PXButton(), PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable hold2(PXAdapter adapter)
        {
             return adapter.Get();
        }

        public PXAction<ServiceMaterialRequest> Cancel2;
        [PXButton(), PXUIField(DisplayName = "Cancel",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable cancel2(PXAdapter adapter)
        {
            
            return adapter.Get();
        }

        public PXAction<ServiceMaterialRequest> Close;
        [PXButton(), PXUIField(DisplayName = "Close",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable close(PXAdapter adapter)
        {
          
            return adapter.Get();
        }

        public PXAction<ServiceMaterialRequest> Reverse;
        [PXButton(), PXUIField(DisplayName = "Reverse",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable reverse(PXAdapter adapter)
        {
            return adapter.Get();
        }

        #endregion

        #region EventHandlers

        protected virtual void _(Events.RowSelected<ServiceMaterialRequest> e)
        {
            ServiceMaterialRequest doc = (ServiceMaterialRequest)e.Row;
            if (doc == null) return;
            bool isReleased = doc.Status == GSynchExt.FTRStatus.Released && doc.ReqNbr != null;
            CreateSMTransfer.SetEnabled(isReleased);

            ServiceMaterialRequestDetails totalsRec = this.SMatlRequestTotals.Select().Where(x => x.Record.ReqNbr == doc.ReqNbr).FirstOrDefault();
            if (totalsRec != null)
            {
                doc.TransferQty = totalsRec.TransferQty;
                doc.RequestQty = totalsRec.RequestedQty;
            }


        }


        #endregion

        #region Methods 

        public void ReCalculateSMRQuantities(ServiceMaterialRequestEntry graph, ServiceMaterialRequest mtr, string docType)
        {
            foreach (ServiceMaterialRequestDetails lines in graph.MatlRequestDet.Select().Where(x => x.Record.ReqNbr == mtr.ReqNbr))
            {
                INTran totalRec2 = PXSelectGroupBy<INTran,
                                  Where<INTran.inventoryID, Equal<Required<INTran.inventoryID>>>,
                                  Aggregate<GroupBy<INTran.inventoryID, Sum<INTran.qty>>>>.Select(graph, lines.InventoryID, mtr.ReqNbr).FirstOrDefault();

                //    if (totalRec2 == null) continue;

                        /*
                            tran.LocationID = info.LocationID;
                            tran.CostLayerType = info.CostLayerType;
                            tran.ToLocationID = info.ToLocationID;
                            tran.ToCostLayerType = info.ToCostLayerType;
                            tran.SiteID = info.FromSiteID;
                            tran.InventoryID = lineRec.InventoryID;*/




                if (totalRec2 != null && docType == INDocType.Transfer)
                {
                    lines.TransferQty = totalRec2.Qty;
                }

                graph.MatlRequestDet.Update(lines);
                }

                graph.Actions.PressSave();
                }
        #endregion

        #region Add Items
        // Add Inv Item action button
        public PXAction<ServiceMaterialRequest> addInvBySite;
        [PXUIField(DisplayName = "Add Items", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable AddInvBySite(PXAdapter adapter)
        {
        if (smrSitestatus.AskExt() == WebDialogResult.OK)
        {
        return AddInvSelBySite(adapter);
        }
        smrSitestatus.Cache.Clear();
        return adapter.Get();
        }
        #endregion

        public PXAction<ServiceMaterialRequest> addInvSelBySite;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddInvSelBySite(PXAdapter adapter)
        {
            foreach (SMRSiteStatusSelected line in smrSitestatus.Cache.Cached)
            {
                if (line.Selected == true)
                {
                    InventoryItem inventoryItem =
                        PXSelect<InventoryItem,
                        Where<
                            InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                    .Select(this, line.InventoryID);

                    ServiceMaterialRequestDetails newline = PXCache<ServiceMaterialRequestDetails>.
                        CreateCopy(MatlRequestDet.Insert(new ServiceMaterialRequestDetails()));

                    newline.InventoryID = line.InventoryID;
                    newline.RequestedQty = line.QtySelected;
                    newline.UoM = line.SalesUnit;

                    MatlRequestDet.Update(newline);
                }
            }
            smrSitestatus.Cache.Clear();
            return adapter.Get();
        }
    }
}
