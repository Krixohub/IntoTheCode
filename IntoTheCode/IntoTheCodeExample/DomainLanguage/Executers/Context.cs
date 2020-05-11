using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Context
    {

        private Context _parent;

        public Context(Context parent) //Declarations
        {
            _parent = parent;
        }

        public Dictionary<string, VariableBase> Variables { get; private set; }

        public Dictionary<string, FunctionBase> Functions { get; private set; }

        public void SetVariable(string name, ExpBase exp)
        {
            switch (exp.ExpressionType)
            {
                case DefType.Int:
                    int i = ((ExpBaseTyped<int>)exp).Calculate();
                    break;
                case DefType.String:
                    break;
                case DefType.Float:
                    break;
                case DefType.Bool:
                    break;
                case DefType.Void:
                    break;
                default:
                    break;
            }
        }

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

        private VariableBase GetVariable(string name)
        {
            if (Variables.ContainsKey(name))
                return Variables[name];
            //if (_parent != null)
            //    return _parent.GetVariable(name);
            throw new Exception(string.Format("This variable is not declared '{0}'", name, elem.GetLineAndColumn()));
        }
    }
}
