using System;
using FubuCore.Conversion;
using StructureMap;

namespace StoryTeller.Engine
{
    public interface ISystem
    {
        object Get(Type type);
        void RegisterServices(ITestContext context);
        void SetupEnvironment();
        void TeardownEnvironment();
        void Setup();
        void Teardown();
        void RegisterFixtures(FixtureRegistry registry);

        IContainer BuildFixtureContainer();        
        IObjectConverter BuildConverter();
    }

    public static class SystemExtensions
    {
        public static T Get<T>(this ISystem system)
        {
            return (T)system.Get(typeof (T));
        }

        public static IStartupAction GetAction(this ISystem system, Type type)
        {
            return (IStartupAction) system.Get(type);
        }
    }
}