using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;

namespace GSynchExt
{
    public class CEBLocations : PXBqlTable, IBqlTable
    {

        #region StateID
        public abstract class stateID : PX.Data.BQL.BqlString.Field<stateID> { }
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDBDefault(typeof(Provinces.stateID))]
        [PXParent(typeof(SelectFrom<Provinces>.Where<Provinces.stateID.IsEqual<CEBLocations.stateID.FromCurrent>>))]
        [PXUIField(DisplayName = "Province ", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
        //[PXReferentialIntegrityCheck]
        public virtual String StateID
        {
            get;
            set;
        }
        #endregion



        #region LocationID
        [PXDBString(6, IsKey = true, IsUnicode = true, InputMask = ">CCCCCC")]
        [PXUIField(DisplayName = "Location ID")]
     //   [PXReferentialIntegrityCheck]
        public virtual string LocationID { get; set; }
        public abstract class locationID : PX.Data.BQL.BqlString.Field<locationID> { }
        #endregion


        #region LocationType
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXStringList(
         new string[]
         {
             GSynchExt.DUtility.CEB,
             GSynchExt.DUtility.LECO

         },
         new string[]
         {
             GSynchExt.Messages.CEB,
             GSynchExt.Messages.LECO

         })]
        [PXDefault(GSynchExt.DUtility.CEB)]
        [PXUIField(DisplayName = "Location Type", Enabled = false)]

        // [PXSelector(typeof(Search<CSAttributeDetail.description, Where<CSAttributeDetail.attributeID, Equal<attributeDistrUtil>>>))]
        public virtual string LocationType { get; set; }
        public abstract class locationType : PX.Data.BQL.BqlString.Field<locationType> { }
        #endregion

        #region Description
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
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
        public class CountryLK : PX.Data.BQL.BqlString.Constant<CountryLK> { public CountryLK() : base("LK") { } }
    }
}