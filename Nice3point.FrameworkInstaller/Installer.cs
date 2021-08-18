using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

namespace Nice3point.FrameworkInstaller
{
    public static class Installer
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
        public static void Main(string[] args)
        {
            var filesStorage = args[0];
            var projectStorage = args[1];
            var versions = args.Skip(2);

            var project = new Project
            {
                Name            = "Nice3point.FrameworkInstaller", /*caret*/
                OutDir          = "output",
                Version         = new Version(1, 0, 0),
                Platform        = Platform.x64,
                UI              = WUI.WixUI_InstallDir,
                InstallScope    = InstallScope.perUser,
                BackgroundImage = $@"{projectStorage}\Resources\Icons\InstallerIcon.png",
                GUID            = new Guid("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Dirs = new[]
                {
                    new Dir($"{InstallationDir}", GetOutputFolders(filesStorage, versions))
                }
            };

            var outNameBuilder = new StringBuilder();
            outNameBuilder.Append("Nice3point.FrameworkInstaller");
            outNameBuilder.Append("-");
            outNameBuilder.Append(project.Version);

            project.OutFileName = outNameBuilder.ToString();
            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            project.BuildMsi();
        }

        private static WixEntity[] GetOutputFolders(string filesDir, IEnumerable<string> versions)
        {
            var entity = new WixEntity[] { };
            foreach (var version in versions)
            {
                var files = $@"{filesDir}\Addin {version}\*.*";
                entity.Combine(new Dir(version, new Files(files)));
            }

            return entity;
        }
    }
}