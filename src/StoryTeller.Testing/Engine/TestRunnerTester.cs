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
        private readonly List<string> _messages = new List<string>();

        public void Record(string message)
        {
            _messages.Add(message);
        }

        public string[] Messages
        {
            get
            {
                return _messages.ToArray();
            }
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public object Get(Type type)
        {
            var container = new Container(x => x.For<RecordingSystem>().Use(this));
            return container.GetInstance(type);
        }

        public void RegisterServices(ITestContext context)
        {
            Record("RegisterServices");
            context.Store(this);
        }

        public void SetupEnvironment()
        {
            Record("SetupEnvironment");
        }

        public void TeardownEnvironment()
        {
            Record("TeardownEnvironment");
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
    }

    public class Startup1Action : IStartupAction
    {
        private readonly RecordingSystem _system;

        public Startup1Action(RecordingSystem system)
        {
            _system = system;
        }

        public void Startup(ITestContext context)
        {
           _system.Record("Setup 1");
        }

        public void Teardown(ITestContext context)
        {
            _system.Record("Teardown 1");
        }
    }

    public class Startup2Action : IStartupAction
    {
        private readonly RecordingSystem _system;

        public Startup2Action(RecordingSystem system)
        {
            _system = system;
        }

        public void Startup(ITestContext context)
        {
            _system.Record("Setup 2");
        }

        public void Teardown(ITestContext context)
        {
            _system.Record("Teardown 2");
        }
    }

    public class RecordingFixture : Fixture
    {
        private readonly RecordingSystem _system;

        public RecordingFixture(RecordingSystem system)
        {
            _system = system;
        }

        public void Execute()
        {
            _system.Record("Execute");
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
            var container = new Container(x =>
            {
                x.For<IFixture>().Add<RecordingFixture>().Named("Recording");
                x.For<IStartupAction>().Add<Startup1Action>().Named("Startup1");
                x.For<IStartupAction>().Add<Startup2Action>().Named("Startup2");
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
                TimeoutInSeconds = 1200,
                StartupActions = new string[]{"Startup1", "Startup2"}
            });

        }

        [Test]
        public void should_call_setup_on_both_startup_actions()
        {
            system.Messages.ShouldContain("Setup 1");
            system.Messages.ShouldContain("Setup 2");
        }

        [Test]
        public void should_call_teardown_on_both_startup_actions()
        {
            system.Messages.ShouldContain("Teardown 1");
            system.Messages.ShouldContain("Teardown 2");
        }

        [Test]
        public void should_start_the_application_because_it_has_not_already_been_started()
        {
            system.Messages.ShouldContain("SetupEnvironment");
        }

        [Test]
        public void should_register_services()
        {
            system.Messages.ShouldContain("RegisterServices");
        }

        [Test]
        public void should_setup_the_execution()
        {
            system.Messages.ShouldContain("Setup");
        }

        [Test]
        public void should_have_executed_the_fixture()
        {
            system.Messages.ShouldContain("Execute");
        }

        [Test]
        public void should_teardown_after_the_test()
        {
            system.Messages.ShouldContain("Teardown");
        }

        [Test]
        public void should_not_teardown_the_environment()
        {
            system.Messages.ShouldNotContain("TeardownEnvironment");
        }

        [Test]
        public void should_do_the_steps_in_the_proper_order()
        {
            system.Messages.Length.ShouldEqual(9);

            system.Messages[0].ShouldEqual("SetupEnvironment");
            system.Messages[1].ShouldEqual("RegisterServices");
            system.Messages[2].ShouldEqual("Setup");
            system.Messages[3].ShouldEqual("Setup 1");
            system.Messages[4].ShouldEqual("Setup 2");


            system.Messages[5].ShouldEqual("Execute");
            system.Messages[6].ShouldEqual("Teardown 1");
            system.Messages[7].ShouldEqual("Teardown 2");

            system.Messages[8].ShouldEqual("Teardown");
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

        [Test]
        public void setup_environment_should_only_be_called_once()
        {
            system.Messages[0].ShouldEqual("SetupEnvironment");
            system.Messages.Count(x => x == "SetupEnvironment").ShouldEqual(1);
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
            runner.Lifecycle.StartApplication();

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
            system.Messages.Length.ShouldEqual(5);

            system.Messages[0].ShouldEqual("SetupEnvironment");
            system.Messages[1].ShouldEqual("RegisterServices");
            system.Messages[2].ShouldEqual("Setup");
            system.Messages[3].ShouldEqual("Execute");
            system.Messages[4].ShouldEqual("Teardown");
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
            lifecycle.StartApplication();

            lifecycle.RecycleEnvironment();

            var test = new Test("something");
            test.Add(new Section("Recording").WithStep("Execute"));

            runner.RunTest(new TestExecutionRequest()
            {
                Test = test,
                TimeoutInSeconds = 1200
            });

            system.Messages.Each(x => Debug.WriteLine(x));
        }

        [Test]
        public void should_do_the_steps_in_the_proper_order_and_not_repeat_SetupEnvironment()
        {
            system.Messages.Length.ShouldEqual(7);

            system.Messages[0].ShouldEqual("SetupEnvironment");
            system.Messages[1].ShouldEqual("TeardownEnvironment");
            system.Messages[2].ShouldEqual("SetupEnvironment");

            system.Messages[3].ShouldEqual("RegisterServices");
            system.Messages[4].ShouldEqual("Setup");
            system.Messages[5].ShouldEqual("Execute");
            system.Messages[6].ShouldEqual("Teardown");
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
            system.Messages.ShouldHaveTheSameElementsAs("TeardownEnvironment");
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

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterServices(ITestContext context)
        {
        }

        public void SetupEnvironment()
        {
        }

        public void TeardownEnvironment()
        {
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