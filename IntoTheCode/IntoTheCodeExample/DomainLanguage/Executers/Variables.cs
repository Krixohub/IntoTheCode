using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Variables //Context
    {

        private Variables _runtimeParent;

        public Variables(Variables runtimeParent, Dictionary<string, ValueBase> variables) //, LocalScope functionScope)
        {
            _runtimeParent = runtimeParent;
            Vars = variables == null ? new Dictionary<string, ValueBase>() : variables;
        }

        private Dictionary<string, ValueBase> Vars { get; set; }

        public void SetVariable(string name, ExpBase exp)
        {
            ValueBase variable;
            if (Vars.TryGetValue(name, out variable))
                variable.SetValue(this, exp);
            else if (_runtimeParent != null)
                _runtimeParent.SetVariable(name, exp);
            else
                throw new Exception(string.Format("A variable called '{0}' is not declared", name));

            //return true;
//            throw new Exception(string.Format("A variable called '{0}', {1}, is not declared", name, elem.GetLineAndColumn()));
        }

        public void BuildVariable(DefType theType, string name, CodeElement elem)
        {
            if (Vars.ContainsKey(name))
                throw new Exception(string.Format("A variable called '{0}', {1}, is allready declared", name, elem.GetLineAndColumn()));

            ValueBase variable = ValueBase.Compile(theType, this, null);
            Vars.Add(name, variable);
        }

        public void DeclareVariable(DefType theType, string name, ExpBase exp)
        {
            ValueBase variable = ValueBase.Create(theType, this, exp);
            Vars.Add(name, variable);
        }

        public bool ExistsVariable(string name)
        {
            return Vars.ContainsKey(name);
            //    return variable;
            //else if (_runtimeParent != null)
            //    return _runtimeParent.GetVariable(name);
            //else

        }

        public ValueBase GetVariable(string name)
        {
            ValueBase variable;
            if (Vars.TryGetValue(name, out variable))
                return variable;
            else if (_runtimeParent != null)
                return _runtimeParent.GetVariable(name);
            else
                throw new Exception(string.Format("This variable is not declared '{0}'", name));
        }
    }
}
