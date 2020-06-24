using System.Collections.Generic;

using IntoTheCode.Buffer;
using System;
using System.Linq;
using IntoTheCode.Read.Words;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Structure
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
    internal class Expression : Or
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

        private void AddAlternatives(Rule ExprRule, ParserElementBase alternative)
        {
            // is it more alternatives?
            var or = alternative as Or;
            if (or != null)
            {
                AddAlternatives(ExprRule, or.ChildNodes[0]);
                AddAlternatives(ExprRule, or.ChildNodes[1]);
                return;
            }

            // is it a binary operator? (depends opun how Or is implemented)
            WordSymbol symbol = null;
            Rule rule = null;
            if (IsBinaryAlternative(ExprRule, alternative, out symbol, out rule))
            {
                var wordOp = new WordBinaryOperator(symbol, rule != null ? rule.Name : symbol.Value, TextBuffer);
                _binaryOperators.Add(wordOp);
                if (rule != null)
                    rule.ReplaceSubElement(symbol, wordOp);
                else
                    Add(wordOp);
            }

            // Other forms
            else
                _otherForms.Add(alternative);
        }

        /// <summary>
        /// Set precedence for binary operators.
        /// Operators with precendence set are ordered by this.
        /// Operators without precedence are ordered according to the position in syntax.
        /// </summary>
        internal void SetPrecedence()
        {
            // list of Operators with precendence set 
            List<WordBinaryOperator> preSet = _binaryOperators.
                Where(o => o.Precedence > 0).
                OrderByDescending(o => o.Precedence).ToList();

            int preSetIndex = 0;
            int preSetPre = preSet.Count > 0 ? preSet[0].Precedence + 1 : 0;
            int loopIndex = 0;
            int newPrecedence = _binaryOperators.Count;
            while (loopIndex < _binaryOperators.Count)
            {
                if (_binaryOperators[loopIndex].Precedence == 0)
                    _binaryOperators[loopIndex].Precedence = newPrecedence;
                else
                {
                    bool allreadySet = false;
                    for (int i = 0; i < preSetIndex; i++)
                        allreadySet = allreadySet || _binaryOperators[loopIndex] == preSet[i];

                    if (allreadySet)
                    {
                        loopIndex++;
                        continue;
                    }
                    else
                    {
                        int loopPre = _binaryOperators[loopIndex].Precedence;
                        int preSetNext = preSet[preSetIndex].Precedence;
                        while (preSetIndex < preSet.Count)
                        {
                            preSetPre = preSetNext;
                            if (preSetPre < loopPre) break;
                            preSet[preSetIndex].Precedence = newPrecedence;
                            preSetIndex++;
                            preSetNext = preSet.Count > preSetIndex ? preSet[preSetIndex].Precedence : 0;
                            if (preSetIndex < preSet.Count &&
                                preSetPre > preSetNext &&
                                preSetNext >= loopPre)
                                newPrecedence--;

                        }
                    }
                }

                newPrecedence--;
                loopIndex++;
            }
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
            if (alternative is SetOfElements)
                binaryOperation = alternative;

            // Is it a binary operation ?
            if (binaryOperation != null && binaryOperation.ChildNodes.Count == 3)
            {
                var expre1 = binaryOperation.ChildNodes[0] as RuleLink;
                symbol = binaryOperation.ChildNodes[1] as WordSymbol;
                var expre2 = binaryOperation.ChildNodes[2] as RuleLink;

                // add binary operator
                if (expre1 != null && expre1.RuleElement == ExprRule &&
                    symbol != null &&
                    expre2 != null && expre2.RuleElement == ExprRule)
                    return true;
            }

            symbol = null;
            return false;
        }

        /// <summary>
        /// Get Precedence and 'RightAssociative' for operators.
        /// </summary>
        /// <param name="settings"></param>
        public override void GetSettings(List<Tuple<string, string>> settings)
        {
            foreach (WordBinaryOperator op in _binaryOperators)
            {
                if (_binaryOperators.Any(o => o != op && o.Precedence == op.Precedence))
                    settings.Add(new Tuple<string, string>(op.Name, nameof(op.Precedence) + string.Format(" = '{0}'", op.Precedence)));

                if (op.RightAssociative)
                    settings.Add(new Tuple<string, string>(op.Name, nameof(op.RightAssociative)));
            }

            foreach (ParserElementBase item in ChildNodes)
                item.GetSettings(settings);
        }

        public override bool Load(List<TextElement> outElements, int level)
        {
            int from = TextBuffer.PointerNextChar;

            // Read a value first
            if (!LoadValue(outElements, level, true))
                // if the expression does'nt start with a value; the intire expression fails.
                return SetPointerBack(from);

            TextBuffer.Status.ThisIsUnambiguous(this, outElements[outElements.Count - 1]);

            // Read following operations as alternately binary operators and values.
            var operations = new List<TextElement>();
            from = TextBuffer.PointerNextChar;
            while (LoadBinaryOperator(operations, level) && LoadValue(operations, level, false))
            {
                TextBuffer.Status.ThisIsUnambiguous(this, operations[operations.Count - 1]);
                from = TextBuffer.PointerNextChar;
            }

            // Set pointer back
            TextBuffer.PointerNextChar = from;

            // If only one value the return that.
            if (operations.Count == 0)
                return true;

            // Order elements in a tree according to precedence and association rules.
            // set first value under a dummy parent
            CodeElement parent = new CodeElement(this, null);
            parent.Add(outElements[0]);

            var comments = new List<TextElement>();
            int nextValueIndex = 0;
            while (nextValueIndex < operations.Count - 1 && operations[nextValueIndex] is CommentElement)
                comments.Add(operations[nextValueIndex++]);

            while (nextValueIndex < operations.Count - 1)
            {
                int opIndex = nextValueIndex++;

                while (nextValueIndex < operations.Count - 1 && operations[nextValueIndex] is CommentElement)
                    comments.Add(operations[nextValueIndex++]);

                int expIndex = nextValueIndex++;

                while (nextValueIndex < operations.Count && operations[nextValueIndex] is CommentElement)
                    comments.Add(operations[nextValueIndex++]);

                AddOperationToTree(parent.Codes().First(), (CodeElement)operations[opIndex], (CodeElement)operations[expIndex], comments);
                comments.Clear();
            }

            _completeOperations.Insert(0, (CodeElement)parent.ChildNodes[0]);

            outElements[0] = parent.ChildNodes[0];
            return true;
        }

        private void AddOperationToTree(CodeElement leftExprCode, CodeElement rightOpCode, CodeElement rightExprCode, List<TextElement> comments)
        {
            WordBinaryOperator leftOperator = leftExprCode.ParserElement as WordBinaryOperator;
            WordBinaryOperator rightOpSymbol = rightOpCode.ParserElement as WordBinaryOperator;

            // is the insertpoint a value
            // Or the insertPoint is a completed expression (eg parentheses)
            if (leftOperator == null || 
                (_binaryOperators.Contains(leftOperator) && _completeOperations.Contains(leftExprCode)))
            {
                // insert 
                TextElement parent = leftExprCode.Parent;
                rightOpCode.Add(leftExprCode);
                rightOpCode.Add(rightExprCode);
                parent.ReplaceSubElement(leftExprCode, rightOpCode);
                rightOpCode.AddRange(comments);
                return;
            }

            // if the insertPoint has higther precedence
            if (leftOperator.Precedence > rightOpSymbol.Precedence)
            {
                // insert 
                TextElement parent = leftExprCode.Parent;
                rightOpCode.Add(leftExprCode);
                rightOpCode.Add(rightExprCode);
                parent.ReplaceSubElement(leftExprCode, rightOpCode);
                rightOpCode.AddRange(comments);
                return;
            }

            // if the insertPoint has lower precedence
            if (leftOperator.Precedence < rightOpSymbol.Precedence)
                AddOperationToTree(leftExprCode.ChildNodes[1] as CodeElement, rightOpCode, rightExprCode, comments);

            // if the insertPoint has same precedence and operator is rigth associative
            else if (rightOpSymbol.RightAssociative)
                AddOperationToTree(leftExprCode.ChildNodes[1] as CodeElement, rightOpCode, rightExprCode, comments);

            // if the insertPoint has same precedence and operator is left associative
            else
            {
                // insert 
                TextElement parent = leftExprCode.Parent;
                rightOpCode.Add(leftExprCode);
                rightOpCode.Add(rightExprCode);
                parent.ReplaceSubElement(leftExprCode, rightOpCode);
                rightOpCode.AddRange(comments);
                return;
            }
        }

        /// <summary>Load a value. Inclusive const, variables and unary operators.</summary>
        /// <param name="operations">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool LoadValue(List<TextElement> operations, int level, bool first)
        {
            if (!first)
                TextBuffer.Status.ThisIsUnambiguous(this, operations[operations.Count - 1]);

            // Read a value 
            foreach (var item in _otherForms)
                if (item.Load(operations, level))
                    return true;

            if (!first)
            {
                // Expects a value
                TextBuffer.Status.AddParseError(() => MessageRes.itc08, GetRule(this).Name);

                // Read a value 
                foreach (var item in _otherForms)
                {
                    TextBuffer.GetLoopForward(null);
                    item.ResolveErrorsForward(0);
                }
            }

            return false;
        }

        /// <summary>Load a binary operator.</summary>
        /// <param name="outElements">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool LoadBinaryOperator(List<TextElement> outElements, int level)
        {
            // Read a value 
            foreach (var item in _binaryOperators)
                if (item.Load(outElements, level))
                    return true;
            return false;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            bool isOp = _binaryOperators.Any(op => last.ParserElement == op);

            if (isOp)
            {
                TextBuffer.GetLoopForward(null);
                if (!ResolveErrorsForwardBinaryOperator(0))
                    return 1;
            }

            return 0;
        }

        public override bool ResolveErrorsForward(int level)
        {
            int from = TextBuffer.PointerNextChar;

            // Read a value first
            if (!ResolveErrorsForwardValue(level, true))
                // if the expression does'nt start with a value; the intire expression fails.
                return SetPointerBack(from);

            // Read following operations as alternately binary operators and values.
            from = TextBuffer.PointerNextChar;
            while (ResolveErrorsForwardBinaryOperator(level) && ResolveErrorsForwardValue(level, false))
                from = TextBuffer.PointerNextChar;

            // Set pointer back
            TextBuffer.PointerNextChar = from;

            return true;

        }

        /// <summary>ResolveErrorsForward a value. Inclusive const, variables and unary operators.</summary>
        /// <param name="operations">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool ResolveErrorsForwardValue(int level, bool first)
        {

            // Read a value 
            foreach (var item in _otherForms)
                if (item.ResolveErrorsForward(level))
                    return true;

            if (!first)
            {
                // Expects a value
                TextBuffer.Status.AddParseError(() => MessageRes.itc08, GetRule(this).Name);

                // Read a value 
                foreach (var item in _otherForms)
                {
                    TextBuffer.GetLoopForward(null);
                    item.ResolveErrorsForward(0);
                }
            }

            return false;
        }

        /// <summary>ResolveErrorsForward a binary operator.</summary>
        /// <param name="outElements">The value is added to this list.</param>
        /// <param name="level">Level of rule links.</param>
        /// <returns>True if succes.</returns>
        private bool ResolveErrorsForwardBinaryOperator(int level)
        {
            // Read a value 
            foreach (var item in _binaryOperators)
                if (item.ResolveErrorsForward(level))
                    return true;

            // Expects a operator
            TextBuffer.Status.AddParseError(() => MessageRes.itc09, GetRule(this).Name);

            return false;
        }


        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            return _otherForms.Aggregate(false, (ok, item) => ok | item.InitializeLoop(rules, path, status));
        }

        public override bool InitializeLoopHasWord(RuleLink link, List<RuleLink> subPath, ref bool linkFound)
        {
            foreach (ParserElementBase item in _otherForms)
                if (item.InitializeLoopHasWord(link, subPath, ref linkFound))
                    return true;
                else if (linkFound)
                    return false;

            return false;
        }
    }
}