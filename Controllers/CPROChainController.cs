using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WorkflowLib;
using System.IO;
using Microsoft.Extensions.Configuration;
using WorkflowEngineMVC.ViewModels;
using OptimaJet.Workflow.Core.Persistence;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkflowEngineMVC.Models;
using OptimaJet.Workflow.Core.Model;

namespace WorkflowEngineMVC.Controllers
{
    public class CPROChainController : Controller
    {
        static string schemeCode = "SimpleWF";
        static Guid processId;
        // GET: CPROController            
        public ActionResult Index()
        {
            return View("StartNewRemedy");          
        }
        public ActionResult StartRemedy(int caseId)
        {
            CreateInstance();
            List<CommandModel> commandModelList = GetAvailableCommands(processId);
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.CaseId = caseId;            
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.ListCommandModel = commandModelList;
            return View("Activity", WorkflowInit.WorkflowActionProvider.WorkflowResponseModel);
        }        
        public ActionResult ShowAllActivity(WorkFlowResponseModel wfResponseModel)
        {            
            List<CommandModel> commandModelList = GetAvailableCommands(wfResponseModel.ProcessId);
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.ListCommandModel = commandModelList;            
            return View("Activity", WorkflowInit.WorkflowActionProvider.WorkflowResponseModel);
        }

        private void CreateInstance()
        {
            processId = Guid.NewGuid();
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel = new WorkFlowResponseModel();
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.ProcessId = processId;
            try
            {
                var createInstanceParameters = new CreateInstanceParams("SchemeCode", processId)
                                                    //.AddPersistentParameter("StringParameter", "Some String")
                                                    //.AddPersistentParameter("ObjectParameter", ObjectParameter)
                                                    .AddTemporaryParameter("CurrentDate", DateTime.Now);
                WorkflowInit.Runtime.CreateInstance(schemeCode, processId);
                Console.WriteLine("CreateInstance - OK.", processId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);                
            }
        }

        private List<CommandModel> GetAvailableCommands(Guid processId)
        {                        
            var schema = WorkflowInit.Runtime.GetProcessScheme(processId);
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.Processdefinition = schema;                     
            List <CommandModel> commandModelList = new List<CommandModel>();
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId);
            var workflowCommands = WorkflowInit.Runtime.GetAvailableCommands(processId, string.Empty);
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.CurrentActivityName = activityName;
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.CurrentStateName = stateName;            
            foreach (var workflowCommand in workflowCommands)
            {
                var commandModel = new CommandModel();
                commandModel.ProcessId= workflowCommand.ProcessId;
                commandModel.CommandName = workflowCommand.CommandName;                
                commandModelList.Add(commandModel);
            }            
            return commandModelList;
        }

        public ActionResult ProcessCommand(string commandName, Guid processId)
        {
            WorkflowInit.WorkflowActionProvider.ExecuteCommand(commandName, processId);            
            WorkflowInit.WorkflowActionProvider.WorkflowResponseModel.ListCommandModel = GetAvailableCommands(processId);            
            return RedirectToAction("ShowAllActivity", WorkflowInit.WorkflowActionProvider.WorkflowResponseModel);
        }

        private List<StateViewModel> GetAvailableState()
        {
            var workflowStates = WorkflowInit.Runtime
                            .GetAvailableStateToSet(processId, Thread.CurrentThread.CurrentCulture);
            var stateViewModelList = new List<StateViewModel>();
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId);
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

        public ActionResult ProcessState(string stateName)
        {
            SetState(stateName);
            List<StateViewModel> stateViewModelList = GetAvailableState();
            return View("ShowAllActivity", stateViewModelList);
        }

        private void SetState(string stateName)
        {            
            var state = WorkflowInit.Runtime
                    .GetAvailableStateToSet(processId, Thread.CurrentThread.CurrentCulture)
                    .Where(c => c.Name.Trim().ToLower() == stateName.Trim().ToLower()).FirstOrDefault();
            var stateParams = new SetStateParams(processId, stateName);
            WorkflowInit.Runtime.SetState(stateParams);
        }

        private void DeleteProcess()
        {
            if (processId == null)
            {
                Console.WriteLine("The process isn't created. Please, create process instance.");
                return;
            }
            WorkflowInit.Runtime.DeleteInstance(processId);
            Console.WriteLine("DeleteProcess - OK.", processId);            
        }

        // GET: CPROController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CPROController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CPROController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CPROController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CPROController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CPROController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
