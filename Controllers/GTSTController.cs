using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WorkflowEngineMVC.Controllers
{
    public class GTSTController : Controller
    {
        // GET: GTSTController
        public ActionResult Index()
        {
            return View();
        }

        // GET: GTSTController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GTSTController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GTSTController/Create
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

        // GET: GTSTController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GTSTController/Edit/5
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

        // GET: GTSTController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GTSTController/Delete/5
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
