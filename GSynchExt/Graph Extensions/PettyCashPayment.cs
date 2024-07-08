using System;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Common;
using static PX.Data.Events;
using PX.SM;

namespace GSynchExt
{
    public class PettyCashPayment : PXGraphExtension<APPaymentEntry>
    {

        #region Constants
        private string screenID = PXContext.GetScreenID();
        private bool isNotPettyCash;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            isNotPettyCash = screenID != "GS.30.20.00";

        }
        /*
        PettyCashPayment()
        {
            base.Initialize(); 

            APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();
            
           //FieldDefaulting.AddHandler<APPayment.docType>((sender, e) => { if (isNotPettyCash == false) e.NewValue = APDocType.Prepayment; });
          

        }*/

        protected virtual void _(Events.FieldDefaulting<APPayment.docType> e)
        {
            if (e.Row == null) return;
            if (isNotPettyCash == true) return;
            var userID = base.Base.Accessinfo.UserID;

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

        protected virtual void _(Events.RowSelected<APPayment> e)
        {
            if (e.Row == null) return;
            if (isNotPettyCash == true) return;
            var time = DateTime.Now;
            var desc = $"PR on : {time}";
            e.Cache.SetValueExt<APPayment.extRefNbr>(e.Row, desc);
        }
    }
}