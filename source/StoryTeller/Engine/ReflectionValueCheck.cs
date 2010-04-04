using System.Collections.Generic;
using System.Reflection;
using StoryTeller.Engine.Reflection;

namespace StoryTeller.Engine
{
    public class ReflectionValueCheck : ReflectionGrammar
    {
        private readonly List<Cell> _cells;

        public ReflectionValueCheck(MethodInfo method, object target)
            : base(method, target)
        {
            _cells = new List<Cell>(_method.GetArgumentCells())
            {
                _method.GetReturnCell()
            };
        }

        public override IList<Cell> GetCells()
        {
            return _cells;
        }
    }
}