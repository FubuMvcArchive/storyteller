using System;
using Examples;
using Examples.Fixtures.Example;
using FubuCore.Conversion;
using NUnit.Framework;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class FixtureGraphTester
    {
        [Test]
        public void build_by_assembly()
        {
            var graph = FixtureGraph.ForAssemblies(typeof (ExampleSystem).Assembly.GetName().Name);

            graph.Build(typeof (Calculator2Fixture).GetFixtureAlias())
                .ShouldBeOfType<Calculator2Fixture>();

            graph.SystemTypes.ShouldContain(typeof(ExampleSystem));
        }

        [Test]
        public void build_for_app_domain()
        {
            var graph = FixtureGraph.ForAppDomain();

            graph.Build(typeof(Calculator2Fixture).GetFixtureAlias())
                .ShouldBeOfType<Calculator2Fixture>();

            graph.SystemTypes.ShouldContain(typeof(ExampleSystem));


            graph.Build("Scooby").ShouldBeOfType<ScoobyFixture>();

            graph.SystemTypes.ShouldContain(typeof(ScoobySystem));
        }
    }

    public class ScoobySystem : ISystem
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
            throw new NotImplementedException();
        }

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void Teardown()
        {
            throw new NotImplementedException();
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            throw new NotImplementedException();
        }

        public IObjectConverter BuildConverter()
        {
            throw new NotImplementedException();
        }
    }
    public class ScoobyFixture : Fixture{}
}