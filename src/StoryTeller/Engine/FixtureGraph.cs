using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuCore;

namespace StoryTeller.Engine
{
    public class FixtureGraph
    {
        private readonly Cache<string, Type> _fixtureTypes = new Cache<string,Type>(name => {
            throw new NonExistentFixtureException(name);
        });

        private readonly IList<Type> _systemTypes = new List<Type>(); 

        private FixtureGraph(IEnumerable<Assembly> assemblies)
        {
            var tasks = assemblies.Select(assem => Task.Factory.StartNew(() => new AssemblyResult(assem)));

            var results = tasks.Select(x => x.Result);

            // TODO -- blow up if duplicate fixture names

            results.SelectMany(x => x.FixtureTypes).Each(type => {
                _fixtureTypes[type.GetFixtureAlias()] = type;
            });

            _systemTypes.AddRange(results.SelectMany(x => x.SystemTypes));
        }


        public IFixture Build(string fixtureName)
        {
            return (IFixture) Activator.CreateInstance(_fixtureTypes[fixtureName]);
        }

        public IEnumerable<Type> SystemTypes
        {
            get { return _systemTypes; }
        }


        // TODO -- this is so common here and in FubuMVC, just get something into FubuCore
        public static IEnumerable<Assembly> AssembliesFromPath(string path)
        {


            var assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                       Path.GetExtension(file).Equals(
                           ".dll",
                           StringComparison.OrdinalIgnoreCase));

            foreach (string assemblyPath in assemblyPaths)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                }
                catch
                {
                }

                if (assembly != null) yield return assembly;
            }
        }

        public static FixtureGraph ForAssemblies(params string[] names)
        {
            var assemblies = names.Select(Assembly.Load);
            return new FixtureGraph(assemblies);
        }

        // TODO -- memoize this
        public static FixtureGraph ForAppDomain()
        {
            var list = new List<string>() {AppDomain.CurrentDomain.SetupInformation.ApplicationBase};

            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (binPath.IsNotEmpty())
            {
                list.Add(binPath);
            }

            var assemblies = list.SelectMany(AssembliesFromPath);

            return new FixtureGraph(assemblies);
        }
    }
}