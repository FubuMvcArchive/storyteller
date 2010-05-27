using System;
using System.Windows.Controls;

namespace StoryTeller.UserInterface.Screens
{
    public class StoryTellerTabItem : TabItem
    {
        private Label _label;
        private IScreen _screen;

        public StoryTellerTabItem(IScreen screen, IEventAggregator events)
        {
            _screen = screen;

            Func<Action<IScreenConductor>, Action> sendMessage = a => () => events.SendMessage(a);

            Header = new StackPanel().Horizontal()
                .Configure(x => x.Height = 25)
                .AddText(screen.Title, x => _label = x)
                .IconButton(Icon.Close, sendMessage(s => s.Close(screen)), b => b.SmallerImages());

            Content = new DockPanel().With(screen.View);
            Tag = screen;


            // Sets up a context menu for each tab in the screen that can capture "Close"
            // messages
            ContextMenu = new ContextMenu().Configure(o =>
            {
                o.AddItem("Close", sendMessage(s => s.Close(screen)));
                o.AddItem("Close All But This", sendMessage(s => s.CloseAllBut(screen)));
                o.AddItem("Close All", sendMessage(s => s.CloseAll()));
            });
        }

        public void RefreshTitle()
        {
            HeaderText = _screen.Title;
        }

        public string HeaderText { get { return _label.Content as string; } set { _label.Content = value; } }
    }
}