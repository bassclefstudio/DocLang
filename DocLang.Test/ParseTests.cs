using BassClefStudio.DocLang.Content;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Test
{
    [TestClass]
    public class ParseTests
    {
        /// <summary>
        /// The <see cref="DocLangParser"/> setup with default settings that will be used as a baseline for parsing <see cref="Document"/>s.
        /// </summary>
        public static DocLangParser? Parser { get; private set; }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Parser = new DocLangParser();
        }

        [TestMethod]
        public void TestDocument()
        {
            Assert.IsNotNull(Parser, nameof(Parser));
            //// Version 1.0 of the DocLang specification.
            TextNode docTitleText = new TextNode("My Test Document");
            ContentNode docTitle = new ContentNode(ContentNode.TitleType);
            docTitle.AddContent(docTitleText);
            Document testDocument = new Document("testDoc", "Test Document", docTitle);
            testDocument.Authors.Add(new Metadata.Author(Metadata.AuthorType.Creator, "bassclefstudio"));
            TextNode h1TitleText = new TextNode("My Test Document");
            ContentNode h1Title = new ContentNode(ContentNode.TitleType);
            h1Title.AddContent(h1TitleText);
            Heading heading1 = new Heading("h1", "Heading 1", h1Title);
            ContentNode p1 = new ContentNode(ContentNode.ParagraphType);
            p1.AddContent(new TextNode("I'm generally a very boring person, much like this document."));
            ContentNode p2 = new ContentNode(ContentNode.ParagraphType);
            p2.AddContent(new TextNode("Most interestingly, I enjoy taking long walks on the beach."));
            heading1.AddContent(p1);
            heading1.AddContent(p2);
            testDocument.AddContent(heading1);
            XDocument document = Parser.Write(testDocument);
            Console.WriteLine(document.ToString());
        }
    }
}
