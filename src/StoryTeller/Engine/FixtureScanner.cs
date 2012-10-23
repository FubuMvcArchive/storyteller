using System;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StoryTeller.Engine
{
    public class FixtureScanner : IRegistrationConvention
    {
        private readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();

        public void Process(Type type, Registry registry)
        {
            if (type.Assembly == _thisAssembly)
            {
                return;
            }

            if (type.CanBeCastTo(typeof(IFixture)))
            {
                registry.AddType(typeof(IFixture), type, type.GetFixtureAlias());
            }
        }
    }
}