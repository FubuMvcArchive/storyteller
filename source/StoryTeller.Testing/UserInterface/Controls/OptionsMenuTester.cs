using System.Windows.Controls;
using System.Windows.Input;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Controls;

namespace StoryTeller.Testing.UserInterface.Controls
{
    [TestFixture]
    public class CommandMenuItemTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            command = MockRepository.GenerateMock<ICommand>();
            command.Expect(x => x.CanExecute(null)).Return(true).IgnoreArguments();

            screenAction = new ScreenAction
            {
                Binding = new InputBinding(command, new KeyGesture(Key.F3)),
                Name = "some text"
            };

            item = new CommandMenuItem(screenAction);
        }

        #endregion

        private ICommand command;
        private ScreenAction screenAction;
        private CommandMenuItem item;

        [Test]
        public void the_display()
        {
            var header = item.Header.ShouldBeOfType<DockPanel>();
            header.Children[0].ShouldBeOfType<Label>().Content.ShouldEqual(screenAction.Name);
            header.Children[1].ShouldBeOfType<Label>().Content.ShouldEqual(screenAction.KeyString);
        }
    }
}