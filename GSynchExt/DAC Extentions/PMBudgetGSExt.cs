using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System.Collections;
using System;
using PX.Objects.AP;
using GSynchExt;
using PX.Objects.CR;

namespace PX.Objects.PM
{
  public class PMBudgetGSExt : PXCacheExtension<PX.Objects.PM.PMBudget>
  {
    #region UsrVendorID
    [PXInt]
    [PXSelector(typeof(Search<BAccount.bAccountID>), SubstituteKey =typeof(BAccount.acctName))]
    [PXUIField(DisplayName="Vendor ID")]
    public virtual int? UsrVendorID { get; set; }
    public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
    #endregion
  }
}