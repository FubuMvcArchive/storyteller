using System.Collections.Generic;
using HtmlTags;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class GrammarWriter
    {
        private readonly FixtureLibrary _library;
        private FixtureTag _fixtureTag;
        private HtmlTag _top;

        public GrammarWriter(FixtureLibrary library)
        {
            _library = library;
        }

        public HtmlTag Build()
        {
            _top = new DivTag(GrammarConstants.FIXTURE_SELECTOR).Hide();

            FixtureGraph topFixture = _library.BuildTopLevelGraph();
            writeFixture(topFixture);

            _library.ActiveFixtures.Each(writeFixture);

            return _top;
        }

        private void writeFixture(FixtureGraph fixture)
        {
            _fixtureTag = new FixtureTag(fixture);
            _top.Child(_fixtureTag);
        }
    }
}