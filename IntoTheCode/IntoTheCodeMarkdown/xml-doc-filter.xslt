<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output version="1.0" encoding="UTF-8" indent="no" />

  <!-- Filter documentation for component. -->

  <xsl:template match="doc">
    <xsl:value-of select="'&#xa;'"/>
    <doc>
      <xsl:value-of select="concat('&#xa;','    ')"/>
      <xsl:copy-of select="/doc/assembly" />
      <xsl:value-of select="concat('&#xa;','    ')"/>

      <members>
        <xsl:call-template name="class"><xsl:with-param name="cn" select="'IntoTheCode.Basic.TreeNode'"/></xsl:call-template>
        <xsl:call-template name="class"><xsl:with-param name="cn" select="'IntoTheCode.CodeDocument'"/></xsl:call-template>
        <xsl:call-template name="class"><xsl:with-param name="cn" select="'IntoTheCode.CodeElement'"/></xsl:call-template>
        <xsl:call-template name="class"><xsl:with-param name="cn" select="'IntoTheCode.CommentElement'"/></xsl:call-template>
        <xsl:call-template name="class"><xsl:with-param name="cn" select="'IntoTheCode.HardElement'"/></xsl:call-template>
        <xsl:call-template name="class"><xsl:with-param name="cn" select="'IntoTheCode.Read.Parser'"/></xsl:call-template>
        
        <xsl:value-of select="concat('&#xa;','    ')"/>
      </members>
      <xsl:value-of select="'&#xa;'"/>
    </doc>
  </xsl:template>

  <xsl:template name="class">
    <xsl:param name="cn" />
    <xsl:value-of select="concat('&#xa;','        ')"/>
    <xsl:copy-of select="members/member[@name = concat('T:',$cn)]"/>

    <xsl:apply-templates select="members/member[contains(@name, concat('M:',concat($cn,'.')))]"/>
    <xsl:apply-templates select="members/member[contains(@name, concat('P:',concat($cn,'.')))]"/>
  </xsl:template>

  <xsl:template match="member">
    <xsl:if test="not(./exclude)">
      <xsl:value-of select="concat('&#xa;','        ')"/>
      <xsl:copy-of select="."/>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>