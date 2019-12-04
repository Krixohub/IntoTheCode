using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCodeInternal.UnitTest
{
    [TestClass]
    public class LinqToXml
    {
        [TestMethod]
        public void LinqToXml_Linq()
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
    }
}
