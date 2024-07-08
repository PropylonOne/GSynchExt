using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects;
using PX.Objects.CS;
using PX.Objects.EP;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("BOQ Setup")]
    [PXPrimaryGraph(typeof(GSBOQMaint))]
    public class GSBOQSetup : PXBqlTable, IBqlTable
    {
        #region NumberingID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "BOQ Numbering Sequence")]
        [PXDefault("GSBOQ")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string NumberingID { get; set; }
        public abstract class numberingID : PX.Data.BQL.BqlString.Field<numberingID> { }
        #endregion
    
        public abstract class approvalMap : BqlBool.Field<approvalMap>
        {
        }
        [EPRequireApproval]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "Require Approval")]
        public virtual bool? ApprovalMap { get; set; }
        

        #region Minuom
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Min UOM")]
        public virtual string Minuom { get; set; }
        public abstract class minuom : PX.Data.BQL.BqlString.Field<minuom> { }
        #endregion

        #region RevisionID
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Default Revision ID")]
        public virtual string RevisionID
        {
            get;
            set;
        }
        #endregion

        #region Maxuom
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Max UOM")]
        public virtual string Maxuom { get; set; }
        public abstract class maxuom : PX.Data.BQL.BqlString.Field<maxuom> { }
        #endregion

        #region MaxCapacity
        [PXDBInt()]
        [PXUIField(DisplayName = "Max Capacity")]
        public virtual int? MaxCapacity { get; set; }
        public abstract class maxCapacity : PX.Data.BQL.BqlInt.Field<maxCapacity> { }
        #endregion

        #region MinCapacity
        [PXDBInt()]
        [PXUIField(DisplayName = "Min Capacity")]
        public virtual int? MinCapacity { get; set; }
        public abstract class minCapacity : PX.Data.BQL.BqlInt.Field<minCapacity> { }
        #endregion

        #region MinUnitConvertion
        [PXDBInt()]
        [PXUIField(DisplayName = "Min Unit Convertion")]
        public virtual int? MinUnitConvertion { get; set; }
        public abstract class minUnitConvertion : PX.Data.BQL.BqlInt.Field<minUnitConvertion> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedByID
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        protected Guid? _CreatedByID;
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID
        {
            get
            {
                return this._CreatedByID;
            }
            set
            {
                this._CreatedByID = value;
            }
        }
        #endregion

        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        protected DateTime? _LastModifiedDateTime;
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime
        {
            get
            {
                return this._LastModifiedDateTime;
            }
            set
            {
                this._LastModifiedDateTime = value;
            }
        }
        #endregion

        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        protected String _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID()]
        public virtual String LastModifiedByScreenID
        {
            get
            {
                return this._LastModifiedByScreenID;
            }
            set
            {
                this._LastModifiedByScreenID = value;
            }
        }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        protected Guid? _LastModifiedByID;
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID
        {
            get
            {
                return this._LastModifiedByID;
            }
            set
            {
                this._LastModifiedByID = value;
            }
        }
        #endregion

        #region Tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        protected Byte[] _tstamp;
        [PXDBTimestamp()]
        public virtual Byte[] tstamp
        {
            get
            {
                return this._tstamp;
            }
            set
            {
                this._tstamp = value;
            }
        }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }
}