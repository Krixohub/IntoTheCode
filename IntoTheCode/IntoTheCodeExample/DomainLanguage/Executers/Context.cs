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

        public bool ExistsVariable(string name)
        {
            ValueBase variable;
            if (Variables.TryGetValue(name, out variable))
                return true;
            else if (_parent != null)
                return _parent.ExistsVariable(name);
            else 
                return false;
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
