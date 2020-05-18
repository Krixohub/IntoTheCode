using IntoTheCode;
using System;
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

        public Scope FunctionScope;

        /// <summary>
        /// For functions added to the program.
        /// </summary>
        public Action<String> ExternalFunction;

        public override bool Run(Variables runtime)
        {
            //if (ExternalFunction != null)
            //{
            //    string str = runtime.Vars[Parameters[0].TheName].
            //}
            throw new System.NotImplementedException();
        }
    }
}
