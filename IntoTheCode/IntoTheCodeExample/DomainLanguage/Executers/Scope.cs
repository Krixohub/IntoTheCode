using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Scope : ProgramBase
    {
        public Scope(Scope parentScope, Variables vars, Dictionary<string, Function> functions = null)
        {
            ParentScope = parentScope;
            Functions = functions == null ? new Dictionary<string, Function>() : functions ;

            Vars = vars == null ? new Variables(null, null) : vars;
        }

        public List<ProgramBase> Commands;
        public Scope ParentScope;
        public Variables Vars { get; private set; }
        public Dictionary<string, Function> Functions { get; private set; }

        public override bool Run(Variables runtime)
        {
            foreach (ProgramBase item in Commands)
                if (item.Run(runtime)) return true;
            
            return false;
        }

        #region Compiletime resolving Function and variables

        public DefType ExistsVariable(string name, CodeElement elem)
        {
            ValueBase variable;
            if (Vars.Vars.TryGetValue(name, out variable))
                return variable.ResultType;
            else if (ParentScope != null)
                return ParentScope.ExistsVariable(name, elem);
            else
                throw new Exception(string.Format("A variable called '{0}', {1}, does not exists", name, elem.GetLineAndColumn()));
        }

        public bool ExistsFunction(string name)
        {
            return Functions.ContainsKey(name) || (ParentScope != null && ParentScope.Functions.ContainsKey(name));
        }

        private Function GetFunction(string name, List<DefType> types)
        {
            Function function;
            if (Functions.TryGetValue(name, out function))
                // todo check type and parameters
                return function;
            else if (ParentScope != null)
                return ParentScope.GetFunction(name, types);
            else
                throw new Exception(string.Format("This function is not declared '{0}'", name));
        }

        #endregion
    }
}
