using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Func : ProgramBase
    {
        public string Name;


        public Func(DefType resultType, CodeElement elem) : base(resultType)
        {

        }

        public override bool Run(Context runtime)
        {
            throw new System.NotImplementedException();
        }
    }
}
