namespace WorkflowEngineMVC.Models
{
    public class CaseDetailsModel
    {        
        public string? CaseId { get; set; }
        public string? CP { get; set; }
        public string? NCP { get; set; }

        public string? CPFirstName { get; set; }
        public string? CPLastName { get; set; }
        public string? NCPFirstName { get; set; }
        public string? NCPLastName { get; set; }
        public bool IsStartRemedy { get; set; }
    }
}
