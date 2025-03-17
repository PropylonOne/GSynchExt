using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt
{
    public static class Type
    {
        //Site Types
        public const string School                         = "S";
        public const string Hospital                       = "H";
        public const string PoliceStation                  = "P";
        public const string Stadium                        = "S";
        public const string Other                          = "O";
    }
    public static class DUtility
    {
        //Utility
        public const string CEB                            = "CEB";
        public const string LECO                           = "LECO";
    }
    public static class Status
    {
        //Status
        public const string Initial                        = "_";
        public const string Planned                        = "H";
        public const string OnHold                         = "OH";
        public const string UnderSurvey                    = "US";
        public const string PendingApproval                = "PA";
        public const string Approved                       = "AP";
        public const string Completed                      = "CMP";
        public const string Active                         = "AC";
        public const string Rejected                       = "RE";
        public const string InService                      = "IS";
        public const string Constructed                    = "CON";
        public const string SiteSelected                   = "IC";
        public const string Commissioned                   = "CM";
        public const string ConnectedToGrid                = "CTG";
        public const string Cancelled                      = "C";
        public const string Suspended                      = "S";
        public const string Closed                         = "CL";
        public const string Archived                       = "ARC";

        //Rev Status
        public const string Released                       = "RL";
        public class SSListAttribute : PXStringListAttribute
        {
            public SSListAttribute() : base(GetSSStatuses)
            {

            }
            public static (string, string)[] GetSSStatuses => new[] {
                (Planned, Messages.Planned),
                (UnderSurvey, Messages.UnderSurvey),
                (InService, Messages.InService),
                (Constructed, Messages.Constructed),
                (SiteSelected, Messages.SiteSelected),
                (Commissioned, Messages.Commissioned),
                (ConnectedToGrid, Messages.ConnectedToGrid),
                (Completed, Messages.Completed),
                (Suspended, Messages.Suspended),
                (Cancelled, Messages.Cancelled)
            };
        }
        public static class Desc
        {
            public static string Planned                   => Messages.Planned;
            public static string UnderSurvey               => Messages.UnderSurvey;
            public static string Constructed               => Messages.Constructed;
            public static string SiteSelected              => Messages.SiteSelected;
            public static string Commissioned              => Messages.Commissioned;
            public static string ConnectedToGrid           => Messages.ConnectedToGrid;
            public static string Completed                 => Messages.Completed;
            public static string Cancelled                 => Messages.Cancelled;
            public static string Suspended                 => Messages.Suspended;
            public static string InService                 => Messages.InService;
        }
        public static string GetStatusDescription(string statusID)
        {
            switch (statusID)
            {
                case Planned:
                    return Desc.Planned;
                case Released:
                    return Desc.UnderSurvey;
                case Constructed:
                    return Desc.Constructed;
                case SiteSelected:
                    return Desc.SiteSelected;
                case Commissioned:
                    return Desc.Commissioned;
                case ConnectedToGrid:
                    return Desc.ConnectedToGrid;
                case Completed:
                    return Desc.Completed;
                case Cancelled:
                    return Desc.Cancelled;
                case InService:
                    return Desc.InService;
                case Suspended:
                    return Desc.Suspended;
            }

            return null;
        }
        public class completed : PX.Data.BQL.BqlString.Constant<completed>
        {
            public completed() : base(Completed) {; }
        }
        public class active : PX.Data.BQL.BqlString.Constant<active>
        {
            public active() : base(Active) {; }
        }
        public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
        {
            public rejected() : base(Rejected) {; }
        }
        public class planned : PX.Data.BQL.BqlString.Constant<planned>
        {
            public planned() : base(Planned) {; }
        }
        public class underSurvey : PX.Data.BQL.BqlString.Constant<underSurvey>
        {
            public underSurvey() : base(UnderSurvey) {; }
        }
        public class siteSelected : PX.Data.BQL.BqlString.Constant<siteSelected>
        {
            public siteSelected() : base(SiteSelected) {; }
        }
        public class constructed : PX.Data.BQL.BqlString.Constant<constructed>
        {
            public constructed() : base(Constructed) {; }
        }
        public class commissioned : PX.Data.BQL.BqlString.Constant<commissioned>
        {
            public commissioned() : base(Commissioned) {; }
        }
        public class connectedToGrid : PX.Data.BQL.BqlString.Constant<connectedToGrid>
        {
            public connectedToGrid() : base(ConnectedToGrid) {; }
        }
        public class cancelled : PX.Data.BQL.BqlString.Constant<cancelled>
        {
            public cancelled() : base(Cancelled) {; }
        }

        public class closed : PX.Data.BQL.BqlString.Constant<closed>
        {
            public closed() : base(Closed) {; }
        }
        public class suspended : PX.Data.BQL.BqlString.Constant<suspended>
        {
            public suspended() : base(Suspended) {; }
        }
        public class inService : PX.Data.BQL.BqlString.Constant<inService>
        {
            public inService() : base(InService) {; }
        }
    }
    public static class FTRStatus
    {
        public const string Planning                       = "In Planning";
        public const string Released                       = "Released";
        public const string Cancelled                      = "Cancelled";
        public const string Closed                         = "Closed";
        public const string Archived                       = "Archived";
        public const string OnHold                         = "OnHold";
        public const string Rejected                       = "Rejected";
        public const string PendingApproval                = "Pending Approval";
    }
    public static class FARequestStatus
    {
        public const string OnHold                         = "O";
        public const string PendingApproval                = "P";
        public const string Open                           = "N";
        public const string Rejected                       = "J";
        public const string Closed                         = "C";
    }
    public static class FARequestActionType
    {
        public const string Issue                          = "I";
        public const string Return                         = "T";
    }
    public static class ReqType
    {
        public const string FundTransfer                   = "Fund Transfer";
        public const string MaterialRequest                = "Material Request";
        public const string MaterialRequestServices        = "Material Request Services";
        public const string ToolRequest                    = "T";
    }
    /// <summary>
    /// Non stock item prefixes.
    /// </summary>
    public static class ItemPrefix
    {
        public const string SSItem                         = "SS-";
        public const string RRItem                         = "RR-";
    }
}