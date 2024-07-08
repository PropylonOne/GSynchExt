using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN.Attributes;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.IN
{
  public class INTranGSExt : PXCacheExtension<PX.Objects.IN.INTran>
  {
    #region UsrcreatedByMTR
    [PXDBBool]
    [PXUIField(DisplayName="createdByMTR")]

    public virtual bool? UsrcreatedByMTR { get; set; }
    public abstract class usrcreatedByMTR : PX.Data.BQL.BqlBool.Field<usrcreatedByMTR> { }
    #endregion

    #region UsrMTRRef
    [PXDBString(30)]
    [PXUIField(DisplayName="MTRRef")]

    public virtual string UsrMTRRef { get; set; }
    public abstract class usrMTRRef : PX.Data.BQL.BqlString.Field<usrMTRRef> { }
    #endregion

        #region UsrCreatedBySMR
        [PXDBBool]
        [PXUIField(DisplayName = "Created By SMR")]

        public virtual bool? UsrCreatedBySMR { get; set; }
        public abstract class usrCreatedBySMR : PX.Data.BQL.BqlBool.Field<usrCreatedBySMR> { }
        #endregion

        #region UsrSMRRef
        [PXDBString(30)]
        [PXUIField(DisplayName = "Service Material Request Reference")]

        public virtual string UsrSMRRef { get; set; }
        public abstract class usrSMRRef : PX.Data.BQL.BqlString.Field<usrSMRRef> { }
        #endregion


    }
}