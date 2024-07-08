using System;
using PX.Data;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("SolarSiteInverterData")]
  public class SolarSiteInverterData : PXBqlTable, IBqlTable
  {

    #region SolarSiteID
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Solar Site ID")]
    public virtual int? SolarSiteID { get; set; }
    public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }
    #endregion

    #region GenDate
    [PXDBDate(IsKey = true)]
    [PXUIField(DisplayName = "Solar Gen. Date")]
    public virtual DateTime? GenDate { get; set; }
    public abstract class genDate : PX.Data.BQL.BqlDateTime.Field<genDate> { }
    #endregion

    #region PlantID
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Plant ID")]
    public virtual int? PlantID { get; set; }
    public abstract class plantID : PX.Data.BQL.BqlInt.Field<plantID> { }
    #endregion

    #region Uom
    [PXDBString(6, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "UoM")]
    public virtual string Uom { get; set; }
    public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
    #endregion

    #region TransDate
    [PXDBDate()]
    [PXUIField(DisplayName = "Trans Date")]
    public virtual DateTime? TransDate { get; set; }
    public abstract class transDate : PX.Data.BQL.BqlDateTime.Field<transDate> { }
    #endregion

    #region InverterQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Inverter Qty")]
    public virtual Decimal? InverterQty { get; set; }
    public abstract class inverterQty : PX.Data.BQL.BqlDecimal.Field<inverterQty> { }
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