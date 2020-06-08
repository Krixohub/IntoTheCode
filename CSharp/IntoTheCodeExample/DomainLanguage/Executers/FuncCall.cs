using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class FuncCall
    {
        public FuncCall(Function func, List<ExpBase> parameters)
        {
            Func = func;
            Parameters = parameters;
        }

        public Function Func;
        public List<ExpBase> Parameters;

        public Variables InitiateVariables(Variables runtime)
        {
            var parm = new Dictionary<string, ValueBase>();
            for (int i = 0; i < Parameters.Count; i++)
                parm.Add(Func.Parameters[i].TheName, ValueBase.Create(Func.Parameters[i].TheType, runtime, Parameters[i]));

            if (Func.FuncType != DefType.Void)
                parm.Add(ProgramBuilder.VariableReturn, ValueBase.Create(Func.FuncType));

            return new Variables(runtime, parm);
        }
    }
}
