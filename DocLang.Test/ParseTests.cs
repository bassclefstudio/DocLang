﻿using BassClefStudio.DocLang.Content;
using BassClefStudio.DocLang.Metadata;
using BassClefStudio.DocLang.Parsing;
using BassClefStudio.DocLang.Parsing.Base;
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
        /// The <see cref="XmlParser"/> setup with default settings that will be used as a baseline for parsing <see cref="Document"/>s.
        /// </summary>
        public static XmlParser? Parser { get; private set; }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Parser = new XmlParser(BaseDocLangSchema.Version1); ;
        }

        [TestMethod]
        public void TestDocument()
        {
            Assert.IsNotNull(Parser, nameof(Parser));
            //// Version 1.0 of the DocLang specification.
            TextNode docTitleText = new TextNode("My Test Document");
            Document testDocument = new Document("testDoc", "Test Document");
            testDocument.Title.Add(docTitleText);
            testDocument.Authors.Add(new Author(AuthorType.Creator, "bassclefstudio"));
            TextNode h1TitleText = new TextNode("First Heading");
            HeadingNode heading1 = new HeadingNode("h1", "Heading 1");
            heading1.Title.Add(h1TitleText);
            ParagraphNode p1 = new ParagraphNode(new TextNode("I'm generally a very boring person, much like this document."));
            ParagraphNode p2 = new ParagraphNode(new TextNode("Most interestingly, I enjoy taking long walks on the beach."));
            heading1.Content.Add(p1);
            heading1.Content.Add(p2);
            testDocument.Content.Add(heading1);
            XNode document = Parser.Write(testDocument);
            Console.WriteLine(document.ToString());
        }
    }
}