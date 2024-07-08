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

    }
}