using System;
using StructureMap;
using StructureMap.Configuration.DSL;
using System.Collections.Generic;
using System.Linq;

namespace StoryTeller.Engine
{
    public class FixtureContainerSource : IFixtureContainerSource
    {
        private readonly IContainer _container;
        private readonly Registry _registry = new Registry();

        public FixtureContainerSource(IContainer container)
        {
            _container = container;
            //// TODO -- Replace this with Child Containers in SM 3.0
            //container.Model.For<IFixture>().Instances.Each(i =>
            //{
            //    RegisterFixture(i.Name, i.ConcreteType);
            //});

            //container.Model.For<IStartupAction>().Instances.Each(i =>
            //{
            //    _registry.For(typeof (IStartupAction)).Use(i.ConcreteType).Named(i.Name);
            //});
        }

        public void RegisterFixture(string name, Type fixtureType)
        {
            _container.Configure(x => x.For(typeof(IFixture)).Add(fixtureType).Named(name));
            //_registry.For(typeof (IFixture)).Add(fixtureType).Named(name);
        }

        public IContainer Build()
        {
            return _container.GetNestedContainer();
            //return new Container(_registry);
        }
    }
}