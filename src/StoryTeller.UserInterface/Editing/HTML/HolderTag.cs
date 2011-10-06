using HtmlTags;
using HtmlTags.Extended.TagBuilders;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class HolderTag : HtmlTag
    {
        public HolderTag(FixtureGraph fixture)
            : base("div")
        {
            Add("div").AddClass(GrammarConstants.STEP_HOLDER);

            if (fixture.IsSingleSelection() || fixture.Policies.SelectionMode == SelectionMode.MandatoryAutoSelect)
            {
                return;
            }

            this.ActionLink(fixture.Policies.AddGrammarText, GrammarConstants.ADD_SECTION_ACTIVATOR);
        }
    }
}