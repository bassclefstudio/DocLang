<xsl:stylesheet
        xmlns:t="https://bassclefstudio.dev/DocLang/v1/Base"
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        version="1.0">
    <xsl:output method="html" encoding="ASCII"/>
    <!-- Document Root -->
    <xsl:template match="/t:Document">
        <div class="toc-container">
            <h2 class="title toc-title">Table of Contents</h2>
            <xsl:for-each select="t:Content/t:Heading">
                <xsl:call-template name="toc-header" />
            </xsl:for-each>
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
</xsl:stylesheet>