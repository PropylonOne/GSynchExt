using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
    public static class MessagesBOQ
    {

        public const string Planning = "In Planning";
        public const string OnHold = "On Hold";
        public const string Archived = "Archived";
        public const string Active = "Active";
        public const string PendingApproval = "Pending Approval";
        public const string Rejected = "Rejected";

        public const string Phase1 = "P01";
        public const string Phase2 = "P02";
        public const string Phase3 = "P03";
        public const string Phase4 = "P04";
        public const string Phase5 = "P05";
        public const string Phase6 = "P06";
        public const string Phase7 = "P07";
        public const string Phase8 = "P08";
        public const string Phase9 = "P09";
        public const string Phase10 = "P10";

    }
    public static class BOQStatus
    {

        public const string Planning = "In Planning";
        public const string Active = "Active";
        public const string Archived = "Archived";
        public const string OnHold = "OnHold";
        public const string Rejected = "Rejected";
        public const string PendingApproval = "Pending Approval";

        public class active : PX.Data.BQL.BqlString.Constant<active>
        {
            public active() : base(Active) {; }
        }

        public class archived : PX.Data.BQL.BqlString.Constant<archived>
        {
            public archived() : base(Archived) {; }
        }
        public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
        {
            public onHold() : base(OnHold) {; }
        }

        public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval>
        {
            public pendingApproval() : base(PendingApproval) {; }
        }

        public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
        {
            public rejected() : base(Rejected) {; }
        }
    }

    public static class Phases
    {

        public const string Phase1 = "P01";
        public const string Phase2 = "P02";
        public const string Phase3 = "P03";
        public const string Phase4 = "P04";
        public const string Phase5 = "P05";
        public const string Phase6 = "P06";
        public const string Phase7 = "P07";
        public const string Phase8 = "P08";
        public const string Phase9 = "P09";
        public const string Phase10 = "P10";


    }

}
