using System;
using PX.Data;

namespace GSynchExt
{
  public class RRSetup : PXGraph<RRSetup>
  {
        public PXSave<RRRates> save;
        public PXCancel<RRRates> cancel;
        public PXSelect<RRRates> rrSetup;
  }
}