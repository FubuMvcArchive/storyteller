using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StoryTeller.UserInterface.Actions;

namespace StoryTeller.UserInterface.Controls
{
    public interface IOptionsMenu
    {
        void Refill(IEnumerable<ScreenAction> actions);
    }

    public class OptionsMenu : Button, IOptionsMenu
    {
        public OptionsMenu()
        {
            ContextMenu = new ContextMenu();

            this.ToIconButton(Icon.Unknown).Text("Commands");
            Click += (s, e) => ShowOptions();
            VerticalAlignment = VerticalAlignment.Stretch;
            Height = 30;
            Margin = new Thickness(5, 0, 5, 0);
            HorizontalAlignment = HorizontalAlignment.Right;
            Width = 100;
        }

        #region IOptionsMenu Members

        public void Refill(IEnumerable<ScreenAction> actions)
        {
            ContextMenu.Items.Clear();
            actions.Each(x =>
            {
                var item = new CommandMenuItem(x);
                ContextMenu.Items.Add(item);
            });
        }

        #endregion

        public void ShowOptions()
        {
            ContextMenu.IsOpen = true;
        }
    }

    public class CommandMenuItem : MenuItem
    {
        public CommandMenuItem(ScreenAction screenAction)
        {
            this.SetIcon(screenAction.Icon);
            Header = new DockPanel()
                .Left<Label>(x => x.Content = screenAction.Name);
                //.Right<Label>(x =>
                //{
                //    x.Content = screenAction.KeyString;
                //    x.HorizontalAlignment = HorizontalAlignment.Right;
                //});

            InputGestureText = screenAction.KeyString;
            ScreenAction = ScreenAction;
            Command = screenAction.Command;
        }

        protected ScreenAction ScreenAction { get; set; }
    }
}