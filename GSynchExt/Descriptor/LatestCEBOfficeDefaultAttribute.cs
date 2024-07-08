
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Compliance.PM.CacheExtensions;
using PX.Objects.PM;
using PX.Objects.PR.Standalone;
using System;
using static PX.Objects.TX.CSTaxCalcType;

namespace GSynchExt
{
    public class LatestCEBOfficeDefaultAttribute : PXUnboundDefaultAttribute
    {
        private System.Type _SolarSiteIDField;

        public LatestCEBOfficeDefaultAttribute(System.Type solarsiteIDfield)
        {
            _SolarSiteIDField = solarsiteIDfield;
        }

        public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            base.FieldDefaulting(sender, e);

            int? solarsiteID = sender.GetValue(e.Row, _SolarSiteIDField.Name) as int?;
            SolarSiteSurvey solarSiteSurveys = PXSelect<SolarSiteSurvey, Where<SolarSiteSurvey.solarSiteID,
                           Equal<Required<SolarSiteSurvey.solarSiteID>>, And<SolarSiteSurvey.siteStatus,
                           Equal<GSynchExt.Status.completed>>>, OrderBy<Desc<SolarSiteSurvey.surveyID>>>.SelectWindowed(sender.Graph, 0, 1, solarsiteID);


            if (solarSiteSurveys == null)
            {
                return;
            }

            e.NewValue = solarSiteSurveys.CEBOffice;

        }
    }
}