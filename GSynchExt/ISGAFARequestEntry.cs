using PX.Data;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.EP;
using PX.Objects.FA;
using System;
using System.Collections;
using System.Linq;

namespace GSynchExt
{
    public class ISGAFARequestEntry : PXGraph<ISGAFARequestEntry, ISGAFARequest>
    {
        #region Selects
        [PXViewName(GSynchExt.Messages.FARequests)]
        public PXSelect<ISGAFARequest> FARequest;
        public PXSetup<ISGAFARequestSetup> AutoNumSetup;
        public PXSetup<ISGAFARequestSetupApproval> SetupApproval;
        [PXViewName(GSynchExt.Messages.ApprovalDetails)]
        [PXCopyPasteHiddenView]
        public EPApprovalAutomation<ISGAFARequest, ISGAFARequest.approved, ISGAFARequest.rejected, ISGAFARequest.hold, ISGAFARequestSetupApproval> Approval;
        public PXSelect<ISGAFARequest, Where<ISGAFARequest.reqNbr, Equal<Current<ISGAFARequest.reqNbr>>>> CurrentFARequest;
        //[PXHidden]
        public PXSelect<ISGAFARequestEvnt, Where<ISGAFARequestEvnt.reqNbr, Equal<Current<ISGAFARequest.reqNbr>>>> ReqEventRecords;
        [PXViewName(GSynchExt.Messages.FARequestDetails)]
        public PXSelect<ISGAFARequestLine, Where<ISGAFARequestLine.reqNbr, Equal<Current<ISGAFARequest.reqNbr>>>> FARequestDet;
        public PXSelect<FixedAsset> RequestedFAs;
        #endregion
        #region Graph constructor
        public ISGAFARequestEntry()
        {
            ISGAFARequestSetup setup = AutoNumSetup.Current;
        }
        #endregion
        #region EPApproval Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(ISGAFARequest.reqDate), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.docDate> e) { }
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(ISGAFARequest.reqBy), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.bAccountID> e) { }
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(ISGAFARequest.ownerID), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.documentOwnerID> e) { }
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(ISGAFARequest.description), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.descr> e) { }
        #endregion
        #region Actions
        public PXAction<ISGAFARequest> removeHold;
        [PXButton(),
        PXUIField(DisplayName = "Remove Hold",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable RemoveHold(PXAdapter adapter)
        {
            if (this.FARequest.Current != null)
            {
                FARequest.Current.Hold = false;
                FARequest.Current.ApprovalReqDate = Accessinfo.BusinessDate;
                FARequest.Current.ApprovalReqBy = GetCurrentEmployee(this)?.BAccountID;
                FARequest.Update(FARequest.Current);
            }
            return adapter.Get();
        }
        public PXAction<ISGAFARequest> close;
        [PXButton(),
        PXUIField(DisplayName = "Close",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Close(PXAdapter adapter)
        {
            return adapter.Get();
        }
        public PXAction<ISGAFARequest> reopen;
        [PXButton(),
        PXUIField(DisplayName = "Reopen",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable Reopen(PXAdapter adapter)
        {
            return adapter.Get();
        }
        public PXAction<ISGAFARequest> hold;
        [PXButton(),
        PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Hold(PXAdapter adapter)
        {
            if (FARequest.Current != null)
            {
                FARequest.Current.Hold = true;
                FARequest.Update(FARequest.Current);
            }
            return adapter.Get();
        }
        public PXAction<ISGAFARequest> issue;
        [PXButton(Connotation = PX.Data.WorkflowAPI.ActionConnotation.Secondary),
        PXUIField(DisplayName = "Issue",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Issue(PXAdapter adapter)
        {
            if (FARequest.Current != null)
            {
                ISGAFAProcessFARequests graph = PXGraph.CreateInstance<ISGAFAProcessFARequests>();
                ISGAFARequestFilter filter = new ISGAFARequestFilter();
                filter.ReqNbr = FARequest.Current.ReqNbr;
                filter.ActionType = FARequestActionType.Issue;
                graph.Filter.Current = filter;
                throw new PXRedirectRequiredException(graph, GSynchExt.Messages.IssueFAReqLines);
            }
            return adapter.Get();
        }
        public PXAction<ISGAFARequest> Return2;
        [PXButton(Connotation = PX.Data.WorkflowAPI.ActionConnotation.Secondary),
        PXUIField(DisplayName = "Return",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable return2(PXAdapter adapter)
        {
            if (FARequest.Current != null)
            {
                ISGAFAProcessFARequests graph = PXGraph.CreateInstance<ISGAFAProcessFARequests>();
                ISGAFARequestFilter filter = new ISGAFARequestFilter();
                filter.ReqNbr = FARequest.Current.ReqNbr;
                filter.ActionType = FARequestActionType.Return;
                graph.Filter.Current = filter;
                throw new PXRedirectRequiredException(graph, GSynchExt.Messages.ReturnFAReqLines);
            }
            return adapter.Get();
        }
        #endregion
        #region Event Handlers
        public static EPEmployee GetCurrentEmployee(PXGraph graph)
        {
            EPEmployee currentEmployee = PXSelect<EPEmployee,
                                         Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>
                                         .Select(graph);
            return currentEmployee;
        }
        protected virtual void _(Events.RowSelected<ISGAFARequest> e)
        {
            ISGAFARequest row = e.Row;
            if (row == null) return;

            bool enableProcessActions = row.Status == FARequestStatus.Open;
            reopen.SetVisible(false);
            issue.SetEnabled(enableProcessActions);
            Return2.SetEnabled(enableProcessActions);
            if (e.Row != null && e.Cache.GetStatus(e.Row) != PXEntryStatus.Inserted)
            {
                SetIssueToField(e.Cache, e.Row);
            }
        }
        protected virtual void _(Events.RowSelected<ISGAFARequestLine> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row == null) return;
            ISGAFARequest doc = this.FARequest.Current;
            if (doc == null) return;

            PXUIFieldAttribute.SetEnabled<ISGAFARequestLine.completed>(e.Cache, row, false);
            bool enableRequestLines = doc.Status == FARequestStatus.OnHold;
            PXUIFieldAttribute.SetEnabled<ISGAFARequestLine.assetID>(e.Cache, row, enableRequestLines);
            PXUIFieldAttribute.SetEnabled<ISGAFARequestLine.requestQty>(e.Cache, row, enableRequestLines);
            FARequestDet.AllowInsert = enableRequestLines;
            FARequestDet.AllowDelete = enableRequestLines;

        }
        protected virtual void _(Events.RowPersisting<ISGAFARequestLine> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row == null) return;

            if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
            {
                ValidateAndUpdateReqQtys(row);
            }
        }
        protected virtual void _(Events.RowPersisting<ISGAFARequest> e)
        {
            ISGAFARequest row = e.Row;
            if (row == null) return;

            if (e.Operation == PXDBOperation.Insert)
            {
                bool notClosed = FARequestDet.Cache.Inserted.Cast<ISGAFARequestLine>().Where(d => d.Completed != true).Any();
                if (FARequestDet.Cache.Inserted.Cast<ISGAFARequestLine>().Count() > 0 && !notClosed)
                {
                    row.Status = FARequestStatus.Closed;
                }
            }
            if (e.Operation == PXDBOperation.Update)
            {
                bool notClosed = FARequestDet.Select().Cast<ISGAFARequestLine>().Where(d => d.Completed != true).Any();
                if (FARequestDet.Select().Cast<ISGAFARequestLine>().Count() > 0 && !notClosed)
                {
                    row.Status = FARequestStatus.Closed;
                }
            }

        }
        protected virtual void _(Events.RowUpdating<ISGAFARequestLine> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row == null) return;
            bool isComplete = row.RequestQty == row.IssuedQty && row.IssuedQty == row.ReturnedQty && row.RequestQty > 0;
            if (isComplete)
            {
                row.Completed = true;
            }
        }
        protected virtual void _(Events.RowDeleted<ISGAFARequestLine> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row != null)
            {
                DeductFAReqQtys(row);
            }
        }
        protected virtual void _(Events.RowPersisted<ISGAFARequestLine> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row == null) return;

            IncreaseFAReqQtys(row);

            ISGAFARequest currentRequest = FARequest.Current;
            if (currentRequest == null) return;

            bool allLinesFullyIssued = true;
            foreach (ISGAFARequestLine line in FARequestDet.Select())
            {
                if ((line.RequestQty - line.IssuedQty ?? 0m) != 0)
                {
                    allLinesFullyIssued = false;
                    break;
                }
            }
            currentRequest.IsFullyIssued = allLinesFullyIssued;
            FARequest.Update(currentRequest);
        }
        protected virtual void _(Events.FieldDefaulting<ISGAFARequest, ISGAFARequest.issueTo> e)
        {
            if (e.Row != null && e.NewValue == null)
            {
                EPEmployee currentEmployee = GetCurrentEmployee(this);
                if (currentEmployee != null)
                {
                    e.NewValue = currentEmployee.BAccountID;
                }
            }
        }
        protected virtual void _(Events.FieldVerifying<ISGAFARequestLine, ISGAFARequestLine.requestQty> e)
        {
            ISGAFARequestLine row = e.Row;
            FixedAsset fAsset = FixedAsset.PK.Find(this, row.AssetID);
            if ((decimal)e.NewValue > fAsset?.Qty)
            {
                throw new PXException(GSynchExt.Messages.ReqQTYLessFAQty);
            }
        }
        protected virtual void _(Events.FieldSelecting<ISGAFARequestLine, ISGAFARequestLine.fAQty> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row == null) return;
            FixedAsset fAsset = FixedAsset.PK.Find(this, row.AssetID);

            e.ReturnValue = fAsset?.Qty;
        }
        protected virtual void _(Events.FieldSelecting<ISGAFARequestLine, ISGAFARequestLine.description> e)
        {
            ISGAFARequestLine row = e.Row;
            if (row == null) return;
            FixedAsset fAsset = FixedAsset.PK.Find(this, row.AssetID);

            e.ReturnValue = fAsset?.Description;
        }
        protected void ValidateAndUpdateReqQtys(ISGAFARequestLine row)
        {
            if (this.Accessinfo.ScreenID == GSynchExt.Messages.FARequest) return;

            FixedAsset fAsset = FixedAsset.PK.Find(this, row.AssetID);
            FixedAssetGSExt fAssetExt = fAsset.GetExtension<FixedAssetGSExt>();
            decimal? totalRequestQty = Decimal.Zero;
            GetTotalReqQtyPerAsset(fAsset, row, out totalRequestQty);

            if (fAsset.Qty - totalRequestQty < row.RequestQty)
            {
                throw new PXException(Messages.PersistError, (row.RequestQty - (fAsset.Qty - totalRequestQty)), fAsset?.AssetCD.Trim());
            }
            else
            {
                fAssetExt.UsrISGARequestedQty = (totalRequestQty ?? 0) + row.RequestQty;
                row.OpenQty = (row.RequestQty ?? 0) - (row.IssuedQty ?? 0);
            }
            RequestedFAs.Update(fAsset);
            FARequestDet.Update(row);
        }
        private void SetIssueToField(PXCache cache, ISGAFARequest row)
        {
            if (row == null) return;
            if (PXSelectorAttribute.Select<ISGAFARequest.issueTo>(cache, row) is EPEmployee employee)
            {
                cache.SetValueExt<ISGAFARequest.issueTo>(row, employee.BAccountID);
            }
        }
        public void GetTotalReqQtyPerAsset(FixedAsset asset, ISGAFARequestLine reqRow, out decimal? totalRequestQty)
        {
            totalRequestQty = 0;
            if (reqRow == null || asset == null) return;

            var reqLines = PXSelectGroupBy<ISGAFARequestLine, Where<ISGAFARequestLine.assetID, Equal<Required<ISGAFARequestLine.assetID>>,
                                   And<ISGAFARequestLine.reqNbr, NotEqual<Required<ISGAFARequestLine.reqNbr>>>>,
                                   Aggregate<GroupBy<ISGAFARequestLine.assetID,
                                   Sum<ISGAFARequestLine.requestQty,
                                   Sum<ISGAFARequestLine.issuedQty,
                                   Sum<ISGAFARequestLine.returnedQty>>>>>>
                                   .Select(this, reqRow.AssetID, reqRow.ReqNbr);
            ISGAFARequestLine issuedReqLinePerAsset = reqLines;
            if (issuedReqLinePerAsset == null)
            {
                return;
            }
            else
            {
                totalRequestQty = issuedReqLinePerAsset.RequestQty - issuedReqLinePerAsset.ReturnedQty;
            }
        }
        protected virtual void DeductFAReqQtys(ISGAFARequestLine row)
        {
            FixedAsset fAsset = FixedAsset.PK.Find(this, row.AssetID);
            if (fAsset == null) return;
            RequestedFAs.Current = fAsset;
            var fAssetExt = fAsset.GetExtension<FixedAssetGSExt>();

            fAssetExt.UsrISGARequestedQty -= row.RequestQty ?? 0;
            RequestedFAs.Update(RequestedFAs.Current);
        }
        protected virtual void IncreaseFAReqQtys(ISGAFARequestLine row)
        {
            FixedAsset fAsset = FixedAsset.PK.Find(this, row.AssetID);
            if (fAsset == null) return;
            RequestedFAs.Current = fAsset;
            var fAssetExt = fAsset.GetExtension<FixedAssetGSExt>();
            decimal? totalRequestQty = Decimal.Zero;
            GetTotalReqQtyPerAsset(fAsset, row, out totalRequestQty);

            fAssetExt.UsrISGARequestedQty = (totalRequestQty ?? 0) + row.RequestQty;
            RequestedFAs.Update(RequestedFAs.Current);
        }
        #endregion
    }
}