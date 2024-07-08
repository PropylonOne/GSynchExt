using System;
using System.Linq;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.PM;
using PX.TM;
using static PX.Objects.RQ.RQRequisitionContent.FK;

namespace GSynchExt
{
    /// <summary>
    /// Customer Order Nbr is used to store the Solar Sales Revenue Gen ID (with the prefix based on the INV: SS, RR, SB)
    /// </summary>
    public class ARInvoiceEntryGSExt : PXGraphExtension<ARInvoiceEntry>
    {
        #region DAC Overrides
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(Enabled = false)]
        protected void ARInvoice_InvoiceNbr_CacheAttached(PXCache sender) { }

        #endregion

        protected virtual void _(Events.RowSelected<ARInvoice> e)
        {
            ARInvoice row = e.Row;
            if (row == null) return;

            bool notFromSRevGen = row.CreatedByScreenID != "GS501021";
            PXUIFieldAttribute.SetEnabled<ARInvoice.invoiceNbr>(e.Cache, row, notFromSRevGen);


        }
        protected virtual void _(Events.RowDeleting<ARInvoice> e)
        {
            ARInvoice row = e.Row as ARInvoice;
            if (row == null || row.CreatedByScreenID != "GS501021") return;
            string refNbr = row.InvoiceNbr;
            string result = refNbr.Substring(2, refNbr.Length-2);

            SolarRevGen revGen = PXSelect<SolarRevGen, Where<SolarRevGen.solarRevGenID, Equal<Required<SolarRevGen.solarRevGenID>>>>.Select(this.Base, result);

            if (revGen.SiteBillRefNbr == row.RefNbr)
            {

                var sbGraph = PXGraph.CreateInstance<SolarRevGenEntry>(); //Site Bill
                var solarRevGen = SolarRevGen.UK.Find(sbGraph, result);
                var message = string.Format("This document is referenced in the following record: SolarRevGen ({0}). Do you want to proceed?", solarRevGen.SolarRevGenID);

                if (this.Base.Document.Ask(message, MessageButtons.OKCancel) != WebDialogResult.OK) return;
                solarRevGen.SiteBillRefNbr = null;

                sbGraph.Document.Update(solarRevGen);
                sbGraph.Save.Press();
            }
            if (revGen.Ssrefnbr == row.RefNbr)
            {

                var sSGraph = PXGraph.CreateInstance<SolarRevGenEntry>(); //Solar Sales
                var solarRevGen = SolarRevGen.UK.Find(sSGraph, result);
                var message = string.Format("This document is referenced in the following record: SolarRevGen ({0}). Do you want to proceed?", solarRevGen.SolarRevGenID);

                if (this.Base.Document.Ask(message, MessageButtons.OKCancel) != WebDialogResult.OK) return;
                solarRevGen.Ssrefnbr = null;

                sSGraph.Document.Update(solarRevGen);
                sSGraph.Save.Press();

            }
       



    }

    }
}
