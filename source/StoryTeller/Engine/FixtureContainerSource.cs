using System;
using StructureMap;
using StructureMap.Configuration.DSL;
using System.Collections.Generic;

namespace StoryTeller.Engine
{
    public class FixtureContainerSource : IFixtureContainerSource
    {
        private readonly Registry _registry = new Registry();

        public FixtureContainerSource(IContainer container)
        {
            // TODO -- Replace this with Child Containers in SM 3.0
            container.Model.For<IFixture>().Instances.Each(i =>
            {
                RegisterFixture(i.Name, i.ConcreteType);
            });

            container.Model.For<IStartupAction>().Instances.Each(i =>
            {
                _registry.For(typeof (IStartupAction)).Use(i.ConcreteType).Named(i.Name);
            });
        }

        public void RegisterFixture(string name, Type fixtureType)
        {
            _registry.For(typeof (IFixture)).Add(fixtureType).Named(name);
        }

        public IContainer Build()
        {
            return new Container(_registry);
        }
    }
}