<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns:ns0="http://linfox.com/Horizons/PGR/V1" xmlns:ns1="http://schemas.microsoft.com/BizTalk/EDI/EDIFACT/2006">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0"/>
  <xsl:template match="/">
    <xsl:apply-templates select="ns0:PostGoodsReceipt"/>
  </xsl:template>
  <xsl:template match="ns0:PostGoodsReceipt">
    <xsl:variable name="cdatetime" select="substring(/ns0:PostGoodsReceipt/ns0:MessageHeader/ns0:CreationDate, 0, 11)"/>
    <xsl:variable name="grdatetime" select="substring(/ns0:PostGoodsReceipt/ns0:GRHeader/ns0:GRDate, 0, 11)"/>
    <xsl:variable name="wmsdnum" select="/ns0:PostGoodsReceipt/ns0:GRHeader/ns0:WMSDocNumber"/>
    <ns1:EFACT_D96A_RECADV xmlns:ns1="http://schemas.microsoft.com/BizTalk/EDI/EDIFACT/2006">
      <UNH>
        <UNH1>
          <xsl:value-of select="/ns0:PostGoodsReceipt/ns0:MessageHeader/ns0:ID"/>
        </UNH1>
        <UNH2>
          <UNH2.1>RECADV</UNH2.1>
          <UNH2.2>D</UNH2.2>
          <UNH2.3>96A</UNH2.3>
          <UNH2.4>UN</UNH2.4>
        </UNH2>
      </UNH>
      <ns1:BGM>
        <ns1:C002>
          <C00201>532</C00201>
          <C00204>
            <xsl:call-template name="MapValue">
              <xsl:with-param name="category">WHS</xsl:with-param>
              <xsl:with-param name="value" select="/ns0:PostGoodsReceipt/ns0:MessageHeader/ns0:WarehouseLocationCode"/>
            </xsl:call-template>
          </C00204>
        </ns1:C002>
        <BGM02>
          <xsl:value-of select="substring-before($wmsdnum, '-' )"/>
        </BGM02>
      </ns1:BGM>
      <ns1:DTM>
        <ns1:C507>
          <C50701>137</C50701>
          <C50702>
            <xsl:value-of select="translate($cdatetime, '-', '')"/>
          </C50702>
          <C50703>102</C50703>
        </ns1:C507>
      </ns1:DTM>
      <ns1:RFFLoop1>
        <ns1:RFF>
          <ns1:C506>
            <C50601>ON</C50601>
            <C50602>
              <xsl:value-of select="substring-before(/ns0:PostGoodsReceipt/ns0:GRHeader/ns0:PrmCustDocRefNo, '-')"/>
            </C50602>
            <C50603>1</C50603>
          </ns1:C506>
        </ns1:RFF>
      </ns1:RFFLoop1>
      <ns1:RFFLoop1>
        <ns1:RFF>
          <ns1:C506>
            <C50601>ON</C50601>
            <C50602>
              <xsl:value-of select="/ns0:PostGoodsReceipt/ns0:GRHeader/ns0:PrimCustDocType"/>
            </C50602>
            <C50603>2</C50603>
          </ns1:C506>
        </ns1:RFF>
      </ns1:RFFLoop1>
      <ns1:RFFLoop1>
        <ns1:RFF>
          <ns1:C506>
            <C50601>DQ</C50601>
            <C50602>
              <xsl:value-of select="substring-before($wmsdnum, '-' )"/>
            </C50602>
          </ns1:C506>
        </ns1:RFF>
        <ns1:DTM_2>
          <ns1:C507_2>
            <C50701>50</C50701>
            <C50702>
              <xsl:value-of select="translate($grdatetime, '-', '')"/>
            </C50702>
            <C50703>102</C50703>
          </ns1:C507_2>
        </ns1:DTM_2>
      </ns1:RFFLoop1>
      <ns1:NADLoop1>
        <ns1:NAD>
          <NAD01>SU</NAD01>
          <ns1:C082>
            <C08201>
              <xsl:value-of select="/ns0:PostGoodsReceipt/ns0:GRItemLevel/ns0:GRItem/ns0:AddnRefNumber3"/>
            </C08201>
          </ns1:C082>
        </ns1:NAD>
      </ns1:NADLoop1>
      <ns1:NADLoop1>
        <ns1:NAD>
          <NAD01>ST</NAD01>
          <ns1:C082>
            <C08201>
              <xsl:value-of select="/ns0:PostGoodsReceipt/ns0:GRItemLevel/ns0:GRItem/ns0:AddnRefNumber4"/>
            </C08201>
          </ns1:C082>
        </ns1:NAD>
      </ns1:NADLoop1>
      <ns1:CPSLoop1>
        <ns1:CPS>
          <CPS01>1</CPS01>
        </ns1:CPS>
        <xsl:for-each select="/ns0:PostGoodsReceipt/ns0:GRItemLevel/ns0:GRItem">
          <ns1:LINLoop1>
            <ns1:LIN>
              <LIN01>
                <xsl:value-of select="./ns0:PrmCustOrderLineItmNumber"/>
              </LIN01>
              <ns1:C212>
                <xsl:variable name="matcod" select="./ns0:MaterialCode"/>
                <C21201>
                  <xsl:value-of select="$matcod"/>
                </C21201>
                <C21203>ZZZ</C21203>
              </ns1:C212>
            </ns1:LIN>
            <ns1:QTY>
              <ns1:C186>
                <C18601>21</C18601>
                <C18602>
                  <xsl:value-of select="./ns0:AddnRefNumber5"/>
                </C18602>
                <C18603>
                  <xsl:value-of select="./ns0:StockCategory/ns0:QuantityUOM"/>
                </C18603>
              </ns1:C186>
            </ns1:QTY>
            <ns1:QTY>
              <ns1:C186>
                <C18601>48</C18601>
                <C18602>
                  <xsl:value-of select="./ns0:StockCategory/ns0:Quantity"/>
                </C18602>
                <C18603>
                  <xsl:value-of select="./ns0:StockCategory/ns0:QuantityUOM"/>
                </C18603>
              </ns1:C186>
            </ns1:QTY>
            <ns1:DOCLoop2>
              <ns1:DOC_2>
                <ns1:C002_3/>
                <ns1:C503_2>
                  <C50301>
                    <xsl:call-template name="MapValue">
                      <xsl:with-param name="category">WHS</xsl:with-param>
                      <xsl:with-param name="value" select="/ns0:PostGoodsReceipt/ns0:MessageHeader/ns0:WarehouseLocationCode"/>
                    </xsl:call-template>
                  </C50301>
                  <C50302>20</C50302>
                  <C50303>
                    <xsl:call-template name="StkCat">
                      <xsl:with-param name="category">
                        <xsl:value-of select="/ns0:PostGoodsReceipt/ns0:HandlingUnitDetails/ns0:HandlingUnit/ns0:HULine/ns0:StockCategory/ns0:StockCategoryType"/>
                      </xsl:with-param>
                    </xsl:call-template>
                  </C50303>
                </ns1:C503_2>
              </ns1:DOC_2>
            </ns1:DOCLoop2>
          </ns1:LINLoop1>
        </xsl:for-each>
      </ns1:CPSLoop1>
    </ns1:EFACT_D96A_RECADV>
  </xsl:template>
  <xsl:template name="MapValue">
    <xsl:param name="category"/>
    <xsl:param name="value"/>
    <xsl:param name="default"/>
    <xsl:choose>
      <xsl:when test="$category='WHS' and $value='NSW1'">AU001</xsl:when>
      <xsl:otherwise>
        <!-- If we have a $default supplied, use it -->
        <xsl:if test="$default">
          <xsl:value-of select="$default"/>
        </xsl:if>
        <!-- Otherwise throw an 'exception' -->
        <xsl:if test="not($default)">
          <xsl:message terminate="yes">
            <xsl:value-of select="concat('Error: MapValue(', $category, ',', $value, ') not mapped')"/>
          </xsl:message>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="StkCat">
    <xsl:param name="category"/>
    <xsl:choose>
      <xsl:when test="$category='02'">
        <xsl:value-of select="'AU01'"/>
      </xsl:when>
      <xsl:when test="$category='33'">
        <xsl:value-of select="'AU0117'"/>
      </xsl:when>
      <xsl:when test="$category='34'">
        <xsl:value-of select="'AU0121'"/>
      </xsl:when>
      <xsl:when test="$category='22'">
        <xsl:value-of select="'AU0150'"/>
      </xsl:when>
      <xsl:when test="$category='14'">
        <xsl:value-of select="'AU0152'"/>
      </xsl:when>
      <xsl:when test="$category='83'">
        <xsl:value-of select="'AU0153'"/>
      </xsl:when>
      <xsl:when test="$category='25'">
        <xsl:value-of select="'AU01R'"/>
      </xsl:when>
      <xsl:when test="$category='50'">
        <xsl:value-of select="'AU06B'"/>
      </xsl:when>
      <xsl:when test="$category='51'">
        <xsl:value-of select="'AU06C'"/>
      </xsl:when>
      <xsl:when test="$category='52'">
        <xsl:value-of select="'AU06D'"/>
      </xsl:when>
      <xsl:otherwise>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
