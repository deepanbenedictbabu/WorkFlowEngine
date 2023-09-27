namespace WorkflowEngineMVC.Models
{
    public class ActionModel
    {
        public Guid ProcessId { get; set; }
        public string? ActionName { get; set; }
        public WorkFlowInputParameter? InputParameter { get; set; }        
    }
}
