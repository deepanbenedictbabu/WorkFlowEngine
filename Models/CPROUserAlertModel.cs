namespace WorkflowEngineMVC.Models
{
    public class CPROUserAlertModel
    {
        public string? CaseId { get; set; }
        public string? MemberId { get; set; }
        public string? AlertName { get; set; }
        public string? AlertMessage { get; set; }
        public string? UserId { get; set; }
        public bool IsShowAlert { get; set; }

    }
}
