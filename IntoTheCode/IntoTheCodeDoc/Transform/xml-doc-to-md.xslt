<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="html" indent="no"/>
  <!--<xsl:variable name="myAssembly" select="'MoehlData.Basic.AbstractModel'"/>-->

  <xsl:template match="doc">---
Title: Classes
---
<xsl:comment> This is a generated file. Dont edit! </xsl:comment>

<xsl:call-template name="findClasses"/>
</xsl:template>

<!-- //// CLASS ////////////////////////////////// -->
<xsl:template name="class">
  <xsl:param name="ns" />
  <xsl:param name="cn" />
  <xsl:param name="fn" />
    
    <!-- Print class name as ancor -->

### <a><xsl:attribute name="name"><xsl:value-of select="$fn"/></xsl:attribute>Class: <xsl:value-of select="$cn"/></a>
(Namespace: <xsl:value-of select="$ns"/>)
<xsl:apply-templates select="remarks"/>
<xsl:apply-templates select="summary"/>
<xsl:call-template name="findMembers">
  <xsl:with-param name="ns" select="$ns"/>
  <xsl:with-param name="cn" select="$cn"/>
  <xsl:with-param name="fn" select="$fn"/>
  </xsl:call-template>
</xsl:template>

  <!-- //// PROPERTY ////////////////////////////////// -->
  <xsl:template name="classProp">
    <xsl:param name="ns" />
    <xsl:param name="cn" />
    <xsl:param name="fn" />
    <xsl:variable name="memberName" select="substring-after(@name,concat($fn,'.'))"/>
**Property: <xsl:value-of select="$memberName"/>**
<xsl:apply-templates select="summary"/>
  </xsl:template>
  
  <!-- //// METHOD ////////////////////////////////// -->
  <xsl:template name="classMethod">
    <xsl:param name="ns" />
    <xsl:param name="cn" />
    <xsl:param name="fn" />
    <xsl:variable name="methodWithParams" select="substring-after(@name,concat($fn,'.'))"/>
    <xsl:variable name="method" select="substring-before(concat($methodWithParams,'('),'(')"/>
    <!--<xsl:variable name="params" select="substring-after($methodWithParams, $method)"/>-->
      <xsl:variable name="params">
        <xsl:if test="$methodWithParams != $method"><xsl:value-of select="substring-after($methodWithParams, $method)"/></xsl:if>
        <xsl:if test="$methodWithParams = $method"><xsl:value-of select="'()'"/></xsl:if>
      </xsl:variable>

    <xsl:if test="$method = '#ctor'">
**Constructor<xsl:value-of select="$params"/>**
    </xsl:if>
    <xsl:if test="$method != '#ctor'">
**Function: <xsl:value-of select="$method"/> <xsl:value-of select="$params"/>**
    </xsl:if>
    <xsl:apply-templates select="param"/>
    <xsl:apply-templates select="returns"/>
    <xsl:apply-templates select="summary"/>
  </xsl:template>

  <!-- ////////////////////////////////////// -->

  <!-- //////////////////////////////////////////////// -->
  <xsl:template match="exception">
    <!-- Show the exceptions. --><i>* Exception: </i>(<xsl:value-of select="@cref"/>) <xsl:value-of select="."/><br/>
  </xsl:template>


  <!-- //// Summary ////////////////////////////////// -->
  <xsl:template match="summary">
    <xsl:if test=". != ''">
<br/>*<xsl:value-of select="normalize-space(.)"/>*
    </xsl:if>
  </xsl:template>

  <!-- //// REMARK, Inherids ////////////////////////////////// -->
  <xsl:template match="remarks">
    <xsl:variable name="rem" select="."/>
    <xsl:variable name="link" select="substring-after(see/@cref,'T:')"/>
    <xsl:if test="contains($rem,'Inherids')">
Inherits [<xsl:value-of select="$link"/>](#<xsl:value-of select="$link"/>)
    </xsl:if>
</xsl:template>

  <!-- //// Parameters ////////////////////////////////// -->
  <xsl:template match="param">
* Parameter <xsl:value-of select="@name"/>:<xsl:value-of select="' '"/> <xsl:value-of select="."/>
  </xsl:template>

  <!-- //// Returns ////////////////////////////////// -->
  <xsl:template match="returns">
* Returns: <xsl:value-of select="."/>
  </xsl:template>

  <!-- ////////////////////////////////////// -->
  
  <!-- Find all members for a class: <doc><members><member name=""> -->
  <xsl:template name="findMembers">
    <xsl:param name="ns" />
    <xsl:param name="cn" />
    <xsl:param name="fn" />

    <xsl:variable name="pName" select="concat('P:',$fn, '.')"/>
    <xsl:variable name="mName" select="concat('M:',$fn, '.')"/>

    <xsl:for-each select="../member">
      <xsl:variable name="memberName" select="@name"/>
      <xsl:if test="contains($memberName,$pName)">
        <xsl:call-template name="classProp">
          <xsl:with-param name="ns" select="$ns"/>
          <xsl:with-param name="cn" select="$cn"/>
          <xsl:with-param name="fn" select="$fn"/>
        </xsl:call-template>
      </xsl:if>
      <xsl:if test="contains($memberName,$mName)">
        <xsl:call-template name="classMethod">
          <xsl:with-param name="ns" select="$ns"/>
          <xsl:with-param name="cn" select="$cn"/>
          <xsl:with-param name="fn" select="$fn"/>
        </xsl:call-template>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <!-- Find all classes: <doc> - <members><member name="T:"> -->
  <xsl:template name="findClasses">
    <xsl:for-each select="members/member">
      <!--<xsl:for-each select="members/member[contains(@name, 'MoehlData.Basic.TreeGraph.TreeNode')]">-->
      <xsl:variable name="memberName" select="@name"/>
      <xsl:if test="contains($memberName,'T:')">
        <xsl:variable name="fullClassName" select="substring-after($memberName,'T:')"/>
        <xsl:call-template name="classNamespaceLoop">
          <xsl:with-param name="ns" select="''"/>
          <xsl:with-param name="cn" select="$fullClassName"/>
          <xsl:with-param name="fn" select="$fullClassName"/>
        </xsl:call-template>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

    <!-- This template extracts the classname from the qualified classname (full classname)
        and calls the 'class' template with namespace and classname. -->
    <xsl:template name="classNamespaceLoop">
    <xsl:param name="ns" />
    <xsl:param name="cn" />
    <xsl:param name="fn" />
    <xsl:choose><!--Call 'class' template-->
      <xsl:when test="not(contains($cn,'.'))">
        <xsl:call-template name="class">
          <xsl:with-param name="ns" select="substring-after($ns,'.')"/>
          <xsl:with-param name="cn" select="$cn"/>
          <xsl:with-param name="fn" select="$fn"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise><!--Make recursive call-->
        <xsl:variable name="ns1" select="concat(concat($ns,'.'),substring-before($cn,'.'))"/>
        <xsl:variable name="cn1" select="substring-after($cn,'.')"/>
        <xsl:call-template name="classNamespaceLoop">
          <xsl:with-param name="ns" select="$ns1"/>
          <xsl:with-param name="cn" select="$cn1"/>
          <xsl:with-param name="fn" select="$fn"/>
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>