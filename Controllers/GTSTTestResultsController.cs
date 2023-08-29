using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC.Controllers
{
    public class GTSTTestResultsController : Controller
    {
        GTSTTestResultsModel gtstTestResultsModel;
        MoqData moqData;
        public GTSTTestResultsController()
        {
            moqData = new MoqData();
            gtstTestResultsModel = new GTSTTestResultsModel();
        }

        // GET: GTSTSchedulerController/Show
        public GTSTTestResultsModel Show(string? caseId)
        {
            gtstTestResultsModel = moqData.GetGTSTTestResults(caseId);
            return gtstTestResultsModel;
        }
        // GET: GTSTTestResultsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: GTSTTestResultsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GTSTTestResultsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GTSTTestResultsController/Create
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

        // GET: GTSTTestResultsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GTSTTestResultsController/Edit/5
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

        // GET: GTSTTestResultsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GTSTTestResultsController/Delete/5
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
