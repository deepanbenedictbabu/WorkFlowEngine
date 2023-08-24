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

namespace WorkflowEngineMVC.Controllers
{
    public class CPROChainController : Controller
    {
        static string schemeCode = "SimpleWF";
        static Guid processId;
        // GET: CPROController            
        public ActionResult Index()
        {
            return View();          
        }
        public ActionResult StartRemedy(int caseId)
        {
            CreateInstance();
            List<CommandViewModel> commandViewModelList = GetAvailableCommands(processId);
            WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel.CaseId = caseId;            
            WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel.ListCommandView_Model = commandViewModelList;
            return View("Details", WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel);
        }
        // GET: CPROController/Details/5
        public ActionResult Details(WorkFlowResponseModel wfResponseModel)
        {            
            List<CommandViewModel> commandViewModelList = GetAvailableCommands(wfResponseModel.ProcessId);
            WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel.ListCommandView_Model = commandViewModelList;            
            return View("Details", WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel);
        }

        private void CreateInstance()
        {
            processId = Guid.NewGuid();
            WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel = new WorkFlowResponseModel();
            WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel.ProcessId = processId;
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

        private List<CommandViewModel> GetAvailableCommands(Guid processId)
        {            
            List<CommandViewModel> commandViewModelList = new List<CommandViewModel>();
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId);
            var workflowCommands = WorkflowInit.Runtime.GetAvailableCommands(processId, string.Empty);
            foreach (var workflowCommand in workflowCommands)
            {
                var commandViewModel = new CommandViewModel();
                commandViewModel.ProcessId= workflowCommand.ProcessId;
                commandViewModel.CommandName = workflowCommand.CommandName;
                commandViewModel.CurrentActivityName = activityName;
                commandViewModel.CurrentStateName = stateName;
                commandViewModelList.Add(commandViewModel);
            }
            if(commandViewModelList.Count() == 0)
            {
                var commandViewModel = new CommandViewModel();                
                commandViewModel.CurrentActivityName = activityName;
                commandViewModel.CurrentStateName = stateName;
                commandViewModelList.Add(commandViewModel);
            }
            return commandViewModelList;
        }

        public ActionResult ProcessCommand(string commandName, Guid processId)
        {
            WorkflowInit.WorkflowActionProvider.ExecuteCommand(commandName, processId);
            //workflowActionProvider.ExecuteCommand(commandName, processId);
            WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel.ListCommandView_Model = GetAvailableCommands(processId);
            //ViewData["WFResponseModel"] = workFlowResponseModel;
            return RedirectToAction("Details", WorkflowInit.WorkflowActionProvider.WorkFlowResponseModel);
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
            return View("Details", stateViewModelList);
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
