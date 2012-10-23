using System;
using System.Reflection;
using FubuCore.Conversion;
using StructureMap;

namespace StoryTeller.Engine
{
    public class NulloSystem : ISystem
    {
        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}