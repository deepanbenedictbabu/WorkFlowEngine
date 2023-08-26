namespace WorkflowEngineMVC.Models
{
    public class GTSTSchedulerModel
    {
        public Guid ProcessId { get; set; }
        public string CaseId { get; set; }
        public string MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string ScheduledTime { get; set; }
    }
}
