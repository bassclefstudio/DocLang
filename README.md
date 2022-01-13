# DocLang
An easy-to-use, but fully-featured semantic document markup language. DocLang is intended as a personal alternative to things like Markdown and LaTeX, where I was looking for a simple way to write notes and other documents that could satisfy the following constraints:

- My "source code" needed to look readable to a programmer like myself, conforming to programming and writing conventions wherever possible.
- I needed to be able to store all data related to my document in plain text, or some other format that could be easily understood by something like Git.
- The content should be separated from its styling, but it should be evident how the content *should* be styled by looking at it. This means that I wanted to separate the style and content data, but still have helpful and understandable tags in the content that let me know what it was, rather than some obscure class attribute that could mean anything *(looking at you, HTML/CSS...)*.
- For acedemic work, I wanted to be able to easily include cross-references to other places in my document, other books/websites/etc. that could be turned into citations if desired.

While this obviously isn't the most simple solution, I decided to see if I could create something myself that could do these things, and thus the idea of DocLang was born.

# Notable Features
Notable (additional) features of DocLang include:

- DocLang allows you to specify where a heading ends, so you can return to an upper-level header after writing in a sub-header (impossible in most Markdown).
- Metadata support - basic information about a document can be retrieved easily.
- Fully XML-serializable (easy to write your own parser for the language, or add an extension).

# DocLang Syntax
Writing in DocLang is designed to be easy to learn and use while writing just about anything. Broadly, DocLang syntax uses XML to manage tags and content, similar to HTML or XAML[^1].

```XML
<Document Name="Test Document" Id="testDoc">
  <Title>My Test Document</Title>
  <Authors>
    <Author Type="Creator">bassclefstudio</Author>
  </Authors>
  <Content>
    <Heading Name="Heading 1" Id="h1">
      <Title>First Heading</Title>
      <Content>
        <Paragraph>
            I'm generally a very boring person, much like this document.
        </Paragraph>
        <Paragraph>
            Most interestingly, I enjoy taking long walks on the beach.
        </Paragraph>
      </Content>
    </Heading>
  </Content>
</Document>
```

## Using XML
DocLang XML syntax consists of XML tags (such as `<tag>`, `</tag>`, and `<tag/>`, which specify various sections within the document in which certain features are enabled. Each one of these pairs of open/close tags (or a complete tag `<tag/>`) constitutes an **element** in XML, which corresponds to a **node** in the DocLang document tree. Additionally, elements can have attributes as well as children: as in the example above (`Id="chapter-1"`), simple property values can be added inline without having their own element. All *nodes*, however, require their own full XML element.

# Document
The **Document** is the root *node* of every DocLang file. It contains basic information about the entirety of the document, as well as a content collection of one or more *nodes*. These contain the structure of the document itself.

[^1]: Yes, technically HTML and XAML aren't always "true XML", but anyone familiar with these concepts understands the basics of XML syntax so it's fine for this comparison.