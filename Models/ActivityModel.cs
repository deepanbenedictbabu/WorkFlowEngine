namespace WorkflowEngineMVC.Models
{
    public class ActivityModel
    {
        public Guid ProcessId { get; set; }
        public string? ActivityName { get; set; }
        public bool IsInitial { get; set; }
        public bool IsFinal { get; set; }        
        public string? LocalizedName { get; set; }
        public string? Classifier { get; set; }
        public double DaysDue { get; set; }
        public string? MinorActivityCode { get; set; }
    }
}
