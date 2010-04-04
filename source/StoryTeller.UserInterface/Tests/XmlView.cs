using System.Windows.Forms.Integration;
using ICSharpCode.XmlEditor;

namespace StoryTeller.UserInterface.Tests
{
    public interface IXmlView
    {
        string Xml { get; set; }
    }

    public class XmlView : WindowsFormsHost, IXmlView
    {
        private readonly XmlEditorControl _editor = new XmlEditorControl();

        public XmlView()
        {
            Child = _editor;
        }

        #region IXmlView Members

        public string Xml { get { return _editor.Text; } set { _editor.Text = value; } }

        #endregion
    }
}