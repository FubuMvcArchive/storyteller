using System.Collections.Generic;
using System.Linq;
using StoryTeller.Domain;
using StoryTeller.UserInterface.Dialogs;
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Workspace
{
    public class WorkspaceSelector : IStartable, IListener<ProjectLoaded>, IListener<Hierarchy>
    {
        private readonly IDialogLauncher _launcher;
        private readonly IWorkspaceSelectionDialog _dialog;
        private readonly IWorkspaceSelectorMenu _menu;
        private readonly IEventAggregator _events;

        public WorkspaceSelector(IDialogLauncher launcher, IWorkspaceSelectionDialog dialog, IWorkspaceSelectorMenu menu, IEventAggregator events)
        {
            _launcher = launcher;
            _dialog = dialog;
            _menu = menu;
            _events = events;

            _menu.OnSelection = OpenWorkspaceSelection;
            _dialog.OnSelection = SelectWorkspaces;
        }

        public void SelectWorkspaces()
        {
            Project.SelectWorkspaces(_dialog.GetSelections());

            UpdateSelectionMenu();

            _events.SendMessage(new WorkflowFiltersChanged(Project));
        }

        public void OpenWorkspaceSelection()
        {
            _dialog.Clear();

            var selected = selectedWorkspaces();
            workspaces().Each(x =>
            {
                var isSelected = selected.Contains(x);
                _dialog.Add(x, isSelected);
            });

            if (!selected.Any())
            {
                _dialog.SelectAll();
            }

            _launcher.LaunchDialog(_dialog);
        }

        public void Start()
        {
            _menu.Hide();
        }

        void IListener<ProjectLoaded>.Handle(ProjectLoaded message)
        {
            _menu.Hide();
            Project = message.Project;
        }

        // public for unit testing
        public IProject Project { get; set; }
        public void Handle(Hierarchy message)
        {
            Hierarchy = message;

            UpdateSelectionMenu();
        }

        public virtual void UpdateSelectionMenu()
        {
            if (Project.SelectedWorkspaces.Any())
            {
                var selected = selectedWorkspaces();
                var missing = workspaces()
                    .Where(x => !selected.Contains(x)).ToArray();

                _menu.ShowThatWorkspacesAreHidden(missing);
            }
            else
            {
                _menu.ShowAllWorkspacesAreAvailable();
            }
        }

        private IEnumerable<string> workspaces()
        {
            return Hierarchy.FindAllWorkspaces(Project)
                .Select(x => x.Name);
        }

        private IEnumerable<string> selectedWorkspaces()
        {
            return Project.SelectedWorkspaces.Select(x => x.Name);
        }

        public Hierarchy Hierarchy { get; set; }
    }
}