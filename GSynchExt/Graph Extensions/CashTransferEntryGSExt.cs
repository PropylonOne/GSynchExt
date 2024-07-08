using System;
using System.Collections;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.IN;
using PX.Objects.PM;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.FA.FABookSettings.midMonthType;
using static PX.Objects.TX.CSTaxCalcType;

namespace GSynchExt
{
    public class CashTransferEntryGSExt : PXGraphExtension<CashTransferEntry>
    {
        public virtual CATransfer CreateTransferFromFTR(FundTransferRequest fTRequest, bool redirect = false)
        {

            var rec = CreateTransfer(fTRequest);
            if (this.Base.Transfer.Cache.IsDirty)
            {
                if (redirect)
                {

                    throw new PXRedirectRequiredException(this.Base, "");
                }
                else
                {
                    return this.Base.Transfer.Current;

                }
            }
            throw new PXException("");

        }

        public virtual CATransfer CreateTransfer(FundTransferRequest fTRequest)
        {
            CATransfer rec = this.Base.Transfer.Insert();
            rec.Descr = fTRequest.Description;
            rec.OutExtRefNbr = fTRequest.ReqNbr;
            rec.OutCuryID = this.Base.Accessinfo.BaseCuryID;
            rec.CuryTranOut = fTRequest.Amount;
            if (fTRequest.OpenBalance != decimal.Zero)
            {
                rec.CuryTranOut = fTRequest.OpenBalance;

            }
            rec.InAccountID = fTRequest.CashAccntID;
            rec.InExtRefNbr = fTRequest.ReqNbr;
            rec.InCuryID = this.Base.Accessinfo.BaseCuryID;
            rec.CuryTranIn = fTRequest.Amount;

            rec = this.Base.Transfer.Update(rec);

            return rec;
        }

        protected virtual void _(Events.RowSelected<CATransfer> e)
        {
            CATransfer doc = (CATransfer)e.Row;
            if (doc == null) return;

            bool fromFTR = (doc.CreatedByScreenID == "GS301015" && doc.InExtRefNbr != null);
            if (fromFTR )
            {
                PXUIFieldAttribute.SetEnabled<CATransfer.inExtRefNbr>(e.Cache, doc, !fromFTR);
                PXUIFieldAttribute.SetEnabled<CATransfer.outExtRefNbr>(e.Cache, doc, !fromFTR);
                PXUIFieldAttribute.SetEnabled<CATransfer.curyTranIn>(e.Cache, doc, !fromFTR);

            }
        }
        protected virtual void _(Events.RowPersisting<CATransfer> e)
        {
            CATransfer row = e.Row as CATransfer;
            if (row == null) return;
            // Fetch FTR relevant to  Current Transfer record
            FundTransferRequest tranRequest = FundTransferRequest.UK.Find(this.Base, row.InExtRefNbr);
            bool fromFTR = (row.CreatedByScreenID == "GS301015" || tranRequest != null);
            if (fromFTR && row.InExtRefNbr != null && row.OrigTransferNbr == null)
            {
                bool isDeleted = e.Cache.GetStatus(row) == PXEntryStatus.Deleted;
                if (!isDeleted) /// New Transfer or Update
                {
                    var FTRGraph = PXGraph.CreateInstance<FundTransferRequestEntry>();
                    FundTransferRequest transferRequest = FundTransferRequest.UK.Find(FTRGraph, row.InExtRefNbr);

                    if (transferRequest == null) return;
                    //Calculate amounts
                    var inExisitngTotal = GetExistingTotalIn(FTRGraph, row, transferRequest);
                    var outExisitngTotal = GetExistingTotalOut(FTRGraph, row, transferRequest);
                    var requestedTotal = transferRequest.Amount;
                    var currentInAmount = row.CuryTranIn;
                    var oldOpenBal = requestedTotal - (inExisitngTotal - outExisitngTotal); ///Excluding the current document
                    FTRGraph.EmpRequest.Current = transferRequest;
                    if (currentInAmount + inExisitngTotal + outExisitngTotal != 0) // Transfer IN
                    {
                        decimal? openBal = 0;
                        openBal = oldOpenBal - currentInAmount;
                        FTRGraph.EmpRequest.Current.OpenBalance = openBal;
                        if (FTRGraph.EmpRequest.Current.OpenBalance < 0)
                        {
                            throw new PXException(GSynchExt.Messages.ValidateCAAmount, oldOpenBal);
                        }
                        FTRGraph.EmpRequest.Current.Requested = true;
                        FTRGraph.EmpRequest.Update(FTRGraph.EmpRequest.Current);
                        FTRGraph.Actions.PressSave();
                    }
                }
            }
        }

        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable release(PXAdapter adapter, ReleaseDelegate BaseMethod)
        {
            CATransfer doc = this.Base.Transfer.Current;
            if (doc == null) return adapter.Get();
            CATransfer orgTransfer = CATransfer.PK.Find(this.Base, doc.OrigTransferNbr);
            var FTRGraph = PXGraph.CreateInstance<FundTransferRequestEntry>();

            // Fetch FTR relevant to  Original Transfer record
            FundTransferRequest orgTranRequest = FundTransferRequest.UK.Find(FTRGraph, orgTransfer?.InExtRefNbr);
            bool fromFTR = (doc.CreatedByScreenID == "GS301015" || orgTranRequest != null);
            try
            {
                /// If a reversal
                if (fromFTR && doc?.OrigTransferNbr != null)
                {
                    //Fetch the Original Transfer record
                    if (orgTransfer != null)
                    {
                        if (orgTranRequest != null)
                        {
                            //Calculate amounts
                            var inExisitngTotal = GetExistingTotalIn(FTRGraph, doc, orgTranRequest);
                            var outExisitngTotal = GetExistingTotalOut(FTRGraph, doc, orgTranRequest);
                            var requestedTotal = orgTranRequest.Amount;
                            var currentInAmount = doc.CuryTranIn;
                            var currentOutAmount = doc.CuryTranOut;
                            FTRGraph.EmpRequest.Current = orgTranRequest;

                            if (orgTranRequest.CashAccntID == doc.OutAccountID) // Fund  Out
                            {
                                decimal? openBal = 0;
                                openBal = requestedTotal - (inExisitngTotal - outExisitngTotal) + currentOutAmount;
                                FTRGraph.EmpRequest.Current.OpenBalance = openBal;
                                if (FTRGraph.EmpRequest.Current.OpenBalance > requestedTotal)
                                {
                                    throw new PXException(GSynchExt.Messages.ValidateReversedCAAmount, inExisitngTotal);
                                }
                                if (FTRGraph.EmpRequest.Current.OpenBalance != decimal.Zero)
                                {
                                    FTRGraph.EmpRequest.Current.Status = FTRStatus.Released;
                                }
                                else
                                {
                                    FTRGraph.EmpRequest.Current.Status = FTRStatus.Closed;
                                }
                                FTRGraph.EmpRequest.Current.Requested = true;
                                FTRGraph.EmpRequest.Update(FTRGraph.EmpRequest.Current);
                                FTRGraph.Actions.PressSave();
                            }
                            if (orgTranRequest.CashAccntID == doc.InAccountID) //Fund In
                            {
                                decimal? openBal = 0;
                                openBal = requestedTotal - (inExisitngTotal - outExisitngTotal) - currentInAmount;
                                FTRGraph.EmpRequest.Current.OpenBalance = openBal;
                                if (FTRGraph.EmpRequest.Current.OpenBalance < 0)
                                {
                                    throw new PXException(GSynchExt.Messages.ValidateCAAmount, orgTranRequest.OpenBalance);
                                }
                                if (FTRGraph.EmpRequest.Current.OpenBalance != decimal.Zero)
                                {
                                    FTRGraph.EmpRequest.Current.Status = FTRStatus.Released;
                                }
                                else
                                {
                                    FTRGraph.EmpRequest.Current.Status = FTRStatus.Closed;
                                }
                                FTRGraph.EmpRequest.Current.Requested = true;
                                FTRGraph.EmpRequest.Update(FTRGraph.EmpRequest.Current);
                                FTRGraph.Actions.PressSave();
                            }
                        }
                    }
                }
                else /// Not a reversal
                {
                    FundTransferRequest transferRequest = FundTransferRequest.UK.Find(FTRGraph, doc.InExtRefNbr);
                    if (transferRequest != null)
                    {
                        FTRGraph.EmpRequest.Current = transferRequest;
                        FTRGraph.EmpRequest.Current.Transferred = true;
                        FTRGraph.EmpRequest.Update(FTRGraph.EmpRequest.Current);
                        FTRGraph.Actions.PressSave();
                    }
                }
                return BaseMethod.Invoke(adapter);
            }
            catch (Exception e)
            {
                throw new PXException(e.Message);
            }

        }

        protected virtual void _(Events.RowDeleting<CATransfer> e)
        {
            CATransfer row = e.Row as CATransfer;
            if (row == null) return;

            // Fetch FTR relevant to  Current Transfer record
            FundTransferRequest tranRequest = FundTransferRequest.UK.Find(this.Base, row.InExtRefNbr);
            bool fromFTR = (row.CreatedByScreenID == "GS301015" || tranRequest != null);

            if (!fromFTR && row.InExtRefNbr == null) return;

            bool isDeleted = e.Cache.GetStatus(row) == PXEntryStatus.Deleted;

            if (isDeleted)
            {
                var FTRGraph = PXGraph.CreateInstance<FundTransferRequestEntry>();

                FundTransferRequest transferRequest = FundTransferRequest.UK.Find(FTRGraph, row.InExtRefNbr);

                if (transferRequest == null) return;
                FTRGraph.EmpRequest.Current = transferRequest;
                FTRGraph.EmpRequest.Current.Requested = false;
                FTRGraph.EmpRequest.Current.Transferred = false;
                FTRGraph.EmpRequest.Current.OpenBalance += row?.CuryTranIn;
                FTRGraph.EmpRequest.Update(FTRGraph.EmpRequest.Current);
                FTRGraph.Actions.PressSave();
            }
        }

        protected decimal GetExistingTotalIn(FundTransferRequestEntry FTRGraph, CATransfer doc, FundTransferRequest orgTranRequest)
        {
            CATransfer fTransferIn = PXSelectGroupBy<CATransfer,
             Where<CATransfer.inExtRefNbr, Equal<Required<CATransfer.inExtRefNbr>>,
             And<CATransfer.transferNbr, NotEqual<Required<CATransfer.transferNbr>>,
             And<CATransfer.inAccountID, Equal<Required<CATransfer.inAccountID>>,
             And<CATransfer.status, Equal<CATransferStatus.released>>>>>,
             Aggregate<GroupBy<CATransfer.inExtRefNbr, Sum<CATransfer.curyTranIn>>>>.Select(FTRGraph, doc.InExtRefNbr, doc.TransferNbr, orgTranRequest.CashAccntID);
            if (fTransferIn != null)
            {
                return (decimal)fTransferIn.CuryTranIn;
            }
            else
            {
                return decimal.Zero;
            }
        }
        protected decimal GetExistingTotalOut(FundTransferRequestEntry FTRGraph, CATransfer doc, FundTransferRequest orgTranRequest)
        {
            CATransfer fTransferOut = PXSelectGroupBy<CATransfer,
                 Where<CATransfer.outExtRefNbr, Equal<Required<CATransfer.outExtRefNbr>>,
                 And<CATransfer.outAccountID, Equal<Required<CATransfer.outAccountID>>,
                 And<CATransfer.status, Equal<CATransferStatus.released>,
                 And<CATransfer.transferNbr, NotEqual<Required<CATransfer.transferNbr>>>>>>,
                 Aggregate<GroupBy<CATransfer.outExtRefNbr, GroupBy<CATransfer.outAccountID, Sum<CATransfer.curyTranOut>>>>>.Select(FTRGraph, doc.OutExtRefNbr, orgTranRequest.CashAccntID, doc.TransferNbr);

            if (fTransferOut != null)
            {
                return (decimal)fTransferOut.CuryTranOut;
            }
            else
            {
                return decimal.Zero;
            }
        }
    }
}