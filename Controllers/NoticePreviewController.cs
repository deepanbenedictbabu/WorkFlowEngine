using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WorkflowEngineMVC.Controllers
{
    public class NoticePreviewController : Controller
    {
        // GET: NoticePreviewController
        public ActionResult Index()
        {
            return View();
        }

        // GET: NoticePreviewController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NoticePreviewController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NoticePreviewController/Create
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

        // GET: NoticePreviewController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NoticePreviewController/Edit/5
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

        // GET: NoticePreviewController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NoticePreviewController/Delete/5
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
