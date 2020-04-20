<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output version="1.0" encoding="UTF-8" indent="no" />

    <!-- Filter classes out of documentation. -->
    <xsl:template name="filterClasses">
        <xsl:param name="cn" />
        
        <!-- Add class to this list to filter the class. -->
        <xsl:variable name="exclude">
            <a>IntoTheCode.Basic.Util.DotNetUtil</a>
            <a>IntoTheCode.Message.MessageRes</a>
        </xsl:variable>
        <xsl:if test="not(msxsl:node-set($exclude)/a/text()[contains(., $cn)])">
            <xsl:call-template name="class"><xsl:with-param name="cn" select="$cn"/></xsl:call-template>
        </xsl:if>
    </xsl:template>

    <!-- List of classes to put on top of documentation. -->
    <xsl:template name="classes">
        <xsl:param name="cn" />

        <!-- Add class to this list to put the class on top. -->
        <xsl:variable name="putOnTop">
            <a>IntoTheCode.CodeDocument</a>
            <a>IntoTheCode.CodeElement</a>
            <a>IntoTheCode.Read.Parser</a>
        </xsl:variable>

        <xsl:choose>
            <xsl:when test="$cn='top'">
                <!-- This writes classes on the top list -->
                <xsl:for-each select="/doc/members/member[substring-after(@name,'T:') = msxsl:node-set($putOnTop)/a]">
                    <xsl:variable name="cn2" select="substring-after(@name,'T:')"/>


                    <xsl:call-template name="class"><xsl:with-param name="cn" select="$cn2"/></xsl:call-template>
                </xsl:for-each>
            </xsl:when>
            <xsl:otherwise>
                <!-- This transforms all the classes not on top -->
                <xsl:if test="not(msxsl:node-set($putOnTop)/a/text()[contains(., $cn)])">
                    <xsl:call-template name="filterClasses"><xsl:with-param name="cn" select="$cn"/></xsl:call-template>
                </xsl:if>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <!-- Template for document. -->
    <xsl:template match="doc">
        <xsl:value-of select="'&#xd;&#xa;'"/>
        <doc>
            <!--<xsl:value-of select="concat('&#xd;&#xa;','    ')"/> --><!--Insert linebreak and indent-->
            <xsl:copy-of select="/doc/assembly" /> <!--Copy content of assembly element-->

            <members>

                <!-- First add classes on top. -->
                <xsl:call-template name="classes"><xsl:with-param name="cn" select="'top'"/></xsl:call-template>

                <!-- Then add all other classes (not in filter). -->
                <xsl:for-each select="members/member">
                    <xsl:variable name="className" select="substring-after(@name,'T:')"/>
                    <xsl:if test="contains(@name,'T:')">
                        <xsl:call-template name="classes"><xsl:with-param name="cn" select="$className"/></xsl:call-template>
                    </xsl:if>
                </xsl:for-each>
            </members>
        </doc>
    </xsl:template>

    <!-- Template for classes -->
    <xsl:template name="class">
        <xsl:param name="cn" />
        
        <!-- Copy (T)ype -->
        <xsl:copy-of select="/doc/members/member[@name = concat('T:',$cn)]"/>

        <!-- Add templates for F=field, P=property, M=methods. -->
        <xsl:apply-templates select="/doc/members/member[contains(@name, concat('F:',$cn,'.'))]"/>
        <xsl:apply-templates select="/doc/members/member[contains(@name, concat('P:',$cn,'.'))]"/>
        <xsl:apply-templates select="/doc/members/member[contains(@name, concat('M:',$cn,'.'))]"/>
    </xsl:template>

    <!-- Template for members F=field, P=property, M=method. -->
    <xsl:template match="member">
        <xsl:if test="not(./exclude)">
            <xsl:copy-of select="."/>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>