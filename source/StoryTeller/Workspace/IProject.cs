using System.Collections.Generic;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Workspace
{
    public interface IProject
    {
        string Name { get; set; }
        string FileName { get; }
        string ConfigurationFileName { get; }
        string SystemTypeName { get; }
        int TimeoutInSeconds { get; set; }
        string FixtureAssembly { get; set; }
        IEnumerable<WorkspaceFilter> SelectedWorkspaces { get; }
        string GetBinaryFolder();
        Hierarchy LoadTests();
        void Save(Test test);
        void DeleteFile(Test test);
        void RenameTest(Test test, string name);

        ITestRunner LocalRunner();
        void CreateDirectory(Suite suite);
        WorkspaceFilter CurrentFixtureFilter();
        void SelectWorkspaces(IEnumerable<string > workspaceNames);
        WorkspaceFilter WorkspaceFor(string workspaceName);

        string[] SelectedWorkspaceNames { get; }

        CodegenOptions Options { get; }
    }

    public class CodegenOptions
    {
        public string FileTemplate { get; set; }
        public string MethodTemplate { get; set; }
        public string TargetFile { get; set; }
    }


}