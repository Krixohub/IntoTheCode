using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Basic;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Basic.Util;
using IntoTheCode.Message;

namespace IntoTheCode.Read
{
    /// <summary>Build syntax elements from dokument</summary>
    internal class ParserFactory
    {
        ///
        internal static bool BuildRules(Parser parser, CodeDocument doc, ParserStatus status)
        {
            parser.Rules = new List<Rule>();

            //string debug1 = doc.ToMarkup();

            foreach (TreeNode ruleElement in doc.Elements(MetaParser.Rule_______))
            {
                //string debug1 = string.Empty;
                //if (parser.Name == null)
                //    debug1 = "(" + parser.Name + ")".NL() + ruleElement.ToMarkupProtected("");

                string debug1 = "(" + parser.Name + ")".NL() + ruleElement.ToMarkupProtected("");

                CodeElement elementId = ruleElement.SubElements[0] as CodeElement;
                List<TreeNode> docSubNotes = ruleElement.Elements(n => n != elementId).ToList();
                List<ParserElementBase> elements = BuildExpression(parser, docSubNotes, status);
                Rule rule = AddRule(parser, elementId, elements.ToArray());

                string debug2 = debug1 + rule.GetSyntax();
            }

            return InitializeSyntax(parser, parser.Rules, status) &&
                    ApplySettingsFromSyntax(parser, doc, status) &&
                    ValidateSyntax(parser, status);
        }

        private static Rule AddRule(Parser parser, CodeElement defElement, params ParserElementBase[] elements)
        {
            var rule = new Rule(defElement.Value, elements) {
                DefinitionCodeElement = defElement
            };
            parser.Rules.Add(rule);
            return rule;
        }

        private static List<ParserElementBase> BuildExpression(Parser parser, IList<TreeNode> docNotes, ParserStatus status)
        {
            //string debug1 = "(" + parser.Name + ")".NL() + docNotes.Aggregate("", (s, n) => s + n.ToMarkupProtected(""));

            List<ParserElementBase> elements = new List<ParserElementBase>();
            foreach (CodeElement element in docNotes.OfType<CodeElement>())
            {
                switch (element.Name)
                {
                    case MetaParser.Expression_:
                        if (docNotes.Count() > 1)
                        //{
                            status.AddBuildError(() => MessageRes.pb02, element, parser.Name);
                        //}
                        return BuildExpression(parser, element.SubElements, status);
                    case MetaParser.Or_________:
                        ParserElementBase el1, el2;
                        // find position
                        int pos = 0;
                        while (pos + 2 < docNotes.Count() && docNotes[++pos] != element) { }
                        if (pos < 1 || pos + 2 > docNotes.Count())
                            status.AddBuildError(() => MessageRes.pb03, element, parser.Name);
                        //throw new Exception(string.Format("{0}: The 'or' symbol is misplaced in expression", parser.Name));

                        if (pos == 1)
                            el1 = elements[0];
                        else
                            el1 = new Parentheses(elements.ToArray());

                        IList<TreeNode> elementElements2 = new List<TreeNode>();
                        for (int i = pos + 1; i < docNotes.Count(); i++)
                            elementElements2.Add(docNotes[i]);
                        List<ParserElementBase> elements2 = BuildExpression(parser, elementElements2, status);

                        if (elements2.Count() == 1)
                            el2 = elements2[0];
                        else
                            el2 = new Parentheses(elements2.ToArray());
                        return new List<ParserElementBase> { new Or(el1, el2) };
                    //elements.Add(new Or(el1, el2));
                    //break;

                    case MetaParser.WordIdent__:
                        ParserElementBase elem;
                        switch (element.Value)
                        {
                            case MetaParser.WordString_:
                                elem = new WordString();
                                break;
                            case MetaParser.WordIdent__:
                                elem = new WordIdent(element.Value);
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
                        elements.Add(new Sequence(BuildExpression(parser, element.SubElements, status).ToArray()));
                        break;
                    case MetaParser.Optional___:
                        elements.Add(new Optional(BuildExpression(parser, element.SubElements, status).ToArray()));
                        break;

                    case MetaParser.Parentheses:
                        elements.Add(new Parentheses(BuildExpression(parser, element.SubElements, status).ToArray()));
                        break;
                    default:
                        status.AddBuildError(() => MessageRes.pb04, element, element.Name);
//                        throw new ParserException(string.Format("Parser factory: No read element for '{0}'", element.Name));
                        break;
                }
            }
            return elements;
        }

        internal static bool InitializeSyntax(Parser parser, List<Rule> rules, ParserStatus status)
        {
            foreach (Rule rule in rules)
            {
                rule.Parser = parser;

                string debug1 = "" + parser.Level + ": " + rule.GetSyntax().NL() +
                    rule.ToMarkupProtected(string.Empty);

                if (rules.Any(r => r != rule && r.Name.ToLower() == rule.Name.ToLower()))
                    status.AddBuildError(() => MessageRes.pb05, rule.DefinitionCodeElement, 
                        rule.Name, 
                        parser.Name);
                //throw new ParserException(string.Format("Link syntax {1}. Identifier {0} is defined twice",
                //        rule.Name, parser.Name));
            }

            foreach (Rule rule in rules)
                InitializeElements(rule.SubElements.OfType<ParserElementBase>(), rules, status);

            return status.Error == null;
        }

        private static void InitializeElements(IEnumerable<ParserElementBase> elements, List<Rule> rules, ParserStatus status)
        {
            if (elements == null || elements.Count() == 0) return;
            foreach (ParserElementBase element in elements)

            {
                var ruleId = element as RuleLink;
                if (ruleId != null && ruleId.SymbolElement == null) ruleId.SymbolElement = InitializeResolve(rules, ruleId, status);
                InitializeElements(element.SubElements.OfType<ParserElementBase>(), rules, status);
                // element.Initialize();
            }
        }

        private static ParserElementBase InitializeResolve(List<Rule> rules, RuleLink link, ParserStatus status)
        {
            Rule rule = rules.FirstOrDefault(r => r.Name == link.GetValue());
            if (rule == null)
                status.AddBuildError(() => MessageRes.pb06, link.DefinitionCodeElement, link.GetValue());
            //throw new ParserException(string.Format("Identifier '{0}' not found in syntax", name));
            return rule;
        }

        /// <summary>Apply settings to a linked syntax.</summary>
        /// <returns></returns>
        private static bool ApplySettingsFromSyntax(Parser parser, CodeDocument doc, ParserStatus status)
        {
            bool ok = true;
            foreach (TreeNode SetterElement in doc.Elements(MetaParser.Setter_____))
            {
                CodeElement elementId = SetterElement.SubElements[0] as CodeElement;

                Rule rule = parser.Rules.FirstOrDefault(r => r.Name == elementId.Value);
                if (rule == null)
                {
                    status.AddBuildError(() => MessageRes.pb07, elementId, elementId.Value);
                    ok = false;
                    continue;
                    //return false;
                }
                //throw new Exception(string.Format("{1} Settings: Identifier '{0}' cant be resolved", elementId.Value, parser.Name));

                foreach (TreeNode assignElement in SetterElement.Elements(MetaParser.Assignment_))
                {
                    CodeElement propName = assignElement.SubElements[0] as CodeElement;
                    string propValue = assignElement.SubElements.Count > 1 ? assignElement.SubElements[1].Value : string.Empty;
                    switch (propName.Value)
                    {
                        //case MetaParser.Tag________:
                        //    rule.Tag = propValue != "false";
                        //    break;
                        case MetaParser.Trust______:
                            rule.Trust = propValue != "false";
                            break;
                        case MetaParser.Collapse___:
                            rule.Collapse = propValue != "false";
                            break;
                        default:
                            status.AddBuildError(() => MessageRes.pb08, propName, elementId.Value, propName.Value);
                            ok = false;
                            break;
                     //       return false;
                            //throw new ParserException(string.Format("Parser factory: No property for '{0}'", propName));
                    }

                }
            }
            return ok;
        }

        /// <summary>Apply settings to a linked syntax.</summary>
        /// <returns></returns>
        internal static bool ValidateSyntax(Parser parser, ParserStatus status)
        {
            // Check first rule must represent all document. Tag = true
            bool ok = true;
            if (parser.Rules[0].Collapse)
            {
                status.AddBuildError(() => MessageRes.pb09, parser.Rules[0].DefinitionCodeElement, parser.Rules[0].Name);
                ok = false;
            }
//            throw new Exception(string.Format("First rule '{0} must represent all document and have Collapse=false", parser.Rules[0].Name));

            //recursive check
            foreach (Rule rule in parser.Rules)
                ok = ValidateSyntaxElement(rule, status) && ok;

            return ok;
        }

        private static bool ValidateSyntaxElement(ParserElementBase elem, ParserStatus status)
        {
            bool ok = true;
            if (elem == null)
            {
                int i = 3;
            }
            if (elem.ElementContent == ElementContentType.NotSet)
            {
                status.AddBuildError(() => MessageRes.pb01,
                    elem.DefinitionCodeElement,
                    elem.GetType().Name, elem.Name, elem.Value);
                ok = false;
            }
            //throw new Exception(string.Format("Element content type not set, {0}, {1} , {2}", elem.GetType().Name, elem.Name, elem.Value));
            if (elem.SubElements == null) return ok;
            foreach (ParserElementBase sub in elem.SubElements)
                ok = ValidateSyntaxElement(sub, status) && ok;
            return ok;
        }

    }
}
