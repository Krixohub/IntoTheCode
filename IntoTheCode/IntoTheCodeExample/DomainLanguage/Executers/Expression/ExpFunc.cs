using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpFunc<TType> : ExpTyped<TType> // : ValueBase
    {
        public ExpFunc(string name, Function func, List<ExpBase> parameters)
        {
            Name = name;
            Func = func;
            Parameters = parameters;

        }

        public string Name;
        public Function Func;
        public List<ExpBase> Parameters;

        public override TType Compute(Variables runtime)
        {

            var parm = new Dictionary<string, ValueBase>();
            for (int i = 0; i < Parameters.Count; i++)
                parm.Add(Func.Parameters[i].TheName, ValueBase.Create(Func.Parameters[i].TheType, runtime, Parameters[i]));

            //if (typeof(TType) )
            parm.Add(ProgramCompiler.VariableReturn, null);

            Variables local = new Variables(runtime, parm);
            Func.Run(local);

            ValueTyped<TType> value = parm[ProgramCompiler.VariableReturn] as ValueTyped<TType>;
            return value.Value;
        }

    }
}
