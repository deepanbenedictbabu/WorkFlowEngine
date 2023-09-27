namespace WorkflowEngineMVC.Models
{
    public class CommandModel
    {
        public Guid ProcessId { get; set; }
        public string? CommandName { get; set; }
        public string? LocalizedName { get; set; }
        public string? Classifier { get; set; }
        public List<ActionModel>? ListActionModel { get; set; }
    }
}
