using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System.ComponentModel.Design;
using System.Text.Json;
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
        public NoticeGenerationModel Show(string noticeId, string? caseId, string? noticeRecipient)
        {
            noticeGenerationModel = moqData.GetNoticeGenerationDetails(noticeId, caseId);
            noticeGenerationModel.NoticeRecipient = noticeRecipient;
            //Implement the Notice generation logic here
            return noticeGenerationModel;
        }
        // GET: GTSTSchedulerController
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GenerateNotice(string jsonString, string? commandName)
        {
            //Save the notice details to db            
            return RedirectToAction("ProcessCommand", "CPROChain", new { jsonString, commandName });
        }

        public ActionResult ShowGenerateNoticeForm(string jsonString, string? commandName, string? noticeId)
        {
            var workFlowResponseModel = JsonSerializer.Deserialize<WorkFlowResponseModel>(jsonString) ?? new WorkFlowResponseModel();
            var inputParam = workFlowResponseModel.ListCommandModel.Where(c => c.CommandName == commandName).Select(p => p.ListActionModel).FirstOrDefault();
            var noticeInput = inputParam?.Where(a=> a.InputParameter?.NoticeId== noticeId).Select(i=>i.InputParameter).FirstOrDefault();
            noticeGenerationModel = moqData.GetNoticeGenerationDetails(noticeInput?.NoticeId, workFlowResponseModel?.CaseDetailsModel?.CaseId); ;
            noticeGenerationModel.NoticeRecipient = noticeInput.NoticeRecipient;
            workFlowResponseModel.ScreenName = "NoticeGeneration";
            workFlowResponseModel.CurrentCommandName = commandName;            
            workFlowResponseModel.CurrentNoticeId = noticeId;
            workFlowResponseModel.ListNoticeGenerationModel.Add(noticeGenerationModel);
            //Implement the Notice generation logic here
            //Save the notice details to db
            jsonString = JsonSerializer.Serialize(workFlowResponseModel);
            return RedirectToAction("UpdateActivity", "CPROChain", new { jsonString });
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
