namespace StoryTeller.Workspace
{
    public class ReloadTestsMessage
    {
    }

    public class ClearResultsMessage
    {
    }


    public class ProjectLoaded
    {
        private readonly IProject _project;

        public ProjectLoaded(IProject project)
        {
            _project = project;
        }

        public IProject Project { get { return _project; } }

        public bool Equals(ProjectLoaded obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._project, _project);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ProjectLoaded)) return false;
            return Equals((ProjectLoaded) obj);
        }

        public override int GetHashCode()
        {
            return (_project != null ? _project.GetHashCode() : 0);
        }
    }

    public class WorkflowFiltersChanged
    {
        public readonly IProject _project;

        public WorkflowFiltersChanged(IProject project)
        {
            _project = project;
        }

        public IProject Project
        {
            get { return _project; }
        }

        public bool Equals(WorkflowFiltersChanged other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._project, _project);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (WorkflowFiltersChanged)) return false;
            return Equals((WorkflowFiltersChanged) obj);
        }

        public override int GetHashCode()
        {
            return (_project != null ? _project.GetHashCode() : 0);
        }
    }
}