using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class ExpMinusInt : ExpTyped<int>
    {
        private ExpMinusInt _op1;
        private ExpMinusInt _op2;

        public ExpMinusInt(CodeElement elem, ExpBase op1, ExpBase op2)
        {
            _op1 = (ExpMinusInt)op1;
            _op2 = (ExpMinusInt)op2;

            // Resolve type
            if (_op1 == null)
                throw new Exception(string.Format("Minus: The left operator at '{0}' is not an int", elem.GetLineAndColumn()));
            if (_op2 == null)
                throw new Exception(string.Format("Minus: The right operator at '{0}' is not an int", elem.GetLineAndColumn()));

            ExpressionType = ExpType.Int;
        }

        public override ValueBase execute()
        {
            throw new NotImplementedException();
        }

        public override int execute2()
        {
            return _op1.execute2() - _op2.execute2();
        }
    }

    public class ExpMinusFloat : ExpTyped<float>
    {
        private ExpBase _op1;
        private ExpBase _op2;

        public ExpMinusFloat(CodeElement elem, ExpBase op1, ExpBase op2)
        {
            _op1 = op1;
            _op2 = op2;

            // Resolve type
            if (_op1.ExpressionType != ExpType.Int && _op1.ExpressionType != ExpType.Float)
                throw new Exception(string.Format("Minus: The left operator at '{0}' is not a number", elem.GetLineAndColumn()));
            if (_op2.ExpressionType != ExpType.Int && _op2.ExpressionType != ExpType.Float)
                throw new Exception(string.Format("Minus: The right operator at '{0}' is not a number", elem.GetLineAndColumn()));

            ExpressionType = ExpType.Float;
        }

        public override ValueBase execute()
        {
            throw new NotImplementedException();
        }

        public override float execute2()
        {
            float f1;
            if (_op1 is ExpMinusInt) f1 = ((ExpMinusInt)_op1).execute2();
            else if (_op1 is ExpMinusFloat) f1 = ((ExpMinusFloat)_op1).execute2();
            else throw new Exception("The value must be a number");

            float f2;
            if (_op2 is ExpMinusInt) f2 = ((ExpMinusInt)_op2).execute2();
            else if (_op2 is ExpMinusFloat) f2 = ((ExpMinusFloat)_op2).execute2();
            else throw new Exception("The value must be a number");

            return f1 - f2;
        }
    }

    public class ExpMinus : ExpBase
    {
        private ExpBase _op1;
        private ExpBase _op2;

        public ExpMinus(CodeElement elem)
        {
            CodeElement first = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault();
            CodeElement next = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault(c => c != first);
            _op1 = Factory(first);
            _op2 = Factory(next);

            // Resolve type
            if (_op1.ExpressionType != ExpType.Int && _op1.ExpressionType != ExpType.Float)
                throw new Exception(string.Format("Minus: The left operator at '{0}' is not a number", elem.GetLineAndColumn()));
            if (_op2.ExpressionType != ExpType.Int && _op2.ExpressionType != ExpType.Float)
                throw new Exception(string.Format("Minus: The right operator at '{0}' is not a number", elem.GetLineAndColumn()));

            if (_op1.ExpressionType == ExpType.Int && _op2.ExpressionType == ExpType.Int)
                ExpressionType = ExpType.Int;
            else
                ExpressionType = ExpType.Float;
        }

        public override ValueBase execute()
        {
            if (ExpressionType == ExpType.Int)
                return new ValueInt(((ValueInt)_op1.execute()).Value - ((ValueInt)_op2.execute()).Value);
            float f1 = _op1.execute().ToFloat();
            float f2 = _op1.execute().ToFloat();
            return new ValueFloat(f1 - f2);
        }
    }
}
