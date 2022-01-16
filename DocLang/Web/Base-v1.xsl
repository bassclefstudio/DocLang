<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" />
  <!-- Document Root -->
  <xsl:template match="Document">
    <div class="document-container container body">
      <div class="document">
        <h1 class="header-container container">
          <xsl:apply-templates select="Title" />
        </h1>
        <div class="author-container container">
          <span class="author-title">
            <h2>Authors</h2>
          </span>
          <xsl:apply-templates select="Authors//*" />
        </div>
        <div class="document-body">
          <xsl:apply-templates select="Content" />
        </div>
      </div>
    </div>
  </xsl:template>

  <!-- Author -->
  <xsl:template match="Author">
    <div class="author">
      <span class="author-name">
        <xsl:value-of select="." />:
      </span>
      <span class="author-type">
        <xsl:value-of select="@Type" />
      </span>
    </div>
  </xsl:template>

  <!-- Copy content -->
  <xsl:template match="Content">
    <div class="content-container container">
      <xsl:apply-templates />
    </div>
  </xsl:template>

  <!-- <Link> links (not in v1) -->
  <xsl:template match="Link">
    <a href="#{@ref}" class="link">
      <xsl:apply-templates />
    </a>
  </xsl:template>

  <!-- Paragraphs -->
  <xsl:template match="Paragraph">
	  <div class="paragraph-container container">
		  <p class="paragraph">
			  <xsl:apply-templates />
		  </p>
	  </div>
  </xsl:template>

  <xsl:template match="Heading">
    <div class="heading-container container">
      <h2 class="heading-title">
        <xsl:value-of select="Title" />
      </h2>
      <xsl:apply-templates select="Content" />
    </div>
  </xsl:template>

  <!-- All mixed-content text is preserved -->
  <xsl:template match="text()">
    <xsl:copy-of select="." />
  </xsl:template>
</xsl:stylesheet>