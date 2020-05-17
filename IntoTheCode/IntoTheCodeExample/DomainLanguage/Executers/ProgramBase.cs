namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ProgramBase
    {

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="resultType">If the statement is part of a function, this is the return type.</param>
        //public ProgramBase(DefType resultType)
        //{
        //    ResultType = resultType;
        //}

        ///// <summary>If the statement is part of a function, this is the return type.</summary>
        //public readonly DefType ResultType;

        /// <summary>Runtime execution of statement.</summary>
        /// <param name="runtime">The context.</param>
        /// <returns>True: the statement or body has executed a 'return' statement.</returns>
        public abstract bool Run(Variables runtime);
    }
}
