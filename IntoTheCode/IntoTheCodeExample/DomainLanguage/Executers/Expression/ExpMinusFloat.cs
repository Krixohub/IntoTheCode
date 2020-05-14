﻿using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMinusFloat : ExpTyped<float>
    {
        private Func<Context, float> _run;

        public ExpMinusFloat(ExpBase op1, ExpBase op2)
        {
            _run = (runtime) => op1.RunAsFloat(runtime) - op2.RunAsFloat(runtime);
        }

        public override float Run(Context runtime)
        {
            return _run(runtime);
        }
    }
}
