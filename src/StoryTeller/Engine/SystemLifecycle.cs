using System;
using System.Linq;
using System.Collections.Generic;
using FubuCore.Conversion;

namespace StoryTeller.Engine
{
    public class SystemLifecycle : IDisposable
    {
        private readonly ISystem _system;
        private readonly object _locker = new object();

        public SystemLifecycle(ISystem system)
        {
            _system = system;
        }

        public void RecycleEnvironment()
        {
            _system.Recycle();
        }

        public void ExecuteContext(ITestContext context, Action action)
        {
            try
            {
                _system.Setup();

                var startups = context.StartupActionTypes.Select(x => _system.GetAction(x));

                startups.Each(x => x.Startup(context));

                action();

                // TODO -- this might need to go to system teardown
                startups.Each(x => x.Teardown(context));
            }
            finally
            {
                _system.Teardown();
            }
        }

        public void Dispose()
        {
            _system.Dispose();
        }

        public Func<Type, object> Resolver
        {
            get
            {
                return t => _system.Get(t);
            }
        }

        public IObjectConverter BuildConverter()
        {
            return _system.BuildConverter();
        }
    }
}