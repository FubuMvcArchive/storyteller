using System;
using System.Windows;
using System.Windows.Controls;

namespace StoryTeller.UserInterface.Tests
{
    public interface IHtmlView
    {
        string Html { set; }
        object Listener { set; }
        void OpenFile(string path);
    }

    public class HtmlView : ContentControl, IHtmlView
    {
        private readonly WebBrowser _browser = new WebBrowser();
        private string _html = string.Empty;

        public HtmlView()
        {
            Content = _browser;

            _browser.Visibility = Visibility.Visible;

            Loaded += (x, y) => _browser.NavigateToString(_html);
        }

        #region IHtmlView Members

        public string Html
        {
            set
            {
                _html = value;
                _browser.NavigateToString(value);
            }
            get { return _browser.Document.ToString(); }
        }

        public object Listener
        {
            set
            {
                _browser.ObjectForScripting = value;
            }
        }

        public void OpenFile(string path)
        {
            _browser.Navigate(new Uri(path));
        }

        #endregion
    }
}