using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CN.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GSynchExt.ISGAFARequest;
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.ISGAFARequestEntry, GSynchExt.ISGAFARequest>;

namespace GSynchExt.WorkFlows
{
    public class ISGAFARequestEntry_WorkFlow : PX.Data.PXGraphExtension<ISGAFARequestEntry>
    {
        #region Constants
        public static class States
        {
            public const string OnHold          = FARequestStatus.OnHold;
            public const string PendingApproval = FARequestStatus.PendingApproval;
            public const string Open            = FARequestStatus.Open;
            public const string Rejected        = FARequestStatus.Rejected;
            public const string Closed          = FARequestStatus.Closed;
            public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
            {
                public onHold() : base(OnHold) { }
            }
            public class open : PX.Data.BQL.BqlString.Constant<open>
            {
                public open() : base(Open) { }
            }
            public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval>
            {
                public pendingApproval() : base(PendingApproval) { }
            }
            public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
            {
                public rejected() : base(Rejected) { }
            }
            public class closed : PX.Data.BQL.BqlString.Constant<closed>
            {
                public closed() : base(Closed) { }
            }
        }
        #endregion
        public override void Configure(PXScreenConfiguration config)
        {
            var context = config.GetScreenConfigurationContext<ISGAFARequestEntry, ISGAFARequest>();
            #region Categories
            var processingCategory = context.Categories.CreateNew(GSynchExt.Messages.CategoryID.Processing,
                category => category.DisplayName(  GSynchExt.Messages.CategoryNames.Processing));
            var approvalCategory = context.Categories.CreateNew(GSynchExt.Messages.CategoryID.Approval,
                category => category.DisplayName(GSynchExt.Messages.CategoryNames.Approval));
            #endregion
            #region Conditions
            Condition Bql<T>() where T : IBqlUnary, new() => context.Conditions.FromBql<T>();
            var conditions = new
            {
                IsApproved
                    = Bql<approved.IsEqual<True>>()
            }.AutoNameConditions();
            #endregion
            context.AddScreenConfigurationFor(screen =>
                     screen
                     .StateIdentifierIs<ISGAFARequest.status>()
                     .AddDefaultFlow(flow => flow
                     .WithFlowStates(fss =>
                     {
                         fss.Add<States.onHold>(flowState =>
                         {
                             return flowState
                             .IsInitial()
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.removeHold, a => a
                                 .IsDuplicatedInToolbar()
                                 .WithConnotation(ActionConnotation.Success));    
                             });
                         });
                         fss.Add<States.open>(flowState =>
                         {
                             return flowState
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.close, a => a.IsDuplicatedInToolbar());
                                 actions.Add(g => g.hold, a => a.IsDuplicatedInToolbar());                          
                             });
                         });
                         fss.Add<States.closed>(flowState =>
                         {
                             return flowState
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.reopen, a => a.IsDuplicatedInToolbar());                         
                             });
                         });
                     })
                     .WithTransitions(transitions =>
                     {
                         transitions.AddGroupFrom <States.onHold>(ts =>
                         {
                             ts.Add(t => t.To<States.open>()
                             .IsTriggeredOn(g => g.removeHold).When(conditions.IsApproved));
                         });
                         transitions.AddGroupFrom<States.open>(ts =>
                         {
                             ts.Add(t => t.To<States.onHold>().IsTriggeredOn(g => g.hold));
                             ts.Add(t => t.To<States.closed>().IsTriggeredOn(g => g.close));
                         });
                         transitions.AddGroupFrom<States.closed>(ts =>
                         {
                             ts.Add(t => t.To<States.open>().IsTriggeredOn(g => g.reopen));
                         });
                     }))
                     .WithCategories(categories =>
                     {
                         categories.Add(processingCategory);
                     })
                     .WithActions(actions =>
                     {
                         actions.Add(g => g.removeHold, c => c
                          .WithCategory(processingCategory));
                         actions.Add(g => g.hold, c => c
                          .WithCategory(processingCategory));
                         actions.Add(g => g.close, c => c
                          .WithCategory(processingCategory));
                         actions.Add(g => g.reopen, c => c
                          .WithCategory(processingCategory));
                     }));
        }
    }
}













    

