using Microsoft.Data.SqlClient;
using OptimaJet.Workflow.Core.Model;
using System.Data;
using System.Text.Json;
using WorkflowEngineMVC.Models;

namespace WorkflowEngineMVC.DBContext
{
    public class ActivityDBContext
    {
        public ActivityDBContext() { }
        public void ActivityReferenceTableUpdate(string? connectionString, string? majorActivityCode, ProcessDefinition schema)
        {
            try
            {
                int displayOrder = 0;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sqlQry = @"DELETE FROM RefMajorActivity_T1 WHERE ActivityMajor_CODE = '" + majorActivityCode + "';" +
                                  @"DELETE FROM RefMinorActivity_T1;
                                    DELETE FROM RefNextActivity_T1;
                                    DELETE FROM RefMaintenance_T1
                                    DELETE FROM RefActivityCatRole_T1;
                                    DELETE FROM RefActivityFormMaster_T1;";
                    using (SqlCommand command = new SqlCommand(sqlQry, con))
                    {
                        command.CommandType = CommandType.Text;
                        int count = command.ExecuteNonQuery();
                    }

                    sqlQry = @"INSERT INTO RefMajorActivity_T1 
                              (
                                ActivityMajor_CODE	,
                                Subsystem_CODE	,
                                DescriptionActivity_TEXT	,
                                BeginValidity_DATE	,
                                EndValidity_DATE	,
                                WorkerUpdate_ID	,
                                Update_DTTM	,
                                TransactionEventSeq_NUMB	,
                                Stop_INDC	,
                                MultipleActiveInstance_INDC	,
                                ManualStop_INDC	
                              )
                              VALUES
                              (
                                 '" + majorActivityCode + @"', 
                                 'ES',
                                 '" + majorActivityCode + @"',
                                 '" + DateTime.Now.ToShortDateString() + @"',
                                 '12/31/9999',
                                 'KIDSFIRST',
                                 '" + DateTime.Now.ToShortDateString() + @"',	
                                 1,	
                                 'N',
                                 'Y',
                                 'N'
                              )";
                    using (SqlCommand command = new SqlCommand(sqlQry, con))
                    {
                        command.CommandType = CommandType.Text;
                        int count = command.ExecuteNonQuery();
                    }

                    int activityOrder = 0;
                    foreach (var activity in schema.Activities)
                    {
                        int noticeOrder = 0;
                        int reasonOrder = 0;
                        activityOrder++;
                        WorkFlowInputParameter? activityInput = new WorkFlowInputParameter();
                        foreach (var impl in activity.Implementation.Where(a => a.ActionName == "SetActivityInputs"))
                        {
                            activityInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(impl.ActionParameter);
                        }
                        sqlQry = @"INSERT INTO RefMinorActivity_T1				
                                    (
                                        ActivityMinor_CODE,
                                        TypeActivity_CODE,
                                        DescriptionActivity_TEXT,
                                        DayToComplete_QNTY,
                                        ActionAlert_CODE,
                                        Element_ID,
                                        DayAlertWarn_QNTY,
                                        MemberCombinations_CODE,
                                        TypeLocation1_CODE,
                                        TypeLocation2_CODE,
                                        BeginValidity_DATE,
                                        EndValidity_DATE,
                                        WorkerUpdate_ID,
                                        Update_DTTM,
                                        TransactionEventSeq_NUMB,
                                        ScreenFunction_CODE,
                                        BusinessDays_INDC,
                                        CaseJournal_INDC	
                                    )
                                    VALUES
                                      ('"
                                      + activityInput.MinorActivity + "',"
                                      + "'" + activityInput.TypeActivity + "',"
                                      + "'" + activity.Name + "',"
                                      + activityInput.DaysDue + ","
                                      + "'" + activityInput.ActionAlertCode + "',"
                                      + "'',"
                                      + activityInput.AlertWarningInDays + ","
                                      + "'',"
                                      + "'',"
                                      + "'',"
                                      + "'" + DateTime.Now.ToShortDateString() + "',"
                                      + "'12/31/9999',"
                                      + "'KIDSFIRST',"
                                      + "'12/31/9999',"
                                      + "1,"
                                      + "'',"
                                      + "'Y',"
                                      + "'N'"
                                      + ")";
                        using (SqlCommand command = new SqlCommand(sqlQry, con))
                        {
                            command.CommandType = CommandType.Text;
                            int count = command.ExecuteNonQuery();
                        }
                        sqlQry = @"INSERT INTO RefActivityCatRole_T1
                                    (
                                    ActivityMinor_CODE,
                                    Category_CODE,
                                    SubCategory_CODE,
                                    Role_ID,
                                    BeginValidity_DATE,
                                    EndValidity_DATE,
                                    WorkerUpdate_ID,
                                    TransactionEventSeq_NUMB,
                                    Update_DTTM,
                                    ScreenFunction_CODE,
                                    WorkerAssign_INDC,
                                    TypeOfficeAssign_CODE	
                                    )
                                    VALUES
                                    ('"
                                    + activityInput.MinorActivity + "',"
                                    + "'" + activityInput.Category + "',"
                                    + "'" + activityInput.SubCategory + "',"
                                    + "'',"
                                    + "'" + DateTime.Now.ToShortDateString() + "',"
                                    + "'12/31/9999',"
                                    + "'KIDSFIRST',"
                                    + "1,"
                                    + "'',"
                                    + "'" + activityInput.ScreenFunctionCode + "',"
                                    + "'Y',"
                                    + "''"
                                    + ")";
                        using (SqlCommand command = new SqlCommand(sqlQry, con))
                        {
                            command.CommandType = CommandType.Text;
                            int count = command.ExecuteNonQuery();
                        }
                        foreach (var data in schema.Transitions.Where(d => d.From.Name == activity.Name))
                        {
                            WorkFlowInputParameter? toActivityInput = new WorkFlowInputParameter();
                            foreach (var impl in data.To.Implementation.Where(a => a.ActionName == "SetActivityInputs"))
                            {
                                toActivityInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(impl.ActionParameter);
                            }
                            reasonOrder++;
                            sqlQry = @"INSERT INTO RefNextActivity_T1
                                        (
                                        ActivityMajor_CODE,
                                        ActivityMinor_CODE,
                                        ActivityOrder_QNTY,
                                        Reason_CODE,
                                        ReasonOrder_QNTY,"
                                        //ParallelSeq_QNTY	,
                                        //RespManSys_CODE	,
                                        + @"ActivityMajorNext_CODE,
                                        ActivityMinorNext_CODE,
                                        Group_ID,
                                        GroupNext_ID,
                                        BeginValidity_DATE,
                                        EndValidity_DATE,
                                        WorkerUpdate_ID,
                                        Update_DTTM,
                                        TransactionEventSeq_NUMB"
                                       //Function1_CODE	,
                                       //Action1_CODE	,
                                       //Reason1_CODE	,
                                       //Function2_CODE	,
                                       //Action2_CODE	,
                                       //Reason2_CODE	,
                                       //Error_CODE	,
                                       //Procedure_NAME	,
                                       //NavigateTo_CODE	,
                                       //CsenetComment1_TEXT	,
                                       //CsenetComment2_TEXT	,
                                       //Alert_CODE	,
                                       //ScannedDocuments_INDC	,
                                       //RejectionReason_INDC	
                                       + @")
                                        VALUES
                                        ('"
                                       + majorActivityCode + "',"
                                       + "'" + activityInput.MinorActivity + "',"
                                       + activityOrder + ","
                                       + "'" + data.Trigger.Command.InputParameters.Where(a => a.Name == "Reason").FirstOrDefault()?.DefaultValue + "',"
                                       + reasonOrder + ","
                                       + "'" + majorActivityCode + "',"
                                       + "'" + toActivityInput.MinorActivity + "',"
                                       + "'" + activityInput.Group + "',"
                                       + "'" + toActivityInput.Group + "',"
                                       + "'" + DateTime.Now.ToShortDateString() + "',"
                                       + "'12/31/9999',"
                                       + "'KIDSFIRST',"
                                       + "'',"
                                       + "1"
                                       + ")";
                            using (SqlCommand command = new SqlCommand(sqlQry, con))
                            {
                                command.CommandType = CommandType.Text;
                                int count = command.ExecuteNonQuery();
                            }
                            displayOrder++;
                            sqlQry = @"INSERT INTO RefMaintenance_T1
                                        (
                                        Table_ID,
                                        TableSub_ID,
                                        DescriptionTable_TEXT,
                                        Value_CODE,
                                        DescriptionValue_TEXT,
                                        DispOrder_NUMB,
                                        BeginValidity_DATE,
                                        WorkerUpdate_ID,
                                        TransactionEventSeq_NUMB,
                                        Update_DTTM	
                                        )
                                        VALUES
                                        ('CPRO',
                                         'REAS',
                                        'ACTIVITY REASON',"
                                       + "'" + data.Trigger.Command.InputParameters.Where(a => a.Name == "Reason").FirstOrDefault()?.DefaultValue + "',"
                                       + "'" + data.Trigger.Command.Name + "',"
                                       + displayOrder + ","
                                       + "'" + DateTime.Now.ToShortDateString() + "',"
                                       + "'KIDSFIRST',"
                                       + "1,"
                                       + "'" + DateTime.Now.ToShortDateString() + "'"
                                       + ")";
                            using (SqlCommand command = new SqlCommand(sqlQry, con))
                            {
                                command.CommandType = CommandType.Text;
                                int count = command.ExecuteNonQuery();
                            }

                            foreach (var dataOut in data.Conditions.Where(d => d.Action?.ActionName == "GenerateNotice"))
                            {
                                noticeOrder++;
                                WorkFlowInputParameter NoticeInput = JsonSerializer.Deserialize<WorkFlowInputParameter>(dataOut.Action.ActionParameter) ?? new WorkFlowInputParameter();
                                sqlQry = @"INSERT INTO RefActivityFormMaster_T1
                                    (
                                    ActivityMinor_CODE,
                                    Notice_ID,
                                    BeginValidity_DATE,
                                    EndValidity_DATE,
                                    WorkerUpdate_ID,
                                    Update_DTTM,
                                    TransactionEventSeq_NUMB,
                                    ApprovalRequired_INDC,
                                    TypeEditNotice_CODE,
                                    ActivityMajor_CODE,
                                    Reason_CODE,
                                    ActivityMajorNext_CODE,
                                    ActivityMinorNext_CODE,
                                    NoticeOrder_NUMB,                                    
                                    Recipient_CODE,
                                    RecipientSeq_NUMB,
                                    TypeService_CODE,
                                    PrintMethod_CODE,                                    	
                                    Mask_INDC	
                                    )
                                    VALUES
                                    ('"
                                        + activityInput.MinorActivity + "',"
                                        + "'" + NoticeInput.NoticeId + "',"
                                        + "'" + DateTime.Now.ToShortDateString() + "',"
                                        + "'12/31/9999',"
                                        + "'KIDSFIRST',"
                                        + "'',"
                                        + "1,"
                                        + "'N',"
                                        + "'O',"
                                        + "'" + majorActivityCode + "',"
                                        + "'" + data.Trigger.Command.InputParameters.Where(a => a.Name == "Reason").FirstOrDefault()?.DefaultValue + "',"
                                        + "'" + majorActivityCode + "',"
                                        + "'" + toActivityInput.MinorActivity + "',"
                                        + noticeOrder + ","
                                        + "'" + NoticeInput.NoticeRecipient + "',"
                                        + "1,"
                                        + "'R',"
                                        + "'" + NoticeInput.PrintMethod + "',"
                                        + "'N'"
                                 + ")";
                                using (SqlCommand command = new SqlCommand(sqlQry, con))
                                {
                                    command.CommandType = CommandType.Text;
                                    int count = command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);
            }
        }

        public void ActivityDiaryTableUpdate(string? connectionString, string? majorActivityCode)
        {
            try
            {
                int displayOrder = 0;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                   
                    string sqlQry = @"INSERT INTO MajorActivityDiary_T1 
                              (
                                Case_IDNO	,
                                OrderSeq_NUMB	,
                                MajorIntSeq_NUMB	,
                                MemberMci_IDNO	,
                                ActivityMajor_CODE	,
                                Subsystem_CODE	,
                                TypeOthpSource_CODE	,
                                OthpSource_IDNO	,
                                Entered_DATE	,
                                Status_DATE	,
                                Status_CODE	,
                                ReasonStatus_CODE	,
                                BeginExempt_DATE	,
                                EndExempt_DATE	,
                                Forum_IDNO	,
                                TotalTopics_QNTY	,
                                PostLastPoster_IDNO	,
                                UserLastPoster_ID	,
                                SubjectLastPoster_TEXT	,
                                LastPost_DTTM	,
                                BeginValidity_DATE	,
                                WorkerUpdate_ID	,
                                Update_DTTM	,
                                TransactionEventSeq_NUMB	,
                                TypeReference_CODE	,
                                Reference_ID	
                              )
                              VALUES
                              (
                                 '" + majorActivityCode + @"', 
                                 'ES',
                                 '" + majorActivityCode + @"',
                                 '" + DateTime.Now.ToShortDateString() + @"',
                                 '12/31/9999',
                                 'KIDSFIRST',
                                 '" + DateTime.Now.ToShortDateString() + @"',	
                                 1,	
                                 'N',
                                 'Y',
                                 'N'
                              )";
                    using (SqlCommand command = new SqlCommand(sqlQry, con))
                    {
                        command.CommandType = CommandType.Text;
                        int count = command.ExecuteNonQuery();
                    }

                    sqlQry = @"INSERT INTO MinorActivityDiary_T1 
                              (
                                Case_IDNO	,
                                OrderSeq_NUMB	,
                                MajorIntSeq_NUMB	,
                                MinorIntSeq_NUMB	,
                                MemberMci_IDNO	,
                                ActivityMinor_CODE	,
                                ActivityMinorNext_CODE	,
                                Entered_DATE	,
                                Due_DATE	,
                                Status_DATE	,
                                Status_CODE	,
                                ReasonStatus_CODE	,
                                Schedule_NUMB	,
                                Forum_IDNO	,
                                Topic_IDNO	,
                                TotalReplies_QNTY	,
                                TotalViews_QNTY	,
                                PostLastPoster_IDNO	,
                                UserLastPoster_ID	,
                                LastPost_DTTM	,
                                AlertPrior_DATE	,
                                BeginValidity_DATE	,
                                WorkerUpdate_ID	,
                                Update_DTTM	,
                                TransactionEventSeq_NUMB	,
                                WorkerDelegate_ID	,
                                ActivityMajor_CODE	,
                                Subsystem_CODE	
                              )
                              VALUES
                              (
                                 '" + majorActivityCode + @"', 
                                 'ES',
                                 '" + majorActivityCode + @"',
                                 '" + DateTime.Now.ToShortDateString() + @"',
                                 '12/31/9999',
                                 'KIDSFIRST',
                                 '" + DateTime.Now.ToShortDateString() + @"',	
                                 1,	
                                 'N',
                                 'Y',
                                 'N'
                              )";
                    using (SqlCommand command = new SqlCommand(sqlQry, con))
                    {
                        command.CommandType = CommandType.Text;
                        int count = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateInstance - Exception: {0}", ex.Message);
            }
        }
    }
}
