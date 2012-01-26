using System;
using FubuCore.Conversion;
using NUnit.Framework;
using StoryTeller.Engine;
using StructureMap;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class TestRunnerBuilderTester
    {
        [Test]
        public void should_check_if_system_implements_IRequireFixtureContainer()
        {
            var stub = new TestContainerSystemStub();
            TestRunnerBuilder.BuildFixtureContainer(stub);
            stub.ConfigureFixtureContainerWasCalled.ShouldBeTrue();
        }

        private class TestContainerSystemStub : ISystem, IRequireFixtureContainer
        {
            public object Get(Type type)
            {
                throw new NotImplementedException();
            }

            public void RegisterServices(ITestContext context)
            {
                throw new NotImplementedException();
            }

            public void SetupEnvironment()
            {
                throw new NotImplementedException();
            }

            public void TeardownEnvironment()
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

            public void ConfigureFixtureContainer(IContainer container)
            {
                ConfigureFixtureContainerWasCalled = true;
            }

            public bool ConfigureFixtureContainerWasCalled { get; set; }
        }
    }
}