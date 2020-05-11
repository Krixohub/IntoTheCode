using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Declarations
    {


        public Declarations()
        {
            Variables = new Dictionary<string, VariableBase>();
            Functions = new Dictionary<string, FunctionBase>();
        }

        public Dictionary<string, VariableBase> Variables { get; private set; }

        public Dictionary<string, FunctionBase> Functions { get; private set; }

        public void DeclareVariable(CodeElement elem, string name, VariableBase var)
        {
            if (Variables.ContainsKey(name))
                throw new Exception(string.Format("A variable called '{0}', {1}, is allready declared", name, elem.GetLineAndColumn()));
            if (Functions.ContainsKey(name))
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", name, elem.GetLineAndColumn()));

            Variables.Add(name, var);
        }

        public void AddVariables(Dictionary<string, VariableBase> variables)
        {
            foreach (KeyValuePair<string, VariableBase> item in variables)
                if (!Variables.ContainsKey(item.Key))
                    Variables.Add(item.Key, item.Value);
        }

        public void DeclareFunction(CodeElement elem, string name, FunctionBase fun)
        {
            if (Variables.ContainsKey(name))
                throw new Exception(string.Format("A variable called '{0}', {1}, is allready declared", name, elem.GetLineAndColumn()));
            if (Functions.ContainsKey(name))
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", name, elem.GetLineAndColumn()));

            Functions.Add(name, fun);
        }
    }
}
