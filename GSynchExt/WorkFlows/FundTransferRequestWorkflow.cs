using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CN.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GSynchExt.FundTransferRequest;
using static GSynchExt.Messages;
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.FundTransferRequestEntry, GSynchExt.FundTransferRequest>;

namespace GSynchExt.WorkFlows
{
    public class FundTransferRequestWorkflow : PX.Data.PXGraphExtension<FundTransferRequestEntry>
    {
        #region Constants
        public static class States
        {
            public const string OnHold = FundTransferStatus.OnHold;
            public const string Rejected = FundTransferStatus.Rejected;
            public const string PendingApproval = FundTransferStatus.PendingApproval;
            public const string Released = FundTransferStatus.Released;
            public const string Closed = FundTransferStatus.Closed;
           

            public class released : PX.Data.BQL.BqlString.Constant<released>
            {
                public released() : base(Released) { }
            }
            public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval>
            {
                public pendingApproval() : base(PendingApproval) { }
            }

            public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
            {
                public onHold() : base(OnHold) { }
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

            var context = config.GetScreenConfigurationContext<FundTransferRequestEntry, FundTransferRequest>();

            #region Categories
            var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
                category => category.DisplayName(CategoryNames.Processing));
            var approvalCategory = context.Categories.CreateNew(CategoryID.Approval,
                category => category.DisplayName(CategoryNames.Approval));
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
                     .StateIdentifierIs<FundTransferRequest.status>()
                     .AddDefaultFlow(flow => flow
                     .WithFlowStates(fss =>
                     {
                         fss.Add<States.onHold>(flowState =>
                         {
                             return flowState
                             .IsInitial()
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.RemoveHold, a => a
                                 .IsDuplicatedInToolbar()
                                 .WithConnotation(ActionConnotation.Success));
                                
                             });
                         });
                         fss.Add<States.released>(flowState =>
                         {
                             return flowState
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.Hold2, a => a.IsDuplicatedInToolbar());
                                 actions.Add(g => g.Close, a => a.IsDuplicatedInToolbar());                             
                             });
                         });
                         fss.Add<States.closed>();

                     })
                     .WithTransitions(transitions =>
                     {
                         transitions.AddGroupFrom <States.onHold>(ts =>
                         {
                             ts.Add(t => t.To<States.released>()
                             .IsTriggeredOn(g => g.RemoveHold).When(conditions.IsApproved));

                         });
                         transitions.AddGroupFrom<States.released>(ts =>
                         {
                            ts.Add(t => t.To<States.onHold>()
                           .IsTriggeredOn(g => g.Hold2));
                             ts.Add(t => t.To<States.closed>()
                         .IsTriggeredOn(g => g.Close));
                         });
                     }))
                     .WithCategories(categories =>
                     {
                         categories.Add(processingCategory);
                     })
                     .WithActions(actions =>
                     {
                         actions.Add(g => g.RemoveHold, c => c
                            .WithCategory(processingCategory));
                         actions.Add(g => g.Hold2, c => c
                             .WithCategory(processingCategory));
                         actions.Add(g => g.Close, c => c
                             .WithCategory(processingCategory));

                     }));
        }
        public static class CategoryNames
        {
            public const string Processing = "Processing";
            public const string Approval = "Approval";
        }
        public static class CategoryID
        {
            public const string Processing = "Processing";
            public const string Approval = "Approval";
        }
    }
}













    

