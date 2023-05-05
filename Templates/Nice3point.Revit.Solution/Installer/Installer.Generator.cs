using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WixSharp;

namespace Installer;

public static class Generator
{
    public static WixEntity[] GenerateWixEntities(IEnumerable<string> args)
    {
        var versionRegex = new Regex(@"\d+");
        var versionStorages = new Dictionary<string, List<WixEntity>>();

        foreach (var directory in args)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var fileVersion = versionRegex.Match(directoryInfo.Name).Value;
            var files = new Files($@"{directory}\*.*");
            if (versionStorages.TryGetValue(fileVersion, out var storage))
                storage.Add(files);
            else
                versionStorages.Add(fileVersion, new List<WixEntity> {files});

            var assemblies = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            Console.WriteLine($"Installer files for version '{fileVersion}':");
            foreach (var assembly in assemblies) Console.WriteLine($"'{assembly}'");
        }

        return versionStorages.Select(storage => new Dir(storage.Key, storage.Value.ToArray())).Cast<WixEntity>().ToArray();
    }
}