﻿using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Basic;
using IntoTheCode.Read.Structure;
using IntoTheCode.Read.Words;
using IntoTheCode.Basic.Util;
using IntoTheCode.Message;

namespace IntoTheCode.Read
{
    /// <summary>Build Grammar elements for a parser from dokument</summary>
    internal class ParserBuilder
    {
        ///
        internal static bool BuildRules(Parser parser, CodeDocument doc, ParserStatus status)
        {
            parser.Rules = new List<Rule>();

            foreach (CodeElement ruleElement in doc.Codes(MetaParser.Rule_______))
            {
                string debug1 = "(" + parser.Name + ")".NL() + ruleElement.ToMarkupProtected("");

                List<CodeElement> ruleElements = ruleElement.Codes().ToList();
                CodeElement elementId = ruleElements.First();
                ruleElements.Remove(elementId);
                List<ParserElementBase> elements = BuildExpression(parser, ruleElements, status);
                Rule rule = AddRule(parser, elementId, elements.ToArray());

                string debug2 = debug1 + rule.GetGrammar();
            }

            parser.Rules[0].Comment = true;

            return ApplySettingsFromGrammar(parser, doc, status) &&
                    InitializeGrammar(parser, parser.Rules, status) &&
                    ValidateGrammar(parser, status);
        }

        private static Rule AddRule(Parser parser, CodeElement defElement, params ParserElementBase[] elements)
        {
            var rule = new Rule(defElement.Value, elements) {
                DefinitionCodeElement = defElement
            };
            parser.Rules.Add(rule);
            return rule;
        }

        private static List<ParserElementBase> BuildExpression(Parser parser, IEnumerable<CodeElement> docNodes, ParserStatus status)
        {
            //string debug1 = "(" + parser.Name + ")".NL() + docNotes.Aggregate("", (s, n) => s + n.ToMarkupProtected(""));

            List<ParserElementBase> elements = new List<ParserElementBase>();
            foreach (CodeElement element in docNodes)
            {
                switch (element.Name)
                {
                    case MetaParser.Expression_:
                        if (docNodes.Count() > 1)
                            status.AddBuildError(() => MessageRes.itc29, element, parser.Name);

                        return BuildExpression(parser, element.Codes(), status);

                    case MetaParser.Or_________:
                        ParserElementBase el1, el2;
                        List<CodeElement> orNodes = docNodes.ToList();
                        // find position
                        int pos = 0;
                        while (pos + 2 < orNodes.Count() && orNodes[++pos] != element) { }
                        if (pos < 1 || pos + 2 > orNodes.Count())
                            status.AddBuildError(() => MessageRes.itc30, element, parser.Name);

                        if (pos == 1)
                            el1 = elements[0];
                        else
                            el1 = new SetOfElements(elements.ToArray());

                        var elementElements2 = new List<CodeElement>();
                        for (int i = pos + 1; i < orNodes.Count(); i++)
                            elementElements2.Add(orNodes[i]);
                        List<ParserElementBase> elements2 = BuildExpression(parser, elementElements2, status);

                        if (elements2.Count() == 1)
                            el2 = elements2[0];
                        else
                            el2 = new SetOfElements(elements2.ToArray());
                        return new List<ParserElementBase> { new Or(el1, el2) };

                    case MetaParser.WordIdent__:
                        ParserElementBase elem;
                        switch (element.Value)
                        {
                            case MetaParser.WordString_:
                                elem = new WordString();
                                break;
                            case MetaParser.WordInt____:
                                elem = new WordInt();
                                break;
                            case MetaParser.WordFloat__:
                                elem = new WordFloat();
                                break;
                            case MetaParser.WordIdent__:
                                elem = new WordIdent();
                                break;
                            case MetaParser.WordBool___:
                                elem = new WordBool();
                                break;
                            default:
                                elem = new RuleLink(element.Value);
                                elem.DefinitionCodeElement = element;
                                break;
                        }
                        elements.Add(elem);
                        break;

                    case MetaParser.WordSymbol_:
                        elements.Add(new WordSymbol(element.Value));
                        break;
                    case MetaParser.Sequence___:
                        elements.Add(new Sequence(BuildExpression(parser, element.Codes(), status).ToArray()));
                        break;
                    case MetaParser.Optional___:
                        elements.Add(new Optional(BuildExpression(parser, element.Codes(), status).ToArray()));
                        break;
                    case MetaParser.Parentheses:
                        elements.Add(new Parentheses(BuildExpression(parser, element.Codes(), status).ToArray()));
                        break;
                    case MetaParser.Comment____:
                        break;
                    default:
                        status.AddBuildError(() => MessageRes.itc31, element, element.Name);
                        break;
                }
            }
            return elements;
        }

        /// <summary>Initialize rules and elements. Called when creating a new parser and when cloning grammar.</summary>
        /// <param name="parser">The parser.</param>
        /// <param name="rules">Set of rules.</param>
        /// <param name="status">The parser status.</param>
        /// <returns>True: no error. False: error.</returns>
        internal static bool InitializeGrammar(Parser parser, List<Rule> rules, ParserStatus status)
        {
            foreach (Rule rule in rules)
            {
                rule.Parser = parser;

                //string debug1 = "" + parser.Level + ": " + rule.GetGrammar().NL() +
                //    rule.ToMarkupProtected(string.Empty);

                if (rules.Any(r => r != rule && r.Name.ToLower() == rule.Name.ToLower()))
                    status.AddBuildError(() => MessageRes.itc23, rule.DefinitionCodeElement, 
                        rule.Name, 
                        parser.Name);
            }

            foreach (Rule rule in rules)
                InitializeElements(rule.ChildNodes, rules, status);

            // Transformation of syntax: Initialize expressions here
            foreach (Rule rule in rules)
            {
                Or or = rule.ChildNodes[0] as Or;
                WordSymbol symbol = null;
                Rule ruleOp = null;
                if (or != null && 
                    Expression.IsBinaryAlternative(rule, or.ChildNodes[0], out symbol, out ruleOp))
                    rule.ReplaceSubElement(0, new Expression(rule, or));
            }

            if (status.Error == null)
            {
                // Loop check and set RuleLink.Recursive
                var effectiveRules = new List<Rule>();
                parser.Rules[0].InitializeLoop(effectiveRules, new List<ParserElementBase>(), status);

                parser.Rules[0].InitializeLoop(effectiveRules, new List<ParserElementBase>(), status);

                foreach (Rule rule in effectiveRules)
                    if (!rule.LoopHasEnd)
                        status.AddBuildError(() => MessageRes.itc25, rule.DefinitionCodeElement, rule.Name);

                foreach (Rule rule in effectiveRules)
                    if (rule.LoopLeftRecursive)
                        status.AddBuildError(() => MessageRes.itc28, rule.DefinitionCodeElement, rule.Name);
            }
            return status.Error == null;
        }

        /// <summary>Initialize rules and elements. Called when creating a new parser and when cloning grammar.</summary>
        /// <param name="elements">Elements to Initialize.</param>
        /// <param name="rules">Set of rules.</param>
        /// <param name="status">The parser status.</param>
        private static void InitializeElements(IEnumerable<ParserElementBase> elements, List<Rule> rules, ParserStatus status)
        {
            if (elements == null || elements.Count() == 0) return;
            foreach (ParserElementBase element in elements)
            {
                var ruleId = element as RuleLink;
                if (ruleId != null && ruleId.RuleElement == null) ruleId.RuleElement = InitializeResolve(rules, ruleId, status);
                InitializeElements(element.ChildNodes, rules, status);
            }
        }

        /// <summary>Find the rule to a RuleLink.</summary>
        /// <param name="rules"></param>
        /// <param name="link"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private static Rule InitializeResolve(List<Rule> rules, RuleLink link, ParserStatus status)
        {
            Rule rule = rules.FirstOrDefault(r => r.Name == link.Value);
            if (rule == null)
                status.AddBuildError(() => MessageRes.itc24, link.DefinitionCodeElement, link.Value);
            return rule;
        }

        /// <summary>Apply settings to a linked Grammar.</summary>
        /// <returns></returns>
        private static bool ApplySettingsFromGrammar(Parser parser, CodeDocument doc, ParserStatus status)
        {
            bool ok = true;
            foreach (CodeElement SetterElement in doc.Codes(MetaParser.Setter_____))
            {
                CodeElement elementId = SetterElement.Codes().First();

                Rule rule = parser.Rules.FirstOrDefault(r => r.Name == elementId.Value);
                if (rule == null)
                {
                    status.AddBuildError(() => MessageRes.itc26, elementId, elementId.Value);
                    ok = false;
                    continue;
                }

                foreach (CodeElement assignElement in SetterElement.Codes(MetaParser.Assignment_))
                {
                    List<CodeElement> assignNodes = assignElement.Codes().ToList();
                    CodeElement propName = assignNodes[0];
                    string propValue = assignNodes.Count > 1 ? assignNodes[1].Value : string.Empty;
                    switch (propName.Value)
                    {
                        case MetaParser.Trust______:
                            rule.Trust = propValue != "false";
                            break;
                        case MetaParser.Collapse___:
                            rule.Collapse = propValue != "false";
                            break;
                        case MetaParser.Comment____:
                            rule.Comment = propValue != "false";
                            break;
                        default:
                            // set the property on rule or sub elements.
                            ok = rule.SetProperty(propName, propValue, status);

                            if (!ok)
                                status.AddBuildError(() => MessageRes.itc27, propName, elementId.Value, propName.Value);
                            break;
                    }
                }
            }
            return ok;
        }

        /// <summary>Apply settings to a linked Grammar.</summary>
        /// <returns></returns>
        internal static bool ValidateGrammar(Parser parser, ParserStatus status)
        {
            // Check first rule must represent all document. Tag = true
            bool ok = true;
            if (parser.Rules[0].Collapse)
            {
                status.AddBuildError(() => MessageRes.itc21, parser.Rules[0].DefinitionCodeElement, parser.Rules[0].Name);
                ok = false;
            }

            // Recursive check
            foreach (Rule rule in parser.Rules)
                ok = ValidateGrammarElement(rule, status) && ok;

            return ok;
        }

        private static bool ValidateGrammarElement(ParserElementBase elem, ParserStatus status)
        {
            bool ok = true;

            if (elem.ChildNodes == null) return ok;
            foreach (ParserElementBase sub in elem.ChildNodes)
                ok = ValidateGrammarElement(sub, status) && ok;

            return ok;
        }
    }
}
