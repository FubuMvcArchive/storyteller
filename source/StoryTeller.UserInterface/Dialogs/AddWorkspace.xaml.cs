using System.Windows;
using System.Windows.Controls;
using FubuCore;
using StoryTeller.Engine;
using StoryTeller.UserInterface.Commands;

namespace StoryTeller.UserInterface.Dialogs
{
    /// <summary>
    /// Interaction logic for AddTest.xaml
    /// </summary>
    public partial class AddWorkspace : ICommandDialog<IAddWorkspaceCommand>
    {
        private readonly IAddWorkspaceCommand _command;

        public AddWorkspace()
        {
            InitializeComponent();

            icon.SetIcon(Icon.Plus);
            enableCreate();
            name.TextChanged += (s, e) => enableCreate();
            name.Focus();

            create.Click += (s, e) =>
            {
                _command.CreateWorkspace(name.Text);
                Parent.CallOn<Window>(x => x.DialogResult = true);
            };
        }

        public AddWorkspace(IAddWorkspaceCommand command)
            : this()
        {
            _command = command;
        }

        public TextBox NameBox { get { return name; } }

        public Button Create { get { return create; } }

        #region ICommandDialog<IAddSuiteCommand> Members

        public string Title { get { return "Add Test"; } }

        #endregion

        private void enableCreate()
        {
            Create.IsEnabled = name.Text.Trim().IsNotEmpty();
        }
    }
}