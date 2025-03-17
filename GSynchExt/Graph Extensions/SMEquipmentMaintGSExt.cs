using PX.Data;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static PX.Objects.FA.FABookSettings.midMonthType;
using static PX.Objects.FS.ID;

namespace PX.Objects.FS
{
    public class SMEquipmentMaintGSExt : PXGraphExtension<SMEquipmentMaint>    
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.inventory>(); }
        #endregion

        protected virtual void _(Events.RowPersisting<FSEquipment> e)
        {

            
            if (e.Row == null)
            {
                return;
            }

            var createdByScreenID = e.Row.CreatedByScreenID;
            if (createdByScreenID != "FS300100") return; 

            FSEquipment fsEquipmentRow = (FSEquipment)e.Row;
            PXCache cache = e.Cache;


            
            FSServiceOrder srvOrder = PXSelect<FSServiceOrder, Where<FSServiceOrder.refNbr, Equal<Required<FSServiceOrder.refNbr>>>>.Select(this.Base, fsEquipmentRow.InstServiceOrderRefNbr);
            if(e.Row.BranchID == null || e.Row.BranchLocationID == null || e.Row.LocationType == null)
            {
                cache.SetValue<FSEquipment.locationType>(e.Row, LocationType.COMPANY);
                cache.SetValue<FSEquipment.branchID>(e.Row, srvOrder?.BranchID);
                cache.SetValue<FSEquipment.branchLocationID>(e.Row, srvOrder?.BranchLocationID);
                e.Cache.Update(fsEquipmentRow);
            }                    
        }
    }
}
