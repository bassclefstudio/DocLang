<xsl:stylesheet xmlns:t="http://bassclefstudio.dev/DocLang/v1/Base" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" />
  <!-- Document Root -->
  <xsl:template match="/t:Document">
    <div class="document-container container body">
      <div class="document">
        <h1 class="header-container container">
          <xsl:apply-templates select="t:Title" />
        </h1>
        <div class="author-container container">
          <xsl:apply-templates select="t:Authors//*" />
        </div>
        <div class="toc container">
          <h2>Table of Contents</h2>
          <xsl:for-each select="t:Content//t:Heading">
            <div class="toc-heading">
              <xsl:element name="a">
                <xsl:attribute name="href">#<xsl:value-of select="@Id" /></xsl:attribute>
                <xsl:value-of select="@Name" />
              </xsl:element>
            </div>
          </xsl:for-each>
        </div>
        <div class="document-body">
          <xsl:apply-templates select="t:Content" />
        </div>
      </div>
    </div>
  </xsl:template>

  <!-- Author -->
  <xsl:template match="t:Author">
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
  <xsl:template match="t:Content">
    <div class="content-container container">
      <xsl:apply-templates />
    </div>
  </xsl:template>

  <!-- <Link> links (not in v1) -->
  <xsl:template match="t:Link">
    <a href="#{@ref}" class="link">
      <xsl:apply-templates />
    </a>
  </xsl:template>

  <!-- Paragraphs -->
  <xsl:template match="t:Paragraph">
    <div class="paragraph-container container">
      <p class="paragraph">
        <xsl:apply-templates />
      </p>
    </div>
  </xsl:template>

  <xsl:template match="t:Heading">
    <div class="heading-container container">
      <xsl:element name="h{count(ancestor-or-self::t:Heading) + 1}">
        <xsl:attribute name="class">heading-title</xsl:attribute>
        <xsl:attribute name="id"><xsl:value-of select="@Id"/></xsl:attribute>
        <span>
          <xsl:value-of select="t:Title" />
        </span>
      </xsl:element>
      <xsl:apply-templates select="t:Content" />
    </div>
  </xsl:template>

  <!-- All mixed-content text is preserved -->
  <xsl:template match="text()">
    <xsl:copy-of select="." />
  </xsl:template>
</xsl:stylesheet>