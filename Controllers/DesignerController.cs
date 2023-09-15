using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OptimaJet.Workflow;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.Models;
using WorkflowLib;

namespace WorkflowEngineMVC.Controllers
{
    public class DesignerController : Controller
    {
        private readonly IConfiguration _configuration;
        const string _constStrMinorActivity = "MinorActivity";
        const string _constStrMajorActivity = "MajorActivity";
        const string _constStrCPROCaseId = "CPROCaseId";
        const string _constStrDaysDue = "DaysDue";
        const string _constStrCurrentDate = "CurrentDate";
        const string _constStrFamilyViolence = "FamilyViolence";
        const string _constStrWorkflowResponseModel = "WorkflowResponseModel";
        const string _actionAlertCode = "ActionAlertCode";
        const string _alertWarningInDays = "AlertWarningInDays";
        
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


        [HttpPost("SaveWorkFlowData")]
        public void SaveWorkFlowData(string majorActivityCode)
        {
            var processId = Guid.NewGuid();            
            try
            {
                var createInstanceParameters = new CreateInstanceParams(majorActivityCode, processId);
                WorkflowInit.Runtime.CreateInstance(majorActivityCode, processId);                
                var schema = WorkflowInit.Runtime.GetProcessScheme(processId);

                var ConnectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    foreach (var activity in schema.Activities)
                    {                                                
                        string sqlQry = @"INSERT INTO RefMinorActivity_T1				
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
                                      + schema.GetActivityAnnotation(activity.Name, _constStrMinorActivity) + "'," 
                                      + "'',"
                                      + "'" + activity.Name + "'" +
                                      + Convert.ToDouble(schema.GetActivityAnnotation(activity.Name, _constStrDaysDue)) + ","
                                      + Convert.ToDouble(schema.GetActivityAnnotation(activity.Name, _actionAlertCode)) + ","                                      
                                      +"'',"
                                      + Convert.ToDouble(schema.GetActivityAnnotation(activity.Name, _alertWarningInDays)) + ","
                                      + "'',"
                                      + "'',"
                                      + "'',"
                                      + "'" + DateTime.Now.ToShortDateString() +"'" 
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
                    }
                }
                
                WorkflowInit.Runtime.DeleteInstance(processId);                
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);
            }            
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}