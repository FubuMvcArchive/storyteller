using HtmlTags;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller.Html
{
    public class SectionTag : HtmlTag
    {
        private readonly Section _section;
        private readonly FixtureGraph _fixture;
        private readonly HtmlTag _stepHolder;

        public SectionTag(Section section, FixtureGraph fixture)
            : base("div")
        {
            _section = section;
            _fixture = fixture;

            AddClass("section");
            AddClass("embedded");
            
            Add("div").AddClass("section-header")
                .Add("div").AddClass(HtmlClasses.SECTION_TITLE).Text(_fixture.Title);

            _stepHolder = Add("div").Add("div").AddClass("step-holder");
        }

        public HtmlTag StepHolder { get { return _stepHolder; } }

        public void WriteResults(ITestContext context)
        {
            context.ResultsFor(_section).ForExceptionText(text => _stepHolder.Child(new ExceptionTag(text)));
        }
    }


    public class EmbeddedSectionTag : HtmlTag
    {
        private readonly EmbeddedSection _section;
        private readonly IStep _step;

        public EmbeddedSectionTag(EmbeddedSection section, IStep step) : base("div")
        {
            _section = section;
            _step = step;

            AddClass("embedded");

            AddClass(section.Style.ToString());

            if (section.Style == EmbedStyle.TitledAndIndented)
            {
                Add("div").AddClass(HtmlClasses.SECTION_TITLE).Text(section.Label);
            }
        }

        public void WritePreview(ITestContext context)
        {
            
        }

        public void WriteResults(ITestContext context)
        {
            context.ResultsFor(_step).ForExceptionText(text => Child(new ExceptionTag(text)));
        }
    }
}