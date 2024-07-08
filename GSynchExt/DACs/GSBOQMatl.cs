using System;
using GSynchExt;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("GSBOQMatl")]
    public class GSBOQMatl : PXBqlTable, IBqlTable
    {
        public static class FK
        {
            public class MasterRec : GSBOQ.UK.ForeignKeyOf<GSBOQMatl>.By<bOQID, revisionID> { }
            public class CostCode : PMCostCode.PK.ForeignKeyOf<GSBOQMatl>.By<costCode> { }
            public class InventoryID : InventoryItem.PK.ForeignKeyOf<GSBOQMatl>.By<inventoryID> { }
        }

        #region BOQID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "BOQ ID")]
        [PXDBDefault(typeof(GSBOQ.bOQID), DefaultForUpdate = false)]
        [PXParent(typeof(FK.MasterRec))]
        public virtual Int32? BOQID { get; set; }
        public abstract class bOQID : PX.Data.BQL.BqlInt.Field<bOQID> { }
        #endregion

        #region RevisionID
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        [PXDBString(10, IsKey = true)]
        [PXDBDefault(typeof(GSBOQ.revisionID))]
        [PXUIField(DisplayName = "Revision")]
        public virtual string RevisionID { get; set; }
        #endregion

        #region GroupID
        [PXDBString(15, IsUnicode = true)]
        [PXDefault]
        [PXSelector(typeof(Search<BOQGroup.groupID>))]
        [PXUIField(DisplayName = "Group ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PX.Data.EP.PXFieldDescription]
        public virtual String GroupID { get; set; }
        public abstract class groupID : PX.Data.BQL.BqlString.Field<groupID> { }
        #endregion
        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr")]
        [PXLineNbr(typeof(GSBOQ.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXDefault]
        [PXSelector(typeof(Search2<InventoryItem.inventoryID,
            InnerJoin<BOQGroupItems,
                On<InventoryItem.inventoryID, Equal<BOQGroupItems.inventoryID>,
                    And<InventoryItem.itemStatus, Equal<InventoryItemStatus.active>,
                    And<BOQGroupItems.groupID, Equal<Current<groupID>>>>
                        >>>), SubstituteKey = (typeof(InventoryItem.inventoryCD)), DescriptionField = typeof(InventoryItem.descr))]
        [PXForeignReference(typeof(FK.InventoryID))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region CostCode
        [PXUIField(DisplayName = "Cost Code", Enabled = false)]
        /*[PXSelector(typeof(Search2<BOQGroup.costCodeID,
            InnerJoin<PMCostCode,
                On<PMCostCode.costCodeID, Equal<BOQGroup.costCodeID>>>,
                    Where<BOQGroup.groupID, Equal<Current<GSBOQMatl.groupID>>>>), 
            SubstituteKey = (typeof(PMCostCode.costCodeCD)), DescriptionField = typeof(PMCostCode.description))]*/
        /*[PXSelector(typeof(Search<BOQGroup.costCodeID, Where<BOQGroup.groupID, Equal<Current<GSBOQMatl.groupID>>>>), 
            SubstituteKey = (typeof(PMCostCode.costCodeCD)))]*/
        [PXDefault(typeof(Search<BOQGroup.costCodeID, Where<BOQGroup.groupID, Equal<Current<GSBOQMatl.groupID>>>>))]
        [CostCode(null, null, null, DescriptionField = typeof(PMCostCode.description))]
        [PXForeignReference(typeof(FK.CostCode))]
        public virtual int? CostCode { get; set; }
        public abstract class costCode : PX.Data.BQL.BqlInt.Field<costCode> { }
        #endregion

        #region Completed
        [PXDBBool()]
        [PXUIField(DisplayName = "Completed", Enabled = false)]
        public virtual bool? Completed { get; set; }
        public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
        #endregion
        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected>
        {
        }
        protected bool? _Selected = false;
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }
        #endregion
        /*
        #region ReqQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Req Qty")]
        public virtual Decimal? ReqQty { get; set; }
        public abstract class reqQty : PX.Data.BQL.BqlDecimal.Field<reqQty> { }
        #endregion*/
        #region CuryInfoID
        public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
        protected Int64? _CuryInfoID;

        [PXDBLong()]
        [CurrencyInfo(typeof(GSBOQ.curyInfoID))]
        public virtual Int64? CuryInfoID
        {
            get
            {
                return this._CuryInfoID;
            }
            set
            {
                this._CuryInfoID = value;
            }
        }
        #endregion
        #region UnitCost
        [PXDBPriceCost]
        [PXDefault(typeof(Search<INItemCost.lastCost,
            Where<INItemCost.inventoryID, Equal<Current<GSBOQMatl.inventoryID>>,
                And<INItemCost.curyID, Equal<Current<GSBOQ.curyID>>>>>))]

        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region EstQtyPhase
        [PXDBQuantity(typeof(GSBOQMatl.uOM), typeof(GSBOQMatl.gGHEstQtyPhase), HandleEmptyKey = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Req. Qty Phase")]
        public virtual Decimal? EstQtyPhase { get; set; }
        public abstract class estQtyPhase : PX.Data.BQL.BqlDecimal.Field<estQtyPhase> { }
        #endregion

        #region EPCEstQtyPhase
        [PXDBDecimal()]
        [PXUIField(DisplayName = "EPC Est. Qty Phase")]
        public virtual Decimal? EPCEstQtyPhase { get; set; }
        public abstract class ePCEstQtyPhase : PX.Data.BQL.BqlDecimal.Field<ePCEstQtyPhase> { }
        #endregion

        #region EPCEstQtyType1
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "EPC Est. Qty Type1", bOQID: typeof(GSBOQ.bOQID))]
        //  [PXUIField(DisplayName = " EPC Est. Qty Type1")]
        public virtual Decimal? EPCEstQtyType1 { get; set; }
        public abstract class ePCEstQtyType1 : PX.Data.BQL.BqlDecimal.Field<ePCEstQtyType1> { }
        #endregion

        #region EPCEstQtyType2
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "EPC Est. Qty Type2", bOQID: typeof(GSBOQ.bOQID))]
        // [PXUIField(DisplayName = " EPC Est. Qty Type2")]

        public virtual Decimal? EPCEstQtyType2 { get; set; }
        public abstract class ePCEstQtyType2 : PX.Data.BQL.BqlDecimal.Field<ePCEstQtyType2> { }
        #endregion

        #region EPCEstQtyType3
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "EPC Est. Qty Type3", bOQID: typeof(GSBOQ.bOQID))]
        // [PXUIField(DisplayName = " EPC Est. Qty Type3")]

        public virtual Decimal? EPCEstQtyType3 { get; set; }
        public abstract class ePCEstQtyType3 : PX.Data.BQL.BqlDecimal.Field<ePCEstQtyType3> { }
        #endregion

        #region EPCEstQtyType4
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "EPC Est. Qty Type4", bOQID: typeof(GSBOQ.bOQID))]
        // [PXUIField(DisplayName = " EPC Est. Qty Type4")]

        public virtual Decimal? EPCEstQtyType4 { get; set; }
        public abstract class ePCEstQtyType4 : PX.Data.BQL.BqlDecimal.Field<ePCEstQtyType4> { }
        #endregion

        #region EPCEstQtyType5
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "EPC Est. Qty Type5", bOQID: typeof(GSBOQ.bOQID))]
        // [PXUIField(DisplayName = " EPC Est. Qty Type5")]

        public virtual Decimal? EPCEstQtyType5 { get; set; }
        public abstract class ePCEstQtyType5 : PX.Data.BQL.BqlDecimal.Field<ePCEstQtyType5> { }
        #endregion

        #region EstQtyType1
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "Req. Qty Type1", bOQID: typeof(GSBOQ.bOQID))]
        // [PXUIField(DisplayName = " Est. Qty Type1")]

        public virtual Decimal? EstQtyType1 { get; set; }
        public abstract class estQtyType1 : PX.Data.BQL.BqlDecimal.Field<estQtyType1> { }
        #endregion

        #region EstQtyType2
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "Req. Qty Type2", bOQID: typeof(GSBOQ.bOQID))]
        // [PXUIField(DisplayName = " Est. Qty Type2")]

        public virtual Decimal? EstQtyType2 { get; set; }
        public abstract class estQtyType2 : PX.Data.BQL.BqlDecimal.Field<estQtyType2> { }
        #endregion

        #region EstQtyType3
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "Req. Qty Type3", bOQID: typeof(GSBOQ.bOQID))]
        //  [PXUIField(DisplayName = " Est. Qty Type3")]

        public virtual Decimal? EstQtyType3 { get; set; }
        public abstract class estQtyType3 : PX.Data.BQL.BqlDecimal.Field<estQtyType3> { }
        #endregion

        #region EstQtyType4
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "Req. Qty Type4", bOQID: typeof(GSBOQ.bOQID))]
        //   [PXUIField(DisplayName = " Est. Qty Type4")]

        public virtual Decimal? EstQtyType4 { get; set; }
        public abstract class estQtyType4 : PX.Data.BQL.BqlDecimal.Field<estQtyType4> { }
        #endregion

        #region EstQtyType5
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "Req. Qty Type5", bOQID: typeof(GSBOQ.bOQID))]
        //  [PXUIField(DisplayName = " Est. Qty Type5")]

        public virtual Decimal? EstQtyType5 { get; set; }
        public abstract class estQtyType5 : PX.Data.BQL.BqlDecimal.Field<estQtyType5> { }
        #endregion

        #region GGHEstQtyPhase
        [PXDBDecimal()]
        [PXUIField(DisplayName = "GGH Est Qty Phase")]
        public virtual Decimal? GGHEstQtyPhase { get; set; }
        public abstract class gGHEstQtyPhase : PX.Data.BQL.BqlDecimal.Field<gGHEstQtyPhase> { }
        #endregion

        #region GGHEstQtyType1
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "GGH Est. Qty Type1", bOQID: typeof(GSBOQ.bOQID))]
        //    [PXUIField(DisplayName = "GGH Est. Qty Type1")]
        public virtual Decimal? GGHEstQtyType1 { get; set; }
        public abstract class gGHEstQtyType1 : PX.Data.BQL.BqlDecimal.Field<gGHEstQtyType1> { }
        #endregion

        #region GGHEstQtyType2
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "GGH Est. Qty Type2", bOQID: typeof(GSBOQ.bOQID))]
        //    [PXUIField(DisplayName = "GGH Est. Qty Type2")]
        public virtual Decimal? GGHEstQtyType2 { get; set; }
        public abstract class gGHEstQtyType2 : PX.Data.BQL.BqlDecimal.Field<gGHEstQtyType2> { }
        #endregion

        #region GGHEstQtyType3
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "GGH Est. Qty Type3", bOQID: typeof(GSBOQ.bOQID))]
        //   [PXUIField(DisplayName = "GGH Est. Qty Type3")]
        public virtual Decimal? GGHEstQtyType3 { get; set; }
        public abstract class gGHEstQtyType3 : PX.Data.BQL.BqlDecimal.Field<gGHEstQtyType3> { }
        #endregion

        #region GGHEstQtyType4
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "GGH Est. Qty Type4", bOQID: typeof(GSBOQ.bOQID))]
        //   [PXUIField(DisplayName = "GGH Est. Qty Type4")]

        public virtual Decimal? GGHEstQtyType4 { get; set; }
        public abstract class gGHEstQtyType4 : PX.Data.BQL.BqlDecimal.Field<gGHEstQtyType4> { }
        #endregion

        #region GGHEstQtyType5
        [PXDBDecimal()]
        [GSBOQCapacity(fieldName: "GGH Est. Qty Type5", bOQID: typeof(GSBOQ.bOQID))]
        //  [PXUIField(DisplayName = "GGH Est. Qty Type5")]

        public virtual Decimal? GGHEstQtyType5 { get; set; }
        public abstract class gGHEstQtyType5 : PX.Data.BQL.BqlDecimal.Field<gGHEstQtyType5> { }
        #endregion

        #region Uom
        [PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(inventoryID), DisplayName = "UoM")]
        public virtual string UOM { get; set; }
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        #endregion

        #region GGHRemark
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "GGH Remark")]
        public virtual string GGHRemark { get; set; }
        public abstract class gGHremark : PX.Data.BQL.BqlString.Field<gGHremark> { }
        #endregion

        #region EPCRemark
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "EPC Remark")]
        public virtual string EPCRemark { get; set; }
        public abstract class ePCRemark : PX.Data.BQL.BqlString.Field<ePCRemark> { }
        #endregion

        /// <summary>
        /// TODO - Not Required
        /// </summary>
        #region Category
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Category", Visible = false)]
        public virtual string Category { get; set; }
        public abstract class category : PX.Data.BQL.BqlString.Field<category> { }
        #endregion

        #region Description
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
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