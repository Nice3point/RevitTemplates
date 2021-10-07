using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

namespace Installer
{
    public static class Installer
    {
        private const string InstallationDir = @"%AppDataFolder%\Autodesk\Revit\Addins\";
        private const string ProjectName = "Nice3point.Revit.Solution";
        private const string OutputName = "Nice3point.Revit.Solution";
        private const string OutputDir = "output";
        private const string Version = "1.0.0";

        public static void Main(string[] args)
        {
            var filesStorage = args[0];
            var configurations = args.Skip(1);

            var outFileNameBuilder = new StringBuilder().Append(OutputName).Append("-").Append(Version);
            //Additional suffixes for unique configurations add here
            var outFileName = outFileNameBuilder.ToString();

            var project = new Project
            {
                Name = ProjectName,
                OutDir = OutputDir,
                OutFileName = outFileName,
                Platform = Platform.x64,
                Version = new Version(Version),
                InstallScope = InstallScope.perUser,
                MajorUpgrade = MajorUpgrade.Default,
                UI = WUI.WixUI_InstallDir,
                GUID = new Guid("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                BackgroundImage = @"Installer\Resources\Icons\BackgroundImage.png",
                BannerImage = @"Installer\Resources\Icons\BannerImage.png",
                ControlPanelInfo =
                {
                    ProductIcon = @"Installer\Resources\Icons\ShellIcon.ico"
                },
                Dirs = new Dir[]
                {
                    new InstallDir(InstallationDir, GetOutputFolders(filesStorage, configurations))
                }
            };

            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            project.BuildMsi();
        }

        private static WixEntity[] GetOutputFolders(string filesStorage, IEnumerable<string> configurations)
        {
            var entity = new List<WixEntity>();
            var versionRegex = new Regex(@"\d+");
            foreach (var configuration in configurations)
            {
                var files = @$"{filesStorage}\{configuration}\*.*";
                var version = versionRegex.Match(configuration).Value;
                entity.Add(new Dir(version, new Files(files)));
            }

            return entity.ToArray();
        }
    }
}