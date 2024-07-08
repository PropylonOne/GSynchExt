using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;

namespace GSynchExt
{
  [Serializable]
  [PXCacheName("Cluster")]
  public class Cluster : PXBqlTable, IBqlTable
  {
    #region StateID
        public abstract class stateID : PX.Data.BQL.BqlString.Field<stateID> { }
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDBDefault(typeof(Provinces.stateID))]
        [PXParent(typeof(SelectFrom<Provinces>.Where<Provinces.stateID.IsEqual<Cluster.stateID.FromCurrent>>))]
        [PXUIField(DisplayName = "Province ", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
        [PXReferentialIntegrityCheck]
        public virtual String StateID
        {
            get;
            set;
        }
        #endregion

    #region ClusterID
    [PXDBString(3, IsKey = true, IsUnicode = true, InputMask = ">CCC")]
    [PXUIField(DisplayName = "Cluster ID")]
   // [PXReferentialIntegrityCheck]
        public virtual string ClusterID { get; set; }
    public abstract class clusterID : PX.Data.BQL.BqlString.Field<clusterID> { }
    #endregion

    #region Description
    [PXDBString(50, IsUnicode = true, InputMask = "")]
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
  }
}