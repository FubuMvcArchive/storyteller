using System;
using System.Collections.Generic;

namespace StoryTeller.Domain
{
    public interface IStepHolder : ITestVisitable
    {
        IList<IStep> AllSteps();
    }
}