using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GSynchExt;
using PX.Api;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.EP;
using PX.Objects.PJ.DailyFieldReports.Descriptor;
using PX.Objects.RQ;
using static GSynchExt.SolarSite;
using Events = PX.Data.Events;
using PX.Data.BQL.Fluent;
namespace GSynchExt
{
    public class DistrictsEntry : PXGraph<DistrictsEntry>
    {
        public PXFilter<Provinces> MasterView;

        public PXCancelClose<Provinces> CancelClose;
        public PXSaveClose<Provinces> SaveClose;
        public PXCancel<Provinces> Cancel;
        public PXInsert<Provinces> Insert;
        public PXDelete<Provinces> Delete;

        public PXSelect<Districts, Where<Districts.stateID, Equal<Current<Provinces.stateID>>>> DetailsView;
        public PXSelect<Cluster, Where<Cluster.stateID, Equal<Current<Provinces.stateID>>>> ClusterView;
        public PXSelect<Phase, Where<Phase.stateID, Equal<Current<Provinces.stateID>>>> PhaseVieww;
        

        public PXSelect<CEBLocations, Where<CEBLocations.stateID,
                Equal<Current<Provinces.stateID>>>> LocCEBView;

        public PXSelect<LECOLocations, Where<LECOLocations.stateID,
                Equal<Current<Provinces.stateID>>>> LocLECOView;

        public PXSelect<CEBLocations, Where<CEBLocations.stateID,
               Equal<Current<Provinces.stateID>>>> LocView;

        protected virtual void _(Events.RowDeleting<Phase> e)
        {
            var row = e.Row;
            if(row == null ) return;
            SolarSite solarSite = PXSelect<SolarSite, Where<SolarSite.phaseID, Equal<Required<SolarSite.phaseID>>, And<SolarSite.province, Equal<Required<SolarSite.province>>>>>.Select(this, row.PhaseID, row.StateID);
            if(solarSite != null)
            {
                throw new PXException(Messages.CannotDeletePhase);
            }
        }
        // Step 1: Disable UI editing when the phase is used in SolarSite
        protected virtual void _(Events.RowSelected<Phase> e)
        {
            var row = e.Row;
            if (row == null) return;

            // Check if this Phase is linked to a SolarSite
            SolarSite solarSite = PXSelect<SolarSite,
                                    Where<SolarSite.phaseID, Equal<Required<SolarSite.phaseID>>,
                                    And<SolarSite.province, Equal<Required<SolarSite.province>>>>>.
                                    Select(this, row.PhaseID, row.StateID);

            // Disable the PhaseID field in the UI if it's associated with SolarSite
            bool isPhaseUsedInSolarSite = solarSite != null ;
            if(row.EstimatedCost != null)
            {
                PXUIFieldAttribute.SetEnabled<Phase.estimatedCost>(e.Cache, row, !isPhaseUsedInSolarSite);
            }
            PXUIFieldAttribute.SetEnabled<Phase.phaseID>(e.Cache, row, !isPhaseUsedInSolarSite);
            PXUIFieldAttribute.SetEnabled<Phase.description>(e.Cache, row, !isPhaseUsedInSolarSite);
        }

        



    }

}
