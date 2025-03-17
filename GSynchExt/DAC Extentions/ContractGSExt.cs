using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Discount;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.PO;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Collections;
using System;
using PX.Objects.AP;
using GSynchExt;

namespace PX.Objects.CT
{
    public sealed class ContractGSExt : PXCacheExtension<PX.Objects.CT.Contract>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        #region UsrEPCVendorID
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "EPC Vendor")]
        public Int32? UsrEPCVendorID { get; set; }
        public abstract class usrEPCVendorID : PX.Data.BQL.BqlString.Field<usrEPCVendorID> { }

        #endregion

        #region UsrSubContractor
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "SubContractor")]
        public Int32? UsrSubContractor { get; set; }
        public abstract class usrSubContractor : PX.Data.BQL.BqlString.Field<usrSubContractor> { }

        #endregion

        #region UsrAreaEngineer
        public abstract class usrAreaEngineer : PX.Data.BQL.BqlInt.Field<usrAreaEngineer> { }
        protected int? _UsrAreaEngineer;
        [PX.TM.Owner( Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Area Engineer ", Visibility = PXUIVisibility.Visible)]

        public int? UsrAreaEngineer { get; set; }
        #endregion

        #region UsrAreaEngApprover
        public abstract class usrAreaEngApprover : PX.Data.BQL.BqlInt.Field<usrAreaEngApprover> { }
        protected int? _UsrAreaEngApprover;
        [PXDBInt]
        [PXEPEmployeeSelector]
        [PXForeignReference(typeof(Field<usrAreaEngApprover>.IsRelatedTo<BAccount.bAccountID>))]
        [PXUIField(DisplayName = "Area Engineer Approver ", Visibility = PXUIVisibility.Visible)]

        public  int? UsrAreaEngApprover { get; set; }
        #endregion

    }
}