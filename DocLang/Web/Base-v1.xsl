<xsl:stylesheet 
  xmlns:t="http://bassclefstudio.dev/DocLang/v1/Base"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  version="1.0">
  <xsl:output method="html" encoding="ASCII"/>
  <!-- Document Root -->
  <xsl:template match="/t:Document">
    <div class="document-container">
      <div class="document">
        <h1 class="title document-title">
          <xsl:apply-templates select="t:Title" />
        </h1>
        <div class="author-container">
          <xsl:apply-templates select="t:Authors//*" />
        </div>
        <div class="toc-container">
          <h2 class="title toc-title">Table of Contents</h2>
          <xsl:for-each select="t:Content/t:Heading">
            <xsl:call-template name="toc-header" />
          </xsl:for-each>
        </div>
        <div class="document-body">
          <xsl:apply-templates select="t:Content" />
        </div>
      </div>
    </div>
  </xsl:template>

  <!--Table of Contents-->
  <xsl:template name="toc-header">
    <div class="toc-heading">
      <xsl:element name="a">
        <xsl:attribute name="href">#<xsl:value-of select="@Id"/></xsl:attribute>
        <xsl:value-of select="@Name" />
      </xsl:element>
      <div class="toc-group">
        <xsl:for-each select="t:Content/t:Heading">
          <xsl:call-template name="toc-header" />
        </xsl:for-each>
      </div>
    </div>
  </xsl:template>

  <!-- Author -->
  <xsl:template match="t:Author">
    <div class="author">
      <span class="author-name">
        <xsl:value-of select="." />,
      </span>
      <span class="author-type">
        <xsl:value-of select="@Type" />
      </span>
    </div>
  </xsl:template>

  <!-- Copy content -->
  <xsl:template match="t:Content">
    <div class="content-container">
      <xsl:apply-templates />
    </div>
  </xsl:template>

  <!-- <Link> links (not in v1) -->
  <xsl:template match="t:Link">
    <a href="#{@ref}" class="link">
      <xsl:apply-templates />
    </a>
  </xsl:template>

  <!-- <Extension> blocks ("Web" only) -->
  <xsl:template match="t:Extension[@Type='Web']">
    <xsl:copy-of select="t:Content/*"/>
  </xsl:template>

  <!-- Paragraphs -->
  <xsl:template match="t:Paragraph">
    <div class="paragraph-container">
      <p class="paragraph">
        <xsl:apply-templates />
      </p>
    </div>
  </xsl:template>

  <!-- Lists -->
  <xsl:template match="t:List[@Type='Numerical']">
    <div class="list-container">
      <ol class="list">
        <xsl:call-template name="list-content"/>
      </ol>
    </div>
  </xsl:template>
  <xsl:template match="t:List[@Type='Bulleted']">
    <div class="list-container">
      <ul class="list">
        <xsl:call-template name="list-content" />
      </ul>
    </div>
  </xsl:template>

  <!-- List Content (including sublists) -->
  <xsl:template name="list-content">
    <xsl:for-each select="t:Item">
      <xsl:choose>
        <xsl:when test="t:List">
          <xsl:apply-templates select="." />
        </xsl:when>
        <xsl:otherwise>
          <li class="list-item">
            <xsl:apply-templates select="." />
          </li>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <!-- Formatted Text -->
  <xsl:template match="t:Bold">
    <strong>
      <xsl:apply-templates />
    </strong>
  </xsl:template>
  <xsl:template match="t:Italic">
    <em>
      <xsl:apply-templates />
    </em>
  </xsl:template>

  <xsl:template match="t:Heading">
    <div class="heading-container container">
      <xsl:element name="h{count(ancestor-or-self::t:Heading) + 1}">
        <xsl:attribute name="class">title heading-title</xsl:attribute>
        <xsl:attribute name="id">
          <xsl:value-of select="@Id" />
        </xsl:attribute>
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