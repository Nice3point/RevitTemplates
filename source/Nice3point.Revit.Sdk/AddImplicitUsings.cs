using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Nice3point.Revit.Sdk;

[PublicAPI]
public class AddImplicitUsings : Task
{
    [Required] public required ITaskItem[] AdditionalUsings { get; set; }
    public required ITaskItem[] References { get; set; } = [];
    public ITaskItem[] GlobalPackageReferences { get; set; } = [];
    [Output] public string[]? Usings { get; private set; }

    public override bool Execute()
    {
        try
        {
            var usings = new HashSet<string>();
            foreach (var item in AdditionalUsings)
            {
                if (CanResolveUsing(item))
                {
                    usings.Add(item.ItemSpec);
                }
            }

            Usings = [..usings];

            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);
            return false;
        }
    }

    private bool CanResolveUsing(ITaskItem usingItem)
    {
        var requiredAssembly = usingItem.GetMetadata("RequiredAssembly");
        var requiredGlobalReference = usingItem.GetMetadata("RequiredGlobalReference");

        if (string.IsNullOrEmpty(requiredAssembly) && string.IsNullOrEmpty(requiredGlobalReference))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(requiredAssembly) && !HasRequiredAssemblies(requiredAssembly))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(requiredGlobalReference) && !HasRequiredGlobalReferences(requiredGlobalReference))
        {
            return false;
        }

        return true;
    }

    private bool HasRequiredAssemblies(string metadata)
    {
        var assemblies = metadata.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var assembly in assemblies)
        {
            if (References.All(refItem => !refItem.ItemSpec.EndsWith(assembly))) return false;
        }

        return true;
    }

    private bool HasRequiredGlobalReferences(string metadata)
    {
        var references = metadata.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var reference in references)
        {
            if (GlobalPackageReferences.All(refItem => refItem.ItemSpec != reference)) return false;
        }

        return true;
    }
}