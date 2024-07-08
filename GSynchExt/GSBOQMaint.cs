using System;
using System.Collections;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.CM;
using PX.Objects.RQ;
using PX.Objects.CN.ProjectAccounting;
using PX.Objects.PM;
using static PX.Objects.CN.ProjectAccounting.CostProjectionEntry;
using GSynchExt.Descriptor;
using static PX.Objects.CT.ContractAction;
using PX.Objects.CT;

namespace GSynchExt
{

    public class GSBOQMaint : PXGraph<GSBOQMaint, GSBOQ>
    {
        #region Selects
        [PXViewName("BOQ")]
        public PXSelect<GSBOQ> BOQ;
        [PXViewName("Current BOQ")]
        public PXSelect<GSBOQ, Where<GSBOQ.bOQID, Equal<Current<GSBOQ.bOQID>>,
            And<GSBOQ.revisionID, Equal<Current<GSBOQ.revisionID>>>>> CurrentBOQ;
        [PXImport]
        public PXSelect<GSBOQMatl, Where<GSBOQMatl.bOQID, Equal<Current<GSBOQ.bOQID>>,
            And<GSBOQMatl.revisionID, Equal<Current<GSBOQ.revisionID>>>>> Materials;

        public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<GSBOQ.curyInfoID>>>> Current_currencyinfo;

        [PXViewName("Answers")]
        public CRAttributeList<GSBOQ> Answers;

        public PXSelect<BOQSetupApproval> SetupApproval;
        [PXViewName("Approval Details")]
        [PXCopyPasteHiddenView]
        public EPApprovalAutomation<GSBOQ, GSBOQ.approved, GSBOQ.rejected, GSBOQ.hold, BOQSetupApproval> Approval;

        public PXFilter<GSBOQCopyDialogInfo> CopyDialog;
        public PXSetup<GSBOQSetup> AutoNumSetup;
        public PXSelect<GSBOQSetup> Setup;
        [PXHidden]
        public PXSetup<Numbering,
                LeftJoin<GSBOQSetup, On<GSBOQSetup.numberingID, Equal<Numbering.numberingID>>>,
                Where<Numbering.numberingID, Equal<GSBOQSetup.numberingID>>> BOQNumbering;
        #endregion

        #region DAC Overrides

        [BOQID(IsKey = true)]
        [PXSelector(typeof(Search<Contract.contractID,
            Where<Contract.baseType, Equal<CTPRType.project>,
                And<Contract.nonProject, Equal<False>,
                And<NotExists<Select<SolarSite,
                    Where<SolarSite.solarSiteCD, Equal<Contract.contractCD>>>>>>>>),
            typeof(Contract.contractCD), typeof(Contract.description), typeof(Contract.status),
            SubstituteKey = (typeof(Contract.contractCD)))]
        protected virtual void _(Events.CacheAttached<GSBOQ.bOQID> e)
        {
            // Do Not Remove this
        }
        
        [RevisionIDField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        [PXDefault(typeof(GSBOQSetup.revisionID), PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXDefault()]
        [PXSelector(typeof(Search<GSBOQ.revisionID, Where<GSBOQ.bOQID, Equal<Current<GSBOQ.bOQID>>>, OrderBy<Desc<GSBOQ.revisionID>>>), DescriptionField = typeof(GSBOQ.description))]

        /*[BOQRev.ID(typeof(GSBOQSetup.revisionID),
            typeof(GSBOQ.bOQID),
            typeof(GSBOQ.revisionID),
            typeof(GSBOQ.revisionID),
            typeof(GSBOQ.description),
            typeof(GSBOQ.status),
            typeof(GSBOQ.startDate),
            typeof(GSBOQ.endDate), DescriptionField = typeof(GSBOQ.description))]*/
        protected virtual void _(Events.CacheAttached<GSBOQ.revisionID> e)
        {
            // Do Not Remove this
        }
        #endregion
        #region Constructor
        public GSBOQMaint()
        {
            GSBOQSetup setup = AutoNumSetup.Current;
            if (string.IsNullOrWhiteSpace(setup?.NumberingID))
            {
                throw new PXException(GSynchExt.Messages.BOQSetupNotEnteredException);
            }
        }

        #endregion

        #region Actions
        public PXAction<GSBOQ> Approve;
        [PXButton(), PXUIField(DisplayName = "Approve",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable approve(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<GSBOQ> Reject;
        [PXButton(), PXUIField(DisplayName = "Reject",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable reject(PXAdapter adapter)
        {
            return adapter.Get();
        }


        public PXAction<GSBOQ> Hold2;
        [PXButton(), PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable hold2(PXAdapter adapter)
        {
            if (BOQ.Current != null)
            {
                BOQ.Current.Hold = true;
                BOQ.Update(BOQ.Current);
            }
            return adapter.Get();
        }


        public PXAction<GSBOQ> CreatePurchaseRequest;
        [PXButton(), PXUIField(DisplayName = "Create Purchase Request",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable createPurchaseRequest(PXAdapter adapter)
        {
            GSBOQ boq = (GSBOQ)this.BOQ.Current;
            {
                PXLongOperation.StartOperation(this, () => CreateInstance<RQRequestEntry>().GetExtension<RQRequestEntryGSExt>().CreateRequestFromBOQ(boq, redirect: true));
            }

            return adapter.Get();
        }

        public PXAction<GSBOQ> Archive;
        [PXButton(), PXUIField(DisplayName = "Archive",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable archive(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<GSBOQ> RemoveHold;
        [PXButton(), PXUIField(DisplayName = "Remove Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable removeHold(PXAdapter adapter)
        {
            if (BOQ.Current != null)
            {
                BOQ.Current.Hold = false;
                BOQ.Update(BOQ.Current);
            }
            return adapter.Get();
        }

        public PXAction<GSBOQ> CreateRevision;
        [PXUIField(DisplayName = "New Revision", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton]
        protected virtual IEnumerable createRevision(PXAdapter adapter)
        {
            if (BOQ.Current == null)
                return adapter.Get();

            this.Save.Press();

            if (CopyDialog.View.Answer == WebDialogResult.None)
            {
                CopyDialog.Cache.Clear();
                GSBOQCopyDialogInfo filterdata = CopyDialog.Cache.Insert() as GSBOQCopyDialogInfo;
            }
            WebDialogResult result = CopyDialog.AskExt();

            if (result != WebDialogResult.OK || string.IsNullOrEmpty(CopyDialog.Current.NewRevisionID))
            {
                return adapter.Get();
            }
            if (BOQ.Current != null)
            {
                GSBOQMaint graph = PXGraph.CreateInstance<GSBOQMaint>();
                GSBOQ boqRec = CreateNewRevision(this.BOQ.Current, CopyDialog.Current, graph);

                boqRec = PXSelect<GSBOQ, Where<GSBOQ.bOQID, Equal<Required<GSBOQ.bOQID>>,
                    And<GSBOQ.revisionID, Equal<Required<GSBOQ.revisionID>>>>>.Select(this, boqRec.BOQID, boqRec.RevisionID);
                if (boqRec != null)
                {
                    graph.BOQ.Current = boqRec;
                    this.BOQ.View.Clear();
                    this.BOQ.View.Cache.ClearQueryCache();
                    this.BOQ.Current = boqRec;
                    throw new PXRedirectRequiredException(graph, true, "New Revision")
                    {
                        Mode = PXBaseRedirectException.WindowMode.Same
                    };
                }
            }

            return adapter.Get();

        }
        #endregion
        #region Event Handlers
        protected virtual void _(Events.RowSelected<GSBOQ> e)
        {
            GSBOQ doc = (GSBOQ)e.Row;
            if (doc == null) return;
            bool isOnHold = doc.Status == GSynchExt.BOQStatus.OnHold && doc.BOQID != null;
            bool hasMatl = this.Materials.Select().Count != 0;
            bool hasCapacity = !(string.IsNullOrEmpty(string.Concat(doc.PhaseCapacity, doc.CapacityType1, doc.CapacityType2, doc.CapacityType3, doc.CapacityType4, doc.CapacityType5)));

            e.Cache.AllowInsert = true;
            e.Cache.AllowDelete = isOnHold;

            this.Materials.Cache.AllowInsert = isOnHold && hasCapacity;
            this.Materials.Cache.AllowUpdate = isOnHold && hasCapacity;
            this.Materials.Cache.AllowDelete = isOnHold;

            PXUIFieldAttribute.SetRequired<GSBOQ.bOQID>(e.Cache, true);
            PXUIFieldAttribute.SetRequired<GSBOQ.description>(e.Cache, true);
            PXUIFieldAttribute.SetEnabled<GSBOQ.phaseCapacity>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType1>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType2>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType3>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType4>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType5>(e.Cache, doc, isOnHold && !hasMatl);
            //PXUIFieldAttribute.SetEnabled<GSBOQ.projectID>(e.Cache, doc, isOnHold && !hasMatl);
            // PXUIFieldAttribute.SetEnabled<GSBOQ.revisionID>(e.Cache, doc, isOnHold && !hasMatl);

            CreateRevision.SetVisible(hasMatl && !e.Cache.IsDirty && doc.Status == GSynchExt.BOQStatus.Active);
            CreatePurchaseRequest.SetVisible(false); //this is not used. Use the action button Add BOQ item in the request screen
        }
        protected virtual void _(Events.FieldUpdated<GSBOQMatl, GSBOQMatl.inventoryID> e)
        {
            var row = e.Row as GSBOQMatl;
            if (row == null) return;
            e.Cache.SetDefaultExt<GSBOQMatl.uOM>(e.Row);
            e.Cache.SetDefaultExt<GSBOQMatl.unitCost>(e.Row);
        }

        #endregion
        #region Methods
        protected virtual GSBOQ CreateNewRevision(GSBOQ original, GSBOQCopyDialogInfo info, GSBOQMaint newGraph)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                try
                {
                    ///Set Original to Archived
                    original.Status = BOQStatus.Archived;
                    original.EndDate = this.Accessinfo.BusinessDate;
                    this.BOQ.Update(original);
                    this.Actions.PressSave();

                    /// Create Summary Record
                    GSBOQ newDoc = new GSBOQ();
                    newDoc.BOQID = original.BOQID;
                    newDoc.RevisionID = info.NewRevisionID;
                    newDoc.StartDate = this.Accessinfo.BusinessDate;
                    newDoc.EndDate = null;

                    newDoc.TaskID = original.TaskID;
                    newDoc.StartDate = original.StartDate;
                    newDoc.EndDate = original.EndDate;
                    newDoc.PhaseCapacity = original.PhaseCapacity;
                    newDoc.CapacityType1 = original.CapacityType1;
                    newDoc.CapacityType2 = original.CapacityType2;
                    newDoc.CapacityType3 = original.CapacityType3;
                    newDoc.CapacityType4 = original.CapacityType4;
                    newDoc.CapacityType5 = original.CapacityType5;
                    newDoc.EPCVendorID = original.EPCVendorID;
                    newDoc.Creator = original.Creator;
                    newDoc.Approver = original.Approver;

                    //GSBOQMaint newGraph = PXGraph.CreateInstance<GSBOQMaint>();
                    newGraph.Clear();
                    newGraph.SelectTimeStamp();
                    var res = newGraph.BOQ.Insert(newDoc);
                    newGraph.BOQ.Cache.SetValue<GSBOQ.bOQID>(res, newDoc.BOQID);
                    newGraph.BOQ.Cache.SetValue<GSBOQ.revisionID>(res, newDoc.RevisionID);
                    if (info.CopyNotes == true)
                    {
                        string note = PXNoteAttribute.GetNote(BOQ.Cache, original);
                        PXNoteAttribute.SetNote(newGraph.BOQ.Cache, newDoc, note);
                    }
                    if (info.CopyFiles == true)
                    {
                        Guid[] files = PXNoteAttribute.GetFileNotes(BOQ.Cache, original);
                        PXNoteAttribute.SetFileNotes(newGraph.BOQ.Cache, newDoc, files);
                    }

                    /// Create Details
                    foreach (GSBOQMatl line in this.Materials.Select())
                    {
                        GSBOQMatl copy = (GSBOQMatl)Materials.Cache.CreateCopy(line);
                        /// Set new values
                        copy.BOQID = newDoc.BOQID;
                        copy.RevisionID = newDoc.RevisionID;
                        copy.Noteid = null;
                        copy.LineNbr = null;
                        copy = newGraph.Materials.Insert(copy);

                        if (info.CopyNotes == true)
                        {
                            string note = PXNoteAttribute.GetNote(Materials.Cache, line);
                            PXNoteAttribute.SetNote(newGraph.Materials.Cache, copy, note);
                        }
                        if (info.CopyFiles == true)
                        {
                            Guid[] files = PXNoteAttribute.GetFileNotes(Materials.Cache, line);
                            PXNoteAttribute.SetFileNotes(newGraph.Materials.Cache, copy, files);
                        }
                    }

                    /// Save  new graph
                    newGraph.Actions.PressSave();
                    newGraph.BOQ.Current = res;
                    ts.Complete();
                    return newGraph.BOQ.Current;

                }
                catch (Exception e)
                {
                    ts.Complete();
                    throw new PXException(e.Message);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [PXHidden]
        public class GSBOQCopyDialogInfo : PXBqlTable, IBqlTable
        {
            #region RevisionID

            public abstract class newRevisionID : PX.Data.BQL.BqlString.Field<newRevisionID> { }
            [PXString(10, InputMask = ">CCCCCCCCCC")]
            [PXDefault]
            [PXUIField(DisplayName = "New Revision")]
            public virtual string NewRevisionID { get; set; }

            #endregion
            #region copyNotes
            public abstract class copyNotes : PX.Data.BQL.BqlBool.Field<copyNotes> { }
            [PXBool()]
            [PXUIField(DisplayName = "Copy Notes")]
            public virtual bool? CopyNotes { get; set; }

            #endregion
            #region copyFiles
            public abstract class copyFiles : PX.Data.BQL.BqlBool.Field<copyFiles> { }
            [PXBool()]
            [PXUIField(DisplayName = "Copy Files")]
            public virtual bool? CopyFiles { get; set; }

            #endregion
        }
        #endregion


        public PXAction<GSBOQ> Assign;
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
        [PXUIField(DisplayName = "Assign", Visible = false)]
        public virtual IEnumerable assign(PXAdapter adapter)
        {
            foreach (GSBOQ req in adapter.Get<GSBOQ>())
            {
                if (Setup.Current.ApprovalMap != null)
                {
                    var processor = new EPAssignmentProcessor<GSBOQ>();
                    processor.Assign(req, SetupApproval.Current.AssignmentMapID);
                    req.WorkgroupID = req.ApprovalWorkgroupID;
                    req.OwnerID = req.ApprovalOwnerID;

                }
                yield return req;
            }
        }

        #region EPApproval Cache Attached - Approvals Fields
        [PXDBDate()]
        [PXDefault(typeof(GSBOQ.startDate), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.docDate> e)

        {

        }
        [PXDBString(60, IsUnicode = true)]
        [PXDefault(typeof(GSBOQ.description), PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<EPApproval.descr> e)

        {

        }

        #endregion
    }
}