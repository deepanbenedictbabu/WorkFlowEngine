using Microsoft.CodeAnalysis.Operations;
using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC.Data
{
    public class MoqData
    {
        public GTSTSchedulerModel GetGTSTScheduleDetails(string? caseId)
        {
            CaseDetailsModel? caseDetails = GetCaseDetails(caseId);
            GTSTSchedulerModel gTSTSchedulerModel = new GTSTSchedulerModel();
            gTSTSchedulerModel.MemberId = caseDetails?.CP;
            gTSTSchedulerModel.CaseId = caseDetails?.CaseId;
            gTSTSchedulerModel.FirstName = caseDetails?.CPFirstName;            
            gTSTSchedulerModel.LastName = caseDetails?.CPLastName;
            gTSTSchedulerModel.ScheduledDate = DateTime.Now.AddDays(1);
            gTSTSchedulerModel.ScheduledTime = DateTime.Now.ToShortTimeString();            
            return gTSTSchedulerModel;
        }

        public CaseDetailsModel? GetCaseDetails(string? caseId)
        {
            List<CaseDetailsModel> caseDetailsModels= new List<CaseDetailsModel>();            
            CaseDetailsModel caseDetails1 = new CaseDetailsModel();
            caseDetails1.CP = "1054230";
            caseDetails1.NCP = "1980512";
            caseDetails1.CaseId = "100010";
            caseDetails1.CPFirstName = "Sam";
            caseDetails1.CPLastName = "Luther";
            caseDetails1.NCPFirstName = "Jame";
            caseDetails1.NCPLastName = "Mike";
            caseDetailsModels.Add(caseDetails1);

            CaseDetailsModel caseDetails2 = new CaseDetailsModel();
            caseDetails2.CP = "1000230";
            caseDetails2.NCP = "1230563";
            caseDetails2.CaseId = "100016";
            caseDetails2.CPFirstName = "John";
            caseDetails2.CPLastName = "Joseph";
            caseDetails2.NCPFirstName = "Miller";
            caseDetails2.NCPLastName = "Peter";
            caseDetailsModels.Add(caseDetails2);

            CaseDetailsModel? caseDetails = new CaseDetailsModel();
            caseDetails = caseDetailsModels.Where(c => c.CaseId == caseId).FirstOrDefault();

            return caseDetails;
        }

        public NoticeGenerationModel GetNoticeGenerationDetails(string? caseId)
        {
            CaseDetailsModel? caseDetails = GetCaseDetails(caseId);
            NoticeGenerationModel noticeGenerationModel = new NoticeGenerationModel();
            noticeGenerationModel.MemberId = caseDetails?.CP;
            noticeGenerationModel.CaseId = caseDetails?.CaseId;
            noticeGenerationModel.CPFirstName = caseDetails?.CPFirstName;            
            noticeGenerationModel.CPLastName = caseDetails?.CPLastName;                        
            noticeGenerationModel.CPDOB = "10/06/1990";            
            noticeGenerationModel.CPAddress = "5201 NE 20Th Street";
            noticeGenerationModel.CPZip = "66614";
            noticeGenerationModel.CPCity = "Topeka";
            noticeGenerationModel.CPState = "KS";
            noticeGenerationModel.NCPFirstName = caseDetails?.NCPFirstName;
            noticeGenerationModel.NCPLastName = caseDetails?.NCPLastName;
            noticeGenerationModel.NCPDOB = "10/06/1980";
            noticeGenerationModel.NCPAddress = "3001 SE 10Th Street";
            noticeGenerationModel.NCPZip = "66602";
            noticeGenerationModel.NCPCity = "Topeka";
            noticeGenerationModel.NCPState = "KS";
            return noticeGenerationModel;
        }

        public GTSTTestResultsModel GetGTSTTestResults(string? caseId)
        {
            CaseDetailsModel? caseDetails = GetCaseDetails(caseId);
            GTSTTestResultsModel gtstTestResultsModel = new GTSTTestResultsModel();
            gtstTestResultsModel.FirstName = caseDetails?.CPFirstName;
            gtstTestResultsModel.LastName = caseDetails?.CPLastName;
            gtstTestResultsModel.MemberId = caseDetails?.CP;
            gtstTestResultsModel.CaseId = caseDetails?.CaseId;
            gtstTestResultsModel.TestReslt = "Genetic Test Result Values";
            return gtstTestResultsModel;
        }
    }
}
