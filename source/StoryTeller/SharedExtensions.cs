using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using StoryTeller.Domain;
using StoryTeller.Persistence;

namespace StoryTeller
{
    public static class SharedExtensions
    {
        public static IList<IStep> AllSteps(this IEnumerable<ITestPart> parts)
        {
            return parts.Where(x => x is IStep).Select(x => (IStep) x).ToList();
        }

        private static Func<T, bool> finderWithin<T, U>(Predicate<U> predicate) where U : class
        {
            return x =>
            {
                var u = x as U;
                return u == null ? false : predicate(u);
            };
        }

        public static IEnumerable<T> WhereMatching<T, U>(this IEnumerable<T> enumerable, Predicate<U> predicate)
            where U : class
            where T : class
        {
            Func<T, bool> finder = finderWithin<T, U>(predicate);
            return enumerable.Where(finder);
        }

        public static U First<T, U>(this IEnumerable<T> enumerable, Predicate<U> predicate)
            where U : class
            where T : class
        {
            Func<T, bool> finder = finderWithin<T, U>(predicate);
            return enumerable.First(finder) as U;
        }

        public static T As<T>(this object target)
        {
            return (T) target;
        }

        public static T Parse<T>(this string value)
        {
            return (T) Convert.ChangeType(value, typeof (T));
        }

        public static bool IsTrue(this string value)
        {
            return bool.Parse(value);
        }

        public static bool IsTrue(this object value)
        {
            return value is bool ? (bool) value : value.ToString().IsTrue();
        }

        [Obsolete("HOAKUM")]
        public static bool IsSuite(this object target)
        {
            return target == null ? false : target.GetType() == typeof (Suite) || target.GetType() == typeof(WorkspaceSuite);
        }

        public static int ToInt(this string stringValue)
        {
            return int.Parse(stringValue);
        }

        public static bool IsSame(this object target, object other)
        {
            return ReferenceEquals(target, other);
        }

        public static void MoveUp<T>(this IList<T> list, T subject)
        {
            int index = list.IndexOf(subject);
            if (index == 0) return;

            list.Remove(subject);
            list.Insert(index - 1, subject);
        }

        public static void MoveDown<T>(this IList<T> list, T subject)
        {
            if (ReferenceEquals(subject, list.LastOrDefault())) return;

            int index = list.IndexOf(subject);
            list.Remove(subject);
            list.Insert(index + 1, subject);
        }

        public static INode WithProperties(this INode node, Cache<string, string> values)
        {
            values.Each((key, val) => node[key] = val);
            return node;
        }
    }
}