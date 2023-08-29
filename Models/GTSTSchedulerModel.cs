namespace WorkflowEngineMVC.Models
{
    public class GTSTSchedulerModel
    {
        public Guid ProcessId { get; set; }
        public string? CaseId { get; set; }       
        public DateTime ScheduledDate { get; set; }
        public string? ScheduledTime { get; set; }        
    }
}
