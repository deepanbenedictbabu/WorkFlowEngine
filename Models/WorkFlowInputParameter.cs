namespace WorkflowEngineMVC.Models
{
    public class WorkFlowInputParameter
    {
        public string? MinorActivity { get; set; }
        public string? TypeActivity { get; set; }
        public string? Group { get; set; }
        public double DaysDue { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public double AlertWarningInDays { get; set; }        
        public string? ActionAlertCode { get; set; }
        public string? NoticeId { get; set; }
        public string? NoticeRecipient { get; set; }
        public string? PrintMethod { get; set; }    
        public string? ScreenFunctionCode { get; set; } 
        public WorkFlowInputParameter()
        {
            MinorActivity = "";
            Group = "";
            DaysDue = 0;
            Category = "";
            SubCategory = "";
            AlertWarningInDays = 0;
            ActionAlertCode = "A";
            NoticeId = "";
            NoticeRecipient = "";
            PrintMethod = "";
            ScreenFunctionCode = "";
        }
    }
}
