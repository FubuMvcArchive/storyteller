using System.Windows.Controls;
using System.Windows.Media;
using ShadeTree.Validation;

namespace StoryTeller.UserInterface.Controls
{
    public class ValidationSummary : Grid
    {
        public ValidationSummary()
        {
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());

            Background = new SolidColorBrush(Colors.White);
        }

        public void ShowMessages(Notification notification)
        {
            RowDefinitions.Clear();

            foreach (NotificationMessage message in notification.AllMessages)
            {
                RowDefinitions.Add(new RowDefinition());
                int rowIndex = RowDefinitions.Count - 1;

                this.Add(new Label
                {
                    Content = message.FieldName,
                    Foreground = new SolidColorBrush(Colors.Red)
                }, rowIndex, 0);

                this.Add(new Label
                {
                    Content = message.Message,
                    Foreground = new SolidColorBrush(Colors.Red)
                }, rowIndex, 1);
            }
        }
    }
}