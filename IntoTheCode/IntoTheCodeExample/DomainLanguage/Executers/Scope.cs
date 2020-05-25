using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Scope : OperationBase
    {
        private bool _createRuntimeVariables;
        public Scope(Scope parentScope, Variables vars, Dictionary<string, Function> functions = null)
        {
            ParentScope = parentScope;
            Functions = functions == null ? new Dictionary<string, Function>() : functions ;

            _createRuntimeVariables = vars == null;
            Vars = vars == null ? new Variables(null, null) : vars;
        }

        public List<OperationBase> Operations;
        public Scope ParentScope;
        public Variables Vars { get; private set; }
        public Dictionary<string, Function> Functions { get; private set; }

        public override bool Run(Variables runtime)
        {
            // It isn't nessesary to create a nested scope of variables if this scope is inside a 'Function'.
            Variables localRuntime = _createRuntimeVariables ? new Variables(runtime, null) : runtime;
            foreach (OperationBase item in Operations)
                if (item.Run(localRuntime)) return true;
            
            return false;
        }

        #region Compiletime resolving Function and variables

        public DefType ExistsVariable(string name, CodeElement elem)
        {
            if (Vars.ExistsVariable(name))
                return Vars.GetVariable(name).ResultType;
            else if (ParentScope != null)
                return ParentScope.ExistsVariable(name, elem);
            else
                throw new Exception(string.Format("A variable called '{0}', {1}, does not exists", name, elem.GetLineAndColumn()));
        }

        public bool ExistsFunction(string name)
        {
            return Functions.ContainsKey(name) || (ParentScope != null && ParentScope.ExistsFunction(name));
        }

        public Function GetFunction(string name)
        {
            Function function;
            if (Functions.TryGetValue(name, out function))
                // todo check type and parameters
                return function;
            else if (ParentScope != null)
                return ParentScope.GetFunction(name);
            else
                throw new Exception(string.Format("This function is not declared '{0}'", name));
        }

        #endregion
    }
}
