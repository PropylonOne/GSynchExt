using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.FS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.Extensions.MultiCurrency.CR;
using PX.Objects.CR;
using System.Collections.Generic;
using PX.Objects.GDPR;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using static PX.Objects.FS.FSAppointment.FK;
using static PX.SM.PXEmailSyncOperation;
using Branch = PX.Objects.GL.Branch;


/// <summary>
/// Solar sites and its life-cycle management
/// MASS Actions:
/// 1. Activate Survey
/// 2. Select Site
/// 3. Create FA
/// 4. Create Service Location
/// </summary>
namespace GSynchExt
{
    public class SolarSiteEntry : PXGraph<SolarSiteEntry, SolarSite>
    {

        public const string SubType = "SUBACCOUNT";

        #region Views

        [PXViewName("Solar Site")]
        public SelectFrom<SolarSite>.View Sites;
        public SelectFrom<SolarSite>.Where<SolarSite.solarSiteID.IsEqual<SolarSite.solarSiteID.FromCurrent>>.View Site;
        [PXImport(typeof(SolarSite))]
        [PXCopyPasteHiddenView]
        public PXSelect<SolarSiteSurvey,
        Where<SolarSiteSurvey.solarSiteID, Equal<Current<SolarSite.solarSiteID>>>> SiteSurvey;
        public PXSetup<SiteSetup> SiteSetup;
        [PXViewName("Answers")]
        public CRAttributeList<SolarSite> Answers;
        public PXFilter<PhaseInfo> PhaseDialog;
        public PXFilter<SiteInfo> SiteDialog;
        #endregion

        // public PXChangeID<SolarSite, SolarSite.solarSiteCD> ChangeID;

        #region Constructor
        public SolarSiteEntry()
        {
            if (SiteSetup.Current == null)
            {
                throw new PXException(Messages.SetupNotConfigured);
            }
            SiteSetup siteSetup = this.SiteSetup.Current;
            FieldDefaulting.AddHandler<SolarSite.siteType>((sender, e) => { if (e.Row != null) e.NewValue = GSynchExt.Type.School; });
        }

        #endregion

        #region Partial Classes
        public partial class AttrView : PXBqlTable, IBqlTable
        {

            #region Branch Location data (Service Location)

            #region DfltSiteID
            public abstract class dfltSiteID : PX.Data.BQL.BqlInt.Field<dfltSiteID> { }

            [Site(DisplayName = "Default Warehouse", DescriptionField = typeof(INSite.descr))]
            public virtual int? DfltSiteID { get; set; }
            #endregion

            #endregion

            #region Non - Stock Item Data

            #region SubID

            [SubAccount(DisplayName = "Sub Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
            public virtual Int32? SubID { get; set; }
            public abstract class subID : BqlInt.Field<subID> { }
            #endregion


            #region ItemClassID

            /// <summary>
            /// The identifier of the <see cref="INItemClass">Item Class</see>, to which the Inventory Item belongs.
            /// Item Classes provide default settings for items, which belong to them, and are used to group items.
            /// </summary>
            /// <value>
            /// Corresponds to the <see cref="INItemClass.ItemClassID"/> field.
            /// </value>
            [PXDBInt]
            [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
            [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
            [PXDefault(typeof(SiteSetup.itemClassID))]
            [PXUIRequired(typeof(INItemClass.stkItem))]
            public virtual int? ItemClassID { get; set; }
            public abstract class itemClassID : BqlInt.Field<itemClassID> { }
            #endregion

            #endregion

            #region FA Data

            #region ClassID
            public abstract class fAClassID : PX.Data.BQL.BqlInt.Field<fAClassID> { }
            /// <summary>
            /// A reference to the fixed asset class.
            /// </summary>
            /// <value>An integer identifier of the fixed asset class. It is a required value.</value>
            [PXDBInt]
            [PXRestrictor(typeof(Where<FAClass.active, Equal<True>>), "", typeof(FAClass.assetCD))]
            [PXSelector(typeof(Search<FAClass.assetID, Where<FAClass.recordType, Equal<FARecordType.classType>>>),
                typeof(FAClass.assetCD),
                typeof(FAClass.assetTypeID),
                typeof(FAClass.description),
                typeof(FAClass.usefulLife),
                SubstituteKey = typeof(FAClass.assetCD),
                DescriptionField = typeof(FAClass.description),
                CacheGlobal = true)]
            [PXUIField(DisplayName = "Asset Class", Visibility = PXUIVisibility.Visible, TabOrder = 3)]
            public virtual int? FAClassID { get; set; }
            #endregion

            #region Department

            public abstract class department : PX.Data.BQL.BqlString.Field<department> { }
            /// <summary>
            /// The department of the fixed asset.
            /// A reference to <see cref="EPDepartment"/>.
            /// Changing this field leads to the creation of a revision of the asset location.
            /// </summary>
            /// <value>
            /// An integer identifier of the department. 
            /// This is a required field. 
            /// By default, the value is set to the custodian department.
            /// </value>
            [PXDBString(10, IsUnicode = true)]
            [PXDefault(typeof(Search<EPEmployee.departmentID, Where<EPEmployee.bAccountID, Equal<Current<FALocationHistory.employeeID>>>>))]
            [PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
            [PXUIField(DisplayName = "Department")]
            public virtual String Department { get; set; }
            #endregion

            #region FASubID

            [SubAccount(DisplayName = "Sub Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
            public virtual Int32? FASubID { get; set; }
            public abstract class fASubID : BqlInt.Field<fASubID> { }
            #endregion

            #endregion

        }
        #endregion

        #region Actions

        public PXInitializeState<SolarSite> initializeState;


        public PXAction<SolarSite> updatePhaseIDAction;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Update Phase ID",
            Enabled = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable UpdatePhaseIDAction(PXAdapter adapter)
        {
            WebDialogResult dialogResult = PhaseDialog.AskExt(setStateFilter, true);
            return adapter.Get();
        }




        private void setStateFilter(PXGraph aGraph, string ViewName)
        {
            if (ViewName == "PhaseDialog") UpdatePhaseID.SetEnabled(true);
        }

        public PXAction<SolarSite> UpdatePhaseID;
        [PXButton(), PXUIField(DisplayName = "Update Phase ID", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable updatePhaseID(PXAdapter adapter)
        {
            PXCache cacheSite = Site.Cache;
            SolarSite row = Site.Current;

            PhaseInfo dialog = PhaseDialog.Current;
            PXCache dialogCache = PhaseDialog.Cache;

            if (row == null) return adapter.Get();

            PXLongOperation.StartOperation(this, delegate ()
            {
                var solarSite = this.Site.Current;
                solarSite.PhaseID = dialog.PhaseID;
                this.Site.Update(this.Site.Current);
                this.Actions.PressSave();

                var subGraph = PXGraph.CreateInstance<SubAccountMaint>();
                Sub sub = PXSelect<Sub, Where<Sub.subCD, Contains<Required<Sub.subCD>>>>.Select(subGraph, solarSite.SolarSiteCD);
                subGraph.SubRecords.Current = Sub.UK.Find(subGraph, sub.SubCD);
                var subRec = subGraph.SubRecords.Current;
                if (subRec != null)
                {
                    if (subRec.SubCD == solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD)
                    {
                        return;

                    }
                    else
                    {
                        using (PXTransactionScope ts = new PXTransactionScope())
                        {
                            string subCD = solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD;
                            /// To Change a CD value directly from database
                            PXDatabase.Update<Sub>(new PXDataFieldAssign<Sub.subCD>(subCD), new PXDataFieldRestrict<Sub.subID>(PXDbType.Int, sub.SubID));
                            ts.Complete();
                        }
                    }

                }
            });
            return adapter.Get();
        }
        public PXAction<SolarSite> UpdateSolarSiteIDAction;
        [PXButton(), PXUIField(DisplayName = "Change Solar Site ID", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable updateSolarSiteIDAction(PXAdapter adapter)
        {

            WebDialogResult dialogResult = SiteDialog.AskExt(setStateFilterForSiteID, true);
            return adapter.Get();
        }
        private void setStateFilterForSiteID(PXGraph aGraph, string ViewName)
        {
            if (ViewName == "SiteDialog") UpdateSolarSiteID.SetEnabled(true);
        }

        public PXAction<SolarSite> UpdateSolarSiteID;
        [PXButton(), PXUIField(DisplayName = "Change ID", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable updateSolarSiteID(PXAdapter adapter)
        {
            PXCache cacheSite = Site.Cache;
            SolarSite row = Site.Current;

            SiteInfo dialog = SiteDialog.Current;

            PXCache dialogCache = SiteDialog.Cache;
            var projectEntry = PXGraph.CreateInstance<ProjectEntry>();
            var subGraph = PXGraph.CreateInstance<SubAccountMaint>();
            SolarSite sSite = PXSelect<SolarSite, Where<SolarSite.solarSiteCD, Equal<Required<SolarSite.solarSiteCD>>>>.Select(subGraph, row.SolarSiteCD);
            Sub sub = PXSelect<Sub, Where<Sub.subCD, Contains<Required<Sub.subCD>>>>.Select(subGraph, row.SolarSiteCD);

            if (sSite.SiteStatus == Status.SiteSelected)
            {
                if (Site.Ask(Messages.OKtoChangeProjectID, MessageButtons.OKCancel) != WebDialogResult.OK) return adapter.Get();
            }

            if (row == null) return adapter.Get();
            PXLongOperation.StartOperation(this, delegate ()
            {
                var solarSite = this.Site.Current.SolarSiteCD;

                var solarSiteCD = dialog.SiteID;

                foreach (SolarSite site in PXSelect<SolarSite, Where<SolarSite.solarSiteCD, Equal<Required<SolarSite.solarSiteCD>>>>.Select(subGraph, row.SolarSiteCD))
                {
                    PXDatabase.Update<SolarSite>(new PXDataFieldAssign<SolarSite.solarSiteCD>(solarSiteCD), new PXDataFieldRestrict<SolarSite.solarSiteID>(PXDbType.Int, sSite.SolarSiteID));

                    foreach (Contract proj in PXSelect<Contract, Where<Contract.contractCD, Equal<Required<Contract.contractCD>>>>.Select(projectEntry, solarSite))
                    {
                        PXDatabase.Update<Contract>(new PXDataFieldAssign<Contract.contractCD>(solarSiteCD), new PXDataFieldRestrict<Contract.contractID>(PXDbType.Int, proj.ContractID));
                    }

                }
                if (sub != null)
                {
                    subGraph.SubRecords.Current = Sub.UK.Find(subGraph, sub.SubCD);
                    var subRec = subGraph.SubRecords.Current;

                    if (subRec.SubCD == row.Province + row.PhaseID + solarSiteCD)
                    {
                        return;

                    }
                    else
                    {
                        using (PXTransactionScope ts = new PXTransactionScope())
                        {
                            SegmentValue val3 = PXSelect<SegmentValue,
                                                Where<SegmentValue.dimensionID, Equal<Required<SegmentValue.dimensionID>>,
                                                And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>,
                                                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(this, SubType, 3, solarSiteCD);
                            var segGraph = PXGraph.CreateInstance<SegmentMaint>();
                            if (val3 == null)
                            {
                                SegmentValue seg = segGraph.Values.Insert(new SegmentValue
                                {
                                    DimensionID = SubType,
                                    SegmentID = 3,
                                    Value = solarSiteCD,
                                    Descr = row.SiteName
                                });
                                segGraph.Actions.PressSave();
                            }

                            string subCD = row.Province + row.PhaseID + solarSiteCD;
                            /// To Change a CD value directly from database
                            PXDatabase.Update<Sub>(new PXDataFieldAssign<Sub.subCD>(subCD), new PXDataFieldRestrict<Sub.subID>(PXDbType.Int, sub.SubID));
                            ts.Complete();
                        }
                    }
                }

            });
            return adapter.Get();
        }



        public PXAction<SolarSite> ReActivate;
        [PXButton(), PXUIField(DisplayName = "Re - Activate", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable reActivate(PXAdapter adapter)
        {
            PXCache cacheSite = Site.Cache;
            SolarSite row = Site.Current;

            if (row.SiteStatus == Status.Suspended)
            {
                if (row.InServiceDate != null)
                {
                    row.SiteStatus = Status.InService;

                }
                else if (row.ConnectedtoGridDate != null)
                {
                    row.SiteStatus = Status.ConnectedToGrid;
                }
                else if (row.CommissionedDate != null)
                {
                    row.SiteStatus = Status.Commissioned;
                }
                else if (row.ConstructionEndDate != null)
                {
                    row.SiteStatus = Status.Constructed;
                }
                else if (row.ConstructionStartDate != null)
                {
                    row.SiteStatus = Status.SiteSelected;
                }
                else if (row.ProjPlannedStartDate != null)
                {

                    SolarSiteSurvey survey = PXSelect<SolarSiteSurvey, Where<SolarSiteSurvey.solarSiteID, Equal<Required<SolarSiteSurvey.solarSiteID>>>>.Select(this, row.SolarSiteID);
                    if (survey == null)
                    {
                        row.SiteStatus = Status.Planned;
                    }
                    if (survey != null)
                    {
                        row.SiteStatus = Status.UnderSurvey;
                    }

                }
            }
            if (row.SiteStatus == Status.Cancelled)
            {
                SolarSiteSurvey survey = PXSelect<SolarSiteSurvey, Where<SolarSiteSurvey.solarSiteID, Equal<Required<SolarSiteSurvey.solarSiteID>>>>.Select(this, row.SolarSiteID);
                if (survey == null)
                {
                    row.SiteStatus = Status.Planned;
                }
                if (survey != null)
                {
                    row.SiteStatus = Status.UnderSurvey;


                }
            }
            this.Site.Update(this.Site.Current);
            this.Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<SolarSite> CreateFAAction;
        [PXButton(), PXUIField(DisplayName = "Create FA", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable createFAAction(PXAdapter adapter)
        {
            CreateFA(this.Site.Current);
            return adapter.Get();

        }


        public PXAction<SolarSite> CreateSLAction;
        [PXButton(), PXUIField(DisplayName = "Create Service Location", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable createSLAction(PXAdapter adapter)
        {
            if (ValidateAction("CreateServiceLocation", this.Site.Current))
            {
                CreateServiceLocation(this.Site.Current);
                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
        }



        public PXAction<SolarSite> CreateInventory;
        [PXButton(), PXUIField(DisplayName = "Create Inventory", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable createInventory(PXAdapter adapter)
        {
            CreateSSNonStockItem(this.Site.Current);
            return adapter.Get();

        }


        public PXAction<SolarSite> ActivateSurvey;
        [PXButton(), PXUIField(DisplayName = "Activate Survey", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable activateSurvey(PXAdapter adapter)
        {
            if (ValidateAction("ActivateSurvey", this.Site.Current))
            {
                CreateSurvey(this.Site.Current);
                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
        }

        public PXAction<SolarSite> SelectSite;
        [PXButton(), PXUIField(DisplayName = "Select Site for Construction", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable selectSite(PXAdapter adapter)
        {
            SolarSite row = Sites.Current;
            PXCache cacheSites = Sites.Cache;

            if (ValidateAction("SelectSite", row))
            {
                CreateProject(this.Site.Current);
                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
        }

        public PXAction<SolarSite> CompleteConstruction;
        [PXButton(), PXUIField(DisplayName = "Complete Construction", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable completeConstruction(PXAdapter adapter)
        {
            if (ValidateAction("CompleteConstruction", this.Site.Current))
            {
                this.Site.Current.ConstructionEndDate = this.Accessinfo.BusinessDate;
                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
        }

        public PXAction<SolarSite> ConnectToGrid;
        [PXButton(), PXUIField(DisplayName = "Connect to Grid", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable connectToGrid(PXAdapter adapter)
        {
            if (ValidateAction("ConnectToGrid", this.Site.Current))
            {
                CreateSSNonStockItem(this.Site.Current);
                this.Site.Current.ConnectedtoGridDate = this.Accessinfo.BusinessDate;

                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
        }
        public PXAction<SolarSite> PlaceInService;
        [PXButton(), PXUIField(DisplayName = "Place In Service", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable placeInService(PXAdapter adapter)
        {
            if (ValidateAction("PlaceInService", this.Site.Current))
            {
                ///TO DO - Set the correct date
                this.Site.Current.ConstructionEndDate = this.Accessinfo.BusinessDate;
                CreateServiceLocation(this.Site.Current);
                return adapter.Get();
            }
            else
            {
                throw new PXException(GSynchExt.Messages.OperationError);
            }
        }
        public PXAction<SolarSite> Suspend;
        [PXButton(), PXUIField(DisplayName = "Suspend", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable suspend(PXAdapter adapter)
        {
            return adapter.Get();

        }
        public PXAction<SolarSite> Commission;
        [PXButton(), PXUIField(DisplayName = "Commission", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable commission(PXAdapter adapter)
        {
            return adapter.Get();

        }


        public PXAction<SolarSite> Cancel2;
        [PXButton(), PXUIField(DisplayName = "Cancel", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable cancel2(PXAdapter adapter)
        {
            this.Site.Current.CancelledDate = this.Accessinfo.BusinessDate;

            return adapter.Get();
        }
        public virtual IEnumerable MenuActions(PXAdapter adapter)
        {
            return adapter.Get();
        }
        public PXAction<SolarSite> UpdateProject;
        [PXButton(), PXUIField(DisplayName = "Update Project", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable updateProject(PXAdapter adapter)
        {
            var proj = PMProject.UK.Find(this, CTPRType.Project, this.Site.Current.SolarSiteCD);
            if (proj == null) return adapter.Get();
            var row = new SolarSite();
            row = (SolarSite)Site.Cache.CreateCopy(Site.Current);
            var projExt = proj.GetExtension<ContractGSExt>();
            row.ProjectID = proj.ContractID;
            row.ProjectManager = proj.OwnerID;
            row.StartDate = proj.ActivationDate;
            if (proj.IsCompleted == true)
                row.EndDate = proj.LastModifiedDateTime;
            row.EPCVendorID = projExt.UsrEPCVendorID;
            row.AreaEngineer = projExt.UsrAreaEngineer;
            Site.Update(row);
            this.Actions.PressSave();
            return adapter.Get();
        }

        #endregion

        #region EventHandlers

        protected virtual void _(Events.FieldUpdated<SolarSite.siteCapacity> e)
        {
            var row = e.Row as SolarSite;
            if(row == null)return;
            if (e.NewValue == null || row.SiteCapacity == 0) return;
            Phase phase = Phase.PK.Find(this, row.Province, row.PhaseID);
            if (phase == null) return;
            row.EstSiteValue = row.SiteCapacity * phase?.EstimatedCost;
            
        }
        protected virtual void _(Events.RowSelected<SolarSite> e)
        {
            SolarSite doc = (SolarSite)e.Row;
            if (doc == null) return;
            bool isPlanned = doc.SiteStatus == GSynchExt.Status.Planned && doc.SolarSiteCD != null;
            bool canEdit = doc.SiteStatus == GSynchExt.Status.Planned || doc.SiteStatus == GSynchExt.Status.UnderSurvey;
            bool isProj = doc.SiteStatus == GSynchExt.Status.SiteSelected || doc.SiteStatus == GSynchExt.Status.Constructed || doc.SiteStatus == GSynchExt.Status.Commissioned || doc.SiteStatus == GSynchExt.Status.ConnectedToGrid || doc.SiteStatus == GSynchExt.Status.InService || doc.SiteStatus == GSynchExt.Status.Completed;
            bool fromSolarSite = doc.CreatedByScreenID == "PM770024";

            e.Cache.AllowInsert = true;
            e.Cache.AllowDelete = isPlanned;

            PXUIFieldAttribute.SetEnabled<SolarSiteSurvey.description>(e.Cache, e.Row, !fromSolarSite);
            PXUIFieldAttribute.SetEnabled<SolarSite.province>(e.Cache, doc, isPlanned);
            PXUIFieldAttribute.SetEnabled<SolarSite.phaseID>(e.Cache, doc, canEdit);
            PXUIFieldAttribute.SetEnabled<SolarSite.clusterID>(e.Cache, doc, canEdit);
            ///Project Data
            PXUIFieldAttribute.SetEnabled<SolarSite.projectManager>(e.Cache, doc, canEdit);
            PXUIFieldAttribute.SetEnabled<SolarSite.projPlannedStartDate>(e.Cache, doc, canEdit);
            PXUIFieldAttribute.SetEnabled<SolarSite.templateID>(e.Cache, doc, canEdit);

            UpdateSolarSiteIDAction.SetVisible(!e.Cache.IsDirty && (canEdit || doc.SiteStatus == GSynchExt.Status.SiteSelected));
            ActivateSurvey.SetEnabled(!e.Cache.IsDirty);
            ActivateSurvey.SetVisible(!e.Cache.IsDirty);
            SelectSite.SetEnabled(doc.ProjectID == null && canEdit && !e.Cache.IsDirty);
            SelectSite.SetVisible(doc.ProjectID == null && canEdit && !e.Cache.IsDirty);
            /// The following statuses are set by the Project Task Completion
            CompleteConstruction.SetVisible(false);
            Commission.SetVisible(false);
            ConnectToGrid.SetVisible(false);
            PlaceInService.SetVisible(false);
            /// The following are allowed from Solar Sites or as Mass Actions
            CreateSLAction.SetVisible(doc.InServiceDate != null && !e.Cache.IsDirty);
            CreateFAAction.SetVisible((doc.InServiceDate != null || doc.ConnectedtoGridDate != null) && !e.Cache.IsDirty);
            updatePhaseIDAction.SetVisible(doc.SiteStatus == GSynchExt.Status.SiteSelected);
            ReActivate.SetVisible(doc.SiteStatus == GSynchExt.Status.Cancelled || doc.SiteStatus == GSynchExt.Status.Suspended);
            UpdateProject.SetVisible(isProj);
            UpdateProject.SetEnabled(isProj);

        }
        protected virtual void _(Events.FieldUpdated<SolarSite, SolarSite.siteStatus> e)
        {
            SolarSite doc = (SolarSite)e.Row;
            if (doc == null) return;
            var newStatus = e.NewValue.ToString();
            if (newStatus == Status.ConnectedToGrid)
            {
                ///Create the Solar Sales Nonstock Item
            }
            /*
             * TO DO - When the services are ready uncomment this block
            if (newStatus == Status.InService)
            {
                ///Create FA
                ///Create Service Location
                CreateServiceLocation(doc);
                ///Create Service Equipments
            }
            */
        }

        public virtual void AddingTasksToProject(SolarSite row, ProjectEntry projectEntry, Dictionary<string, int> taskMap, bool? copyNotes, bool? copyFiles)
        {
            var resultset = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>>>.Select(projectEntry, row.TemplateID);
            foreach (PMTask task in resultset)
            {
                PMTask templateTask = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                    And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(this, row.TemplateID, task.TaskCD);

                PMTask newTask;
                if (templateTask == null) { return; }

                newTask = projectEntry.CopyTask(templateTask, row.ProjectID.GetValueOrDefault(), new ProjectEntry.DefaultFromTemplateSettings() { CopyProperties = true, CopyAttributes = true, CopyEmployees = false, CopyEquipment = false, CopyNotification = false, CopyRecurring = true, CopyNotes = false, CopyFiles = false });
                taskMap.Add(task.TaskCD, newTask.TaskID.GetValueOrDefault());


            }
        }

        #endregion

        #region Methods
        protected virtual bool ValidateAction(String action, SolarSite row)
        {
            if (row == null) return true;
            bool noErrors = true;
            SiteSetup solarPref = this.SiteSetup.Current;
            if (action == "SelectSite")
            {
                var ongoingSurveys = PXSelect<SolarSiteSurvey,
                    Where<SolarSiteSurvey.solarSiteID, Equal<Required<SolarSiteSurvey.solarSiteID>>,
                    And<SolarSiteSurvey.state, NotEqual<Status.completed>,
                    And<SolarSiteSurvey.state, NotEqual<Status.rejected>>>>>.Select(this, row.SolarSiteID);
                if (ongoingSurveys?.Count != 0)
                    throw new PXException(GSynchExt.Messages.HasOngoingSurveys, PXUIFieldAttribute.GetDisplayName(this.Site.Cache, this.Site.Cache.GetField(typeof(SolarSite.templateID))));
                if (row.TemplateID == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.templateID>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
                //<<<<<<< Updated upstream

                if (row.DfltWareHouse == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.dfltWareHouse>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
                /*
                if (row.EstSiteValue == null || row.EstSiteValue == decimal.Zero)
                {
                    PXUIFieldAttribute.SetError<SolarSite.estSiteValue>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                
                if (row.SiteCapacity == null || row.SiteCapacity == decimal.Zero)
                {
                    PXUIFieldAttribute.SetError<SolarSite.siteCapacity>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }}*/
                //=======
                if (row.DfltWareHouse == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.areaEngineer>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
                /*if (row.EstSiteValue == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.estSiteValue>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }*/
                //>>>>>>> Stashed changes
                if (row.ProjectManager == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.projectManager>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
                if (row.ProjPlannedStartDate == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.projPlannedStartDate>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
                if (row.PhaseID == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.phaseID>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
            }
            if (action == "CompleteConstruction")
            {
                if (row.ProjectID == null)
                {
                    PXUIFieldAttribute.SetError<SolarSite.phaseID>(this.Site.Cache, row, GSynchExt.Messages.MissingFieldGen);
                    noErrors = false;
                }
                var projRec = PMProject.PK.Find(this, row.ProjectID);

                if (projRec == null)
                {
                    throw new PXException(GSynchExt.Messages.MissingProject);
                }
                else
                {
                    var defTask = GSProjectHelper.GetDefaultTask(this, row, solarPref?.MapConsEnd);
                    if (defTask?.Status != ProjectTaskStatus.Completed && defTask != null)
                    {
                        throw new PXException(GSynchExt.Messages.InvalidActionTask, "Complete Construction", defTask.TaskCD);
                    }
                    else
                    {
                        if (projRec.Status != ProjectStatus.Active || projRec.Status != ProjectStatus.Completed)
                        {
                            throw new PXException(GSynchExt.Messages.InvalidActionProj, "Complete Construction", projRec.ContractCD);
                        }
                    }
                }
                if (solarPref?.ItemClassID == null)
                {
                    throw new PXException(Messages.CannotCreateNS, PXUIFieldAttribute.GetDisplayName(this.SiteSetup.Cache, this.SiteSetup.Cache.GetField(typeof(SiteSetup.itemClassID))));
                }
            }
            if (action == "ConnectToGrid")
            {
                var projRec = PMProject.PK.Find(this, row.ProjectID);
                if (projRec == null)
                {
                    throw new PXException(GSynchExt.Messages.MissingProject);
                }
                else
                {
                    if (projRec.Status != ProjectStatus.Active)
                    {
                        throw new PXException(GSynchExt.Messages.InvalidActionProj, "Connect to Grid", projRec.ContractCD, projRec.Status);
                    }
                }
                if (row.CEBAccount == null)
                {
                    throw new PXException(Messages.MissingField, PXUIFieldAttribute.GetDisplayName(this.Site.Cache, this.Site.Cache.GetField(typeof(SolarSite.cEBAccount))));
                }
            }
            if (action == "CreateServiceLocation")
            {
                var projRec = PMProject.PK.Find(this, row.ProjectID);
                if (projRec != null)
                {
                    if (projRec.Status != ProjectStatus.Active || projRec.Status != ProjectStatus.Completed)
                    {
                        throw new PXException(GSynchExt.Messages.InvalidActionProj, "Create Service Location", projRec.ContractCD, projRec.Status);
                    }
                }
                if (row.DfltWareHouse == null)
                {
                    throw new PXException(Messages.CannotCreateSL, PXUIFieldAttribute.GetDisplayName(this.SiteSetup.Cache, this.SiteSetup.Cache.GetField(typeof(SolarSite.dfltWareHouse))));
                }
            }
            if (action == "CreateFA")
            {
                var projRec = PMProject.PK.Find(this, row.ProjectID);
                if (projRec != null)
                {
                    if (solarPref?.AssetClassID == null)
                    {
                        throw new PXException(Messages.CannotCreateFA, PXUIFieldAttribute.GetDisplayName(this.SiteSetup.Cache, this.SiteSetup.Cache.GetField(typeof(SiteSetup.assetClassID))));
                    }
                    if (solarPref?.Department == null)
                    {
                        throw new PXException(Messages.CannotCreateFA, PXUIFieldAttribute.GetDisplayName(this.SiteSetup.Cache, this.SiteSetup.Cache.GetField(typeof(SiteSetup.department))));
                    }
                    if (row.DfltWareHouse == null)
                    {
                        throw new PXException(Messages.CannotCreateFA, PXUIFieldAttribute.GetDisplayName(this.SiteSetup.Cache, this.SiteSetup.Cache.GetField(typeof(SolarSite.dfltWareHouse))));
                    }
                }

            }
            return noErrors;
        }
        protected virtual void CreateSurvey(SolarSite row)
        {
            if (row == null) return;
            PXLongOperation.StartOperation(this, delegate ()
            {

                SolarSiteSurveyMaint surGraph = CreateInstance<SolarSiteSurveyMaint>();

                var ssSurvey = surGraph.CurrentDocument.Insert(new SolarSiteSurvey
                {
                    SolarSiteID = row.SolarSiteID,
                    StartDate = this.Accessinfo.BusinessDate,
                    Description = "Solar Site Survey for " + row.SiteName,
                    Province = row.Province,
                    DistributionUtility = "CEB"
                });
                surGraph.Actions.PressSave();
                Site.View.RequestRefresh();
            });
        }

        public string GetLatestCompletedSurvey(SolarSite solarSite)
        {
            /*  var solarSiteSurveys  = PXSelect<SolarSiteSurvey, Where<SolarSiteSurvey.solarSiteID, 
                  Equal<Required<SolarSiteSurvey.solarSiteID>>, And<SolarSiteSurvey.siteStatus, 
                  Equal<GSynchExt.Status.completed>>>>.Select(this, solarSite.SolarSiteID);*/

            var solarSiteSurveys = PXSelect<SolarSiteSurvey, Where<SolarSiteSurvey.solarSiteID,
                Equal<Required<SolarSiteSurvey.solarSiteID>>, And<SolarSiteSurvey.siteStatus,
                Equal<GSynchExt.Status.completed>>>, OrderBy<Asc<SolarSiteSurvey.surveyID>>>.SelectWindowed(this, 0, 1, solarSite.SolarSiteID);

            SolarSiteSurvey surveys = (SolarSiteSurvey)solarSiteSurveys;

            string latestSurvey = surveys.SurveyID;
            return latestSurvey;

            /*
            int latestNbr = 0;
            string latestSurveyID = null;
            foreach (SolarSiteSurvey siteSurvey in solarSiteSurveys)
            {
                string latestCompletedSurvey = surveys.SurveyID.ToString();

              //  SolarSiteSurvey completedSurvey = (SolarSiteSurvey)surveys ;
                var surveyNbr = latestCompletedSurvey[latestCompletedSurvey.Length - 1].ToString();
                int intSurveyNbr = int.Parse(surveyNbr);
                if (intSurveyNbr > latestNbr)
                {
                    latestNbr = intSurveyNbr;
                    latestSurveyID = siteSurvey.SurveyID;
                }
            }
            return latestSurveyID;*/
        }

        protected virtual void CreateProject(SolarSite row)
        {
            if (row == null) return;
            // Acuminator disable once PX1008 LongOperationDelegateSynchronousExecution [Justification]
            PXLongOperation.StartOperation(this, delegate ()
            {
                var projRec = PMProject.UK.Find(this, CTPRType.Project, row.SolarSiteCD);
                var sub = new Sub();
                sub = GSProjectHelper.GetSubaccount(this, row);
                if (sub?.SubCD == null)
                {
                    sub = GSProjectHelper.CreateSubaccount(this, row);
                }

                if (projRec == null)
                {
                    ProjectEntry prjEntryGraph = CreateInstance<ProjectEntry>();
                    prjEntryGraph.SuppressTemplateIDUpdated = true;
                    prjEntryGraph.Clear();
                    var info = prjEntryGraph.GetExtension<ProjectEntry.MultiCurrency>().GetDefaultCurrencyInfo();

                    PXCache cache = prjEntryGraph.Project.Cache;

                    PMProject proj = new PMProject();
                    proj.BaseType = CTPRType.Project;
                    proj.CuryID = prjEntryGraph.Accessinfo.BaseCuryID;
                    proj.CuryInfoID = info?.CuryInfoID;
                    proj.RateTypeID = info?.CuryRateTypeID;
                    if (!DimensionMaint.IsAutonumbered(this, ProjectAttribute.DimensionName))
                        proj.ContractCD = row.SolarSiteCD;
            

                    proj = prjEntryGraph.Project.Insert(proj);
                    
                    /// Update subaccount
                    proj.DefaultSalesSubID = sub?.SubID;
                    proj.DefaultAccrualSubID = sub?.SubID;
                    proj.DefaultExpenseSubID = sub?.SubID;
                    proj.Status = ProjectStatus.Planned;
                    proj.LockCommitments = true;
                    proj.IsCancelled = true;
                    proj = prjEntryGraph.Project.Update(proj);
                    proj.TemplateID = row.TemplateID;
                    prjEntryGraph.DefaultFromTemplate(proj, row.TemplateID, PX.Objects.PM.ProjectEntry.DefaultFromTemplateSettings.Default);

                    EPEmployee contact = PXSelect<EPEmployee, Where<EPEmployee.ownerID,
                                                Equal<Required<EPEmployee.ownerID>>>>.Select(this, row?.ProjectManager);


                    if (contact != null)
                    {
                        proj.ApproverID = contact?.BAccountID;
                    }
                    /// Update subaccount
                    proj.DefaultSalesSubID = sub?.SubID;
                    proj.DefaultAccrualSubID = sub?.SubID;
                    proj.DefaultExpenseSubID = sub?.SubID;
                    proj.OwnerID = row.ProjectManager;
                    proj.Description = row.SiteName + " " + row.Province + " " + row.PhaseID;
                    proj.StartDate = row.ProjPlannedStartDate;

                    var projExt = proj.GetExtension<ContractGSExt>();
                    projExt.UsrAreaEngineer = row.AreaEngineer;
                    prjEntryGraph.Project.Update(proj);
                    prjEntryGraph.Actions.PressSave();
                    row.ProjectID = proj.ContractID;
                    this.Site.Update(row);
                    this.Actions.PressSave();

                }
            });
        }
        public virtual void CreateSSNonStockItem(SolarSite row)
        {
            if (row == null) return;
            PXLongOperation.StartOperation(this, delegate ()
            {
                try
                {
                    Sub subRec = GSProjectHelper.GetSubaccount(this, row);
                    if (subRec == null)
                        subRec = GSProjectHelper.CreateSubaccount(this, row);
                    string ssItem = string.Concat("SS-", row.SolarSiteCD);

                    NonStockItemMaint nsGraph = CreateInstance<NonStockItemMaint>();
                    InventoryItem itemRec = new InventoryItem();
                    itemRec = InventoryItem.UK.Find(this, ssItem);
                    if (itemRec != null)
                    {
                        itemRec.ExpenseAccrualSubID = subRec.SubID;
                        itemRec.ExpenseSubID = subRec.SubID;
                        itemRec.InvtSubID = subRec.SubID;
                        itemRec.LCVarianceSubID = subRec.SubID;
                        itemRec.COGSSubID = subRec.SubID;
                        itemRec.DeferralSubID = subRec.SubID;
                        itemRec.POAccrualSubID = subRec.SubID;
                        itemRec.PPVSubID = subRec.SubID;
                        itemRec.ReasonCodeSubID = subRec.SubID;
                        itemRec.SalesSubID = subRec.SubID;
                        nsGraph.Item.Update(itemRec);
                        nsGraph.Actions.PressSave();
                        /// Add the default price
                        var priceRec = new InventoryItemCurySettings();
                        if (nsGraph.ItemCurySettings.Current != null)
                        {
                            priceRec = nsGraph.ItemCurySettings.Current;
                            priceRec.BasePrice = (decimal?)22;
                            nsGraph.ItemCurySettings.Update(priceRec);
                            nsGraph.Actions.PressSave();
                        }
                        else
                        {
                            priceRec.InventoryID = itemRec.InventoryID;
                            priceRec.CuryID = this.Accessinfo.BaseCuryID;
                            priceRec.BasePrice = (decimal?)22;
                            nsGraph.ItemCurySettings.Insert(priceRec);
                            nsGraph.Actions.PressSave();
                        }



                    }
                    else
                    {
                        itemRec = nsGraph.Item.Insert(new InventoryItem
                        {
                            InventoryCD = ssItem,
                            Descr = "Solar Sales for " + row.SiteName,
                            ItemClassID = this.SiteSetup.Current.ItemClassID,
                            ExpenseAccrualSubID = subRec.SubID,
                            ExpenseSubID = subRec.SubID,
                            InvtSubID = subRec.SubID,
                            LCVarianceSubID = subRec.SubID,
                            COGSSubID = subRec.SubID,
                            DeferralSubID = subRec.SubID,
                            POAccrualSubID = subRec.SubID,
                            PPVSubID = subRec.SubID,
                            ReasonCodeSubID = subRec.SubID,
                            SalesSubID = subRec.SubID
                        });
                        nsGraph.Item.Insert(itemRec);
                        nsGraph.Actions.PressSave();
                        /// Add the default price
                        var priceRec = new InventoryItemCurySettings();
                        if (nsGraph.ItemCurySettings.Current != null)
                        {
                            priceRec = nsGraph.ItemCurySettings.Current;
                            priceRec.BasePrice = (decimal?)22;
                            nsGraph.ItemCurySettings.Update(priceRec);
                            nsGraph.Actions.PressSave();
                        }
                        else
                        {
                            priceRec.InventoryID = itemRec.InventoryID;
                            priceRec.CuryID = this.Accessinfo.BaseCuryID;
                            priceRec.BasePrice = (decimal?)22;
                            nsGraph.ItemCurySettings.Insert(priceRec);
                            nsGraph.Actions.PressSave();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new PXException(e.Message);
                }

            });
        }
        public virtual void CreateSSNonStockItem(SolarSiteEntry graph, SolarSite row)
        {
            if (row == null) return;
            Sub subRec = GSProjectHelper.GetSubaccount(graph, row);
            if (subRec == null)
                subRec = GSProjectHelper.CreateSubaccount(graph, row);
            string ssItem = string.Concat("SS-", row.SolarSiteCD);

            NonStockItemMaint nsGraph = CreateInstance<NonStockItemMaint>();
            InventoryItem itemRec = new InventoryItem();
            itemRec = InventoryItem.UK.Find(graph, ssItem);
            if (itemRec != null)
            {
                itemRec.ExpenseAccrualSubID = subRec.SubID;
                itemRec.ExpenseSubID = subRec.SubID;
                itemRec.InvtSubID = subRec.SubID;
                itemRec.LCVarianceSubID = subRec.SubID;
                itemRec.COGSSubID = subRec.SubID;
                itemRec.DeferralSubID = subRec.SubID;
                itemRec.POAccrualSubID = subRec.SubID;
                itemRec.PPVSubID = subRec.SubID;
                itemRec.ReasonCodeSubID = subRec.SubID;
                itemRec.SalesSubID = subRec.SubID;
                nsGraph.Item.Update(itemRec);
                nsGraph.Actions.PressSave();
                /// Add the default price
                var priceRec = new InventoryItemCurySettings();
                if (nsGraph.ItemCurySettings.Current != null)
                {
                    priceRec = nsGraph.ItemCurySettings.Current;
                    priceRec.BasePrice = (decimal?)22;
                    nsGraph.ItemCurySettings.Update(priceRec);
                    nsGraph.Actions.PressSave();
                }
                else
                {
                    priceRec.InventoryID = itemRec.InventoryID;
                    priceRec.CuryID = graph.Accessinfo.BaseCuryID;
                    priceRec.BasePrice = (decimal?)22;
                    nsGraph.ItemCurySettings.Insert(priceRec);
                    nsGraph.Actions.PressSave();
                }



            }
            else
            {
                itemRec = nsGraph.Item.Insert(new InventoryItem
                {
                    InventoryCD = ssItem,
                    Descr = "Solar Sales for " + row.SiteName,
                    ItemClassID = this.SiteSetup.Current.ItemClassID,
                    ExpenseAccrualSubID = subRec.SubID,
                    ExpenseSubID = subRec.SubID,
                    InvtSubID = subRec.SubID,
                    LCVarianceSubID = subRec.SubID,
                    COGSSubID = subRec.SubID,
                    DeferralSubID = subRec.SubID,
                    POAccrualSubID = subRec.SubID,
                    PPVSubID = subRec.SubID,
                    ReasonCodeSubID = subRec.SubID,
                    SalesSubID = subRec.SubID
                });
                nsGraph.Item.Insert(itemRec);
                nsGraph.Actions.PressSave();
                /// Add the default price
                var priceRec = new InventoryItemCurySettings();
                if (nsGraph.ItemCurySettings.Current != null)
                {
                    priceRec = nsGraph.ItemCurySettings.Current;
                    priceRec.BasePrice = (decimal?)22;
                    nsGraph.ItemCurySettings.Update(priceRec);
                    nsGraph.Actions.PressSave();
                }
                else
                {
                    priceRec.InventoryID = itemRec.InventoryID;
                    priceRec.CuryID = this.Accessinfo.BaseCuryID;
                    priceRec.BasePrice = (decimal?)22;
                    nsGraph.ItemCurySettings.Insert(priceRec);
                    nsGraph.Actions.PressSave();
                }
            }
        }

        //     public PXChangeID<Sub, Sub.subCD> ChangeID;
        public virtual void CreateFA(SolarSite row)
        {
            if (row == null) return;

            PXLongOperation.StartOperation(this, delegate ()
            {
                string ssFA = row.SolarSiteCD.Trim();
                AssetMaint faGraph = CreateInstance<AssetMaint>();
                faGraph.Asset.Current = (FixedAsset)PXSelect<FixedAsset, Where<FixedAsset.assetCD, Equal<Required<FixedAsset.assetCD>>>>.Select(this, ssFA);

                if (faGraph.Asset.Current != null)
                {
                    throw new PXException(GSynchExt.Messages.FAExists, row.SolarSiteCD);
                }
                var classRec = FAClass.PK.Find(faGraph, this.SiteSetup.Current.AssetClassID);
                if (classRec == null)
                {
                    throw new PXException(GSynchExt.Messages.OperationError);
                }
                /*if (row.EstSiteValue == 0 || row.EstSiteValue == null)
                  {
                      throw new PXException(GSynchExt.Messages.SiteValueMissing);
                  }*/

                Sub subRec = GSProjectHelper.GetSubaccount(this, row);
                if (subRec == null)
                    subRec = GSProjectHelper.CreateSubaccount(this, row);


                ///Create New record.

                var faRec = faGraph.Asset.Insert(new FixedAsset
                {
                    AssetCD = ssFA,
                    Description = row.SiteName
                });
                faRec.ClassID = classRec?.AssetID;
                faRec.UsefulLife = classRec.UsefulLife;
                faRec.AssetTypeID = classRec.AssetTypeID;

                //TO DO Add FAAccrual Account to Solar Site Preference
                faRec.FAAccrualAcctID = Account.UK.Find(this, "19000")?.AccountID;

                faRec.FAAccountID = classRec.FAAccountID;
                var sub = GSProjectHelper.GetSubaccount(this, this.Site.Current);
                if (sub == null)
                    sub = GSProjectHelper.CreateSubaccount(this, this.Site.Current);

                faRec.FAAccrualSubID = sub?.SubID ?? classRec.FASubID;
                faRec.FASubID = sub?.SubID ?? classRec.FASubID;
                faRec.GainAcctID = classRec.GainAcctID;
                faRec.GainSubID = faRec.FASubID;
                faRec.LossAcctID = classRec.LossAcctID;
                faRec.LossSubID = faRec.FASubID;
                faRec.DisposalAccountID = classRec.DisposalAccountID;
                faRec.DisposalSubID = faRec.FASubID;
                faRec.AccumulatedDepreciationAccountID = classRec.AccumulatedDepreciationAccountID;
                faRec.AccumulatedDepreciationSubID = faRec.FASubID;
                faRec.DepreciatedExpenseAccountID = classRec.DepreciatedExpenseAccountID;
                faRec.DepreciatedExpenseSubID = faRec.FASubID;

                FADetails details = faGraph.AssetDetails.Current;
                details.AssetID = faRec.AssetID;
                details.ReceiptDate = this.Accessinfo.BusinessDate;
                details.AcquisitionCost = row.EstSiteValue;
                details.DepreciateFromDate = this.Accessinfo.BusinessDate;
                details = faGraph.AssetDetails.Update(details);

                var fALocation = faGraph.AssetLocation.Current;
                fALocation.AssetID = faRec.AssetID;
                fALocation.Department = this.SiteSetup.Current.Department;
                faGraph.AssetLocation.Update(fALocation);
                foreach (FABookSettings bookSet in PXSelect<FABookSettings, Where<FABookSettings.assetID, Equal<Required<FABookSettings.assetID>>>>.Select(this, classRec.AssetID))
                {
                    FABookBalance balRec = (FABookBalance)faGraph.AssetBalance.Cache.Insert(new FABookBalance
                    {
                        BookID = bookSet.BookID,
                        AssetID = faRec.AssetID
                    });
                    balRec.UsefulLife = faRec.UsefulLife;
                    balRec.AcquisitionCost = details.AcquisitionCost;
                    balRec = faGraph.AssetBalance.Insert(balRec);
                    faGraph.AssetBalance.Update(balRec);
                }

                faGraph.Asset.Update(faRec);
                faGraph.Actions.PressSave();
            });
        }
        public virtual void CreateServiceLocation(SolarSite row)
        {
            if (row == null) return;

            PXLongOperation.StartOperation(this, delegate ()
            {
                BranchLocationMaint slGraph = CreateInstance<BranchLocationMaint>();
                FSBranchLocation serviceLoc = (FSBranchLocation)PXSelect<FSBranchLocation, Where<FSBranchLocation.branchLocationCD, Equal<Required<FSBranchLocation.branchLocationCD>>>>.Select(this, row.SolarSiteCD);
                var branchRec = Branch.UK.Find(this, this.Site.Current.Province);
                Sub subRec = GSProjectHelper.GetSubaccount(this, row);
                if (subRec == null)
                    subRec = GSProjectHelper.CreateSubaccount(this, row);

                if (serviceLoc == null)
                {
                    var fsBranchLocation = slGraph.BranchLocationRecords.Insert(new FSBranchLocation
                    {
                        BranchID = branchRec?.BranchID,
                        BranchLocationCD = row.SolarSiteCD,
                        Descr = row.SiteName,
                        DfltSiteID = row.DfltWareHouse ?? SiteSetup.Current.DfltSiteID,
                        SubID = subRec.SubID
                    });
                    slGraph.BranchLocationRecords.Update(fsBranchLocation);
                    var fsContact = slGraph.BranchLocation_Contact.Insert(new FSContact
                    {
                        FullName = row.SiteName
                    });
                    var fsAddress = slGraph.BranchLocation_Address.Insert(new FSAddress
                    {
                        AddressLine1 = row.Address
                    });

                    slGraph.BranchLocation_Contact.Update(fsContact);
                    slGraph.BranchLocation_Address.Update(fsAddress);
                    slGraph.Actions.PressSave();
                }
            });
        }
        #endregion
        public interface ISolarSiteFilter
        {
            int? SolarSiteID { get; }
        }

        [Serializable]
        public partial class PhaseInfo : PXBqlTable, IBqlTable
        {
            #region PhaseID
            [PXDBString(3, IsUnicode = true, InputMask = ">CCC")]
            [PXUIField(DisplayName = "Phase ID")]
            [PXSelector(typeof(Search<Phase.phaseID, Where<GSynchExt.Phase.stateID, Equal<Current<GSynchExt.SolarSite.province>>>>))]
            public virtual string PhaseID { get; set; }
            public abstract class phaseID : PX.Data.BQL.BqlString.Field<phaseID> { }
            #endregion
        }

        [Serializable]
        public partial class SiteInfo : PXBqlTable, IBqlTable
        {

            #region SiteID
            [PXDBString(9, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Solar Site ID", Required = true)]
            [PXDefault]
            public virtual string SiteID { get; set; }
            public abstract class siteID : PX.Data.BQL.BqlString.Field<siteID> { }
            #endregion


        }


    }
}