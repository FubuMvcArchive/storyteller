namespace StoryTeller.Domain
{
    public interface ITestVisitor
    {
        void RunStep(IStep step);
        void WriteComment(Comment comment);
        void StartSection(Section section);
        void EndSection(Section section);
    }
}