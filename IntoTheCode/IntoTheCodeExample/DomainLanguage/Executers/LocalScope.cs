using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class LocalScope : ProgramBase
    {


        public LocalScope(LocalScope parentScope, Dictionary<string, Function> functions = null)
        {
            ParentScope = parentScope;
            Functions = functions == null ? new Dictionary<string, Function>() : functions ;
        }

        public List<ProgramBase> Commands;
        public LocalScope ParentScope;
        public Dictionary<string, Function> Functions { get; private set; }

        public override bool Run(Context runtime)
        {
            foreach (ProgramBase item in Commands)
                if (item.Run(runtime)) return true;
            
            return false;
        }

        #region Function context

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
