using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PM;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("BOQGroup")]
    [PXPrimaryGraph(typeof(BOQItemGroupMaint))]
    public class BOQGroup : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<BOQGroup>.By<groupID>
        {
            public static BOQGroup Find(PXGraph graph, string groupID) => FindBy(graph, groupID);
        }
        #endregion
        #region GroupID

        public abstract class groupID : PX.Data.BQL.BqlString.Field<groupID> { }
        protected String _GroupID;
        [PXDBString(30, IsUnicode = true, IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Group ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(BOQGroup.groupID), DescriptionField = typeof(BOQGroup.groupDescription))]
        [PX.Data.EP.PXFieldDescription]
        public virtual String GroupID
        {
            get
            {
                return this._GroupID;
            }
            set
            {
                this._GroupID = value;
            }
        }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXUIField(DisplayName = "Line Cntr")]
        [PXDefault(0)]
        public virtual int? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
        #endregion

        #region GroupDescription
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Group Description")]
        public virtual string GroupDescription { get; set; }
        public abstract class groupDescription : PX.Data.BQL.BqlString.Field<groupDescription> { }
        #endregion

        #region CostCodeID
        [CostCode(null, null, null, DescriptionField = typeof(PMCostCode.description))]
        /*[PXSelector(typeof(Search<PMCostCode.costCodeID>),
            SubstituteKey = (typeof(PMCostCode.costCodeCD)), DescriptionField = typeof(PMCostCode.description))]*/
        public virtual int? CostCodeID
        {
            get;
            set;
        }

        public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }


        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }
}