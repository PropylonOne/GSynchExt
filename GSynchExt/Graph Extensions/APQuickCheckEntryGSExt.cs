using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.PJ.Common.DAC;
using PX.Objects.PJ.Submittals.PJ.DAC;
using PX.Objects.CR;
using System;
using System.Collections;
using System.Linq;
using StatusDefinition = PX.Objects.PJ.Submittals.PJ.DAC.PJSubmittal.status;
using System.Collections.Generic;
using PX.Objects.Common.Labels;
using PX.Objects;
using GSynchExt;
using PX.Objects.RQ;
using PX.Objects.PM;
using static GSynchExt.GSBOQMaint;
using PX.Objects.CS;
using PX.Objects.CA;
using PX.Objects.AP.Standalone;

namespace PX.Objects.AP {
    public class APQuickCheckEntryGSExt : PXGraphExtension<APQuickCheckEntry>
    {
        [PXImport]
        public PXSelect<APTran, Where<APTran.tranType, Equal<Current<APQuickCheck.docType>>, And<APTran.refNbr, Equal<Current<APQuickCheck.refNbr>>>>> Transactions;

    }
}