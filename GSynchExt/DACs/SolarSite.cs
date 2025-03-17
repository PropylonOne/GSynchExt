using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.TM;
using static GSynchExt.SolarSiteEntry;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("SolarSite")]
    [PXPrimaryGraph(typeof(SolarSiteEntry))]
    public class SolarSite : PXBqlTable, IBqlTable, IAssign, ISolarSiteFilter
    {
        #region Keys
        public class PK : PrimaryKeyOf<SolarSite>.By<solarSiteID>
        {
            public static SolarSite Find(PXGraph graph, int? solarSiteID) => FindBy(graph, solarSiteID);
            public static SolarSite FindDirty(PXGraph graph, int? solarSiteID)
                => (SolarSite)PXSelect<SolarSite, Where<solarSiteID, Equal<Required<solarSiteID>>>>.SelectWindowed(graph, 0, 1, solarSiteID);
        }
        public class UK : PrimaryKeyOf<SolarSite>.By<solarSiteCD>
        {
            public static SolarSite Find(PXGraph graph, string solarSiteCD) => FindBy(graph, solarSiteCD);

        }
        public class FK
        {
            public class Phase : GSynchExt.Phase.PK.ForeignKeyOf<SolarSite>.By<phaseID, province> { }
        }


        #endregion

        #region Events
        public class Events : PXEntityEvent<SolarSite>.Container<Events>
        {
            public PXEntityEvent<SolarSite> Event1;
        }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion

        #region SolarSiteID
        [PXDBIdentity(IsKey = true)]
        [PXReferentialIntegrityCheck]
        public virtual int? SolarSiteID { get; set; }
        public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }
        #endregion

        #region SolarSiteCD
        [SiteRawAttribute(IsKey = true)]
        [PXDefault]
        [PXReferentialIntegrityCheck]

        public virtual string SolarSiteCD { get; set; }
        public abstract class solarSiteCD : PX.Data.BQL.BqlString.Field<solarSiteCD> { }
        #endregion

        #region ProjectID
        [PXDBInt]
        [PXUIField(DisplayName = "Project Ref.", Enabled = false)]
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.contractCD, Equal<Current<SolarSite.solarSiteCD>>>>), SubstituteKey = (typeof(PMProject.contractCD)))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region DfltWareHouse
        public abstract class dfltWareHouse : PX.Data.BQL.BqlInt.Field<dfltWareHouse> { }
        [PXDBInt]
        [PXUIField(DisplayName = "Default Warehouse", Visibility = PXUIVisibility.Visible)]
        [PXDimensionSelector(SiteAttribute.DimensionName, typeof(INSite.siteID), typeof(INSite.siteCD))]
        public virtual Int32? DfltWareHouse
        {
            get;
            set;
        }
        #endregion

        #region UoM
        //[INUnit(DisplayName = "UoM", Visibility = PXUIVisibility.SelectorVisible)]
        [INUnit(DisplayName = "UoM", Visible = false, Enabled = false)]
        public virtual string UoM { get; set; }
        public abstract class uoM : PX.Data.BQL.BqlString.Field<uoM> { }
        #endregion

        #region SiteCapacity
        public abstract class siteCapacity : PX.Data.BQL.BqlDecimal.Field<siteCapacity> { }
        [PXDBQuantity]

        [PXUIField(DisplayName = "DC Capacity (kW)", Visibility = PXUIVisibility.Visible)]
        public virtual decimal? SiteCapacity
        {
            get;
            set;
        }
        #endregion

        #region ACCapacity
        public abstract class aCCapacity : PX.Data.BQL.BqlDecimal.Field<aCCapacity> { }
        [PXDBQuantity]
        [PXUIField(DisplayName = "AC Capacity (kW)", Visibility = PXUIVisibility.Visible)]
        public virtual decimal? ACCapacity
        {
            get;
            set;
        }
        #endregion

        #region PVSyst
        public abstract class pVSyst : PX.Data.BQL.BqlDecimal.Field<pVSyst> { }
        [PXDBQuantity]
        [PXUIField(DisplayName = "PVSyst (No Shading)", Visibility = PXUIVisibility.Visible)]
        public virtual decimal? PVSyst
        {
            get;
            set;
        }
        #endregion

        #region EstSiteValue
        public abstract class estSiteValue : PX.Data.BQL.BqlDecimal.Field<estSiteValue> { }
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Estimated Site Value (LKR)", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual decimal? EstSiteValue
        {
            get;
            set;
        }
        #endregion


        #region EPCVendorID
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "EPC Vendor", Enabled = false)]
        public virtual int? EPCVendorID { get; set; }
        public abstract class ePCVendorID : PX.Data.BQL.BqlString.Field<ePCVendorID> { }

        #endregion

        #region AreaEngineer
        public abstract class areaEngineer : PX.Data.BQL.BqlInt.Field<areaEngineer> { }
        protected int? _AreaEngineer;
        [PX.TM.Owner(typeof(SolarSite.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Area Engineer ", Visibility = PXUIVisibility.Visible)]

        public virtual int? AreaEngineer
        { get; set; }
        #endregion


        #region SiteDesignBy
        public abstract class siteDesignBy : PX.Data.BQL.BqlInt.Field<siteDesignBy> { }
        [PX.TM.Owner(typeof(SolarSite.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Site Design Done By", Visibility = PXUIVisibility.Visible)]
        public virtual int? SiteDesignBy
        { get; set; }
        #endregion

        #region ProjectManager
        public abstract class projectManager : PX.Data.BQL.BqlInt.Field<projectManager> { }
        protected int? _ProjectManager;
        [PX.TM.Owner(typeof(SolarSite.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Site Engineer ", Visibility = PXUIVisibility.Visible)]

        public virtual int? ProjectManager
        { get; set; }
        #endregion

        #region Address
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Address", Required = true)]
        [PXDefault]
        public virtual string Address { get; set; }
        public abstract class address : PX.Data.BQL.BqlString.Field<address> { }
        #endregion

        #region TemplateID
        public new abstract class templateID : PX.Data.BQL.BqlInt.Field<templateID> { }

        /// <summary>The template for the project.</summary>
        [PXUIField(DisplayName = "Project Template (BOM)", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionNameTemplate)]
        [PXDimensionSelector(ProjectAttribute.DimensionNameTemplate,
                typeof(Search2<PMProject.contractID,
                        LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID, Equal<PMProject.contractID>>>,
                            Where<PMProject.baseType, Equal<PX.Objects.CT.CTPRType.projectTemplate>, And<PMProject.isActive, Equal<True>>>>),
                typeof(PMProject.contractCD),
                typeof(PMProject.contractCD),
                typeof(PMProject.description),
                typeof(PMProject.budgetLevel),
                typeof(PMProject.billingID),
                typeof(ContractBillingSchedule.type),
                typeof(PMProject.ownerID),
                DescriptionField = typeof(PMProject.description))]
        [PXDBInt]
        [PXForeignReference(typeof(Field<templateID>.IsRelatedTo<PMProject.contractID>))]
        public Int32? TemplateID
        {
            get;
            set;
        }
        #endregion

        #region CEBAccount

        [PXDBString(30, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "CEB Account")]
        [PXCheckUnique(ErrorMessage = Messages.CEBAccountIsNotUnique)]
        public virtual string CEBAccount { get; set; }
        public abstract class cEBAccount : PX.Data.BQL.BqlString.Field<cEBAccount> { }
        #endregion

        #region SiteType
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<SiteType.sSiteType>), SubstituteKey = typeof(SiteType.description))]

        [PXUIField(DisplayName = "Site Type")]
        public virtual string SiteType { get; set; }
        public abstract class siteType : PX.Data.BQL.BqlString.Field<siteType> { }
        #endregion

        #region SiteStatus
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [GSynchExt.Status.SSList]
        [PXUIField(DisplayName = "Site Status", Enabled = false)]
        public virtual string SiteStatus { get; set; }
        public abstract class siteStatus : PX.Data.BQL.BqlString.Field<siteStatus> { }
        #endregion

        #region SiteName
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Site Name")]
        [PXDefault]
        public virtual string SiteName { get; set; }
        public abstract class siteName : PX.Data.BQL.BqlString.Field<siteName> { }
        #endregion

        #region SurveyApproved
        [PXDBBool()]
        [PXUIField(DisplayName = "Survey Approved", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(false)]
        public virtual bool? SurveyApproved { get; set; }
        public abstract class surveyApproved : PX.Data.BQL.BqlBool.Field<surveyApproved> { }
        #endregion

        #region Province
        [PXDBString(3, IsUnicode = true, InputMask = ">CCC")]
        [PXUIField(DisplayName = "Province")]
        [PXSelector(typeof(Search<State.stateID, Where<State.countryID, Equal<Provinces.CountryLK>>>))]
        [PXDefault]
        public virtual string Province { get; set; }
        public abstract class province : PX.Data.BQL.BqlString.Field<province> { }
        #endregion

        #region District
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "District")]

        [PXSelector(typeof(Search<Districts.districtID>), typeof(Districts.districtID),SubstituteKey = typeof(Districts.description))]
        [PXRestrictor(typeof(Where<GSynchExt.Districts.stateID.IsEqual<GSynchExt.SolarSite.province.FromCurrent>>), "Invalid District")]

        public virtual string District { get; set; }
        public abstract class district : PX.Data.BQL.BqlString.Field<district> { }
        #endregion

        #region PhaseID
        [PXDBString(3, IsUnicode = true, InputMask = ">CCC")]
        [PXUIField(DisplayName = "Phase ID")]
        [PXSelector(typeof(Search<Phase.phaseID, Where<GSynchExt.Phase.stateID, Equal<Current<GSynchExt.SolarSite.province>>>>))]
        [PXForeignReference(typeof(FK.Phase))]
        // [PXRestrictor(typeof(Where<GSynchExt.Phase.stateID.IsEqual<GSynchExt.SolarSite.province.FromCurrent>>), "Invalid Phase")]

        public virtual string PhaseID { get; set; }
        public abstract class phaseID : PX.Data.BQL.BqlString.Field<phaseID> { }
        #endregion

        #region ClusterID
        [PXDBString(3, IsUnicode = true, InputMask = ">CCC")]
        [PXUIField(DisplayName = "Cluster ID")]
        [PXSelector(typeof(Search<Cluster.clusterID, Where<GSynchExt.Cluster.stateID, Equal<Current<GSynchExt.SolarSite.province>>>>))]

        //[PXRestrictor(typeof(Where<GSynchExt.Cluster.stateID.IsEqual<GSynchExt.SolarSite.province.FromCurrent>>), "Invalid Cluster")]

        public virtual string ClusterID { get; set; }
        public abstract class clusterID : PX.Data.BQL.BqlString.Field<clusterID> { }
        #endregion

        #region FieldExecutive
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Field Executive")]
        [PXSelector(typeof(Search<PX.Objects.EP.EPEmployee.acctCD>), SubstituteKey = typeof(PX.Objects.EP.EPEmployee.acctName))]
        public virtual string FieldExecutive { get; set; }
        public abstract class fieldExecutive : PX.Data.BQL.BqlString.Field<fieldExecutive> { }
        #endregion

        #region StartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Project Start", Enabled = false)]
        public virtual DateTime? StartDate { get; set; }
        public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion

        #region InServiceDate
        [PXDBDate()]
        [PXUIField(DisplayName = "In Service", Enabled = false)]
        public virtual DateTime? InServiceDate { get; set; }
        public abstract class inServiceDate : PX.Data.BQL.BqlDateTime.Field<inServiceDate> { }
        #endregion

        #region EndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Project End", Enabled = false)]
        public virtual DateTime? EndDate { get; set; }
        public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
        #endregion

        #region ProjPlannedStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Project Planned Start Date")]
        public virtual DateTime? ProjPlannedStartDate { get; set; }
        public abstract class projPlannedStartDate : PX.Data.BQL.BqlDateTime.Field<projPlannedStartDate> { }
        #endregion

        #region ConstructionStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Construction Start", Enabled = false)]
        public virtual DateTime? ConstructionStartDate { get; set; }
        public abstract class constructionStartDate : PX.Data.BQL.BqlDateTime.Field<constructionStartDate> { }
        #endregion

        #region ConstructionEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Construction End", Enabled = false)]
        public virtual DateTime? ConstructionEndDate { get; set; }
        public abstract class constructionEndDate : PX.Data.BQL.BqlDateTime.Field<constructionEndDate> { }
        #endregion

        #region CommissionedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Commissioned", Enabled = false)]
        public virtual DateTime? CommissionedDate { get; set; }
        public abstract class commissionedDate : PX.Data.BQL.BqlDateTime.Field<commissionedDate> { }
        #endregion

        #region ConnectedtoGridDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Connected to Grid", Enabled = false)]
        public virtual DateTime? ConnectedtoGridDate { get; set; }
        public abstract class connectedtoGridDate : PX.Data.BQL.BqlDateTime.Field<connectedtoGridDate> { }
        #endregion

        #region CancelledDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Cancelled", Enabled = false)]
        public virtual DateTime? CancelledDate { get; set; }
        public abstract class cancelledDate : PX.Data.BQL.BqlDateTime.Field<cancelledDate> { }
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

        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        protected Guid? _NoteID;
        [PXNote(DescriptionField = typeof(SolarSite.solarSiteID))]
        public virtual Guid? NoteID
        {
            get
            {
                return this._NoteID;
            }
            set
            {
                this._NoteID = value;
            }
        }
        #endregion

        #region WorkgroupID
        public abstract class workgroupID : PX.Data.BQL.BqlInt.Field<workgroupID> { }
        protected int? _WorkgroupID;
        [PXDBInt]
        [PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSubordinateGroupSelectorAttribute]
        public virtual int? WorkgroupID
        {
            get;
            set;
        }
        #endregion

        #region OwnerID
        public abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
        protected int? _OwnerID;
        [PX.TM.Owner(typeof(SolarSite.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? OwnerID
        { get; set; }
        #endregion

        #region Rejected
        public abstract class rejected : BqlBool.Field<rejected>
        {
        }
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? Rejected { get; set; }
        #endregion

        #region Hold
        public abstract class hold : BqlBool.Field<hold>
        {
        }
        [PXDBBool]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        #endregion

        #region IsApprover
        public abstract class isApprover : BqlDecimal.Field<isApprover>
        {
        }
        [PXBool]
        public virtual bool? IsApprover { get; set; }
        #endregion


        #region Attributes
        public abstract class attributes : BqlAttributes.Field<attributes> { }

        /// <summary>The entity attributes.</summary>
        [CRAttributesField(typeof(classID))]
        public virtual string[] Attributes { get; set; }

        #region ClassID

        public abstract class classID : PX.Data.BQL.BqlString.Field<classID> { }
        /// <summary>The class ID for the attributes.</summary>
        /// <value>Always returns <see cref="GroupTypes.SolarSite" />.</value>
        [PXString(20)]
        public virtual string ClassID
        {
            get { return GroupTypes.SolarSite; }
        }
        #endregion

        #endregion


        public class attributeDistRange : PX.Data.BQL.BqlString.Constant<attributeDistRange> { public attributeDistRange() : base("SURDISTRAN") { } }
        public class attributeStories : PX.Data.BQL.BqlString.Constant<attributeStories> { public attributeStories() : base("SURSTORIES") { } }
        public class attributeMaterial : PX.Data.BQL.BqlString.Constant<attributeMaterial> { public attributeMaterial() : base("SURROOFMAT") { } }
        public class attributeBuilCondition : PX.Data.BQL.BqlString.Constant<attributeBuilCondition> { public attributeBuilCondition() : base("SURBUILCON") { } }
        public class attributeRoofCondition : PX.Data.BQL.BqlString.Constant<attributeRoofCondition> { public attributeRoofCondition() : base("SURROOFCON") { } }
        public class attributePitch : PX.Data.BQL.BqlString.Constant<attributePitch> { public attributePitch() : base("SURSTORIES") { } }
        public class attributeRepPercent : PX.Data.BQL.BqlString.Constant<attributeRepPercent> { public attributeRepPercent() : base("SURREPPNTG") { } }
        public class attributeOrientation : PX.Data.BQL.BqlString.Constant<attributeRepPercent> { public attributeOrientation() : base("SURORIENT") { } }
        public class projCDStartsWith : PX.Data.BQL.BqlString.Constant<attributeRepPercent> { public projCDStartsWith() : base("GG") { } }
        public class dutilityCEB : PX.Data.BQL.BqlString.Constant<dutilityCEB> { public dutilityCEB() : base("CEB") { } }
        public class dutilityLECO : PX.Data.BQL.BqlString.Constant<dutilityLECO> { public dutilityLECO() : base("LECO") { } }
    }
}