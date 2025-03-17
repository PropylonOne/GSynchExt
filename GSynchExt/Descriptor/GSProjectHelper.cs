using GSynchExt;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Helpers;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.PJ.Submittals.PJ.DAC;
using PX.SM;
using PX.TM;
using System;
using System.Linq;
using static GSynchExt.SolarSiteEntry;
/// <summary>
/// A utility class that to assist Projects, Solar Sites & Tasks related actions
/// Completion of KEY Milestone tasks
/// 1. Construction Start:Solar Budget Gets locked
/// 2. Construction Completion:Solar site status gets updated
/// 3. Commisioned: Solar site status gets updated
/// 4. Connected To Grid: Solar site status gets updated. NON-STOCK item gets created
/// 5. Place in Service: Solar site status gets updated. Service Location gets created.
/// </summary>
namespace PX.Objects.PM
{
    public static class GSProjectHelper
    {
        public const string SubType = "SUBACCOUNT";
        //public const string accGroup = "MILSTONE";
        #region Project Methods
        public static bool ValidatePredecessorTask(PMTask preTask, PMTask task, bool isTemp, out DateTime? preTaskDate, out DateTime? sucTaskkDate)
        {
            preTaskDate = null;
            sucTaskkDate = null;
            if (preTask == null) return true;
            if (isTemp)
            {
                var preID = preTask.TaskCD.Substring(0, 3);
                var origID = task.TaskCD.Substring(0, 3);

                if (int.Parse(preID) >= int.Parse(origID))
                {
                    return false;
                }
            }
            else
            {
                preTaskDate = preTask.EndDate ?? preTask.PlannedEndDate;
                sucTaskkDate = task.StartDate ?? task.PlannedStartDate;

                if (preTaskDate == null || sucTaskkDate == null) return true;
                if ((preTaskDate >= sucTaskkDate) || (task.TaskID == preTask.TaskID)) return false;
            }
            return true;
        }
        public static string ValidateBudgetForTimelineTask(PMBudget costBudget, int? timelineGrpID)
        {
            if (costBudget == null) return null;
            if (!costBudget.Amount.Equals(0.00m))
            {
                if (costBudget.AccountGroupID == timelineGrpID)
                {
                    return GSynchExt.Messages.NoCostBudgetforTimeline;
                }
            }

            return null;
        }

        public static DateTime? CalculatePlannedEndDate(PMTask task, DateTime plannedStartDate, int leadDays, string calendar)
        {
            var taskExt = task.GetExtension<PMTaskGSExt>();
            if (taskExt.UsrLeadDays == null) return null;
            if (plannedStartDate != null)
            {
                task.PlannedEndDate = DateTimeHelper.CalculateBusinessDate((DateTime)plannedStartDate, leadDays, calendar);
            }

            return task.PlannedEndDate;
        }
        public static int GetLeadDays(DateTime startDate, DateTime endDate, string calendarId)
        {
            var daysDifference = 1;
            var graph = PXGraph.CreateInstance<PXGraph>();
            while (true)
            {
                var newBusinessDate = startDate.AddDays(daysDifference);
                if (CalendarHelper.IsWorkDay(graph, calendarId, newBusinessDate) &&
                    newBusinessDate.Date >= endDate.Date)
                {
                    break;
                }
                daysDifference++;
            }
            return daysDifference;
        }
        public static PMTask GetPreviousTask(PXGraph graph, PMTask task)
        {
            var taskExt = task.GetExtension<PMTaskGSExt>();
            var prevTask = new PMTask();
            if (taskExt.UsrPredecessorTaskID != null)
            {
                prevTask = PMTask.PK.Find(graph, task.ProjectID, taskExt.UsrPredecessorTaskID);
            }
            else
            {
                prevTask = SelectFrom<PMTask>.Where<PMTask.projectID.IsEqual<@P.AsInt>.
                        And<PMTask.taskCD.IsLess<@P.AsString>>>.OrderBy<PMTask.taskCD.Desc>.View.Select(graph, task.ProjectID, task.TaskCD).TopFirst;
            }
            return prevTask;
        }

        #region Project Mapping with Solar Sites
        /// <summary>
        /// When EPC vendor is updated in the project, Solar sites EPC to be updated.
        /// </summary>
        public static void UpdateSolarSiteEPCVendor(string siteCD, int vendorID)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.EPCVendorID = vendorID;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteStartDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.StartDate = newDate;
                ssRec.SiteStatus = GSynchExt.Status.SiteSelected;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteCompletionDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.ConstructionEndDate = newDate;
                ssRec.SiteStatus = GSynchExt.Status.Constructed;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteConStartDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.ConstructionStartDate = newDate;
                // ssRec.SiteStatus = GSynchExt.Status.Constructed;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteCommissionedDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.CommissionedDate = newDate;
                ssRec.SiteStatus = GSynchExt.Status.Commissioned;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteConGridDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ///Create Solar Sales NON-Stock Item
                ssgraph.CreateSSNonStockItem(ssRec);
                ///Update Solar Site
                ssRec.ConnectedtoGridDate = newDate;
                ssRec.SiteStatus = GSynchExt.Status.ConnectedToGrid;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteServiceDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);

            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ///Create Service Location
                ssgraph.CreateServiceLocation(ssRec);
                ///Update Solar Site
                ssRec.InServiceDate = newDate;
                ssRec.SiteStatus = GSynchExt.Status.InService;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void UpdateSolarSiteProjectEndDate(string siteCD, DateTime newDate)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.EndDate = newDate;
                ssRec.SiteStatus = Status.Completed;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void ReverseSolarSiteCompletion(string siteCD, string status)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.ConstructionEndDate = null;
                ssRec.SiteStatus = status;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void ReverseSolarSiteCommissioned(string siteCD, string status)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.CommissionedDate = null;
                ssRec.SiteStatus = status;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void ReverseSolarSiteConnectedToGrid(string siteCD, string status)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.ConnectedtoGridDate = null;
                ssRec.SiteStatus = status;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void ReverseSolarSiteServiceDate(string siteCD, string status)
        {
            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, siteCD);
            var ssRec = ssgraph.Site.Current;
            if (ssRec != null)
            {
                ssRec.ConnectedtoGridDate = null;
                ssRec.SiteStatus = status;
                ssgraph.Site.Update(ssRec);
                ssgraph.Actions.PressSave();
            }
        }
        public static void LockProjectBudget(PMProject proj)
        {
            ProjectEntry graph = PXGraph.CreateInstance<ProjectEntry>();

            if (proj != null)
            {
                graph.Project.Current = proj;
                graph.Actions["lockBudget"].Press();
            }
        }
        public static void ProcessTaskStatusUpdate(string oldval, string newval, SiteSetup sitePref, PMTask row, PXGraph graph)
        {
            try
            {
                if (oldval != newval)
                {
                    var proj = PMProject.PK.Find(graph, row.ProjectID);
                    if (newval == ProjectTaskStatus.Completed)
                    {
                        if (sitePref != null)
                        {
                            if (row.TaskCD.Length > 3)
                            {
                                var task = row.TaskCD.Remove(0, 3) ?? "XXX";
                                task = task.TrimEnd();
                                /// Locking the budget is carried out at the project task - Row persisted.
                                /*
                                if (task == sitePref.MapConsStart)
                                {
                                    GSProjectHelper.LockProjectBudget(proj);
                                }*/
                                if (task == sitePref.MapConsStart)
                                {
                                    GSProjectHelper.UpdateSolarSiteConStartDate(proj?.ContractCD, (DateTime)graph.Accessinfo.BusinessDate);
                                }
                                if (task == sitePref.MapConsEnd)
                                {
                                    GSProjectHelper.UpdateSolarSiteCompletionDate(proj?.ContractCD, (DateTime)graph.Accessinfo.BusinessDate);
                                    
                                }
                                if (task == sitePref.MapCommissioned)
                                {
                                    GSProjectHelper.UpdateSolarSiteCommissionedDate(proj?.ContractCD, (DateTime)graph.Accessinfo.BusinessDate);
                                }
                                if (task == sitePref.MapConnectedToGrid)
                                {
                                    GSProjectHelper.UpdateSolarSiteConGridDate(proj?.ContractCD, (DateTime)graph.Accessinfo.BusinessDate);
                                }
                                if (task == sitePref.MapReleasedToServices)
                                {
                                    GSProjectHelper.UpdateSolarSiteServiceDate(proj?.ContractCD, (DateTime)graph.Accessinfo.BusinessDate);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (oldval == ProjectTaskStatus.Completed)
                        {
                            if (row.TaskCD.Length > 3)
                            {
                                var task = row.TaskCD.Remove(0, 3) ?? "XXX";
                                task = task.TrimEnd();
                                if (task == sitePref.MapConsEnd)
                                {
                                    GSProjectHelper.ReverseSolarSiteCompletion(proj?.ContractCD, GSynchExt.Status.SiteSelected);
                                }
                                if (task == sitePref.MapCommissioned)
                                {
                                    GSProjectHelper.ReverseSolarSiteCommissioned(proj?.ContractCD, GSynchExt.Status.Constructed);
                                }
                                if (task == sitePref.MapConnectedToGrid)
                                {
                                    GSProjectHelper.ReverseSolarSiteConnectedToGrid(proj?.ContractCD, GSynchExt.Status.Commissioned);
                                }
                                if (task == sitePref.MapReleasedToServices)
                                {
                                    GSProjectHelper.ReverseSolarSiteServiceDate(proj?.ContractCD, GSynchExt.Status.ConnectedToGrid);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new PXException(GSynchExt.Messages.StatusUpdateError, e.Message.Trim());
            }
        }

        public static void ProcessTaskEndDateUpdate(DateTime? oldval, DateTime? newval, SiteSetup sitePref, PMTask row, PXGraph graph)
        {
            try
            {
                if (oldval != newval)
                {
                    var proj = PMProject.PK.Find(graph, row.ProjectID);
                    if (proj == null) return;
                    if (row.Status == ProjectTaskStatus.Completed)
                    {
                        if (sitePref != null)
                        {
                            if (row.TaskCD.Length > 3)
                            {
                                var task = row.TaskCD.Remove(0, 3) ?? "XXX";
                                task = task.TrimEnd();

                                SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
                                ssgraph.Site.Current = SolarSite.UK.Find(ssgraph, proj.ContractCD);
                                var ssRec = ssgraph.Site.Current;
                                if (ssRec == null) return;

                                if (task == sitePref.MapConsStart)
                                {
                                    ssRec.ConstructionStartDate = newval;
                                    ssgraph.Site.Update(ssRec);
                                    ssgraph.Actions.PressSave();
                                }
                                if (task == sitePref.MapConsEnd)
                                {
                                    ssRec.ConstructionEndDate = newval;
                                    ssgraph.Site.Update(ssRec);
                                    ssgraph.Actions.PressSave();
                                }
                                if (task == sitePref.MapCommissioned)
                                {
                                    ssRec.CommissionedDate = newval;
                                    ssgraph.Site.Update(ssRec);
                                    ssgraph.Actions.PressSave();
                                }
                                if (task == sitePref.MapConnectedToGrid)
                                {
                                    ssRec.ConnectedtoGridDate = newval;
                                    ssgraph.Site.Update(ssRec);
                                    ssgraph.Actions.PressSave();
                                }
                                if (task == sitePref.MapReleasedToServices)
                                {
                                    ssRec.InServiceDate = newval;
                                    ssgraph.Site.Update(ssRec);
                                    ssgraph.Actions.PressSave();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new PXException(GSynchExt.Messages.EndDateUpdateError, e.Message.Trim());
            }
        }

        #endregion
        #endregion
        #region Subaccount methods
        public static Sub GetSubaccount(PXGraph graph, PMProject proj)
        {
            SolarSite solarSite = PXSelect<SolarSite,
                    Where<SolarSite.solarSiteCD, Equal<Required<SolarSite.solarSiteCD>>>>.Select(graph, proj.ContractCD);
            if (solarSite == null) return null;
            return GetSubaccount(graph, solarSite);
        }
        public static Sub GetSubaccount(PXGraph graph, SolarSite solarSite)
        {
            Sub subAcc = PXSelect<Sub,
                    Where<Sub.subCD, Contains<Required<Sub.subCD>>>>.Select(graph, solarSite.SolarSiteCD);
            if (subAcc != null)
            {
                if (subAcc.SubCD == solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD)
                {
                    return subAcc;
                }
            }

            return null;
        }
        public static Sub CreateSubaccount(PXGraph graph, PMProject proj)
        {

            SolarSite solarSite = PXSelect<SolarSite,
                    Where<SolarSite.solarSiteCD, Equal<Required<SolarSite.solarSiteCD>>>>.Select(graph, proj.ContractCD);
            if (solarSite == null) return null;
            return CreateSubaccount(graph, solarSite);
        }
        public static Sub CreateSubaccount(PXGraph graph, SolarSite solarSite)
        {
            Sub sub = PXSelect<Sub,
                    Where<Sub.subCD, Contains<Required<Sub.subCD>>>>.Select(graph, solarSite.SolarSiteCD);
            if (sub != null)
            {
                if (sub.SubCD == solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD)
                {
                    return sub;
                }
            }
            else
            {

                SegmentValue val1 = PXSelect<SegmentValue,
                Where<SegmentValue.dimensionID, Equal<Required<SegmentValue.dimensionID>>,
                And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>,
                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(graph, SubType, 1, solarSite.Province);

                SegmentValue val2 = PXSelect<SegmentValue,
                Where<SegmentValue.dimensionID, Equal<Required<SegmentValue.dimensionID>>,
                And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>,
                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(graph, SubType, 2, solarSite.PhaseID);

                SegmentValue val3 = PXSelect<SegmentValue,
                Where<SegmentValue.dimensionID, Equal<Required<SegmentValue.dimensionID>>,
                And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>,
                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(graph, SubType, 3, solarSite.SolarSiteCD);


                if (val1 == null || val2 == null || val3 == null)
                {
                    var segGraph = PXGraph.CreateInstance<SegmentMaint>();
                    if (val1 == null)
                    {
                        SegmentValue seg = segGraph.Values.Insert(new SegmentValue
                        {
                            DimensionID = SubType,
                            SegmentID = 1,
                            Value = solarSite.Province,
                            Descr = solarSite.Province
                        });
                    }
                    if (val2 == null)
                    {
                        SegmentValue seg = segGraph.Values.Insert(new SegmentValue
                        {
                            DimensionID = SubType,
                            SegmentID = 2,
                            Value = solarSite.PhaseID,
                            Descr = solarSite.PhaseID
                        });
                    }
                    if (val3 == null)
                    {
                        SegmentValue seg = segGraph.Values.Insert(new SegmentValue
                        {
                            DimensionID = SubType,
                            SegmentID = 3,
                            Value = solarSite.SolarSiteCD,
                            Descr = solarSite.SiteName
                        });
                    }
                    segGraph.Actions.PressSave();
                }
                var subGraph = PXGraph.CreateInstance<SubAccountMaint>();
                sub = subGraph.SubRecords.Insert(new Sub
                {
                    SubCD = solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD,
                    Active = true
                });
                subGraph.Actions.PressSave();
                return sub;
            }
            return sub;
        }


        public static Sub UpdateSubaccount(PXGraph graph, SolarSite solarSite)
        {


            SolarSiteEntry ssgraph = PXGraph.CreateInstance<SolarSiteEntry>();
            var subGraph = PXGraph.CreateInstance<SubAccountMaint>();
            Sub sub = PXSelect<Sub, Where<Sub.subCD, Contains<Required<Sub.subCD>>>>.Select(graph, solarSite.SolarSiteCD);
            subGraph.SubRecords.Current = Sub.UK.Find(subGraph, sub.SubCD);
            var subRec = subGraph.SubRecords.Current;
            if (subRec != null)
            {
                if (subRec.SubCD == solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD)
                {
                    return subRec;
                }
                else
                {
                    subRec.SubCD = solarSite.Province + solarSite.PhaseID + solarSite.SolarSiteCD;
                    subGraph.SubRecords.Update(subRec);
                    subGraph.Actions.PressSave();
                }
                return subRec;

            }

            return subRec;
        }
        #endregion
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static PMAccountGroup GetTimelineDefaultAccntGrp(PXGraph graph)
        {
            SiteSetup setup = PXSelect<SiteSetup>.Select(graph);
            PMAccountGroup rec = PXSelect<PMAccountGroup,
                        Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(graph, setup?.TimeAccntGrp);
            return rec;
        }
        #region Mapped Tasks

        public static PMTask GetConstructionStartTask(PXGraph graph, SolarSite row)
        {
            SiteSetup setup = PXSelect<SiteSetup>.Select(graph);
            return GetDefaultTask(graph, row, setup?.MapConsStart);
        }
        public static PMTask GetConstructionEndTask(PXGraph graph, SolarSite row)
        {
            SiteSetup setup = PXSelect<SiteSetup>.Select(graph);
            return GetDefaultTask(graph, row, setup?.MapConsEnd);
        }
        public static PMTask GetCommissionedTask(PXGraph graph, SolarSite row)
        {
            SiteSetup setup = PXSelect<SiteSetup>.Select(graph);
            return GetDefaultTask(graph, row, setup?.MapCommissioned);
        }
        public static PMTask GetConnectedToGridTask(PXGraph graph, SolarSite row)
        {
            SiteSetup setup = PXSelect<SiteSetup>.Select(graph);

            return GetDefaultTask(graph, row, setup?.MapConnectedToGrid);
        }
        public static PMTask GetReleasedToServicesTask(PXGraph graph, SolarSite row)
        {
            SiteSetup setup = PXSelect<SiteSetup>.Select(graph);
            return GetDefaultTask(graph, row, setup?.MapReleasedToServices);
        }
        public static PMTask GetDefaultTask(PXGraph graph, SolarSite row, String setupTask)
        {
            var taskRec = PXSelect<PMTask,
                Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                And<PMTask.taskCD, Contains<Required<PMTask.taskCD>>>>>.Select(graph, row.ProjectID, setupTask ?? "XXX");
            foreach (PMTask res in taskRec)
            {
                var task = (PMTask)res;
                if (task.TaskCD.Length > 3)
                {
                    var taskSeg = res.TaskCD.Remove(0, 3) ?? "XXX";
                    if (taskSeg.TrimEnd() == setupTask)
                        return task;
                }
            }
            return null;
        }
        #endregion
        public static String GetTaskSegment2(PMTask row)
        {
            if (row.TaskCD.Length > 3)
            {
                var task = row.TaskCD.Remove(0, 3) ?? "XXX";
                return task.TrimEnd();
            }
            return null;
        }

        public static string ValidateTaskStatusUpdate(PXGraph graph, PMProject proj, PMTask task, string newStatus, string oldStatus)
        {
            SolarSite sSite = SolarSite.UK.Find(graph, proj?.ContractCD);
            if (proj == null || task == null) return null;
            string error = null;
            try
            {
                /// Project Validations
                if (newStatus != ProjectTaskStatus.Planned)
                {
                    /// Check if Project is Active
                    if (proj.Status != ProjectStatus.Active)
                    {
                        throw new PXException(GSynchExt.Messages.ProjInactive, newStatus);
                    }
                }
                /// Solar Site Validations
                if (sSite != null)
                {
                    /// Check if Key Milestone Task
                    bool isConsEnd = GSProjectHelper.GetConstructionEndTask(graph, sSite)?.TaskID == task.TaskID;
                    bool isCommissioned = GSProjectHelper.GetCommissionedTask(graph, sSite)?.TaskID == task.TaskID;
                    bool isConnectedToGrid = GSProjectHelper.GetConnectedToGridTask(graph, sSite)?.TaskID == task.TaskID;
                    bool isReleasedToServices = GSProjectHelper.GetReleasedToServicesTask(graph, sSite)?.TaskID == task.TaskID;
                    bool isConsStart = GSProjectHelper.GetConstructionStartTask(graph, sSite)?.TaskID == task.TaskID;
                    if (isConsEnd || isCommissioned || isConnectedToGrid || isReleasedToServices || isConsStart) //Impacts Solar Site Status / Key Dates                   
                    {
                        bool isTaskUpStatus = oldStatus != ProjectTaskStatus.Completed && oldStatus != ProjectTaskStatus.Canceled;
                        bool isSolarSiteUpStatus = sSite.ConstructionEndDate == null && sSite.CommissionedDate == null && sSite.ConnectedtoGridDate == null;
                        /// Reversing Status
                        if (newStatus == ProjectTaskStatus.Active && oldStatus != ProjectTaskStatus.Planned)
                        {
                            if (isReleasedToServices || isConnectedToGrid || isConsStart) return null; // Do nothing.
                            if (isCommissioned && sSite.SiteStatus == Status.ConnectedToGrid)
                            {
                                throw new PXException(GSynchExt.Messages.InvalidTaskStatusUpdate, task.TaskCD.TrimEnd(), sSite.SolarSiteCD.TrimEnd(), Status.GetStatusDescription(sSite.SiteStatus));
                            }
                            if (isConsEnd && (sSite.SiteStatus == Status.ConnectedToGrid || sSite.SiteStatus == Status.Commissioned))
                            {
                                throw new PXException(GSynchExt.Messages.InvalidTaskStatusUpdate, task.TaskCD.TrimEnd(), sSite.SolarSiteCD.TrimEnd(), Status.GetStatusDescription(sSite.SiteStatus));
                            }

                        }
                        if (newStatus == ProjectTaskStatus.Completed)
                        {
                            if (oldStatus == ProjectTaskStatus.Active || oldStatus == ProjectTaskStatus.Planned || oldStatus == ProjectTaskStatus.Canceled)

                            {
                                if (sSite.EstSiteValue == null || sSite.EstSiteValue == 0 || sSite.SiteCapacity == null || sSite.SiteCapacity == 0 || sSite.ACCapacity == null || sSite.ACCapacity == 0)
                                {
                                    throw new PXException(GSynchExt.Messages.CannotCompleteTask);
                                }
                            }
                        }

                        if (newStatus == ProjectTaskStatus.Canceled)
                        {
                            if (oldStatus == ProjectTaskStatus.Active || oldStatus == ProjectTaskStatus.Planned || oldStatus == ProjectTaskStatus.Completed)
                            {
                                throw new PXException(GSynchExt.Messages.CannotCancelTask);
                            }
                        }
                        if (newStatus == ProjectTaskStatus.Completed && (oldStatus == ProjectTaskStatus.Active || oldStatus == ProjectTaskStatus.Planned))
                        {
                            if ((isConsStart || isConsEnd || isCommissioned || isConnectedToGrid || isReleasedToServices) && (sSite.SiteCapacity == decimal.Zero || sSite.SiteCapacity == null || sSite.EstSiteValue == decimal.Zero || sSite.EstSiteValue == null))
                            {
                                throw new PXException(GSynchExt.Messages.CheckNullsForTaskStatusUpdate);
                            }
                        }
                        if (newStatus == ProjectTaskStatus.Canceled && (oldStatus == ProjectTaskStatus.Active || oldStatus == ProjectTaskStatus.Planned || oldStatus == ProjectTaskStatus.Completed))
                        {
                            if (isConsStart || isConsEnd || isCommissioned || isConnectedToGrid || isReleasedToServices)
                            {
                                throw new PXException(GSynchExt.Messages.CannotCancelSolarSiteTask);
                            }
                        }
                    }
                }

                ///Other Validations
                var taskExt = task.GetExtension<PMTaskGSExt>();
                if (taskExt == null) return null;
                if (newStatus == ProjectTaskStatus.Completed)
                {
                    ///1. Check if Docs are Required
                    if (taskExt.UsrIsComplDocReq == true)
                    {
                        bool submExist = false;
                        bool openSubmExist = false;
                        var pjSubRec = PXSelect<PJSubmittal,
                            Where<PJSubmittal.projectId, Equal<Required<PJSubmittal.projectId>>,
                            And<PJSubmittal.projectTaskId, Equal<Required<PJSubmittal.projectTaskId>>>>>.Select(graph, task.ProjectID, task.TaskID);
                        var count = 0;
                        count = PXNoteAttribute.GetFileNotes(graph.Caches[typeof(PMTask)], task).Length;
                        foreach (PJSubmittal subm in pjSubRec)
                        {
                            submExist = true;
                            openSubmExist = subm.Status != "C";
                            if (openSubmExist) break;
                        }
                        if (!submExist && count <= 0)
                        {
                            throw new PXException(GSynchExt.Messages.SubmRequired, task.TaskCD.Trim());
                        }
                        if (openSubmExist)
                        {
                            throw new PXException(GSynchExt.Messages.SubmNotClosed, task.TaskCD.Trim());
                        }
                    }
                    ///2. Predecessor Tasks Completed
                    if (taskExt.UsrPredecessorTaskCD != null)
                    {
                        var preTask = PMTask.UK.Find(graph, task.ProjectID, taskExt.UsrPredecessorTaskCD);
                        if (preTask != null)
                        {
                            if (preTask.Status == ProjectTaskStatus.Active || preTask.Status == ProjectTaskStatus.Planned)
                            {
                                throw new PXException(GSynchExt.Messages.PredecessorNotCompleted, taskExt.UsrPredecessorTaskCD.Trim());
                            }
                        }
                        else
                        {
                            throw new PXException(GSynchExt.Messages.PredecessorNoExist, taskExt.UsrPredecessorTaskCD.Trim());
                        }
                    }
                }
                if (newStatus == ProjectTaskStatus.Active)
                {
                    ///1. Predecessor task should be completed
                    if (taskExt.UsrPredecessorTaskCD != null)
                    {
                        var preTask = PMTask.UK.Find(graph, task.ProjectID, taskExt.UsrPredecessorTaskCD);
                        if (preTask != null)
                        {
                            if (preTask.Status != ProjectTaskStatus.Completed)
                            {
                                throw new PXException(GSynchExt.Messages.PredecessorNotCompleted, taskExt.UsrPredecessorTaskCD.Trim());
                            }
                        }
                        else
                        {
                            throw new PXException(GSynchExt.Messages.PredecessorNoExist, taskExt.UsrPredecessorTaskCD.Trim());
                        }
                    }
                }
                if (newStatus == ProjectTaskStatus.Active || newStatus == ProjectTaskStatus.Planned)
                {
                    ///1. If current task is a Predecessor check any completed project tasks with Predecessor
                    var postrec = PXSelect<PMTask,
                        Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                        And<PMTaskGSExt.usrPredecessorTaskCD, Equal<Required<PMTaskGSExt.usrPredecessorTaskCD>>,
                        And<Where<PMTask.status, Equal<ProjectTaskStatus.completed>,
                        Or<PMTask.status, Equal<ProjectTaskStatus.active>>>>>>>.Select(graph, task.ProjectID, task.TaskCD);
                    if (postrec?.Count != 0)
                    {
                        throw new PXException(GSynchExt.Messages.SuccessorTasksError);
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return error;
            }
            return error;
        }
        #region AR Methods
        public static DateTime? GetARInvoiceDate(int branchID, string period)
        {
            var graph = new PXGraph();
            var finRec = OrganizationFinPeriod.PK.FindByBranch(graph, branchID, period);

            return finRec?.EndDate;
        }
        #endregion
        #region Others
        /// <summary>
        /// This is used to filter the load site data
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("{SolarSiteID}")]
        public readonly struct SiteKeyTuple : IEquatable<SiteKeyTuple>
        {
            public readonly int SolarSiteID;

            public SiteKeyTuple(int solarSiteID)
            {
                SolarSiteID = solarSiteID;
            }

            public static SiteKeyTuple Create(ISolarSiteFilter solarSite)
            {
                return new SiteKeyTuple(
                    solarSite.SolarSiteID.GetValueOrDefault());
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 23 + SolarSiteID.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object obj) => obj is SiteKeyTuple other && Equals(other);

            public bool Equals(SiteKeyTuple other) =>
                SolarSiteID == other.SolarSiteID;
        }

        public struct SolarSiteFilter : ISolarSiteFilter
        {
            public int? SolarSiteID { get; set; }
        }

        #endregion
        public static void AddEmailActivity(PMTask task, Int32 notificationTemplateID)
        {
            if (task == null) return;
            var taskExt = task.GetExtension<PMTaskGSExt>();
            if (taskExt == null) return;

            if (taskExt.UsrNotifier != null || taskExt.UsrNotifyWorkgroup != null)
            {
                ProjectTaskEntry graph = PXGraph.CreateInstance<ProjectTaskEntry>();
                var notifier = Contact.PK.Find(graph, taskExt.UsrNotifier);
                String toList = notifier?.EMail;
                /// Fetch the group member list if the workgroup id is not null
                if (taskExt.UsrNotifyWorkgroup != null)
                {
                    var memberList = PXSelect<EPCompanyTreeMember,
                        Where<EPCompanyTreeMember.workGroupID,
                            Equal<Required<EPCompanyTreeMember.workGroupID>>>>
                       .Select(graph, taskExt.UsrNotifyWorkgroup);

                    // Create the to and cc list

                    foreach (EPCompanyTreeMember list in memberList)
                    {
                        notifier = Contact.PK.Find(graph, list.ContactID);
                        if (notifier != null)
                        {
                            if (toList == null)
                            {
                                toList = notifier.EMail;
                            }
                            else
                            {
                                toList = String.Concat(toList, ";", notifier.EMail);
                            }
                        }

                    }
                }

                Notification notification = PXSelect<Notification,
                    Where<Notification.notificationID, Equal<Required<Notification.notificationID>>>>
                    .Select(graph, notificationTemplateID);
                bool sent = false;
                string sError = "Failed to send E-mail.";
                try
                {
                    TemplateNotificationGenerator sender = TemplateNotificationGenerator.Create(task, notification.NotificationID.Value);
                    sender.MailAccountId = (notification.NFrom.HasValue)
                        ? notification.NFrom.Value
                        : PX.Data.EP.MailAccountManager.DefaultMailAccountID;
                    sender.RefNoteID = task.NoteID;
                    sender.Owner = graph.Accessinfo.ContactID;
                    sender.To = toList;
                    sent |= sender.Send().Any();
                }
                catch (Exception ex)
                {
                    sent = false;
                    sError = ex.Message;
                }
            }
        }
    }
}