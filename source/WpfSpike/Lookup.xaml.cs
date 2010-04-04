using System.Windows;
using System.Windows.Controls;

namespace WpfSpike
{
    /// <summary>
    /// Interaction logic for Lookup.xaml
    /// </summary>
    public partial class Lookup : Window
    {
        private readonly TextBox _parent;


        static Lookup()
        {
            WindowStyleProperty.OverrideMetadata(typeof (Lookup), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowActivatedProperty.OverrideMetadata(typeof (Lookup), new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof (Lookup), new FrameworkPropertyMetadata(false));
            ResizeModeProperty.OverrideMetadata(typeof (Lookup), new FrameworkPropertyMetadata(ResizeMode.NoResize));
        }

        public Lookup()
        {
            InitializeComponent();

            Margin = new Thickness(0);
            Padding = new Thickness(0);
            BorderBrush = null;
        }

        public Lookup(TextBox parent, Window owner)
            : this()
        {
            _parent = parent;
            Owner = owner;

            var relative = new Point(0, parent.ActualHeight);
            Point desiredLocation = owner.TranslatePoint(relative, parent);

            Point point = Owner.PointToScreen(desiredLocation);

            Top = point.Y;
            Left = point.X;

            Show();
        }

        //void SetPosition()
        //{
        //    var visualLocation = textView.GetVisualPosition(this.TextArea.Caret.Position, VisualYPosition.LineBottom);
        //    visualLocationTop = textView.GetVisualPosition(this.TextArea.Caret.Position, VisualYPosition.LineTop);
        //    UpdatePosition();
        //}

        //void UpdatePosition()
        //{
        //    TextView textView = this.TextArea.TextView;
        //    Point location = textView.PointToScreen(visualLocation - textView.ScrollOffset);
        //    Point locationTop = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);

        //    Size completionWindowSize = new Size(this.ActualWidth, this.ActualHeight);
        //    Rect bounds = new Rect(location, completionWindowSize);
        //    Rect workingScreen = System.Windows.Forms.Screen.GetWorkingArea(location.ToSystemDrawing()).ToWpf();
        //    if (!workingScreen.Contains(bounds))
        //    {
        //        if (bounds.Left < workingScreen.Left)
        //        {
        //            bounds.X = workingScreen.Left;
        //        }
        //        else if (bounds.Right > workingScreen.Right)
        //        {
        //            bounds.X = workingScreen.Right - bounds.Width;
        //        }
        //        if (bounds.Bottom > workingScreen.Bottom)
        //        {
        //            bounds.Y = locationTop.Y - bounds.Height;
        //        }
        //        if (bounds.Y < workingScreen.Top)
        //        {
        //            bounds.Y = workingScreen.Top;
        //        }
        //    }
        //    this.Left = bounds.X;
        //    this.Top = bounds.Y;
        //}
    }
}