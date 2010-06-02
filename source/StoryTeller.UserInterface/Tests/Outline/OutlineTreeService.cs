using System;
using StoryTeller.Domain;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public class OutlineTreeService : IOutlineTreeService
    {
        private readonly ProjectContext _context;
        private OutlineNode _topNode;

        public OutlineTreeService(ProjectContext context)
        {
            _context = context;
        }

        public OutlineNode BuildNode(Test test, IOutlineController controller)
        {
            var configurer = new OutlineConfigurer(controller);
            var builder = new OutlineTreeBuilder(test, _context.Library, configurer);

            _topNode = builder.Build();
            return _topNode;
        }

        public void RedrawNode(OutlineNode topNode, IPartHolder partHolder)
        {
            throw new NotImplementedException();
        }

        public void SelectNodeFor(ITestPart part)
        {
            throw new NotImplementedException();
        }
    }
}