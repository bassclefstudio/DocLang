using BassClefStudio.DocLang.Content;
using BassClefStudio.DocLang.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Test
{
    [TestClass]
    public class HtmlTests
    {
        /// <summary>
        /// The <see cref="HtmlDocFormatter"/> setup with debug enabled for exporting <see cref="Document"/>s as HTML.
        /// </summary>
        public static HtmlDocFormatter? Format { get; private set; }

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            //// Compare to version 1.0 of the DocLang specification.
            Format = new HtmlDocFormatter(true);
            if (!Format.Initialized)
            {
                await Format.InitializeAsync();
            }
        }

        [TestMethod]
        public async Task ReadTestDocument()
        {
            Assert.IsNotNull(Format, "HtmlDocFormatter was not set before test execution.");
            Assert.IsTrue(Format.Initialized, "HtmlDocFormatter was not properly initialized.");
            //// Test against version 1.0 of the schema.
            Document testDocument = TestDoc.Version1;
            using (Stream stream = new MemoryStream())
            {
                await Format.WriteDocumentAsync(testDocument, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = await reader.ReadToEndAsync();
                    Console.WriteLine(content);
                }
            }
        }
    }
}
