using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FubuCore.Util;
using StoryTeller.UserInterface.Handlers;

namespace StoryTeller.UserInterface.Screens
{
    public interface IScreenFinder
    {
        IScreen Find(IScreenLocator _locator);
    }

    public class ScreenFinder : IScreenFinder
    {
        private readonly IScreenCollection _screens;

        public ScreenFinder(IScreenCollection screens)
        {
            _screens = screens;
        }

        #region IScreenFinder Members

        public IScreen Find(IScreenLocator _locator)
        {
            return _screens.AllScreens.FirstOrDefault(_locator.Matches);
        }

        #endregion
    }

    public class ScreenCollection : IScreenCollection
    {
        private readonly Cache<IScreen, StoryTellerTabItem> _tabItems
            = new Cache<IScreen, StoryTellerTabItem>();

        private readonly TabControl _tabs;

        public ScreenCollection(TabControl tabs, IEventAggregator events)
        {
            _tabs = tabs;
            _tabItems.OnMissing = screen => new StoryTellerTabItem(screen, events);

            // Sends a message when the user select a different tab on the screen
            _tabs.SelectionChanged += (s, c) => events.SendMessage<UserScreenActivation>();

            // Hack.  Sigh.
            events.AddListener(new RenameTestHandler(new ScreenFinder(this), this));
        }

        #region IScreenCollection Members

        public void ClearAll()
        {
            _tabs.Items.Clear();
        }

        public IScreen Active
        {
            get
            {
                // If there is a selected tab, this 
                // will return the IScreen object
                // matching the selected tab
                if (_tabs.SelectedItem != null)
                {
                    return toScreen(_tabs.SelectedItem);
                }
                ;

                return null;
            }
        }

        public void Show(IScreen screen)
        {
            _tabs.SelectedItem = _tabItems[screen];
        }

        public void Add(IScreen screen)
        {
            // Add a new screen to the tabbed display
            StoryTellerTabItem cache = _tabItems[screen];
            _tabs.Items.Add(cache);
        }

        public void Remove(IScreen screen)
        {
            TabItem tabItem = _tabItems[screen];
            _tabItems.Remove(screen);
            _tabs.Items.Remove(tabItem);
        }

        public void RenameTab(IScreen screen, string name)
        {
            _tabItems[screen].HeaderText = name;
        }

        public IEnumerable<IScreen> AllScreens { get { return new List<IScreen>(_tabItems.GetAllKeys()); } }

        public void Start()
        {
            // no-op
        }

        #endregion

        private IScreen toScreen(object tab)
        {
            return tab.As<TabItem>().Tag.As<IScreen>();
        }
    }

    public class StoryTellerTabItem : TabItem
    {
        private Label _label;

        public StoryTellerTabItem(IScreen screen, IEventAggregator events)
        {
            Func<Action<IScreenConductor>, Action> sendMessage = a => () => events.SendMessage(a);

            Header = new StackPanel().Horizontal()
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

        public string HeaderText { get { return _label.Content as string; } set { _label.Content = value; } }
    }
}