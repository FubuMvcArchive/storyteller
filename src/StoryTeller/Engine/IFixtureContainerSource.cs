using StructureMap;

namespace StoryTeller.Engine
{
    public interface IFixtureContainerSource
    {
        IContainer Build();
    }

    public class FixtureContainerSource : IFixtureContainerSource
    {
        private readonly IContainer _container;

        public FixtureContainerSource(IContainer container)
        {
            _container = container;
        }

        public IContainer Build()
        {
            return _container.GetNestedContainer();
        }
    }
}