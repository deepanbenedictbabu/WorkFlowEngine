﻿using OptimaJet.Workflow.Core.Model;

namespace WorkflowEngineMVC.Models
{
    public class WorkFlowResponseModel
    {
        public Guid ProcessId { get; set; }
        public string? CurrentStateName { get; set; }
        public string? CurrentActivityName { get; set; }
        public int CaseId { get; set; }
        public GTSTSchedulerModel? GTSTScheduler_Model { get; set; }
        public NoticeGenerationModel? NoticeGeneration_Model { get; set; }
        public NoticePreviewModel? NoticePreview_Model { get; set; }        
        public List<CommandModel> ListCommandModel { get; set; }                   
        public ProcessDefinition Processdefinition { get; set; }

        public WorkFlowResponseModel()
        {
            Processdefinition = new ProcessDefinition();
        }

    }
}
