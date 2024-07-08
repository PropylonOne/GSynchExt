using System;
using PX.Data;

namespace GSynchExt
{
  public class SolarSiteInverterDataMaint : PXGraph<SolarSiteInverterDataMaint>
  {
        public PXSave<SolarSiteInverterData> Save;
        public PXCancel<SolarSiteInverterData> Cancel;
        [PXImport]
        public PXSelect<SolarSiteInverterData> MasterView;     
  }
}