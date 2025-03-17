using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.RQ;
using PX.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace PX.Objects.RQ
{
  [PXProjection(typeof(Select<RQRequisitionLine>), Persistent = false)]
  [Serializable]
  public sealed class RQRequisitionLineBiddingGSExt : PXCacheExtension<PX.Objects.RQ.RQRequisitionLineBidding>
  {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.inventory>(); }
        #endregion

        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
    protected Guid? _NoteID;
    [PXNote(BqlTable = typeof(RQRequisitionLine), BqlField = typeof(RQRequisitionLine.noteID))]
    public Guid? NoteID
    {
      get
      {
        return this._NoteID;
      }
      set
      {
        this._NoteID = value;
      }
    }
    #endregion
  }
}