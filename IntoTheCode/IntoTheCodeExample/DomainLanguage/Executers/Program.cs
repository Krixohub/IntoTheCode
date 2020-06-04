using System;
using System.Collections.Generic;
using System.Globalization;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Program : OperationBase
    {

        public static Dictionary<string, Function> AddFunction(Dictionary<string, Function> functions, string name, Action<string> fun, params string[] parmName)
        {
            if (functions == null) functions = new Dictionary<string, Function>();
            
            var function = new Function() { Name = name, ExternalFunction = fun, Parameters = new List<Declare>() };
            function.Parameters.Add(new Declare { TheName = parmName[0], TheType = DefType.String });
            functions.Add(name, function);

            return functions;
        }

        public static Dictionary<string, ValueBase> AddParameter(Dictionary<string, ValueBase> parameters, string name, int value)
        {
            if (parameters == null) parameters = new Dictionary<string, ValueBase>();
            // todo add parameter;

            return parameters;
        }

        public Scope RootScope;

        public static int MaxIterations { get; internal set; }
        public static CultureInfo Culture { get; internal set; }

        public void RunProgram(Dictionary<string, ValueBase> parameters)
        {
            MaxIterations = 5;
            Culture = new CultureInfo("en-US");

            Variables vars = new Variables(null, parameters);
            Run(vars);
        }

        /// <summary>Runtime execution of statement.</summary>
        /// <param name="runtime">The context.</param>
        /// <returns>True: the statement or body has executed a 'return' statement.</returns>
        public override bool Run(Variables runtime)
        {
            RootScope.Run(runtime);

            return false;
        }
    }
}
