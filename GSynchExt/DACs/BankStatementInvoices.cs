using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.IN;
using static PX.Data.PXAccess.Branch;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("BankStatementInvoices")]
  public class BankStatementInvoices : PXBqlTable, IBqlTable
  {
    #region InventoryID
    [PXDBInt()]
    [PXUIField(DisplayName = "Inventory ID")]
    [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey =(typeof(InventoryItem.inventoryCD)))]
    public virtual int? InventoryID { get; set; }
    public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
    #endregion

    #region RefNbr
    [PXDBString(15, IsUnicode = true, IsKey = true)]
    [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = true)]
    public virtual string RefNbr
    {
        get;
        set;
    }
    public abstract class refNbr : PX.Data.BQL.BqlInt.Field<refNbr> { }
    #endregion

    #region RefNbr

    [PXDBDate]
    [PXUIField(DisplayName = "Document Date", Visible = false)]
    public virtual DateTime? TranDate { get; set; }
    #endregion

    #region LineNbr
    public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible)]
      
    public virtual int? LineNbr
    {
        get;
        set;
    }
    #endregion

    #region CashAccountCD
    [PXDBString(10, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Cash Account")]
    public virtual string CashAccountCD { get; set; }
    public abstract class cashAccountCD : PX.Data.BQL.BqlString.Field<cashAccountCD> { }
    #endregion

    #region Province
    [PXDBString(15, IsUnicode = true)]
    [PXUIField(DisplayName = "Province", Visibility = PXUIVisibility.Visible, Visible = true)]
    public virtual string Province
    {
        get;
        set;
    }
    public abstract class province : PX.Data.BQL.BqlInt.Field<province> { }

    #endregion

    #region TranAmt
    public abstract class tranAmt : PX.Data.BQL.BqlDecimal.Field<tranAmt> { }
    [PXDBBaseCury(typeof(branchID))]
    [PXDefault(TypeCode.Decimal, "0.0")]
    [PXUIField(DisplayName = " Invoice Amount", Enabled = false)]

    public virtual Decimal? TranAmt
    {
        get;
        set;
    }
    #endregion

    #region TranBal
    public abstract class tranBal : PX.Data.BQL.BqlDecimal.Field<tranBal> { }

    [PXDBBaseCury]
    [PXDefault(TypeCode.Decimal, "0.0")]
    [PXUIField(DisplayName = "Unmatched Amount", Enabled = false)]
    public virtual decimal? TranBal
    {
        get;
        set;
    }
    #endregion

    #region TransactionID
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Transaction ID")]
    public virtual int? TransactionID { get; set; }
    public abstract class transactionID : PX.Data.BQL.BqlInt.Field<transactionID> { }
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