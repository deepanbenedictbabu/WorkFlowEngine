using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;

namespace WorkflowEngineMVC
{
    public class DesignerParameterFormatProvider : IDesignerParameterFormatProvider
    {
        public List<CodeActionParameterDefinition> GetFormat(CodeActionType type, string name, string schemeCode)
        {

            if (type == CodeActionType.Condition && name == "GenerateNotice")
            {
                return new List<CodeActionParameterDefinition>()
            {                
                new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = false,
                    Name = "NoticeId",
                    Title = "NoticeId",
                    Type = ParameterType.Dropdown,
                    DropdownValues = new List<DropdownValue>
                    {
                        new DropdownValue
                        {
                            Name = "INT-01",
                            Value = "INT-01"
                        },
                        new DropdownValue
                        {
                            Name = "EST-07",
                            Value = "EST-07"
                        },
                        new DropdownValue
                        {
                            Name = "EST-08",
                            Value = "EST-08"
                        },
                        new DropdownValue
                        {
                            Name = "EST-09",
                            Value = "EST-09"
                        },
                        new DropdownValue
                        {
                            Name = "EST-10",
                            Value = "EST-10"
                        }
                    }
                },
                 new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = false,
                    Name = "NoticeRecipient",
                    Title = "NoticeRecipient",
                    Type = ParameterType.Dropdown,
                    DropdownValues = new List<DropdownValue>
                    {
                        new DropdownValue
                        {
                            Name = "MC",
                            Value = "MC"
                        },
                        new DropdownValue
                        {
                            Name = "MN",
                            Value = "MN"
                        }
                    }
                },
                 new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = false,
                    Name = "PrintMethod",
                    Title = "PrintMethod",
                    Type = ParameterType.Dropdown,
                    DropdownValues = new List<DropdownValue>
                    {
                        new DropdownValue
                        {
                            Name = "C",
                            Value = "C"
                        },
                        new DropdownValue
                        {
                            Name = "L",
                            Value = "L"
                        }
                    }
                }
            };
            }

            if (type == CodeActionType.Action && name == "SetActivityInputs")
            {
                return new List<CodeActionParameterDefinition>()
            {
                new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = true,
                    Name = "MinorActivity",
                    Title = "Minor Activity",
                    Type = ParameterType.Text
                },
                new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = true,
                    Name = "Group",
                    Title = "Group",
                    Type = ParameterType.Text
                },
                new CodeActionParameterDefinition
                {
                    DefaultValue = "5",
                    IsRequired = false,
                    Name = "DaysDue",
                    Title = "Days Due",
                    Type = ParameterType.Number
                },
                new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = true,
                    Name = "Category",
                    Title = "Category",
                    Type = ParameterType.Text
                },
                new CodeActionParameterDefinition
                {
                    DefaultValue = "",
                    IsRequired = true,
                    Name = "SubCategory",
                    Title = "SubCategory",
                    Type = ParameterType.Text
                },
                new CodeActionParameterDefinition
                {
                    DefaultValue = "5",
                    IsRequired = false,
                    Name = "AlertWarningInDays",
                    Title = "Alert Warning In Days",
                    Type = ParameterType.Number
                },
                new CodeActionParameterDefinition
                {
                    DefaultValue = "A",
                    IsRequired = false,
                    Name = "ActionAlertCode",
                    Title = "Action Alert Code",
                    Type = ParameterType.Text
                }
            };
            }

            return null;
        }    
    }
}
