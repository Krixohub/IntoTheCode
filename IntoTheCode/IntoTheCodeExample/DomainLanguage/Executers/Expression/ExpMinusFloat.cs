﻿using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMinusFloat : ExpTyped<float>
    {
        private Func<Variables, float> _run;

        public ExpMinusFloat(ExpBase op1, ExpBase op2)
        {
            _run = (runtime) => op1.RunAsFloat(runtime) - op2.RunAsFloat(runtime);
        }

        public override float Run(Variables runtime)
        {
            return _run(runtime);
        }
    }
}
