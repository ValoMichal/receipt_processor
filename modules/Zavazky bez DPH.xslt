<!--   Trasnformuje zav. poukaz. bez DPH z formatu financnej spravy do formatu MoneyS3 zavazky. -->

<xsl:stylesheet version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
>
<xsl:output method="xml" indent="yes" encoding="UTF-8"/>

<xsl:template match="/receipts">
  <MoneyData>
    <SeznamZavazku>
      <!-- variable vytvara kvoli sortovaniu, ktore je neskor -->
      <xsl:variable name="receipts">
        <xsl:apply-templates/>
        <!-- <xsl:for-each select="receipt"></xsl:for-each> -->
      </xsl:variable>
      <!-- output -->
      <!-- <xsl:for-each select="exsl:node-set($receipts)/Zavazek"> -->
      <xsl:for-each select="msxsl:node-set($receipts)/Zavazek">
        <xsl:sort select="@dateTime"/>
        <xsl:copy-of select="."/>
      </xsl:for-each>
    </SeznamZavazku>
  </MoneyData>
</xsl:template>

<xsl:template match="/receipts/receipt">

  <!-- prevod datumov: '08.11.2022 19:18:19' => '2022-11-08' -->
  <!-- prevod datumov: '08.11.2022 23:18:19' => '2022-11-08 23:18:19' -->
  <xsl:variable name="createDateVar">
    <xsl:value-of select="concat(substring(createDate,7,4),'-',substring(createDate,4,2),'-',substring(createDate,1,2))"/>
  </xsl:variable>
  <xsl:variable name="issueDateVar">
    <xsl:value-of select="concat(substring(issueDate,7,4),'-',substring(issueDate,4,2),'-',substring(issueDate,1,2))"/>
  </xsl:variable>
  <xsl:variable name="dateTimeVar">
    <xsl:value-of select="concat($createDateVar,' ',substring(createDate,12,8))"/>
  </xsl:variable>    

  <Zavazek>
    <xsl:attribute name="dateTime"><xsl:value-of select="$dateTimeVar"/></xsl:attribute>
    <Vydej>1</Vydej>
    <DatUcPr><xsl:value-of select="$createDateVar"/></DatUcPr>
    <DatVyst><xsl:value-of select="$issueDateVar"/></DatVyst>
    <DatPlat><xsl:value-of select="$createDateVar"/></DatPlat>
    <DatPln><xsl:value-of select="$createDateVar"/></DatPln>
    <Doruceno><xsl:value-of select="$createDateVar"/></Doruceno>
    <Adresa>
      <xsl:apply-templates select="organization"/>
    </Adresa>
    <SSazba><xsl:value-of select="vatRateReduced"/></SSazba><!-- receipts/receipt/vatRateReduced -->
    <ZSazba><xsl:value-of select="vatRateBasic"/></ZSazba><!-- receipts/receipt/vatRateBasic -->
    <SouhrnDPH>
      <Zaklad0><xsl:value-of select="format-number(sum(freeTaxAmount),'0.##')"/></Zaklad0><!-- receipts/receipt/freeTaxAmount -->
      <Zaklad5><xsl:value-of select="format-number(sum(taxBaseReduced),'0.##')"/></Zaklad5><!-- receipts/receipt/taxBaseReduced -->
      <Zaklad22><xsl:value-of select="format-number(sum(taxBaseBasic),'0.##')"/></Zaklad22><!-- receipts/receipt/taxBaseBasic -->
      <DPH5><xsl:value-of select="format-number(sum(vatAmountReduced),'0.##')"/></DPH5><!-- receipts/receipt/vatAmountReduced -->
      <DPH22><xsl:value-of select="format-number(sum(vatAmountBasic),'0.##')"/></DPH22><!-- receipts/receipt/vatAmountBasic -->
    </SouhrnDPH>
    <Celkem><xsl:value-of select="totalPrice"/></Celkem><!-- receipts/receipt/totalPrice -->
    <ZjednD>1</ZjednD>
    <xsl:call-template name="CreateNazovPozn">
      <xsl:with-param name="nodeSet" select="items/item"/>
    </xsl:call-template>
    <!--<NazovDokladu>
      <xsl:value-of select="items/item[1]/name"/>
    </NazovDokladu>-->
    <SeznamNormPolozek>
      <xsl:apply-templates select="items/item"/>
    </SeznamNormPolozek>
  </Zavazek>
</xsl:template>

<xsl:template match="organization">
  <ObchNazev><xsl:value-of select="name"/></ObchNazev>
  <ObchAdresa>
    <Ulice><xsl:value-of select="concat(streetName,' ',buildingNumber)"/></Ulice><!-- receipts/receipt/organization/streetName receipts/receipt/organization/buildingNumber -->
    <Misto><xsl:value-of select="municipality"/></Misto><!-- receipts/receipt/organization/municipality -->
    <PSC><xsl:value-of select="postalCode"/></PSC><!-- receipts/receipt/organization/postalCode -->
    <Stat><xsl:value-of select="country"/></Stat><!-- receipts/receipt/organization/country -->
    <!-- KodStatu vypocita iba ak icDph bude pritomne a bude mat not empty hodnotu -->
    <xsl:if test="normalize-space(icDph) != ''">
      <KodStatu><xsl:value-of select="substring(icDph,1,2)"/></KodStatu><!-- receipts/receipt/organization/icDph - prvé 2 znaky. Vstup môže byť aj prázdny. Ak to je problém, tento riadok vynechaj. -->
    </xsl:if>
  </ObchAdresa>
  <ICO><xsl:value-of select="ico"/></ICO><!-- receipts/receipt/organization/ico -->
  <DIC><xsl:value-of select="icDph"/></DIC><!-- receipts/receipt/organization/icDph -->
  <PlatceDPH><xsl:value-of select="vatPayer"/></PlatceDPH><!-- receipts/receipt/organization/vatPayer -->
  <DICSK><xsl:value-of select="dic"/></DICSK><!-- receipts/receipt/organization/dic -->
</xsl:template>

<xsl:template match="items/item">
  <!-- vypocet jednotkovej ceny -->
  <xsl:variable name="cenaVar">
    <xsl:value-of select="price div quantity"/>
  </xsl:variable>
  <!-- zaokruhlovanie ceny na styri desatinne miesta -->
  <xsl:variable name="cenaVar2">
    <xsl:value-of select="format-number($cenaVar,'0.####')"/>
  </xsl:variable>  
  <NormPolozka>
    <Poradi><xsl:value-of select="position()"/></Poradi><!-- poradie položky vo vstupnom XML. Ak je to veľa roboty, vynechaj. -->
    <Popis><xsl:value-of select="name"/></Popis><!-- receipts/receipt/items/item/name -->
    <Cena><xsl:value-of select="$cenaVar2"/></Cena><!-- receipts/receipt/items/item/price / receipts/receipt/items/item/quantity format-number-->
    <CenaTyp>4</CenaTyp>
    <SazbaDPH><xsl:value-of select="vatRate"/></SazbaDPH><!-- receipts/receipt/items/item/vatRate -->
    <PocetMJ><xsl:value-of select="quantity"/></PocetMJ><!-- receipts/receipt/items/item/quantity -->  
  </NormPolozka>
</xsl:template>

<!-- vypocita nazov (element Popis) a poznamku (element Pozn)
  do popisu vloží názov položky s najvyššou cenou, do poznámky to isté + ďalšie veci
  pozn je v speci formate: receiptId<enter>name - price (vatRate%)<enter>...
  pocita to rekurzivne, postupne pridava do Pozn, aktualizuje Popis a odobera item z nodeSet -->
<xsl:template name="CreateNazovPozn">
  <xsl:param name="nodeSet"/>  <!-- obsahuje items/item -->
  <xsl:param name="theOneName" select="''" />
  <xsl:param name="theOnePrice" select="-10" />
  <xsl:param name="allTriples" select="receiptId" />
  <xsl:param name="separator" select="'&#xA;'"/>
  <xsl:choose>
    <xsl:when test="not($nodeSet)">
      <Popis><xsl:value-of select="$theOneName"/></Popis>
      <Pozn><xsl:value-of select="$allTriples"/></Pozn>
    </xsl:when>
    <xsl:otherwise>
        <xsl:variable name="nazov" select="$nodeSet[1]/name" />
        <xsl:variable name="price" select="$nodeSet[1]/price" />
        <xsl:variable name="vatRate" select="$nodeSet[1]/vatRate" />
        <xsl:variable name="triple" select="concat($nazov,' - ',$price,' (',$vatRate,'%)')" />
        <xsl:choose>
          <xsl:when test="$price > $theOnePrice">
            <xsl:call-template name="CreateNazovPozn">
              <xsl:with-param name="nodeSet" select="$nodeSet[position() > 1]"/>
              <xsl:with-param name="theOneName" select="$nazov"/>
              <xsl:with-param name="theOnePrice" select="$price"/>
              <xsl:with-param name="allTriples" select="concat($allTriples,$separator,$triple)"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="CreateNazovPozn">
              <xsl:with-param name="nodeSet" select="$nodeSet[position() > 1]"/>
              <xsl:with-param name="theOneName" select="$theOneName"/>
              <xsl:with-param name="theOnePrice" select="$theOnePrice"/>
              <xsl:with-param name="allTriples" select="concat($allTriples,$separator,$triple)"/>
            </xsl:call-template>  
          </xsl:otherwise>
        </xsl:choose>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

</xsl:stylesheet>