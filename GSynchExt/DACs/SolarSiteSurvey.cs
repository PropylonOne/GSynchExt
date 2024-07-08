using System;
using PX.CS.Contracts.Interfaces;
using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.SM;
using PX.TM;
using PX.Web.UI;
using static PX.Data.PXAccess;
using BAccount = PX.Data.PXAccess.BAccount;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("SolarSiteSurvey")]
    [PXPrimaryGraph(typeof(SolarSiteSurveyMaint))]
    public class SolarSiteSurvey : PXBqlTable, IBqlTable, IAssign, IAddressLocation
    {

        #region Keys
        public class UK : PrimaryKeyOf<SolarSiteSurvey>.By<surveyID>
        {
            public static SolarSiteSurvey Find(PXGraph graph, string surveyID) => FindBy(graph, surveyID);
        }

        #endregion

        #region SolarSiteID
        [PXUIField(DisplayName = "Solar Site ID")]
        [PXSelector(typeof(Search<SolarSite.solarSiteID,

            Where<SolarSite.siteStatus, NotEqual<Status.cancelled>,

            And<SolarSite.siteStatus, NotEqual<Status.closed>>>>), typeof(SolarSite.siteName), typeof(SolarSite.siteStatus), typeof(SolarSite.province), typeof(SolarSite.phaseID), SubstituteKey = typeof(SolarSite.solarSiteCD))]
        [PXDBInt]
        public virtual int? SolarSiteID { get; set; }
        public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }
        #endregion

        #region SurveyID
        [PXDBString(30, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<SolarSiteSurvey.surveyID>), typeof(SolarSiteSurvey.surveyID))]
        [AutoNumber(typeof(SiteSetup.surveyNumberingID), typeof(SolarSiteSurvey.createdDateTime))]
        public virtual string SurveyID { get; set; }
        public abstract class surveyID : PX.Data.BQL.BqlString.Field<surveyID> { }
        #endregion

        #region StartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Start Date", Enabled = false)]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? StartDate { get; set; }
        public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Line Cntr")]
        public virtual int? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion

        #region FieldExecutive
        public abstract class fieldExecutive : PX.Data.BQL.BqlInt.Field<fieldExecutive> { }
        protected int? _FieldExecutive;
        [PX.TM.Owner(typeof(SolarSiteSurvey.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Survey Proj. Assistance", Visibility = PXUIVisibility.Visible)]

        public virtual int? FieldExecutive
        { get; set; }
        #endregion

        #region CEBAccount

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CEB Account")]
        public virtual string CEBAccount { get; set; }
        public abstract class cEBAccount : PX.Data.BQL.BqlString.Field<cEBAccount> { }
        #endregion


        #region SiteStatus
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXDefault(Messages.OnHold)]
        [PXStringList(
         new string[]
         {
         GSynchExt.Status.OnHold ,
         GSynchExt.Status.PendingApproval,
         GSynchExt.Status.Completed,
         GSynchExt.Status.Rejected,

         },
         new string[]
         {
         GSynchExt.Messages.OnHold,
         GSynchExt.Messages.PendingApproval,
         GSynchExt.Messages.Completed,
         GSynchExt.Messages.Rejected,

         })]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        public virtual string SiteStatus { get; set; }
        public abstract class siteStatus : PX.Data.BQL.BqlString.Field<siteStatus> { }
        #endregion

        #region Description
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        [PXDefault()]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region SurveyApproved
        [PXDBBool()]
        [PXUIField(DisplayName = "Approved", Enabled = false)]
        [PXDefault(false)]
        public virtual bool? SurveyApproved { get; set; }
        public abstract class surveyApproved : PX.Data.BQL.BqlBool.Field<surveyApproved> { }
        #endregion

        #region TODO Remove
        #region SiteType
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXStringList(
         new string[]
         {
         GSynchExt.Type.School,
         GSynchExt.Type.Hospital,
         GSynchExt.Type.PoliceStation,
         GSynchExt.Type.Stadium,
         GSynchExt.Type.Other

         },
         new string[]
         {
         GSynchExt.Messages.School,
         GSynchExt.Messages.Hospital,
         GSynchExt.Messages.PoliceStation,
         GSynchExt.Messages.Stadium,
         GSynchExt.Messages.Other

         })]

        [PXUIField(DisplayName = "Site Type")]
        public virtual string SiteType { get; set; }
        public abstract class siteType : PX.Data.BQL.BqlString.Field<siteType> { }
        #endregion
        #region Province
        [PXDBString(3, IsUnicode = true, InputMask = ">CCC")]
        [PXUIField(DisplayName = "Province")]
        public virtual string Province { get; set; }
        public abstract class province : PX.Data.BQL.BqlString.Field<province> { }
        #endregion

        #region District
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "District")]

        [PXSelector(typeof(Search<Districts.districtID>), SubstituteKey = typeof(Districts.description))]
        [PXRestrictor(typeof(Where<GSynchExt.Districts.stateID.IsEqual<GSynchExt.SolarSite.province.FromCurrent>>), "")]

        public virtual string District { get; set; }
        public abstract class district : PX.Data.BQL.BqlString.Field<district> { }
        #endregion

        #region PhaseID
        [PXDBString(3, IsUnicode = true, InputMask = ">CCCC")]
        [PXUIField(DisplayName = "Phase ID")]
        public virtual string PhaseID { get; set; }
        public abstract class phaseID : PX.Data.BQL.BqlString.Field<phaseID> { }
        #endregion

        #region ClusterID
        [PXDBString(3, IsUnicode = true, InputMask = ">CCCC")]
        [PXUIField(DisplayName = "Cluster ID")]
        public virtual string ClusterID { get; set; }
        public abstract class clusterID : PX.Data.BQL.BqlString.Field<clusterID> { }
        #endregion
        #region ProjPlannedStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Project Planned Start Date")]
        public virtual DateTime? ProjPlannedStartDate { get; set; }
        public abstract class projPlannedStartDate : PX.Data.BQL.BqlDateTime.Field<projPlannedStartDate> { }
        #endregion

        #endregion ///TODO

        #region EndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "End Date", Enabled = false)]
        public virtual DateTime? EndDate { get; set; }
        public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
        #endregion



        #region DistributionUtility
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXStringList(
         new string[]
         {
         GSynchExt.DUtility.CEB,
         GSynchExt.DUtility.LECO

         },
         new string[]
         {
         GSynchExt.Messages.CEB,
         GSynchExt.Messages.LECO

         })]
        [PXUIField(DisplayName = "Distribution Utility")]
        // [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<attributeDistrUtil>>>))]
        public virtual string DistributionUtility { get; set; }
        public abstract class distributionUtility : PX.Data.BQL.BqlString.Field<distributionUtility> { }
        #endregion

        #region CEBOffice
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CEB Office", Enabled = false)]

        //  [PXSelector(typeof(Search<CEBLocations.locationID>), SubstituteKey = typeof(CEBLocations.description))]
       [PXSelector(typeof(Search2<
            CEBLocations.locationID,
                LeftJoin<SolarSite,
                    On<SolarSite.solarSiteID, Equal<Current<solarSiteID>>>>,
            Where<
                CEBLocations.stateID, Equal<SolarSite.province>>>), SubstituteKey = typeof(CEBLocations.description))]

        //  [PXRestrictor(typeof(Where<CEBLocations.stateID.IsEqual<province.FromCurrent>>), "Invalid CEB Office Location")]

        public virtual string CEBOffice { get; set; }
        public abstract class cEBOffice : PX.Data.BQL.BqlString.Field<cEBOffice> { }
        #endregion

        #region Lecooffice
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "LECO Office", Enabled = false)]

        // [PXSelector(typeof(Search<LECOLocations.locationID>), SubstituteKey = typeof(LECOLocations.description))]
        //[PXRestrictor(typeof(Where<LECOLocations.stateID.IsEqual<province.FromCurrent>>), "Invalid LECO Office Location")]

        [PXSelector(typeof(Search2<
            LECOLocations.locationID,
                LeftJoin<SolarSite,
                    On<SolarSite.solarSiteID, Equal<Current<solarSiteID>>>>,
            Where<
                LECOLocations.stateID, Equal<SolarSite.province>>>), SubstituteKey = typeof(LECOLocations.description))]

        public virtual string Lecooffice { get; set; }
        public abstract class lecooffice : PX.Data.BQL.BqlString.Field<lecooffice> { }
        #endregion

        #region GPSCoordinates
        [PXDBString()]
        [PXUIField(DisplayName = "GPS Coordinates")]
        public virtual string GPSCoordinates { get; set; }
        public abstract class gPSCoordinates : PX.Data.BQL.BqlString.Field<gPSCoordinates> { }
        #endregion

        #region Longitiude / Latitude

        [PXDBDecimal(9, MaxValue = 90f, MinValue = -90f)]
        [PXUIField(DisplayName = "Latitude")]
        public decimal? Latitude
        {
            get;
            set;
        }

        [PXDBDecimal(9, MaxValue = 180f, MinValue = -180f)]
        [PXUIField(DisplayName = "Longitude")]
        public decimal? Longitude
        {
            get;
            set;
        }
        #endregion

        #region MainContact
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Main Contact")]
        public virtual string MainContact { get; set; }
        public abstract class mainContact : PX.Data.BQL.BqlString.Field<mainContact> { }
        #endregion

        #region PhoneNo
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Phone No")]
        public virtual string PhoneNo { get; set; }
        public abstract class phoneNo : PX.Data.BQL.BqlString.Field<phoneNo> { }
        #endregion

        #region SinglePhaseConnections
        [PXDBInt()]
        [PXUIField(DisplayName = "Single Phase Connections")]
        public virtual int? SinglePhaseConnections { get; set; }
        public abstract class singlePhaseConnections : PX.Data.BQL.BqlInt.Field<singlePhaseConnections> { }
        #endregion

        #region 30A Connections
        [PXDBInt()]
        [PXUIField(DisplayName = "30A  Connections")]
        public virtual int? ThirtyAConnections { get; set; }
        public abstract class thirtyAConnections : PX.Data.BQL.BqlInt.Field<thirtyAConnections> { }
        #endregion

        #region 60A Connections
        [PXDBInt()]
        [PXUIField(DisplayName = "60A  Connections")]
        public virtual int? SixtyAConnections { get; set; }
        public abstract class sixtyAConnections : PX.Data.BQL.BqlInt.Field<sixtyAConnections> { }
        #endregion

        #region ContractDemand
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract Demand")]
        public virtual string ContractDemand { get; set; }
        public abstract class contractDemand : PX.Data.BQL.BqlString.Field<contractDemand> { }
        #endregion

        #region FeederLength
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Feeder Length")]
        public virtual Decimal? FeederLength { get; set; }
        public abstract class feederLength : PX.Data.BQL.BqlDecimal.Field<feederLength> { }
        #endregion

        #region TransformerLocation
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Location")]
        public virtual string TransformerLocation { get; set; }
        public abstract class transformerLocation : PX.Data.BQL.BqlString.Field<transformerLocation> { }
        #endregion

        #region TransformerCapacity
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Capacity")]
        public virtual Decimal? TransformerCapacity { get; set; }
        public abstract class transformerCapacity : PX.Data.BQL.BqlDecimal.Field<transformerCapacity> { }
        #endregion

        #region TransformerDistance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Distance")]
        public virtual Decimal? TransformerDistance { get; set; }
        public abstract class transformerDistance : PX.Data.BQL.BqlDecimal.Field<transformerDistance> { }
        #endregion

        #region TransformerDistanceRange
        [PXDBString()]
        [PXUIField(DisplayName = "Distance Range")]
        [PXSelector(typeof(Search<PX.CS.CSAttributeDetail.description, Where<PX.Objects.CS.CSAttributeDetail.attributeID, Equal<SolarSite.attributeDistRange>>>))]

        public virtual String TransformerDistanceRange { get; set; }

        public abstract class transformerDistanceRange : PX.Data.BQL.BqlString.Field<transformerDistanceRange> { }
        #endregion

        #region Nearest3PhasePointDist
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Nearest 3 Phase Point")]
        public virtual Decimal? Nearest3PhasePointDist { get; set; }
        public abstract class nearest3PhasePointDist : PX.Data.BQL.BqlDecimal.Field<nearest3PhasePointDist> { }
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

        #region Attributes
        public abstract class attributes : BqlAttributes.Field<attributes> { }

        /// <summary>The entity attributes.</summary>
        [CRAttributesField(typeof(classID))]
        public virtual string[] Attributes { get; set; }

        #region ClassID

        public abstract class classID : PX.Data.BQL.BqlString.Field<classID> { }
        /// <summary>The class ID for the attributes.</summary>
        /// <value>Always returns <see cref="GroupTypes.SolarSiteSurvey" />.</value>
        [PXString(20)]
        public virtual string ClassID
        {
            get { return GroupTypes.SolarSiteSurvey; }
        }
        #endregion

        #endregion

        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        protected Guid? _NoteID;
        [PXNote(DescriptionField = typeof(SolarSiteSurvey.surveyID))]
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





        #region Approver
        public abstract class approver : PX.Data.BQL.BqlInt.Field<approver> { }
        protected int? _Approver;
        [PX.TM.Owner(typeof(SolarSiteSurvey.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIField(DisplayName = "Approver", Visibility = PXUIVisibility.Visible)]

        public virtual int? Approver
        { get; set; }
        #endregion

        #region ApproveUser
        public abstract class approveUser : PX.Data.BQL.BqlString.Field<approveUser> { }
        [PXDBString(255)]
        [PXUIField(DisplayName = "Approver")]
        [PXSelector(typeof(Users.username), SubstituteKey = typeof(Users.fullName))]
        public virtual string ApproveUser
        {
            get;
            set;
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
        [PX.TM.Owner(typeof(SolarSiteSurvey.workgroupID), Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? OwnerID
        { get; set; }
        #endregion


        [PXInt]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Approval Workgroup ID", Enabled = false)]
        public virtual int? ApprovalWorkgroupID
        {
            get;
            set;
        }

        [Owner(IsDBField = false, DisplayName = "Approver", Enabled = false)]
        public virtual int? ApprovalOwnerID
        {
            get;
            set;
        }

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

        #region IAssign Members

        int? PX.Data.EP.IAssign.WorkgroupID
        {
            get { return WorkgroupID; }
            set { WorkgroupID = value; }
        }

        int? PX.Data.EP.IAssign.OwnerID
        {
            get { return OwnerID; }
            set { OwnerID = value; }
        }


        #endregion
        #region IAddressLocation Fields


        public string AddressLine1
        {
            get
            {
                return SiteAddress;
            }
            set
            {
                SiteAddress = value;
            }
        }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }



        [PXDBLocalizableString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Site Address")]
        public string SiteAddress
        {
            get;
            set;
        }

        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "City")]
        public string City
        {
            get;
            set;
        }

        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Country")]
        [Country]
        public string CountryID
        {
            get;
            set;
        }

        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "State")]
        [State(typeof(countryId))]
        public string State
        {
            get;
            set;
        }

        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Postal Code")]
        [PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask), typeof(countryId))]
        public string PostalCode
        {
            get;
            set;
        }

        public abstract class siteAddress : BqlString.Field<siteAddress>
        {
        }

        public abstract class city : BqlString.Field<city>
        {
        }

        public abstract class countryId : BqlString.Field<countryId>
        {
        }

        public abstract class state : BqlString.Field<state>
        {
        }

        public abstract class postalCode : BqlString.Field<postalCode>
        {
        }

        #endregion

    }
        
}