using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using static PX.Objects.AP.APDocumentEnq;

namespace GSynchExt
{

    public partial class SolarSiteRevGenFilter : PXBqlTable, IBqlTable
    {
        #region EntryDate
        [PXDBDate(IsKey = true)]
        [PXUIField(DisplayName = "Entry Date")]
        public virtual DateTime? EntryDate { get; set; }
        public abstract class entryDate : PX.Data.BQL.BqlDateTime.Field<entryDate> { }
        #endregion


        #region PhaseID
        [PXString]
        [PXUIField(DisplayName = "Phase ID")]
        [PXStringList(
         new string[]
         {
             GSynchExt.Phases.Phase1 ,
             GSynchExt.Phases.Phase2 ,
             GSynchExt.Phases.Phase3 ,
             GSynchExt.Phases.Phase4 ,
             GSynchExt.Phases.Phase5 ,
             GSynchExt.Phases.Phase6 ,
             GSynchExt.Phases.Phase7 ,
             GSynchExt.Phases.Phase8 ,
             GSynchExt.Phases.Phase9 ,
             GSynchExt.Phases.Phase10 ,
         },
         new string[]
         {
             GSynchExt.MessagesBOQ.Phase1,
             GSynchExt.MessagesBOQ.Phase2,
             GSynchExt.MessagesBOQ.Phase3,
             GSynchExt.MessagesBOQ.Phase4,
             GSynchExt.MessagesBOQ.Phase5,
             GSynchExt.MessagesBOQ.Phase6,
             GSynchExt.MessagesBOQ.Phase7,
             GSynchExt.MessagesBOQ.Phase8,
             GSynchExt.MessagesBOQ.Phase9,
             GSynchExt.MessagesBOQ.Phase10,

         })]
        public virtual string PhaseID { get; set; }
        public abstract class phaseID : PX.Data.BQL.BqlString.Field<phaseID> { }
        #endregion


    }



  [Serializable]
  [PXCacheName("SolarSiteRevGen")]
  public class SolarSiteRevGen : PXBqlTable, IBqlTable
  {
    #region SolarSiteID
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Solar Site ID")]
    [PXSelector(typeof(Search<SolarSite.solarSiteID>), SubstituteKey = typeof(SolarSite.solarSiteCD))]
    public virtual int? SolarSiteID { get; set; }
    public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }
    #endregion

    #region EntryDate
        [PXDBDate(IsKey = true)]
        [PXUIField(DisplayName = "Entry Date")]
        public virtual DateTime? EntryDate { get; set; }
        public abstract class entryDate : PX.Data.BQL.BqlDateTime.Field<entryDate> { }
        #endregion

    #region InventoryID
    [PXDBInt()]
    [PXUIField(DisplayName = "Inventory ID")]
    [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey = typeof(InventoryItem.inventoryCD))]
    public virtual int? InventoryID { get; set; }
    public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
    #endregion

/*
    #region Uom
    [PXDBString(10, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "UOM")]
    public virtual string Uom { get; set; }
    public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion
*/

    #region UOM
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        [PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<SolarSiteRevGen.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(SolarSiteRevGen.inventoryID), DisplayName = "UOM")]
        public virtual String UOM
        {
            get;
            set;
        }
        #endregion

    #region PeriodID
        public abstract class periodID : PX.Data.BQL.BqlString.Field<periodID> { }

        [FinPeriodSelector(typeof(SolarSiteRevGen.entryDate))]
        [PXUIField(DisplayName = "Period", Visibility = PXUIVisibility.Visible, Required = false)]
        public virtual string PeriodID { get; set; }

        #endregion

    #region EndDate
        [PXDBDate()]
    [PXUIField(DisplayName = "End Date")]
    public virtual DateTime? EndDate { get; set; }
    public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
    #endregion

    #region UnitPrice
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        [PXDBPriceCost()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Unit Price", Enabled = false)]
        public virtual Decimal? UnitPrice    { get; set;}
        #endregion

    #region InvQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Inv Qty")]
    public virtual Decimal? InvQty { get; set; }
    public abstract class invQty : PX.Data.BQL.BqlDecimal.Field<invQty> { }
    #endregion

    #region EstQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Est Qty")]
    public virtual Decimal? EstQty { get; set; }
    public abstract class estQty : PX.Data.BQL.BqlDecimal.Field<estQty> { }
    #endregion

    #region ActualQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Actual Qty")]
    public virtual Decimal? ActualQty { get; set; }
    public abstract class actualQty : PX.Data.BQL.BqlDecimal.Field<actualQty> { }
    #endregion

    #region InvSolarAmount       
        [PXDecimal]
        [PXUIField(DisplayName = "Inv. Solar Price")]
        [PXFormula(typeof(
            Mult<unitPrice, invQty>))]
        public virtual Decimal? InvSolarAmount { get; set; }
        public abstract class invSolarAmount : PX.Data.BQL.BqlDecimal.Field<invSolarAmount> { }
        #endregion       

    #region EstSolarAmount
        [PXDecimal]
        [PXUIField(DisplayName = "Est. Solar Price")]
        [PXFormula(typeof(
            Mult<unitPrice, estQty>))]
        public virtual Decimal? EstSolarAmount { get; set; }
        public abstract class estSolarAmount : PX.Data.BQL.BqlDecimal.Field<estSolarAmount> { }
        #endregion

    #region ActSolarAmount
        [PXDecimal]
        [PXUIField(DisplayName = "Act. Solar Price")]
        [PXFormula(typeof(
            Mult<unitPrice, actualQty>))]
        public virtual Decimal? ActSolarAmount { get; set; }
        public abstract class actSolarAmount : PX.Data.BQL.BqlDecimal.Field<actSolarAmount> { }
        #endregion

    #region ActNormalAmount
        [PXDecimal]
        [PXUIField(DisplayName = "Act. Normal Bill")]
        public virtual Decimal? ActNormalAmount { get; set; }
        public abstract class actNormalAmount : PX.Data.BQL.BqlDecimal.Field<actNormalAmount> { }
        #endregion

    #region ActSetOffAmount
        [PXDecimal]
        [PXUIField(DisplayName = "Normal Bill Set-Off")]
        public virtual Decimal? ActSetOffAmount { get; set; }
        public abstract class actSetOffAmount : PX.Data.BQL.BqlDecimal.Field<actSetOffAmount> { }
        #endregion

    #region PhaseID
        [PXString]
        [PXUIField(DisplayName = "Phase ID")]
        [PXStringList(
         new string[]
         {
             GSynchExt.Phases.Phase1 ,
             GSynchExt.Phases.Phase2 ,
             GSynchExt.Phases.Phase3 ,
             GSynchExt.Phases.Phase4 ,
             GSynchExt.Phases.Phase5 ,
             GSynchExt.Phases.Phase6 ,
             GSynchExt.Phases.Phase7 ,
             GSynchExt.Phases.Phase8 ,
             GSynchExt.Phases.Phase9 ,
             GSynchExt.Phases.Phase10 ,
         },
         new string[]
         {
             GSynchExt.MessagesBOQ.Phase1,
             GSynchExt.MessagesBOQ.Phase2,
             GSynchExt.MessagesBOQ.Phase3,
             GSynchExt.MessagesBOQ.Phase4,
             GSynchExt.MessagesBOQ.Phase5,
             GSynchExt.MessagesBOQ.Phase6,
             GSynchExt.MessagesBOQ.Phase7,
             GSynchExt.MessagesBOQ.Phase8,
             GSynchExt.MessagesBOQ.Phase9,
             GSynchExt.MessagesBOQ.Phase10,

         })]
        public virtual string PhaseID { get; set; }
        public abstract class phaseID : PX.Data.BQL.BqlString.Field<phaseID> { }
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
}