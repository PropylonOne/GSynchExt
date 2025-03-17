using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.CS;
using System.Collections;
using System.Linq;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Common;
using PX.Objects.FA;
using PX.DbServices;
using GSynchExt;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.FS;

namespace PX.Objects.FA
{
    #region DACS
    [Serializable]
    [PXCacheName(GSynchExt.Messages.FARequestFilter)]
    public partial class ISGAFARequestFilter : PXBqlTable, IBqlTable
    {
        public class PK : PrimaryKeyOf<ISGAFARequestFilter>.By<reqNbr>
        {
            public static ISGAFARequestFilter Find(PXGraph graph, string reqNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, reqNbr);
        }
        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXSelector(typeof(Search<ISGAFARequest.reqNbr>), typeof(ISGAFARequest.reqNbr), typeof(ISGAFARequest.status))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion
        #region ActionType
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXStringList(new string[]
                  {
                   GSynchExt.FARequestActionType.Issue ,
                   GSynchExt.FARequestActionType.Return,},
                  new string[]
                  { GSynchExt.Messages.IssueActionType,
                    GSynchExt.Messages.ReturnActionType,})]
        [PXUIField(DisplayName = "Action Type")]
        public virtual string ActionType { get; set; }
        public abstract class actionType : PX.Data.BQL.BqlString.Field<actionType> { }
        #endregion
    }
    [Serializable]
    [PXCacheName(GSynchExt.Messages.FARequestItems)]
    public partial class ISGAFARequestItems : PXBqlTable, IBqlTable
    {
        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]
        public virtual Boolean? Selected
        {
            get;
            set;
        }
        #endregion
        #region AssetID
        [PXDBInt(IsKey = true)]
        [PXSelector(typeof(Search<FixedAsset.assetID>), SubstituteKey = typeof(FixedAsset.assetCD))]
        [PXUIField(DisplayName = "Asset ID")]
        public virtual int? AssetID { get; set; }
        public abstract class assetID : PX.Data.BQL.BqlInt.Field<assetID> { }
        #endregion
        #region Description
        [PXString(255, IsUnicode = true, InputMask = "")]
        [PXDependsOnFields(typeof(assetID))]
        [PXUnboundDefault(typeof(Search<FixedAsset.description, Where<FixedAsset.assetID, Equal<Current<assetID>>>>))]
        [PXFormula(typeof(Search<FixedAsset.description, Where<FixedAsset.assetID, Equal<Current<assetID>>>>))]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
        #region Qty
        [PXDBDecimal]
        [PXUIField(DisplayName = "Qty")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion
        #region OpenQty
        [PXDBDecimal]
        [PXUIField(DisplayName = "OpenQty", Enabled = false)]
        public virtual Decimal? OpenQty { get; set; }
        public abstract class openQty : PX.Data.BQL.BqlDecimal.Field<openQty> { }
        #endregion
        #region ReqNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXSelector(typeof(Search<ISGAFARequest.reqNbr>), typeof(ISGAFARequest.reqNbr), typeof(ISGAFARequest.status))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion
        #region ActionType
        [PXDBString(1, IsFixed = true, InputMask = "", IsKey = true)]
        [PXStringList(
                  new string[]
                  {
                 GSynchExt.FARequestActionType.Issue ,
                 GSynchExt.FARequestActionType.Return ,
                  },
                  new string[]
                  {
                 GSynchExt.Messages.IssueActionType,
                 GSynchExt.Messages.ReturnActionType,
                  })]
        [PXUIField(DisplayName = "Action Type")]
        public virtual string ActionType { get; set; }
        public abstract class actionType : PX.Data.BQL.BqlString.Field<actionType> { }
        #endregion
    }
    #endregion
    [PX.Objects.GL.TableAndChartDashboardType]
    public class ISGAFAProcessFARequests : PXGraph<ISGAFAProcessFARequests>
    {
        #region views
        public PXCancel<ISGAFARequestFilter> Cancel;
        public PXFilter<ISGAFARequestFilter> Filter;
        [PXFilterable]
        public PXFilteredProcessing<ISGAFARequestItems, ISGAFARequestFilter> FARequests;
        #endregion
        public ISGAFAProcessFARequests()
        {
            FARequests.SetProcessCaption(PX.Objects.IN.Messages.Process);
            FARequests.SetProcessAllCaption(PX.Objects.IN.Messages.ProcessAll);
            PXUIFieldAttribute.SetEnabled<ISGAFARequestItems.qty>(FARequests.Cache, null, isEnabled: true);
        }
        protected virtual IEnumerable Farequests()
        {
            bool found = false;
            ISGAFARequestFilter filter = Filter.Current;
            if (filter == null)
            {
                yield break;
            }
            foreach (ISGAFARequestItems item in FARequests.Cache.Inserted)
            {
                found = true;
                yield return item;
            }
            if (found)
                yield break;
            if (Filter.Current.ActionType != null && Filter.Current.ReqNbr != null)
            {
                if (Filter.Current.ActionType == FARequestActionType.Issue)
                {
                    PXSelectBase<ISGAFARequestLine> c = new PXSelect
                                        <ISGAFARequestLine,
                                        Where<ISGAFARequestLine.reqNbr,
                                        Equal<Current<ISGAFARequestFilter.reqNbr>>,
                                        And<ISGAFARequestLine.openQty, Greater<decimal0>>>>(this);
                    foreach (PXResult<ISGAFARequestLine> row in c.Select())
                    {
                        ISGAFARequestLine fARequestLine = row;

                        ISGAFARequestItems requestLine = new ISGAFARequestItems();
                        requestLine.AssetID = fARequestLine.AssetID;
                        requestLine.Description = fARequestLine.Description;
                        requestLine.Qty = fARequestLine.OpenQty;
                        requestLine.ReqNbr = fARequestLine.ReqNbr;
                        requestLine.ActionType = filter.ActionType;
                        yield return FARequests.Insert(requestLine);
                    }
                }
                if (Filter.Current.ActionType == FARequestActionType.Return)
                {
                    PXSelectBase<ISGAFARequestLine> c = new PXSelect
                                        <ISGAFARequestLine,
                                        Where<ISGAFARequestLine.reqNbr,
                                        Equal<Current<ISGAFARequestFilter.reqNbr>>,
                                        And<ISGAFARequestLine.issuedQty, Greater<decimal0>,
                                        And<ISGAFARequestLine.completed, NotEqual<True>>>>>(this);
                    foreach (PXResult<ISGAFARequestLine> row in c.Select())
                    {
                        ISGAFARequestLine fARequestLine = row;

                        ISGAFARequestItems requestLine = new ISGAFARequestItems();
                        requestLine.AssetID = fARequestLine.AssetID;
                        requestLine.Description = fARequestLine.Description;
                        requestLine.Qty = fARequestLine.IssuedQty - fARequestLine.ReturnedQty;
                        requestLine.ReqNbr = fARequestLine.ReqNbr;
                        requestLine.ActionType = filter.ActionType;
                        yield return FARequests.Insert(requestLine);
                    }
                }

            }
        }
        #region Override
        public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
        {
            try
            {
                return base.ExecuteUpdate(viewName, keys, values, parameters);
            }
            catch (Exception ex)
            {
                if (viewName.ToLower() == GSynchExt.Messages.farequests && ((ex is System.Data.SqlClient.SqlException exception && exception.Number == 208) || (ex is PXArgumentException exception1 && exception1.ParamName == GSynchExt.Messages.table)))
                {
                    FARequests.Select();
                    try
                    {
                        return base.ExecuteUpdate(viewName, keys, values, parameters);
                    }
                    catch (System.Data.SqlClient.SqlException)
                    {
                        return 0;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion
        #region EventHandlers
        protected virtual void _(Events.RowPersisting<ISGAFARequestFilter> e)
        {
            if (string.IsNullOrEmpty(e.Row.ReqNbr))
            {
                throw new PXRowPersistingException(nameof(e.Row.ReqNbr), null, GSynchExt.Messages.ReqDateMandatory);
            }
        }
        public void _(Events.RowSelected<ISGAFARequestFilter> e)
        {
            if (e.Row != null)
            {
                FARequests.SetProcessDelegate<UpdateFARequestProcess>(UpdateFARecords);
            }
        }
        #endregion
        protected virtual void ISGAFARequestFilter_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            FARequests.Cache.Clear();
        }
        public static void UpdateFARecords(UpdateFARequestProcess graph, ISGAFARequestItems items)
        {
            graph.UpdateFARecords(items);
        }
    }
    public class UpdateFARequestProcess : PXGraph<UpdateFARequestProcess>
    {
        public virtual void UpdateFARecords(ISGAFARequestItems rec)
        {
            using (new PXConnectionScope())
            {
                var assetMaint = PXGraph.CreateInstance<AssetMaint>();
                var iSGAFARequestEntry = PXGraph.CreateInstance<ISGAFARequestEntry>();
                ISGAFARequestLine fARequestLine = ISGAFARequestLine.PK.Find(this, rec.ReqNbr, rec.AssetID);
                if (fARequestLine == null)
                {
                    throw new PXException(GSynchExt.Messages.ReqLineNotFound);
                }
                if (rec.ActionType == FARequestActionType.Issue && rec.Qty > fARequestLine.OpenQty)
                {
                    throw new PXException(GSynchExt.Messages.IssuedQtyExceedRequested);
                }
                if (rec.ActionType == FARequestActionType.Return && rec.Qty > (fARequestLine.IssuedQty - fARequestLine.ReturnedQty))
                {
                    throw new PXException(GSynchExt.Messages.ReturnedQtyExceedRequested);
                }
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    FixedAsset fAsset = FixedAsset.PK.Find(this, rec.AssetID);
                    ISGAFARequest fARequest = ISGAFARequest.PK.Find(this, rec.ReqNbr);
                    assetMaint.CurrentAsset.Current = fAsset;
                    FixedAssetGSExt assetGSExt = fAsset.GetExtension<FixedAssetGSExt>();
                    iSGAFARequestEntry.FARequest.Current = fARequest;
                    iSGAFARequestEntry.FARequestDet.Current = fARequestLine;
                    if (rec.ActionType == FARequestActionType.Issue)
                    {
                        ISGAFARequestEvnt requestEvnt = iSGAFARequestEntry.ReqEventRecords.Insert();
                        requestEvnt.ReqNbr = rec.ReqNbr;
                        requestEvnt.AssetID = rec.AssetID;
                        requestEvnt.ActionType = rec.ActionType;
                        requestEvnt.IssuedDate = this.Accessinfo.BusinessDate;
                        requestEvnt.IssuedQty = rec.Qty;
                        requestEvnt.OpenQty = fARequestLine.RequestQty - rec.Qty - (fARequestLine.IssuedQty ?? 0m);
                        requestEvnt.IssuedBy = ISGAFARequestEntry.GetCurrentEmployee(this).BAccountID;
                        iSGAFARequestEntry.FARequestDet.Current.IssuedQty += rec.Qty;
                        iSGAFARequestEntry.FARequestDet.Current.OpenQty -= rec.Qty;
                        iSGAFARequestEntry.FARequestDet.Current.IssuedBy = ISGAFARequestEntry.GetCurrentEmployee(this).BAccountID;
                        iSGAFARequestEntry.FARequestDet.Current.IssuedDate = this.Accessinfo.BusinessDate;
                        assetGSExt.UsrISGAIssuedQty += rec.Qty;

                        if (iSGAFARequestEntry.FARequestDet.Current.IssuedQty == iSGAFARequestEntry.FARequestDet.Current.RequestQty)
                        {
                            fARequest.IsFullyIssued = true;
                        }
                        iSGAFARequestEntry.ReqEventRecords.Update(requestEvnt);
                        iSGAFARequestEntry.FARequest.Update(fARequest);
                        iSGAFARequestEntry.FARequestDet.Update(iSGAFARequestEntry.FARequestDet.Current);
                        iSGAFARequestEntry.Actions.PressSave();
                        assetMaint.CurrentAsset.Current = fAsset;
                        assetMaint.CurrentAsset.Update(fAsset);
                        assetMaint.Actions.PressSave();
                    }
                    if (rec.ActionType == FARequestActionType.Return)
                    {
                        ISGAFARequestEvnt requestEvnt = iSGAFARequestEntry.ReqEventRecords.Insert();
                        requestEvnt.ReqNbr = rec.ReqNbr;
                        requestEvnt.AssetID = rec.AssetID;
                        requestEvnt.ActionType = rec.ActionType;
                        requestEvnt.ReturnedDate = this.Accessinfo.BusinessDate;
                        requestEvnt.ReturnedQty = rec.Qty;
                        requestEvnt.OpenQty = fARequestLine.OpenQty;
                        requestEvnt.ReturnedBy = ISGAFARequestEntry.GetCurrentEmployee(this).BAccountID;
                        iSGAFARequestEntry.FARequestDet.Current.ReturnedQty += rec.Qty;
                        iSGAFARequestEntry.FARequestDet.Current.ReturnedBy = ISGAFARequestEntry.GetCurrentEmployee(this).BAccountID;
                        iSGAFARequestEntry.FARequestDet.Current.ReturnedDate = this.Accessinfo.BusinessDate;
                        bool isComplete = iSGAFARequestEntry.FARequestDet.Current.RequestQty == iSGAFARequestEntry.FARequestDet.Current.IssuedQty
                                        && iSGAFARequestEntry.FARequestDet.Current.IssuedQty == iSGAFARequestEntry.FARequestDet.Current.ReturnedQty
                                        && iSGAFARequestEntry.FARequestDet.Current.RequestQty > 0;
                        if (isComplete)
                        {
                            iSGAFARequestEntry.FARequestDet.Current.Completed = true;
                        }
                        assetGSExt.UsrISGARequestedQty -= rec.Qty;
                        assetGSExt.UsrISGAIssuedQty -= rec.Qty;

                        iSGAFARequestEntry.ReqEventRecords.Update(requestEvnt);
                        iSGAFARequestEntry.FARequest.Update(fARequest);
                        iSGAFARequestEntry.FARequestDet.Update(iSGAFARequestEntry.FARequestDet.Current);
                        iSGAFARequestEntry.Actions.PressSave();
                        assetMaint.CurrentAsset.Update(fAsset);
                        assetMaint.Actions.PressSave();
                    }
                    ts.Complete();
                }
            }
        }
    }
}
