using GSynchExt;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.BQL;
using PX.Objects.AP;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.CN.Common.Helpers;
using PX.Objects.CN.ProjectAccounting.PM.Descriptor;
using PX.Objects.CT;
using PX.Objects.FA;
using PX.Objects.GL;
using PX.Objects.PJ.DailyFieldReports.PJ.Graphs;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using PX.Objects.PJ.Submittals.PJ.Graphs;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static GSynchExt.APInvoiceEntryGSExt;
using static PX.Data.PXAccess;
using static PX.Data.PXGenericInqGrph;
using static PX.Objects.CS.TermsDueType;
using static PX.Objects.CT.ContractAction;
using static PX.Objects.IN.InventoryItem;
using System.Threading.Tasks;
using static PX.SM.EMailAccount;

namespace PX.Objects.PM
{
    public class ProjectEntryGSExt : PXGraphExtension<ProjectEntry>
    {
        #region DAC Attributes Override
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(BudgetLevels.Detail)]
        protected virtual void _(Events.CacheAttached<PMProject.costBudgetLevel> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PMCompletedPctMethod.ByQuantity)]
        protected virtual void _(Events.CacheAttached<PMTask.completedPctMethod> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(ProjectTaskType.Cost)]
        protected virtual void _(Events.CacheAttached<PMTask.type> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(true)]
        protected virtual void _(Events.CacheAttached<PMCostBudget.isProduction> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PMProductivityTrackingType.Template)]
        protected virtual void _(Events.CacheAttached<PMCostBudget.productivityTracking> e) { }
        #region UsrPredecessorTaskCD
        [PXDBString(30)]
        [PXUIField(DisplayName = "Predecessor Task")]
        //Validate flase to enable copy function. Value is checked again at updating
        [PXSelector(typeof(Search<
            PMTask.taskCD,
            Where<PMTask.projectID, Equal<Current<PMTask.projectID>>,
                Or<PMTask.taskCD, Contains<PMTaskGSExt.usrPredecessorTaskCD>>>>), typeof(PMTask.taskCD), typeof(PMTaskGSExt.usrPredecessorTaskCD), ValidateValue = false)]
        protected virtual void _(Events.CacheAttached<PMTaskGSExt.usrPredecessorTaskCD> e)
        {
        }
        #endregion

        #region OwnerID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Site Engineer")]
        protected virtual void _(Events.CacheAttached<PMProject.ownerID> e) { }
        #endregion
        #endregion

        #region Views

        [PXHidden]
        public PXSetup<ProjectManagementSetup> PMSetup;

        public PXFilter<InvoDialogInfo> Dialog;

        #endregion
        #region Event Handlers
        protected virtual void _(Events.RowSelected<PMTask> e)
        {
            PMTask doc = (PMTask)e.Row;
            if (doc == null) return;

            PXUIFieldAttribute.SetEnabled<PMTaskGSExt.usrPredecessorTaskCD>(e.Cache, doc, doc.ProjectID > 0 && doc.Status == ProjectTaskStatus.Planned);
        }
        protected virtual void _(Events.RowPersisting<PMCostBudget> e)
        {
            var row = e.Row;
            if (row == null) return;
            var accntGrp = GSProjectHelper.GetTimelineDefaultAccntGrp(this.Base);
            string error = GSProjectHelper.ValidateBudgetForTimelineTask(e.Row, accntGrp?.GroupID);
            if (error != null)
            {
                var task = PMTask.PK.Find(this.Base, e.Row.ProjectID, e.Row.ProjectTaskID);
                var proj = PMProject.PK.Find(this.Base, e.Row.ProjectID);
                e.Cache.RaiseExceptionHandling<PMBudget.amount>(e.Row, e.Row.Amount, new PXSetPropertyException(error, PXErrorLevel.Error, accntGrp.GroupCD, proj?.ContractCD, task?.TaskCD));
            }
        }
        protected virtual void _(Events.FieldVerifying<PMTask, PMTask.status> e)
        {
            var row = e.Row;
            if (row == null) return;
            if (e.NewValue == null || row == null || this.Base.Project?.Current == null) return;
            bool validateOK = true;
            ValidateTaskStatusUpdate(this.Base.Project.Current, row, e.NewValue.ToString(), e.OldValue.ToString(), out validateOK);
            if (!validateOK)
            {
                e.Cancel = true;
            }
            else
            {
                if (e.OldValue.ToString() == ProjectTaskStatus.Completed && e.NewValue.ToString() != e.OldValue.ToString())
                {
                    if (Base.Tasks.Ask(GSynchExt.Messages.ConfirmStatusReverse, MessageButtons.YesNo) != WebDialogResult.Yes)
                    {
                        e.NewValue = null;
                        e.Cancel = true;
                    }
                    else
                    {
                        row.EndDate = null;
                    }
                }
            }
        }
        protected virtual void _(Events.FieldVerifying<PMTask, PMTaskGSExt.usrPredecessorTaskCD> e)
        {
            var row = e.Row as PMTask;
            if (row == null) return;

            /// UsrPredecessorTaskCD is used since the task IDs are not generated at the time of new entry.
            if (e.NewValue == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();

            PMTask prTask = new PMTask();
            prTask = PMTask.UK.Find(this.Base, row.ProjectID, e.NewValue.ToString());
            if (prTask == null && row.ProjectID > 0) // Raise error only the project is saved.
            {
                throw new PXSetPropertyException(GSynchExt.Messages.PredecessorNoExist, rowExt.UsrPredecessorTaskCD);
            }
            if (prTask == null) return;
            ///Rest of the validation should be done if prTask is not null
            DateTime? preTaskDate = null;
            DateTime? sucTaskkDate = null;
            if (!GSProjectHelper.ValidatePredecessorTask(prTask, row, false, out preTaskDate, out sucTaskkDate))
            {
                e.Cancel = true;
                throw new PXSetPropertyException<PMTaskGSExt.usrPredecessorTaskCD>(GSynchExt.Messages.InvalidPredecessor, preTaskDate, sucTaskkDate, row.TaskCD);
            }
            if (prTask?.TaskID > 0)
            {
                e.Cache.SetValue<PMTaskGSExt.usrPredecessorTaskID>(rowExt, prTask?.TaskID);
            }
        }

        /// <summary>
        /// internal case begins
        /// </summary>
        /// <param name="e"></param>

        protected virtual void _(Events.RowDeleting<PMProject> e)
        {
            var row = e.Row;
            if (row == null) return;

            var task = PXSelect<
                PMTask, 
                Where<PMTask.projectID, Equal<Required<PMTask.projectID>>>>
                .Select(this.Base, row.ContractID);
            if (task == null) return;


            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();

            foreach (PMTask tasks in task)
            {
                var tasksExt = tasks.GetExtension<PMTaskGSExt>();
                tasksExt.UsrPredecessorTaskCD = null;
                this.Base.Actions.PressSave();
            }
            var projSite = row.ContractCD;   
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, projSite);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.ProjectID = null;
                ssgraph.Site.Update(ssRec);

                SolarSiteSurvey survey = PXSelect<
                    SolarSiteSurvey, 
                    Where<SolarSiteSurvey.solarSiteID, Equal<Required<SolarSiteSurvey.solarSiteID>>>>
                    .Select(this.Base, ssRec.SolarSiteID);
                if(survey == null)
                {
                    ssRec.SiteStatus = Status.Planned;
                }
                if (survey != null)
                {
                    ssRec.SiteStatus = Status.UnderSurvey;
                }
                ssgraph.Actions.PressSave();
            }

        }
        protected virtual void _(Events.RowPersisted<PMTask> e)
        {
            var row = e.Row;
            if (row == null) return;
            /// Locking the budget
            SiteSetup sitePref = PXSelect<SiteSetup>.Select(this.Base);
            if (sitePref == null) return;
            if (e.TranStatus == PXTranStatus.Completed)
            {
                var task = row.TaskCD.Remove(0, 3) ?? "XXX";
                task = task.TrimEnd();
                if (task == sitePref.MapConsStart && row.Status == ProjectTaskStatus.Completed)
                {
                    if (this.Base.Project.Current?.BudgetFinalized != true || this.Base.Project.Current?.BudgetFinalized == null)
                    {
                        // this.Base.lockBudget.Press();

                        this.Base.Project.Current.BudgetFinalized = true;

                    }
                }
            }
        }
        protected virtual void _(Events.FieldUpdated<PMTask, PMTask.status> e)
        {
            var row = e.Row;
            if (row == null) return;
            SiteSetup sitePref = PXSelect<SiteSetup>.Select(this.Base);
            if (sitePref == null) return;
            string oldval = e.OldValue.ToString();
            string newval = e.NewValue.ToString();
            bool validateOK = true;
            ValidateTaskStatusUpdate(this.Base.Project.Current, row, newval, oldval, out validateOK);
            if (validateOK)
            {
                try
                {
                    GSProjectHelper.ProcessTaskStatusUpdate(oldval, newval, sitePref, row, this.Base);
                }
                catch (Exception ex)
                {
                    throw new PXException(ex.Message);
                }
                /// Get template notification id
                if (newval == ProjectTaskStatus.Completed)
                {
                    Notification templ = PXSelect<
                        Notification,
                        Where<Notification.screenID, Equal<Required<Notification.screenID>>,
                            And<Notification.subject, Contains<Required<Notification.subject>>>>>
                        .Select(this.Base, "PM302000", "Completion of Project Task")
                        .FirstOrDefault();
                    if (templ != null) GSProjectHelper.AddEmailActivity(row, (int)templ.NotificationID);
                    else throw new PXSetPropertyException(GSynchExt.Messages.NoEmailSent, PXErrorLevel.Warning);
                }
            }
        }


        protected virtual void _(Events.FieldUpdated<Contract, ContractGSExt.usrAreaEngineer> e)
        {
            e.Cache.SetDefaultExt<ContractGSExt.usrAreaEngApprover>(e.Row);
        }
         
        protected virtual void _(Events.FieldDefaulting<Contract, ContractGSExt.usrAreaEngApprover> e)
        {
            PX.Objects.CR.Standalone.EPEmployee owner = PXSelect<
                PX.Objects.CR.Standalone.EPEmployee, 
                Where<PX.Objects.CR.Standalone.EPEmployee.defContactID, Equal<Current<ContractGSExt.usrAreaEngineer>>>>
                .Select(this.Base);
            if (owner != null)
            {
                e.NewValue = owner.BAccountID;
            }
        }

        protected virtual void _(Events.FieldUpdated<PMProject, PMProject.status> e)
        {
            var row = e.Row;
            if (row == null) return;
            var newStatus = e.NewValue.ToString();
            if (newStatus == ProjectStatus.Completed)
            {
                GSProjectHelper.UpdateSolarSiteProjectEndDate(row.ContractCD, DateTime.Today);
            }
        }
        protected virtual void _(Events.FieldUpdated<Contract, ContractGSExt.usrEPCVendorID> e)
        {
            Contract row = e.Row as Contract;
            if (e.NewValue == null || row == null) return;
            GSProjectHelper.UpdateSolarSiteEPCVendor(row.ContractCD, (int)e.NewValue);
        }
        protected virtual void _(Events.FieldUpdating<PMTask, PMTaskGSExt.usrPredecessorTaskCD> e)
        {
            PMTask row = e.Row as PMTask;
            if (e.NewValue == null || row == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();
            if (e.NewValue != e.OldValue)
            {
                //Recalculate dates
                var preTask = PMTask.UK.Find(this.Base, row.ProjectID, e.NewValue.ToString());
                if (preTask != null)
                {
                    ProjectManagementSetup pmSetup = (ProjectManagementSetup)PXSelect<ProjectManagementSetup>.Select(this.Base);
                    var scrTask = (PMTask)this.Base.Tasks.Select().Where(x => x.Record.ProjectID == row.ProjectID && x.Record.TaskCD == preTask.TaskCD).FirstOrDefault();
                    rowExt.UsrPredecessorTaskID = scrTask?.TaskID;
                    rowExt.UsrPredecessorTaskCD = scrTask?.TaskCD;

                    if(scrTask?.EndDate != null || scrTask?.PlannedEndDate != null)
                    {
                        var planStart = DateTimeHelper.CalculateBusinessDate((DateTime)(scrTask?.EndDate ?? scrTask?.PlannedEndDate), 1, pmSetup.CalendarId);

                        if (planStart != null)
                        {
                            row.PlannedStartDate = planStart;
                            row.PlannedEndDate = DateTimeHelper.CalculateBusinessDate((DateTime)row.PlannedStartDate, rowExt.UsrLeadDays ?? 0, pmSetup.CalendarId);
                        }
                    }                  
                }
            }
        }
        protected virtual void _(Events.RowDeleted<PMTask> e)
        {
            var row = e.Row;
            if (row == null) return;
            var rowExt = row.GetExtension<PMTaskGSExt>();
            var preTaskID = row.TaskID;
            var IsExists = false;

            var task = PXSelect<
                PMTask, 
                Where<PMTask.projectID, Equal<Required<PMTask.projectID>>>>
                .Select(this.Base, row.ProjectID);
            if (task == null) return;

            foreach (PMTask tasks in task)
            {
                var tasksExt = tasks.GetExtension<PMTaskGSExt>();
                if (tasksExt.UsrPredecessorTaskCD != null)
                {
                    if (tasksExt.UsrPredecessorTaskCD.TrimEnd() == row.TaskCD.TrimEnd())
                    {
                        IsExists = true;
                    }
                    if (IsExists)
                    {
                        throw new PXException(GSynchExt.Messages.CannotDeleteTask);
                    }
                }
            }
        }

        #endregion
        #region Actions

        public PXAction<PMCostBudget> CreateAPBillDialog;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Create AP Bill",
            Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable createAPBillDialog(PXAdapter adapter)
        {
            PMCostBudget row = this.Base.CostBudget.Current;
            if (row == null) return adapter.Get();

            PMTask task = PXSelect<
                PMTask, 
                Where<PMTask.taskID,
                                        Equal<Required<PMTask.taskID>>>>
                .Select(this.Base, row.TaskID);

            if(task.Status != ProjectTaskStatus.Active || row.AccountGroupID != 17 )
            {
                throw new PXException(GSynchExt.Messages.CreateAPBillError);
            }

            WebDialogResult dialogResult = Dialog.AskExt(setStateFilter, true);
            return adapter.Get();
        }

        private void setStateFilter(PXGraph aGraph, string ViewName)
        {
            if (ViewName == "Dialog") createAPBill.SetEnabled(true);
        }

         public PXAction<PMCostBudget> createAPBill;
        [PXButton]
        [PXUIField(DisplayName = "Create Bill", Enabled = true)]
        protected virtual IEnumerable CreateAPBill(PXAdapter adapter)
        {
            PMCostBudget row = this.Base.CostBudget.Current;
            PXCache cacheCB = this.Base.CostBudget.Cache;
            InvoDialogInfo info = Dialog.Current;
            PXCache cacheAttr = Dialog.Cache;

            if (row == null) return adapter.Get();

            PMCostBudget budget = (PMCostBudget)this.Base.CostBudget.Current;
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<APInvoiceEntry>().GetExtension<APInvoiceEntryGSExt>().CreateAPBillFromCostBudget(budget, info, redirect: true));
            }

            return adapter.Get();

        }


        public PXAction<PMTask> UpdatePlanDates;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Recalculate Plan Dates", Enabled = true)]
        protected virtual IEnumerable updatePlanDates(PXAdapter adapter)
        {
            PMTask tasks = this.Base.Tasks.Current;
            if (tasks == null) return adapter.Get();
            DateTime? prevEndDate = null;
            ProjectManagementSetup pmSetup = (ProjectManagementSetup)PXSelect<ProjectManagementSetup>.Select(this.Base);
            PXLongOperation.StartOperation(this, delegate ()
            {
                foreach (PMTask rec in this.Base.Tasks.Select())
                {
                    PMTaskGSExt recExt = rec.GetExtension<PMTaskGSExt>();
                    if (rec.Status == ProjectTaskStatus.Planned || rec.Status == ProjectTaskStatus.Active)
                    {
                        if (recExt != null && recExt.UsrPredecessorTaskCD != null)
                        {
                            ///Check against the predecessor tasks end date
                            var preTask = PMTask.UK.Find(this.Base, rec.ProjectID, recExt.UsrPredecessorTaskCD);
                            if (preTask != null)
                            { 
                                recExt.UsrPredecessorTaskID = preTask.TaskID;
                                var planStart = DateTimeHelper.CalculateBusinessDate((DateTime)(preTask.EndDate ?? preTask.PlannedEndDate), 1, pmSetup.CalendarId);
                                rec.PlannedStartDate = planStart;
                            }
                        }
                        else
                        {
                            //Update Planned start and end dates based on the lead days.
                            if (prevEndDate == null)
                            {
                                var proj = this.Base.Project.Current;
                                rec.PlannedStartDate = proj?.StartDate ?? this.Base.Accessinfo.BusinessDate;
                            }
                            else
                            {
                                rec.PlannedStartDate = DateTimeHelper.CalculateBusinessDate((DateTime)prevEndDate, 1, pmSetup.CalendarId);
                            }
                        }
                        rec.PlannedEndDate = DateTimeHelper.CalculateBusinessDate((DateTime)rec.PlannedStartDate, recExt.UsrLeadDays ?? 0, pmSetup.CalendarId);
                        prevEndDate = rec.PlannedEndDate;
                    }
                }
            });
            return adapter.Get();
        }
        public PXAction<PMProject> CreateSubmittal;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Submittal", Enabled = true)]
        protected virtual IEnumerable createSubmittal(PXAdapter adapter)
        {
            PMCostBudget budget = (PMCostBudget)this.Base.CostBudget.Current;
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<SubmittalEntry>().GetExtension<SubmittalEntryGSExt>().CreateSubmittalFromCostBudget(budget, redirect: true));
            }

            return adapter.Get();
        }

        public PXAction<PMProject> CreateDFR;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Daily Field Report", Enabled = true)]
        protected virtual IEnumerable createDFR(PXAdapter adapter)
        {
            PMProject proj = (PMProject)this.Base.Project.Current;
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<DailyFieldReportEntry>().GetExtension<DailyFieldReportEntryGSExt>().CreateDFRFromProject(proj, redirect: true));
            }

            return adapter.Get();
        }


        public PXAction<PMProject> CreateMaterialRequest;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Material Request", Enabled = true)]
        protected virtual IEnumerable createMaterialRequest(PXAdapter adapter)
        {
            PMProject project = (PMProject)this.Base.Project.Current;
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<MaterialTransferRequestEntry>().CreateMTRequestFromProject(project, redirect: true));
            }

            return adapter.Get();
        }

        public PXAction<PMCostBudget> CreateSubmittal2;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Submittal", Enabled = true)]
        protected virtual IEnumerable createSubmittal2(PXAdapter adapter)
        {
            PMCostBudget budget = (PMCostBudget)this.Base.CostBudget.Current;
            {
                PXLongOperation.StartOperation(this, () => PXGraph.CreateInstance<SubmittalEntry>().GetExtension<SubmittalEntryGSExt>().CreateSubmittalFromCostBudget(budget, redirect: true));
            }

            return adapter.Get();
        }
        #endregion
        #region Overrided methods

        public delegate IEnumerable ActivateDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Activate(PXAdapter adapter, ActivateDelegate BaseMethod)
        {
            BaseMethod.Invoke(adapter);
            var solarRec = (SolarSite)PXSelect<
                SolarSite,
                Where<SolarSite.solarSiteCD, Equal<Required<SolarSite.solarSiteCD>>,
                    And<SolarSite.projectID, Equal<Required<SolarSite.projectID>>>>>
                .Select(this.Base, this.Base.Project.Current.ContractCD, this.Base.Project.Current.ContractID)
                .FirstOrDefault();
            if (solarRec != null)
            {
                GSProjectHelper.UpdateSolarSiteStartDate(solarRec.SolarSiteCD, (DateTime)this.Base.Accessinfo.BusinessDate);
            }
            return adapter.Get();
        }
        public delegate IEnumerable CompleteDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Complete(PXAdapter adapter, CompleteDelegate BaseMethod)
        {
            BaseMethod.Invoke(adapter);
            var solarRec = (SolarSite)PXSelect<
                SolarSite,
                Where<SolarSite.solarSiteCD, Equal<Required<SolarSite.solarSiteCD>>,
                    And<SolarSite.projectID, Equal<Required<SolarSite.projectID>>>>>
                .Select(this.Base, this.Base.Project.Current.ContractCD, this.Base.Project.Current.ContractID)
                .FirstOrDefault();
            if (solarRec != null)
            {
                GSProjectHelper.UpdateSolarSiteProjectEndDate(solarRec.SolarSiteCD, (DateTime)this.Base.Accessinfo.BusinessDate);

            }
            return adapter.Get();
        }
        [PXOverride]
        public virtual void OnDefaultFromTemplateTasksInserted(PMProject prj, PMProject templ, Dictionary<int, int> taskMap, Action<PMProject, PMProject, Dictionary<int, int>> method)
        {
            if (prj == null) return;
            if (templ == null) return;
            var sub = new Sub();
            sub = GSProjectHelper.GetSubaccount(this.Base, prj);
            if (sub?.SubCD == null)
            {
                sub = GSProjectHelper.CreateSubaccount(this.Base, prj);
            }
            var prjTasks = PXSelect<
                PMTask, 
                Where<PMTask.projectID, Equal<Current<PMProject.contractID>>>>
                .Select(this.Base);
            ProjectManagementSetup pmSetup = (ProjectManagementSetup)PXSelect<ProjectManagementSetup>.Select(this.Base);
            PMTask updatedTsk = null;

            foreach (PMTask res in prjTasks)
            {
                PMTask dst = (PMTask)res;
                var scr = (PMTask)PXSelect<
                    PMTask, 
                    Where<PMTask.taskCD, Equal<Required<PMTask.taskCD>>,
                        And<PMTask.projectID, Equal<Required<PMTask.projectID>>>>>
                    .Select(this.Base, dst.TaskCD, templ.ContractID);

                if (scr.AutoIncludeInPrj == true)
                {
                    updatedTsk = UpdateTaskExt(scr, dst, prj, updatedTsk?.PlannedEndDate, pmSetup.CalendarId, sub);
                }
            }
        }

        #endregion
        #region New Methods


        protected virtual void ValidateTaskStatusUpdate(PMProject proj, PMTask task, string newStatus, string oldStatus, out bool validateOK)
        {
            validateOK = true;
            var error = GSProjectHelper.ValidateTaskStatusUpdate(this.Base, proj, task, newStatus, oldStatus);
            if (error != null)
            {
                validateOK = false;
                throw new PXSetPropertyException(error);
            }
        }

        public PMTask UpdateTaskExt(PMTask scr, PMTask dst, PMProject proj, DateTime? prevEndDate, string cal, Sub sub)
        {
            if (dst == null) return dst;

            PMTaskGSExt scrExt = scr.GetExtension<PMTaskGSExt>();

            //Update Planned start and end dates based on the lead days.
            if (prevEndDate == null)
            {
                var solarRec = SolarSite.UK.Find(this.Base, proj.ContractCD);
                dst.PlannedStartDate = solarRec?.ProjPlannedStartDate ?? proj.StartDate ?? this.Base.Accessinfo.BusinessDate;
            }
            else
            {
                dst.PlannedStartDate = DateTimeHelper.CalculateBusinessDate((DateTime)prevEndDate, 1, cal);
            }
            this.Base.Tasks.Update(dst);
            PMTaskGSExt dstExt = PXCache<PMTask>.GetExtension<PM.PMTaskGSExt>(dst);
            if (dstExt != null)
            {
                ///Check against the predecessor tasks end date
                if (scrExt.UsrPredecessorTaskCD != null)
                {
                    var preTask = PMTask.UK.Find(this.Base, scr.ProjectID, scrExt.UsrPredecessorTaskCD);
                    if (preTask != null)
                    {
                        var scrTask = (PMTask)this.Base.Tasks.Select().Where(x => x.Record.ProjectID == dst.ProjectID && x.Record.TaskCD == preTask.TaskCD).FirstOrDefault();
                        dstExt.UsrPredecessorTaskID = scrTask?.TaskID;
                        dstExt.UsrPredecessorTaskCD = scrTask?.TaskCD;
                        var planStart = DateTimeHelper.CalculateBusinessDate((DateTime)(scrTask?.EndDate ?? scrTask?.PlannedEndDate), 1, cal);
                        if (planStart != null)
                        {
                            /*if (planStart.Date >= dst.PlannedStartDate)
                            {
                                dst.PlannedStartDate = planStart;
                            }*/
                            dst.PlannedStartDate = planStart;
                        }
                    }
                }
                //Update subaccounts
                dst.DefaultAccrualSubID = sub?.SubID ?? dst.DefaultAccrualSubID;
                dst.DefaultExpenseSubID = sub?.SubID ?? dst.DefaultExpenseSubID;

                dst.PlannedEndDate = DateTimeHelper.CalculateBusinessDate((DateTime)dst.PlannedStartDate, scrExt.UsrLeadDays ?? 0, cal);
                dstExt.UsrLeadDays = scrExt.UsrLeadDays;
                dstExt.UsrNotifier = scrExt.UsrNotifier;
                dstExt.UsrOwnerID = scrExt.UsrOwnerID;
                dstExt.UsrNotifyWorkgroup = scrExt.UsrNotifyWorkgroup;
                dstExt.UsrPredecessorTaskCD = scrExt.UsrPredecessorTaskCD;
                dstExt.UsrIsComplDocReq = scrExt.UsrIsComplDocReq;
            }
            //Update or Insert Caches
            dst = (PMTask)this.Base.Caches[typeof(PMTask)].Update(dst);
            dstExt = (PM.PMTaskGSExt)this.Base.Caches[typeof(PM.PMTaskGSExt)].Update(dstExt);
            return dst;
        }
        #endregion
    }
}


