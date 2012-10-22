using System;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Conversion;
using StructureMap;

namespace StoryTeller.Engine
{
    public interface IExecutionContext : IDisposable
    {
        IServiceLocator Services { get; }
        BindingRegistry BindingRegistry { get; }
    }

    public interface ISystem : IDisposable
    {
        IExecutionContext CreateContext();
        void Recycle();

        object Get(Type type);
        void Setup();
        void Teardown();

        void RegisterFixtures(FixtureRegistry registry);

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