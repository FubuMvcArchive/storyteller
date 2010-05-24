using System;
using System.Collections.Generic;
using System.Windows.Input;
using StoryTeller.Domain;
using StoryTeller.Execution;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Projects;
using StoryTeller.UserInterface.Screens;
using System.Linq;

namespace StoryTeller.UserInterface.Workspace
{
    public class WorkspaceEditor : IScreen<WorkspaceSuite>, IListener<BinaryRecycleFinished>
    {
        private readonly WorkspaceSuite _suite;
        private readonly ProjectContext _context;
        private readonly IWorkspaceEditorView _view;
        private readonly IProjectController _controller;
        private readonly IFixtureSelectorOrganizer _organizer;
        private bool _hasUpdatedView = false;

        public WorkspaceEditor(WorkspaceSuite suite, ProjectContext context, IWorkspaceEditorView view, IProjectController controller, IFixtureSelectorOrganizer organizer)
        {
            _suite = suite;
            _context = context;
            _view = view;
            _controller = controller;
            _organizer = organizer;
        }

        public void Dispose()
        {

        }

        public object View
        {
            get { return _view; }
        }

        public string Title
        {
            get { return _suite.Name; }
        }

        public void Activate(IScreenObjectRegistry screenObjects)
        {
            screenObjects.Action("Save Filters and Actions").Bind(ModifierKeys.Control, Key.S).To(Save).Icon = Icon.Save;
        
            if (!_hasUpdatedView)
            {
                BuildView();
            }
        }

        public void BuildView()
        {
            Selectors = _organizer.Organize(_context.Library, _suite.Filter);
            _view.ShowFixtureNamespaces(Selectors);

            StartupActions = _organizer.GetActionSelectors(_context.Library, _suite.Filter);
            _view.ShowActionSelectors(StartupActions);

            _hasUpdatedView = true;
        }

        public IEnumerable<IStartupActionSelector> StartupActions { get; set; }

        public IEnumerable<IFixtureSelector> Selectors { get; set; }

        public void Save()
        {
            _suite.Filter.Filters = Selectors.SelectMany(x => x.GetFilters()).ToArray();
            _suite.Filter.StartupActions = StartupActions.Where(x => x.IsSelected()).Select(x => x.ActionName).ToArray();

            _controller.SaveWorkspace(_suite);
        }

        // TODO -- implemente is dirty crap
        public bool CanClose()
        {
            return true;
        }

        public WorkspaceSuite Subject
        {
            get { return _suite; }
        }

        public void Handle(BinaryRecycleFinished message)
        {
            BuildView();
        }
    }
}