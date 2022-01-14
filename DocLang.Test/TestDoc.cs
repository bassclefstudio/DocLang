using BassClefStudio.DocLang.Content;
using BassClefStudio.DocLang.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Test
{
    /// <summary>
    /// A service for creating a basic test document which is used for testing the full feature set of a given DocLang schema.
    /// </summary>
    public static class TestDoc
    {
        private static Document? version1;
        /// <summary>
        /// A full <see cref="Document"/> conforming to v1.0 of the DocLang schema.
        /// </summary>
        public static Document Version1 
        { 
            get
            {
                if (version1 is null)
                {
                    //// Full version 1.0 specification
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
                    version1 = testDocument;
                }
                return version1;
            }
        }
    }
}
