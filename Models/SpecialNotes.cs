namespace WorkflowEngineMVC.Models
{
    public class SpecialNotes
    {
        public Guid ProcessId { get; set; }
        public string? CaseId { get; set; }
        public string? MajorActivityCode { get; set; }
        public string? MinorActivityCode { get; set; }
        public string? Notes { get; set; }
    }
}
