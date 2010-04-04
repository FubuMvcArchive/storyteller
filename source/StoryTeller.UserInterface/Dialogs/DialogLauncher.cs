using StructureMap;

namespace StoryTeller.UserInterface.Dialogs
{
    public class DialogLauncher : IDialogLauncher
    {
        private readonly IContainer _container;

        public DialogLauncher(IContainer container)
        {
            _container = container;
        }

        #region IDialogLauncher Members

        public void Launch<COMMAND>(COMMAND command)
        {
            Dialog dialog = BuildDialog(command);

            dialog.ShowDialog();
        }

        public void Launch<COMMAND>()
        {
            var command = _container.GetInstance<COMMAND>();
            Launch(command);
        }

        #endregion

        public Dialog BuildDialog<COMMAND>(COMMAND command)
        {
            var control = _container.With(command).GetInstance<ICommandDialog<COMMAND>>();

            return _container.With<ICommandDialog>(control).GetInstance<Dialog>();
        }
    }
}