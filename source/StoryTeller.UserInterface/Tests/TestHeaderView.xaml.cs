using System.Windows.Controls;
using ShadeTree.Binding;
using ShadeTree.Binding.WPF;

namespace StoryTeller.UserInterface.Tests
{
    /// <summary>
    /// Interaction logic for TestHeaderView.xaml
    /// </summary>
    public partial class TestHeaderView : UserControl, ITestHeaderView
    {
        private readonly ScreenBinder<TestHeaderViewModel> _binder = new ScreenBinder<TestHeaderViewModel>();
        private TestHeaderViewModel _model;

        public TestHeaderView()
        {
            InitializeComponent();

            _binder.Bind(x => x.Name).To(name);
            _binder.Bind(x => x.Path).To(path);
            _binder.Bind(x => x.Status).To(status);

            var element = new ButtonElement(lifecycle);
            element.OnClick(() => _model.ToggleLifecycle());
            _binder.AddElement(element);
        }

        #region ITestHeaderView Members

        public void Refresh()
        {
            _binder.UpdateScreen();
            lifecycle.Content = _model.Lifecycle;
        }

        #endregion

        public void BindTo(TestHeaderViewModel model)
        {
            _model = model;
            _binder.BindToModel(model);
            lifecycle.Content = _model.Lifecycle;
        }
    }
}