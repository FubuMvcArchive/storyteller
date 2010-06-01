using StoryTeller.Domain;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public interface IOutlineTreeService
    {
        OutlineNode BuildNode(Test test, IOutlineController controller);
        void RedrawNode(OutlineNode topNode, IPartHolder partHolder);
        void SelectNodeFor(ITestPart part);
    }
}