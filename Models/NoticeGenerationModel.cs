namespace WorkflowEngineMVC.Models
{
    public class NoticeGenerationModel
    {
        public string? CaseId { get; set; }        
        public string? NoticeId { get; set; }
        public string? MemberId { get; set; }
        public string? CPFirstName { get; set; }
        public string? CPLastName { get; set; }
        public string? CPDOB { get; set; }
        public string? CPAddress { get; set; }
        public string? CPZip { get; set; }
        public string? CPCity { get; set; }
        public string? CPState { get; set; }

        public string? NCPFirstName { get; set; }
        public string? NCPLastName { get; set; }
        public string? NCPDOB { get; set; }
        public string? NCPAddress { get; set; }
        public string? NCPZip { get; set; }
        public string? NCPCity { get; set; }
        public string? NCPState { get; set; }        
    }
}
