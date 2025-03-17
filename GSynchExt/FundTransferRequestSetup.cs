using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.IN.ItemClassTree.INItemClass;

namespace GSynchExt
{
	[Serializable]
	[PXCacheName("Transfer Request Preference")]
	public class FundTransferRequestSetup : PXBqlTable, IBqlTable
    {
        #region FTRequestNumberingID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Fund Transfer Request Numbering Sequence")]
        [PXDefault("FTREQUEST")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string FTRequestNumberingID { get; set; }
        public abstract class fTRequestNumberingID : PX.Data.BQL.BqlString.Field<fTRequestNumberingID> { }
        #endregion

        #region MTRequestNumberingID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Material Transfer Request Numbering Sequence")]
        [PXDefault("MTREQUEST")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string MTRequestNumberingID { get; set; }
        public abstract class mTRequestNumberingID : PX.Data.BQL.BqlString.Field<mTRequestNumberingID> { }
        #endregion

        #region FARequestNumberingID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "FA Request Numbering Sequence")]
        [PXDefault("FAREQUEST")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string FARequestNumberingID { get; set; }
        public abstract class fARequestNumberingID : PX.Data.BQL.BqlString.Field<fARequestNumberingID> { }
        #endregion


        #region SMRequestNumberingID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Service Material Request Numbering Sequence")]
        [PXDefault("SMREQUEST")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string SMRequestNumberingID { get; set; }
        public abstract class sMRequestNumberingID : PX.Data.BQL.BqlString.Field<sMRequestNumberingID> { }
        #endregion

        #region FTRequestapprovalMap
        public abstract class approvalMap : BqlBool.Field<approvalMap>
        {
        }
        [EPRequireApproval]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "Require Approval For Fund Transfer Request")]
        public virtual bool? ApprovalMap { get; set; }
        #endregion


        #region FARequestapprovalMap

        [EPRequireApproval]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "Require Approval For FA Request")]
        public virtual bool? FARequestapprovalMap { get; set; }
        public abstract class fARequestapprovalMap : BqlBool.Field<fARequestapprovalMap> { }
        #endregion
    }
}

