using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects.SM;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System;
using PX.Objects.PM;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;

namespace PX.Objects.RQ
{
  public class RQRequestGSExt : PXCacheExtension<PX.Objects.RQ.RQRequest>
  {

        public class FK
        {
            public class Project : PMProject.PK.ForeignKeyOf<RQRequest>.By<usrProjectID> { }

        }

        #region UsrProjectID
         public abstract class usrProjectID : PX.Data.BQL.BqlInt.Field<usrProjectID>
        {
        }
        protected Int32? _UsrProjectID;
        [PXDBInt]
        [PXUIField(DisplayName = "Project Ref.")]
        [PXSelector(typeof(Search<PMProject.contractID>), SubstituteKey = (typeof(PMProject.contractCD)))]
        public virtual Int32? UsrProjectID
        {
            get
            {
                return this._UsrProjectID;
            }
            set
            {
                this._UsrProjectID = value;
            }
        }
        #endregion

        #region UsrMassSubItem
        [PXDBInt]
        [PXUIField(DisplayName = "Mass Sub Acct")]
        [PXSelector(typeof(SelectFrom<Sub>.Where<Sub.active.IsEqual<True>>.SearchFor<Sub.subID>),
            new Type[]
            {
                typeof(Sub.subCD),
                typeof(Sub.description)
            },
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description)
            )]

        public virtual int? UsrMassSubItem { get; set; }
        public abstract class usrMassSubItem : PX.Data.BQL.BqlInt.Field<usrMassSubItem> { }
        #endregion
  }
}