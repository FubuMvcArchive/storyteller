using System.Diagnostics;
using NUnit.Framework;
using StoryTeller.Model;
using StoryTeller.Samples.Grammars;
using StoryTeller.UserInterface.Editing.HTML;

namespace StoryTeller.Testing.UserInterface.Editing.HTML
{
    [TestFixture]
    public class TestEditorTagTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            library = FixtureLibrary.For(x => x.AddFixturesFromAssemblyContaining<SentenceFixture>());
            tag = new TestEditorTag(library);
        }

        #endregion

        private FixtureLibrary library;
        private TestEditorTag tag;

        [Test]
        public void write_out_the_html()
        {
            Debug.WriteLine(tag.ToString());
        }
    }
}