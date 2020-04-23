﻿using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element;

namespace IntoTheCode
{
    /// <summary>Represents a text or code document.</summary>
    /// <remarks>Inherids <see cref="ReadElement"/></remarks>
    public class CodeDocument : ReadElement
    {
        //internal CodeDocument()
        //{
        //}

        internal CodeDocument(IEnumerable<TopElement> elements, ParserElementBase element) : base (element, null)
        {
            foreach (var item in elements)
                AddElement(item);
        }
        
        /// <summary>
        /// Parse code with a given parser.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static CodeDocument Load(Parser parser, string input)
        {
            TextBuffer buffer = new FlatBuffer(input);
            CodeDocument doc = parser.ParseString(buffer);
            if (doc != null) return doc;

            // only place to throw exception is CodeDocument.Load and Parser.SetGrammar (and MetaGrammar)
            var error = new ParserException(buffer.Status.Error.Message);
            error.AllErrors.AddRange(buffer.Status.AllErrors);
            throw error;
        }

        /// <inheritdoc cref="TopElement"/>
        public override string GetValue() { return _value; }

        /// <summary>Transform document to markup.</summary>
        /// <param name="xmlEncode">Encode the values to xml.</param>
        /// <returns>A string of markup.</returns>
        public string ToMarkup(bool xmlEncode = false)
        {
            return ToMarkupProtected(string.Empty, xmlEncode);
        }

        // https://msdn.microsoft.com/en-us/library/bb675186(v=vs.110).aspx
        /*
        string xslMarkup = @"<?xml version='1.0'?>
    <xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
        <xsl:template match='/Parent'>
            <Root>
                <C1>
                <xsl:value-of select='Child1'/>
                </C1>
                <C2>
                <xsl:value-of select='Child2'/>
                </C2>
            </Root>
        </xsl:template>
    </xsl:stylesheet>";

    XDocument xmlTree = new XDocument(
        new XElement("Parent",
            new XElement("Child1", "Child1 data"),
            new XElement("Child2", "Child2 data")
        )
    );

    XDocument newTree = new XDocument();
    using (XmlWriter writer = newTree.CreateWriter()) {
        // Load the style sheet.
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(XmlReader.Create(new StringReader(xslMarkup)));

        // Execute the transform and output the results to a writer.
        xslt.Transform(xmlTree.CreateReader(), writer);
    }
     */

        /// <summary>Compare two TextDocuments. Only elements in the 'expect' document are compared.</summary>
        /// <returns>Empty string if equal. Message with path if different.</returns>
        /// <exclude/>
        internal static string CompareCode(CodeDocument actual, CodeDocument expect)
        {
            string msg;
            if (expect == null && actual == null) return string.Empty;
            if (expect == null) return "Expected doc is null";
            if (actual == null) return "Actual doc is null";
            if (actual.SubElements == null && expect.SubElements == null) return string.Empty;
            if (expect == null || expect.SubElements == null) return "Expected doc has no elements";
            if (actual == null || actual.SubElements == null) return "Actual doc has no elements";
            foreach (TopElement xpct in expect.SubElements)
            {
                TopElement actualElement = actual.SubElements.FirstOrDefault(n => n.Name == xpct.Name && GetRuleName(n) == GetRuleName(xpct));
                if (actualElement == null)
                    return string.Format("Actual element '{0}', '{1}' is missing", xpct.Name, GetRuleName(xpct));
                msg = CompareElement(actualElement, xpct, string.Format("{0}[{1}]", xpct.Name, GetRuleName(xpct)));
                if (!string.IsNullOrEmpty(msg))
                    return msg;
            }
            return string.Empty;
        }

        private static string GetRuleName(TopElement rule)
        {
            if (rule == null) return "Rule is null";
            if (rule.SubElements == null) return "Rule has no sub elements";
            TopElement ident = rule.SubElements.FirstOrDefault(n => n.Name == MetaParser.WordIdent__);
            if (ident == null) return "Rule has no ruleId element";
            return string.IsNullOrEmpty(ident.Value) ? "Rule has no name" : ident.Value;
        }

        /// <summary>Compare two Element.</summary>
        /// <returns>Empty string if equal. Message with path if different.</returns>
        /// <exclude/>
        private static string CompareElement(TopElement actual, TopElement expect, string path)
        {
            string msg;
            if (expect == null || actual == null) return string.Format("Path: ({0}). Missing element", path);
            if (expect.Name != actual.Name) return string.Format("Path: ({0}). Name difference ({1})", path, actual.Name);
            if (expect.Value != actual.Value) return string.Format("Path: ({0}). Value difference ({1})", path, actual.Value);
            if (expect.SubElements == null && actual.SubElements == null) return string.Empty;
            if (expect.SubElements == null && actual.SubElements != null) return string.Format("Path: ({0}). Expected has no subelements", path);
            if (expect.SubElements != null && actual.SubElements == null) return string.Format("Path: ({0}). Actual has no subelements", path);
            if (expect.SubElements.Count() != actual.SubElements.Count()) return string.Format("Path: ({0}). Number of subelements difference", path);
            for (int i = 0; i < actual.SubElements.Count(); i++)
            {
                msg = CompareElement(actual.SubElements[i], expect.SubElements[i], path + string.Format("[{1}]-'{0}'", expect.Name, i));
                if (!string.IsNullOrEmpty(msg))
                    return msg;
            }
            return string.Empty;
        }
    }
}
