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
    public class ContractGSExt : PXCacheExtension<PX.Objects.CT.Contract>
    {
        #region UsrEPCVendorID
        // [PXSelector(typeof(Search<PX.Objects.EP.EPEmployee.acctCD>), SubstituteKey = typeof(PX.Objects.EP.EPEmployee.acctName))]
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "EPC Vendor")]
        public virtual Int32? UsrEPCVendorID { get; set; }
        public abstract class usrEPCVendorID : PX.Data.BQL.BqlString.Field<usrEPCVendorID> { }

        #endregion

        #region UsrSubContractor
        // [PXSelector(typeof(Search<PX.Objects.EP.EPEmployee.acctCD>), SubstituteKey = typeof(PX.Objects.EP.EPEmployee.acctName))]
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "SubContractor")]
        public virtual Int32? UsrSubContractor { get; set; }
        public abstract class usrSubContractor : PX.Data.BQL.BqlString.Field<usrSubContractor> { }

        #endregion

        #region UsrAreaEngineer
        public abstract class usrAreaEngineer : PX.Data.BQL.BqlInt.Field<usrAreaEngineer> { }
        protected int? _UsrAreaEngineer;
        [PX.TM.Owner( Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Area Engineer ", Visibility = PXUIVisibility.Visible)]

        public virtual int? UsrAreaEngineer
        { get; set; }
        #endregion

        #region UsrAreaEngApprover
        public abstract class usrAreaEngApprover : PX.Data.BQL.BqlInt.Field<usrAreaEngApprover> { }
        protected int? _UsrAreaEngApprover;
        [PXDBInt]
        [PXEPEmployeeSelector]
        [PXForeignReference(typeof(Field<usrAreaEngApprover>.IsRelatedTo<BAccount.bAccountID>))]
        [PXUIField(DisplayName = "Area Engineer Approver ", Visibility = PXUIVisibility.Visible)]

        public virtual int? UsrAreaEngApprover
        { get; set; }
        #endregion

        /*

        #region UsrActACCAp
        public abstract class UsrActACCAp : PX.Data.BQL.BqlDecimal.Field<UsrActACCAp> { }
        [PXDBQuantity]
        [PXUIField(DisplayName = "Actual AC Capacity (KW)", Visibility = PXUIVisibility.Visible)]
        public virtual decimal? usrActACCAp
        {
            get;
            set;
        }
        #endregion

        #region UsrActDCCAp
        public abstract class UsrActDCCAp : PX.Data.BQL.BqlDecimal.Field<UsrActDCCAp> { }
        [PXDBQuantity]
        [PXUIField(DisplayName = "Actual DC Capacity (KW)", Visibility = PXUIVisibility.Visible)]
        public virtual decimal? usrActDCCAp
        {
            get;
            set;
        }
        #endregion
        */
    }
}