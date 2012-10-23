using System;
using FubuCore.Conversion;
using StoryTeller.Engine;
using StructureMap;

namespace StoryTeller.Samples
{
    public class GrammarSystem : ISystem
    {
        public T Get<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
            throw new NotImplementedException();
        }

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }


        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void Teardown()
        {
            throw new NotImplementedException();
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            
        }

        public IObjectConverter BuildConverter()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }



}