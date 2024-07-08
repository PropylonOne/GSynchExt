using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("SolarSiteSurveyBuildings")]
  public class SolarSiteSurveyBuildings : PXBqlTable, IBqlTable
  {
    public static class FK
    {
      public class MasterRec : SolarSiteSurvey.UK.ForeignKeyOf<SolarSiteSurveyBuildings>.By<surveyID> { }
    }

    #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(SolarSiteSurvey.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

    #region SurveyID
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey ID")]
        [PXDBDefault(typeof(SolarSiteSurvey.surveyID), DefaultForUpdate = false)]
        [PXParent(typeof(FK.MasterRec))]

        public virtual string SurveyID { get; set; }
        public abstract class surveyID : PX.Data.BQL.BqlString.Field<surveyID> { }
    #endregion

    #region SiteID
   [PXDBInt]
   [PXDefault(typeof(SolarSiteSurvey.solarSiteID))]
    public virtual int? SiteID{ get; set; }
    public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
    #endregion

    #region Building
    [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Building")]
    public virtual string Building { get; set; }
    public abstract class building : PX.Data.BQL.BqlString.Field<building> { }
    #endregion

    #region Length
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Length")]
    public virtual Decimal? Length { get; set; }
    public abstract class length : PX.Data.BQL.BqlDecimal.Field<length> { }
    #endregion

    #region Height
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Height")]
    public virtual Decimal? Height { get; set; }
    public abstract class height : PX.Data.BQL.BqlDecimal.Field<height> { }
    #endregion

    #region Width
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Width")]
    public virtual Decimal? Width { get; set; }
    public abstract class width : PX.Data.BQL.BqlDecimal.Field<width> { }
    #endregion

    #region Stories
    [PXDBInt()]
    [PXUIField(DisplayName = "Stories")]
    public virtual int? Stories { get; set; }
    public abstract class stories : PX.Data.BQL.BqlInt.Field<stories> { }
    #endregion

    #region RoofMaterial
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Roof Material")]
        //   [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<SolarSite.attributeMaterial>>>), typeof(CSAttributeDetail.description), SubstituteKey = (typeof(CSAttributeDetail.description)))]
        [GSynchExt.Descriptor.SSSurveyListAttributes.SSSurveyrRoofMaterial]

        public virtual string RoofMaterial { get; set; }
    public abstract class roofMaterial : PX.Data.BQL.BqlString.Field<roofMaterial> { }
    #endregion

    #region BuildingConditions
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Building Conditions")]
        //  [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<SolarSite.attributeBuilCondition>>>), typeof(CSAttributeDetail.description), SubstituteKey = (typeof(CSAttributeDetail.description)))]
    [GSynchExt.Descriptor.SSSurveyListAttributes.SSSurveyBuildingCondition]

        public virtual string BuildingConditions { get; set; }
    public abstract class buildingConditions : PX.Data.BQL.BqlString.Field<buildingConditions> { }
    #endregion
       
    #region RoofCondition
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Roof Condition")]
        //   [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<SolarSite.attributeRoofCondition>>>), SubstituteKey = (typeof(CSAttributeDetail.description)))]
        //    [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<SolarSite.attributeRoofCondition>>>), typeof(CSAttributeDetail.description), SubstituteKey = (typeof(CSAttributeDetail.description)))]
        [GSynchExt.Descriptor.SSSurveyListAttributes.SSSurveyrRoofCondition]

        public virtual string RoofCondition { get; set; }
    public abstract class roofCondition : PX.Data.BQL.BqlString.Field<roofCondition> { }
        #endregion

    #region RepairPercentage
    [PXDBString(30, IsUnicode = true, InputMask = "")]

    [PXUIField(DisplayName = "Repair Percentage")]
        //   [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<SolarSite.attributeRepPercent>>>), typeof(CSAttributeDetail.description), SubstituteKey = (typeof(CSAttributeDetail.description)))]
        [GSynchExt.Descriptor.SSSurveyListAttributes.SSSurveyrRepairPercentage]

        public virtual string RepairPercentage { get; set; }
    public abstract class repairPercentage : PX.Data.BQL.BqlString.Field<repairPercentage> { }
    #endregion

    #region Orientation
    [PXDBString(10, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Orientation")]
        // [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<SolarSite.attributeOrientation>>>), typeof(CSAttributeDetail.description), SubstituteKey = (typeof(CSAttributeDetail.description)))]
    [GSynchExt.Descriptor.SSSurveyListAttributes.SSSurveyrOrientation]

        public virtual string Orientation { get; set; }
    public abstract class orientation : PX.Data.BQL.BqlString.Field<orientation> { }
    #endregion

    #region Pitch
    [PXDBInt()]
    [PXUIField(DisplayName = "Pitch")]
    public virtual int? Pitch { get; set; }
    public abstract class pitch : PX.Data.BQL.BqlInt.Field<pitch> { }
    #endregion

    #region Shading
    [PXDBBool()]
    [PXUIField(DisplayName = "Shading")]
    public virtual bool? Shading { get; set; }
    public abstract class shading : PX.Data.BQL.BqlBool.Field<shading> { }
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

    public class attributeBuildingCond : PX.Data.BQL.BqlString.Constant<attributeBuildingCond> { public attributeBuildingCond() : base("SURBUICOND") { } }

  }
}