using IntoTheCode;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Assign : ProgramBase
    {

        public Assign(CodeElement elem) : base(DefType.Void)
        {

        }

        public override bool Run(Context runtime)
        {
            throw new NotImplementedException("lige her");
        }
    }
}
