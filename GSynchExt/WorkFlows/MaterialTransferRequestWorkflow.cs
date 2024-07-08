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
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.MaterialTransferRequestEntry, GSynchExt.MaterialTransferRequest>;

namespace GSynchExt.WorkFlows
{
    public class MaterialTransferRequestWorkflow : PX.Data.PXGraphExtension<MaterialTransferRequestEntry>
    {
        #region Constants
        public static class States
        {
            public const string OnHold = FundTransferStatus.OnHold;
            public const string Released = FundTransferStatus.Released;
            public const string Cancelled = FundTransferStatus.Cancelled;
            public const string Closed = FundTransferStatus.Closed;
 
           

            public class released : PX.Data.BQL.BqlString.Constant<released>
            {
                public released() : base(Released) { }
            }

            public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
            {
                public onHold() : base(OnHold) { }
            }


            public class cancelled : PX.Data.BQL.BqlString.Constant<cancelled>
            {
                public cancelled() : base(Cancelled) { }
            }

            public class closed : PX.Data.BQL.BqlString.Constant<closed>
            {
                public closed() : base(Closed) { }
            }
        }
        #endregion

        public override void Configure(PXScreenConfiguration config)
        {

            var context = config.GetScreenConfigurationContext<MaterialTransferRequestEntry, MaterialTransferRequest>();

            #region Categories
            var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
                category => category.DisplayName(CategoryNames.Processing));
          
            #endregion

            

            context.AddScreenConfigurationFor(screen =>
                     screen
                     .StateIdentifierIs<MaterialTransferRequest.status>()
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
                                 actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                 actions.Add(g => g.Close, a => a.IsDuplicatedInToolbar());                             
                             });
                         });
                         fss.Add<States.cancelled>();
                         fss.Add<States.closed>(flowState =>
                         {
                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.Reverse, a => a.IsDuplicatedInToolbar());

                              });
                         });
                     })
                     .WithTransitions(transitions =>
                     {
                         transitions.AddGroupFrom <States.onHold>(ts =>
                         {
                             ts.Add(t => t.To<States.released>()
                             .IsTriggeredOn(g => g.RemoveHold));

                         });
                         transitions.AddGroupFrom<States.released>(ts =>
                         {
                             ts.Add(t => t.To<States.onHold>()
                           .IsTriggeredOn(g => g.Hold2));
                             ts.Add(t => t.To<States.cancelled>()
                          .IsTriggeredOn(g => g.Cancel2));
                         });
                         transitions.AddGroupFrom<States.closed>(ts =>
                         {
                             ts.Add(t => t.To<States.released>()
                           .IsTriggeredOn(g => g.Reverse));
                             
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
                         actions.Add(g => g.Cancel2, c => c
                            .WithCategory(processingCategory));

                     }));
        }
        public static class CategoryNames
        {
            public const string Processing = "Processing";
        }
        public static class CategoryID
        {
            public const string Processing = "Processing";
        }
    }
}













    

