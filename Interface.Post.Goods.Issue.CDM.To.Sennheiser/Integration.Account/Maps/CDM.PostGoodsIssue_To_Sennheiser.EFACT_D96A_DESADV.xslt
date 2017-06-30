<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns:ns0="http://linfox.com/Horizons/PGI/V1" xmlns:ns1="http://schemas.microsoft.com/BizTalk/EDI/EDIFACT/2006">
	<xsl:output omit-xml-declaration="yes" method="xml" version="1.0"/>
	<xsl:template match="/">
		<xsl:apply-templates select="ns0:PostGoodsIssue"/>
	</xsl:template>
	<xsl:template match="ns0:PostGoodsIssue">
		<xsl:variable name="connote" select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:ConNoteNumber"/>
		<xsl:variable name="cdate" select="substring(/ns0:PostGoodsIssue/ns0:MessageHeader/ns0:CreationDate, 0, 11)"/>
		<!-- Sales order number is received in CustPONbr -->
		<xsl:variable name="sonum" select="substring-before(/ns0:PostGoodsIssue/ns0:GIHeader/ns0:CustPONbr, '-')"/>
		<xsl:variable name="addref1" select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:AddnRefDetails1"/>
		<xsl:variable name="addref2" select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:AddnRefDetails2"/>
		<xsl:variable name="warloc" select="/ns0:PostGoodsIssue/ns0:MessageHeader/ns0:WarehouseLocationCode"/>
		<!-- Purchase order number is received in PrmCustShpOdrNbr --> 
    <!-- We are ignoring SalesOrderNumber field value which has Customer PO Number as this is not required in PGI EDIFACT-->
		<xsl:variable name="pcson" select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:PrmCustShpOdrNbr"/>
		<ns1:EFACT_D96A_DESADV>
			<UNH>
				<UNH1>
					<xsl:value-of select="/ns0:PostGoodsIssue/ns0:MessageHeader/ns0:ID"/>
				</UNH1>
				<UNH2>
					<UNH2.1>DESADV</UNH2.1>
					<UNH2.2>D</UNH2.2>
					<UNH2.3>96A</UNH2.3>
					<UNH2.4>UN</UNH2.4>
				</UNH2>
			</UNH>
			<ns1:BGM>
				<ns1:C002>
					<C00201>351</C00201>
					<C00204>
						<xsl:call-template name="substring-after-last">
							<xsl:with-param name="addref2" select="$addref2"/>
							<xsl:with-param name="delimiter" select="'BR'"/>
						</xsl:call-template>
					</C00204>
				</ns1:C002>
				<BGM02>
					<xsl:value-of select="translate($connote, 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz', '')"/>
				</BGM02>
			</ns1:BGM>
			<ns1:DTM>
				<ns1:C507>
					<C50701>137</C50701>
					<C50702>
						<xsl:value-of select="translate($cdate, '-', '')"/>
					</C50702>
					<C50703>102</C50703>
				</ns1:C507>
			</ns1:DTM>
			<ns1:MEA>
				<MEA01>AAD</MEA01>
				<ns1:C502>
					<C50201>AAE</C50201>
					<C50202>12</C50202>
					<C50203>LBR</C50203>
					<C50204>
						<xsl:value-of select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:TotalShipWeight"/>
					</C50204>
				</ns1:C502>
			</ns1:MEA>
			<ns1:RFFLoop1>
				<ns1:RFF>
					<ns1:C506>
						<C50601>ON</C50601>
						<C50602>
							<xsl:value-of select="$sonum"/>
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
							<xsl:value-of select="$addref1"/>
						</C50602>
						<C50603>2</C50603>
					</ns1:C506>
				</ns1:RFF>
			</ns1:RFFLoop1>
			<ns1:RFFLoop1>
				<ns1:RFF>
					<ns1:C506>
						<C50601>ACC</C50601>
						<C50602>
							<xsl:value-of select="$connote"/>
						</C50602>
						<C50603>3</C50603>
					</ns1:C506>
				</ns1:RFF>
			</ns1:RFFLoop1>
			<ns1:RFFLoop1>
				<ns1:RFF>
					<ns1:C506>
						<C50601>AAY</C50601>
						<C50602>
							<xsl:value-of select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:TransportProvider"/>
						</C50602>
					</ns1:C506>
				</ns1:RFF>
			</ns1:RFFLoop1>
			<ns1:NADLoop1>
				<ns1:NAD>
					<NAD01>DP</NAD01>
					<ns1:C082>
						<C08201>
							<xsl:value-of select="substring-before(substring-after($addref2, 'DP'), 'BY')"/>
						</C08201>
					</ns1:C082>
				</ns1:NAD>
			</ns1:NADLoop1>
			<ns1:NADLoop1>
				<ns1:NAD>
					<NAD01>BY</NAD01>
					<ns1:C082>
						<C08201>
							<xsl:value-of select="substring-before(substring-after($addref2, 'BY'), 'DS')"/>
						</C08201>
					</ns1:C082>
				</ns1:NAD>
			</ns1:NADLoop1>
			<ns1:NADLoop1>
				<xsl:choose>
					<xsl:when test="substring-after($addref2, 'DS')">
						<ns1:NAD>
							<NAD01>DC</NAD01>
							<ns1:C082>
								<C08201>
									<xsl:value-of select="translate(substring-after($addref2, 'DS'), 'BR', '' )"/>
								</C08201>
							</ns1:C082>
						</ns1:NAD>
					</xsl:when>
					<xsl:otherwise>
					</xsl:otherwise>
				</xsl:choose>
				<ns1:RFFLoop2>
					<ns1:RFF_2>
						<ns1:C506_2>
							<C50601>ZZZ</C50601>
							<C50602>
								<xsl:value-of select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:TrackingURL"/>
							</C50602>
						</ns1:C506_2>
					</ns1:RFF_2>
				</ns1:RFFLoop2>
			</ns1:NADLoop1>
			<xsl:for-each select="/ns0:PostGoodsIssue/ns0:GIItemLevel/ns0:GIItem">
				<xsl:variable name="linnum" select="./ns0:ShpOdrLineItemNo"/>
				<ns1:CPSLoop1>
					<ns1:CPS>
						<CPS01>1</CPS01>
					</ns1:CPS>
					<ns1:LINLoop1>
						<ns1:LIN>
							<LIN01>
								<xsl:value-of select="$linnum"/>
							</LIN01>
						</ns1:LIN>
						<ns1:PIA>
							<PIA01>1</PIA01>
							<ns1:C212_2>
								<xsl:variable name="matcod" select="./ns0:MaterialCode"/>
								<C21201>
									<xsl:value-of select="$matcod"/>
								</C21201>
								<C21202>MF</C21202>
							</ns1:C212_2>
						</ns1:PIA>
						<ns1:QTY_2>
							<ns1:C186_2>
								<C18601>12</C18601>
								<C18602>
									<xsl:value-of select="./ns0:PGIQuantity"/>
								</C18602>
								<C18603>
									<xsl:value-of select="./ns0:BaseUOM"/>
								</C18603>
							</ns1:C186_2>
						</ns1:QTY_2>
						<ns1:QTY_2>
							<ns1:C186_2>
								<C18601>21</C18601>
								<C18602>
									<xsl:value-of select="./ns0:OrderQuantity"/>
								</C18602>
								<C18603>
									<xsl:value-of select="./ns0:BaseUOM"/>
								</C18603>
							</ns1:C186_2>
						</ns1:QTY_2>
						<ns1:QTY_2>
							<ns1:C186_2>
								<C18601>185</C18601>
								<C18602>
									<xsl:value-of select="./ns0:BackOrderQuantity"/>
								</C18602>
								<C18603>
									<xsl:value-of select="./ns0:BaseUOM"/>
								</C18603>
							</ns1:C186_2>
						</ns1:QTY_2>
						<ns1:DLM_2>
							<ns1:C214_2>
								<C21404>
									<xsl:value-of select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:TransportProvider"/>
								</C21404>
							</ns1:C214_2>
						</ns1:DLM_2>
						<ns1:DTM_6>
							<ns1:C507_6>
								<C50701>186</C50701>
								<C50702>
									<xsl:value-of select="translate($cdate, '-', '')"/>
								</C50702>
								<C50703>102</C50703>
							</ns1:C507_6>
						</ns1:DTM_6>
						<ns1:RFFLoop3>
							<ns1:RFF_4>
								<ns1:C506_4>
									<C50601>ON</C50601>
									<C50602>
										<xsl:value-of select="$sonum"/>
									</C50602>
									<C50603>1</C50603>
								</ns1:C506_4>
							</ns1:RFF_4>
						</ns1:RFFLoop3>
						<ns1:RFFLoop3>
							<ns1:RFF_4>
								<ns1:C506_4>
									<C50601>ON</C50601>
									<C50602>
										<xsl:value-of select="$addref1"/>
									</C50602>
									<C50603>2</C50603>
								</ns1:C506_4>
							</ns1:RFF_4>
						</ns1:RFFLoop3>
						<ns1:RFFLoop3>
							<ns1:RFF_4>
								<ns1:C506_4>
									<C50601>DQ</C50601>
									<C50602>
										<xsl:value-of select="$pcson"/>
									</C50602>
									<C50603>1</C50603>
								</ns1:C506_4>
							</ns1:RFF_4>
						</ns1:RFFLoop3>
						<ns1:LOCLoop2>
							<ns1:LOC_4>
								<LOC01>ZZZ</LOC01>
								<ns1:C517_4>
									<C51701>
										<xsl:call-template name="StkCat">
											<xsl:with-param name="category">
												<xsl:value-of select="./ns0:StockCategoryType"/>
											</xsl:with-param>
										</xsl:call-template>
									</C51701>
									<C51702/>
									<C51703/>
									<C51704>
										<xsl:value-of select="./ns0:AddnRefDetails5"/>
									</C51704>
								</ns1:C517_4>
							</ns1:LOC_4>
						</ns1:LOCLoop2>
						<xsl:for-each select="/ns0:PostGoodsIssue/ns0:HandlingUnitDetails/ns0:HandlingUnitDetail/ns0:HULineItem[ns0:PGIQuantity != '0']">
							<xsl:variable name="lfxlinnum" select="./ns0:ShipOrderLineItemNo"/>
							<xsl:choose>
								<xsl:when test="$lfxlinnum = $linnum">
									<ns1:PCILoop2>
										<ns1:PCI_2>
											<PCI01>30</PCI01>
											<ns1:C210_2>
												<C21001>
													<xsl:value-of select="../ns0:HUNumber"/>
												</C21001>
												<C21002>
													<xsl:value-of select="../ns0:HUNumber"/>
												</C21002>
												<C21003>
													<xsl:value-of select="/ns0:PostGoodsIssue/ns0:GIHeader/ns0:TrackingURL"/>
												</C21003>
											</ns1:C210_2>
											<ns1:C827_2>
												<C82701>003</C82701>
											</ns1:C827_2>
										</ns1:PCI_2>
										<ns1:MEA_5>
											<MEA01>AAI</MEA01>
											<ns1:C502_5>
												<C50201>AAE</C50201>
												<C50202>12</C50202>
												<C50203>LBR</C50203>
												<C50204>
													<xsl:value-of select="../ns0:HUWeight"/>
												</C50204>
											</ns1:C502_5>
										</ns1:MEA_5>
										<ns1:QTY_6>
											<ns1:C186_6>
												<C18601>12</C18601>
												<C18602>
													<xsl:value-of select="./ns0:PGIQuantity"/>
												</C18602>
												<C18603>
													<xsl:value-of select="./ns0:UOM"/>
												</C18603>
											</ns1:C186_6>
										</ns1:QTY_6>
										<xsl:for-each select="./ns0:SerialNumberDetails/ns0:SerialNumber">
											<ns1:GINLoop2>
												<ns1:GIN_3>
													<GIN01>BN</GIN01>
													<ns1:C208_11>
														<C20801>
															<xsl:value-of select="."/>
														</C20801>
													</ns1:C208_11>
												</ns1:GIN_3>
											</ns1:GINLoop2>
										</xsl:for-each>
									</ns1:PCILoop2>
								</xsl:when>
								<xsl:otherwise>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:for-each>
					</ns1:LINLoop1>
				</ns1:CPSLoop1>
			</xsl:for-each>
			<ns1:CNT>
				<ns1:C270>
					<C27001>1</C27001>
					<C27002>1</C27002>
					<C27003>1</C27003>
				</ns1:C270>
			</ns1:CNT>
			<UNT>
				<UNT1>76</UNT1>
				<UNT2>
					<xsl:value-of select="/ns0:PostGoodsIssue/ns0:MessageHeader/ns0:ID"/>
				</UNT2>
			</UNT>
		</ns1:EFACT_D96A_DESADV>
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
	<xsl:template name="substring-after-last">
		<xsl:param name="addref2"/>
		<xsl:param name="delimiter"/>
		<xsl:choose>
			<xsl:when test="contains($addref2, $delimiter)">
				<xsl:call-template name="substring-after-last">
					<xsl:with-param name="addref2" select="substring-after($addref2, $delimiter)"/>
					<xsl:with-param name="delimiter" select="$delimiter"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$addref2"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="StkCat">
		<xsl:param name="category"/>
		<xsl:choose>
			<xsl:when test="$category=''">
				<xsl:value-of select="'AU01'"/>
			</xsl:when>
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
