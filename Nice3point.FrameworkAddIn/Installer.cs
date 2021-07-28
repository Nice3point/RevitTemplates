using System;
using System.IO;
using System.Text;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

namespace Nice3point.FrameworkAddIn
{
    public class Installer
    {
        private const string InstallationDir = @"%AppDataFolder%\Autodesk\Revit\Addins\";

        /// <remarks>
        ///     The installer is generated only for the build versions, from the "AddIn 'Revit version'" folder, otherwise you will
        ///     receive an error.
        ///     If you still get errors, also check that the "Working directory" in "Run/Debug Configurations" ends with "/bin",
        ///     everything after you need to delete
        /// </remarks>
        /// <example>
        ///     If the plugin is made only for Revit 2022, the final Dirs collection will look like this:
        ///     <code>
        ///         new Dir($"{InstallationDir}",
        ///             new Dir("2022",
        ///                 new Files(@"AddIn 2022\*.*")))
        ///  </code>
        /// </example>
        public static void Install()
        {
            var project = new Project
            {
                Name            = "Nice3point.FrameworkAddIn",
                Version         = new Version(1, 0, 0),
                Platform        = Platform.x64,
                UI              = WUI.WixUI_InstallDir,
                InstallScope    = InstallScope.perUser,
                BackgroundImage = $@"{Directory.GetParent(Directory.GetCurrentDirectory())}\Resources\Icons\InstallerIcon.png",
                GUID            = new Guid("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Dirs = new[]
                {
                    new Dir($"{InstallationDir}",
                        new Dir("2019",
                            new Files(@"AddIn 2019\*.*")),
                        new Dir("2020",
                            new Files(@"AddIn 2020\*.*")),
                        new Dir("2021",
                            new Files(@"AddIn 2021\*.*")),
                        new Dir("2022",
                            new Files(@"AddIn 2022\*.*")))
                }
            };

            var outNameBuilder = new StringBuilder();
            outNameBuilder.Append("Nice3point.FrameworkAddIn");
            outNameBuilder.Append("-");
            outNameBuilder.Append(project.Version);

            project.OutFileName = outNameBuilder.ToString();
            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            project.BuildMsi();
        }
    }
}