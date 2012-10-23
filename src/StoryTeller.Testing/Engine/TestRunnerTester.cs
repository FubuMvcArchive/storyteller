using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore.Conversion;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Engine.Constraints;
using StoryTeller.Execution;
using StoryTeller.Model;
using System.Linq;
using StructureMap;
using TestContext = StoryTeller.Engine.TestContext;

namespace StoryTeller.Testing.Engine
{

    public class RecordingSystem : ISystem
    {
        private static readonly List<string> _messages = new List<string>();

        public void Record(string message)
        {
            _messages.Add(message);
        }

        public static string[] Messages
        {
            get
            {
                return _messages.ToArray();
            }
        }

        public static void Clear()
        {
            _messages.Clear();
        }

        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
            Record("Recycle");
        }

        public object Get(Type type)
        {
            var container = new Container(x => x.For<RecordingSystem>().Use(this));
            return container.GetInstance(type);
        }

        public void Setup()
        {
            Record("Setup");
        }

        public void Teardown()
        {
            Record("Teardown");
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            
        }

        public IObjectConverter BuildConverter()
        {
            return new ObjectConverter();
        }

        public void Dispose()
        {
            Record("Dispose");
        }
    }


    public class RecordingFixture : Fixture
    {


        public void Execute()
        {
            Retrieve<RecordingSystem>().Record("Execute");
        }        
    }

    [TestFixture]
    public class when_executing_a_test
    {
        private RecordingSystem system;
        private TestRunner runner;

        [SetUp]
        public void SetUp()
        {
            RecordingSystem.Clear();

            var container = new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
            });
            var registry = new FixtureRegistry();
            registry.AddFixture<RecordingFixture>();
            registry.AddFixturesToContainer(container);
            
            system = new RecordingSystem();
            var fixtureContainerSource = new FixtureContainerSource(container);

            runner = new TestRunner(system, new FixtureLibrary(), fixtureContainerSource);

            var test = new Test("something");
            test.Add(new Section("Recording").WithStep("Execute"));

            runner.RunTest(new TestExecutionRequest()
            {
                Test = test,
                TimeoutInSeconds = 1200
            });

        }

        [Test]
        public void should_setup_the_execution()
        {
            RecordingSystem.Messages.ShouldContain("Setup");
        }

        [Test]
        public void should_have_executed_the_fixture()
        {
            RecordingSystem.Messages.ShouldContain("Execute");
        }

        [Test]
        public void should_teardown_after_the_test()
        {
            RecordingSystem.Messages.ShouldContain("Teardown");
        }

        [Test]
        public void should_do_the_steps_in_the_proper_order()
        {

            RecordingSystem.Messages.ShouldHaveTheSameElementsAs("Setup", "Execute", "Teardown");
        }
    }

    [TestFixture]
    public class starting_the_application_only_happens_on_the_first_Test_if_necessary
    {
        private RecordingSystem system;
        private TestRunner runner;

        [SetUp]
        public void SetUp()
        {
            var container = new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
            });

            var registry = new FixtureRegistry();
            registry.AddFixture<RecordingFixture>();
            registry.AddFixturesToContainer(container);

            var library = FixtureLibrary.For(x => x.AddFixture<RecordingFixture>());
            system = new RecordingSystem();
            var fixtureContainerSource = new FixtureContainerSource(new Container());


            runner = new TestRunner(system, library, fixtureContainerSource);

            var test = new Test("something");
            test.Add(new Section("Recording").WithStep("Execute"));

            runner.RunTest(new TestExecutionRequest()
            {
                Test = test,
                TimeoutInSeconds = 1200
            });

            runner.RunTest(new TestExecutionRequest()
            {
                Test = test,
                TimeoutInSeconds = 1200
            });

        }

    }

    [TestFixture]
    public class when_executing_a_test_after_the_application_has_been_started
    {
        private RecordingSystem system;
        private TestRunner runner;

        [SetUp]
        public void SetUp()
        {
            RecordingSystem.Clear();

            var container = new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
            });

            var registry = new FixtureRegistry();
            registry.AddFixture<RecordingFixture>();
            registry.AddFixturesToContainer(container);


            var library = FixtureLibrary.For(x => x.AddFixture<RecordingFixture>());
            system = new RecordingSystem();


            var fixtureContainerSource = new FixtureContainerSource(container);

            

            runner = new TestRunner(system, library, fixtureContainerSource);

            var test = new Test("something");
            test.Add(new Section("Recording").WithStep("Execute"));

            runner.RunTest(new TestExecutionRequest()
            {
                Test = test,
                TimeoutInSeconds = 1200
            });

        }

        [Test]
        public void should_do_the_steps_in_the_proper_order_and_not_repeat_SetupEnvironment()
        {
            RecordingSystem.Messages.ShouldHaveTheSameElementsAs("Setup", "Execute", "Teardown");
        }
    }

    [TestFixture]
    public class when_recycling_the_test_runner_environment
    {
        private RecordingSystem system;
        private TestRunner runner;
        private SystemLifecycle lifecycle;

        [SetUp]
        public void SetUp()
        {
            RecordingSystem.Clear();

            var container = new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
            });

             var registry = new FixtureRegistry();
            registry.AddFixturesToContainer(container);

            var library = FixtureLibrary.For(x => x.AddFixture<RecordingFixture>());
            system = new RecordingSystem();


            var fixtureContainerSource = new FixtureContainerSource(container);

            lifecycle = new SystemLifecycle(system);
            runner = new TestRunner(lifecycle, library, fixtureContainerSource);

            lifecycle.RecycleEnvironment();

            var test = new Test("something");
            test.Add(new Section("Recording").WithStep("Execute"));

            runner.RunTest(new TestExecutionRequest()
            {
                Test = test,
                TimeoutInSeconds = 1200
            });

            RecordingSystem.Messages.Each(x => Debug.WriteLine(x));
        }

        [Test]
        public void should_do_the_steps_in_the_proper_order()
        {
            RecordingSystem.Messages.ShouldHaveTheSameElementsAs("Recycle", "Setup", "Execute", "Teardown");
        }
    }

    [TestFixture]
    public class when_disposing_a_test_runner
    {
        private RecordingSystem system;
        private TestRunner runner;

        [SetUp]
        public void SetUp()
        {
            var container = new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
            });

            var registry = new FixtureRegistry();
            registry.AddFixturesToContainer(container);

            var library = new FixtureLibrary();
            system = new RecordingSystem();


            var fixtureContainerSource = new FixtureContainerSource(new Container());
            //fixtureContainerSource.RegisterFixture("Recording", typeof(RecordingFixture));

            runner = new TestRunner(system, library, fixtureContainerSource);

            runner.Dispose();


        }

        [Test]
        public void should_teardown_the_environment()
        {
            RecordingSystem.Messages.ShouldHaveTheSameElementsAs("Dispose");
        }
    }
    

    [TestFixture]
    public class TestRunnerTester : AAAMockingContext<TestContext>
    {


        [Test]
        public void run_a_test_when_setup_blows_up_do_not_rethrow_exception_and_log_the_exception_to_the_test()
        {
            var runner = TestRunnerBuilder.ForSystem<SystemThatBlowsUpInSetup>();

            var test = new Test("Some test");

            runner.RunTest(test);

            test.LastResult.ExceptionText.ShouldContain("NotImplementedException");
        }

        [Test]
        public void run_a_test_when_teardown_blows_up_do_not_rethrow_exception_and_log_the_exception_to_the_test()
        {
            var system = MockRepository.GenerateMock<ISystem>();
            system.Stub(x => x.BuildConverter()).Return(new ObjectConverter());

            var fixtureContainerSource = new FixtureContainerSource(new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
            }));
            var runner = new TestRunner(system, new FixtureLibrary(), fixtureContainerSource);

            system.Expect(x => x.Teardown()).Throw(new NotImplementedException());

            var test = new Test("something");
            runner.RunTest(test);

            test.LastResult.ExceptionText.ShouldContain("NotImplementedException");


        }
    }


    public class SystemThatBlowsUpInSetup : ISystem
    {
        public T Get<T>() where T : class
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
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            
        }

        public IObjectConverter BuildConverter()
        {
            return new ObjectConverter();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }




    public class GrammarThatAssertsFailure : IGrammar
    {
        #region IGrammar Members

        public void Execute(IStep containerStep, ITestContext context)
        {
            StoryTellerAssert.Fail("I don't want to run");
        }

        public GrammarStructure ToStructure(FixtureLibrary library)
        {
            throw new NotImplementedException();
        }

        public string Description { get { throw new NotImplementedException(); } }

        #endregion
    }

}