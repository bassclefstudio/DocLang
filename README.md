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
Writing in DocLang is designed to be easy to learn and use while writing just about anything. Broadly, DocLang syntax uses XML to manage tags and content, similar to HTML or XAML.

```XML
<Title>My Article</Title>
<Header>
    <Name>A New Chapter</Name>
    <Content>
        You can write text like so, that will appear as content. Within this content, you will find that you can <Bold>format</Bold> text, as well as <Important>specify the purpose of a specific piece of the text</Important>.
    </Content>
</Header>
```