namespace WorkflowEngineMVC.ViewModels
{
    public class CommandViewModel
    {
        public Guid ProcessId { get; set; }
        public string? CommandName { get; set; }
        public string? LocalizedName { get; set; }
        public string? Classifier { get; set; }
        public string? CurrentStateName { get; set; }
        public string? CurrentActivityName { get; set; }
    }
}
