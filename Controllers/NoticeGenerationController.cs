using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC.Controllers
{
    public class NoticeGenerationController : Controller
    {
        NoticeGenerationModel noticeGenerationModel;
        MoqData moqData;
        public NoticeGenerationController()
        {
            moqData = new MoqData();
            noticeGenerationModel = new NoticeGenerationModel();
        }
            // GET: NoticeGenerationController
        public NoticeGenerationModel Show(string noticeId, string? caseId)
        {
            noticeGenerationModel = moqData.GetNoticeGenerationDetails(caseId);
            return noticeGenerationModel;
        }
        // GET: GTSTSchedulerController
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Save(Guid processId, string caseId)
        {
            if (processId != Guid.Empty)
            {
                return RedirectToAction("ShowProcessListView", "CPROChain", new { processId, caseId });
            }
            else
            {
                return RedirectToAction("Index");
            }
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
