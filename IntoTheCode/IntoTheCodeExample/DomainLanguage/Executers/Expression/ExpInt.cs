﻿using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpInt : ExpTyped<int>
    {
        private int _value;

        public ExpInt(CodeElement elem)
        {
            int.TryParse(elem.Value, out _value);
        }

        public override int Run(Context runtime)
        {
            return _value;
        }
    }
}
