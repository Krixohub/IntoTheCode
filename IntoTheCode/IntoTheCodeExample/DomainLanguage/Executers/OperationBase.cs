namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class OperationBase
    {
        /// <summary>Runtime execution of operation/statement.</summary>
        /// <param name="runtime">The context of variables.</param>
        /// <returns>True: the statement or body has executed a 'return' statement.</returns>
        public abstract bool Run(Variables runtime);
    }
}
