﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OptimaJet.Workflow.Core.BPMN;
using OptimaJet.Workflow.Core.Runtime;
using System.Diagnostics;
using System.Text.Json;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.DBContext;
using WorkflowEngineMVC.Models;
using WorkflowLib;

namespace WorkflowEngineMVC.Controllers
{
    public class CPROChainController : Controller
    {
        private readonly IConfiguration _configuration;
        static Guid _processId;
        MoqData _moqData;
        CaseDetailsModel? _caseDetailsModel;
        WorkFlowResponseModel _workFlowResponseModel;
        string? _caseId;        
        const string  _constStrMinorActivity = "MinorActivity";
        const string _constStrMajorActivity = "MajorActivity";
        const string _constStrCPROCaseId = "CPROCaseId";        
        const string _constStrDaysDue = "DaysDue";
        const string _constStrCurrentDate = "CurrentDate";
        const string _constStrFamilyViolence = "FamilyViolence";
        const string _constStrWorkflowResponseModel = "WorkflowResponseModel";        

        public CPROChainController(IConfiguration configuration)
        {
            _configuration = configuration;
            _workFlowResponseModel = new WorkFlowResponseModel();
            _moqData = new MoqData();
            _caseDetailsModel = new CaseDetailsModel();                        
        }

        public ActionResult Index()
        {            
            return View("Index");
        }

        // GET: CPROController            
        public ActionResult GetCaseDetails(string caseId)
        {
            _caseId = caseId;
            _caseDetailsModel = _moqData?.GetCaseDetails(caseId);            
            _workFlowResponseModel.CaseDetailsModel = _caseDetailsModel;
            return View("Index", _workFlowResponseModel);
        }

        public string GetCaseDetailInfo([FromBody] string caseId)
        {
            _caseId = caseId;
            _caseDetailsModel = _moqData?.GetCaseDetails(caseId);
            _workFlowResponseModel.CaseDetailsModel = _caseDetailsModel;
            return JsonSerializer.Serialize(_caseDetailsModel);
        }

        // GET: CPROController            
        public ActionResult ShowStartRemedy(string caseId, string majorActivity)
        {
            _caseId = caseId;
            _caseDetailsModel = _moqData?.GetCaseDetails(caseId);
            _caseDetailsModel.IsStartRemedy = true;            
            _workFlowResponseModel.CaseDetailsModel = _caseDetailsModel;
            _workFlowResponseModel.MajorActivityCode = majorActivity;
            return View("Index", _workFlowResponseModel);          
        }
        public ActionResult ShowCPROAlerts(string jsonString)
        {
            _workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            _caseId = _workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(false);                    
            _workFlowResponseModel.CPROUserAlertModel.IsShowAlert = true;
            return View("ActivityChain", _workFlowResponseModel);          
        }
        public ActionResult StartRemedy(string caseId, string majorActivity)
        {            
            CreateInstance(caseId, majorActivity);
            _caseId = caseId;
            GetAllActivitiesAndCommands(_processId);
            return View("ActivityChain", _workFlowResponseModel);
        }        
        public ActionResult ShowProcessHistoryView(string jsonString)
        {
            _workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString)?? new WorkFlowResponseModel();
            _caseId = _workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(true);
            return View("ActivityChain", _workFlowResponseModel);
        }
        public ActionResult ShowProcessListView(string jsonString)
        {
            _workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            _caseId = _workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(false);            
            return View("ActivityChain", _workFlowResponseModel);
        }
        public ActionResult UpdateActivity(string jsonString)
        {
            _workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            _caseId = _workFlowResponseModel?.CaseDetailsModel?.CaseId;
            processView(false);
            return View("UpdateActivity", _workFlowResponseModel);
        }

        private void processView(bool isHistoryView)
        {            
            GetAllActivitiesAndCommands(_workFlowResponseModel.ProcessId);            
            _workFlowResponseModel.IsHistoryView = isHistoryView;            
        }

        private void CreateInstance(string caseId, string majorActivity)
        {
            _processId = Guid.NewGuid();            
            _workFlowResponseModel.ProcessId = _processId;
            _workFlowResponseModel.MajorActivityCode = majorActivity;
            _workFlowResponseModel.ProcessStartedDate = DateTime.Now;
            try
            {
                var createInstanceParameters = new CreateInstanceParams(majorActivity, _processId)                                                    
                                                    .AddPersistentParameter(_constStrCPROCaseId, caseId)
                                                    .AddPersistentParameter(_constStrMajorActivity, majorActivity)
                                                    .AddTemporaryParameter(_constStrCurrentDate, DateTime.Now);
                WorkflowInit.Runtime.CreateInstance(majorActivity, _processId);
                Console.WriteLine("CreateInstance - OK.", _processId);
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
            var pi = WorkflowInit.Runtime.GetProcessInstanceAndFillProcessParameters(processId);
            var listHistory = WorkflowInit.Runtime.PersistenceProvider.GetProcessHistoryAsync(pi.ProcessId).Result;
            _workFlowResponseModel.ListHistory = listHistory.OrderBy(d => d.StartTransitionTime).ToList();           
            var activityName = WorkflowInit.Runtime.GetCurrentActivityName(processId);
            var stateName = WorkflowInit.Runtime.GetCurrentStateName(processId);
            _workFlowResponseModel.ListCommandModel = new List<CommandModel>();
            _workFlowResponseModel.ListActivityModel = new List<ActivityModel>();
            WorkFlowInputParameter? activityInput = new WorkFlowInputParameter();            
            foreach (var activity in schema.Activities)
            {
                foreach (var impl in activity.Implementation.Where(a => a.ActionName == "SetActivityInputs"))
                {
                    activityInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(impl.ActionParameter);
                }
                ActivityModel activityModel = new ActivityModel();
                activityModel.IsInitial = activity.IsInitial;
                activityModel.IsFinal = activity.IsFinal;
                activityModel.ActivityName = activity.Name;
                activityModel.ProcessId = processId;
                activityModel.DaysDue = activityInput.DaysDue;
                activityModel.MinorActivityCode = activityInput.MinorActivity;
                _workFlowResponseModel.ListActivityModel.Add(activityModel);
            }
            var workflowCommands = WorkflowInit.Runtime.GetAvailableCommands(processId, string.Empty);
            _caseDetailsModel = _moqData.GetCaseDetails(_caseId);
            _workFlowResponseModel.CurrentActivityName = activityName;
            var activityParam = _workFlowResponseModel.ListActivityModel.Where(a => a.ActivityName == activityName).FirstOrDefault();
            _workFlowResponseModel.CurrentMinorActivityCode = activityParam?.MinorActivityCode;
            _workFlowResponseModel.ProcessId = processId;
            _workFlowResponseModel.CurrentStateName = stateName;
            _workFlowResponseModel.CaseDetailsModel = _caseDetailsModel;
            foreach (var workflowCommand in workflowCommands)
            {
                var commandModel = new CommandModel();
                commandModel.ProcessId = workflowCommand.ProcessId;
                commandModel.CommandName = workflowCommand.CommandName;
                List<ActionModel> actionModels = new List<ActionModel>();
                foreach (var condition in schema.Transitions.Where(a => a.Trigger.Command.Name == workflowCommand.CommandName).Select(t=> t.Conditions))
                {                    
                    foreach (var action in condition.Select(t => t.Action))
                    {
                        if (action != null)
                        {
                            ActionModel actionModel = new ActionModel();
                            actionModel.ProcessId = processId;
                            actionModel.ActionName = action.ActionName;
                            if (action.ActionParameter != null && action.ActionParameter.Contains("{"))
                            {
                                actionModel.InputParameter = JsonSerializer.Deserialize<WorkFlowInputParameter>(action.ActionParameter);
                            }
                            actionModels.Add(actionModel);
                        }
                    }
                }
                commandModel.ListActionModel = actionModels;
                _workFlowResponseModel.ListCommandModel.Add(commandModel);
            }            
        }

        public ActionResult ProcessCommand(string jsonString, string commandName, string notes)
        {
            _workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString);
            _caseId = _workFlowResponseModel?.CaseDetailsModel?.CaseId;
            _workFlowResponseModel.CurrentCommandName = commandName;
            //Parameter List
            WorkflowInit.Runtime.SetPersistentProcessParameter(_processId, _constStrCPROCaseId, _caseId);
            WorkflowInit.Runtime.SetPersistentProcessParameter(_processId, _constStrFamilyViolence, _workFlowResponseModel.CaseDetailsModel?.FamilyViolence);
            WorkflowInit.Runtime.SetPersistentProcessParameter(_processId, _constStrWorkflowResponseModel, _workFlowResponseModel);
            WorkflowInit.Runtime.SetPersistentProcessParameter(_processId, _constStrMinorActivity, _workFlowResponseModel.CurrentMinorActivityCode);
            _workFlowResponseModel = WorkflowInit.WorkflowActionProvider.ExecuteCommand(_workFlowResponseModel, commandName );
            var connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            ActivityDBContext activityDBContext = new ActivityDBContext();
            activityDBContext.ActivityDiaryTableUpdate(connectionString, _caseId, _workFlowResponseModel.MajorActivityCode, _workFlowResponseModel.CurrentMinorActivityCode, _processId);
            GetAllActivitiesAndCommands(_processId);
            if (!string.IsNullOrEmpty(notes))
            {
                SpecialNotes specialNotes = new SpecialNotes();
                specialNotes.ProcessId = _processId;
                specialNotes.CaseId = _caseId;
                specialNotes.Notes = notes;
                specialNotes.MajorActivityCode = _workFlowResponseModel.MajorActivityCode;
                specialNotes.MinorActivityCode = _workFlowResponseModel.CurrentMinorActivityCode;
                _workFlowResponseModel.ListSpecialNotes.Add(specialNotes);
            }
            return View("ActivityChain", _workFlowResponseModel);
        }

        private void DeleteProcess()
        {
            if (_processId == null)
            {
                Console.WriteLine("The process isn't created. Please, create process instance.");
                return;
            }
            WorkflowInit.Runtime.DeleteInstance(_processId);
            Console.WriteLine("DeleteProcess - OK.", _processId);            
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
