using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.RQ;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using GSynchExt;

namespace PX.Objects.RQ
{
  public class RQRequestLineGSExt : PXCacheExtension<PX.Objects.RQ.RQRequestLine>
  {
    #region UsrBOQID
    [PXDBInt]
    [PXUIField(DisplayName="BOQ-Project")]

    public virtual int? UsrBOQID { get; set; }
    public abstract class usrBOQID : PX.Data.BQL.BqlInt.Field<usrBOQID> { }
        #endregion

    #region UsrRevisionID
    [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
    [PXUIField(DisplayName = "BoQ Revision")]
    public virtual string UsrRevisionID { get; set; }
    public abstract class usrRevisionID : PX.Data.BQL.BqlString.Field<usrRevisionID> { }
    #endregion
  }
}