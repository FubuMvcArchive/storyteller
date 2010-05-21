using System;
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

        public void Launch<TCommand>(TCommand command)
        {
            Dialog dialog = BuildDialog(command);

            dialog.ShowDialog();
        }



        public void Launch<TCommand>()
        {
            var command = _container.GetInstance<TCommand>();
            Launch(command);
        }

        public void Launch(ICommandDialog dialog)
        {
            Dialog d = _container.With(dialog).GetInstance<Dialog>();
            d.ShowDialog();
        }



        #endregion

        public Dialog BuildDialog<TCommand>(TCommand command)
        {
            var control = _container.With(command).GetInstance<ICommandDialog<TCommand>>();

            return _container.With<ICommandDialog>(control).GetInstance<Dialog>();
        }
    }
}