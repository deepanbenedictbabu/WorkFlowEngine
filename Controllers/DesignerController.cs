using System;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OptimaJet.Workflow;
using OptimaJet.Workflow.Core;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.Models;
using WorkflowLib;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkflowEngineMVC.Controllers
{
    public class DesignerController : Controller
    {
        private readonly IConfiguration _configuration;        
        const string _constStrCPROCaseId = "CPROCaseId";        
        const string _constStrFamilyViolence = "FamilyViolence";
        const string _constStrWorkflowResponseModel = "WorkflowResponseModel";
        
        public DesignerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Api()
        {
            Stream? filestream = null;
            var parameters = new NameValueCollection();

            //Defining the request method
            var isPost = Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase);

            //Parse the parameters in the query string
            foreach (var q in Request.Query)
            {
                parameters.Add(q.Key, q.Value.First());
            }

            if (isPost)
            {
                //Parsing the parameters passed in the form
                var keys = parameters.AllKeys;

                foreach (var key in Request.Form.Keys)
                {
                    if (!keys.Contains(key))
                    {
                        parameters.Add(key, Request.Form[key]);
                    }
                }

                //If a file is passed
                if (Request.Form.Files.Count > 0)
                {
                    //Save file
                    filestream = Request.Form.Files[0].OpenReadStream();
                }
            }

            //Calling the Designer Api and store answer
            var (result, hasError) = await WorkflowInit.Runtime.DesignerAPIAsync(parameters, filestream);

            //If it returns a file, send the response in a special way
            if (parameters["operation"]?.ToLower() == "downloadscheme" && !hasError)
                return File(Encoding.UTF8.GetBytes(result), "text/xml");

            if (parameters["operation"]?.ToLower() == "downloadschemebpmn" && !hasError)
                return File(Encoding.UTF8.GetBytes(result), "text/xml");

            //response
            return Content(result);
        }
        
        public IActionResult SaveWorkFlowData(string majorActivityCode)
        {
            var processId = Guid.NewGuid();
            int displayOrder = 0;
            try
            {
                var createInstanceParameters = new CreateInstanceParams(majorActivityCode, processId);
                WorkflowInit.Runtime.CreateInstance(majorActivityCode, processId);                
                var schema = WorkflowInit.Runtime.GetProcessScheme(processId);
                var ConnectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");                
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string sqlQry = @"DELETE FROM RefMinorActivity_T1;
                                    DELETE FROM RefNextActivity_T1;
                                    DELETE FROM RefMaintenance_T1
                                    DELETE FROM RefActivityCatRole_T1;
                                    DELETE FROM RefActivityFormMaster_T1;";
                    using (SqlCommand command = new SqlCommand(sqlQry, con))
                    {
                        command.CommandType = CommandType.Text;
                        int count = command.ExecuteNonQuery();
                    }
                    int activityOrder = 0;
                    foreach (var activity in schema.Activities)
                    {
                        int noticeOrder = 0;
                        int reasonOrder = 0;
                        activityOrder++;
                        WorkFlowInputParameter? activityInput = new WorkFlowInputParameter();
                        foreach (var impl in activity.Implementation.Where(a => a.ActionName == "SetActivityInputs"))
                        {
                            activityInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(impl.ActionParameter);
                        }
                        sqlQry = @"INSERT INTO RefMinorActivity_T1				
                                    (
                                        ActivityMinor_CODE,
                                        TypeActivity_CODE,
                                        DescriptionActivity_TEXT,
                                        DayToComplete_QNTY,
                                        ActionAlert_CODE,
                                        Element_ID,
                                        DayAlertWarn_QNTY,
                                        MemberCombinations_CODE,
                                        TypeLocation1_CODE,
                                        TypeLocation2_CODE,
                                        BeginValidity_DATE,
                                        EndValidity_DATE,
                                        WorkerUpdate_ID,
                                        Update_DTTM,
                                        TransactionEventSeq_NUMB,
                                        ScreenFunction_CODE,
                                        BusinessDays_INDC,
                                        CaseJournal_INDC	
                                    )
                                    VALUES
                                      ('"
                                      + activityInput.MinorActivity + "'," 
                                      + "'',"
                                      + "'" + activity.Name + "'," 
                                      +  activityInput.DaysDue + ","
                                      + "'" + activityInput.ActionAlertCode + "',"                                      
                                      + "'',"
                                      + activityInput.AlertWarningInDays + ","
                                      + "'',"
                                      + "'',"
                                      + "'',"
                                      + "'" + DateTime.Now.ToShortDateString() + "'," 
                                      + "'12/31/9999',"
                                      + "'KIDSFIRST',"
                                      + "'12/31/9999',"
                                      + "1,"
                                      + "'',"
                                      + "'Y',"
                                      + "'N'" 
                                      + ")";
                        using (SqlCommand command = new SqlCommand(sqlQry, con))
                        {
                            command.CommandType = CommandType.Text;
                            int count = command.ExecuteNonQuery();
                        }                        
                        sqlQry = @"INSERT INTO RefActivityCatRole_T1
                                    (
                                    ActivityMinor_CODE,
                                    Category_CODE,
                                    SubCategory_CODE,
                                    Role_ID,
                                    BeginValidity_DATE,
                                    EndValidity_DATE,
                                    WorkerUpdate_ID,
                                    TransactionEventSeq_NUMB,
                                    Update_DTTM,
                                    ScreenFunction_CODE,
                                    WorkerAssign_INDC,
                                    TypeOfficeAssign_CODE	
                                    )
                                    VALUES
                                    ('"
                                    + activityInput.MinorActivity + "',"
                                    + "'" + activityInput.Category + "',"
                                    + "'" + activityInput.SubCategory + "',"                                    
                                    + "'',"
                                    + "'" + DateTime.Now.ToShortDateString() + "',"
                                    + "'12/31/9999',"
                                    + "'KIDSFIRST',"
                                    + "1,"
                                    + "'',"
                                    + "'" + activityInput.ScreenFunctionCode + "',"
                                    + "'Y',"
                                    + "''"
                                    + ")";
                        using (SqlCommand command = new SqlCommand(sqlQry, con))
                        {
                            command.CommandType = CommandType.Text;
                            int count = command.ExecuteNonQuery();
                        }
                        foreach (var data in schema.Transitions.Where(d => d.From.Name == activity.Name))
                        {
                            WorkFlowInputParameter? toActivityInput = new WorkFlowInputParameter();
                            foreach (var impl in data.To.Implementation.Where(a => a.ActionName == "SetActivityInputs"))
                            {
                                toActivityInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(impl.ActionParameter);
                            }
                            reasonOrder++;
                            sqlQry = @"INSERT INTO RefNextActivity_T1
                                        (
                                        ActivityMajor_CODE,
                                        ActivityMinor_CODE,
                                        ActivityOrder_QNTY,
                                        Reason_CODE,
                                        ReasonOrder_QNTY,"
                                        //ParallelSeq_QNTY	,
                                        //RespManSys_CODE	,
                                        + @"ActivityMajorNext_CODE,
                                        ActivityMinorNext_CODE,
                                        Group_ID,
                                        GroupNext_ID,
                                        BeginValidity_DATE,
                                        EndValidity_DATE,
                                        WorkerUpdate_ID,
                                        Update_DTTM,
                                        TransactionEventSeq_NUMB"
                                        //Function1_CODE	,
                                        //Action1_CODE	,
                                        //Reason1_CODE	,
                                        //Function2_CODE	,
                                        //Action2_CODE	,
                                        //Reason2_CODE	,
                                        //Error_CODE	,
                                        //Procedure_NAME	,
                                        //NavigateTo_CODE	,
                                        //CsenetComment1_TEXT	,
                                        //CsenetComment2_TEXT	,
                                        //Alert_CODE	,
                                        //ScannedDocuments_INDC	,
                                        //RejectionReason_INDC	
                                       + @")
                                        VALUES
                                        ('"
                                       + majorActivityCode + "',"
                                       + "'" + activityInput.MinorActivity + "',"
                                       + activityOrder + ","                                       
                                       + "'" + data.Trigger.Command.InputParameters.Where(a => a.Name == "Reason").FirstOrDefault()?.DefaultValue + "',"
                                       + reasonOrder + ","
                                       + "'" + majorActivityCode + "',"
                                       + "'" + toActivityInput.MinorActivity + "',"
                                       + "'" + activityInput.Group + "',"
                                       + "'" + toActivityInput.Group + "',"
                                       + "'" + DateTime.Now.ToShortDateString() + "',"
                                       + "'12/31/9999',"
                                       + "'KIDSFIRST',"
                                       + "'',"
                                       + "1"
                                       + ")";
                            using (SqlCommand command = new SqlCommand(sqlQry, con))
                            {
                                command.CommandType = CommandType.Text;
                                int count = command.ExecuteNonQuery();
                            }
                            displayOrder++;
                            sqlQry = @"INSERT INTO RefMaintenance_T1
                                        (
                                        Table_ID,
                                        TableSub_ID,
                                        DescriptionTable_TEXT,
                                        Value_CODE,
                                        DescriptionValue_TEXT,
                                        DispOrder_NUMB,
                                        BeginValidity_DATE,
                                        WorkerUpdate_ID,
                                        TransactionEventSeq_NUMB,
                                        Update_DTTM	
                                        )
                                        VALUES
                                        ('CPRO',
                                         'REAS',
                                        'ACTIVITY REASON',"                                       
                                       + "'" + data.Trigger.Command.InputParameters.Where(a => a.Name == "Reason").FirstOrDefault()?.DefaultValue + "',"
                                       + "'" + data.Trigger.Command.Name + "',"
                                       + displayOrder + ","
                                       + "'" + DateTime.Now.ToShortDateString() + "',"                                       
                                       + "'KIDSFIRST',"                                       
                                       + "1,"
                                       + "'" + DateTime.Now.ToShortDateString() + "'"
                                       + ")";
                            using (SqlCommand command = new SqlCommand(sqlQry, con))
                            {
                                command.CommandType = CommandType.Text;
                                int count = command.ExecuteNonQuery();
                            }

                            foreach (var dataOut in data.Conditions.Where(d => d.Action?.ActionName == "GenerateNotice"))
                            {
                                noticeOrder++;
                                WorkFlowInputParameter NoticeInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(dataOut.Action.ActionParameter) ?? new WorkFlowInputParameter();
                                sqlQry = @"INSERT INTO RefActivityFormMaster_T1
                                    (
                                    ActivityMinor_CODE,
                                    Notice_ID,
                                    BeginValidity_DATE,
                                    EndValidity_DATE,
                                    WorkerUpdate_ID,
                                    Update_DTTM,
                                    TransactionEventSeq_NUMB,
                                    ApprovalRequired_INDC,
                                    TypeEditNotice_CODE,
                                    ActivityMajor_CODE,
                                    Reason_CODE,
                                    ActivityMajorNext_CODE,
                                    ActivityMinorNext_CODE,
                                    NoticeOrder_NUMB,                                    
                                    Recipient_CODE,
                                    RecipientSeq_NUMB,
                                    TypeService_CODE,
                                    PrintMethod_CODE,                                    	
                                    Mask_INDC	
                                    )
                                    VALUES
                                    ('"
                                        + activityInput.MinorActivity + "',"
                                        + "'" + NoticeInput.NoticeId + "',"
                                        + "'" + DateTime.Now.ToShortDateString() + "',"
                                        + "'12/31/9999',"
                                        + "'KIDSFIRST',"
                                        + "'',"
                                        + "1,"
                                        + "'N',"
                                        + "'O',"
                                        + "'" + majorActivityCode + "',"                                        
                                        + "'" + data.Trigger.Command.InputParameters.Where(a => a.Name == "Reason").FirstOrDefault()?.DefaultValue + "',"
                                        + "'" + majorActivityCode + "',"
                                        + "'" + toActivityInput.MinorActivity + "',"
                                        + noticeOrder + ","
                                        + "'" + NoticeInput.NoticeRecipient + "',"
                                        + "1,"
                                        + "'R',"
                                        + "'" + NoticeInput.PrintMethod + "',"
                                        + "'N'"
                                 + ")";
                                using (SqlCommand command = new SqlCommand(sqlQry, con))
                                {
                                    command.CommandType = CommandType.Text;
                                    int count = command.ExecuteNonQuery();
                                }
                            }
                        }
                    }                    
                }
                
                WorkflowInit.Runtime.DeleteInstance(processId);                
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);
            }
            return View("Index");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}