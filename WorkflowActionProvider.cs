using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.Controllers;
using WorkflowEngineMVC;
using WorkflowEngineMVC.Models;
using Microsoft.CodeAnalysis.Operations;
using OptimaJet.Workflow.Core.Persistence;

namespace WorkflowLib
{
    public class WorkflowActionProvider: IWorkflowActionProvider
    {
        private WorkFlowResponseModel workflowResponseModel;
        private readonly Dictionary<string, Action<ProcessInstance, WorkflowRuntime, string>> _actions = new();

        private readonly Dictionary<string, Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task>>
            _asyncActions = new();

        private readonly Dictionary<string, Func<ProcessInstance, WorkflowRuntime, string, bool>> _conditions = new();

        private readonly Dictionary<string, Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task<bool>>>
            _asyncConditions = new();

        public WorkflowActionProvider()
        {
            // Register your actions in _actions and _asyncActions dictionaries            
            //_actions.Add("ShowGTSTScheduler", ShowGTSTScheduler); // sync
            //_actions.Add("RecordGTSTTestResults", RecordGTSTTestResults); // sync
            //_actions.Add("GenerateNotice", GenerateNotice); // sync            
            //_actions.Add("GenerateAlert", GenerateAlert); // sync
            _asyncActions.Add("MyActionAsync", MyActionAsync); // async

            // Register your conditions in _conditions and _asyncConditions dictionaries
            _conditions.Add("MyCondition", MyCondition); // sync
            _conditions.Add("ShowGTSTScheduler", ShowGTSTScheduler); // sync            
            _conditions.Add("RecordGTSTTestResults", RecordGTSTTestResults); // sync            
            _conditions.Add("GenerateNotice", GenerateNotice); // sync            
            _conditions.Add("GenerateAlert", GenerateAlert); // sync            
            _asyncConditions.Add("MyConditionAsync", MyConditionAsync); // async
        }
        private bool ShowGTSTScheduler(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            workflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            if (workflowResponseModel.GTSTSchedulerModel == null)
            {
                GTSTSchedulerController gTSTSchedulerController = new GTSTSchedulerController();
                string caseId = processInstance.GetParameter<string>("CPROCaseId");                
                workflowResponseModel.ScreenName = "GTSTScheduler";
                workflowResponseModel.GTSTSchedulerModel = gTSTSchedulerController.Show(caseId);
                processInstance.SetParameter("WorkflowResponseModel", workflowResponseModel);
                return false;
            }
            else
            {
                workflowResponseModel.ScreenName = "";
                return true;
            }
        }
        private bool RecordGTSTTestResults(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            workflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            if (workflowResponseModel.GTSTTestResultsModel == null)
            {
                GTSTTestResultsController gTSTTestResultsController = new GTSTTestResultsController();
                string caseId = processInstance.GetParameter<string>("CPROCaseId");                
                workflowResponseModel.ScreenName = "GTSTTestResults";
                workflowResponseModel.GTSTTestResultsModel = gTSTTestResultsController.Show(caseId);
                processInstance.SetParameter("WorkflowResponseModel", workflowResponseModel);
                return false;
            }
            else
            {
                workflowResponseModel.ScreenName = "";
                return true;
            }
        }
        private bool GenerateNotice(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)//actionParameter should be the notice id 
        {
            workflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            var listNoticeGenerationModel = workflowResponseModel.ListNoticeGenerationModel.Where(d => d.NoticeId == actionParameter).ToList();
            if (listNoticeGenerationModel == null || listNoticeGenerationModel.Count() < 1)
            {
                NoticeGenerationController noticeGenerationController = new NoticeGenerationController();
                string caseId = processInstance.GetParameter<string>("CPROCaseId");                
                workflowResponseModel.ScreenName = "NoticeGeneration";
                workflowResponseModel.CurrentNoticeId = actionParameter;
                var noticeGenerationModel = noticeGenerationController.Show(actionParameter, caseId);                
                workflowResponseModel.ListNoticeGenerationModel.Add(noticeGenerationModel);
                processInstance.SetParameter("WorkflowResponseModel", workflowResponseModel);
                return false;
            }
            else
            {
                workflowResponseModel.ScreenName = "";
                return true;
            }
        }

        private bool GenerateAlert(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            workflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            if (workflowResponseModel.CPROUserAlertModel == null)
            {
                CPROUserAlertController generateUserAlerts = new CPROUserAlertController();
                string caseId = processInstance.GetParameter<string>("CPROCaseId");                
                workflowResponseModel.ScreenName = "CPROUserAlert";
                workflowResponseModel.CPROUserAlertModel = generateUserAlerts.GenerateAlert(caseId);
                processInstance.SetParameter("WorkflowResponseModel", workflowResponseModel);                
            }
            return true;
        }

        private async Task MyActionAsync(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter, CancellationToken token)
        {
            // Execute your asynchronous code here. You can use await in your code.
        }

        private bool MyCondition(ProcessInstance processInstance, WorkflowRuntime runtime, string actionParameter)
        {
            // Execute your code here
            return false;
        }

        private async Task<bool> MyConditionAsync(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter, CancellationToken token)
        {
            // Execute your asynchronous code here. You can use await in your code.
            return false;
        }

        public WorkFlowResponseModel ExecuteCommand(WorkFlowResponseModel workflowResponseModel, string commandName)
        {
            this.workflowResponseModel = workflowResponseModel;
            this.workflowResponseModel.ProcessId = this.workflowResponseModel.ProcessId;            
            WorkflowCommand? workflowCommand = WorkflowInit.Runtime
                                                .GetAvailableCommands(workflowResponseModel.ProcessId, string.Empty)
                                                .Where(c => c.CommandName.Trim().ToLower() == commandName.Trim().ToLower()).FirstOrDefault();
            WorkflowInit.Runtime.ExecuteCommand(workflowCommand, string.Empty, string.Empty);                        
            return this.workflowResponseModel;
        }

        #region Implementation of IWorkflowActionProvider

        public void ExecuteAction(string name, ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            if (!_actions.ContainsKey(name))
            {
                throw new NotImplementedException($"Action with name {name} isn't implemented");
            }

            _actions[name].Invoke(processInstance, runtime, actionParameter);
        }

        public async Task ExecuteActionAsync(string name, ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter, CancellationToken token)
        {
            //token.ThrowIfCancellationRequested(); // You can use the transferred token at your discretion
            if (!_asyncActions.ContainsKey(name))
            {
                throw new NotImplementedException($"Async Action with name {name} isn't implemented");
            }

            await _asyncActions[name].Invoke(processInstance, runtime, actionParameter, token);
        }

        public bool ExecuteCondition(string name, ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            if (_conditions.ContainsKey(name))
            {
                return _conditions[name].Invoke(processInstance, runtime, actionParameter);
            }

            throw new NotImplementedException($"Condition with name {name} isn't implemented");
        }

        public async Task<bool> ExecuteConditionAsync(string name, ProcessInstance processInstance,
            WorkflowRuntime runtime, string actionParameter, CancellationToken token)
        {
            //token.ThrowIfCancellationRequested(); // You can use the transferred token at your discretion
            if (_asyncConditions.ContainsKey(name))
            {
                return await _asyncConditions[name].Invoke(processInstance, runtime, actionParameter, token);
            }

            throw new NotImplementedException($"Async Condition with name {name} isn't implemented");
        }

        public bool IsActionAsync(string name, string schemeCode)
        {
            return _asyncActions.ContainsKey(name);
        }

        public bool IsConditionAsync(string name, string schemeCode)
        {
            return _asyncActions.ContainsKey(name);
        }

        public List<string> GetActions(string schemeCode, NamesSearchType namesSearchType)
        {
            return _actions.Keys.Union(_asyncActions.Keys).ToList();
        }

        public List<string> GetConditions(string schemeCode, NamesSearchType namesSearchType)
        {
            return _conditions.Keys.Union(_asyncConditions.Keys).ToList();
        }

        #endregion
    }
}
