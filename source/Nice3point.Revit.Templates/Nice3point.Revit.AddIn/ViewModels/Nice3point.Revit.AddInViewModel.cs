#if (addinLogging && diContainer)
using Serilog;

#endif
#if (addinLogging && diHosting)
using Microsoft.Extensions.Logging;

#endif
namespace Nice3point.Revit.AddIn.ViewModels;

#if (!addinLogging)
public sealed class Nice3point.Revit.AddInViewModel : ObservableObject
#elseif (diContainer)
public sealed class Nice3point.Revit.AddInViewModel(ILogger logger) : ObservableObject
#elseif (diHosting)
public sealed class Nice3point.Revit.AddInViewModel(ILogger<Nice3point.Revit.AddInViewModel> logger) : ObservableObject
#else
public sealed class Nice3point.Revit.AddInViewModel : ObservableObject
#endif
{
}