using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CT;
using PX.Objects.IN;
using PX.Objects.PM;

namespace PX.Objects
{
    [Serializable]
    [PXCacheName("Project Stock2")]
    public class ProjectStock2 : PXBqlTable, IBqlTable
    {

        public class UK : PrimaryKeyOf<ProjectStock>.By<projectID>
        {
            public static ProjectStock Find(PXGraph graph, int projectID, int taskID, int siteID, int costCodeID, int locationID ) => FindBy(graph,  projectID);
        }
        #region Selected
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : BqlBool.Field<selected> { }
        #endregion

        #region CompanyID
        // Acuminator disable once PX1027 ForbiddenFieldsInDacDeclaration [Justification]
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Company ID", Enabled = false)]
        public virtual int? CompanyID { get; set; }
        // Acuminator disable once PX1027 ForbiddenFieldsInDacDeclaration [Justification]
        public abstract class companyID : PX.Data.BQL.BqlInt.Field<companyID> { }
        #endregion

        #region QtySelected
        public abstract class qtySelected : PX.Data.BQL.BqlDecimal.Field<qtySelected> { }
        protected Decimal? _QtySelected;
        [PXQuantity]
       // [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Selected")]
        public virtual Decimal? QtySelected
        {
            get;
            set;
        }
        #endregion

        #region InventoryID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey = (typeof(InventoryItem.inventoryCD)))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region Uom
        //    [PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(DisplayName = "UoM", IsKey = true)]
        public virtual string UOM { get; set; }
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        #endregion

        #region ProjectID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Project ID", Enabled = false)]
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.nonProject, Equal<False>, And<PMProject.baseType, Equal<CTPRType.project>>>>),
            SubstituteKey = (typeof(PMProject.contractCD)))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region TaskID
        [ProjectTask(typeof(ProjectStock.projectID), IsKey = true)]
        [PXUIField(DisplayName = "Task ID", Enabled = false)]
        public virtual int? TaskID { get; set; }
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
        #endregion

        #region Descr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Descr", Enabled = false)]
        public virtual string Descr { get; set; }
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
        #endregion

        #region SiteID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Warehouse", Enabled = false)]
        [PXSelector(typeof(Search<INSite.siteID>), SubstituteKey = (typeof(INSite.siteCD)))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        #endregion

        #region LocationID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Location ID", Enabled = false)]
        [PXSelector(typeof(Search<INLocation.locationID>), SubstituteKey = (typeof(INLocation.locationCD)))]
        public virtual int? LocationID { get; set; }
        public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
        #endregion

        #region CostCodeID
        [PXUIField(DisplayName = "Cost Code ID", Enabled = false)]
        [CostCode(null, null, null, DescriptionField = typeof(PMCostCode.description), IsKey = true)]
        public virtual int? CostCodeID { get; set; }
        public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
        #endregion

        #region LotSerialNbr
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Lot Serial Nbr", Enabled = false)]
        public virtual string LotSerialNbr { get; set; }
        public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<lotSerialNbr> { }
        #endregion

        #region TotalTransferINQty
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "TotalTransferINQty", Enabled = false)]
        public virtual Decimal? TotalTransferINQty { get; set; }
        public abstract class totalTransferINQty : PX.Data.BQL.BqlDecimal.Field<totalTransferINQty> { }
        #endregion

        #region TotalTransferOUTQty
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "TotalTransferOUTQty", Enabled = false)]
        public virtual Decimal? TotalTransferOUTQty { get; set; }
        public abstract class totalTransferOUTQty : PX.Data.BQL.BqlDecimal.Field<totalTransferOUTQty> { }
        #endregion

        #region TotalIssueQty
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "TotalIssueQty", Enabled = false)]
        public virtual Decimal? TotalIssueQty { get; set; }
        public abstract class totalIssueQty : PX.Data.BQL.BqlDecimal.Field<totalIssueQty> { }
        #endregion

        #region TotalAvailableQty
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "Total Available Qty.", Enabled = false)]
        public virtual Decimal? TotalAvailableQty { get; set; }
        public abstract class totalAvailableQty : PX.Data.BQL.BqlDecimal.Field<totalAvailableQty> { }
        #endregion

    }

    [PXCacheName("Project Stock Filter2")]
    public partial class ProjectStockFilter2 : PXBqlTable, IBqlTable
    {
        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project ID")]
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.nonProject, Equal<False>>>), typeof(PMProject.contractCD),
            typeof(PMProject.description), typeof(PMProject.status),
            SubstituteKey = (typeof(PMProject.contractCD)))]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region IsActiveTasks
        [PXBool]
        [PXUnboundDefault(true)]
        [PXUIField(DisplayName = "Active Tasks Only")]
        public virtual bool? IsActiveTasks { get; set; }
        public abstract class isActiveTasks : BqlBool.Field<isActiveTasks> { }
        #endregion

        #region IsReturn
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Return")]
        public virtual bool? IsReturn { get; set; }
        public abstract class isReturn : BqlBool.Field<isReturn> { }
        #endregion

        #region FromWarehouse
        [PXDBInt()]
        [PXUIField(DisplayName = "From Warehouse")]
        [PXSelector(typeof(Search<INSite.siteID>), SubstituteKey = (typeof(INSite.siteCD)))]
        [PXDefault(typeof(Current<INRegister.siteID>))]
        public virtual int? FromWarehouse { get; set; }
        public abstract class fromWarehouse : PX.Data.BQL.BqlInt.Field<fromWarehouse> { }
        #endregion 
    }
 //   public class ProjectType : PX.Data.BQL.BqlString.Constant<ProjectType> { public ProjectType() : base("P") { } }

}