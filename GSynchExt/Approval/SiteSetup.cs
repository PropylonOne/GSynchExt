using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.IN.ItemClassTree.INItemClass;

namespace GSynchExt
{
	[Serializable]
	[PXCacheName("Solar Site Preference")]
	[PXPrimaryGraph(typeof(SolarSiteSurveyMaint))]
	public class SiteSetup : PXBqlTable, IBqlTable
	{
		public abstract class siteApprovalMap : BqlBool.Field<siteApprovalMap>
		{
		}
		[EPRequireApproval]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
		[PXUIField(DisplayName = "Require Approval")]
		public virtual bool? SiteApprovalMap { get; set; }

        #region SurveyNumberingID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Survey Numbering Sequence")]
        [PXDefault("SSSurvey")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string SurveyNumberingID { get; set; }
        public abstract class surveyNumberingID : PX.Data.BQL.BqlString.Field<surveyNumberingID> { }
        #endregion

        #region RevGenNumberigID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Solar Rev. Gen. Numbering Sequence")]
        [PXDefault("SREVGEN")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string RevGenNumberigID { get; set; }
        public abstract class revGenNumberigID : PX.Data.BQL.BqlString.Field<revGenNumberigID> { }
        #endregion

        #region MapConsStart 
        public abstract class mapConsStart : PX.Data.BQL.BqlString.Field<mapConsStart> { }

        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Construction Start")]
        public virtual String MapConsStart
        {
            get;
            set;
        }
        #endregion

        #region MapConsEnd 
        public abstract class mapConsEnd  : PX.Data.BQL.BqlString.Field<mapConsEnd > { }
        
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Construction End")]
        public virtual String MapConsEnd 
        {
			get;
			set;
        }
        #endregion

        #region MapCommissioned 
        public abstract class mapCommissioned : PX.Data.BQL.BqlString.Field<mapCommissioned> { }

        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Commission of Site")]
        public virtual String MapCommissioned
        {
            get;
            set;
        }
		#endregion

		#region MapConnectedToGrid
		public abstract class mapConnectedToGrid : PX.Data.BQL.BqlString.Field<mapConnectedToGrid> { }

		[PXDBString(IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Connected To Grid")]
		public virtual String MapConnectedToGrid
		{
			get;
			set;
		}
        #endregion

        #region MapReleasedToServices
        public abstract class mapReleasedToServices : PX.Data.BQL.BqlString.Field<mapReleasedToServices> { }

        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Released To Services")]
        public virtual String MapReleasedToServices
        {
            get;
            set;
        }
        #endregion

        #region TimeAccntGrp
        [PXDBInt]
        [PXUIField(DisplayName = "Default Account Group for Timeline Tasks", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search< PMAccountGroup.groupID> ), SubstituteKey = typeof(PMAccountGroup.groupCD) )]
        public virtual int? TimeAccntGrp { get; set; }
        public abstract class timeAccntGrp : BqlInt.Field<timeAccntGrp> { }
        #endregion

        #region AssetClassID
        public abstract class assetClassID : PX.Data.BQL.BqlInt.Field<assetClassID> { }

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
		public virtual int? AssetClassID { get; set; }
        #endregion

        #region ItemClassID
        [PXDBInt]
		[PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
		[PXDefault(typeof(
			Search2<INItemClass.itemClassID, InnerJoin<INSetup,
				On<stkItem.FromCurrent.IsEqual<False>.And<INSetup.dfltNonStkItemClassID.IsEqual<INItemClass.itemClassID>>.
				Or<stkItem.FromCurrent.IsEqual<True>.And<INSetup.dfltStkItemClassID.IsEqual<INItemClass.itemClassID>>>>>>))]
		[PXUIRequired(typeof(INItemClass.stkItem))]
		public virtual int? ItemClassID { get; set; }
		public abstract class itemClassID : BqlInt.Field<itemClassID> { }
		#endregion

		#region Department
		
		public abstract class department : PX.Data.BQL.BqlString.Field<department> { }
		
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
		[PXUIField(DisplayName = "Department")]
		public virtual String Department{  get; set; }
		#endregion

		#region SubID

		[SubAccount(DisplayName = "Sub Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXUIField(DisplayName = "Default Sub Account")]
        public virtual Int32? SubID { get; set; }
		public abstract class subID : BqlInt.Field<subID> { }
		#endregion

		#region MfRate

		[PXDBBaseCury]
		[PXUIField(DisplayName = "Management Fee Rate ($)")]
        public virtual Decimal? MfRate { get; set; }
        public abstract class mfRate : BqlDecimal.Field<mfRate> { }
		#endregion

		#region StampDutyLimit

		[PXDBQuantity]
		[PXUIField(DisplayName = "Stamp Duty Limit (LKR)")]
        public virtual Decimal? StampDutyLimit { get; set; }
        public abstract class stampDutyLimit : BqlDecimal.Field<stampDutyLimit> { }
		#endregion

		#region StampDutyAmount

		[PXDBBaseCury]
		[PXUIField(DisplayName = "Stamp Duty Amount (Rs.) ")]
        public virtual Decimal? StampDutyAmount { get; set; }
        public abstract class stampDutyAmount : BqlDecimal.Field<stampDutyAmount> { }
        #endregion

        #region DfltSiteID
        public abstract class dfltSiteID : PX.Data.BQL.BqlInt.Field<dfltSiteID> { }
		[Site(DisplayName = "Default Warehouse", DescriptionField = typeof(INSite.descr))]
		public virtual int? DfltSiteID { get; set; }
        #endregion


        #region IssueReasonCodePrefix 
        public abstract class issueReasonCodePrefix : PX.Data.BQL.BqlString.Field<issueReasonCodePrefix> { }

        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Reason Code Prefix")]
        public virtual String IssueReasonCodePrefix
        {
            get;
            set;
        }
        #endregion

        #region ReceiptReasonCodePrefix 
        public abstract class receiptReasonCodePrefix : PX.Data.BQL.BqlString.Field<receiptReasonCodePrefix> { }

        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Receipt Reason Code Prefix")]
        public virtual String ReceiptReasonCodePrefix
        {
            get;
            set;
        }
        #endregion


    }
}
