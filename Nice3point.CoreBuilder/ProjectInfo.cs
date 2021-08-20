using System;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

public class ProjectInfo
{
    public ProjectInfo(Solution solution, string projectName)
    {
        ProjectName = projectName;
        var project = solution.GetProject(projectName);
        Project = project ?? throw new NullReferenceException($"Cannon find project \"{projectName}\"");
    }

    public string ProjectName { get; }
    public Project Project { get; }

    public AbsolutePath BinDirectory => Project.Directory / "bin";
    public AbsolutePath ExecutableFile => BinDirectory / "Release" / $"{ProjectName}.exe";
}