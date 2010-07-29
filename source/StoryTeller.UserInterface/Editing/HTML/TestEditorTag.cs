using HtmlTags;
using StoryTeller.Html;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class TestEditorTag : TestHolderTag
    {
        public TestEditorTag(FixtureLibrary library)
        {
            FixtureGraph fixture = library.BuildTopLevelGraph();

            HtmlTag selector = new GrammarSelector(fixture).Build();

            Container
                .MetaData(GrammarConstants.LEAF_NAME, GrammarConstants.TEST)
                .MetaData(GrammarConstants.FIXTURE, GrammarConstants.TEST)
                .MetaData(GrammarConstants.SELECTION_MODE, SelectionMode.OneOrMore.ToString())
                .Child(new HolderTag(fixture).AddClass("top-level-holder"))
                .Child(new HtmlTag("hr"))
                .Child(selector);


        }
    }
}