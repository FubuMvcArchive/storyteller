using System.Windows;
using System.Windows.Input;

namespace WpfSpike
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Lookup _lookup;

        public Window1()
        {
            InitializeComponent();


            box.KeyDown += box_KeyDown;
            box.Focus();
        }

        private void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_lookup != null) _lookup.Close();
                Close();
            }

            if (_lookup == null)
            {
                _lookup = new Lookup(box, this);
            }
        }
    }
}