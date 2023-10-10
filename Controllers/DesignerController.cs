using System;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OptimaJet.Workflow;
using OptimaJet.Workflow.Core;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.DBContext;
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
            
            try
            {
                var createInstanceParameters = new CreateInstanceParams(majorActivityCode, processId);
                WorkflowInit.Runtime.CreateInstance(majorActivityCode, processId);                
                var schema = WorkflowInit.Runtime.GetProcessScheme(processId);
                var connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
                ActivityDBContext activityDBContext = new ActivityDBContext();
                activityDBContext.ActivityReferenceTableUpdate(connectionString, majorActivityCode, schema);
                WorkflowInit.Runtime.DeleteInstance(processId);                
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);
            }
            return View("Index", getMajorActivityList());
        }


        public IActionResult Index()
        {            
            return View(getMajorActivityList());
        }

        private List<SelectListItem> getMajorActivityList()
        {
            var ConnectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            var majorActivityList = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string sqlQry = @"SELECT ActivityMajor_CODE FROM RefMajorActivity_T1";
                using (SqlCommand command = new SqlCommand(sqlQry, con))
                {
                    command.CommandType = CommandType.Text;
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var majorActivity = new SelectListItem
                            {
                                Value = reader.GetString(0),
                                Text = reader.GetString(0)
                            };
                            majorActivityList.Add(majorActivity);
                        }
                    }
                }
            }
            return majorActivityList;
        }
    }
}