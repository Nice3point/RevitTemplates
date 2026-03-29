using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Nice3point.Revit.Sdk;

[PublicAPI]
public class AddImplicitUsings : Task
{
    [Required] public required ITaskItem[] AdditionalUsings { get; set; }
    [Required] public required ITaskItem[] References { get; set; }
    [Output] public string[]? Usings { get; private set; }

    public override bool Execute()
    {
        try
        {
            var implicitUsings = new List<string>();
            foreach (var additionalUsing in AdditionalUsings)
            {
                var requiredReferencesMetadata = additionalUsing.GetMetadata("RequiredReference");
                if (string.IsNullOrEmpty(requiredReferencesMetadata))
                {
                    implicitUsings.Add(additionalUsing.ItemSpec);
                }
                else if (requiredReferencesMetadata.Contains(';'))
                {
                    var hasRequiredReferences = true;
                    var requiredReferences = requiredReferencesMetadata.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var requiredReference in requiredReferences)
                    {
                        var reference = References.FirstOrDefault(refItem => refItem.ItemSpec.EndsWith(requiredReference));
                        if (reference is null)
                        {
                            hasRequiredReferences = false;
                            break;
                        }
                    }

                    if (hasRequiredReferences)
                    {
                        implicitUsings.Add(additionalUsing.ItemSpec);
                    }
                }
                else
                {
                    var reference = References.FirstOrDefault(refItem => refItem.ItemSpec.EndsWith(requiredReferencesMetadata));
                    if (reference is not null)
                    {
                        implicitUsings.Add(additionalUsing.ItemSpec);
                    }
                }
            }

            Usings = implicitUsings.ToArray();
            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);
            return false;
        }
    }
}