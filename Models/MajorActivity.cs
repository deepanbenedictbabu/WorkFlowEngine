namespace WorkflowEngineMVC.Models
{
    public class MajorActivity
    {
        public string? ActivityMajor_CODE { get; set; }
        public string? Subsystem_CODE { get; set; }
        public string? DescriptionActivity_TEXT { get; set; }
        public string? BeginValidity_DATE { get; set; }
        public DateTime? EndValidity_DATE { get; set; }
        public string? WorkerUpdate_ID { get; set; }
        public DateTime? Update_DTTM { get; set; }
        public long? TransactionEventSeq_NUMB { get; set; }
        public string? Stop_INDC { get; set; }
        public string? MultipleActiveInstance_INDC { get; set; }
        public string? ManualStop_INDC { get; set; }
    }
}
