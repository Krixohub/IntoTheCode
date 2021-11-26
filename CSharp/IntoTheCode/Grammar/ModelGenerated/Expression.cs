using System.Collections.Generic;

using IntoTheCode.Buffer;
using System;
using System.Linq;
using IntoTheCode.Grammar;
using IntoTheCode.Message;

namespace IntoTheCode.Grammar
{
    /// <summary>Expressions read strings like this 'a + b * - c * ( 2 + d )' (Infix notation).
    /// The expression rule must look like this "expr = mul | sum | value".
    /// Or like this (inline) : "expr = expr '*' expr | value;".
    /// It is a list of alternatives. The first alternative must be a binary operator.
    /// At least one alternative must be something else; not recursive.
    /// The binary operator rules must have this form: "sum = expr '+' expr". Where '+' is the operator.
    /// 
    /// The default precedence are determined by the order of the operators in the expression rule. 
    /// First operator has highest precedence. The operator precedence can be changed by setting the 'precedence' property.
    /// Operators are default left associative. If an operator is right associative set the 'RightAssociative' property.
    /// 
    /// Output:
    /// The output tree is nested operator elements; An operator has to values elements. 
    /// A value elements can be a new operator element or one of the other alternatives.
    /// </summary>
    /// <remarks>Inherids <see cref="Or"/></remarks>
    internal partial class Expression
    {
        internal List<ParserElementBase> _otherForms = new List<ParserElementBase>();
        //private List<ParserElementBase> _unaryOperators = new List<ParserElementBase>();
        internal List<WordBinaryOperator> _binaryOperators = new List<WordBinaryOperator>();
        //private List<ParserElementBase> _variables = new List<ParserElementBase>();
        //private List<ParserElementBase> _values = new List<ParserElementBase>();

        internal List<CodeElement> _completeOperations = new List<CodeElement>();

        /// <summary>Create the Expression from the alternatives in an Or object.</summary>
        internal Expression(Rule ExprRule, Or or) :
            base(or.ChildNodes[0], or.ChildNodes[1])
        {
            // The elements in the base class are only used for getting the grammar and cloning.

            // the elements are split into alternatives.
            // Each alternative is put in the _binaryOperators list or the _otherForms list.

            // A binary operator is reconized by the form "Expression Symbol Expression" 
            // where the symbol i the operator. The form can be inline or have its ovn rule.
            // The operator precedence is given by the order of operators. If two operators 
            // have the same precedence, there must be rules and the "Precedence" property 
            // of the rules set to the same.
            TextBuffer = or.TextBuffer;
            AddAlternatives(ExprRule, or);
            SetPrecedence();
        }

    }
}