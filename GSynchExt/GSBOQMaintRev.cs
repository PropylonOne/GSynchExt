using System;
using System.Collections;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects.CN.ProjectAccounting;
using PX.Objects.PM;
using static PX.Objects.CN.ProjectAccounting.CostProjectionEntry;

namespace GSynchExt
{

    public class GSBOQMaintRev : PXGraph<GSBOQMaintRev, GSBOQ>
    {
        #region Selects
        [PXImport(typeof(GSBOQ))]
        public PXSelect<GSBOQRev> BOQ;
        [PXImport(typeof(GSBOQ))]
        public PXSelect<GSBOQ, Where<GSBOQRev.bOQID, Equal<Current<GSBOQRev.bOQID>>,
            And<GSBOQRev.revisionID, Equal<Current<GSBOQRev.revisionID>>>>> CurrentBOQ;
        [PXImport(typeof(GSBOQ))]
        public PXSelect<GSBOQMatl, Where<GSBOQMatl.bOQID, Equal<Current<GSBOQRev.bOQID>>,
            And<GSBOQMatl.revisionID, Equal<Current<GSBOQRev.revisionID>>>>> Materials;

        #endregion

        #region Constructor

        #endregion
        /*
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
            if (CopyDialog.AskExt() != WebDialogResult.OK || string.IsNullOrEmpty(CopyDialog.Current.NewRevisionID))
            {
                return adapter.Get();
            }
            if (BOQ.Current != null)
            {
                CreateNewRevision(BOQ.Current, CopyDialog.Current);

            }

            return adapter.Get();

        }
        #endregion
        */
        #region Event Handlers
        /*
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

            PXUIFieldAttribute.SetEnabled<GSBOQ.phaseCapacity>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType1>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType2>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType3>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType4>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.capacityType5>(e.Cache, doc, isOnHold && !hasMatl);
            PXUIFieldAttribute.SetEnabled<GSBOQ.projectID>(e.Cache, doc, isOnHold && !hasMatl);
            // PXUIFieldAttribute.SetEnabled<GSBOQ.revisionID>(e.Cache, doc, isOnHold && !hasMatl);

            CreateRevision.SetVisible(doc.BOQID != null && hasMatl && !e.Cache.IsDirty);
            CreatePurchaseRequest.SetVisible(doc.Status == GSynchExt.Status.Active && !e.Cache.IsDirty);
        }
        */
        #endregion
        #region Methods
        public virtual GSBOQ CreateNewRevision(GSBOQMaintRev target, GSBOQMaint graph, GSBOQ original, GSBOQMaint.GSBOQCopyDialogInfo info)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                try
                {
                    GSBOQRev newDoc = new GSBOQRev();
                    newDoc.BOQID = original.BOQID;
                    newDoc.RevisionID = info.NewRevisionID;

                    newDoc.Status = BOQStatus.OnHold;
                    newDoc.StartDate = graph.Accessinfo.BusinessDate;
                    newDoc.EndDate = null;
                    newDoc.TaskID = original.TaskID;
                    newDoc.CuryID = original.CuryID;
                    newDoc.PhaseCapacity = original.PhaseCapacity;
                    newDoc.CapacityType1 = original.CapacityType1;
                    newDoc.CapacityType2 = original.CapacityType2;
                    newDoc.CapacityType3 = original.CapacityType3;
                    newDoc.CapacityType4 = original.CapacityType4;
                    newDoc.CapacityType5 = original.CapacityType5;
                    newDoc.EPCVendorID = original.EPCVendorID;
                    newDoc.Creator = original.Creator;
                    newDoc.Approver = original.Approver;

                    original.Status = BOQStatus.Archived;
                    original.EndDate = this.Accessinfo.BusinessDate;

                    target.Clear();
                    target.SelectTimeStamp();
                    var res = target.BOQ.Insert(newDoc);
                    target.BOQ.Cache.SetValue<GSBOQ.bOQID>(res, newDoc.BOQID);

                    if (info.CopyNotes == true)
                    {
                        string note = PXNoteAttribute.GetNote(graph.BOQ.Cache, original);
                        PXNoteAttribute.SetNote(target.BOQ.Cache, newDoc, note);
                    }

                    if (info.CopyFiles == true)
                    {
                        Guid[] files = PXNoteAttribute.GetFileNotes(graph.BOQ.Cache, original);
                        PXNoteAttribute.SetFileNotes(target.BOQ.Cache, newDoc, files);
                    }
                    //Copy Lines
                    var materials = graph.Materials.Select();
                    foreach (GSBOQMatl line in materials)
                    {
                        GSBOQMatl copy = (GSBOQMatl)graph.Materials.Cache.CreateCopy(line);

                        copy.RevisionID = newDoc.RevisionID;
                        copy.BOQID = newDoc.BOQID;
                        copy.Noteid = null;
                        copy = target.Materials.Insert(copy);

                        if (info.CopyNotes == true)
                        {
                            string note = PXNoteAttribute.GetNote(graph.Materials.Cache, line);
                            PXNoteAttribute.SetNote(target.Materials.Cache, copy, note);
                        }

                        if (info.CopyFiles == true)
                        {
                            Guid[] files = PXNoteAttribute.GetFileNotes(graph.Materials.Cache, line);
                            PXNoteAttribute.SetFileNotes(target.Materials.Cache, copy, files);
                        }
                    }

                    target.Actions.PressSave();
                    var rec = GSBOQ.UK.Find(graph, newDoc.BOQID, newDoc.RevisionID);
                    graph.BOQ.Update(original);
                    graph.Actions.PressSave();
                    ts.Complete();
                    return rec;
                }
                catch (Exception e)
                {
                    ts.Complete();
                    throw new PXException(e.Message);
                }
            }
        }


        #endregion
    }
}
