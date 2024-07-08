using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.FS;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using static PX.Data.PXGenericInqGrph;
using PX.Objects.AP;
using static PX.Data.BQL.BqlPlaceholder;

namespace GSynchExt
{
    [Serializable]
    [PXCacheName("SolarRevGen")]
    [PXPrimaryGraph(typeof(SolarRevGenEntry))]
    public class SolarRevGen : PXBqlTable, IBqlTable
    {

        #region Keys
        public class UK : PrimaryKeyOf<SolarRevGen>.By<solarRevGenID>
        {
            public static SolarRevGen Find(PXGraph graph, string solarRevGenID) => FindBy(graph, solarRevGenID);
        }

        public class UK2 : PrimaryKeyOf<SolarRevGen>.By<period, province>
        {
            public static SolarRevGen Find(PXGraph graph, string period, string province) => FindBy(graph, period, province);
        }

        public static class FK
        {
            public class SsInvoice : PX.Objects.AR.ARInvoice.PK.ForeignKeyOf<SolarRevGen>.By<docType,ssrefnbr> { }
            public class RrInvoice : PX.Objects.AP.APInvoice.PK.ForeignKeyOf<SolarRevGen>.By<docType,rrrefnbr> { }
            public class SbInvoice : PX.Objects.AR.ARInvoice.PK.ForeignKeyOf<SolarRevGen>.By<docType,siteBillRefNbr> { }

            public class RrInvoice2 : PrimaryKeyOf<PX.Objects.AP.APInvoice>.By<PX.Objects.AP.APInvoice.docType, PX.Objects.AP.APInvoice.refNbr>.ForeignKeyOf<SolarRevGen>.By<docType, rrrefnbr>
            {
            }
        }
        #endregion



        #region SolarRevGenID
        [PXDBString(30, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Solar Rev. Gen. ID")]
        [PXSelector(typeof(Search<SolarRevGen.solarRevGenID>), typeof(SolarRevGen.solarRevGenID))]
        [AutoNumber(typeof(SiteSetup.revGenNumberigID), typeof(SolarRevGen.createdDateTime))]
        public virtual string SolarRevGenID { get; set; }
        public abstract class solarRevGenID : PX.Data.BQL.BqlString.Field<solarRevGenID> { }
        #endregion

        #region Province
        [PXDBString(50, IsUnicode = true, InputMask = "CCC")]
        [PXSelector(typeof(Search<State.stateID, Where<State.countryID, Equal<Provinces.CountryLK>>>))]
        [PXUIField(DisplayName = "Province", Required = true)]

        public virtual string Province { get; set; }
        public abstract class province : PX.Data.BQL.BqlString.Field<province> { }
        #endregion
        #region BranchID
        public new abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        [Branch(typeof(AccessInfo.branchID), IsDetail = false, TabOrder = 0)]
        public virtual Int32? BranchID
        {
            get;
            set;
        }
        #endregion
        #region InvoiceDate
        [PXDBDate()]
        /*[PXDefault(typeof(Search<OrganizationFinPeriod.endDate,
              Where<OrganizationFinPeriod.finPeriodID, Equal<Current<SolarRevGen.period>>,
                  And<OrganizationFinPeriod.organizationID, Equal<Current<SolarRevGen.branchID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]*/
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Invoice Date", Required = true)]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion
        #region Period
        [AROpenPeriod(
            typeof(SolarRevGen.invoiceDate),
            branchSourceType: typeof(SolarRevGen.branchID),
            IsHeader = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Post Period", Enabled = false, Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        public virtual string Period { get; set; }
        public abstract class period : PX.Data.BQL.BqlString.Field<period> { }

        #endregion



        #region CustomerID

        [PXUIField(DisplayName = "Customer", Visibility = PXUIVisibility.Visible, Required = true)]
        [CustomerActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true, TabOrder = 2)]
        public virtual Int32? CustomerID { get; set; }
        [PXDefault()]
        public new abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }

        #endregion

        #region OnHold
        [PXDBBool()]
        [PXUIField(DisplayName = "On Hold")]
        public virtual bool? OnHold { get; set; }
        public abstract class onHold : PX.Data.BQL.BqlBool.Field<onHold> { }
        #endregion

        #region Status
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXStringList(
         new string[]
         {
        GSynchExt.Status.OnHold ,
        GSynchExt.Status.Released ,

         },
         new string[]
         {
        GSynchExt.Messages.OnHold,
        GSynchExt.Messages.Released,


         })]
        [PXUIField(DisplayName = "Status")]

        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion


        #region TransDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Trans Date", Enabled = false)]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? TransDate { get; set; }
        public abstract class transDate : PX.Data.BQL.BqlDateTime.Field<transDate> { }
        #endregion

        #region LnCntrl
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Ln Cntrl")]
        public virtual int? LnCntrl { get; set; }
        public abstract class lnCntrl : PX.Data.BQL.BqlInt.Field<lnCntrl> { }
        #endregion

        #region Ssrefnbr
        [PXDBString]
        [PXUIField(DisplayName = "Solar Sales Ref. Nbr.", Enabled = false)]
     //   [PXForeignReference(typeof(PX.Objects.AR.ARInvoice.PK.ForeignKeyOf<SolarRevGen>.By<docType, ssrefnbr>))]
        public virtual String Ssrefnbr { get; set; }
        public abstract class ssrefnbr : PX.Data.BQL.BqlString.Field<ssrefnbr> { }
        #endregion

        #region Mfrefnbr
        [PXDBString]
        [PXUIField(DisplayName = "Mngt. Fee Ref. Nbr.", Enabled = false)]
      //  [PXSelector(typeof(Search<ARInvoice.invoiceNbr>), typeof(ARInvoice.invoiceNbr))]

        public virtual String Mfrefnbr { get; set; }
        public abstract class mfrefnbr : PX.Data.BQL.BqlString.Field<mfrefnbr> { }
        #endregion

        #region SiteBillRefNbr
        [PXDBString]
        [PXUIField(DisplayName = "Site Bill Ref. Nbr.", Enabled = false)]
        //  [PXSelector(typeof(Search<ARInvoice.invoiceNbr>), typeof(ARInvoice.invoiceNbr))]
     //   [PXForeignReference(typeof(PX.Objects.AR.ARInvoice.PK.ForeignKeyOf<SolarRevGen>.By<docType, siteBillRefNbr>))]

        public virtual String SiteBillRefNbr { get; set; }
        public abstract class siteBillRefNbr : PX.Data.BQL.BqlString.Field<siteBillRefNbr> { }
        #endregion

        #region Rrrefnbr
        [PXDBString]
        [PXUIField(DisplayName = "Roof Rental Ref. Nbr", Enabled = false)]
        // [PXSelector(typeof(Search<ARInvoice.invoiceNbr>), typeof(ARInvoice.invoiceNbr))]
        public virtual String Rrrefnbr { get; set; }
        public abstract class rrrefnbr : PX.Data.BQL.BqlString.Field<rrrefnbr> { }

        #endregion
        #region DocType


        public abstract class docType : PX.Data.BQL.BqlString.Field<docType>
        {
        }

        [PXDBString]
        [PXUIField(DisplayName = "Doc. Type")]
        public virtual string DocType { get; set; }

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