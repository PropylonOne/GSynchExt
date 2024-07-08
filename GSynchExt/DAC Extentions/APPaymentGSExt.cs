using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Common;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System;
using PX.Objects.GL;
using PX.Data.BQL.Fluent;
using PX.Objects.CA;

namespace GSynchExt
{
  public class APPaymentGSExt : PXCacheExtension<APPayment>
  {
        #region UsrIsPettyCash
        [PXDBBool]
        [PXUIField(DisplayName = "IsPettyCash")]

        public virtual bool? UsrIsPettyCash { get; set; }
        public abstract class usrIsPettyCash : PX.Data.BQL.BqlBool.Field<usrIsPettyCash> { }
        #endregion
    }
}