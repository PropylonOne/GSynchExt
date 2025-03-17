using System;
using System.Collections;
using PX.Data;
using PX.Data.Licensing;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.FS;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PM;
using PX.Objects.PM.ChangeRequest;
using PX.SM;
using PX.TM;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.FA.FABookSettings.midMonthType;
using static PX.Objects.TX.CSTaxCalcType;

namespace GSynchExt
{
    public class ChangeRequestEntryGSExt : PXGraphExtension<ChangeRequestEntry>
    {

        #region IsActive
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.projectModule>(); }
        #endregion

        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBInt]
        [PXSelector(typeof(Search5<EPCompanyTree.workGroupID,
        InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>>,
        InnerJoin<EPEmployee, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>>,
        Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>,
        Aggregate<GroupBy<EPCompanyTree.workGroupID, GroupBy<EPCompanyTree.description>>>>),
        SubstituteKey = typeof(EPCompanyTree.description))]
        [PXDefault(typeof(Search5<EPCompanyTree.workGroupID,
        InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>>,
        InnerJoin<EPEmployee, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>>,
        Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>,
        Aggregate<GroupBy<EPCompanyTree.workGroupID, GroupBy<EPCompanyTree.description>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Workgroup ID", Enabled = false)]
        public virtual void _(Events.CacheAttached<PMChangeRequest.workgroupID> e) { }

    }
}
