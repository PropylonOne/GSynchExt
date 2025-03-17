using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Newtonsoft.Json;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using static PX.Objects.PM.GSProjectHelper;
using PX.Objects.CM;
using PX.Objects.AP;

namespace GSynchExt
{
    public class SolarRevGenEntry : PXGraph<SolarRevGenEntry, SolarRevGen>
    {
        public PXSelect<SolarRevGen> Document;
        public PXSelect<SolarRevGen, Where<SolarRevGen.solarRevGenID, Equal<Current<SolarRevGen.solarRevGenID>>>> CurrentDocument;

        [PXImport]
        public PXSelect<SolarRevGenDetails, Where<SolarRevGenDetails.solarRevGenID, Equal<Current<SolarRevGen.solarRevGenID>>>> SiteDetails;

        [PXHidden]
        public PXSetup<SiteSetup> AutoNumSetup;

        #region Constructor
        public SolarRevGenEntry()
        {
            SiteSetup setup = AutoNumSetup.Current;

        }
        #endregion

        #region Events
        protected virtual void _(Events.FieldUpdating<SolarRevGen, SolarRevGen.invoiceDate> e)
        {
            if (e.Row == null) return;
            SolarRevGen row = e.Row as SolarRevGen;
            var checkExist = SolarRevGen.UK2.Find(this, row.Province, row.Period);
            if (checkExist != null && checkExist?.SolarRevGenID != row.SolarRevGenID)
            {
                throw new PXException(Messages.SolarSalesRevExist, checkExist.SolarRevGenID);
            }
        }
        protected virtual void _(Events.RowSelected<SolarRevGen> e)
        {
            SolarRevGen row = e.Row as SolarRevGen;
            if (row != null)
            {
                CreateInvoice.SetEnabled(this.CurrentDocument.Current.Status == GSynchExt.Status.Released);
                Release.SetEnabled(this.CurrentDocument.Current.Status == GSynchExt.Status.OnHold);
                e.Cache.AllowInsert = true;
                e.Cache.AllowUpdate = (row.Status == GSynchExt.Status.OnHold || row.Status == GSynchExt.Status.Released);
                e.Cache.AllowDelete = (row.Status == GSynchExt.Status.OnHold);
                this.SiteDetails.Cache.AllowInsert = (row.Status == GSynchExt.Status.OnHold && row.SiteBillRefNbr == null && row.Rrrefnbr == null && row.Ssrefnbr == null);
                this.SiteDetails.Cache.AllowUpdate = (row.Status == GSynchExt.Status.OnHold && row.SiteBillRefNbr == null && row.Rrrefnbr == null && row.Ssrefnbr == null);
                this.SiteDetails.Cache.AllowDelete = (row.Status == GSynchExt.Status.OnHold && row.SiteBillRefNbr == null && row.Rrrefnbr == null && row.Ssrefnbr == null);

                PXUIFieldAttribute.SetEnabled<SolarRevGen.solarRevGenID>(e.Cache, row, row.Status != GSynchExt.Status.Released);
                PXUIFieldAttribute.SetEnabled<SolarRevGen.invoiceDate>(e.Cache, row, row.Status != GSynchExt.Status.Released);
                PXUIFieldAttribute.SetEnabled<SolarRevGen.customerID>(e.Cache, row, row.Status != GSynchExt.Status.Released);
                PXUIFieldAttribute.SetEnabled<SolarRevGen.period>(e.Cache, row, row.Status != GSynchExt.Status.Released);
                PXUIFieldAttribute.SetEnabled<SolarRevGen.province>(e.Cache, row, row.Status != GSynchExt.Status.Released);
                PXUIFieldAttribute.SetEnabled<SolarRevGen.period>(e.Cache, row, false);

            }
        }
        protected virtual void _(Events.RowSelected<SolarRevGenDetails> e)
        {
            SolarRevGenDetails row = e.Row as SolarRevGenDetails;
            if (row != null)
            {
                SolarSite solarSite = SolarSite.PK.Find(this, row.SolarSiteID);
                row.CEBAccount = solarSite?.CEBAccount;
                row.SiteStatus = solarSite?.SiteStatus;
             //   row.SolarSalesAmount = solarSite?.;
                ///Enable - Disable Actions
                LoadSites.SetEnabled(!this.CurrentDocument.Cache.IsDirty && this.CurrentDocument.Current.Status == GSynchExt.Status.OnHold);
                bool hasLines = this.SiteDetails.Current != null;
                PrepareData.SetEnabled(hasLines && !e.Cache.IsDirty && this.CurrentDocument.Current.Status == GSynchExt.Status.OnHold);
            }
        }
        /*protected virtual void _(Events.FieldUpdating<SolarRevGen, SolarRevGen.period> e)
        {
            SolarRevGen row = e.Row as SolarRevGen;
            if (row == null) return;
            var newPeriod = e.NewValue;
            var finRec = (OrganizationFinPeriod)PXSelect<OrganizationFinPeriod,
                        Where<OrganizationFinPeriod.finPeriodID, Equal<Required<OrganizationFinPeriod.finPeriodID>>,
                        And<OrganizationFinPeriod.organizationID, Equal<Required<OrganizationFinPeriod.organizationID>>>>>.Select(this, newPeriod, this.CurrentDocument?.Current?.BranchID);
            if (finRec == null) return;
            row.InvoiceDate = finRec.EndDate;
        }*/
        protected virtual void _(Events.FieldUpdated<SolarRevGen, SolarRevGen.invoiceDate> e)
        {
            SolarRevGen row = e.Row as SolarRevGen;
            if (row == null) return;
            e.Cache.SetDefaultExt<SolarRevGen.period>(row);
        }

        protected virtual void _(Events.FieldUpdating<SolarRevGenDetails, SolarRevGenDetails.actQty> e)
        {
            SolarRevGenDetails row = e.Row as SolarRevGenDetails;
            if (row == null) return;
            var newQty = e.NewValue;
            if (newQty != null)
            {
                row.MngFeeAmount = CalculateMngFeeAmount(row);
                row.StampDutyAmount = CalculateStampDuty(row);
            }
        }

        protected virtual void _(Events.FieldUpdating<SolarRevGenDetails, SolarRevGenDetails.estQty> e)
        {
            SolarRevGenDetails row = e.Row as SolarRevGenDetails;
            if (row == null) return;
            var newQty = e.NewValue;
            if (newQty != null)
            {
                row.MngFeeAmount = CalculateMngFeeAmount(row);
                row.StampDutyAmount = CalculateStampDuty(row);
            }
        }
        protected virtual void _(Events.FieldUpdating<SolarRevGenDetails, SolarRevGenDetails.inverterQty> e)
        {
            SolarRevGenDetails row = e.Row as SolarRevGenDetails;
            if (row == null) return;
            var newQty = e.NewValue;
            if (newQty != null)
            {
                row.MngFeeAmount = CalculateMngFeeAmount(row);
            }
        }

        protected virtual void _(Events.FieldUpdating<SolarRevGenDetails, SolarRevGenDetails.tariff> e)
        {
            SolarRevGenDetails row = e.Row as SolarRevGenDetails;
            if (row == null) return;
            var newTariff = e.NewValue;
            if (newTariff != null)
            {
                row.MngFeeAmount = CalculateMngFeeAmount(row);
                row.StampDutyAmount = CalculateStampDuty(row);
            }
        }

        #endregion
        #region Actions


        public PXAction<SolarRevGenDetails> LoadSites;
        [PXButton(), PXUIField(DisplayName = "Load Sites", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]

        protected virtual IEnumerable loadSites(PXAdapter adapter)
        {
            if (CurrentDocument.Current.Status == GSynchExt.Status.OnHold && CurrentDocument.Current.SolarRevGenID != null)
            {
                SiteDetails.Cache.ForceExceptionHandling = true;
                var branch = (Branch)PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.SelectSingleBound(this, null, Document.Current.BranchID ?? this.Accessinfo.BranchID);
                var periodRec = FinPeriod.PK.Find(this, branch.OrganizationID, (String)CurrentDocument.Current.Period);

                HashSet<SiteKeyTuple> existing = GetExistingSolarSites();
                ///TODO it should be Completed Date
                var solarSiteSelect = PXSelect<SolarSite,
                        Where<SolarSite.province, Equal<Required<SolarSite.province>>,
                        And<SolarSite.connectedtoGridDate, LessEqual<Required<SolarSite.connectedtoGridDate>>,
                        And<SolarSite.siteStatus, NotEqual<Status.cancelled>,
                        And<SolarSite.siteStatus, NotEqual<Status.suspended>>>>>>.Select(this, CurrentDocument.Current.Province, periodRec.EndDate);

                foreach (SolarSite line in solarSiteSelect)
                {
                    var sSite = (SolarSite)line;
                    if (!existing.Contains(SiteKeyTuple.Create(line)))
                    {
                        SolarRevGenDetails newline = new SolarRevGenDetails();
                        newline.SolarSiteID = sSite.SolarSiteID;
                        newline.BranchID = CurrentDocument.Current.BranchID;
                        newline.SolarRevGenID = CurrentDocument.Current.SolarRevGenID;
                        newline.Active = true;
                        newline.Processed = false;
                        newline.Error = false;
                        SiteDetails.Insert(newline);
                    }
                }
            }
            return adapter.Get();
        }

        protected virtual HashSet<SiteKeyTuple> GetExistingSolarSites()
        {
            HashSet<SiteKeyTuple> existing = new HashSet<SiteKeyTuple>();
            foreach (SolarRevGenDetails line in SiteDetails.Select())
            {
                var sSite = SolarSite.PK.Find(this, line.SolarSiteID);
                existing.Add(SiteKeyTuple.Create(sSite));
            }

            return existing;
        }

        public PXAction<SolarRevGen> PrepareData;
        [PXButton(), PXUIField(DisplayName = "Prepare Data", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable prepareData(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate (){
                try
                {
                    foreach (SolarRevGenDetails line in SiteDetails.Select())
                    {
                        var newLine = (SolarRevGenDetails)line;
                        try
                        {
                            ///Reset
                            newLine.Error = false;
                            newLine.Remark = null;
                            newLine.Processed = false;

                            ///Header defaults
                            RRRates rrRateRec = PXSelect<RRRates, Where<RRRates.province, Equal<Required<RRRates.province>>,
                                And<RRRates.expDate, GreaterEqual<Required<RRRates.expDate>>>>>.Select(this, CurrentDocument.Current.Province, CurrentDocument.Current.InvoiceDate);
                            Customer custRec = Customer.PK.Find(this, this.CurrentDocument.Current.CustomerID);
                            CurrencyRate curRec = (CurrencyRate)PXSelect<CurrencyRate,
                                        Where<CurrencyRate.fromCuryID, Equal<Required<CurrencyRate.fromCuryID>>,
                                        And<CurrencyRate.toCuryID, Equal<Required<CurrencyRate.toCuryID>>,
                                        And<CurrencyRate.curyRateType, Equal<Required<CurrencyRate.curyRateType>>,
                                        And<CurrencyRate.curyEffDate, LessEqual<Required<CurrencyRate.curyEffDate>>>>>>,
                                        OrderBy<Desc<CurrencyRate.curyEffDate>>>.Select(this, "USD", this.Accessinfo.BaseCuryID, custRec?.CuryRateTypeID, this.CurrentDocument.Current.InvoiceDate).FirstOrDefault();

                            if (newLine.Active == true)
                            {
                                /// Get Inventory Item, Customer & Currency defaults
                                var sSite = (SolarSite)PXSelect<SolarSite, Where<SolarSite.solarSiteID, Equal<Required<SolarRevGenDetails.solarSiteID>>>>.Select(this, newLine.SolarSiteID);
                                String ssItem = String.Concat(GSynchExt.ItemPrefix.SSItem, sSite?.SolarSiteCD);
                                String rrItem = String.Concat(GSynchExt.ItemPrefix.RRItem, sSite?.SolarSiteCD);
                                InventoryItem ssRec = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(this, ssItem);
                                if (ssRec == null)
                                {
                                    var solarSiteGraph = new SolarSiteEntry();
                                    solarSiteGraph.Site.Current = sSite;
                                    solarSiteGraph.CreateSSNonStockItem(sSite);
                                    ssRec = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(this, ssItem);

                                }
                                newLine = ValidateRecord(line, ssRec, rrRateRec, curRec);
                                if (newLine.Error == false)
                                {
                                    var totalSolarSales = (newLine.ActQty ?? newLine.EstQty ?? newLine.InverterQty) * newLine.Tariff;
                                    var totalKW = totalSolarSales / 24;

                                    newLine.MngFeeAmount = (this.AutoNumSetup.Current?.MfRate ?? 0) * curRec.CuryRate * totalKW;
                                    

                                    if (totalSolarSales >= AutoNumSetup?.Current?.StampDutyLimit)
                                        newLine.StampDutyAmount = AutoNumSetup?.Current?.StampDutyAmount;

                                    if (rrRateRec != null)
                                    {
                                        newLine.RoofRentPercnt = rrRateRec?.Rrrate;
                                        newLine.RoofRentAmount = totalSolarSales * newLine.RoofRentPercnt / 100 ;
                                    }
                                    newLine.SolarSalesAmount = totalSolarSales - newLine.StampDutyAmount - newLine.SiteBillAmount;
                                    newLine.Processed = string.IsNullOrEmpty(newLine.Remark);
                                    newLine.Error = !string.IsNullOrEmpty(newLine.Remark);
                                }
                                else
                                {
                                    newLine.Processed = string.IsNullOrEmpty(newLine.Remark);
                                    newLine.Error = !string.IsNullOrEmpty(newLine.Remark);
                                }
                                SiteDetails.Update(newLine);
                            }
                        }
                        catch (Exception e)
                        {
                            newLine.Remark = AddError(newLine, e.Message);
                            newLine.Error = !string.IsNullOrEmpty(newLine.Remark);
                            newLine.Processed = string.IsNullOrEmpty(newLine.Remark);
                            SiteDetails.Update(newLine);
                            this.Actions.PressSave();
                        }
                    }
                    this.Actions.PressSave();
                }
                catch (Exception e)
                {
                    var currentRow = this.SiteDetails.Current;
                    currentRow.Remark = AddError(currentRow, e.Message);
                    currentRow.Error = !string.IsNullOrEmpty(currentRow.Remark);
                    currentRow.Processed = string.IsNullOrEmpty(currentRow.Remark);
                    SiteDetails.UpdateCurrent();
                    this.Actions.PressSave();
                }
            });
            return adapter.Get();
        }

        public PXAction<SolarRevGen> ClearErrors;
        [PXUIField(DisplayName = "Clear Errors",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]

        protected virtual IEnumerable clearErrors(PXAdapter adapter)
        {
            foreach (SolarRevGenDetails line in SiteDetails.Select())
            {
                line.Error = false;
                line.Remark = null;
                line.Processed = false;
                SiteDetails.Update(line);
            }
            this.Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<SolarRevGen> CreateInvoice;
        [PXButton(), PXUIField(DisplayName = "Create Invoices",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]

        protected virtual IEnumerable createInvoice(PXAdapter adapter)
        {
            // Acuminator disable once PX1008 LongOperationDelegateSynchronousExecution [Justification]
            PXLongOperation.StartOperation(this, delegate ()
            {
                try
                {
                    /// Fetch Branch & Customers
                    /// TO DO - Fetch from Attributes
                    int pos = 3;
                    if (this.CurrentDocument.Current.Province == "UVA") pos = 1;
                    var invBranch = String.Concat("GG", this.CurrentDocument.Current.Province.Substring(0, pos));
                    var cust = String.Concat("PC-", this.CurrentDocument.Current.Province);
                    var sup = String.Concat("PC-", this.CurrentDocument.Current.Province);

                    var ssbranchRec = Branch.UK.Find(this, invBranch);
                    //var mfbranchRec = Branch.UK.Find(mfGraph, this.CurrentDocument.Current.Province);
                    //var mfCustRec = Customer.UK.Find(mfGraph, String.Concat("GG", this.CurrentDocument.Current.Province.Substring(1, pos)));
                    var sbCustRec = Customer.UK.Find(this, String.Concat("PC-", this.CurrentDocument.Current.Province));
                    var rrSupRec = Vendor.UK.Find(this, String.Concat("PC-", this.CurrentDocument.Current.Province));
                    String docdesc = null;

                    if (ssbranchRec == null) throw new PXException(Messages.BranchMissing, invBranch);
                    if (sbCustRec == null) throw new PXException(Messages.CustomerMissing, cust);
                    if (ssbranchRec == null) throw new PXException(Messages.VendorMissing, sup);

                    /// Create Headers
                    var ssGraph = PXGraph.CreateInstance<ARInvoiceEntry>(); //Solar Sales
                    var sbGraph = PXGraph.CreateInstance<ARInvoiceEntry>(); //CEB Site Bill
                    var rrGraph = PXGraph.CreateInstance<APInvoiceEntry>(); //Roof Rental
                    var ssCache = ssGraph.Caches[typeof(ARInvoice)];
                    var sbCache = sbGraph.Caches[typeof(ARInvoice)];
                    var rrCache = rrGraph.Caches[typeof(APInvoice)];

                    ARInvoice ssInv = new ARInvoice();
                    ARInvoice sbInv = new ARInvoice();
                    APInvoice rrAPInv = new APInvoice();

                    if (this.CurrentDocument.Current.Ssrefnbr == null)
                    {
                        this.CurrentDocument.Current.DocType = ARDocType.Invoice;

                        docdesc = (this.CurrentDocument.Current.Province + " Solar Sales Invoice for " + this.CurrentDocument.Current.Period.ToString());
                        ssCache.SetValueExt<ARInvoice.customerID>(ssInv, this.CurrentDocument.Current.CustomerID);
                        ssCache.SetValueExt<ARInvoice.branchID>(ssInv, ssbranchRec.BranchID);
                        ssCache.SetValueExt<ARInvoice.invoiceDate>(ssInv, this.CurrentDocument.Current.InvoiceDate);
                        ssCache.SetValueExt<ARInvoice.docDate>(ssInv, this.CurrentDocument.Current.InvoiceDate);
                        ssCache.SetValueExt<ARInvoice.paymentsByLinesAllowed>(ssInv, true);
                        ssCache.SetValueExt<ARInvoice.docDesc>(ssInv, docdesc);
                        ssCache.SetValueExt<ARInvoice.invoiceNbr>(ssInv, String.Concat("SS", this.CurrentDocument.Current.SolarRevGenID));
                        ssCache.Insert(ssInv);
                        PXTrace.WriteInformation("AR:Sales Revenue Invoice Header Created " + ssInv.RefNbr);

                    }
                    if (this.CurrentDocument.Current.SiteBillRefNbr == null)
                    {
                        this.CurrentDocument.Current.DocType = ARDocType.Invoice;

                        docdesc = (this.CurrentDocument.Current.Province + " Site Electricity Bill for " + this.CurrentDocument.Current.Period.ToString());
                        sbCache.SetValueExt<ARInvoice.customerID>(sbInv, sbCustRec?.BAccountID);
                        sbCache.SetValueExt<ARInvoice.branchID>(sbInv, ssbranchRec.BranchID);
                        sbCache.SetValueExt<ARInvoice.invoiceDate>(sbInv, this.CurrentDocument.Current.InvoiceDate);
                        sbCache.SetValueExt<ARInvoice.docDate>(sbInv, this.CurrentDocument.Current.InvoiceDate);
                        sbCache.SetValueExt<ARInvoice.paymentsByLinesAllowed>(sbInv, true);
                        sbCache.SetValueExt<ARInvoice.docDesc>(sbInv, docdesc);
                        ssCache.SetValueExt<ARInvoice.invoiceNbr>(sbInv, String.Concat("SB", this.CurrentDocument.Current.SolarRevGenID));
                        sbCache.Insert(sbInv);
                        PXTrace.WriteInformation("AR:CEB Site Bill Header Created " + sbInv.RefNbr);
                    }
                    if (this.CurrentDocument.Current.Rrrefnbr == null)
                    {
                        this.CurrentDocument.Current.DocType = APDocType.Invoice;

                        docdesc = this.CurrentDocument.Current.Province + " Roof Rental for " + this.CurrentDocument.Current.Period.ToString();
                        rrCache.SetValueExt<APInvoice.vendorID>(rrAPInv, rrSupRec?.BAccountID);
                        rrCache.SetValueExt<APInvoice.branchID>(rrAPInv, ssbranchRec.BranchID);
                        rrCache.SetValueExt<APInvoice.invoiceDate>(rrAPInv, this.CurrentDocument.Current.InvoiceDate);
                        rrCache.SetValueExt<APInvoice.docDate>(rrAPInv, this.CurrentDocument.Current.InvoiceDate);
                        rrCache.SetValueExt<APInvoice.paymentsByLinesAllowed>(rrAPInv, true);
                        rrCache.SetValueExt<APInvoice.invoiceNbr>(rrAPInv, String.Concat("RR", this.CurrentDocument.Current.SolarRevGenID));
                        rrCache.SetValueExt<APInvoice.docDesc>(rrAPInv, docdesc);
                        rrCache.Insert(rrAPInv);
                        PXTrace.WriteInformation("AP:Roof Rental Bill Header Created " + rrAPInv.RefNbr);
                    }

                    bool hasLines = false;
                    bool hasSSLines = false;
                    bool hasSiteBillLines = false;
                    bool hasRRLines = false;
                    /// Loop through all Conencted to Grid Sites
                    var detLines = PXSelect<SolarRevGenDetails,
                                      Where<SolarRevGenDetails.active, Equal<Required<SolarRevGenDetails.active>>,
                                      And<SolarRevGenDetails.solarRevGenID, Equal<Required<SolarRevGenDetails.solarRevGenID>>>>>.Select(this, true, CurrentDocument.Current.SolarRevGenID);


                    if (detLines == null) throw new PXException(Messages.NoLines);
                    foreach (SolarRevGenDetails rec in detLines)
                    {
                        SolarRevGenDetails line = (SolarRevGenDetails)rec;

                        var sSite = (SolarSite)PXSelect<SolarSite, Where<SolarSite.solarSiteID, Equal<Required<SolarRevGenDetails.solarSiteID>>>>.Select(this, line.SolarSiteID);

                        if (sSite?.ConnectedtoGridDate > this.CurrentDocument.Current.InvoiceDate || sSite == null) continue;
                        if (line.Active == true && line.Error == false && line.Processed == true)
                        {
                            ///Assign Sub Account
                            var sub = GSProjectHelper.GetSubaccount(ssGraph, sSite);
                            if (sub == null)
                            {
                                sub = CreateSubaccount(this, sSite);
                            }
                            String ssItem = String.Concat(GSynchExt.ItemPrefix.SSItem, sSite?.SolarSiteCD);
                            InventoryItem ssRec = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(this, ssItem);

                            ///1. Create Solar Sales Invoice lines; Amount = Total Solar Sales - Site Bill - Stamp Duty. 
                            if (ssInv?.InvoiceNbr != null)
                            {
                                hasLines = true;
                                var ssLines = ssGraph.Transactions.Insert(new ARTran
                                {
                                    RefNbr = ssInv.RefNbr,
                                    InventoryID = ssRec.InventoryID,
                                    TranDesc = String.Concat(ssRec.Descr, ":Solar Sales Net Amount (minus Site Bill & Stamp Duty)"),
                                    UOM = null,
                                    UnitPrice = null,
                                    BranchID = ssInv.BranchID,
                                    Qty = null, //line.ActQty ?? line.EstQty ?? line.InverterQty,
                                    ExtPrice = line.SolarSalesAmount,
                                    ManualPrice = true,
                                });
                                //Update Ext. Price again
                                ssLines.ExtPrice = line.SolarSalesAmount;
                                ssLines.ManualPrice = true;
                                ssLines.CuryExtPrice = line.SolarSalesAmount;
                                if(ssGraph.Transactions.Update(ssLines) != null) hasSSLines = true;
                                ///Insert one line only - Keep the below code commented. Do not Delete
                                /*
                                if (line.StampDutyAmount > 0)
                                {
                                    var sdLine = ssGraph.Transactions.Insert(new ARTran
                                    {
                                        RefNbr = ssInv.RefNbr,
                                        InventoryID = ssRec.InventoryID,
                                        UOM = "EA",
                                        BranchID = ssInv.BranchID
                                    });
                                    //sdLine.Qty = 1;
                                    sdLine.TranDesc = sSite.SolarSiteCD + ": Stamp Duty Amount";
                                    sdLine.CuryExtPrice = -line.StampDutyAmount;
                                    ssGraph.Transactions.Update(sdLine);
                                }

                                if (line.SiteBillAmount > 0)
                                {
                                    var sbLine = ssGraph.Transactions.Insert(new ARTran
                                    {
                                        RefNbr = ssInv.RefNbr,
                                        InventoryID = ssRec.InventoryID,
                                        UOM = "EA",
                                        BranchID = ssInv.BranchID
                                    });
                                    sbLine.CuryExtPrice = -line.SiteBillAmount;
                                    //sbLine.Qty = 1;
                                    sbLine.TranDesc = sSite.SolarSiteCD + ": Normal Bill Amount";
                                    ssGraph.Transactions.Update(sbLine);
                                }*/
                            }
                            ///2. Create Invoice to collect Solar Site Bill amount from Provincial Council
                            if (line.SiteBillAmount != 0 && sbInv?.InvoiceNbr != null)
                            {
                                hasLines = true;
                                var sbLines = sbGraph.Transactions.Insert(new ARTran
                                {
                                    // ARTran sbLines = sbGraph.Transactions.Insert();
                                    RefNbr = sbInv.RefNbr,
                                    UOM = "EA",
                                    CuryUnitPrice = line.SiteBillAmount,
                                    Qty = 1,
                                    BranchID = sbInv.BranchID,
                                    TranDesc = String.Concat(sSite.SolarSiteCD, ": CEB Site Bill"),
                                    ExpenseAccrualSubID = sub?.SubID,
                                    ExpenseSubID = sub?.SubID,
                                    SubID = sub?.SubID
                                });

                                if (sbGraph.Transactions.Update(sbLines) != null) hasSiteBillLines = true;
                            }
                            ///3. Create AP Invoice Provincial Council for Roof Rental
                            if (line.RoofRentAmount != 0 && rrAPInv?.InvoiceNbr != null)
                            {
                                hasLines = true;
                                var rrLines = rrGraph.Transactions.Insert(new APTran
                                {
                                    // APTran rrLines = rrGraph.Transactions.Insert();
                                    RefNbr = sbInv.RefNbr,
                                    UOM = "EA",
                                    CuryLineAmt = line.RoofRentAmount,
                                    BranchID = sbInv.BranchID,
                                    AccountID = ssRec.COGSAcctID,
                                    SubID = sub?.SubID
                                });
                                rrLines.TranDesc = "Roof Rental " + sSite.SolarSiteCD;
                                if (rrGraph.Transactions.Update(rrLines) != null) hasRRLines = true;
                            }
                        }
                    }

                    if (hasLines)
                    {
                        var ssRev = CurrentDocument.Current;
                        if (hasSSLines)
                        {
                            ssGraph.Actions.PressSave();
                            ssRev.Ssrefnbr = ssGraph.CurrentDocument.Current.RefNbr;
                        }
                        if (hasSiteBillLines)
                        {
                            sbGraph.Actions.PressSave();
                            ssRev.SiteBillRefNbr = sbGraph.CurrentDocument.Current.RefNbr;
                        }
                        if (hasRRLines)
                        {
                            rrGraph.Actions.PressSave();
                            ssRev.Rrrefnbr = rrGraph.CurrentDocument.Current.RefNbr;
                        }
                        CurrentDocument.Update(ssRev);
                        this.Actions.PressSave();
                    }
                }
                catch (Exception e)
                {
                    PXTrace.WriteError(e.StackTrace);
                    throw new PXException(e.Message);
                }
            });
            return adapter.Get();
        }

        public PXAction<SolarRevGen> Hold;
        [PXButton(), PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable hold(PXAdapter adapter)
        {
            /// Clears the Inv. Reference Nbrs if any. Reset them at time of release
            this.CurrentDocument.Current.SiteBillRefNbr = null;
            this.CurrentDocument.Current.Rrrefnbr = null;
            this.CurrentDocument.Current.Ssrefnbr = null;
            ARInvoice ssInvRec = PXSelect<ARInvoice, Where<ARInvoice.invoiceNbr, Equal<Required<ARInvoice.invoiceNbr>>>>.Select(this, String.Concat("SS", this.CurrentDocument.Current.SolarRevGenID));
            APInvoice rrInvRec = PXSelect<APInvoice, Where<APInvoice.invoiceNbr, Equal<Required<APInvoice.invoiceNbr>>>>.Select(this, String.Concat("RR", this.CurrentDocument.Current.SolarRevGenID));
            ARInvoice sBillRec = PXSelect<ARInvoice, Where<ARInvoice.invoiceNbr, Equal<Required<ARInvoice.invoiceNbr>>>>.Select(this, String.Concat("SB", this.CurrentDocument.Current.SolarRevGenID));
            this.CurrentDocument.Current.SiteBillRefNbr = sBillRec?.RefNbr;
            this.CurrentDocument.Current.Rrrefnbr = rrInvRec?.RefNbr;
            this.CurrentDocument.Current.Ssrefnbr = ssInvRec?.RefNbr;
            PrepareData.SetVisible(true);
            return adapter.Get();
        }
        public PXAction<SolarRevGen> Release;
        [PXButton(), PXUIField(DisplayName = "Release",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable release(PXAdapter adapter)
        {
            ARInvoice ssInvRec = PXSelect<ARInvoice, Where<ARInvoice.invoiceNbr, Equal<Required<ARInvoice.invoiceNbr>>>>.Select(this, String.Concat("SS",this.CurrentDocument.Current.SolarRevGenID));
            APInvoice rrInvRec = PXSelect<APInvoice, Where<APInvoice.invoiceNbr, Equal<Required<APInvoice.invoiceNbr>>>>.Select(this, String.Concat("RR",this.CurrentDocument.Current.SolarRevGenID));
            ARInvoice sBillRec = PXSelect<ARInvoice, Where<ARInvoice.invoiceNbr, Equal<Required<ARInvoice.invoiceNbr>>>>.Select(this, String.Concat("SB", this.CurrentDocument.Current.SolarRevGenID));
            this.CurrentDocument.Current.SiteBillRefNbr = sBillRec?.RefNbr;
            this.CurrentDocument.Current.Rrrefnbr = rrInvRec?.RefNbr;
            this.CurrentDocument.Current.Ssrefnbr = ssInvRec?.RefNbr;
            if(this.CurrentDocument.Current.SiteBillRefNbr == null || this.CurrentDocument.Current.Rrrefnbr == null || this.CurrentDocument.Current.Ssrefnbr == null)
            {
                CreateInvoice.SetVisible(true);
            }
            PrepareData.SetVisible(false);
            return adapter.Get();
        }
        #endregion
        #region Methods
        protected SolarRevGenDetails ValidateRecord(SolarRevGenDetails row, InventoryItem ssRec, RRRates rrRateRec, CurrencyRate cRec)
        {
            ///Cehck NON-STOCK IDs for Solar Sales
            if (ssRec == null)
            {
                row.Remark = AddError(row, Messages.MissingSSItem);
            }
            else
            {
                InventoryItemCurySettings priceRec = InventoryItemCurySettings.PK.Find(this, ssRec?.InventoryID, this.Accessinfo.BaseCuryID);
                ///Get Defaults & Formulated Data
                row.UoM = ssRec.SalesUnit;
                row.Tariff = priceRec.BasePrice;
                row.StampDutyAmount = 0;
                if (row.UoM == null) row.Remark = AddError(row, Messages.MissingUoM);
                if (row.Tariff == null || row.Tariff == decimal.Zero) row.Remark = AddError(row, Messages.MissingTariff);
            }
            if (rrRateRec == null) row.Remark = AddError(row, Messages.MissingRRRate);

            if (cRec == null) row.Remark = AddError(row, Messages.MissingRate);

            SiteDetails.Update(row);
            return row;
        }
        protected string AddError(SolarRevGenDetails row, string error)
        {
            if (row == null) return null;
            if (String.IsNullOrWhiteSpace(row.Remark))
                row.Remark = error;
            else
                row.Remark = String.Concat(row.Remark, ",", error);
            if (row.Remark.Length > 250) return row.Remark.Substring(1, 249);
            return row.Remark;
        }
        protected decimal CalculateMngFeeAmount(SolarRevGenDetails row)
        {
            Customer custRec = Customer.PK.Find(this, this.CurrentDocument.Current.CustomerID);
            CurrencyRate curRec = (CurrencyRate)PXSelect<CurrencyRate,
                        Where<CurrencyRate.fromCuryID, Equal<Required<CurrencyRate.fromCuryID>>,
                        And<CurrencyRate.toCuryID, Equal<Required<CurrencyRate.toCuryID>>,
                        And<CurrencyRate.curyRateType, Equal<Required<CurrencyRate.curyRateType>>,
                        And<CurrencyRate.curyEffDate, LessEqual<Required<CurrencyRate.curyEffDate>>>>>>,
                        OrderBy<Desc<CurrencyRate.curyEffDate>>>.Select(this, "USD", this.Accessinfo.BaseCuryID, custRec?.CuryRateTypeID, this.CurrentDocument.Current.InvoiceDate).FirstOrDefault();

            var mngAmount = (((row.ActQty ?? row.EstQty) * row.Tariff) / 24) * this.AutoNumSetup.Current?.MfRate * curRec.CuryRate;

            return mngAmount ?? 0;
        }

        protected decimal CalculateStampDuty(SolarRevGenDetails row)
        {
            var totalSolarSales = (row.ActQty ?? row.EstQty) * row.Tariff;
            var totalKW = totalSolarSales / 24;


            if (totalKW >= AutoNumSetup?.Current?.StampDutyLimit)
                row.StampDutyAmount = AutoNumSetup?.Current?.StampDutyAmount;

            return row.StampDutyAmount ?? 0;
        }

        #endregion
    }
}