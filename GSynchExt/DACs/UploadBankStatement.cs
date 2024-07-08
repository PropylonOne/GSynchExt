using System;
using PX.Data;
using PX.Data.BQL;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("UploadBankStatement")]
    public class UploadBankStatement : PXBqlTable, IBqlTable
    {
        #region TransactionID
        [PXDBIdentity(IsKey = true)]
        public virtual int? TransactionID { get; set; }
        public abstract class transactionID : PX.Data.BQL.BqlInt.Field<transactionID> { }
        #endregion

        #region SolarSiteID
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Solar Site ID")]
        public virtual string SolarSiteID { get; set; }
        public abstract class solarSiteID : PX.Data.BQL.BqlString.Field<solarSiteID> { }
        #endregion

        #region Matched

        [PXDBBool]
        [PXUIField(DisplayName = "Matched", Visibility = PXUIVisibility.Visible)]
        public virtual bool? Matched { get; set; }
        public abstract class matched : BqlBool.Field<matched> { }
        #endregion

        #region CEBAccount
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CEB Account")]
        public virtual string CEBAccount { get; set; }
        public abstract class cEBAccount : PX.Data.BQL.BqlString.Field<cEBAccount> { }
        #endregion

        #region Province
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Province")]
        public virtual string Province { get; set; }
        public abstract class province : PX.Data.BQL.BqlString.Field<province> { }
        #endregion

        #region CashAccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cash Account")]
        public virtual string CashAccountCD { get; set; }
        public abstract class cashAccountCD : PX.Data.BQL.BqlString.Field<cashAccountCD> { }
        #endregion

        #region District
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "District")]
        public virtual string District { get; set; }
        public abstract class district : PX.Data.BQL.BqlString.Field<district> { }
        #endregion

        #region PhaseID
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Phase ID")]
        public virtual string PhaseID { get; set; }
        public abstract class phaseID : PX.Data.BQL.BqlString.Field<phaseID> { }
        #endregion

        #region ClusterID
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cluster ID")]
        public virtual string ClusterID { get; set; }
        public abstract class clusterID : PX.Data.BQL.BqlString.Field<clusterID> { }
        #endregion

        #region TransactionDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Statement Date")]
        public virtual DateTime? TransactionDate { get; set; }
        public abstract class transactionDate : PX.Data.BQL.BqlDateTime.Field<transactionDate> { }
        #endregion

        #region CEBAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "CEB Amount")]
        public virtual Decimal? CEBAmount { get; set; }
        public abstract class cEBAmount : PX.Data.BQL.BqlDecimal.Field<cEBAmount> { }
        #endregion

        #region Active
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion

        #region Processed
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Processed")]
        public virtual bool? Processed { get; set; }
        public abstract class processed : PX.Data.BQL.BqlBool.Field<processed> { }
        #endregion

        #region RefNbr
        [PXDBString(10)]
        [PXUIField(DisplayName = "Ref Nbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Invoice Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region OpenAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Open Amount")]
        public virtual Decimal? OpenAmount { get; set; }
        public abstract class openAmount : PX.Data.BQL.BqlDecimal.Field<openAmount> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
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

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }

    public class nonInventory : PX.Data.BQL.BqlString.Constant<nonInventory> { public nonInventory() : base("SS-GGU") { } }

}