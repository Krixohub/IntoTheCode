using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Basic;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Basic.Util;

namespace IntoTheCode.Read
{
    /// <summary>Build syntax elements from dokument</summary>
    internal class ParserFactory
    {


        ///
        internal static bool BuildRules(Parser parser, CodeDocument doc)
        {
            parser.Rules = new List<Rule>();
            //if (doc.SubElements.Count > 0 && doc.SubElements[0].SubElements.Count > 0)
            //    parser.Name = doc.SubElements[0].SubElements[0].Value;
            //if (parser.Name == string.Empty) parser.Name = "Syntax";

            //string debug1 = doc.ToMarkup();

            foreach (TreeNode ruleElement in doc.Elements(MetaParser.Rule_______))
            {
                //string debug1 = string.Empty;
                //if (parser.Name == null)
                //    debug1 = "(" + parser.Name + ")".NL() + ruleElement.ToMarkupProtected("");

                string debug1 = "(" + parser.Name + ")".NL() + ruleElement.ToMarkupProtected("");

                TreeNode elementId = ruleElement.SubElements[0];
                List<TreeNode> docSubNotes = ruleElement.Elements(n => n != elementId).ToList();
                List<ParserElementBase> elements = ParserFactory.BuildExpression(parser, docSubNotes);
                Rule rule = AddRule(parser, elementId.Value, elements.ToArray());

                string debug2 = debug1 + rule.GetSyntax();
            }

            return InitializeSyntax(parser) &&
                    ApplySettingsFromSyntax(parser, doc) &&
                    ValidateSyntax(parser);
        }

        private static Rule AddRule(Parser parser, string ruleId, params ParserElementBase[] elements)
        {
            var rule = new Rule(ruleId, elements) { Parser = parser/*, Tag = true*/ };
            parser.Rules.Add(rule);
            return rule;
        }

        //internal static Rule AddRule(Parser parser, Rule rule)
        //{
        //    rule.Parser = parser;
        //    parser.Rules.Add(rule);
        //    return rule;
        //}

        private static List<ParserElementBase> BuildExpression(Parser parser, IList<TreeNode> docNotes)
        {
            //string debug1 = "(" + parser.Name + ")".NL() + docNotes.Aggregate("", (s, n) => s + n.ToMarkupProtected(""));

            List<ParserElementBase> elements = new List<ParserElementBase>();
            foreach (TreeNode element in docNotes)
            {
                switch (element.Name)
                {
                    case MetaParser.Expression_:
                        if (docNotes.Count() > 1)
                            throw new Exception(string.Format("{0}: An expression element can't be siebling with other elements", parser.Name));
                        return BuildExpression(parser, element.SubElements);
                    case MetaParser.Or_________:
                        ParserElementBase el1, el2;
                        // find position
                        int pos = 0;
                        while (pos + 2 < docNotes.Count() && docNotes[++pos] != element) { }
                        if (pos < 1 || pos + 2 > docNotes.Count())
                            throw new Exception(string.Format("{0}: The 'or' symbol is misplaced in expression", parser.Name));

                        if (pos == 1)
                            el1 = elements[0];
                        else
                            el1 = new Parentheses(elements.ToArray());

                        IList<TreeNode> elementElements2 = new List<TreeNode>();
                        for (int i = pos + 1; i < docNotes.Count(); i++)
                            elementElements2.Add(docNotes[i]);
                        List<ParserElementBase> elements2 = BuildExpression(parser, elementElements2);

                        if (elements2.Count() == 1)
                            el2 = elements2[0];
                        else
                            el2 = new Parentheses(elements2.ToArray());
                        return new List<ParserElementBase> { new Or(el1, el2) };
                    //elements.Add(new Or(el1, el2));
                    //break;


                    case MetaParser.WordName___:
                        ParserElementBase elem;
                        //var sym =
                        switch (element.Value)
                        {
                            case MetaParser.WordString_:
                                elem = new WordString();// { TodoResolve = true };
                                break;
                            case MetaParser.WordName___:
                                elem = new WordName(element.Value);// { TodoResolve = true };
                                break;
                            default:
                                elem = new RuleLink(element.Value);
                                break;
                        }
                        elements.Add(elem);
                        break;


                    // todo remove this RuleId_____
                    case MetaParser.RuleId_____:
                        ParserElementBase elem2;
                        //var sym =
                        switch (element.Value)
                        {
                            case MetaParser.WordString_:
                                elem2 = new WordString();// { TodoResolve = true };
                                break;
                            case MetaParser.WordName___:
                                elem2 = new WordName(element.Value);// { TodoResolve = true };
                                break;
                            default:
                                elem2 = new RuleLink(element.Value);
                                break;
                        }
                        elements.Add(elem2);
                        break;


                    case MetaParser.WordSymbol_:
                        elements.Add(new WordSymbol(element.Value));
                        break;
                    case MetaParser.Sequence___:
                        elements.Add(new Sequence(BuildExpression(parser, element.SubElements).ToArray()));
                        break;
                    case MetaParser.Optional___:
                        elements.Add(new Optional(BuildExpression(parser, element.SubElements).ToArray()));
                        break;

                    case MetaParser.Parentheses:
                        elements.Add(new Parentheses(BuildExpression(parser, element.SubElements).ToArray()));
                        break;
                    default:
                        throw new SyntaxErrorException(string.Format("Parser factory: No read element for '{0}'", element.Name));
                        break;
                }
            }
            return elements;
        }

        /// <summary>Apply settings to a linked syntax.</summary>
        /// <returns></returns>
        private static bool ApplySettingsFromSyntax(Parser parser, CodeDocument doc)
        {
            foreach (TreeNode SetterElement in doc.Elements(MetaParser.Setter_____))
            {
                TreeNode elementId = SetterElement.SubElements[0];

                Rule rule = parser.Rules.FirstOrDefault(r => r.Name == elementId.Value);
                if (rule == null)
                    throw new Exception(string.Format("{1} Settings: Identifier '{0}' cant be resolved", elementId.Value, parser.Name));

                foreach (TreeNode assignElement in SetterElement.Elements(MetaParser.Assignment_))
                {
                    string propName = assignElement.SubElements[0].Value;
                    string propValue = assignElement.SubElements.Count > 1 ? assignElement.SubElements[1].Value : string.Empty;
                    switch (propName)
                    {
                        //case MetaParser.Tag________:
                        //    rule.Tag = propValue != "false";
                        //    break;
                        case MetaParser.Div________:
                            rule.Div = propValue != "false";
                            break;
                        case MetaParser.Collapse___:
                            rule.Collapse = propValue != "false";
                            break;
                        default:
                            throw new SyntaxErrorException(string.Format("Parser factory: No property for '{0}'", propName));
                    }

                }
            }
            return true;
        }

        /// <summary>Apply settings to a linked syntax.</summary>
        /// <returns></returns>
        internal static bool ValidateSyntax(Parser parser)
        {
            // Check first rule must represent all document. Tag = true
            //if (!parser.Rules[0].Tag)
            //    throw new Exception(string.Format("First rule '{0} must represent all document and have Tag=true", parser.Rules[0].Name));
            if (parser.Rules[0].Collapse)
                throw new Exception(string.Format("First rule '{0} must represent all document and have Tag=true", parser.Rules[0].Name));

            //recursive check
            foreach (Rule rule in parser.Rules)
                ValidateSyntaxElement(rule);

            return true;
        }
        private static void ValidateSyntaxElement(ParserElementBase elem)
        {
            if (elem == null)
            {
                int i = 3;
            }
            if (elem.ElementContent == ElementContentType.NotSet)
                throw new Exception(string.Format("Element content type not set, {0}, {1} , {2}", elem.GetType().Name, elem.Name, elem.Value));
            if (elem.SubElements == null) return;
            foreach (ParserElementBase sub in elem.SubElements)
                ValidateSyntaxElement(sub);
        }

        internal static bool InitializeSyntax(Parser parser)
        {
            foreach (Rule rule in parser.Rules)
            {
                string debug1 = "" + parser.Level + ": " + rule.GetSyntax().NL() + 
                    rule.ToMarkupProtected(string.Empty);

                if (parser.Rules.Any(r => r != rule && r.Name.ToLower() == rule.Name.ToLower()))
                    throw new SyntaxErrorException(string.Format("Link syntax {1}. Identifier {0} is defined twice",
                        rule.Name, parser.Name));
            }

            foreach (Rule rule in parser.Rules)
                InitializeElements(parser, rule.SubElements.OfType<ParserElementBase>());

            return true;
        }

        internal static void InitializeElements(Parser parser, IEnumerable<ParserElementBase> elements)
        {
            if (elements == null || elements.Count() == 0) return;
            foreach (ParserElementBase element in elements)

            {
                var ruleId = element as RuleLink;
                if (ruleId != null && ruleId.SymbolElement == null) ruleId.SymbolElement = Resolve(parser, ruleId.GetValue());
                InitializeElements(parser, element.SubElements.OfType<ParserElementBase>());
                element.Initialize();
            }
        }

        private static ParserElementBase Resolve(Parser parser, string name)
        {
            //switch (name)
            //{
            //    case MetaParser.WordString_: return new WordString();// { TodoResolve = true };
            //    case MetaParser.WordName___: return new WordName(name);// { TodoResolve = true };
            //}

            Rule rule = parser.Rules.FirstOrDefault(r => r.Name == name);
            if (rule == null)
                throw new SyntaxErrorException(string.Format("Identifier '{0}' not found in syntax", name));
            return rule;
        }

    }
}
