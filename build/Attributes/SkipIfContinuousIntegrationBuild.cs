using ModularPipelines.Attributes;
using ModularPipelines.Context;

namespace Build.Attributes;

public sealed class SkipIfContinuousIntegrationBuild : MandatoryRunConditionAttribute
{
    public override Task<bool> Condition(IPipelineHookContext context)
    {
        return Task.FromResult(!context.BuildSystemDetector.IsKnownBuildAgent);
    }
}