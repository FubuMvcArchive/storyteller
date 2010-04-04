using HtmlTags;

namespace StoryTeller.Html
{
    public class ExceptionTag : HtmlTag
    {
        public ExceptionTag(string text) : base("div")
        {
            AddClass(HtmlClasses.EXCEPTION);
            Add("hr");
            Add("pre").Text(text);
            Add("hr");
        }

        public string ExceptionText()
        {
            return Children[1].Text();
        }
    }
}