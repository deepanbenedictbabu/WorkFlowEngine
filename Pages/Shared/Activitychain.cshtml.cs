using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimaJet.Workflow.Core.Persistence;
using System.Net.Http.Headers;
using WorkflowEngineMVC.Controllers;

namespace WorkflowEngineMVC.Pages
{
    public class ActivitychainModel : PageModel
    {
        public void OnGetProcessCommand()//string commandName, Guid processId
        {
            Guid processId = new Guid();
            CPROChainController cproChainController = new CPROChainController();
            cproChainController.ProcessCommand("Schedule Genetic Test", processId);//commandName, processId
        }
        public string Message { get; set; }

        // the click event handler 
        // MUST be prefixed with OnGet if the 
        // request is of HTTP GET type 
        public void OnGetOnClick()
        {

            Message = "Hello";

        }
    }
}
