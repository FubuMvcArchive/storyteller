using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace StoryTeller.Engine
{
    public class AssemblyResult
    {
        public AssemblyResult(Assembly assembly)
        {
            Name = assembly.GetName().Name;

            try
            {
                var types = assembly.GetExportedTypes();

                FixtureTypes = types.Where(x => FubuCore.TypeExtensions.IsConcreteWithDefaultCtor(x) && FubuCore.TypeExtensions.IsConcreteTypeOf<IFixture>(x));
                SystemTypes = types.Where(x => x.IsConcreteTypeOf<ISystem>() && x.IsConcreteWithDefaultCtor());

                WasAbleToScan = true;
            }
            catch (Exception)
            {
                // Nothing you can really do here.
            }
        }

        public bool WasAbleToScan;

        public string Name;

        public IEnumerable<Type> FixtureTypes = new Type[0];
        public IEnumerable<Type> SystemTypes = new Type[0];
    }
}