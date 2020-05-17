﻿using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Program : ProgramBase
    {

        public static Dictionary<string, Function> AddFunction(Dictionary<string, Function> functions, string name, int value)
        {
            if (functions == null) functions = new Dictionary<string, Function>();
            // todo add functions to rootScope;

            return functions;
        }

        public static Dictionary<string, ValueBase> AddParameter(Dictionary<string, ValueBase> parameters, string name, int value)
        {
            if (parameters == null) parameters = new Dictionary<string, ValueBase>();
            // todo add functions to rootScope;

            return parameters;
        }


        public Scope RootScope;

        public void RunProgram(Dictionary<string, ValueBase> parameters)
        {
            // todo add parameters to Context (test for existing names);

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