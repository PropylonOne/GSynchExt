using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("SolarRevGenDetails")]
    public class SolarRevGenDetails : PXBqlTable, IBqlTable
    {
        public static class FK
        {
            public class MasterRec : SolarRevGen.UK.ForeignKeyOf<SolarRevGenDetails>.By<solarRevGenID> { }
        }

        #region SolarRevGenID
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Solar Rev. Gen. ID")]
        [PXDBDefault(typeof(SolarRevGen.solarRevGenID), DefaultForUpdate = false)]
        [PXParent(typeof(FK.MasterRec))]
        public virtual string SolarRevGenID { get; set; }
        public abstract class solarRevGenID : PX.Data.BQL.BqlString.Field<solarRevGenID> { }
        #endregion
        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(SolarRevGen.lnCntrl))]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
        #region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        //protected Int32? _BranchID;
        [Branch()]
        public virtual Int32? BranchID { get; set; }
        #endregion
        #region Active
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active", Enabled = true)]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion
        #region Processed
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Validated", Enabled = false)]
        public virtual bool? Processed { get; set; }
        public abstract class processed : PX.Data.BQL.BqlBool.Field<processed> { }
        #endregion
        #region Error
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Error", Enabled = false)]
        public virtual bool? Error { get; set; }
        public abstract class error : PX.Data.BQL.BqlBool.Field<error> { }
        #endregion
        #region Remark
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark", Enabled = false)]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion
        #region SolarSiteID
        [PXDBInt()]
        [PXUIField(DisplayName = "Solar Site ID")]
        [PXSelector(typeof(Search<SolarSite.solarSiteID>), SubstituteKey = (typeof(SolarSite.solarSiteCD)))]
        public virtual int? SolarSiteID { get; set; }
        public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }
        #endregion
        #region CEBAccount
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CEB Account", Enabled = false)]
        [PXUnboundDefault(typeof(Search<SolarSite.cEBAccount, Where<SolarSite.solarSiteID, Equal<Current<SolarRevGenDetails.solarSiteID>>>>))]
        public virtual string CEBAccount { get; set; }
        public abstract class cEBAccount : PX.Data.BQL.BqlString.Field<cEBAccount> { }
        #endregion
        #region UoM
        [INUnit(DisplayName = "UoM", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string UoM { get; set; }
        public abstract class uoM : PX.Data.BQL.BqlString.Field<uoM> { }
        #endregion
        #region InverterQty
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Inverter Qty")]
        public virtual Decimal? InverterQty { get; set; }
        public abstract class inverterQty : PX.Data.BQL.BqlDecimal.Field<inverterQty> { }
        #endregion


        #region SiteStatus
        [PXString(10, IsUnicode = true, InputMask = "")]
        [GSynchExt.Status.SSList]
        [PXUIField(DisplayName = "Site Status", Enabled = false)]
        public virtual string SiteStatus { get; set; }
        public abstract class siteStatus : PX.Data.BQL.BqlString.Field<siteStatus> { }
        #endregion


        #region EstQty
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Estimated Qty")]
        public virtual Decimal? EstQty { get; set; }
        public abstract class estQty : PX.Data.BQL.BqlDecimal.Field<estQty> { }
        #endregion

        #region ActQty
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Actual Qty")]
        public virtual Decimal? ActQty { get; set; }
        public abstract class actQty : PX.Data.BQL.BqlDecimal.Field<actQty> { }
        #endregion

        #region SiteBillAmount
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Site Bill Amount")]
        public virtual Decimal? SiteBillAmount { get; set; }
        public abstract class siteBillAmount : PX.Data.BQL.BqlDecimal.Field<siteBillAmount> { }
        #endregion

        #region StampDutyAmount
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Stamp Duty Amount", Enabled = false)]
        public virtual Decimal? StampDutyAmount { get; set; }
        public abstract class stampDutyAmount : PX.Data.BQL.BqlDecimal.Field<stampDutyAmount> { }
        #endregion

        #region RoofRentAmount
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Roof Rental Amount", Enabled = false)]
        public virtual Decimal? RoofRentAmount { get; set; }
        public abstract class roofRentAmount : PX.Data.BQL.BqlDecimal.Field<roofRentAmount> { }
        #endregion

        #region MngFeeAmount
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Mng Fee Amount", Enabled = false, Visible = false)]
        public virtual Decimal? MngFeeAmount { get; set; }
        public abstract class mngFeeAmount : PX.Data.BQL.BqlDecimal.Field<mngFeeAmount> { }
        #endregion

        #region SolarSalesAmount
        [PXDBBaseCury]
        [PXUIField(DisplayName = "Solar Sales Net Amount", Enabled = false)]
        public virtual Decimal? SolarSalesAmount { get; set; }
        public abstract class solarSalesAmount : PX.Data.BQL.BqlDecimal.Field<solarSalesAmount> { }
        #endregion

        #region Tariff
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tariff", Enabled = false)]
        public virtual Decimal? Tariff { get; set; }
        public abstract class tariff : PX.Data.BQL.BqlDecimal.Field<tariff> { }
        #endregion

        #region RoofRentPercnt
        [PXDBDecimal(MinValue = 0, MaxValue = 100, BqlField = typeof(SolarRevGenDetails.roofRentPercnt))]
        [PXUIField(DisplayName = "Roof Rental %", Enabled = false)]
        public virtual Decimal? RoofRentPercnt { get; set; }
        public abstract class roofRentPercnt : PX.Data.BQL.BqlDecimal.Field<roofRentPercnt> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
    }
}