using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class ExpMultiply : ExpBase
    {
        private ExpBase _op1;
        private ExpBase _op2;

        public ExpMultiply(CodeElement elem)
        {
            CodeElement first = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault();
            CodeElement next = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault(c => c != first);
            _op1 = Factory(first);
            _op2 = Factory(next);

            // Resolve type
            if (_op1.ExpressionType != ExpType.Int && _op1.ExpressionType != ExpType.Float)
                throw new Exception(string.Format("Multiply: The left operator at '{0}' is not a number", elem.GetLineAndColumn()));
            if (_op2.ExpressionType != ExpType.Int && _op2.ExpressionType != ExpType.Float)
                throw new Exception(string.Format("Multiply: The right operator at '{0}' is not a number", elem.GetLineAndColumn()));

            if (_op1.ExpressionType == ExpType.Int && _op2.ExpressionType == ExpType.Int)
                ExpressionType = ExpType.Int;
            else
                ExpressionType = ExpType.Float;
        }

        public override float execute()
        {
            return _op1.execute() * _op2.execute();
        }
    }
}
