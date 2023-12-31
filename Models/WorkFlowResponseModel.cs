﻿using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;

namespace WorkflowEngineMVC.Models
{
    public class WorkFlowResponseModel
    {
        public Guid ProcessId { get; set; }
        public DateTime ProcessStartedDate { get; set; }
        public string? CurrentStateName { get; set; }
        public string? CurrentActivityName { get; set; }
        public string? CurrentMinorActivityCode { get; set; }
        public string? MajorActivityCode { get; set; }
        public string? CurrentCommandName { get; set; }        
        public bool IsHistoryView  { get; set; }
        public string? ScreenName { get; set; }
        public string? CurrentNoticeId { get; set; }
        public CaseDetailsModel? CaseDetailsModel { get; set; }
        public GTSTSchedulerModel? GTSTSchedulerModel { get; set; }
        public List<NoticeGenerationModel> ListNoticeGenerationModel { get; set; }        
        public GTSTTestResultsModel? GTSTTestResultsModel { get; set; }
        public CPROUserAlertModel? CPROUserAlertModel { get; set; }
        public List<CommandModel> ListCommandModel { get; set; }        
        public List<ActivityModel> ListActivityModel { get; set; }
        public List<ProcessHistoryItem> ListHistory { get; set; }
        public List<SpecialNotes> ListSpecialNotes { get; set; }

        public WorkFlowResponseModel()
        {            
            ListCommandModel = new List<CommandModel>();
            ListActivityModel = new List<ActivityModel>();
            ListNoticeGenerationModel = new List<NoticeGenerationModel>();
            ListHistory = new List<ProcessHistoryItem>();
            ListSpecialNotes = new List<SpecialNotes>();
            IsHistoryView = false;            
        }

    }

}
