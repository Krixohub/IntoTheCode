using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zzz
{
    public class MyElement : XElement
    {
        /// <summary>
        /// Can XElement be used for TextElements?
        /// </summary>
        public MyElement() : base("MyElem")
        {
        }
    }

   [TestClass]
    public class DivTest
    {
        [TestMethod]
        public void LinqToXml()
        {
            string xmlStr = @"
<root>
  <main>
    <a>45</a>
    <b>hej</b>
    <c>4</c>
  </main>
  <main>
    <a>22</a>
    <b>davb</b>
    <c>7</c>
    <subs>
      <sub>
        <f1>rt</f1> 
        <f2>4</f2> 
        <f3>hg</f3> 
      </sub>
      <sub>
        <f1>er</f1> 
        <f2>5</f2> 
        <f3>hj</f3> 
      </sub>
    </subs>
  </main>
</root>";
            //TextReader tr = new StringReader("<?xml version=\"1.0\"?>" + xmlStr);
            //XDocument doc = XDocument.Load(tr);
            XElement elem = XElement.Load(new StringReader(xmlStr));

            string elemStr = elem.ToString();
            Assert.AreEqual("<root>", elemStr.Substring(0, 6), "How to read xml");

            // Linq query example
            IEnumerable<XElement> a22subs = elem.Elements("main").Where(item => (string)item.Element("a") == "22").
                                            Select(item => item.Element("subs"));
            //IEnumerable<XElement> a22subs = from item in elem.Elements("main")
            //                                where (string)item.ElementBase("a") == "22"
            //                                select item.ElementBase("subs");
            IEnumerable<string> a22f3s = from item in a22subs.Elements("sub")
                                         select item.Element("f3").Value;

            Assert.AreEqual("hg", a22f3s.First(), "How to read xml. Linq query");

            // Linq lambda example
            IEnumerable<string> a22f3s2 = from item in
                                              (elem.Elements("main").
                                              Where(item => (string)item.Element("a") == "22").
                                              Select(item => item.Element("subs"))).Elements("sub")
                                          select item.Element("f2").Value;

            Assert.AreEqual("5", a22f3s2.Last(), "How to read xml. Linq lambda");
        }

        [TestMethod]
        public void LinqToXmlComment()
        {
            XElement elem = new XElement("root",
                new XComment("To start with"),
                new XElement("main",
                    new XElement("a", "45"),
                    new XComment("a = 45"),
                    new XElement("b", "hej"),
                    new XElement("c", "7")
                    ),
                new XElement("main",
                    new XElement("a", "22"),
                    new XComment("a is 22  here"),
                    new XElement("b", "davb"),
                    new XElement("c", "7")
                    ),
                new XComment("Wrap it up"));
            // convert to xml
            string xmlActual = elem.ToString();
            string xmlExpect = @"<root>
  <!--To start with-->
  <main>
    <a>45</a>
    <!--a = 45-->
    <b>hej</b>
    <c>7</c>
  </main>
  <main>
    <a>22</a>
    <!--a is 22  here-->
    <b>davb</b>
    <c>7</c>
  </main>
  <!--Wrap it up-->
</root>";
            Assert.AreEqual(xmlExpect, xmlActual, "xml comment. How to read xml");

            // Linq query example
            IEnumerable<XElement> main_a22_b = elem.Elements("main").Where(item => (string)item.Element("a") == "22").
                                            Select(item => item.Element("b"));
            Assert.AreEqual("davb", main_a22_b.First().Value, "xml comment. Test query");

            IEnumerable<XComment> main_comments = elem.Elements("main").Nodes().OfType<XComment>();
            Assert.AreEqual("a is 22  here", main_comments.Last().Value, "xml comment. get comment");

            int elementCount = elem.Elements().Count();
            Assert.AreEqual(2, elementCount, "xml comment. Count elements");

            int nodeCount = elem.Nodes().Count();
            Assert.AreEqual(4, nodeCount, "xml comment. Count Nodes");

            var desc = elem.DescendantNodes();
            int descCount = desc.Count();
            Assert.AreEqual(18, descCount, "xml comment. Count Nodes");
        }
    }
}
