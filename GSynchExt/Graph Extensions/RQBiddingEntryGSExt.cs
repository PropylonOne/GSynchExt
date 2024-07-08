using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.RQ
{
    public class RQBiddingEntryGSExt : PXGraphExtension<RQBiddingEntry>
    {
        protected virtual void _(Events.RowSelected<RQRequisitionLineBidding> e)
        {
            var row = e.Row as RQRequisitionLineBidding;

            if (row == null) return;
            if (row.OrderQty != null && (row.QuoteQty == 0 || row.QuoteQty == null))
            {
                e.Cache.SetValueExt<RQRequisitionLineBidding.quoteQty>(row, row.OrderQty);
            }
        }
    }
}
