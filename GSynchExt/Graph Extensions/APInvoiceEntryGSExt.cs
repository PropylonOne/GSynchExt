using System;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Common;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Objects.CS;


namespace GSynchExt
{
    public class APInvoiceEntryGSExt : PXGraphExtension<APInvoiceEntry>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.inventory>(); }
        #endregion


        #region Constants
        private string screenID = PXContext.GetScreenID();
        private bool isNotPettyCash;
        #endregion

        #region Dialogs
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial class InvoDialogInfo : PXBqlTable, IBqlTable
        {
            #region VendorID
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            [Vendor(typeof(Search<Vendor.bAccountID>), DisplayName = "Vendor", DescriptionField = typeof(Vendor.acctName))]
            public virtual int? VendorID { get; set; }
            #endregion

            #region DefaultExpenseSubID
            public abstract class defaultExpenseSubID : PX.Data.BQL.BqlInt.Field<defaultExpenseSubID> { }
            [SubAccount(DisplayName = "Default Expense Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
            [PXDefault(typeof(PMProject.defaultExpenseSubID))]
            public virtual Int32? DefaultExpenseSubID
            {
                get;
                set;
            }
            #endregion

            #region DefaultExpenseAccountID
            public abstract class defaultExpenseAccountID : PX.Data.BQL.BqlInt.Field<defaultExpenseAccountID> { }
            [Account(DisplayName = "Default Cost Account")]
            [PXDefault(typeof(PMProject.defaultExpenseAccountID))]
            public virtual Int32? DefaultExpenseAccountID
            {
                get;
                set;
            }
            #endregion
        }
        #endregion

        public virtual APInvoice CreateAPBill(PMCostBudget budget, InvoDialogInfo info)
        {
           
            APInvoice aPInvoice = new APInvoice();
            if (budget == null) return aPInvoice;
            aPInvoice.VendorID = info.VendorID;
            aPInvoice = this.Base.Document.Insert(aPInvoice);

            APTran tran = new APTran();
            if(budget.InventoryID != 1)
            {
                tran.InventoryID = budget.InventoryID;
            }
            if (budget.InventoryID == 1)
            {
                tran.TranDesc = budget.Description;
            }
            tran.Qty = budget.RevisedQty - budget.ActualQty;
            tran.UOM = budget.UOM;
            tran.ProjectID = budget.ProjectID;
            tran.TaskID = budget.TaskID;
            tran.CostCodeID = budget.CostCodeID;
            tran.SubID = info.DefaultExpenseSubID;
            tran.AccountID = info.DefaultExpenseAccountID;
            this.Base.Transactions.Insert(tran);

            return aPInvoice;
        }
        public virtual APTran CreateAPBillFromCostBudget(PMCostBudget budget, InvoDialogInfo info, bool redirect = false)
        {
            CreateAPBill(budget, info);
            if (this.Base.Transactions.Cache.IsDirty)
            {
                if (redirect)
                    throw new PXRedirectRequiredException(this.Base, "");
                else
                    return this.Base.Transactions.Current;
            }
            throw new PXException("");
        }
        public override void Initialize()
        {
            base.Initialize();
            isNotPettyCash = screenID != "GS.30.20.00";
            var scID = PXContext.GetScreenID();

        }
        protected virtual void _(Events.FieldDefaulting<APPayment.docType> e)
        {
            if (e.Row == null) return;
            isNotPettyCash = screenID != "GS.30.20.00";
            if (isNotPettyCash == true) return;
            var userID = base.Base.Accessinfo.UserID;
            var userName = base.Base.Accessinfo.UserName;
            //APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();

            APPayment payment = (APPayment)e.Row;
            APPaymentGSExt extension = PXCache<APPayment>.GetExtension<APPaymentGSExt>(payment);

            EPEmployee vendor = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>.
                                Select(base.Base, userID);
            e.Cache.SetValueExt<APPayment.vendorID>(e.Row, vendor?.BAccountID);
            e.Cache.SetValueExt<APPayment.docType>(e.Row, APDocType.Prepayment);
            e.Cache.SetValueExt<APPayment.docDesc>(e.Row, "Petty Cash Request");
            e.Cache.SetValueExt<APPayment.paymentMethodID>(e.Row, "CASH");
            e.Cache.SetValueExt<APPaymentGSExt.usrIsPettyCash>(e.Row, true);

        }
        protected virtual void _(Events.RowSelected<APInvoice> e)
        {
            APInvoice row = e.Row;
            if (row == null) return;

            bool notFromSRevGen = row.CreatedByScreenID != "GS501021";
            PXUIFieldAttribute.SetEnabled<APInvoice.invoiceNbr>(e.Cache, row, notFromSRevGen);


        }

        protected virtual void _(Events.RowDeleting<APInvoice> e)
        {
            APInvoice row = e.Row as APInvoice;
            if (row == null || row.CreatedByScreenID != "GS501021") return;
            string refNbr = row.InvoiceNbr;
            string result = refNbr.Substring(2, refNbr.Length-2);

            SolarRevGen revGen = PXSelect<SolarRevGen, Where<SolarRevGen.solarRevGenID, Equal<Required<SolarRevGen.solarRevGenID>>>>.Select(this.Base, result);

            if (revGen.Rrrefnbr == row.RefNbr)
            {
                    
                var rrGraph = PXGraph.CreateInstance<SolarRevGenEntry>(); //Roof Rental
                var solarRevGen = SolarRevGen.UK.Find(rrGraph, result);
                var message = string.Format("This document is referenced in the following record: SolarRevGen ({0}). Do you want to proceed?", solarRevGen.SolarRevGenID);

                if (this.Base.Document.Ask(message, MessageButtons.OKCancel) != WebDialogResult.OK) return;

                solarRevGen.Rrrefnbr = null;

                rrGraph.Document.Update(solarRevGen);
                rrGraph.Save.Press();

                //throw new PXException("AP document(Bill, {0}) cannot be deleted because it is referenced in the following record: SolarRevGen({1}).", refNbr, result);
            }

        }
    }
}