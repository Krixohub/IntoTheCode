using IntoTheCode;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Function : ProgramBase
    {
        //public FunctionDef(string name, DefType theType, List<TypeAndId> parm)
        //{
        //    FuncType = theType;
        //    Name = name;
        //    Parameters = parm;
        //}

        public string Name;

        public DefType FuncType;

        public List<Declare> Parameters;
        
        public LocalScope FunctionScope;

        public override bool Run(Context runtime)
        {
            throw new System.NotImplementedException();
        }
    }
}
