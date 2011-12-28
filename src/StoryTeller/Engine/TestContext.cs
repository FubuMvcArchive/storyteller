using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using FubuCore;
using FubuCore.Conversion;
using FubuCore.Util;
using Microsoft.Practices.ServiceLocation;
using StoryTeller.Domain;
using StructureMap;
using StructureMap.Query;

namespace StoryTeller.Engine
{
    public interface IFixtureContext
    {
        IGrammar FindGrammar(string grammarKey);
        void LoadFixture(string fixtureKey, ITestPart part);
        void LoadFixture(IFixture fixture, ITestPart part);
        void LoadFixture<T>(ITestPart part) where T : IFixture;
        void RevertFixture(ITestPart part);

        IFixture RetrieveFixture<T>() where T : IFixture;
        IFixture RetrieveFixture(string fixtureName);
    }

    public interface IExceptionTarget
    {
        void CaptureException(string exceptionText);
    }


    public interface ITestContext
    {
        IEnumerable<Type> StartupActionTypes { get; }

        object CurrentObject { get; set; }
        IObjectConverter Finder { get; }
        bool Matches(object expected, object actual);
        
        void Store<T>(T data);
        T Retrieve<T>();
        object Retrieve(Type type);

        void IncrementRights();
        void IncrementWrongs();
        void IncrementExceptions();
        void IncrementSyntaxErrors();

        void ExecuteWithFixture<T>(StepLeaf leaf, ITestPart exceptionTarget) where T : IFixture;
        void RunStep(IGrammar grammar, IStep step);

        void PerformAction(IStep step, GrammarAction action);

        StepResults ResultsFor(ITestPart part);
        string GetDisplay(object value);

        Counts Counts { get; }
        string TraceText { get; }
        void Trace(string text);
    }

    public static class TestContextExtensions
    {
        public static void Configure<T>(this ITestContext context, Action<T> configure)
        {
            configure(context.Retrieve<T>());
        }

        public static T Current<T>(this ITestContext context)
        {
            return (T) context.CurrentObject;
        }

        public static void PerformAction(this ITestContext context, IStep step, Action action)
        {
            context.PerformAction(step, (c, s) => action());
        }
    }

    public interface ITestRun
    {
        void Abort();
    }


    public interface IFixtureVisitor
    {
        int FixtureCount { set; }
        void ReadFixture(string fixtureName, IFixture fixture);
        void LogFixtureFailure(string fixtureName, Exception exception);
    }

    public class TestContext : ITestContext, ITestVisitor, IFixtureContext
    {
        private readonly IContainer _container;
        private readonly Stack<IFixture> _fixtures = new Stack<IFixture>();
        private readonly Test _test;
        private bool _fixtureIsInvalid;
        private ITestObserver _listener;
        private IPrincipal _principal;
        private readonly Cache<ITestPart, StepResults> _results = new Cache<ITestPart,StepResults>(step => new StepResults());
        private readonly StringWriter _traceWriter = new StringWriter();
        private readonly Counts _counts = new Counts();

        public TestContext()
            : this(new Container(), new Test("FAKE"), new TraceListener())
        {
        }

        public TestContext(IContainer container, Test test, ITestObserver listener)
        {
            _container = container;
            _test = test;
            _listener = listener;
            _container.Inject<ITestContext>(this);
            _container.Inject(test);

            _container.Configure(x =>
            {
                x.For<IFixture>().AlwaysUnique();
                x.For<IFixtureContext>().Use(this);
                x.SetAllProperties(o => o.OfType<IFixtureContext>());

                // This is a fallback mechanism.  If the IObjectConverter is not explicitly registered somewhere else, this will be the default
                x.For<IObjectConverter>().Add<ObjectConverter>();
                x.For<IServiceLocator>().Use<StructureMapServiceLocator>();
            });

            StartupActionNames = new string[0];

            BackupResolver = t =>
            {
                throw new ApplicationException("This service is not registered");
            };
        }

        public Func<Type, object> BackupResolver { get; set; }

        public TestContext(IContainer container)
            : this(container, new Test("FAKE"), new TraceListener())
        {
        }

        public TestContext(Action<FixtureRegistry> action)
            : this(FixtureRegistry.ContainerFor(action), new Test("FAKE"), new TraceListener())
        {
        }

        public string[] StartupActionNames { get; set; }

        public IContainer Container
        {
            get { return _container; }
        }

        public string TraceText { get { return _traceWriter.GetStringBuilder().ToString(); } }

        public ITestObserver Listener { get { return _listener; } set { _listener = value; } }

        public Counts Counts { get { return _counts; } }

        public IFixture CurrentFixture { get { return _fixtures.Peek(); } }

        public Test Test { get { return _test; } }
        
        private bool shouldStop { 
            get
            {
                return !_listener.CanContinue(_counts);
            } 
        }

        #region IFixtureContext Members

        public void RevertFixture(ITestPart part)
        {
            if (_fixtureIsInvalid)
            {
                _fixtureIsInvalid = false;
                return;
            }

            if (part == null) throw new ArgumentNullException("part");

            IFixture fixture = _fixtures.Pop();
            if (fixture != null)
            {
                performAction(part, fixture.TearDown);
            }

            _fixtureIsInvalid = false;
        }

        public IFixture RetrieveFixture<T>() where T : IFixture
        {
            // TEMP HACKERY!!!!
            _container.Configure(x => x.For<T>().AlwaysUnique());

            return _container.GetInstance<T>();
        }

        public IFixture RetrieveFixture(string fixtureName)
        {
            return _container.GetInstance<IFixture>(fixtureName);
        }

        public IGrammar FindGrammar(string grammarKey)
        {
            return CurrentFixture[grammarKey];
        }

        public void LoadFixture(string fixtureKey, ITestPart part)
        {
            var results = ResultsFor(part);

            try
            {
                var fixture = _container.GetInstance<IFixture>(fixtureKey);
                LoadFixture(fixture, part);
            }
            catch (StructureMapException e)
            {
                _fixtureIsInvalid = true;
                _listener.Exception(e.ToString());

                if (e.ErrorCode == 200)
                {
                    results.CaptureException("Unable to find a Fixture named '{0}'".ToFormat(fixtureKey));
                    IncrementSyntaxErrors();
                }
                else
                {
                    IncrementExceptions();
                    if (e.InnerException != null)
                    {
                        results.CaptureException(e.InnerException.ToString());
                    }
                    else
                    {
                        results.CaptureException(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                _fixtureIsInvalid = true;
                _listener.Exception(e.ToString());
                results.CaptureException(e.ToString());
                IncrementExceptions();
            }
        }

        public void LoadFixture<T>(ITestPart part) where T : IFixture
        {
            var fixture = RetrieveFixture<T>();



            LoadFixture(fixture, part);
        }

        public void LoadFixture(IFixture fixture, ITestPart part)
        {
            if (part == null) throw new ArgumentNullException("part");

            performAction(part, () =>
            {
                fixture.Context = this;
                fixture.SetUp(this);
            });

            _fixtures.Push(fixture);
        }

        #endregion

        #region ITestContext Members

        public bool Matches(object expected, object actual)
        {
            return Retrieve<EquivalenceChecker>().IsEqual(expected, actual);
        }

        public string GetDisplay(object value)
        {
            if (value == null || value == DBNull.Value) return Step.NULL;
            if (string.Empty.Equals(value)) return Step.BLANK;

            return Retrieve<Stringifier>().GetString(value);
        }

        public virtual void Store<T>(T data)
        {
            _container.Inject(data);
        }

        public T Retrieve<T>()
        {
            if (typeof(T).IsConcrete())
            {
                return _container.GetInstance<T>();
            }

            return _container.Model.HasDefaultImplementationFor<T>()
                ? _container.GetInstance<T>()
                : (T)BackupResolver(typeof(T));

        }


        public object Retrieve(Type type)
        {
            if (type.IsConcrete()) return _container.GetInstance(type);

            return _container.TryGetInstance(type) ?? BackupResolver(type);
        }

        public void IncrementRights()
        {
            Counts.IncrementRights();
        }

        public void IncrementWrongs()
        {
            Counts.IncrementWrongs();
        }

        [Obsolete("have this replaced with a method that takes in the exception text at the same time and goes to the result")]
        public void IncrementExceptions()
        {
            Counts.IncrementExceptions();
        }

        [Obsolete("see above")]
        public void IncrementSyntaxErrors()
        {
            Counts.IncrementSyntaxErrors();
        }

        public void ExecuteWithFixture<T>(StepLeaf leaf, ITestPart exceptionTarget) where T : IFixture
        {
            LoadFixture<T>(exceptionTarget);

            leaf.AcceptVisitor(this);

            RevertFixture(exceptionTarget);
        }

        public IEnumerable<Type> StartupActionTypes
        {
            get { return StartupActionNames.Select(x => GetStartupType(x)); }
        }

        public object CurrentObject { get; set; }

        public void RunStep(IGrammar grammar, IStep step)
        {
            runStep(step, () => grammar.Execute(step, this));
        }

        public virtual void PerformAction(IStep step, GrammarAction action)
        {
            Action continuation = () => action(step, this);

            performAction(step, continuation);
        }

        private void performAction(ITestPart part, Action continuation)
        {
            try
            {
                continuation();
            }
            catch (Exception ex)
            {
                _listener.Exception(ex.ToString());
                IncrementExceptions();
                ResultsFor(part).CaptureException(ex.ToString());
            }
        }

        public StepResults ResultsFor(ITestPart part)
        {
            return _results[part];
        }

        public IObjectConverter Finder
        {
            get
            {
                return Retrieve<IObjectConverter>();
            }
            set
            {
                Store(value);
            }
        }

        #endregion

        #region ITestVisitor Members

        public void RunStep(IStep step)
        {
            if (shouldStop) return;

            storePrincipal();

            if (_fixtureIsInvalid) return;

            runStep(step, () =>
            {
                IGrammar grammar = FindGrammar(step.GrammarKey);
                grammar.Execute(step, this);
            });
        }

        void ITestVisitor.WriteTags(Tags tags)
        {
            // no-op;
        }

        void ITestVisitor.WriteComment(Comment comment)
        {
            // no-op;
        }

        void ITestVisitor.StartSection(Section section)
        {
            if (shouldStop) return;

            storePrincipal();

            _listener.StartSection(section);


            section.StartFixture(this);
        }

        void ITestVisitor.EndSection(Section section)
        {
            if (shouldStop) return;

            storePrincipal();

            RevertFixture(section);

            _listener.FinishSection(section);
        }

        #endregion

        private void runStep(IStep step, Action action)
        {
            _listener.StartStep(step);

            try
            {
                action();
            }
            catch (ThreadAbortException e)
            {
                // Timeout is logged elsewhere
                _listener.Exception(e.ToString());
            }
            catch (Exception e)
            {
                _listener.Exception(e.ToString());
                IncrementExceptions();
                ResultsFor(step).CaptureException(e.ToString());
            }

            _listener.FinishStep(step);
        }

        private void storePrincipal()
        {
            Thread.CurrentPrincipal = _principal;
        }


        public void Execute()
        {
            TextWriter originalConsole = Console.Out;
            var traceListener = new TextWriterTraceListener(_traceWriter);
            Debug.Listeners.Add(traceListener);
            
            //Console.SetOut(_traceWriter);


            try
            {
                _principal = Thread.CurrentPrincipal;

                executeParts();

                
            }
            finally
            {
                Debug.Listeners.Remove(traceListener);
                Console.SetOut(originalConsole);
            }
        }

        public void Trace(string text)
        {
            _traceWriter.WriteLine(text);
        }

        private void executeParts()
        {
            foreach (ITestPart part in _test.Parts)
            {
                part.AcceptVisitor(this);
                if (shouldStop)
                {
                    ResultsFor(_test).CaptureException("TestListener stopped the test before completion");
                    break;
                }
            }
        }

        public Type GetStartupType(string name)
        {
            return _container.Model.For<IStartupAction>().Find(name).ConcreteType;
        }
    }

    public class StructureMapServiceLocator : ServiceLocatorImplBase
    {
        private readonly IContainer _container;

        public StructureMapServiceLocator(IContainer container)
        {
            _container = container;
        }

        public IContainer Container { get { return _container; } }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return key.IsEmpty()
                       ? _container.GetInstance(serviceType)
                       : _container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>().AsEnumerable();
        }

        public override TService GetInstance<TService>()
        {
            return _container.GetInstance<TService>();
        }

        public override TService GetInstance<TService>(string key)
        {
            return _container.GetInstance<TService>(key);
        }

        public override IEnumerable<TService> GetAllInstances<TService>()
        {
            return _container.GetAllInstances<TService>();
        }
    }
}