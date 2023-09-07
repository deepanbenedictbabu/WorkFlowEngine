using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow.Core.Runtime;
using System.Text.Json;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.Models;
using WorkflowEngineMVC.ViewModels;
using WorkflowLib;

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
        public ActionResult ShowCPROAlerts(string jsonString)
        {
            workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            _caseId = workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(false);                    
            workFlowResponseModel.CPROUserAlertModel.IsShowAlert = true;
            return View("ActivityChain", workFlowResponseModel);          
        }
        public ActionResult StartRemedy(string caseId)
        {            
            CreateInstance(caseId);
            _caseId = caseId;
            GetAllActivitiesAndCommands(processId);
            return View("ActivityChain", workFlowResponseModel);
        }        
        public ActionResult ShowProcessHistoryView(string jsonString)
        {
            workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString)?? new WorkFlowResponseModel();
            _caseId = workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(true);
            return View("ActivityChain", workFlowResponseModel);
        }
        public ActionResult ShowProcessListView(string jsonString)
        {
            workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            _caseId = workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(false);            
            return View("ActivityChain", workFlowResponseModel);
        }
        public ActionResult UpdateActivity(string jsonString)
        {
            workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            _caseId = workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(false);
            return View("UpdateActivity", workFlowResponseModel);
        }

        private void processView(bool isHistoryView)
        {            
            GetAllActivitiesAndCommands(workFlowResponseModel.ProcessId);            
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

        private void GetAllActivitiesAndCommands(Guid processId)
        {                        
            var schema = WorkflowInit.Runtime.GetProcessScheme(processId);
            //workFlowResponseModel.Processdefinition = schema;                                 
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId);
            workFlowResponseModel.ListCommandModel = new List<CommandModel>();
            workFlowResponseModel.ListActivityModel = new List<ActivityModel>();
            foreach (var activity in schema.Activities)
            {
                ActivityModel activityModel = new ActivityModel();
                activityModel.IsInitial = activity.IsInitial;
                activityModel.IsFinal = activity.IsFinal;
                activityModel.ActivityName = activity.Name;
                activityModel.ProcessId = processId;
                workFlowResponseModel.ListActivityModel.Add(activityModel);
            }
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
                workFlowResponseModel.ListCommandModel.Add(commandModel);
            }            
        }

        public ActionResult ProcessCommand(string jsonString, string commandName)
        {
            workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString);
            _caseId = workFlowResponseModel?.CaseDetailsModel?.CaseId;
            workFlowResponseModel.CurrentCommandName = commandName;            
            WorkflowInit.Runtime.SetPersistentProcessParameter(processId, "CPROCaseId", _caseId);
            WorkflowInit.Runtime.SetPersistentProcessParameter(processId, "WorkflowResponseModel", workFlowResponseModel);
            workFlowResponseModel = WorkflowInit.WorkflowActionProvider.ExecuteCommand(workFlowResponseModel, commandName );            
            GetAllActivitiesAndCommands(processId);            
            return View("ActivityChain", workFlowResponseModel);
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
