<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var"
    exclude-result-prefixes="msxsl var s0 soapenv"
    version="1.0"
    xmlns:s0="http://linfoxintegration.com/horizon/postgoodsissue/1.0"
    xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:ns0="http://linfox.com/Horizons/PGI/V1"
    xmlns:map="http://linfoxintegration.com/horizon/mapping">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />

  <xsl:variable name="GLNCode">
    <xsl:value-of select="//s0:PostGoodsIssue/s0:MessageHeader/s0:GLNCode/text()" />
  </xsl:variable>

  <xsl:variable name="map:GLNCode">
    <xsl:call-template name="MapValue">
      <xsl:with-param name="category">GLN</xsl:with-param>
      <xsl:with-param name="value">
        <xsl:value-of select="$GLNCode" />
      </xsl:with-param>
    </xsl:call-template>
  </xsl:variable>

  <xsl:template match="/">
    <xsl:apply-templates select="/soapenv:Envelope/soapenv:Body/s0:PostGoodsIssue" />
  </xsl:template>

  <xsl:template match="*[local-name()='PostGoodsIssue']">
    <ns0:PostGoodsIssue>
      <ns0:MessageHeader>
        <ns0:ID>
          <xsl:value-of select="s0:MessageHeader/s0:ID/text()" />
        </ns0:ID>
        <ns0:CreationDate>
          <xsl:value-of select="s0:MessageHeader/s0:CreationDate/text()" />
        </ns0:CreationDate>
        <ns0:GLNCode>
          <xsl:value-of select="$map:GLNCode" />
        </ns0:GLNCode>
        <ns0:CustomerName>
          <xsl:value-of select="'PWS'" />
        </ns0:CustomerName>
        <!-- Map PWS Warehouse to CDM Warehouse -->
        <ns0:WarehouseLocationCode>
          <xsl:call-template name="MapValue">
            <xsl:with-param name="category">WHS</xsl:with-param>
            <xsl:with-param name="value" select="s0:MessageHeader/s0:WarehouseLocationCode/text()" />
          </xsl:call-template>
        </ns0:WarehouseLocationCode>

        <ns0:TestIndicator>
          <xsl:value-of select="s0:MessageHeader/s0:TestIndicator/text()" />
        </ns0:TestIndicator>
      </ns0:MessageHeader>
      <ns0:GIHeader>

        <ns0:LfxShpOdrNbr>
          <!-- Need to remove the PWS specific GLN Code from the start of the Linfox Ship Order Number -->
          <xsl:variable name="lfxShpOdrNbr">
            <!-- Check if the LfxShpOdrNbr start with the GLN Code, and remove it if so -->
            <xsl:call-template name="StripGLNCode">
              <xsl:with-param name="materialCode" select="s0:GIHeader/s0:LfxShpOdrNbr/text()" />
              <xsl:with-param name="glnCode" select="$GLNCode" />
            </xsl:call-template>
          </xsl:variable>

          <xsl:value-of select="$lfxShpOdrNbr" />

        </ns0:LfxShpOdrNbr>

        <ns0:PrmCustShpOdrNbr>
          <!-- Check if the PrmShpOdrNbr start with the GLN Code, and remove it if so -->
          <xsl:variable name="prmCustShpOdrNbr">

            <xsl:call-template name="StripGLNCode">
              <xsl:with-param name="materialCode" select="s0:GIHeader/s0:PrmCustShpOdrNbr/text()" />
              <xsl:with-param name="glnCode" select="$GLNCode" />
            </xsl:call-template>

          </xsl:variable>

          <xsl:value-of select="$prmCustShpOdrNbr" />

        </ns0:PrmCustShpOdrNbr>

        <ns0:SalesOrderNumber>
          <xsl:value-of select="s0:GIHeader/s0:SalesOrderNumber/text()" />
        </ns0:SalesOrderNumber>
        <xsl:if test="s0:GIHeader/s0:CustPONbr">
          <ns0:CustPONbr>
            <xsl:value-of select="s0:GIHeader/s0:CustPONbr/text()" />
          </ns0:CustPONbr>
        </xsl:if>
        <ns0:ConNoteNumber>
          <xsl:value-of select="s0:GIHeader/s0:ConNoteNumber/text()" />
        </ns0:ConNoteNumber>
        <ns0:TrackingURL>
          <xsl:value-of select="s0:GIHeader/s0:TrackingURL/text()" />
        </ns0:TrackingURL>
        <xsl:if test="s0:GIHeader/s0:AddnRefDetails1">
          <ns0:AddnRefDetails1>
            <xsl:value-of select="s0:GIHeader/s0:AddnRefDetails1/text()" />
          </ns0:AddnRefDetails1>
        </xsl:if>
        <xsl:if test="s0:GIHeader/s0:AddnRefDetails2">
          <ns0:AddnRefDetails2>
            <xsl:value-of select="s0:GIHeader/s0:AddnRefDetails2/text()" />
          </ns0:AddnRefDetails2>
        </xsl:if>
        <xsl:if test="s0:GIHeader/s0:AddnRefDetails3">
          <ns0:AddnRefDetails3>
            <xsl:value-of select="s0:GIHeader/s0:AddnRefDetails3/text()" />
          </ns0:AddnRefDetails3>
        </xsl:if>
        <ns0:TransportProvider>
          <xsl:value-of select="s0:GIHeader/s0:TransportProvider/text()" />
        </ns0:TransportProvider>
        <ns0:TransportProviderName>
          <xsl:value-of select="s0:GIHeader/s0:TransportProviderName/text()" />
        </ns0:TransportProviderName>
        <ns0:ShipSrvCode>
          <xsl:value-of select="s0:GIHeader/s0:ShipSrvCode/text()" />
        </ns0:ShipSrvCode>
        <ns0:TotalPalletsCount>
          <xsl:value-of select="s0:GIHeader/s0:TotalPalletsCount/text()" />
        </ns0:TotalPalletsCount>
        <ns0:TotalCaseCount>
          <xsl:value-of select="s0:GIHeader/s0:TotalCaseCount/text()" />
        </ns0:TotalCaseCount>
        <ns0:TotalShipWeight>
          <xsl:value-of select="s0:GIHeader/s0:TotalShipWeight/text()" />
        </ns0:TotalShipWeight>
        <ns0:TotalShipWeightUOM>
          <xsl:value-of select="s0:GIHeader/s0:TotalShipWeightUOM/text()" />
        </ns0:TotalShipWeightUOM>
      </ns0:GIHeader>
      <ns0:GIItemLevel>
        <xsl:for-each select="s0:GIItemLevel/s0:GIItem">
          <ns0:GIItem>
            <ns0:ShpOdrLineItemNo>
              <xsl:value-of select="s0:ShpOdrLineItemNo/text()" />
            </ns0:ShpOdrLineItemNo>
            <ns0:LfxShpOdrLineItemNo>
              <xsl:value-of select="s0:LfxShpOdrLineItemNo/text()" />
            </ns0:LfxShpOdrLineItemNo>
            <xsl:if test="s0:AddnRefDetails1">
              <ns0:AddnRefDetails1>
                <xsl:value-of select="s0:AddnRefDetails1/text()" />
              </ns0:AddnRefDetails1>
            </xsl:if>
            <xsl:if test="s0:AddnRefDetails2">
              <ns0:AddnRefDetails2>
                <xsl:value-of select="s0:AddnRefDetails2/text()" />
              </ns0:AddnRefDetails2>
            </xsl:if>
            <xsl:if test="s0:AddnRefDetails3">
              <ns0:AddnRefDetails3>
                <xsl:value-of select="s0:AddnRefDetails3/text()" />
              </ns0:AddnRefDetails3>
            </xsl:if>
            <xsl:if test="s0:AddnRefDetails4">
              <ns0:AddnRefDetails4>
                <xsl:value-of select="s0:AddnRefDetails4/text()" />
              </ns0:AddnRefDetails4>
            </xsl:if>
            <xsl:if test="s0:AddnRefDetails5">
              <ns0:AddnRefDetails5>
                <xsl:value-of select="s0:AddnRefDetails5/text()" />
              </ns0:AddnRefDetails5>
            </xsl:if>

            <!-- Need to remove the PWS specific GLN Code from the start of the Material Code -->
            <ns0:MaterialCode>
              <xsl:call-template name="StripGLNCode">
                <xsl:with-param name="materialCode" select="s0:MaterialCode/text()" />
                <xsl:with-param name="glnCode" select="$GLNCode" />
              </xsl:call-template>

            </ns0:MaterialCode>

            <xsl:if test="s0:GTINCode">
              <ns0:GTINCode>
                <xsl:value-of select="s0:GTINCode/text()" />
              </ns0:GTINCode>
            </xsl:if>
            <ns0:MaterialName>
              <xsl:value-of select="s0:MaterialName/text()" />
            </ns0:MaterialName>
            <ns0:OrderQuantity>
              <xsl:value-of select="s0:OrderQuantity/text()" />
            </ns0:OrderQuantity>
            <ns0:BaseUOM>
              <xsl:value-of select="s0:BaseUOM/text()" />
            </ns0:BaseUOM>
            <xsl:if test="s0:BackOrderQuantity">
              <ns0:BackOrderQuantity>
                <xsl:value-of select="s0:BackOrderQuantity/text()" />
              </ns0:BackOrderQuantity>
            </xsl:if>
            <ns0:StockCategoryType>
              <xsl:value-of select="s0:StockCategoryType/text()" />
            </ns0:StockCategoryType>
            <ns0:PGIQuantity>
              <xsl:value-of select="s0:PGIQuantity/text()" />
            </ns0:PGIQuantity>
            <ns0:BatchDetails>
              <xsl:for-each select="s0:BatchDetails">
                <xsl:for-each select="s0:Batch">
                  <ns0:Batch>
                    <ns0:BatchCode>
                      <xsl:value-of select="s0:BatchCode/text()" />
                    </ns0:BatchCode>
                    <xsl:if test="s0:SLEDorBBD">
                      <ns0:SLEDorBBD>
                        <xsl:value-of select="s0:SLEDorBBD/text()" />
                      </ns0:SLEDorBBD>
                    </xsl:if>
                    <ns0:Quantity>
                      <xsl:value-of select="s0:Quantity/text()" />
                    </ns0:Quantity>
                    <ns0:UOM>
                      <xsl:value-of select="s0:UOM/text()" />
                    </ns0:UOM>
                  </ns0:Batch>
                </xsl:for-each>
              </xsl:for-each>
            </ns0:BatchDetails>
            <ns0:SerialNumberDetails>
              <xsl:for-each select="s0:SerialNumberDetails">
                <xsl:for-each select="s0:SerialNumber">
                  <ns0:SerialNumber>
                    <xsl:value-of select="./text()" />
                  </ns0:SerialNumber>
                </xsl:for-each>
              </xsl:for-each>
            </ns0:SerialNumberDetails>
          </ns0:GIItem>
        </xsl:for-each>
      </ns0:GIItemLevel>
      <ns0:HandlingUnitDetails>
        <xsl:for-each select="s0:HandlingUnitDetails">
          <!-- Standard Orders -->
          <xsl:for-each select="s0:HandlingUnitDetail[s0:HULineItem/s0:PGIQuantity != '0']">
            <ns0:HandlingUnitDetail>
              <xsl:if test="s0:ParentHU">
                <ns0:ParentHU>
                  <xsl:value-of select="s0:ParentHU/text()" />
                </ns0:ParentHU>
              </xsl:if>
              <xsl:if test="s0:ParentHUWeight">
                <ns0:ParentHUWeight>
                  <xsl:value-of select="s0:ParentHUWeight/text()" />
                </ns0:ParentHUWeight>
              </xsl:if>
              <xsl:if test="s0:ParentHUType">
                <ns0:ParentHUType>
                  <xsl:value-of select="s0:ParentHUType/text()" />
                </ns0:ParentHUType>
              </xsl:if>
              <ns0:HUNumber>
                <xsl:value-of select="s0:HUNumber/text()" />
              </ns0:HUNumber>
              <ns0:HUType>
                <xsl:value-of select="s0:HUType/text()" />
              </ns0:HUType>
              <ns0:HUWeight>
                <xsl:value-of select="s0:HUWeight/text()" />
              </ns0:HUWeight>
              <xsl:if test="Cube">
                <Cube>
                  <xsl:value-of select="Cube/text()" />
                </Cube>
              </xsl:if>
              <xsl:if test="CubeUoM">
                <CubeUoM>
                  <xsl:value-of select="CubeUoM/text()" />
                </CubeUoM>
              </xsl:if>
              <ns0:WeightUOM>
                <xsl:value-of select="s0:WeightUOM/text()" />
              </ns0:WeightUOM>
              <xsl:for-each select="s0:HULineItem">
                <ns0:HULineItem>
                  <ns0:ShipOrderLineItemNo>
                    <xsl:value-of select="s0:ShipOrderLineItemNo/text()" />
                  </ns0:ShipOrderLineItemNo>
                  <ns0:LfxShpOdrLineItemNo>
                    <xsl:value-of select="s0:LfxShpOdrLineItemNo/text()" />
                  </ns0:LfxShpOdrLineItemNo>
                  <xsl:if test="s0:AddnRefDetails1">
                    <ns0:AddnRefDetails1>
                      <xsl:value-of select="s0:AddnRefDetails1/text()" />
                    </ns0:AddnRefDetails1>
                  </xsl:if>
                  <xsl:if test="s0:AddnRefDetails2">
                    <ns0:AddnRefDetails2>
                      <xsl:value-of select="s0:AddnRefDetails2/text()" />
                    </ns0:AddnRefDetails2>
                  </xsl:if>

                  <!-- Need to remove the PWS specific GLN Code from the start of the Material Code -->
                  <ns0:MaterialCode>

                    <xsl:call-template name="StripGLNCode">
                      <xsl:with-param name="materialCode" select="s0:MaterialCode/text()" />
                      <xsl:with-param name="glnCode" select="$GLNCode" />
                    </xsl:call-template>
                  </ns0:MaterialCode>

                  <ns0:StockCategoryType>
                    <xsl:value-of select="s0:StockCategoryType/text()" />
                  </ns0:StockCategoryType>

                  <ns0:PGIQuantity>
                    <xsl:value-of select="s0:PGIQuantity/text()" />
                  </ns0:PGIQuantity>
                  <ns0:UOM>
                    <xsl:value-of select="s0:UOM/text()" />
                  </ns0:UOM>
                  <ns0:BatchDetails>
                    <xsl:for-each select="s0:BatchDetails">
                      <xsl:for-each select="s0:Batch">
                        <ns0:Batch>
                          <ns0:BatchCode>
                            <xsl:value-of select="s0:BatchCode/text()" />
                          </ns0:BatchCode>
                          <xsl:if test="s0:SLEDorBBD">
                            <ns0:SLEDorBBD>
                              <xsl:value-of select="s0:SLEDorBBD/text()" />
                            </ns0:SLEDorBBD>
                          </xsl:if>
                          <ns0:Quantity>
                            <xsl:value-of select="s0:Quantity/text()" />
                          </ns0:Quantity>
                          <ns0:UOM>
                            <xsl:value-of select="s0:UOM/text()" />
                          </ns0:UOM>
                        </ns0:Batch>
                      </xsl:for-each>
                    </xsl:for-each>
                  </ns0:BatchDetails>
                  <ns0:SerialNumberDetails>
                    <xsl:for-each select="s0:SerialNumberDetails">
                      <xsl:for-each select="s0:SerialNumber">
                        <ns0:SerialNumber>
                          <xsl:value-of select="./text()" />
                        </ns0:SerialNumber>
                      </xsl:for-each>
                    </xsl:for-each>
                  </ns0:SerialNumberDetails>
                </ns0:HULineItem>
              </xsl:for-each>
            </ns0:HandlingUnitDetail>
          </xsl:for-each>
        </xsl:for-each>
        <!-- Short Pick Orders -->
        <xsl:for-each select="s0:GIItemLevel/s0:GIItem[s0:PGIQuantity = '0']">
          <ns0:HandlingUnitDetail>
            <ns0:ParentHU>
            </ns0:ParentHU>
            <ns0:ParentHUWeight>
            </ns0:ParentHUWeight>
            <ns0:ParentHUType>
            </ns0:ParentHUType>
            <ns0:HUNumber>
            </ns0:HUNumber>
            <ns0:HUType>
            </ns0:HUType>
            <ns0:HUWeight>
            </ns0:HUWeight>
            <ns0:WeightUOM>
            </ns0:WeightUOM>
            <ns0:HULineItem>
              <ns0:ShipOrderLineItemNo>
                <xsl:value-of select="s0:ShpOdrLineItemNo/text()" />
              </ns0:ShipOrderLineItemNo>
              <ns0:LfxShpOdrLineItemNo>
                <xsl:value-of select="s0:LfxShpOdrLineItemNo/text()" />
              </ns0:LfxShpOdrLineItemNo>

              <!-- Need to remove the PWS specific GLN Code from the start of the Material Code -->
              <ns0:MaterialCode>
                <xsl:call-template name="StripGLNCode">
                  <xsl:with-param name="materialCode" select="s0:MaterialCode/text()" />
                  <xsl:with-param name="glnCode" select="$GLNCode" />
                </xsl:call-template>
              </ns0:MaterialCode>

              <ns0:StockCategoryType>
              </ns0:StockCategoryType>

              <ns0:PGIQuantity>
                <xsl:value-of select="s0:PGIQuantity/text()" />
              </ns0:PGIQuantity>
              <ns0:UOM>
              </ns0:UOM>
              <ns0:BatchDetails>
              </ns0:BatchDetails>
              <ns0:SerialNumberDetails>
              </ns0:SerialNumberDetails>
            </ns0:HULineItem>

          </ns0:HandlingUnitDetail>
        </xsl:for-each>
      </ns0:HandlingUnitDetails>
    </ns0:PostGoodsIssue>
  </xsl:template>

  <xsl:template name="MapValue">
    <xsl:param name="category" />
    <xsl:param name="value" />
    <xsl:param name="default" />

    <xsl:choose>
      <!-- Belkin Suppplier Code -->
      <xsl:when test="$category='GLN' and $value='22868'">3106042055</xsl:when>
      <!-- Sennheiser Supplier Code -->
      <xsl:when test="$category='GLN' and $value='44156'">9400343000001</xsl:when>

      <xsl:when test="$category='WHS' and $value='NSW1'">NSW1</xsl:when>

      <xsl:otherwise>
        <!-- If we have a $default supplied, use it -->
        <xsl:if test="$default">
          <xsl:value-of select="$default" />
        </xsl:if>
        <!-- Otherwise throw an 'exception' -->
        <xsl:if test="not($default)">
          <xsl:message terminate="yes">
            <xsl:value-of select="concat('Error: MapValue(', $category, ',', $value, ') not mapped')" />
          </xsl:message>
        </xsl:if>

      </xsl:otherwise>

    </xsl:choose>
  </xsl:template>


  <xsl:template name="StripGLNCode">
    <xsl:param name="materialCode" />
    <xsl:param name="glnCode" />

    <xsl:variable name="cdmMaterialCode">
      <!-- Check if the Material Code start with the GLN Code, and remove it if so -->
      <xsl:if test="starts-with($materialCode, $glnCode)">
        <xsl:value-of select="substring-after($materialCode, $glnCode)" />
      </xsl:if>
      <xsl:if test="not(starts-with($materialCode, $glnCode))">
        <xsl:value-of select="$materialCode" />
      </xsl:if>
    </xsl:variable>
    <!-- Output the Material Code -->
    <xsl:value-of select="$cdmMaterialCode" />
  </xsl:template>

</xsl:stylesheet>