using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.Controllers;
using WorkflowEngineMVC;
using WorkflowEngineMVC.Models;
using Microsoft.CodeAnalysis.Operations;

namespace WorkflowLib
{
    public class WorkflowActionProvider: IWorkflowActionProvider
    {
        public WorkFlowResponseModel WorkflowResponseModel;
        private readonly Dictionary<string, Action<ProcessInstance, WorkflowRuntime, string>> _actions = new();

        private readonly Dictionary<string, Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task>>
            _asyncActions = new();

        private readonly Dictionary<string, Func<ProcessInstance, WorkflowRuntime, string, bool>> _conditions = new();

        private readonly Dictionary<string, Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task<bool>>>
            _asyncConditions = new();

        public WorkflowActionProvider()
        {
            // Register your actions in _actions and _asyncActions dictionaries            
            _actions.Add("ShowGTSTScheduler", ShowGTSTScheduler); // sync
            _actions.Add("RecordGTSTTestResults", RecordGTSTTestResults); // sync
            _actions.Add("GenerateNotice", GenerateNotice); // sync            
            _actions.Add("GenerateAlert", GenerateAlert); // sync
            _asyncActions.Add("MyActionAsync", MyActionAsync); // async

            // Register your conditions in _conditions and _asyncConditions dictionaries
            _conditions.Add("MyCondition", MyCondition); // sync
            _asyncConditions.Add("MyConditionAsync", MyConditionAsync); // async
        }
        private void ShowGTSTScheduler(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {            
            GTSTSchedulerController gTSTSchedulerController = new GTSTSchedulerController();
            string caseId = processInstance.GetParameter<string>("CPROCaseId");
            WorkflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            WorkflowResponseModel.GTSTSchedulerModel = gTSTSchedulerController.Show(caseId);            
        }
        private void RecordGTSTTestResults(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            GTSTTestResultsController gTSTTestResultsController = new GTSTTestResultsController();
            string caseId = processInstance.GetParameter<string>("CPROCaseId");
            WorkflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            WorkflowResponseModel.GTSTTestResultsModel = gTSTTestResultsController.Show(caseId);
        }
        private void GenerateNotice(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            NoticeGenerationController noticeGenerationController = new NoticeGenerationController();
            string caseId = processInstance.GetParameter<string>("CPROCaseId");
            WorkflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            WorkflowResponseModel.NoticeGenerationModel = noticeGenerationController.Show(actionParameter,caseId);            
        }

        private void GenerateAlert(ProcessInstance processInstance, WorkflowRuntime runtime,
            string actionParameter)
        {
            CPROUserAlertController generateUserAlerts = new CPROUserAlertController();
            string caseId = processInstance.GetParameter<string>("CPROCaseId");
            WorkflowResponseModel = processInstance.GetParameter<WorkFlowResponseModel>("WorkflowResponseModel");
            WorkflowResponseModel.CPROUserAlertModel = generateUserAlerts.GenerateAlert(caseId);
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

        public WorkFlowResponseModel ExecuteCommand(string commandName, Guid processId)
        {            
            WorkflowCommand? workflowCommand = WorkflowInit.Runtime
                                                .GetAvailableCommands(processId, string.Empty)
                                                .Where(c => c.CommandName.Trim().ToLower() == commandName.Trim().ToLower()).FirstOrDefault();
            WorkflowInit.Runtime.ExecuteCommand(workflowCommand, string.Empty, string.Empty);
            WorkflowResponseModel.ProcessId = processId;            
            return WorkflowResponseModel;
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
