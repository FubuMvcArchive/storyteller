using System;
using FubuCore;
using StructureMap;
using System.Collections.Generic;

namespace StoryTeller.Engine
{
    public interface IExtender
    {
        void ForEachGrammar(Action<string, IGrammar> action);
    }

    public interface IExtender<T> : IExtender where T : IFixture
    {
        
    }

    public class ExtendsFixture<T> : Fixture, IExtender<T> where T : IFixture
    {
        public sealed override void SetUp(ITestContext context)
        {
        }

        public sealed override void TearDown()
        {
        }
    }

    public class FixtureExtender : StructureMap.Interceptors.TypeInterceptor
    {
        public object Process(object target, IContext context)
        {
            var adder = typeof (GrammarAdder<>).CloseAndBuildAs<IGrammarAdder>(target.GetType());
            adder.Extend((IFixture) target, context);

            return target;
        }

        public bool MatchesType(Type type)
        {
            return type.CanBeCastTo<IFixture>() && !type.CanBeCastTo<IExtender>() ;
        }

        public interface IGrammarAdder
        {
            void Extend(IFixture fixture, IContext context);
        }

        public class GrammarAdder<T> : IGrammarAdder where T : IFixture
        {
            public void Extend(IFixture fixture, IContext context)
            {
                context.GetAllInstances<IExtender<T>>().Each(extender =>
                {
                    extender.ForEachGrammar((key, grammar) =>
                    {
                        if(fixture.HasGrammar(key)) throw new ApplicationException("Duplicate grammar key " + key);// Later, check for duplicates and throw
                        fixture[key] = grammar;
                    });
                });
            }
        }
   }
}