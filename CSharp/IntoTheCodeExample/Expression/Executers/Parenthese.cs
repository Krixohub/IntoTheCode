﻿using IntoTheCode;
using System.Linq;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Parenthese : ExpressionBase
    {
        private ExpressionBase _op;

        public Parenthese(TextElement elem)
        {
            CodeElement first = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault();
            _op = ExpressionBuilder.BuildExp(first);
        }

        public override float execute()
        {
            return _op.execute();
        }
    }
}
