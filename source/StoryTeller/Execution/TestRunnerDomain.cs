using System;
using System.IO;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Workspace;

namespace StoryTeller.Execution
{
    public class TestRunnerDomain : ITestRunnerDomain
    {
        private readonly IEventPublisher _publisher;
        private AppDomain _domain;
        private TestRunnerProxy _proxy;
        private readonly object _locker = new object();
        private FixtureLibrary _library;

        public TestRunnerDomain(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public void LoadProject(IProject project)
        {
            lock (_locker)
            {
                Teardown();

                try
                {
                    _publisher.Publish<BinaryRecycleStarted>();
                    _proxy = BuildProxy(project);

                    _library = _proxy.StartSystem(new FixtureAssembly(project), (MarshalByRefObject) _publisher);
                    _publisher.Publish(new BinaryRecycleFinished(_library));
                }
                catch (FileNotFoundException ex)
                {
                    if (ex.Message.Contains(GetType().Assembly.GetName().Name))
                    {
                        string message = "Could not find the StoryTeller.dll assembly in the target AppDomain.";
                        message +=
                            "\nYou will not be able to execute tests until the StoryTeller.dll file is copied to " +
                            project.GetBinaryFolder();

                        _publisher.Publish(new BinaryRecycleFailure
                        {
                            ErrorMessage = message
                        });
                    }

                    Teardown();
                }
                catch (Exception ex)
                {
                    Teardown();
                    _publisher.Publish(new BinaryRecycleFailure
                    {
                        ErrorMessage = ex.ToString()
                    });
                }


            }
        }

        public virtual TestRunnerProxy BuildProxy(IProject project)
        {
            var setup = new AppDomainSetup
            {
                ApplicationName = "StoryTeller-Testing-" + Guid.NewGuid(),
                ConfigurationFile = project.ConfigurationFileName,
                ShadowCopyFiles = "true",
                ApplicationBase = project.GetBinaryFolder()
            };

            _domain = AppDomain.CreateDomain("StoryTeller-Testing", null, setup);

            Type proxyType = typeof(TestRunnerProxy);
            var proxy =
                (TestRunnerProxy)
                _domain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);

            return proxy;
        }

        public void Teardown()
        {
            try
            {
                if (_proxy != null) _proxy.Dispose();
            }
            catch (Exception)
            {
            }

            _proxy = null;
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }

        public void RecycleEnvironment()
        {
            if (_proxy != null) _proxy.RecycleEnvironment();
        }

        public bool HasStarted()
        {
            return _proxy != null;
        }

        public TestResult RunTest(TestExecutionRequest request)
        {
            return _proxy.RunTest(request);
        }

        public void AbortCurrentTest()
        {
            _proxy.AbortCurrentTest();
        }

        public bool IsExecuting()
        {
            return _proxy.IsExecuting();
        }

        public FixtureLibrary Library
        {
            get { return _library; } }

        public void Dispose()
        {
            try
            {
                if (IsExecuting())
                {
                    AbortCurrentTest();
                }

                _proxy.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}