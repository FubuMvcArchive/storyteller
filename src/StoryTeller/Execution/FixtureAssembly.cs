using System;
using System.Reflection;
using StoryTeller.Engine;
using StoryTeller.Workspace;
using FubuCore;

namespace StoryTeller.Execution
{
    [Serializable]
    public class FixtureAssembly
    {
        private string _systemTypeName;
        private string _fixtureAssembly;
        private bool _hasFound = false;

        [NonSerialized] private Assembly _assembly;
        [NonSerialized] private ISystem _system;

        // For serialization
        public FixtureAssembly(){}

        public FixtureAssembly(string systemTypeName, string fixtureAssembly)
        {
            _systemTypeName = systemTypeName;
            _fixtureAssembly = fixtureAssembly;
        }

        public FixtureAssembly(IProject project)
        {
            _systemTypeName = project.SystemTypeName;
            _fixtureAssembly = project.FixtureAssembly;
        }

        private void find()
        {
            if (_hasFound) return;

            if (_systemTypeName.IsEmpty())
            {
                _assembly = Assembly.Load(_fixtureAssembly);
                _system = new NulloSystem(_assembly);
            }
            else
            {
                Type type = Type.GetType(_systemTypeName);
                _system = (ISystem)Activator.CreateInstance(type);
                _assembly = type.Assembly;
            }

            _hasFound = true;
        }

        public Assembly Assembly
        {
            get
            {
                find();
                return _assembly;
            }
        }

        public ISystem System
        {
            get
            {
                find();
                return _system;
            }
        }
    }
}