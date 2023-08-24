using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.ViewModels;
using WorkflowLib;

namespace WorkflowEngineMVC.Controllers
{
    public class WorkFlowEngineController : ControllerBase
    {
        static string schemeCode = "SimpleWF";
        static Guid? processId = null;
        public void CreateInstance()
        {
            processId = Guid.NewGuid();
            try
            {
                var createInstanceParameters = new CreateInstanceParams("SchemeCode", processId.Value)
                                                    //.AddPersistentParameter("StringParameter", "Some String")
                                                    //.AddPersistentParameter("ObjectParameter", ObjectParameter)
                                                    .AddTemporaryParameter("CurrentDate", DateTime.Now);
                WorkflowInit.Runtime.CreateInstance(schemeCode, processId.Value);
                Console.WriteLine("CreateInstance - OK.", processId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);
                processId = null;
            }
        }

        public List<CommandViewModel> GetAvailableCommands()
        {
            List<CommandViewModel> commandViewModelList = new List<CommandViewModel>();
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId.Value);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId.Value);
            var workflowCommands = WorkflowInit.Runtime.GetAvailableCommands(processId.Value, string.Empty);
            foreach (var workflowCommand in workflowCommands)
            {
                var commandViewModel = new CommandViewModel();
                commandViewModel.CommandName = workflowCommand.CommandName;
                commandViewModel.CurrentActivityName = activityName;
                commandViewModel.CurrentStateName = stateName;
                commandViewModelList.Add(commandViewModel);
            }
            if (workflowCommands.Count() == 0)
            {
                var commandViewModel = new CommandViewModel();
                commandViewModel.CurrentActivityName = activityName;
                commandViewModel.CurrentStateName = stateName;
                commandViewModelList.Add(commandViewModel);
            }
            return commandViewModelList;
        }

        public List<CommandViewModel> ProcessCommand(string commandName)
        {
            ExecuteCommand(commandName);
            var result = WorkflowInit.Runtime.GetProcessHistory(processId.Value);
            List<CommandViewModel> commandViewModelList = GetAvailableCommands();
            return commandViewModelList;
        }

        //public void ProcessCommand(string commandName)
        //{
        //    ExecuteCommand(commandName);
        //    //var result = WorkflowInit.Runtime.GetProcessHistory(processId.Value);
        //    //List<CommandViewModel> commandViewModelList = GetAvailableCommands();
        //    //return commandViewModelList;
        //}

        private static void ExecuteCommand(string commandName)
        {
            WorkflowCommand? workflowCommand = WorkflowInit.Runtime
                                                .GetAvailableCommands(processId.Value, string.Empty)
                                                .Where(c => c.CommandName.Trim().ToLower() == commandName.Trim().ToLower()).FirstOrDefault();
            WorkflowInit.Runtime.ExecuteCommand(workflowCommand, string.Empty, string.Empty);
        }

        private static List<StateViewModel> GetAvailableState()
        {
            var workflowStates = WorkflowInit.Runtime
                            .GetAvailableStateToSet(processId.Value, Thread.CurrentThread.CurrentCulture);
            var stateViewModelList = new List<StateViewModel>();
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId.Value);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId.Value);
            foreach (var workflowState in workflowStates)
            {
                var stateViewModel = new StateViewModel();
                stateViewModel.StateName = workflowState.Name;
                stateViewModel.CurrentStateName = stateName;
                stateViewModel.CurrentActivityName = activityName;
                stateViewModelList.Add(stateViewModel);
            }
            return stateViewModelList;
        }

        public List<StateViewModel> ProcessState(string stateName)
        {
            SetState(stateName);
            List<StateViewModel> stateViewModelList = GetAvailableState();
            return stateViewModelList;
        }
        public List<ProcessHistoryItem> GetProcessHistory()
        {
            var result = WorkflowInit.Runtime.GetProcessHistory(processId.Value);
            return result;
        }

        private static void SetState(string stateName)
        {
            var state = WorkflowInit.Runtime
                    .GetAvailableStateToSet(processId.Value, Thread.CurrentThread.CurrentCulture)
                    .Where(c => c.Name.Trim().ToLower() == stateName.Trim().ToLower()).FirstOrDefault();
            var stateParams = new SetStateParams(processId.Value, stateName);
            WorkflowInit.Runtime.SetState(stateParams);
        }

        private static void DeleteProcess()
        {
            if (processId == null)
            {
                Console.WriteLine("The process isn't created. Please, create process instance.");
                return;
            }
            WorkflowInit.Runtime.DeleteInstance(processId.Value);
            Console.WriteLine("DeleteProcess - OK.", processId);
            processId = null;
        }
    }
}
