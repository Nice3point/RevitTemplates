using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Nice3point.Revit.Sdk;

public class AddImplicitUsings : Task
{
    [Required] public required ITaskItem[] AdditionalUsings { get; set; }
    [Required] public required ITaskItem[] References { get; set; }
    [Output] public string[]? Usings { get; private set; }

    public override bool Execute()
    {
        try
        {
            var usings = new List<string>();
            foreach (var additionalUsing in AdditionalUsings)
            {
                var requiredPackage = additionalUsing.GetMetadata("RequiredReference");
                if (string.IsNullOrEmpty(requiredPackage))
                {
                    usings.Add(additionalUsing.ItemSpec);
                }
                else
                {
                    var existedPackage = References.FirstOrDefault(item => item.ItemSpec.EndsWith(requiredPackage));
                    if (existedPackage is not null)
                    {
                        usings.Add(additionalUsing.ItemSpec);
                    }
                }
            }

            Usings = usings.ToArray();
            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);
            return false;
        }
    }
}