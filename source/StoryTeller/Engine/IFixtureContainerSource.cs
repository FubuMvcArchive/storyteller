using System;
using StructureMap;

namespace StoryTeller.Engine
{
    public interface IFixtureContainerSource
    {
        IContainer Build();
        void RegisterFixture(string name, Type fixtureType);
    }
}