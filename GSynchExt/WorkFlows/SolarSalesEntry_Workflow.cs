using GSynchExt;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.SolarRevGenEntry, GSynchExt.SolarRevGen>;
namespace GSynchExt.Workflows
{
    public class SolarRevGenWorkflow : PX.Data.PXGraphExtension<SolarRevGenEntry>
    {
        #region Constants
        public static class States
        {
            public const string OnHold = Status.OnHold;
            public const string Released = Status.Released;

            public class onHold : PX.Data.BQL.BqlString.Constant<onHold>
            {
                public onHold() : base(OnHold) { }
            }
            public class released : PX.Data.BQL.BqlString.Constant<released>
            {
                public released() : base(Released) { }
            }
        }
        #endregion


        public override void Configure(PXScreenConfiguration config)
        {

            var context = config.GetScreenConfigurationContext<SolarRevGenEntry, SolarRevGen>();

            #region Categories
            var commonCategories = CommonActionCategories.Get(context);
            var processingCategory = commonCategories.Processing;
            #endregion

            context.AddScreenConfigurationFor(screen =>
                     screen
                     .StateIdentifierIs<SolarRevGen.status>()
                     .AddDefaultFlow(flow => flow
                     .WithFlowStates(fss =>
                     {
                         fss.Add<States.onHold>(flowState =>
                         {
                             return flowState
                             .IsInitial()

                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.Release, a => a
                                 .IsDuplicatedInToolbar()
                                 .WithConnotation(ActionConnotation.Success));

                             });
                         });
                         fss.Add<States.released>(flowState =>
                         {
                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.Hold, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                                  
                              }
                             );

                         });
                     })
                     .WithTransitions(transitions =>
                     {
                         transitions.AddGroupFrom<States.onHold>(ts =>
                         {
                             ts.Add(t => t.To<States.released>()
                             .IsTriggeredOn(g => g.Release));


                         });

                         transitions.AddGroupFrom<States.released>(ts =>
                         {

                             ts.Add(t => t.To<States.onHold>()
                             .IsTriggeredOn(g => g.Hold));
                         });

                     }))
                     .WithCategories(categories =>
                     {
                         categories.Add(processingCategory);
                     }).WithActions(actions =>
                     {
                         actions.Add(g => g.Release, c => c
                             .WithCategory(processingCategory));
                         actions.Add(g => g.Hold, c => c
                            .WithCategory(processingCategory));

                     }));
        }
    }
}