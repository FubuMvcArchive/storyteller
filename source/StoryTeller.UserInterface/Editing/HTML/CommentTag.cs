using HtmlTags;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class CommentTag : HtmlTag
    {
        public CommentTag()
            : base("div")
        {
            AddClass(GrammarConstants.COMMENT);
            AddClass(GrammarConstants.STEP);
            MetaData(GrammarConstants.KEY, GrammarConstants.COMMENT);

            this.Span(x =>
            {
                x.AddClass(GrammarConstants.COMMENT_TEXT_HOLDER);
                x.Span(s => s.AddClass(GrammarConstants.COMMENT_TEXT));
                x.ActionLink("edit", GrammarConstants.COMMENT_EDITOR);
            });

            this.Span(x =>
            {
                x.AddClass(GrammarConstants.COMMENT_EDITOR_HOLDER);
                x.Add("textarea").AddClass("editor").Attr("rows", "4");
                x.ActionLink("close", GrammarConstants.COMMENT_CLOSER);
            });

            Child(new RemoveLinkTag());
        }
    }
}