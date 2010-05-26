using System;
using StoryTeller.Workspace;

namespace StoryTeller.Domain
{
    public class WorkspaceSuite : Suite
    {
        public WorkspaceSuite(string name) : base(name)
        {
            Filter = new WorkspaceFilter()
            {
                Name = name
            };
        }

        public WorkspaceFilter Filter { get; set; }

        public override WorkspaceFilter GetWorkspace()
        {
            return Filter ?? new WorkspaceFilter();
        }
    }
}