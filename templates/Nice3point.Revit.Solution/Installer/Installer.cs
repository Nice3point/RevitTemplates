using System;
using System.Collections.Generic;
using System.IO;
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
                    Manufacturer = Environment.UserName,
                    ProductIcon = @"Installer\Resources\Icons\ShellIcon.ico"
                },
                Dirs = new Dir[]
                {
                    new InstallDir(InstallationDir, GetOutputFolders(args))
                }
            };

            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            project.BuildMsi();
        }

        private static WixEntity[] GetOutputFolders(string[] directories)
        {
            var versionRegex = new Regex(@"\d+");
            var versionStorages = new Dictionary<string, List<WixEntity>>();

            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var version = versionRegex.Match(directoryInfo.Name).Value;
                var files = new Files($@"{directory}\*.*");
                if (versionStorages.ContainsKey(version))
                    versionStorages[version].Add(files);
                else
                    versionStorages.Add(version, new List<WixEntity> {files});

                var assemblies = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                foreach (var assembly in assemblies)
                {
                    Console.WriteLine($"Added {version} version file: {assembly}");
                }
            }

            return versionStorages.Select(storage => new Dir(storage.Key, storage.Value.ToArray())).Cast<WixEntity>().ToArray();
        }
    }
}