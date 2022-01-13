<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" />
  <!-- Document Root -->
  <xsl:template match="Document">
    <html>
      <head>
        <title>
          <xsl:value-of select="Name"/>
        </title>
      </head>
      <body>
        <h1>
          <xsl:apply-templates select="Title" />
        </h1>
        <section>
          <h2>Authors</h2>
          <xsl:apply-templates select="Authors//*" />
        </section>
        <hr/>
        <xsl:apply-templates select="Content" />
      </body>
    </html>
  </xsl:template>

  <!-- Author -->
  <xsl:template match="Author">
    <strong><xsl:value-of select="." />: </strong>
    <xsl:value-of select="@Type" />
  </xsl:template>

  <!-- Copy content -->
  <xsl:template match="Content">
    <div>
      <xsl:apply-templates />
    </div>
  </xsl:template>

  <!-- <Link> links (not in v1) -->
  <xsl:template match="Link">
    <a href="#{@ref}">
      <xsl:apply-templates />
    </a>
  </xsl:template>

  <!-- Paragraphs -->
  <xsl:template match="Paragraph">
    <p>
      <xsl:apply-templates/>
    </p>
  </xsl:template>

  <xsl:template match="Heading">
    <h2>
      <xsl:value-of select="Title" />
    </h2>
    <xsl:apply-templates select="Content" />
  </xsl:template>

  <!-- All mixed-content text is preserved -->
  <xsl:template match="text()">
    <xsl:copy-of select="." />
  </xsl:template>
</xsl:stylesheet>