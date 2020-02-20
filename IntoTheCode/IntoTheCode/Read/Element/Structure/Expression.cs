using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;
using System.Linq;
using IntoTheCode;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element.Struckture
{
    /// <summary>Expressions read strings like this 'a + b * - c * ( 2 + d )'. (Infix notation)
    /// The expression rule must look like this "expr = mul | sum | value".
    /// The binary operator rules must have this form: "sum = expr '+' expr". Where '+' is the operator.
    /// The value rule is the last: "value = [sign] ( int | var | par )"
    /// The unary operator rules must have this form: "sign = '-'". Where '-' is the operator.
    /// Variables can be simple: "var = identifier"
    /// Variables can be complex eg: "var = varRule; varRule = [incrementOp] objectIdent {'.' propIdent}"
    /// Other forms (like parentheses: "par = '(' expr ')'" ).
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
    /// </summary>
    internal class Expression : ParserElementBase
    {
        //private List<ParserElementBase> _otherForms = new List<ParserElementBase>();
        //private List<ParserElementBase> _unaryOperators = new List<ParserElementBase>();
        private List<ParserElementBase> _binaryOperators = new List<ParserElementBase>();
        //private List<ParserElementBase> _variables = new List<ParserElementBase>();
        private List<ParserElementBase> _values = new List<ParserElementBase>();

        /// <summary>Creator for <see cref="Expression"/>.</summary>
        internal Expression(params ParserElementBase[] elements) 
        {
            // todo fra OR

            // find The alternatives of the expression and add to subElements.
            //AddElement(element1);

            //AddElement(element2);
            //if (Element1.ElementContent == ElementContentType.OneValue && Element2.ElementContent == ElementContentType.OneValue)
            //    ElementContent = ElementContentType.OneValue;
            //else
            //    ElementContent = ElementContentType.Many;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            // todo fra OR
            var element = new Or(((ParserElementBase)SubElements[0]).CloneForParse(buffer),
                ((ParserElementBase)SubElements[1]).CloneForParse(buffer));
            element.TextBuffer = buffer;
            return element;
        }

        public override ElementContentType GetElementContent()
        {
            // todo fra OR
            return
                //Element1.ElementContent == ElementContentType.OneValue &&
                //Element2.ElementContent == ElementContentType.OneValue ?
                //ElementContentType.OneValue :
                ElementContentType.Many;
        }

        public override string GetGrammar() {
            // todo fra OR
            return (SubElements[0] as ParserElementBase).GetGrammar() + " | " +
                (SubElements[1] as ParserElementBase).GetGrammar();
        }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements, int level)
        {
            int from = TextBuffer.PointerNextChar;

            // Read a value first
            if (!LoadValue(outElements, level))
                // if the expression does'nt start with a value; the intire expression fails.
                return SetPointerBack(from, this);

            // Read following operations as alternately binary operators and values.
            List<TreeNode> followingOperations = new List<TreeNode>();
            from = TextBuffer.PointerNextChar;
            while (LoadBinaryOperator(followingOperations, level) && LoadValue(followingOperations, level))
                from = TextBuffer.PointerNextChar;

            // Set pointer back
            TextBuffer.PointerNextChar = from;

            // If only one value the return that.
            if (followingOperations.Count == 0)
                return true;

            // Order elements in a tree according to precedence and association rules.
            // set first value under first (binary) operator
            TreeNode rootOperator = followingOperations[0];
            rootOperator.AddElement(outElements[0]);
            rootOperator.AddElement(followingOperations[1]);
            outElements[0] = followingOperations[0];

            int nextValueIndex = 3;
            while (nextValueIndex < followingOperations.Count)
            {
                AddOperationToTree(ref rootOperator, followingOperations[nextValueIndex - 1], followingOperations[nextValueIndex]);
                nextValueIndex += 2;
            }

            outElements[0] = rootOperator;
            return true;
        }

        private void AddOperationToTree(ref TreeNode operationTree, TreeNode newoperator, TreeNode value)
        {
            // todo check for operators belonging to expression
            WordSymbol leftOperator = ((CodeElement)operationTree).WordParser as WordSymbol;
            WordSymbol rightOperator = ((CodeElement)newoperator).WordParser as WordSymbol;
            if (leftOperator.Precedence < rightOperator.Precedence ||
                (leftOperator.Precedence == rightOperator.Precedence &&
                rightOperator.RightAssociative))
            {
                // insert new operator to the right
                if (IsOperator(leftOperator.SubElements[1]))
                {
                    TreeNode rightNode = leftOperator.SubElements[1];
                    AddOperationToTree(ref rightNode, newoperator, value);
                    leftOperator.SubElements[1] = rightNode;
                }
                else
                {
                    newoperator.AddElement(operationTree.SubElements[1]);
                    newoperator.AddElement(value);
                    operationTree.SubElements[1] = newoperator;
                }
            }
            else
            {
                // insert new operator to the left
                if (IsOperator(leftOperator.SubElements[0]))
                {
                    // todo check next level
                    //TreeNode leftNode = leftOperator.SubElements[0];
                    //AddOperationToTree(ref leftNode, newoperator, value);
                    //leftOperator.SubElements[1] = leftNode;
                }
                else
                {
                    newoperator.AddElement(operationTree);
                    newoperator.AddElement(value);
                    operationTree = newoperator;
                }
            }
        }

        private bool IsOperator(TreeNode treeNode)
        {
            throw new NotImplementedException();
        }

        /// <summary>Load a value. Inclusive const, variables and unary operators.</summary>
        /// <param name="outElements">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool LoadValue(List<TreeNode> outElements, int level)
        {
            // Read a value 
            foreach (var item in _values)
                if (item.Load(outElements, level + 1))
                    return true;
            return false;
        }

        /// <summary>Load a binary operator.</summary>
        /// <param name="outElements">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool LoadBinaryOperator(List<TreeNode> outElements, int level)
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