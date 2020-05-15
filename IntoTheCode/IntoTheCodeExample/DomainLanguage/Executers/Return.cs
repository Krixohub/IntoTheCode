﻿using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Return : ProgramBase
    {


        public ExpBase Expression;

        public override bool Run(Context runtime)
        {
            runtime.SetVariable("return", Expression);
            return true;
        }
    }
}
