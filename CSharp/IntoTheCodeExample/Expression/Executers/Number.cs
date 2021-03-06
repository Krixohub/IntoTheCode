﻿using IntoTheCode;
using IntoTheCode.Basic;
using IntoTheCode.Read.Words;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Number : ExpressionBase
    {
        private int _value;

        public Number(TextElement elem)
        {
            int.TryParse(elem.Value, out _value);
        }

        public override float execute()
        {
            return _value;
        }
    }
}
