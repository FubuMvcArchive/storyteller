using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StoryTeller.Engine;
using StoryTeller.Engine.Constraints;
using StoryTeller.Model;

namespace StoryTeller
{
    public interface IFixture
    {
        string Name { get; }
        string Description { get; }

        [IndexerName("Grammar")]
        IGrammar this[string key] { get; set; }

        IEnumerable<GrammarError> Errors { get; }
        IPolicies Policies { get; }
        string Title { get; set; }

        void ForEachGrammar(Action<string, IGrammar> action);

        void SetUp(ITestContext context);
        void TearDown();
    }
}