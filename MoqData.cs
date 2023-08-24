using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC
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
            return gTSTSchedulerModel;
        }
    }
}
