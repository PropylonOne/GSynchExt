using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.FS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using static PX.Objects.FS.FSAppointmentDet;
using PX.Objects.Common;

namespace PX.Objects.FS
{
  public class ServiceOrderEntryGSExt : PXGraphExtension<ServiceOrderEntry>
  {

        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBInt]
        [PXUIField(DisplayName = "Target Equipment ID", FieldClass = FSSetup.EquipmentManagementFieldClass)]
        [PXUIEnabled(typeof(Where<Current<isTravelItem>, NotEqual<True>>))]
        [FSSelectorMaintenanceEquipmentAttribute2(typeof(FSServiceOrder.srvOrdType),
                                        typeof(FSServiceOrder.billCustomerID),
                                        typeof(FSServiceOrder.customerID),
                                        typeof(FSServiceOrder.locationID),
                                        typeof(FSServiceOrder.branchID),
                                        typeof(FSServiceOrder.branchLocationID))]

        [PXRestrictor(typeof(Where<FSEquipment.status, Equal<EPEquipmentStatus.EquipmentStatusActive>>),
                        TX.Messages.EQUIPMENT_IS_INSTATUS, typeof(FSEquipment.status))]
        protected void FSSODet_SMEquipmentID_CacheAttached(PXCache sender) { }


  }
}