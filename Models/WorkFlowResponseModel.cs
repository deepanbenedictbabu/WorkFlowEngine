using WorkflowEngineMVC.ViewModels;

namespace WorkflowEngineMVC.Models
{
    public class WorkFlowResponseModel
    {
        public Guid ProcessId { get; set; }
        public int CaseId { get; set; }
        public GTSTSchedulerModel GTSTScheduler_Model { get; set; }
        public NoticeGenerationModel NoticeGeneration_Model { get; set; }
        public NoticePreviewModel NoticePreview_Model { get; set; }        
        public List<CommandViewModel> ListCommandView_Model { get; set; }        
    }
}
