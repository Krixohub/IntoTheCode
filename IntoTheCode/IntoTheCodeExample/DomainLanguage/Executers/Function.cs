using IntoTheCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Function : OperationBase
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
            if (ExternalFunction == null)
                FunctionScope.Run(runtime);
            else
            {

                ValueBase parm0 = runtime.GetVariable(Parameters[0].TheName);
                var parm0s = parm0 as ValueTyped<string>;
                if (parm0s == null)
                    throw new Exception(string.Format("The parameter '{0}' for function '{1}' is not a string", Parameters[0].TheName, Name));

                ExternalFunction(parm0s.Value);
            }
            return false;
        }
    }
}
