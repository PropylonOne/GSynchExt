using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.IN;
using System.Linq;
using PX.Objects.CR;
using PX.Objects.CA;
using PX.Common;
using PX.SM;
using PX.Objects.CS;
using static PX.Objects.CA.CABankTran.FK;
using PX.Objects.GL;
using Branch = PX.SM.Branch;

namespace GSynchExt
{
    public class UploadBankStatementMaint : PXGraph<UploadBankStatementMaint, UploadBankStatement>
    {

        #region Views

        [PXImport(typeof(UploadBankStatement))]
        public SelectFrom<UploadBankStatement>.View BankStatement;

        public SelectFrom<BankStatementInvoices>.View viewInvo2;

        public PXSelectJoin<ARTran, FullJoin<ARInvoice,
                                On<ARTran.refNbr, Equal<ARInvoice.refNbr>,
                                And<ARTran.tranType, Equal<ARInvoice.docType>,
                                And<ARInvoice.status, Equal<ARDocStatus.closed>,
                                And<ARTran.inventoryID, StartsWith<nonInventory>>>>>>> Invoice2;


        #endregion

        #region Constructor
        public UploadBankStatementMaint()
        {
            //clearLines.SetEnabled(false);
            //validateLines.SetEnabled(false);
            //createOrders.SetEnabled(false);
        }

        #endregion
        #region Events
        protected virtual void _(Events.RowSelected<UploadBankStatement> e)
        {
            UploadBankStatement doc = (UploadBankStatement)e.Row;
            if (doc == null) return;

            loadInvData.SetEnabled(!e.Cache.IsDirty);
            matchAmountsAction.SetEnabled(!e.Cache.IsDirty && this.viewInvo2.Current != null);
        }
        #endregion
        #region Actions

        public PXAction<UploadBankStatement> validateLines;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Validate",
           Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable ValidateLines(PXAdapter adapter)
        {
            UploadBankStatement row = BankStatement.Current;
            PXCache cache = BankStatement.Cache;
            PXGraph graph = cache.Graph;

            if (row == null) return adapter.Get();
            var res = BankStatement.Select();

            ARInvoiceEntry aRInvoice = PXGraph.CreateInstance<ARInvoiceEntry>();

            var invoice = PXSelectJoin<ARInvoice, LeftJoin<ARTran,
                                On<ARInvoice.refNbr, Equal<ARTran.refNbr>,
                                And<ARInvoice.docType, Equal<ARTran.tranType>,
                                And<ARTran.inventoryID, StartsWith<nonInventory>>>>>>.Select(aRInvoice);

            var tran = PXSelectJoin<ARTran, LeftJoin<ARInvoice,
                                On<ARTran.refNbr, Equal<ARInvoice.refNbr>,
                                And<ARTran.tranType, Equal<ARInvoice.docType>,
                                And<ARTran.inventoryID, StartsWith<nonInventory>>>>>>.Select(aRInvoice);

            ARInvoice line = invoice;
            ARInvoice line2 = invoice;

            if (res != null)
            {
                foreach (PXResult<UploadBankStatement> rec in res)
                {
                    UploadBankStatement bankStatement = (UploadBankStatement)rec;

                    //Validate all un-processed rows.
                    if (bankStatement.Processed == false)
                    {
                        //Clear previous Remarks
                        /* bankStatement.Province = null;
                         bankStatement.ClusterID = null;
                         bankStatement.PhaseID = null;
                         bankStatement.TransactionID = null;
                         bankStatement.District = null;*/

                        var sites = PXSelect<SolarSite, Where<SolarSite.cEBAccount, Equal<Required<SolarSite.cEBAccount>>>>.Select(this, bankStatement.CEBAccount);
                        foreach (PXResult<SolarSite> site in sites)
                        {
                            SolarSite siteRes = (SolarSite)site;
                            bankStatement.SolarSiteID = siteRes.SolarSiteCD;
                            bankStatement.Province = siteRes.Province;
                            bankStatement.ClusterID = siteRes.ClusterID;
                            bankStatement.PhaseID = siteRes.PhaseID;
                            bankStatement.District = siteRes.District;
                            bankStatement.TransactionDate = bankStatement.TransactionDate ?? this.Accessinfo.BusinessDate;
                            if (bankStatement.OpenAmount == 0 || bankStatement.OpenAmount == null)
                            {
                                bankStatement.OpenAmount = bankStatement.CEBAmount;
                            }
                            /// Check if the Non stock Item exists. If not create record
                            /// InventoryCD of SOlar Site          
                            var itemCD = "SS-" + bankStatement.SolarSiteID;

                            ///Find records related to InventoryCD
                            InventoryItem inventory = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD,
                                Equal<Required<InventoryItem.inventoryCD>>>>.Select(this, itemCD);
                            if (inventory == null)
                            {
                                SolarSiteEntry ssGraph = PXGraph.CreateInstance<SolarSiteEntry>();
                                ssGraph.Site.Current = siteRes;
                                ssGraph.CreateSSNonStockItem(ssGraph, siteRes);
                            }
                        }
                    }
                    bankStatement = (UploadBankStatement)graph.Caches[typeof(UploadBankStatement)].Update(bankStatement);
                    //return adapter.Get();
                }
            }
            return adapter.Get();
        }


        public PXAction<BankStatementInvoices> loadInvData;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Fetch All",
        Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable LoadInvData(PXAdapter adapter)
        {
            loadInvDataFromStatement();
            return adapter.Get();
        }

        public PXAction<BankStatementInvoices> matchAmountsAction;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Match & Pay",
        Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable MatchAmountsAction(PXAdapter adapter)
        {
            PXResultset<BankStatementInvoices> results = this.viewInvo2.Select();
            var upresults = this.BankStatement.Select().Where(x => x.Record.Processed == null || x.Record.Processed == false);
            if (results?.Count > 0 && upresults != null)
            {
                matchAmounts();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.NoInvLines);
            }

            return adapter.Get();
        }

        public PXAction<BankStatementInvoices> ClearInvoiceRecords;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Clear Invoice Records",
        Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable clearInvoiceRecords(PXAdapter adapter)
        {
            PXResultset<BankStatementInvoices> results = this.viewInvo2.Select();

            foreach (BankStatementInvoices record in results)
            {
                viewInvo2.Delete(record);
            }
            Actions.PressSave();
            return adapter.Get();
        }



        public PXAction<UploadBankStatement> ClearStatementRecords;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Clear Statememnt records",
        Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable clearStatementRecords(PXAdapter adapter)
        {
            PXResultset<UploadBankStatement> results = this.BankStatement.Select();

            foreach (UploadBankStatement record in results)
            {
                BankStatement.Delete(record);
            }
            Actions.PressSave();
            return adapter.Get();
        }

        #endregion

        #region Methods

        public void loadInvDataFromStatement()
        {
            PXLongOperation.StartOperation(this, delegate ()
            {
                var results = this.BankStatement.Select();
                foreach (UploadBankStatement bankStatement in results)
                {
                    if ((bankStatement.Processed == null || bankStatement.Processed == false) && (bankStatement.Matched == null || bankStatement.Matched == false))
                    {
                        /// InventoryCD of SOlar Site          
                        var itemCD = "SS-" + bankStatement.SolarSiteID;

                        ///Find records related to InventoryCD
                        InventoryItem inventory = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD,
                            Equal<Required<InventoryItem.inventoryCD>>>>.Select(this, itemCD);
                        if (inventory == null) continue;
                        ///Find ARTran records related to InventoryID that has an unpaid balance and created before the statement date         
                        var arInvTran = PXSelect<ARTran, Where<ARTran.inventoryID, Equal<Required<ARTran.inventoryID>>,
                            And<ARTran.tranBal, NotEqual<decimal0>,
                            And<ARTran.tranDate, LessEqual<Required<ARTran.tranDate>>>>>>.Select(this, inventory.InventoryID, bankStatement.TransactionDate);
                        if (arInvTran == null) continue;
                        foreach (PXResult<ARTran> rec in arInvTran)
                        {
                            ARTran tran = rec;
                            // if (tran.CustomerID ==  ) ;

                            BankStatementInvoices bsInvoices = new BankStatementInvoices
                            {
                                InventoryID = tran.InventoryID,
                                RefNbr = tran.RefNbr,
                                TranDate = tran.TranDate,
                                TranAmt = tran.TranAmt,
                                TranBal = tran.TranBal,
                                TransactionID = bankStatement.TransactionID,
                                Province = bankStatement.Province,
                                CashAccountCD = bankStatement.CashAccountCD,
                                LineNbr = tran.LineNbr
                            };

                            this.viewInvo2.Update(bsInvoices);
                        }
                        ///Set the Matched flag flase for all records in BankStatementInvoices table
                        bankStatement.Matched = false;
                        this.BankStatement.Update(bankStatement);
                    }
                }
                Actions.PressSave();
            });
        }
        protected void ValidateFetchedInvoices()
        {
            string openrecs = null;

            foreach (BankStatementInvoices rec in this.viewInvo2.Select())
            {
                ///check any of the Fetched invoices have any unreleased payments
                ARAdjust openPayments = PXSelect<ARAdjust,
                                        Where<ARAdjust.adjdDocType, Equal<ARDocType.invoice>,
                                        And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>,
                                        And<ARAdjust.adjdLineNbr, Equal<Required<ARAdjust.adjdLineNbr>>,
                                        And<ARAdjust.released, NotEqual<True>>>>>>.Select(this, rec.RefNbr, rec.LineNbr);
                if (openPayments != null)
                {
                    if (openrecs == null) openrecs = openPayments.AdjgRefNbr;
                    else
                    {
                        openrecs += "," + openPayments.AdjgRefNbr;
                    }
                }
            }
            if (openrecs != null) throw new PXException(Messages.UnlreleasedPayments, openrecs);
        }
        public void matchAmounts()
        {
            PXLongOperation.StartOperation(this, delegate ()
            {
                try
                {
                    ValidateFetchedInvoices();
                    ///Group the loaded records by Province
                    var recByProvince = PXSelectGroupBy<UploadBankStatement,
                                        Aggregate<GroupBy<UploadBankStatement.province, GroupBy<UploadBankStatement.cashAccountCD>>>>.Select(this);
                    bool hasLines = false;
                    foreach (var res in recByProvince)
                    {
                        UploadBankStatement statements = (UploadBankStatement)res;
                        var province = statements.Province;
                        /// Customer name 
                        var custName = "CEB-" + province;

                        /// Find the related customer record
                        var customer = PXSelect<BAccount, Where<BAccount.acctCD,
                                                Equal<Required<BAccount.acctCD>>>>
                                                .Select(this, custName);
                        if (province == "UVA") { province = "U"; }

                        var branchCD = "GG" + province;
                        Branch branchRec = PXSelect<Branch,
                                            Where<Branch.branchCD,
                                            Equal<Required<Branch.branchCD>>>>
                                            .Select(this, branchCD);
                        if (branchRec == null) continue;
                        CashAccount cAccount = PXSelect<PX.Objects.CA.CashAccount,
                                                Where<PX.Objects.CA.CashAccount.cashAccountCD,
                                                Equal<Required<CashAccount.cashAccountCD>>>>
                                                .Select(this, statements.CashAccountCD);

                        BAccount cust = customer;
                        if (cust == null) continue;

                        ///Create Payment Header
                        ARPaymentEntry aRPaymentEntry = PXGraph.CreateInstance<ARPaymentEntry>();

                        var payment = aRPaymentEntry.Document.Insert(new ARPayment
                        {
                            AdjDate = DateTime.Today,
                            CustomerID = cust.BAccountID,
                            CashAccountID = cAccount.CashAccountID,
                            DocDesc = "CEB Payment from Bank upload " + statements.TransactionDate.ToString(),
                            BranchID = branchRec.BranchID
                        });

                        aRPaymentEntry.Document.Update(payment);

                        var paymentCache = aRPaymentEntry.Caches[typeof(ARPayment)];

                        ///Add Lines
                        PXResultset<UploadBankStatement> rec = PXSelect<UploadBankStatement,
                            Where<UploadBankStatement.province, Equal<Required<UploadBankStatement.province>>,
                            And<UploadBankStatement.cashAccountCD, Equal<Required<UploadBankStatement.cashAccountCD>>>>>.Select(this, statements.Province, statements.CashAccountCD);
                        foreach (UploadBankStatement bankStatement in rec)
                        {

                            ///Select tran records by InventoryID
                            var itemCD = "SS-" + bankStatement.SolarSiteID;
                            InventoryItem tranINItem = InventoryItem.UK.Find(this, itemCD);

                            var rec2 = this.viewInvo2.Select().Where(x => x.Record.InventoryID == tranINItem.InventoryID).OrderBy(y => y.Record.TranDate);
                            var cebAmount = bankStatement.CEBAmount;

                            ///First check if the CEB amount can be fully matched
                            BankStatementInvoices canMatch = PXSelectGroupBy<BankStatementInvoices,
                            Where<BankStatementInvoices.inventoryID, Equal<Required<BankStatementInvoices.inventoryID>>>,
                            Aggregate<GroupBy<BankStatementInvoices.inventoryID, Sum<BankStatementInvoices.tranBal>>>>.Select(this, tranINItem.InventoryID);
                            if (canMatch == null) continue;
                            if (canMatch.TranBal < cebAmount) continue;

                            foreach (BankStatementInvoices invoice in rec2)
                            {
                                var tran = PXSelect<ARTran,
                                   Where<ARTran.inventoryID, Equal<Required<ARTran.inventoryID>>,
                                   And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
                                   And<ARTran.tranType, Equal<ARDocType.invoice>>>>>.Select(this, invoice.InventoryID, invoice.RefNbr);

                                foreach (ARTran invToMatch in tran)
                                {

                                    if (invoice.TranBal >= cebAmount && bankStatement.Matched == false) ///CEBAmount is less than the invoice amount
                                    {
                                        ARAdjust adj = new ARAdjust();
                                        adj.AdjdDocType = ARDocType.Invoice;
                                        adj.AdjdRefNbr = invToMatch.RefNbr;
                                        adj.AdjdBranchID = branchRec.BranchID;

                                        //set origamt to zero to apply "full" amounts to invoices.

                                        paymentCache.SetValueExt<ARPayment.curyOrigDocAmt>(payment, 0m);
                                        adj.AdjdLineNbr = invToMatch.LineNbr;
                                        adj.AdjdDocDate = payment.DocDate;
                                        adj.CustomerID = cust.BAccountID;
                                        adj.CuryAdjgAmt = cebAmount;


                                        aRPaymentEntry.Adjustments.Insert(adj);
                                        ///Update the Invoice line loaded for matching
                                        invoice.TranBal -= cebAmount;
                                        cebAmount = 0;
                                        viewInvo2.Update(invoice);
                                        hasLines = true;
                                        ///Update the Loaded bank entry
                                        bankStatement.OpenAmount = cebAmount;
                                        if (bankStatement.OpenAmount == 0)
                                        {
                                            bankStatement.Matched = true;
                                            bankStatement.Processed = true;
                                            bankStatement.RefNbr = adj.AdjdRefNbr;
                                        }
                                        this.BankStatement.Update(bankStatement);
                                    }
                                    if (invoice.TranBal < cebAmount && bankStatement.Matched == false) ///CEB amount is more than the Invoice balance
                                    {

                                        ARAdjust adj2 = new ARAdjust();
                                        adj2.AdjdDocType = ARDocType.Invoice;
                                        adj2.AdjdRefNbr = invToMatch.RefNbr;
                                        adj2.AdjdBranchID = branchRec.BranchID;

                                        //set origamt to zero to apply "full" amounts to invoices.

                                        paymentCache.SetValueExt<ARPayment.curyOrigDocAmt>(payment, 0m);

                                        adj2.AdjdDocDate = payment.DocDate;
                                        adj2.CustomerID = cust.BAccountID;
                                        adj2.AdjdLineNbr = invToMatch.LineNbr;
                                        adj2.CuryAdjgAmt = invoice.TranBal;
                                        var details = aRPaymentEntry.Adjustments.Insert(adj2);
                                        hasLines = true;

                                        cebAmount -= invoice.TranBal;
                                        invoice.TranBal = 0;
                                        viewInvo2.Update(invoice);

                                        bankStatement.OpenAmount = cebAmount;
                                        if (bankStatement.OpenAmount == 0)
                                        {
                                            bankStatement.Matched = true;
                                            bankStatement.Processed = true;
                                            bankStatement.RefNbr = adj2.AdjdRefNbr;
                                        }
                                        this.BankStatement.Update(bankStatement);
                                    }
                                }
                            }
                        }
                        if (!hasLines)
                        {
                            throw new PXException(GSynchExt.Messages.NoInvLines);
                        }
                        else
                        {
                            ///Save at the end to avoid blank payments getting created.
                            aRPaymentEntry.Actions.PressSave();                         
                            Actions.PressSave();
                            var reference = aRPaymentEntry.Document.Current.RefNbr;
                            if (BankStatement.Ask( $"Reference No. of Created Payment : {reference} ", MessageButtons.OK) != WebDialogResult.OK) return;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("Reference No."))
                    {

                        throw new PXException(e.Message);
                    }
                }
            });


        }
        #endregion
    }
}
