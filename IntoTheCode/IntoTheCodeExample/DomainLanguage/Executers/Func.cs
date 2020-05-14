using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Func : ProgramBase
    {
        public string Name;


        public Func(CodeElement elem, Context compileContext) : base()
        {

        }

        public override bool Run(Context runtime)
        {
            throw new System.NotImplementedException();
        }
    }
}
