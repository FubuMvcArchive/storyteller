using StoryTeller.Workspace;

namespace StoryTeller.Domain
{
    public class WorkspaceSuite : Suite
    {
        public WorkspaceSuite()
        {
        }

        public WorkspaceSuite(string name) : base(name)
        {
        }

        public WorkspaceFilter Filter { get; set; }
    }
}