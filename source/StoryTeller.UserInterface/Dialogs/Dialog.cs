using System.Windows;
using System.Windows.Controls;

namespace StoryTeller.UserInterface.Dialogs
{
    public class Dialog : Window
    {
        public Dialog(Window top, ICommandDialog child)
        {
            //This is necessary because on startup we won't have shown the parent window yet.  Just set the parent to null.
            Owner = top.IsVisible ? top : null;

            ResizeMode = ResizeMode.CanResizeWithGrip;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            ShowInTaskbar = false;

            Title = child.Title;
            Content = child;

            Closed += new System.EventHandler(Dialog_Closed);
        }

        void Dialog_Closed(object sender, System.EventArgs e)
        {
            Content = null;
        }
    }
}