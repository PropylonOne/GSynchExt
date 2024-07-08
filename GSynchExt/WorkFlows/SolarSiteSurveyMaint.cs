using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.EP;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;
using PX.Objects.PM;
using PX.Objects.RQ;
using PX.Objects.CR;
using PX.Objects.CS;
using static PX.SM.PXEmailSyncOperation;
using System.Collections.Generic;
using PX.Objects.IN.Overrides.INDocumentRelease;
using PX.Objects.GL;

namespace GSynchExt
{
    public class SolarSiteSurveyMaint : PXGraph<SolarSiteSurveyMaint, SolarSiteSurvey>
    {
        [PXViewName("Solar Site Survey")]
        public SelectFrom<SolarSiteSurvey>.View Survey;
        public PXSelect<SolarSiteSurvey, Where<SolarSiteSurvey.surveyID, Equal<Current<SolarSiteSurvey.surveyID>>>> CurrentDocument;
        public PXSelect<SolarSiteSurveyBuildings, Where<SolarSiteSurveyBuildings.surveyID, Equal<Current<SolarSiteSurvey.surveyID>>>> Buildings;
        public PXSelect<SolarSite, Where<SolarSite.solarSiteID, Equal<Current<SolarSiteSurvey.solarSiteID>>>> CurrentSite;
        public PXSetup<SiteSetup> AutoNumSetup;

        public PXSelect<SiteSetup> Setup;
        public PXSelect<SiteSetupApproval> SetupApproval;
        [PXViewName("Approval Details")]
        [PXCopyPasteHiddenView]
        public EPApprovalAutomation<SolarSiteSurvey, SolarSiteSurvey.surveyApproved, SolarSiteSurvey.rejected, SolarSiteSurvey.hold, SiteSetupApproval> Approval;


        [PXViewName("Answers")]
        public CRAttributeList<SolarSiteSurvey> Answers;

        #region Constructor
        public SolarSiteSurveyMaint()
        {
            SiteSetup setup = AutoNumSetup.Current;
        }
        #endregion

        public virtual SolarSiteSurvey CreateEmptySurveyFrom(SolarSite site)
        {

            SolarSiteSurvey survey = this.Survey.Insert();
            survey.CreatedByScreenID = "PM770024";
            survey.SolarSiteID = site.SolarSiteID;

            return survey;

        }
        public virtual SolarSiteSurvey CreateSurveyFrom(SolarSite site, bool redirect = false)
        {

            CreateEmptySurveyFrom(site);
            if (this.Survey.Cache.IsDirty)
            {
                if (redirect)
                    throw new PXRedirectRequiredException(this, "");
                else
                    return Survey.Current;
            }

            throw new PXException("");
        }


        /*
                public PXAction<SolarSiteSurvey> Archive;
                [PXButton(), PXUIField(DisplayName = "Archive",
                MapEnableRights = PXCacheRights.Select,
                MapViewRights = PXCacheRights.Select)]
                protected virtual IEnumerable archive(PXAdapter adapter)
                {
                    return adapter.Get();
                }

                public PXAction<SolarSiteSurvey> RemoveHold;
                [PXButton(), PXUIField(DisplayName = "Remove Hold",
                MapEnableRights = PXCacheRights.Select,
                MapViewRights = PXCacheRights.Select)]
                protected virtual IEnumerable removeHold(PXAdapter adapter)
                {
                    return adapter.Get();
                }
        */
        public PXAction<SolarSiteSurvey> SubmitSurvey;
        [PXButton(), PXUIField(DisplayName = "Submit Survey ",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable submitSurvey(PXAdapter adapter)
        {
            if (ValidateAction(this.CurrentDocument.Current))
            {
                if (CurrentDocument.Current != null)
                {
                    CurrentDocument.Current.Hold = false;
                    CurrentDocument.Update(CurrentDocument.Current);
                }
                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
          

        } 

        


        public PXAction<SolarSiteSurvey> Cancel2;
        [PXButton(), PXUIField(DisplayName = "Cancel",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable cancel2(PXAdapter adapter)
        {
            return adapter.Get();
        }


        public PXAction<SolarSiteSurvey> Hold2;
        [PXButton(), PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable hold2(PXAdapter adapter)
        {
            if (CurrentDocument.Current != null)
            {
                CurrentDocument.Current.Hold = true;
                CurrentDocument.Update(CurrentDocument.Current);
            }
            return adapter.Get();
        }


        public PXAction<SolarSiteSurvey> Approve;
        [PXButton(), PXUIField(DisplayName = "Approve",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable approve(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<SolarSiteSurvey> Reject;
        [PXButton(), PXUIField(DisplayName = "Reject",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable reject(PXAdapter adapter)
        {
            return adapter.Get();
        }

        #region EventHandlers

        public PXAction<SolarSiteSurvey> ViewAddressOnMap;
        [PXUIField(DisplayName = PX.Objects.CR.Messages.ViewOnMap)]
        [PXButton]
        public virtual void viewAddressOnMap()
        {
            var crntSurvey = CurrentDocument.Current;
            new MapService(this).viewAddressOnMap(crntSurvey);
        }

        protected virtual void _(Events.FieldUpdated<SolarSiteSurvey, SolarSiteSurvey.distributionUtility> e)
        {
            var row = e.Row as SolarSiteSurvey;
            if (row == null) return;
            var isCEB = ((row.DistributionUtility == GSynchExt.DUtility.CEB) || (row.DistributionUtility == null));
            var isLECO = ((row.DistributionUtility == GSynchExt.DUtility.LECO) || (row.DistributionUtility == null));


            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.cEBOffice>(e.Cache, e.Row, isCEB);
            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.lecooffice>(e.Cache, e.Row, isLECO);
        }
        protected virtual void _(Events.FieldVerifying<SolarSiteSurvey, SolarSiteSurvey.solarSiteID> e)

        {

            var row = e.Row as SolarSiteSurvey;

            if (row == null) return;
            SolarSite rec = SolarSite.PK.Find(this,(int)e.NewValue);

            if (rec == null) return;

            if (rec.SiteStatus == Status.Planned && row.CreatedByScreenID != "PM770024" && row.CreatedByScreenID != null)
            {
                e.NewValue = null;
                throw new PXSetPropertyException(Messages.InitialSurveyError);
            }
        }

        protected virtual void _(Events.RowSelected<SolarSiteSurvey> e)
        {
            var row = e.Row as SolarSiteSurvey;
            if (row == null) return;

            bool hasDetails = (row.CEBOffice != null || row.Lecooffice != null || row.Nearest3PhasePointDist != null
                                || row.PhoneNo != null || row.MainContact != null || row.Latitude != null || row.Longitude != null
                                || row.SixtyAConnections != null || row.ThirtyAConnections != null || row.FeederLength != null || row.ContractDemand != null);
            var isCEB = ((row.DistributionUtility == GSynchExt.DUtility.CEB) || (row.DistributionUtility == null));
            if (isCEB)
            {
                row.Lecooffice = null;
            }
            var isLECO = ((row.DistributionUtility == GSynchExt.DUtility.LECO) || (row.DistributionUtility == null));
            if (isLECO)
            {
                row.CEBOffice = null;
            }
            e.Cache.AllowInsert = true;
            e.Cache.AllowDelete = row.SiteStatus == GSynchExt.Status.OnHold;

            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.solarSiteID>(e.Cache, e.Row, (row.SiteStatus == GSynchExt.Status.OnHold && !(hasDetails)) || row.SolarSiteID == null);
            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.cEBOffice>(e.Cache, e.Row, isCEB);
            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.lecooffice>(e.Cache, e.Row, isLECO);
            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.approveUser>(e.Cache, e.Row, row.SurveyApproved != true);
            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.approver>(e.Cache, e.Row, row.SurveyApproved != true);

        }
        #endregion


        public PXAction<SolarSiteSurvey> Assign;
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
        [PXUIField(DisplayName = "Assign", Visible = false)]
        public virtual IEnumerable assign(PXAdapter adapter)
        {
            foreach (SolarSiteSurvey req in adapter.Get<SolarSiteSurvey>())
            {
                if (Setup.Current.SiteApprovalMap != null)
                {
                    var processor = new EPAssignmentProcessor<SolarSiteSurvey>();
                    processor.Assign(req, SetupApproval.Current.AssignmentMapID);
                    req.WorkgroupID = req.ApprovalWorkgroupID;
                    req.OwnerID = req.ApprovalOwnerID;           

                }
                yield return req;
            }
        }

        #region EPApproval Cache Attached - Approvals Fields
        [PXDBDate()]
        [PXDefault(typeof(SolarSiteSurvey.startDate), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.docDate> e)

        {

        }
        [PXDBString(60, IsUnicode = true)]
        [PXDefault(typeof(SolarSiteSurvey.description), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.descr> e)

        {

        }

        #endregion
        #region Methods
        protected virtual bool ValidateAction(SolarSiteSurvey row)
        {
            if (row == null) return true;
            bool noErrors = true;

            if (row.SolarSiteID == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.solarSiteID>(row, row.PhaseID, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.solarSiteID>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            if (row.Description == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.description>(row, row.Description, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.description>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            //if (row.OwnerID == null)
            //{
            //    this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.ownerID>(row, row.OwnerID, null);
            //    PXUIFieldAttribute.SetError<SolarSiteSurvey.ownerID>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
            //    noErrors = false;
            //}
            if (row.DistributionUtility == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.distributionUtility>(row, row.DistributionUtility, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.distributionUtility>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            if (row.TransformerCapacity == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.transformerCapacity>(row, row.TransformerCapacity, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.transformerCapacity>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            if (row.PhoneNo == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.phoneNo>(row, row.PhoneNo, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.phoneNo>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            if (row.MainContact == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.mainContact>(row, row.MainContact, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.mainContact>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            if (row.ApproveUser == null)
            {
                this.CurrentDocument.Cache.RaiseExceptionHandling<SolarSiteSurvey.approveUser>(row, row.MainContact, null);
                PXUIFieldAttribute.SetError<SolarSiteSurvey.approveUser>(CurrentDocument.Cache, row, GSynchExt.Messages.MissingFieldGen);
                noErrors = false;
            }
            return noErrors;
        }








        #endregion


    }
}