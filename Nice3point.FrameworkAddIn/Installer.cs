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