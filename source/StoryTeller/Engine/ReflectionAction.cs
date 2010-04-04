using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StoryTeller.Engine.Reflection;


namespace StoryTeller.Engine
{
    public class ReflectionAction : ReflectionGrammar
    {
        private readonly List<Cell> _cells;

        public ReflectionAction(MethodInfo method, object target)
            : base(method, target)
        {
            _cells = new List<Cell>(_method.GetArgumentCells());
        }

        public override IList<Cell> GetCells()
        {
            return _cells;
        }

        public static ReflectionAction Create<T>(Expression<Action<T>> expression, T target)
        {
            return new ReflectionAction(FubuCore.Reflection.ReflectionHelper.GetMethod(expression), target);
        }

        public static ReflectionAction Create<T>(Expression<Func<T, object>> expression, T target)
        {
            return new ReflectionAction(FubuCore.Reflection.ReflectionHelper.GetMethod(expression), target);
        }
    }
}