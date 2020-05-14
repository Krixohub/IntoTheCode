using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Context
    {

        private Context _parent;

        public Context(Context parent)
        {
            _parent = parent;
        }

        public Dictionary<string, ValueBase> Variables { get; private set; }

        public LocalScope FunctionScope { get; set; }

        public void SetVariable(string name, ExpBase exp)
        {
            ValueBase variable;
            if (Variables.TryGetValue(name, out variable))
                variable.SetValue(this, exp);
            else if (_parent != null)
                _parent.SetVariable(name, exp);
            else
                throw new Exception(string.Format("A variable called '{0}' is not declared", name));

            //return true;
//            throw new Exception(string.Format("A variable called '{0}', {1}, is not declared", name, elem.GetLineAndColumn()));
        }

        public void DeclareVariable(DefType theType, string name, CodeElement elem)
        {
            if (Variables.ContainsKey(name))
                throw new Exception(string.Format("A variable called '{0}', {1}, is allready declared", name, elem.GetLineAndColumn()));

            ValueBase variable = ValueBase.Create(theType, null, null);
            Variables.Add(name, variable);
        }

        public void DeclareVariable(DefType theType, string name, ExpBase exp)
        {
            ValueBase variable = ValueBase.Create(theType, this, exp);
            Variables.Add(name, variable);
        }

        public DefType ExistsVariable(string name, CodeElement elem)
        {
            ValueBase variable;
            if (Variables.TryGetValue(name, out variable))
                return variable.ResultType;
            else if (_parent != null)
                return _parent.ExistsVariable(name, elem);
            else
                throw new Exception(string.Format("A variable called '{0}', {1}, does not exists", name, elem.GetLineAndColumn()));
        }

        private ValueBase GetVariable(string name)
        {
            ValueBase variable;
            if (Variables.TryGetValue(name, out variable))
                return variable;
            else if (_parent != null)
                return _parent.GetVariable(name);
            else
                throw new Exception(string.Format("This variable is not declared '{0}'", name));
        }
    }
}
