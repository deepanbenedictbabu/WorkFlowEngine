namespace WorkflowEngineMVC.Models
{
    public class NoticePreviewModel
    {
        public string NoticeId { get; set; }
        public string MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
