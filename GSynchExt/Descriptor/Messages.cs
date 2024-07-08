using PX.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.BQL.BqlPlaceholder;

namespace GSynchExt
{
    [PXLocalizable]
    public static class Messages
    {

        //Req Types
        public const string FundTransfer = "Fund Transfer";
        public const string MaterialRequest = "Material Request";
        public const string MaterialRequestServices = "Material Request Services";

        //BuildingAttributes
        public const string SSSBuildingCondition1 = "<5Yrs";
        public const string SSSBuildingCondition2 = ">15 Yrs";
        public const string SSSBuildingCondition3 = "5-15 Yrs";

        public const string SSSRoofMaterial1 = "Amano";
        public const string SSSRoofMaterial2 = "Asbesto";
        public const string SSSRoofMaterial3 = "Tile";
        public const string SSSRoofMaterial4 = "Other";


        public const string SSSRoofCondition1 = "Material Replacement";
        public const string SSSRoofCondition2 = "Structure Repair";
        public const string SSSRoofCondition3 = "Both";
        public const string SSSRoofCondition4 = "No Repair";

        public const string SSSRepairPercentage1 = "10%";
        public const string SSSRepairPercentage2 = "50%";
        public const string SSSRepairPercentage3 = "100%";

        public const string SSSOrientation1 = "E/W";
        public const string SSSOrientation2 = "N/S";
        public const string SSSOrientation3 = "NE/SW";
        public const string SSSOrientation4 = "NW/SE";

        //SolarSite Types
        public const string School = "School";
        public const string PoliceStation = "Police Station";
        public const string Stadium = "Stadium";
        public const string Hospital = "Hospital";
        public const string Other = "Other";

        //SolarSite Status
        public const string Planned = "Planned";
        public const string OnHold = "On Hold";
        public const string Completed = "Completed";
        public const string Rejected = "Rejected";
        public const string UnderSurvey = "Under Pre-Survey";
        public const string PendingApproval = "Pending Approval";
        public const string SurveyCompleted = "Survey Completed";
        public const string SiteSelected = "Site Selected";
        public const string Constructed = "Constructed";
        //  public const string ConstructionCompleted = "Construction Completed";
        public const string Commissioned = "Commissioned ";
        public const string InService = "In Service ";
        public const string Suspended = "Suspended ";
        public const string ConnectedToGrid = "Connected To Grid";
        public const string Cancelled = "Cancelled ";
        public const string Released = "Released";
        public const string Active = "Active";
        public const string Archived = "Archived";
        public const string Closed = "Closed";

        public const string CEB = "CEB";
        public const string LECO = "LECO";

        //Solar Site Errors
        public const string SetupNotConfigured = "Solar Site Setup is not configured.";
        public const string CannotCreateSL = "Cannot Create Service Location for after sales Services because {0} is missing in the Solar Site Preference screen.";
        public const string CannotCreateNS = "Cannot Create Non Stock Item for Solar Sales because {0} is missing in the Solar Site Preference screen.";
        public const string CannotCreateFA = "Cannot Create Fixed Asset because {0} is missing in the Solar Site Preference screen.";
        public const string CannotCreateProject = "Cannot Create Project because {0} is missing in the Solar Site Preference screen.";
        public const string MissingField = "{0} Cannot be null";
        public const string MissingFieldGen = "Field Cannot be null";
        public const string MissingProject = "Solar Site Project does not exist";
        public const string InvalidActionProj = "You cannot set Solar Site to {0}. Check the status of the Project {1}";
        public const string InvalidActionTask = "You cannot set Solar Site to {0}. Check the status of the Project Task {1}";
        public const string HasOngoingSurveys = "Complete any ongoing surveys before you select Site for construction";
        public const string OperationError = "Operation resulted in one or more errors";
        public const string FAExists = "Fixed Asset {0} already exists.";
        public const string SiteValueMissing = "Estimate Site Value (Acquisition Cost) is missing in the Solar Site Screen";
        public const string InitialSurveyError = "Activate initial Survey through Solar Site";
        public const string OKtoChangeProjectID = "This will change the linked Project ID";
        public const string OKtoDeleteSRevInvoice = "This document is r referenced in the following record: SolarRevGen (rrSRU000001). Do you want to proceed?";
      //  public const string OKtoChangeProjectID = "This Will change the linked Project ID";

        // Project Errors
        public const string ProjInactive = "Project is not Active. You cannot change the task status.";

        public const string InvalidPredecessor = "Predecessor Task should have an End Date/ Planned End Date ({0}) before the Start Date/Planned Start Date ({1}) of Task {2}.";
        public const string InvalidPredecessor2 = "Predecessor Task {0} should have an ealier sequence to current task {1}";
        public const string PredecessorNoExist = "Predecessor Task {0} cannot be found";

        public const string NoCostBudgetforTimeline = "Tasks with Account Group {0} must not have Budgeted Amounts, only Budgeted Quantity is allowed. Project:{1} Task:{2}";
        public const string InvalidTaskStatusUpdate = "Task {0} status cannot be changed. Solar Site {1} is {2}";
        public const string CheckNullsForTaskStatusUpdate = " This task cannot be completed until values are provided for fields 'DC Capacity (KW)' and 'Estimated Value (LKR)' of Solar Sites (PM770024) screen.\r\n";
        public const string CannotCancelSolarSiteTask = "This task cannot be cancelled as it is connected to a status of the Solar Site.";
        public const string CannotCreateAPBill = "The Task should be Active and the Account Group should be WIP to create an AP Bill.";
        public const string SubmRequired = "No submittals or attachments found. Documents should be submitted before completing task {0}";
        public const string SubmNotClosed = "There are submittals for task {0} that are not closed.";
        public const string PredecessorNotCompleted = "Predecessor task {0} is not complete";
        public const string StatusUpdateError = "Task status update resulted in one or more errors. {0}";
        public const string SuccessorTasksError = "You cannot change tasks status. There are successor tasks that are completed or active.";
        public const string NoEmailSent = "Missing email template. Notifier/Workgroup not notified";
        public const string NoDCCapacity = "DC Capacity is missing in the Solar Site Screen";
        public const string CreateAPBillError = "The Task should be Active and the Account Group should be WIP to create an AP Bill.";
        public const string CannotCompleteTask = "This task cannot be completed until values are provided for fields 'DC Capacity (kW)', 'AC Capacity (kW)' and Estimated Site Value (LKR)' of Solar Sites (PM770024) screen.";
        public const string CannotCancelTask = "This task cannot be canceled as it is connected to a status of the Solar Site.";
        public const string CannotDeleteTask = "This task cannot be deleted as it is defined as a predecessor for another task.";

      //  public const string NoDCCapacity = "DC Capacity is missing in the Solar Site Screen";
        public const string ConfirmStatusReverse = "Are you sure you want to reverse task completion?";
        /// Solar Rev Errors
        public const string MissingSSItem = "Non Stock Item for Solar Sales is not setup";
        public const string MissingRRRate = "Roof Rental Rate is not setup for the Province";
        public const string MissingUoM = "Sales UoM is missing";
        public const string MissingTariff = "Sales Price(Tariff) is missing";
        public const string MissingRate = "Currency Rate (USD to LKR) is missing";
        public const string SolarSalesRevExist = "Solar Revenue Generation Record for this period already exist. Refer {0}";
        public const string NoLines = "No lines selected for invoicing";
        public const string CustomerMissing = "Customer {0} is missing in the system";
        public const string VendorMissing = "Vendor {0} is missing in the system";
        public const string BranchMissing = "Branch {0} is missing in the system";

        ///BOQ Errors
        public const string BOQSetupNotEnteredException = "BOQ Preferences are missing";

        /// CEB Upload Error
        public const string NoInvLines = "No Invoice Lines to Match";
        public const string UnlreleasedPayments = "There are unreleased payments for fetched invoices. Please release them first. Payment Ref. Nbrs. {0}";

        //Fund Transfer request Error
        public const string CannotRelease = "Cannot Release the {0} record. Amount should be greater than 0.00";
        public const string CannotHold = "{0} Contains Released Fund Transfers ";
        public const string CannotClose = "Request {0} has unreleased Fund Transfers.";
        public const string CannotCreateTransfer = "{0} Already Created Fund Transfer(s) for full Amount ";

        //CATransfer  Error

        public const string ValidateReversedCAAmount = "Current reversal transaction is not within the previous transferred amount of {0} ";
        public const string ValidateCAAmount = "Amount Cannnot be Greater than the current Open Balance : {0} ";

        //Material Request Error
        public const string ValidateTotalQty = "Already Released a FundTransfer ";
        public const string ReqQtyError = "Requested quantity should be equal to or less than the budgeted quantity.";
        public const string WarnNoBalanceQty = "No new lines to add. Check Project Cost Budget or any ongoing Material Requests for the project";
        public const string ReleaseError = "Releasing this document will result in requesting {0} items more than budgeted for Inventory ID {1}, Task ID {2} and Cost Code {3}.";
        public const string ChangeOrderError2 = "Pending Material Request(s) exists for full or partial change order quantity. Cancel/ modify the Material Request(s) accordingly to reverse the Change Order.";
        public const string ChangeOrderError1 = "Released Material Request(s) exists for full or partial change order quantity. Cancel/ modify the Material Request(s) accordingly to reverse the Change Order.";
        public const string TransfersExist = "Items have been transferred for this request. You cannot {0}.";
        public const string IssueExist = "Items have been issued for this request. You cannot {0}.";
        public const string TransfersOrIssueExist = "Items have been Transferred or Issued for this request. You cannot {0}.";
        public const string TransferredNotIssued = "There are items that have been Transferred but not Issued for this request. You cannot {0}.";
        public const string NoOpenQty = "All requested items have been {0}.";
        public const string IssueError = "This will result in an over isse of the item";

        public const string OktoClose = "Closing this request will update the current request quantity to already issued quantity. Do you want to proceed?";
        public const string OktoIssue = "No transferred items pending to be issued. Do you still want to proceed?";
        public const string BudgetNotLocked = "The Cost Budget of the Project is not Locked. Lock the budget before any material are requested.";

        public static class FundTransferStatus
        {

            public const string Planning = "In Planning";
            public const string Released = "Released";
            public const string Cancelled = "Cancelled";
            public const string Closed = "Closed";
            public const string Archived = "Archived";
            public const string OnHold = "OnHold";
            public const string Rejected = "Rejected";
            public const string PendingApproval = "Pending Approval";

        }
    }
}
