using HtmlTags;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class RemoveLinkTag : LinkTag
    {
        public RemoveLinkTag()
            : this(new FixtureGraph())
        {
        }

        public RemoveLinkTag(FixtureGraph fixture)
            : base(fixture.IsSingleSelection() ? "change" : "delete", "#", GrammarConstants.DELETE_STEP)
        {
        }
    }
}