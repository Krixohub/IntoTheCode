using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;
using System.Linq;
using IntoTheCode;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Read.Element.Struckture;

namespace IntoTheCode.Read.Element
{
    /// <summary>Expressions read strings like this 'a + b * - c * ( 2 + d )'. (Infix notation)
    /// The expression rule must look like this "expr = mul | sum | value".
    /// It is a list of alternatives. The first alternative must be a binary operator.
    /// At least one alternative must be something else; not recursive.
    /// The binary operator rules must have this form: "sum = expr '+' expr". Where '+' is the operator.
    /// A binary operator can also be inline: "expr = expr '*' expr | value;".
    /// The value rule is the last: "value = [sign] ( int | var | par )"
    /// The unary operator rules must have this form: "sign = '-'". Where '-' is the operator.
    /// Variables can be simple: "var = identifier"
    /// Variables can be complex eg: "var = varRule; varRule = [incrementOp] objectIdent {'.' propIdent}"
    /// Other forms (like parentheses: "par = '(' expr ')'" ).
    /// 
    /// 
    /// Precedence:
    /// The 'Other forms' has highest precedence.
    /// Then unary operators. Among unary oprators: first in expression has higher precedence as default.
    /// (Then variables and values.)
    /// Then binary operators. Among binary oprators: first in expression has higher precedence as default.
    /// 
    /// The binary and unary operator precedence can be changed by setting the 'precedence' rule-property
    /// 
    /// The binary and unary expression is read in a loop rather than in a recursive way.
    /// Other forms (parentheses) are read recursivly as a separate expression.
    /// 
    /// Output:
    /// The 'expression' node is eliminated. The list of values and operators are ordered in 
    /// a tree of nodes with values as leafs and operators with subnotes. The order follows precedence rules
    /// and association rules.
    /// 
    /// The expression class is not just repressenting a node in the syntax tree. It represents a part of the tree.
    /// Thus normal recursiveness is not followed in this part of the tree for some functions.
    /// </summary>
    internal class Expression : Or
    {
        internal List<ParserElementBase> _otherForms = new List<ParserElementBase>();
        //private List<ParserElementBase> _unaryOperators = new List<ParserElementBase>();
        internal List<WordBinaryOperator> _binaryOperators = new List<WordBinaryOperator>();
        //private List<ParserElementBase> _variables = new List<ParserElementBase>();
        //private List<ParserElementBase> _values = new List<ParserElementBase>();

        /// <summary>Creator for <see cref="Expression"/>.</summary>
        internal Expression(Rule ExprRule, Or or) : 
            base ((ParserElementBase)or.SubElements[0], (ParserElementBase)or.SubElements[1]) 
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
        }

        private void AddAlternatives(Rule ExprRule, ParserElementBase alternative)
        {
            // is it more alternatives?
            var or = alternative as Or;
            if (or != null)
            {
                AddAlternatives(ExprRule, or.SubElements[0] as ParserElementBase);
                AddAlternatives(ExprRule, or.SubElements[1] as ParserElementBase);
                return;
            }

            // is it a binary operator? (depends opun how Or is implemented)
            WordSymbol symbol = null;
            Rule rule = null;
            if (IsBinaryAlternative(ExprRule, alternative, out symbol, out rule))
                _binaryOperators.Add(new WordBinaryOperator(symbol.Value, rule != null ? rule.Name : symbol.Value, TextBuffer));
            
            // Other forms
            else 
                _otherForms.Add(alternative);
        }

        internal static bool IsBinaryAlternative(Rule ExprRule, ParserElementBase alternative, out WordSymbol symbol, out Rule rule)
        {
            // is it a binary operator
            // find symbol for 'rule' binary operators
            ParserElementBase binaryOperation = null;
            if (alternative is RuleLink)
            {
                rule = ((RuleLink)alternative).RuleElement;
                binaryOperation = rule;
            }
            else
                rule = null;

            // find symbol for inline binary operators
            if (alternative is Parentheses)
                binaryOperation = alternative;

            // Is it a binary operation ?
            if (binaryOperation != null && binaryOperation.SubElements.Count == 3)
            {
                var expre1 = binaryOperation.SubElements[0] as RuleLink;
                symbol = binaryOperation.SubElements[1] as WordSymbol;
                var expre2 = binaryOperation.SubElements[2] as RuleLink;

                // add binary operator
                if (expre1 != null && expre1.RuleElement == ExprRule &&
                    symbol != null &&
                    expre2 != null && expre2.RuleElement == ExprRule)
                    return true;
            }

            symbol = null;
            return false;
        }
        
        public override bool Load(List<CodeElement> outElements, int level)
        {
            int from = TextBuffer.PointerNextChar;

            // Read a value first
            if (!LoadValue(outElements, level))
                // if the expression does'nt start with a value; the intire expression fails.
                return SetPointerBack(from, this);

            // Read following operations as alternately binary operators and values.
            var operations = new List<CodeElement>();
            from = TextBuffer.PointerNextChar;
            while (LoadBinaryOperator(operations, level) && LoadValue(operations, level))
                from = TextBuffer.PointerNextChar;

            // Set pointer back
            TextBuffer.PointerNextChar = from;

            // If only one value the return that.
            if (operations.Count == 0)
                return true;

            // Order elements in a tree according to precedence and association rules.
            // set first value under a dummy parent
            CodeElement parent = new CodeElement(this, null);
            parent.AddElement(outElements[0]);

            int nextValueIndex = 0;
            while (nextValueIndex < operations.Count - 1)
                AddOperationToTree(parent.SubElements[0], operations[nextValueIndex++], operations[nextValueIndex++]);

            outElements[0] = parent.SubElements[0] as CodeElement;
            return true;
        }

        private void AddOperationToTree(TreeNode leftExprCode, CodeElement rightOpCode, CodeElement rightExprCode)
        {
            // todo check for operators belonging to expression
            WordBinaryOperator leftOperator = ((CodeElement)leftExprCode).WordParser as WordBinaryOperator;
            WordBinaryOperator rightOpSymbol = rightOpCode.WordParser as WordBinaryOperator;

            // is the insertpoint a value
            //            if (!IsBinaryOperator(leftExprCode))
            if (leftOperator == null)
            {
                // insert 
                TreeNode parent = leftExprCode.Parent;
                rightOpCode.AddElement(leftExprCode);
                rightOpCode.AddElement(rightExprCode);
                parent.ReplaceSubElement(leftExprCode, rightOpCode);
                return;
            }

            // if the insertPoint has higther precedence
            if (leftOperator.Precedence > rightOpSymbol.Precedence)
            {
                // insert 
                TreeNode parent = leftExprCode.Parent;
                rightOpCode.AddElement(leftExprCode);
                rightOpCode.AddElement(rightExprCode);
                parent.ReplaceSubElement(leftExprCode, rightOpCode);
                return;
            }

            // if the insertPoint has lower precedence
            if (leftOperator.Precedence < rightOpSymbol.Precedence)
                AddOperationToTree(leftExprCode.SubElements[1] as CodeElement, rightOpCode, rightExprCode);

            // if the insertPoint has same precedence and operator is rigth associative
            else if (rightOpSymbol.RightAssociative)
                AddOperationToTree(leftExprCode.SubElements[1] as CodeElement, rightOpCode, rightExprCode);

            // if the insertPoint has same precedence and operator is left associative
            else
            {
                // insert 
                TreeNode parent = leftExprCode.Parent;
                rightOpCode.AddElement(leftExprCode);
                rightOpCode.AddElement(rightExprCode);
                parent.ReplaceSubElement(leftExprCode, rightOpCode);
                return;
            }
        }

        /// <summary>Load a value. Inclusive const, variables and unary operators.</summary>
        /// <param name="outElements">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool LoadValue(List<CodeElement> outElements, int level)
        {
            // Read a value 
            foreach (var item in _otherForms)
                if (item.Load(outElements, level + 1))
                    return true;
            return false;
        }

        /// <summary>Load a binary operator.</summary>
        /// <param name="outElements">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool LoadBinaryOperator(List<CodeElement> outElements, int level)
        {
            // Read a value 
            foreach (var item in _binaryOperators)
                if (item.Load(outElements, level + 1))
                    return true;
            return false;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            // todo fra OR
            int rc = (SubElements[0] as ParserElementBase).LoadFindLast(last);
            if (rc < 2)
                rc = (SubElements[1] as ParserElementBase).LoadFindLast(last);

            return rc;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            // todo fra OR
            bool ok = (SubElements[0] as ParserElementBase).LoadTrackError(ref wordCount);
            ok = ok || (SubElements[1] as ParserElementBase).LoadTrackError(ref wordCount);

            return ok;
        }
    }
}