using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class FunctionContext
    {

        private FunctionContext _parent;

        public FunctionContext(FunctionContext parent) //Declarations
        {
            _parent = parent;
        }

        public Dictionary<string, FunctionDef> Functions { get; private set; }

        public void DeclareFunction(FunctionDef func, CodeElement elem)
        {
            if (Functions.ContainsKey(func.Name))
                // todo check type and parameters
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", func.Name, elem.GetLineAndColumn()));

            Functions.Add(func.Name, func);
        }

        public bool ExistsFunction(string name)
        {
            return Functions.ContainsKey(name);
        }

        private FunctionDef GetFunction(string name, List<DefType> types)
        {
            FunctionDef function;
            if (Functions.TryGetValue(name, out function))
                // todo check type and parameters
                return function;
            else if (_parent != null)
                return _parent.GetFunction(name, types);
            else
                throw new Exception(string.Format("This function is not declared '{0}'", name));
        }
    }
}
