using OptimaJet.Workflow.Core.Model;

namespace WorkflowEngineMVC.Models
{
    public class WorkFlowResponseModel
    {
        public Guid ProcessId { get; set; }
        public string? CurrentStateName { get; set; }
        public string? CurrentActivityName { get; set; }
        public string? CurrentCommandName { get; set; }        
        public bool IsHistoryView  { get; set; }
        public string? ScreenName { get; set; }        
        public CaseDetailsModel? CaseDetailsModel { get; set; }
        public GTSTSchedulerModel? GTSTSchedulerModel { get; set; }
        public NoticeGenerationModel? NoticeGenerationModel { get; set; }        
        public GTSTTestResultsModel? GTSTTestResultsModel { get; set; }
        public CPROUserAlertModel? CPROUserAlertModel { get; set; }
        public List<CommandModel> ListCommandModel { get; set; }                   
        public List<ActivityModel> ListActivityModel { get; set; }

        public WorkFlowResponseModel()
        {            
            ListCommandModel = new List<CommandModel>();
            ListActivityModel = new List<ActivityModel>();
            IsHistoryView = false;            
        }

    }
}
