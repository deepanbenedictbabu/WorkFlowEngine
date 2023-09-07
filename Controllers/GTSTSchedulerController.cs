using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow.Core.Persistence;
using System.Text.Json;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.Models;
using WorkflowLib;

namespace WorkflowEngineMVC.Controllers
{
    public class GTSTSchedulerController : Controller
    {
        GTSTSchedulerModel gtstSchedulerModel;        
        MoqData moqData;
        public GTSTSchedulerController()
        {
            moqData = new MoqData();
            gtstSchedulerModel = new GTSTSchedulerModel();
        }

        // GET: GTSTSchedulerController/Show
        public GTSTSchedulerModel Show(string? caseId)
        {            
            gtstSchedulerModel = moqData.GetGTSTScheduleDetails(caseId);   
            //Show the screen with all the case and schedule details
            return gtstSchedulerModel;
        }
        // GET: GTSTSchedulerController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveSchedule(string jsonString, string? commandName)
        {               
            //Save the schedule details to db            
            return RedirectToAction("ProcessCommand", "CPROChain", new { jsonString, commandName });                    
        }
        
        // GET: GTSTSchedulerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GTSTSchedulerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GTSTSchedulerController/Create
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

        // GET: GTSTSchedulerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GTSTSchedulerController/Edit/5
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

        // GET: GTSTSchedulerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GTSTSchedulerController/Delete/5
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
