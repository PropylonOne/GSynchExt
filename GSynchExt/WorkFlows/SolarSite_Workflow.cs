using GSynchExt;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;
using static GSynchExt.SolarSite;
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.SolarSiteEntry, GSynchExt.SolarSite>;
namespace GSynchExt.Workflows
{
    public class SolarSite_Workflow : PX.Data.PXGraphExtension<SolarSiteEntry>
    {
        #region Constants
        public static class States
        {
          
            public const string Planned = Status.Planned;
            public const string Rejected = Status.Rejected;
            public const string UnderSurvey = Status.UnderSurvey;
            public const string SiteSelected = Status.SiteSelected;
            public const string Commissioned = Status.Commissioned;
            public const string ConnectedToGrid = Status.ConnectedToGrid;
            public const string Cancelled = Status.Cancelled;
            public const string Constructed = Status.Constructed;
            public const string InService = Status.InService;
            public const string Suspended = Status.Suspended;

            public class suspended : PX.Data.BQL.BqlString.Constant<suspended>
            {
                public suspended() : base(Suspended) { }
            }
            public class planned : PX.Data.BQL.BqlString.Constant<planned>
            {
                public planned() : base(Planned) { }
            }
            public class constructed : PX.Data.BQL.BqlString.Constant<constructed>
            {
                public constructed() : base(Constructed) { }
            }
            public class inService : PX.Data.BQL.BqlString.Constant<inService>
            {
                public inService() : base(InService) { }
            }
            public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
            {
                public rejected() : base(Rejected) { }
            }
            public class underSurvey : PX.Data.BQL.BqlString.Constant<underSurvey>
            {
                public underSurvey() : base(UnderSurvey) { }
            }
            public class siteSelected : PX.Data.BQL.BqlString.Constant<siteSelected>
            {
                public siteSelected() : base(SiteSelected) { }
            }
            public class commissioned : PX.Data.BQL.BqlString.Constant<commissioned>
            {
                public commissioned() : base(Commissioned) { }
            }
            public class connectedToGrid : PX.Data.BQL.BqlString.Constant<connectedToGrid>
            {
                public connectedToGrid() : base(ConnectedToGrid) { }
            }
            public class cancelled : PX.Data.BQL.BqlString.Constant<cancelled>
            {
                public cancelled() : base(Cancelled) { }
            }
        }
        #endregion

        public override void Configure(PXScreenConfiguration config)
        {

            var context = config.GetScreenConfigurationContext<SolarSiteEntry, SolarSite>();

            #region Categories
            var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
                category => category.DisplayName(CategoryNames.Processing));
            var approvalCategory = context.Categories.CreateNew(CategoryID.Approval,
                category => category.DisplayName(CategoryNames.Approval));
            var OtherCategory = context.Categories.CreateNew(CategoryID.Other,
             category => category.DisplayName(CategoryNames.Other));
            #endregion

            context.AddScreenConfigurationFor(screen =>
                     screen
                     .StateIdentifierIs<SolarSite.siteStatus>()
                     .AddDefaultFlow(flow => flow
                     .WithFlowStates(fss =>
                     {
                         fss.Add<States.planned>(flowState =>
                         {
                             return flowState
                             .IsInitial()
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.ActivateSurvey, a => a
                                 .IsDuplicatedInToolbar()
                                 .WithConnotation(ActionConnotation.Success));

                                 actions.Add(g => g.SelectSite, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Secondary));
                                 actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                 actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());

                             });
                         });
                         fss.Add<States.underSurvey>(flowState =>
                         {
                             return flowState
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.SelectSite, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                                 actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                 actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());

                             });                            
                         });                        
                         fss.Add<States.siteSelected>(flowState =>
                         {
                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.CompleteConstruction, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                                  actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                  actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());


                              });
                         });
                         fss.Add<States.constructed>(flowState =>{
                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.Commission, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                                  actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                  actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());


                              });
                         });
                         fss.Add<States.commissioned>(flowState =>
                         {

                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.ConnectToGrid, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                                  actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                  actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());

                              });
                         });
                         fss.Add<States.connectedToGrid>(flowState =>
                         {

                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.PlaceInService, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                                  actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                  actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());

                              });
                         });
                         fss.Add<States.inService>(flowState =>
                         {

                             return flowState
                              .WithActions(actions =>
                              {
                                  actions.Add(g => g.Cancel2, a => a.IsDuplicatedInToolbar());
                                  actions.Add(g => g.Suspend, a => a.IsDuplicatedInToolbar());
                              });
                         });
                         fss.Add<States.suspended>();
                         fss.Add<States.cancelled>();

                     })
                     .WithTransitions(transitions =>
                     {
                         transitions.AddGroupFrom<States.planned>(ts =>
                         {
                             ts.Add(t => t.To<States.underSurvey>()
                             .IsTriggeredOn(g => g.ActivateSurvey));

                             ts.Add(t => t.To<States.siteSelected>()
                            .IsTriggeredOn(g => g.SelectSite));


                             ts.Add(t => t.To<States.cancelled>()
                             .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });
                         transitions.AddGroupFrom<States.underSurvey>(ts =>
                         {

                             ts.Add(t => t.To<States.siteSelected>()
                            .IsTriggeredOn(g => g.SelectSite));


                             ts.Add(t => t.To<States.cancelled>()
                             .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });
                         transitions.AddGroupFrom<States.siteSelected>(ts =>
                         {
                             ts.Add(t => t.To<States.constructed>()
                             .IsTriggeredOn(g => g.CompleteConstruction));

                             ts.Add(t => t.To<States.cancelled>()
                            .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });
                         transitions.AddGroupFrom<States.constructed>(ts =>
                         {
                             ts.Add(t => t.To<States.commissioned>()
                             .IsTriggeredOn(g => g.Commission));

                             ts.Add(t => t.To<States.cancelled>()
                            .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });
                         transitions.AddGroupFrom<States.commissioned>(ts =>
                         {
                             ts.Add(t => t.To<States.connectedToGrid>()
                             .IsTriggeredOn(g => g.ConnectToGrid));

                             ts.Add(t => t.To<States.cancelled>()
                            .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });
                         transitions.AddGroupFrom<States.connectedToGrid>(ts =>
                         {
                             ts.Add(t => t.To<States.inService>()
                             .IsTriggeredOn(g => g.PlaceInService));

                             ts.Add(t => t.To<States.cancelled>()
                            .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });
                         transitions.AddGroupFrom<States.inService>(ts =>
                         {

                             ts.Add(t => t.To<States.cancelled>()
                            .IsTriggeredOn(g => g.Cancel2));

                             ts.Add(t => t.To<States.suspended>()
                            .IsTriggeredOn(g => g.Suspend));

                         });

                     }))
                     .WithCategories(categories =>
                     {
                         categories.Add(processingCategory);
                         categories.Add(OtherCategory);
                     })
                     .WithActions(actions =>
                     {
                         actions.Add(g => g.ActivateSurvey, c => c
                             .WithCategory(processingCategory));
                         actions.Add(g => g.SelectSite, c => c
                            .WithCategory(processingCategory));
                         actions.Add(g => g.CompleteConstruction, c => c
                            .WithCategory(processingCategory));
                         actions.Add(g => g.ConnectToGrid, c => c
                            .WithCategory(processingCategory));
                         actions.Add(g => g.Cancel2, c => c
                            .WithCategory(processingCategory));
                         actions.Add(g => g.Suspend, c => c
                           .WithCategory(processingCategory));
                         actions.Add(g => g.Commission, c => c
                          .WithCategory(processingCategory));
                         actions.Add(g => g.PlaceInService, c => c
                            .WithCategory(processingCategory));

                         actions.Add(g => g.CreateFAAction, c => c
                         .WithCategory(OtherCategory));
                         actions.Add(g => g.CreateInventory, c => c
                        .WithCategory(OtherCategory));
                         actions.Add(g => g.CreateSLAction, c => c
                         .WithCategory(OtherCategory));
                     }));
        }
        public static class CategoryNames
        {
            public const string Processing = "Processing";
            public const string Approval = "Approval";
            public const string Other = "Other";
        }
        public static class CategoryID
        {
            public const string Processing = "Processing";
            public const string Other = "Other";
            public const string Approval = "Approval";
        }
    }
}