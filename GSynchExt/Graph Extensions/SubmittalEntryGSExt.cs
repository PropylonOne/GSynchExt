using PX.Objects.PJ.Common.Actions;
using PX.Objects.PJ.Common.Descriptor;
using PX.Objects.PJ.ProjectManagement.Descriptor;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using PX.Objects.PJ.Submittals.PJ.Descriptor;
using PX.Objects.PJ.Submittals.PJ.Graphs;
using PX.Objects.PJ.Submittals.PJ.Services;
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

namespace PX.Objects.PJ.Submittals.PJ.Graphs
{
    public class SubmittalEntryGSExt : PXGraphExtension<PX.Objects.PJ.Submittals.PJ.Graphs.SubmittalEntry>
    {
        public virtual PJSubmittal CreateSubmittal(PMCostBudget budget)
        {
            PJSubmittal submRec = this.Base.Submittals.Insert();
            if (budget == null) return submRec;
            submRec.ProjectId = budget.ProjectID;
            submRec.ProjectTaskId = budget.TaskID;
            submRec.CostCodeID = budget.CostCodeID;
            submRec.Summary = String.Concat(budget.Description.Trim(), ":");
            submRec = this.Base.Submittals.Insert(submRec);
            return submRec;
        }

        public virtual PJSubmittal CreateSubmittalFromCostBudget(PMCostBudget budget, bool redirect = false)
        {

            CreateSubmittal(budget);

            if (this.Base.CurrentSubmittal.Cache.IsDirty)
            {
                if (redirect)
                    throw new PXRedirectRequiredException(this.Base, "");
                else
                    return this.Base.CurrentSubmittal.Current;
            }

            throw new PXException("");
        }

    }
}