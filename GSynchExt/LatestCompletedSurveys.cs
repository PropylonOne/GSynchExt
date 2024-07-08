using System;
using PX.Data;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("LatestCompletedSurveys")]
  public class LatestCompletedSurveys : PXBqlTable, IBqlTable
  {
    #region SolarSiteID
    [PXDBInt(IsKey =true)]
    [PXUIField(DisplayName = "Solar Site ID")]
    [PXSelector(typeof(Search<SolarSite.solarSiteID>), typeof(SolarSite.siteName), 
            typeof(SolarSite.siteStatus), typeof(SolarSite.province), typeof(SolarSite.phaseID), 
            SubstituteKey = typeof(SolarSite.solarSiteCD))]
        public virtual int? SolarSiteID { get; set; }
    public abstract class solarSiteID : PX.Data.BQL.BqlInt.Field<solarSiteID> { }
    #endregion

    #region SurveyID
    [PXDBString(30, IsUnicode = true, InputMask = "", IsKey =true)]
    [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<SolarSiteSurvey.surveyID>))]
        public virtual string SurveyID { get; set; }
    public abstract class surveyID : PX.Data.BQL.BqlString.Field<surveyID> { }
    #endregion

    #region CEBOffice
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "CEBOffice")]
    [PXSelector(typeof(Search<
            CEBLocations.locationID>), SubstituteKey = typeof(CEBLocations.description))]
        public virtual string CEBOffice { get; set; }
    public abstract class cEBOffice : PX.Data.BQL.BqlString.Field<cEBOffice> { }
    #endregion

    #region Lecooffice
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "LECO Office")]
    [PXSelector(typeof(Search<
            LECOLocations.locationID>), SubstituteKey = typeof(LECOLocations.description))]
        public virtual string Lecooffice { get; set; }
    public abstract class lecooffice : PX.Data.BQL.BqlString.Field<lecooffice> { }
    #endregion

    #region SiteStatus
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Site Status")]
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
        public virtual string SiteStatus { get; set; }
    public abstract class siteStatus : PX.Data.BQL.BqlString.Field<siteStatus> { }
    #endregion
  }
}