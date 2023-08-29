﻿using Microsoft.AspNetCore.Http;
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
using System.Net.NetworkInformation;
using WorkflowEngineMVC.Data;
using Microsoft.AspNetCore.Components;
using OptimaJet.Workflow.MSSQL.Models;

namespace WorkflowEngineMVC.Controllers
{
    public class CPROChainController : Controller
    {
        static string schemeCode = "SimpleWF";
        static Guid processId;
        MoqData moqData;
        CaseDetailsModel? caseDetailsModel;
        WorkFlowResponseModel workFlowResponseModel;
        string? _caseId;
        public CPROChainController()
        {
            workFlowResponseModel = new WorkFlowResponseModel();
            moqData = new MoqData();
            caseDetailsModel = new CaseDetailsModel();
            caseDetailsModel = new CaseDetailsModel();
            workFlowResponseModel.CaseDetailsModel = caseDetailsModel;
        }

        public ActionResult Index()
        {            
            return View("Index");
        }

        // GET: CPROController            
        public ActionResult GetCaseDetails(string caseId)
        {
            _caseId = caseId;
            caseDetailsModel = moqData?.GetCaseDetails(caseId);            
            workFlowResponseModel.CaseDetailsModel = caseDetailsModel;
            return View("Index", workFlowResponseModel);
        }

        // GET: CPROController            
        public ActionResult ShowStartRemedy(string caseId)
        {
            _caseId = caseId;
            caseDetailsModel = moqData?.GetCaseDetails(caseId);
            caseDetailsModel.IsStartRemedy = true;            
            workFlowResponseModel.CaseDetailsModel = caseDetailsModel;            
            return View("Index", workFlowResponseModel);          
        }
        public ActionResult ShowCPROAlerts(string caseId)
        {
            _caseId = caseId;                        
            workFlowResponseModel.CaseDetailsModel = caseDetailsModel;
            workFlowResponseModel.CPROUserAlertModel = moqData?.GetUserAlerts(caseId);
            workFlowResponseModel.CPROUserAlertModel.IsShowAlert = true;
            return View("ActivityChain", workFlowResponseModel);          
        }
        public ActionResult StartRemedy(string caseId)
        {            
            CreateInstance(caseId);
            _caseId = caseId;
            List<CommandModel> commandModelList = GetAvailableCommands(processId);
            workFlowResponseModel.ListCommandModel = commandModelList;
            return View("ActivityChain", workFlowResponseModel);
        }        
        public ActionResult ShowAllActivity(WorkFlowResponseModel wfResponseModel)
        {            
            return View("ActivityChain", workFlowResponseModel);
        }
        public ActionResult ShowProcessHistoryView(Guid processId, string caseId)
        {
            _caseId = caseId;            
            processView(processId, true);
            return View("ActivityChain", workFlowResponseModel);
        }
        public ActionResult ShowProcessListView(Guid processId, string caseId)
        {
            _caseId = caseId;           
            processView(processId, false);            
            return View("ActivityChain", workFlowResponseModel);
        }
        public ActionResult UpdateActivity(Guid processId, string caseId)
        {
            _caseId = caseId;
            processView(processId, false);
            return View("UpdateActivity", workFlowResponseModel);
        }

        private void processView(Guid processId, bool isHistoryView)
        {            
            List<CommandModel> commandModelList = GetAvailableCommands(processId);            
            workFlowResponseModel.ListCommandModel = commandModelList;
            workFlowResponseModel.IsHistoryView = isHistoryView;            
        }

        private void CreateInstance(string caseId)
        {
            processId = Guid.NewGuid();            
            workFlowResponseModel.ProcessId = processId;            
            try
            {
                var createInstanceParameters = new CreateInstanceParams("SchemeCode", processId)                                                    
                                                    .AddPersistentParameter("CaseId", caseId)
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
            workFlowResponseModel.Processdefinition = schema;                     
            List <CommandModel> commandModelList = new List<CommandModel>();
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId);
            var workflowCommands = WorkflowInit.Runtime.GetAvailableCommands(processId, string.Empty);
            caseDetailsModel = moqData.GetCaseDetails(_caseId);
            workFlowResponseModel.CurrentActivityName = activityName;
            workFlowResponseModel.ProcessId = processId;
            workFlowResponseModel.CurrentStateName = stateName;
            workFlowResponseModel.CaseDetailsModel = caseDetailsModel;
            foreach (var workflowCommand in workflowCommands)
            {
                var commandModel = new CommandModel();
                commandModel.ProcessId= workflowCommand.ProcessId;
                commandModel.CommandName = workflowCommand.CommandName;                
                commandModelList.Add(commandModel);
            }            
            return commandModelList;
        }

        public ActionResult ProcessCommand(string commandName, Guid processId, string caseId)
        {
            _caseId = caseId;            
            WorkflowInit.Runtime.SetPersistentProcessParameter(processId, "CPROCaseId", caseId);
            WorkflowInit.Runtime.SetPersistentProcessParameter(processId, "WorkflowResponseModel", workFlowResponseModel);
            workFlowResponseModel = WorkflowInit.WorkflowActionProvider.ExecuteCommand(commandName, processId);            
            workFlowResponseModel.ListCommandModel = GetAvailableCommands(processId);            
            return View("ActivityChain", workFlowResponseModel);
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
