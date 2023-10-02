using System;
using System.Xml.Linq;
using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Bus;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.DbPersistence;
using WorkflowEngineMVC;

namespace WorkflowLib
{
    public static class WorkflowInit
    {
        private static readonly Lazy<WorkflowRuntime> LazyRuntime = new Lazy<WorkflowRuntime>(InitWorkflowRuntime);
        public static WorkflowActionProvider WorkflowActionProvider = new WorkflowActionProvider();
        public static WorkflowRuntime Runtime
        {
            get { return LazyRuntime.Value; }
        }

        public static string ConnectionString { get; set; }

        private static WorkflowRuntime InitWorkflowRuntime()
        {
            // TODO Uncomment for .NET Framework if you don't set ConnectionString externally.
            //ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new Exception("Please init ConnectionString before calling the Runtime!");
            }
            // TODO If you have a license key, you have to register it here
            WorkflowRuntime.RegisterLicense("Conduent-Q29uZHVlbnQ6MTAuMjYuMjAyMzpleUpOWVhoT2RXMWlaWEpQWmtGamRHbDJhWFJwWlhNaU9pMHhMQ0pOWVhoT2RXMWlaWEpQWmxSeVlXNXphWFJwYjI1eklqb3RNU3dpVFdGNFRuVnRZbVZ5VDJaVFkyaGxiV1Z6SWpvdE1Td2lUV0Y0VG5WdFltVnlUMlpVYUhKbFlXUnpJam90TVN3aVRXRjRUblZ0WW1WeVQyWkRiMjF0WVc1a2N5STZMVEY5OkNjOXVlcTBqUVk4OGgwL3RnZXY2ZFpsalVJVG9yb1hNY0RycnZ4Vm5NM1N5a0FvTUtLNlJqOEp6QVRuSkora21FcS9ZQmxuT1ZmYnNDS0htY0VwelR0Q0JHZ3lCV2pwVVR6QTVVOHpmNjZMR0ZjMng3UTc5andVMHlHU01BWWhRdXdEYURjeFM0SVhhSVFzNWttQ3N0dVBrbkJ3ZzVwd2lhdTZ3ODV3Q2dmVT0=");

            // TODO If you are using database different from SQL Server you have to use different persistence provider here.
            var dbProvider = new MSSQLProvider(ConnectionString);

            var builder = new WorkflowBuilder<XElement>(
                dbProvider,
                new OptimaJet.Workflow.Core.Parser.XmlWorkflowParser(),
                dbProvider
            ).WithDefaultCache();

            var runtime = new WorkflowRuntime()
                .WithBuilder(builder)
                .WithPersistenceProvider(dbProvider)
                .WithActionProvider(WorkflowActionProvider)
                .EnableCodeActions()
                //.CodeActionsDebugOn()
                .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn()
                .WithDesignerParameterFormatProvider(new DesignerParameterFormatProvider())
                .AsSingleServer();

            var plugin = new OptimaJet.Workflow.Plugins.BasicPlugin();
            // Settings for SendEmail actions
            // plugin.Setting_Mailserver = "smtp.yourserver.com";
            // plugin.Setting_MailserverPort = 25;
            // plugin.Setting_MailserverFrom = "from@yourserver.com";
            // plugin.Setting_MailserverLogin = "login@yourserver.com";
            // plugin.Setting_MailserverPassword = "pass";
            // plugin.Setting_MailserverSsl = true;
            runtime.WithPlugin(plugin);

            // events subscription
            runtime.ProcessActivityChanged += (sender, args) => { };
            runtime.ProcessStatusChanged += (sender, args) => { };
            // TODO If you have planned to use Code Actions functionality that required references to external assemblies
            // you have to register them here
            //runtime.RegisterAssemblyForCodeActions(Assembly.GetAssembly(typeof(SomeTypeFromMyAssembly)));

            // starts the WorkflowRuntime
            // TODO If you have planned use Timers the best way to start WorkflowRuntime is somewhere outside
            // of this function in Global.asax for example
            runtime.Start();

            return runtime;
        }
    }
}