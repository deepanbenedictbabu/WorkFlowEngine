using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public GTSTSchedulerModel Show()
        {
            gtstSchedulerModel = moqData.GetGTSTScheduleDetails();            
            return gtstSchedulerModel;
        }
        // GET: GTSTSchedulerController
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Save(Guid processId)
        {
            if (processId != Guid.Empty)
            {                
                return RedirectToAction("ShowProcessListView", "CPROChain", new { processId } );
            }
            else
            {
                return RedirectToAction("Index");
            }            
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
