using System;
using System.Collections.Generic;
using System.Linq;

namespace StoryTeller.Domain
{
    public interface IPartHolder
    {
        IList<ITestPart> AllParts { get; }
    }

    public static class PartHolderExtensions
    {
        public static IList<IStep> AllSteps(this IPartHolder holder)
        {
            return holder.AllParts.Where(x => x is IStep).Cast<IStep>().ToList();
        }

        public static void MoveUp(this IPartHolder holder, ITestPart part)
        {
            holder.AllParts.MoveUp(part);
        }

        public static void MoveDown(this IPartHolder holder, ITestPart part)
        {
            holder.AllParts.MoveDown(part);
        }

        public static Step AddStep(this IPartHolder holder, string grammarKey)
        {
            var step = new Step(grammarKey);
            holder.AllParts.Add(step);

            return step;
        }

        public static void Remove(this IPartHolder holder, ITestPart part)
        {
            holder.AllParts.Remove(part);
        }

        public static void RemoveParts(this IPartHolder holder, Func<ITestPart, bool> filter)
        {
            holder.AllParts.RemoveAll(filter);
        }

        public static void Add(this IPartHolder holder, ITestPart part)
        {
            holder.AllParts.Add(part);
        }
    }
}