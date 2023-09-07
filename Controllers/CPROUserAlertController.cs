using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC.Controllers
{
    public class CPROUserAlertController : Controller
    {
        CPROUserAlertModel cproUserAlertModel;
        MoqData moqData;
        public CPROUserAlertController()
        {
            moqData = new MoqData();
            cproUserAlertModel = new CPROUserAlertModel();
        }
            // GET: NoticeGenerationController
        public CPROUserAlertModel GenerateAlert(string? caseId, string message)
        {
            cproUserAlertModel = moqData.GetUserAlerts(caseId);
            cproUserAlertModel.AlertMessage = message;
            return cproUserAlertModel;
        }
        // GET: GTSTSchedulerController
        public ActionResult Index()
        {
            return View();
        }       

        // GET: NoticeGenerationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NoticeGenerationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NoticeGenerationController/Create
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

        // GET: NoticeGenerationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NoticeGenerationController/Edit/5
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

        // GET: NoticeGenerationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NoticeGenerationController/Delete/5
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
