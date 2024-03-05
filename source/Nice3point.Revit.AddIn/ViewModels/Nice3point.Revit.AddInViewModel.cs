using CommunityToolkit.Mvvm.ComponentModel;
#if (Logger && ServicesContainer)
using Serilog;
#endif
#if (Logger && Hosting)
using Microsoft.Extensions.Logging;
#endif

namespace Nice3point.Revit.AddIn.ViewModels;

#if (!Logger)
public sealed class Nice3point.Revit.AddInViewModel : ObservableObject
#elseif (ServicesContainer)
public sealed class Nice3point.Revit.AddInViewModel(ILogger logger) : ObservableObject
#elseif (Hosting)
public sealed class Nice3point.Revit.AddInViewModel(ILogger<Nice3point.Revit.AddInViewModel> logger) : ObservableObject
#else
public sealed class Nice3point.Revit.AddInViewModel : ObservableObject
#endif
{
}