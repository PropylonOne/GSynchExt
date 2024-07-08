using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System;
using PX.Objects.EP;

namespace PX.Objects.PM
{
    public class PMChangeRequestGSExt : PXCacheExtension<PX.Objects.PM.PMChangeRequest>
    {
        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        #region WorkgroupID  
        [PXDBInt]
        [PXSelector(typeof(Search5<EPCompanyTree.workGroupID,
        InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>>,
        InnerJoin<EPEmployee, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>>,
        Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>,
        Aggregate<GroupBy<EPCompanyTree.workGroupID, GroupBy<EPCompanyTree.description>>>>),
        SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Workgroup ID", Enabled = false)]
        public int? WorkgroupID { get; set; }
        #endregion
    }
}