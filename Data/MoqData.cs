using Microsoft.CodeAnalysis.Operations;
using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC.Data
{
    public class MoqData
    {
        public GTSTSchedulerModel GetGTSTScheduleDetails()
        {
            GTSTSchedulerModel gTSTSchedulerModel = new GTSTSchedulerModel();
            gTSTSchedulerModel.FirstName = "John";
            gTSTSchedulerModel.LastName = "Joseph";
            gTSTSchedulerModel.LastName = "Joseph";
            gTSTSchedulerModel.ScheduledDate = DateTime.Now.AddDays(1);
            gTSTSchedulerModel.ScheduledTime = DateTime.Now.ToShortTimeString();
            gTSTSchedulerModel.MemberId = "1000230";
            gTSTSchedulerModel.CaseId = "100016";
            return gTSTSchedulerModel;
        }

        public CaseDetailsModel getCaseDetails()
        {
            CaseDetailsModel CaseDetails = new CaseDetailsModel();
            CaseDetails.CP = "1000230";
            CaseDetails.NCP = "1230563";
            CaseDetails.CaseId = "100016";

            return CaseDetails;
        }
    }
}
