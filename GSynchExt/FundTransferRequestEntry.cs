using PX.Common;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.EP;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.TX.CSTaxCalcType;

namespace GSynchExt
{
    public class FundTransferRequestEntry : PXGraph<FundTransferRequestEntry, FundTransferRequest>
    {

        #region Selects

        [PXViewName("Fund Transfer Request")]
        public PXSelect<FundTransferRequest> EmpRequest;
        public PXSetup<FundTransferRequestSetup> AutoNumSetup;
        public PXSelect<FundTransferRequestSetup> Setup;
        public PXSelect<FundTransferApproval> SetupApproval;
        [PXViewName("Approval Details")]
        [PXCopyPasteHiddenView]
        public EPApprovalAutomation<FundTransferRequest, FundTransferRequest.approved, FundTransferRequest.rejected, FundTransferRequest.hold, FundTransferApproval> Approval;

        #endregion

        #region Actions

        public PXAction<FundTransferRequest> Assign;
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
        [PXUIField(DisplayName = "Assign", Visible = false)]
        public virtual IEnumerable assign(PXAdapter adapter)
        {
            foreach (FundTransferRequest req in adapter.Get<FundTransferRequest>())
            {
                if (Setup.Current.ApprovalMap != null)
                {
                    var processor = CreateInstance<EPAssignmentProcessor<FundTransferRequest>>();
                    processor.Assign(req, SetupApproval.Current.AssignmentMapID);
                    req.WorkgroupID = req.ApprovalWorkgroupID;
                    req.OwnerID = req.ApprovalOwnerID;

                }
                yield return req;
            }
        }

        #endregion

        #region EPApproval Cache Attached - Approvals Fields

        [PXDBDate()]
        [PXDefault(typeof(FundTransferRequest.reqDate), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.docDate> e)

        {

        }

        [PXDBString(60, IsUnicode = true)]
        [PXDefault(typeof(FundTransferRequest.description), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.descr> e)
        {

        }

        [PXDBDecimal]
        [PXDefault(typeof(FundTransferRequest.amount), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.curyTotalAmount> e)
        {

        }

        #endregion

        #region Constructor
        public FundTransferRequestEntry()
        {
            FundTransferRequestSetup setup = AutoNumSetup.Current;

        }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.FieldDefaulting<FundTransferRequest.reqBy> e)
        {
            if (e.Row == null) return;
            var userID = Accessinfo.UserID;
            FundTransferRequest req = (FundTransferRequest)e.Row;
            EPEmployee emp = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>. Select(this, userID);
            e.Cache.SetValueExt<FundTransferRequest.reqBy>(e.Row, emp?.BAccountID);

        }

        protected virtual void _(Events.RowSelected<FundTransferRequest> e)
        {
            FundTransferRequest doc = (FundTransferRequest)e.Row;
            if (doc == null) return;
            CreateFTransfer.SetEnabled(doc.Status == FTRStatus.Released);

            if (doc.Status == FTRStatus.Released || doc.Status == FTRStatus.Closed)
            {
                bool fullyTransferred = (doc.Status == FTRStatus.Released && doc.OpenBalance == 0) ;
                PXUIFieldAttribute.SetEnabled<FundTransferRequest.amount>(e.Cache, doc, false);
                PXUIFieldAttribute.SetEnabled<FundTransferRequest.reqDate>(e.Cache, doc, false);
                PXUIFieldAttribute.SetEnabled<FundTransferRequest.cashAccntID>(e.Cache, doc, false);
                PXUIFieldAttribute.SetEnabled<FundTransferRequest.notify>(e.Cache, doc, false);
                if (fullyTransferred)
                {
                    e.Cache.SetValueExt<FundTransferRequest.status>(e.Row, FTRStatus.Closed);

                }
            }
        }

        protected virtual void _(Events.FieldUpdated<FundTransferRequest.amount> e)
        {
            FundTransferRequest doc = (FundTransferRequest)e.Row;
            if (doc == null) return;

            doc.OpenBalance = doc.Amount;
        }
        #endregion

        #region WorkFlow Actions

        public PXAction<FundTransferRequest> RemoveHold;
        [PXButton(), PXUIField(DisplayName = "Remove Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable removeHold(PXAdapter adapter)
        {
            if (EmpRequest.Current != null)
            {
                if (EmpRequest.Current.Amount == 0)
                {
                    throw new PXException(GSynchExt.Messages.CannotRelease);
                }
                EmpRequest.Current.Hold = false;
                EmpRequest.Update(EmpRequest.Current);
            }
            return adapter.Get();
        }


        public PXAction<FundTransferRequest> Approve;
        [PXButton(), PXUIField(DisplayName = "Approve",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable approve(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<FundTransferRequest> Reject;
        [PXButton(), PXUIField(DisplayName = "Reject",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable reject(PXAdapter adapter)
        {
            return adapter.Get();
        }


        public PXAction<FundTransferRequest> Hold2;
        [PXButton(), PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable hold2(PXAdapter adapter)
        {
            if (EmpRequest.Current != null)
            {

                if (EmpRequest.Current.Requested == true && EmpRequest.Current.Transferred == true)
                {
                    throw new PXException(GSynchExt.Messages.CannotHold, EmpRequest.Current.ReqNbr);
                }
                EmpRequest.Current.Hold = true;
                EmpRequest.Update(EmpRequest.Current);
            }
            return adapter.Get();
        }

        public PXAction<FundTransferRequest> Close;
        [PXButton(), PXUIField(DisplayName = "Close",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable close(PXAdapter adapter)
        {
            if (EmpRequest.Current.Requested == true && EmpRequest.Current.Transferred == false)
            {
                throw new PXException(GSynchExt.Messages.CannotClose, EmpRequest.Current.ReqNbr);
            }
            return adapter.Get();
        }

        public PXAction<FundTransferRequest> CreateFTransfer;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Transfer", Enabled = true)]
        protected virtual IEnumerable createFTransfer(PXAdapter adapter)
        {
            if (EmpRequest.Current.Requested == true)
            {
                if (EmpRequest.Current.OpenBalance == 0)
                {
                    throw new PXException(GSynchExt.Messages.CannotCreateTransfer, EmpRequest.Current.ReqNbr);
                }
            }
            this.Save.Press();

            PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<CashTransferEntry>().GetExtension<CashTransferEntryGSExt>().CreateTransferFromFTR(EmpRequest.Current, redirect: true));
            return adapter.Get();
        }
        #endregion
    }
}
