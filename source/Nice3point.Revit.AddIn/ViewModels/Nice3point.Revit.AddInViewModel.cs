#if (log && Container)
using Serilog;
#endif
#if (log && Hosting)
using Microsoft.Extensions.Logging;
#endif

namespace Nice3point.Revit.AddIn.ViewModels;

#if (!log)
public sealed class Nice3point.Revit.AddInViewModel : ObservableObject
#elseif (Container)
public sealed class Nice3point.Revit.AddInViewModel(ILogger logger) : ObservableObject
#elseif (Hosting)
public sealed class Nice3point.Revit.AddInViewModel(ILogger<Nice3point.Revit.AddInViewModel> logger) : ObservableObject
#else
public sealed class Nice3point.Revit.AddInViewModel : ObservableObject
#endif
{
}