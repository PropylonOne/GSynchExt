using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.CT;
using GSynchExt;

namespace PX.Objects.FS
{
    #region PXProjection

    [Serializable]
    [PXProjection(typeof(
            Select2<InventoryItem,
            InnerJoin<INTran,
                On<INTran.inventoryID, Equal<InventoryItem.inventoryID>,
                And<INTran.docType, Equal<INDocType.issue>,
                And<INTran.costLayerType, Equal<CostLayerType.project>>>>,
            InnerJoin<INTranSplit,
                On<INTranSplit.docType, Equal<INTran.docType>,
                And<INTranSplit.refNbr, Equal<INTran.refNbr>,
                And<INTranSplit.lineNbr, Equal<INTran.lineNbr>>>>,
            InnerJoin<PMBudget,
                On<PMBudget.inventoryID, Equal<INTran.inventoryID>,
                And<PMBudget.projectID, Equal<INTran.projectID>,
                And<PMBudget.projectTaskID, Equal<INTran.taskID>,
                And<PMBudget.costCodeID, Equal<INTran.costCodeID>>>>>,
            InnerJoin<Contract,
                On<Contract.contractID, Equal<PMBudget.projectID>>,
            InnerJoin<SolarSite,
                On<SolarSite.solarSiteCD, Equal<Contract.contractCD>,
                    And<SolarSite.inServiceDate, IsNotNull>>,
            LeftJoin<FSEquipment,
                On<FSEquipment.inventoryID, Equal<InventoryItem.inventoryID>,
                    And<FSEquipment.serialNumber, Equal<INTranSplit.lotSerialNbr>>>>>>>>>,
            Where<InventoryItem.stkItem, Equal<True>,
                And<InventoryItem.itemStatus, Equal<InventoryItemStatus.active>,
                And<FSxEquipmentModel.eQEnabled, Equal<True>,
                And<FSxEquipmentModel.equipmentItemClass, Equal<ListField_EquipmentItemClass.ModelEquipment>,
                And<FSEquipment.refNbr, IsNull>>>>>>))]
    //[PXGroupMask(typeof(InnerJoinSingleTable<Contract, On<Contract.contractID, Equal<ProjIssuedEQInventoryItem.projectID>>>))]
    #endregion
    public class ProjIssuedEQInventoryItem : PXBqlTable, IBqlTable
    {
        #region ProjectID
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

        [PXSelector(typeof(Search<Contract.contractID>), SubstituteKey = typeof(Contract.contractCD))]
        [PXDBInt(BqlField = typeof(Contract.contractID))]
        [PXUIField(DisplayName = "Project ID.", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? ProjectID { get; set; }
        #endregion
        #region SolarSiteID
        public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }

        [PXSelector(typeof(Search<SolarSite.solarSiteID>))]
        [PXDBInt(BqlField = typeof(SolarSite.solarSiteID))]
        [PXUIField(DisplayName = "Solar Site ID", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? SolarSiteID { get; set; }
        #endregion
        #region SolarSiteCD
        public abstract class solarSiteCD : PX.Data.BQL.BqlString.Field<solarSiteCD> { }
        [PXDBString(BqlField = typeof(SolarSite.solarSiteCD))]
        [PXUIField(DisplayName = "Solar Site", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public virtual string SolarSiteCD { get; set; }
        #endregion
        #region RefNbr
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }

        [PXSelector(typeof(Search<INTran.refNbr>))]
        [PXDBString(IsKey = true, BqlField = typeof(INTran.refNbr))]
        [PXUIField(DisplayName = "Ref Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String RefNbr { get; set; }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        [PXDBInt(IsKey = true, BqlField = typeof(INTran.lineNbr))]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? LineNbr { get; set; }
        #endregion

        #region INLineSplitNumber
        public abstract class iNLineSplitNumber : PX.Data.BQL.BqlInt.Field<iNLineSplitNumber> { }

        [PXDBInt(IsKey = true, BqlField = typeof(INTranSplit.splitLineNbr))]
        [PXUIField(DisplayName = "Split Line Nbr.", Visibility = PXUIVisibility.Visible)]
        public virtual int? INLineSplitNumber { get; set; }
        #endregion
        #region DocDate
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }

        [PXSelector(typeof(Search<INTran.tranDate>))]
        [PXDBDate(BqlField = typeof(INTran.tranDate))]
        [PXUIField(DisplayName = "Doc Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DocDate { get; set; }
        #endregion
        #region DocType
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }

        [INDocType.List]
        [PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(INTran.docType))]
        public virtual string DocType { get; set; }
        #endregion
        #region ProjectTaskID
        public abstract class projectTaskID : PX.Data.BQL.BqlInt.Field<projectTaskID> { }

        [PXDBInt(BqlField = typeof(PMBudget.projectTaskID))]
        [PXUIField(DisplayName = "Project Task", Visibility = PXUIVisibility.Visible)]
        public virtual int? ProjectTaskID { get; set; }
        #endregion
        
        #region Descr
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

        [PXDBLocalizableString(255, IsUnicode = true, BqlField = typeof(InventoryItem.descr), IsProjection = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Descr { get; set; }
        #endregion
        #region SiteID
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

        [Site(DisplayName = "Warehouse", DescriptionField = typeof(INSite.descr), BqlField = typeof(INTran.siteID))]
        public virtual int? SiteID { get; set; }
        #endregion
        #region EquipmentTypeID
        public abstract class equipmentTypeID : PX.Data.BQL.BqlInt.Field<equipmentTypeID> { }

        [PXDBInt]
        [PXUIField(DisplayName = "Equipment Type")]
        [FSSelectorEquipmentType]
        public virtual int? EquipmentTypeID { get; set; }
        #endregion
        #region ItemClassID
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

        [PXDBInt(BqlField = typeof(InventoryItem.itemClassID))]
        [PXSelector(typeof(Search<INItemClass.itemClassID>), SubstituteKey = typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr))]
        [PXUIField(DisplayName = "Item Class ID", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? ItemClassID { get; set; }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        [PXDBIdentity(BqlField = typeof(InventoryItem.inventoryID))]
        public virtual int? InventoryID { get; set; }
        #endregion
        #region InventoryCD
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }

        [InventoryRaw(IsKey = true, DisplayName = "Inventory ID", BqlField = typeof(InventoryItem.inventoryCD))]
        [PXDefault]
        [PXFieldDescription]
        public virtual string InventoryCD { get; set; }
        #endregion

        // TODO: Rename this field to LotSerialNbr.
        // Change the label to "Lot/Serial Nbr.".
        #region LotSerialNumber
        public abstract class lotSerialNumber : PX.Data.BQL.BqlString.Field<lotSerialNumber> { }

        [PXDBString(30, IsUnicode = true, BqlField = typeof(INTranSplit.lotSerialNbr))]
        [PXUIField(DisplayName = "Serial Nbr", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string LotSerialNumber { get; set; }
        #endregion

        #region SerialQty
        public abstract class serialQty : PX.Data.BQL.BqlDecimal.Field<serialQty> { }

        [PXDBQuantity(BqlField = typeof(INTranSplit.qty))]
        [PXUIField(DisplayName = "Serial Qty", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual decimal? SerialQty { get; set; }
        #endregion
        #region Qty
        public abstract class qty : IBqlField
        {
        }

        [PXDBQuantity(BqlField = typeof(INTran.qty))]
        [PXUIField(DisplayName = "Issue Qty", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual decimal? Qty { get; set; }
        #endregion
        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        [PXBool]
        [PXFormula(typeof(False))]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion
    }
}
