using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CN.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GSynchExt.GSBOQ;
using static PX.Data.WorkflowAPI.BoundedTo<GSynchExt.GSBOQMaint, GSynchExt.GSBOQ>;

namespace GSynchExt.WorkFlows
{
    public class BOQWorkFlow : PX.Data.PXGraphExtension<GSBOQMaint>
    {
        #region Constants
        public static class States
        {
            public const string OnHold = BOQStatus.OnHold;
            public const string Rejected = BOQStatus.Rejected;
            public const string PendingApproval = BOQStatus.PendingApproval;
            public const string Active = BOQStatus.Active;
            public const string Archived = BOQStatus.Archived;
           

            public class active : PX.Data.BQL.BqlString.Constant<active>
            {
                public active() : base(Active) { }
            }
            public class archived : PX.Data.BQL.BqlString.Constant<archived>
            {
                public archived() : base(Archived) { }
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

        }
        #endregion

        public override void Configure(PXScreenConfiguration config)
        {

            var context = config.GetScreenConfigurationContext<GSBOQMaint, GSBOQ>();

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
                     .StateIdentifierIs<GSBOQ.status>()
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
                         fss.Add<States.active>(flowState =>
                         {
                             return flowState
                             .WithActions(actions =>
                             {
                                 actions.Add(g => g.Archive, a => a.IsDuplicatedInToolbar());
                                 actions.Add(g => g.Hold2, a => a.IsDuplicatedInToolbar());                             
                             });
                         });
                         fss.Add<States.archived>();
                     })
                     .WithTransitions(transitions =>
                     {
                         transitions.AddGroupFrom <States.onHold>(ts =>
                         {
                             ts.Add(t => t.To<States.active>()
                             .IsTriggeredOn(g => g.RemoveHold).When(conditions.IsApproved));

                         });
                         transitions.AddGroupFrom<States.active>(ts =>
                         {
                             ts.Add(t => t.To<States.archived>()
                            .IsTriggeredOn(g => g.Archive));
                             ts.Add(t => t.To<States.onHold>()
                           .IsTriggeredOn(g => g.Hold2));
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
                         actions.Add(g => g.Archive, c => c
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













    

