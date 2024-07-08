using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.FA;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("RRRates")]
  public class RRRates : PXBqlTable, IBqlTable
  {
        #region Province
        [PXDBString(50, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXSelector(typeof(Search<State.stateID, Where<State.countryID, Equal<Provinces.CountryLK>>>))]
        [PXUIField(DisplayName = "Province")]
        public virtual string Province { get; set; }
        public abstract class province : PX.Data.BQL.BqlString.Field<province> { }
        #endregion

        #region ExpDate
        [PXDBDate(MinValue = "2000-01-01", IsKey = true)]
        [PXUIField(DisplayName = "Exp. Date")]
        public virtual DateTime? ExpDate { get; set; }
        public abstract class expDate : PX.Data.BQL.BqlDateTime.Field<expDate> { }
        #endregion

        #region Rrrate
        [PXDBDecimal(MinValue = 0, MaxValue = 100)]
       // [PXDBDecimal]
        [PXUIField(DisplayName = "Roof Rental Rate %")]
        public virtual Decimal? Rrrate { get; set; }
        public abstract class rrrate : PX.Data.BQL.BqlDecimal.Field<rrrate> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
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
        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
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
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
  }
}