using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("BOQ Group Items")]
    public class BOQGroupItems : PXBqlTable, IBqlTable
    {
        #region Keys
        public static class FK
        {
            public class MasterRec : BOQGroup.PK.ForeignKeyOf<BOQGroupItems>.By<groupID> { }
        }
        public class PK : PrimaryKeyOf<BOQGroupItems>.By<groupID, lineNbr>
        {
            public static BOQGroupItems Find(PXGraph graph, string groupID, int lineNbr) => FindBy(graph, groupID, lineNbr);
        }
        public class UK : PrimaryKeyOf<BOQGroupItems>.By<groupID, inventoryID>
        {
            public static BOQGroupItems Find(PXGraph graph, string groupID, int inventoryID) => FindBy(graph, groupID, inventoryID);
        }
        #endregion
        #region GroupID
        public abstract class groupID : PX.Data.BQL.BqlString.Field<groupID> { }
        protected String _GroupID;
        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDBDefault(typeof(BOQGroup.groupID))]
        [PXParent(typeof(FK.MasterRec))]
        [PXUIField(DisplayName = "Group ID", Visibility = PXUIVisibility.Invisible)]
        [PXReferentialIntegrityCheck]
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

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(BOQGroup.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region InventoryID
        [PXDBInt]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXSelector(typeof(Search<PX.Objects.IN.InventoryItem.inventoryID>), SubstituteKey = (typeof(PX.Objects.IN.InventoryItem.inventoryCD)), DescriptionField = typeof(PX.Objects.IN.InventoryItem.descr))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
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