Imports System.Xml.Linq

Public Class SchemaProvider
    Friend Shared Function GetSchemaByCode(ByVal schemaCode As String) As XElement
        schemaCode = schemaCode.ToUpper

        Select Case schemaCode
            Case "AA", "SCHEMAAA"
                Return <AA>
                           <field id="_assetCode" type="text" start="1" len="10" required="Y"/>
                           <field id="_assetSubCode" type="text" start="11" len="5"/>
                           <field id="_lookUp" type="text" start="1" len="10"/>
                           <field id="_descriptn" type="text" start="1" len="30"/>
                           <field id="_analT0" type="text" start="138" len="15"/>
                           <field id="_analT1" type="text" start="153" len="15"/>
                           <field id="_analT2" type="text" start="168" len="15"/>
                           <field id="_analT3" type="text" start="183" len="15"/>
                           <field id="_analT4" type="text" start="213" len="15"/>
                           <field id="_analT5" type="text" start="228" len="15"/>
                           <field id="_analT6" type="text" start="243" len="15"/>
                           <field id="_analT7" type="text" start="258" len="15"/>
                           <field id="_analT8" type="text" start="273" len="15"/>
                           <field id="_analT9" type="text" start="288" len="15"/>
                           <field id="_factor" type="text" start="31" len="10" regex="[0-9,\.,\,]+"/>
                       </AA>
            Case "AD", "SCHEMAAD"
                Return <AD>
                           <field id="_assetCode" type="text" start="1" len="10"/>
                           <field id="_page" type="text" start="11" len="5"/>
                           <field id="_descriptn1" type="text" start="1" len="30"/>
                           <field id="_descriptn2" type="text" start="31" len="30"/>
                           <field id="_descriptn3" type="text" start="61" len="30"/>
                           <field id="_descriptn4" type="text" start="91" len="30"/>
                           <field id="_serialNo" type="text" start="121" len="30"/>
                           <field id="_purchaseDate" type="SD" start="151" len="8"/>
                           <field id="_purchasePrice" type="text" start="160" len="15" regex="[0-9,\.,\.]*|^$"/>
                           <field id="_commissionDate" type="SD" start="175" len="8"/>
                           <field id="_insureValue" type="text" start="184" len="15" regex="[0-9,\.,\.]*|^$"/>
                           <field id="_insureDuePrd" type="text" start="199" len="3" regex="[0-9]{0,3}"/>
                           <field id="_insureDescr" type="text" start="202" len="15"/>
                           <field id="_decommisionDate" type="SD" start="217" len="8"/>
                           <field id="_disposalDate" type="SD" start="225" len="8"/>
                           <field id="_disposalPrice" type="text" start="234" len="15" regex="[0-9,\.,\.]*|^$"/>
                           <field id="_disposalRef" type="text" start="249" len="15"/>
                           <field id="_maintenanceFreq" type="text" start="264" len="5"/>
                           <field id="_maintenanceDesc" type="text" start="269" len="15"/>
                       </AD>
            Case "AFED", "SCHEMAAFED"
                Return <AFED>
                           <field id="_afeLedger" type="text" start="1" len="1"/>
                           <field id="_afeCode" type="text" start="2" len="2"/>
                           <field id="_fieldCode" type="text" start="4" len="2"/>
                           <field id="_wellCode" type="text" start="6" len="2"/>
                           <field id="_departmentCode" type="text" start="8" len="2"/>
                           <field id="_costElement" type="text" start="10" len="2"/>
                       </AFED>
            Case "BC", "SCHEMABC"
                Return <BC>
                           <field id="_accnt_From" type="text" start="1" len="10" required="Y"/>
                           <field id="_accnt_To" type="text" start="16" len="10"/>
                           <field id="_bud_Accnt" type="text" start="1" len="10"/>
                           <field id="_anal_1" type="text" start="16" len="2" regex="[A,T][0-9]|^$"/>
                           <field id="_anal_2" type="text" start="19" len="2" regex="[A,T][0-9]|^$"/>
                           <field id="_budget_Type" type="text" start="22" len="1" regex="[S,M]|^$"/>
                       </BC>
            Case "BD", "SCHEMABC"
                Return <BD>
                           <field id="_budgetCode" type="text" start="1" len="1" required="Y"/>
                           <field id="_descriptn" type="text" start="1" len="25" required="Y"/>
                           <field id="_dag" type="text" start="422" len="5"/>
                           <field id="_updated" type="text" start="1" len="8"/>
                       </BD>
            Case "BDIC", "SCHEMABDIC"
                Return <BDIC>
                           <field id="_currentPeriod" type="text" start="31" len="8"/>
                           <field id="_maxPeriod" type="text" start="39" len="3"/>
                           <field id="_fromPeriod" type="text" start="42" len="8"/>
                           <field id="_toPeriod" type="text" start="50" len="8"/>
                           <field id="_fromDate" type="text" start="58" len="10"/>
                           <field id="_toDate" type="text" start="68" len="10"/>

                           <field id="_convCode" type="text" start="245" len="2"/>
                           <field id="_integerQty" type="text" start="253" len="1" regex="^[Y]$|^\s*$"/>
                           <field id="_lockBalance" type="text" start="293" len="1" regex="^[Y]$|^\s*$"/>
                           <field id="_stkTakePerYear" type="text" start="256" len="3" regex="^[0-9,\.]*$|^\s*$"/>
                           <field id="_defaultStkTakeCycle" type="text" start="259" len="3" regex="^[0-9,\.]*$|^\s*$"/>
                           <field id="_stkTakeTolerance" type="text" start="262" len="5" regex="^[0-9,\.]*$|^\s*$"/>
                           <field id="_stkTakeApproval" type="text" start="267" len="1" regex="^[D,Y]$|^\s*$"/>
                           <field id="_allocationType" type="text" start="251" len="1" regex="^[F,L,E,\s]$|^\s*$"/>

                           <field id="_liT0" type="text" start="225" len="2"/>
                           <field id="_liT1" type="text" start="227" len="2"/>
                           <field id="_liT2" type="text" start="229" len="2"/>
                           <field id="_liT3" type="text" start="231" len="2"/>
                           <field id="_liT4" type="text" start="233" len="2"/>
                           <field id="_liT5" type="text" start="235" len="2"/>
                           <field id="_liT6" type="text" start="237" len="2"/>
                           <field id="_liT7" type="text" start="239" len="2"/>
                           <field id="_liT8" type="text" start="241" len="2"/>
                           <field id="_liT9" type="text" start="243" len="2"/>
                       </BDIC>
            Case "BS", "SCHEMABS"
                Return <BS>
                           <field id="_budAccnt" type="text" start="1" len="10"/>
                           <field id="_anal1" type="text" start="16" len="2"/>
                           <field id="_anal1Value" type="text" start="19" len="15"/>
                           <field id="_anal2" type="text" start="34" len="2"/>
                           <field id="_anal2Value" type="text" start="37" len="15"/>
                           <field id="_actual" type="text" start="1" len="19"/>
                           <field id="_budget" type="text" start="20" len="19"/>
                           <field id="_commitment" type="text" start="39" len="19"/>
                           <field id="_budgetType" type="text" start="58" len="1"/>
                       </BS>
            Case "CD", "SCHEMACD"
                Return <CD>
                           <field id="_calCode" type="text" start="1" len="5" required="Y"/>
                           <field id="_updated" type="SmartDate" start="1" len="8"/>
                           <field id="_descriptn" type="text" start="10" len="30" required="Y"/>
                           <field id="_shortDescriptn" type="text" start="41" len="15" required="Y"/>
                           <field id="_constValue" type="text" start="56" len="18" regex="[0-9,\,,\.]+|^\s*$"/>
                           <field id="_lookupTable" type="text" start="75" len="10"/>
                           <field id="_formular" type="text" start="86" len="50"/>
                       </CD>
            Case "DB", "SCHEMADB"
                Return <DB ClassName="DB" merge2Table="Y" DBTable="SSINSTAL">
                           <field id="_dbId" type="text" start="1" len="3" required="Y" regex="[A-Z0-1]"/>
                           <field id="_description" type="text" start="22" len="30" required="Y"/>
                           <field id="_dateFormat" type="text" start="52" len="1" required="Y"/>
                           <field id="_referenceFileDrive" type="text" start="53" len="10"/>
                           <field id="_ledgerFileDrive" type="text" start="63" len="10"/>
                           <field id="_salesOrderFileDrive" type="text" start="73" len="10"/>
                           <field id="_purchaseOrderFileDrive" type="text" start="83" len="10"/>
                           <field id="_inventoryFileDrive" type="text" start="93" len="10"/>
                           <field id="_backupFileDrive" type="text" start="500" len="100"/>
                           <field id="_printFileDrive" type="text" start="113" len="10"/>
                           <field id="_referenceCreated" type="text" start="123" len="1"/>
                           <field id="_ledgerCreated" type="text" start="124" len="1"/>
                           <field id="_journalHoldCreated" type="text" start="125" len="1"/>
                           <field id="_budgetCreated" type="text" start="126" len="1"/>
                           <field id="_archiveCreated" type="text" start="127" len="1"/>
                           <field id="_salesOrderCreated" type="text" start="128" len="1"/>
                           <field id="_salesHistoryCreated" type="text" start="129" len="1"/>
                           <field id="_purchaseOrderCreated" type="text" start="130" len="1"/>
                           <field id="_purchaseHistoryCreated" type="text" start="131" len="1"/>
                           <field id="_movementCreated" type="text" start="132" len="1"/>
                           <field id="_stockCreated" type="text" start="133" len="1"/>
                           <field id="_multiVolumeBackup" type="text" start="242" len="1"/>
                           <field id="_decimalPlaces" type="text" start="243" len="1"/>
                           <field id="_decimalSeparator" type="text" start="244" len="1" required="Y"/>
                           <field id="_thousandSeparator" type="text" start="245" len="1" required="Y"/>
                           <field id="_primaryBudget" type="text" start="246" len="1"/>
                           <field id="_commitmentLedger" type="text" start="247" len="1"/>
                           <field id="_dataAccessGroup" type="text" start="463" len="5"/>
                           <field id="_sunbusinessDecimalPlaces" type="text" start="468" len="1"/>
                       </DB>
            Case "ID", "SCHEMAID"
                Return <ID>
                           <field id="_itemCode" type="text" start="1" len="20" required="Y"/>
                           <field id="_addressCode" type="text" start="21" len="10"/>
                           <field id="_analCat" type="text" start="31" len="30" required="Y"/>
                           <field id="_analCode" type="text" start="61" len="1"/>
                           <field id="_recordNumber" type="text" start="43" len="1"/>
                           <field id="_descriptn" type="text" start="1" len="180"/>
                           <field id="_additionalCode" type="text" start="181" len="15"/>
                       </ID>
            Case "JD", "SCHEMAJD"
                Return <JD>
                           <field id="_journalType" type="text" start="1" len="5" required="Y"/>
                           <field id="_lookup" type="text" start="26" len="5"/>
                           <field id="_updated" type="SmartDate" start="1" len="8"/>
                           <field id="_journalName" type="text" start="1" len="25" required="Y"/>
                           <field id="_journalPresetCode" type="text" start="26" len="5"/>
                           <field id="_T0" type="text" start="31" len="2"/>
                           <field id="_T1" type="text" start="33" len="2"/>
                           <field id="_T2" type="text" start="35" len="2"/>
                           <field id="_T3" type="text" start="37" len="2"/>
                           <field id="_T4" type="text" start="39" len="2"/>
                           <field id="_T5" type="text" start="41" len="2"/>
                           <field id="_T6" type="text" start="43" len="2"/>
                           <field id="_T7" type="text" start="45" len="2"/>
                           <field id="_T8" type="text" start="47" len="2"/>
                           <field id="_T9" type="text" start="49" len="2"/>
                           <field id="_suppressAgeing" type="text" start="51" len="1"/>
                           <field id="_descriptionPerLine" type="text" start="52" len="1"/>
                           <field id="_allocationMarker" type="text" start="53" len="1"/>
                           <field id="_nextPeriodReversal" type="text" start="54" len="1"/>
                           <field id="_dueDateEntry" type="text" start="55" len="1"/>
                           <field id="_uniqueReference" type="text" start="56" len="1"/>
                           <field id="_createWithoutPause" type="text" start="57" len="1"/>
                           <field id="_conversion" type="text" start="58" len="1"/>
                           <field id="_assetEntry" type="text" start="59" len="1"/>
                           <field id="_suppressJournalEntry" type="text" start="60" len="1"/>
                           <field id="_suppressJournalImport" type="text" start="61" len="1"/>
                           <field id="_suppressAccountAllocation" type="text" start="62" len="1"/>
                           <field id="_sequenceCode" type="text" start="63" len="5"/>
                           <field id="_transactionSequenceNumber" type="text" start="68" len="1"/>
                           <field id="_inputDiscountAccount" type="text" start="69" len="10"/>
                           <field id="_outputDiscountAccount" type="text" start="83" len="10"/>
                           <field id="_discountTotalPercent" type="text" start="98" len="11"/>
                           <field id="_discountTotalValue" type="text" start="109" len="12"/>
                           <field id="_autoTaxIndicator" type="text" start="121" len="1"/>
                           <field id="_taxDebitIndicator" type="text" start="123" len="1"/>
                           <field id="_taxCreditIndicator" type="text" start="124" len="1"/>
                           <field id="_taxCreditorTradeIndicator" type="text" start="125" len="1"/>
                           <field id="_taxProfitOrLossIndicator" type="text" start="126" len="1"/>
                           <field id="_taxBalanceSheetIndicator" type="text" start="127" len="1"/>
                           <field id="_taxMemoIndicator" type="text" start="128" len="1"/>
                           <field id="_stopCreditIndicator" type="text" start="129" len="1"/>
                           <field id="_stopDebitIndicator" type="text" start="130" len="1"/>
                           <field id="_stopCreditorTradeIndicator" type="text" start="131" len="1"/>
                           <field id="_stopProfitOrLossIndicator" type="text" start="132" len="1"/>
                           <field id="_stopBalanceSheetIndicator" type="text" start="133" len="1"/>
                           <field id="_stopMemoIndicator" type="text" start="134" len="1"/>
                           <field id="_ioTaxIndicator" type="text" start="135" len="1"/>
                           <field id="_otherAmtBalOv" type="text" start="136" len="1"/>
                           <field id="_dag" type="text" start="442" len="5"/>
                           <field id="_reportDefinition" type="text" start="447" len="10"/>
                       </JD>
            Case "JP", "SCHEMAJP"
                Return <JP>
                           <field id="JrnalType" type="text" required="Y"/>
                           <field id="Line_No" type="text" required="Y" regex="[0-9]+"/>
                           <field id="Line_Prompt" type="text" start="1" len="25"/>
                           <field id="Base_Line_No" type="text" start="26" len="5"/>
                           <field id="Period" type="text" start="31" len="7"/>
                           <field id="TransDate" type="text" start="39" len="10"/>
                           <field id="Descriptn" type="text" start="59" len="25"/>
                           <field id="AccntCode" type="text" start="84" len="10"/>
                           <field id="AssetCode" type="text" start="100" len="10"/>
                           <field id="SubCode" type="text" start="110" len="5"/>
                           <field id="ConvCode" type="text" start="116" len="5"/>
                           <field id="OtherAmt" type="text" start="121" len="16"/>
                           <field id="Amount" type="text" start="156" len="16"/>
                           <field id="Force" type="text" start="324" len="1"/>
                           <field id="Online_Alloc" type="text" start="323" len="1"/>
                           <field id="DueDate" type="text" start="49" len="10"/>
                           <field id="AccntType1" type="text" start="99" len="1" regex="^[B,C,D,T,P,M,\*]$|^$"/>
                           <field id="AccntType2" type="text" start="325" len="1" regex="^[B,C,D,T,P,M,\*]$|^$"/>
                           <field id="V_I_D" type="text" start="115" len="1" regex="^[V,I,D,-]$|^$"/>
                           <field id="ConvRate" type="text" start="137" len="19"/>
                           <field id="D_C" type="text" start="172" len="1" regex="^[D,C,R,S]$|^$"/>
                           <field id="AnalT0" type="text" start="173" len="15"/>
                           <field id="AnalT1" type="text" start="188" len="15"/>
                           <field id="AnalT2" type="text" start="203" len="15"/>
                           <field id="AnalT3" type="text" start="218" len="15"/>
                           <field id="AnalT4" type="text" start="233" len="15"/>
                           <field id="AnalT5" type="text" start="248" len="15"/>
                           <field id="AnalT6" type="text" start="263" len="15"/>
                           <field id="AnalT7" type="text" start="278" len="15"/>
                           <field id="AnalT8" type="text" start="293" len="15"/>
                           <field id="AnalT9" type="text" start="308" len="15"/>
                       </JP>
            Case "LD", "SCHEMALD"
                Return <LD>
                           <field id="_currentPeriod" type="text" start="31" len="8"/>
                           <field id="_maxPeriod" type="text" start="39" len="3"/>
                           <field id="_fromPeriod" type="text" start="42" len="8"/>
                           <field id="_toPeriod" type="text" start="50" len="8"/>
                           <field id="_fromDate" type="text" start="58" len="10"/>
                           <field id="_toDate" type="text" start="68" len="10"/>
                           <field id="_forceJL" type="text" start="151" len="1"/>
                           <field id="_jLFormat" type="text" start="187" len="5"/>
                           <field id="_forceJS" type="text" start="152" len="1"/>
                           <field id="_nextAllocationReference" type="text" start="169" len="9" regex="^[0-9,\.,\,,\s]*$"/>
                           <field id="_baseAmountCode" type="text" start="132" len="3"/>
                           <field id="_convTolerance" type="text" start="158" len="5" regex="^[0-9]{0,2}[\.\,][0-9]{1,3}$|^[0-9]{0,2}$|^$"/>
                           <field id="_consolidateRate" type="text" start="121" len="11" regex="[0-9,\.,\,]$|^$"/>
                           <field id="_balanceBy" type="text" start="396" len="2"/>
                           <field id="_discountND" type="text" start="494" len="2" regex="^[T][0-9]$|^\s*$"/>
                           <field id="_sequenceND" type="text" start="181" len="2" regex="^[T][0-9]$|^\s*$"/>
                           <field id="_dayBookND" type="text" start="183" len="2" regex="^[T][0-9]$|^\s*$"/>
                           <field id="_roughMode" type="text" start="178" len="1" regex="^[0-2]$|^\s*$"/>
                           <field id="_taxND" type="text" start="179" len="2" regex="^[T][0-9]$|^\s*$"/>
                           <field id="_paymentTermND" type="text" start="102" len="2" regex="^[T][0-9]$|^\s*$"/>
                           <field id="_ratioCode" type="text" start="164" len="3"/>
                           <field id="_excludeFinalValue" type="text" start="196" len="1"/>
                       </LD>
            Case "LO", "SCHEMALO"
                Return <LO>
                           <field id="_locationCode" type="text" start="1" len="5" required="Y"/>
                           <field id="_updated" type="SmartDate" start="1" len="8"/>
                           <field id="_descriptn" type="text" start="1" len="30" required="Y"/>
                           <field id="_suppressStockTaking" type="text" start="43" len="1"/>
                       </LO>
            Case "MD", "SCHEMAMD"
                Return <MD>
                           <field id="_movementType" type="text" start="1" len="5" required="Y"/>
                           <field id="_descriptn" type="text" start="1" len="30" required="Y"/>
                           <field id="_rit" type="text" start="31" len="1" required="Y" regex="^[R,I,T]$"/>
                           <field id="_accntPerLine" type="text" start="56" len="1"/>
                           <field id="_assetCodePerLine" type="text" start="57" len="1"/>
                           <field id="_printDoc" type="text" start="32" len="1"/>
                           <field id="_docFormat" type="text" start="33" len="5"/>
                           <field id="_glInterface" type="text" start="38" len="5"/>
                           <field id="_reportCost" type="text" start="80" len="3" regex="^LAT$|^AVE$|^STD$"/>
                           <field id="_phBalance" type="text" start="43" len="1"/>
                           <field id="_orderBalance" type="text" start="44" len="1"/>
                           <field id="_receiptRef" type="text" start="45" len="1"/>
                           <field id="_refDesc" type="text" start="46" len="10"/>
                           <field id="_manualAlloc" type="text" start="78" len="1"/>
                           <field id="_auditDate" type="text" start="79" len="1"/>
                           <field id="_cost1" type="text" start="86" len="15"/>
                           <field id="_cost1Ctrl" type="text" start="83" len="1" regex="^[Y,Z,L,V,S,\s]$|^$"/>
                           <field id="_cost2" type="text" start="101" len="15"/>
                           <field id="_cost2Ctrl" type="text" start="84" len="1" regex="^[Z,V,\s]$|^$"/>
                           <field id="_cost3" type="text" start="116" len="15"/>
                           <field id="_cost3Ctrl" type="text" start="85" len="1" regex="^[Z,V,\s]$|^$"/>
                           <field id="_analM0" type="text" start="58" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM1" type="text" start="60" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM2" type="text" start="62" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM3" type="text" start="64" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM4" type="text" start="66" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM5" type="text" start="68" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM6" type="text" start="70" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM7" type="text" start="72" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM8" type="text" start="74" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_analM9" type="text" start="76" len="2" regex="^[M][0-9]$|^\s*$"/>
                           <field id="_explodeType" type="text" start="140" len="5"/>
                           <field id="_explodePosting" type="text" start="145" len="1" regex="^[P,H]$|^\s*$"/>
                           <field id="_reportDefinition" type="text" start="146" len="10"/>
                       </MD>
            Case "MP", "SCHEMAMP"
                Return <MP>
                           <field id="_mvmntType" type="text" required="Y"/>
                           <field id="_mvmntRef" type="text" start="1" len="15"/>
                           <field id="_period" type="text" start="16" len="8"/>
                           <field id="_location" type="text" start="24" len="10"/>
                           <field id="_toLocation" type="text" start="34" len="10"/>
                           <field id="_itemCode" type="text" start="44" len="20"/>
                           <field id="_mvmntDate" type="text" start="64" len="10"/>
                           <field id="_recptRef" type="text" start="74" len="15"/>
                           <field id="_quantity" type="text" start="89" len="15"/>
                           <field id="_cost1" type="text" start="104" len="15"/>
                           <field id="_cost2" type="text" start="119" len="15"/>
                           <field id="_cost3" type="text" start="134" len="15"/>
                           <field id="_stdValue" type="text" start="149" len="15"/>
                           <field id="_latValue" type="text" start="164" len="15"/>
                           <field id="_aveValue" type="text" start="179" len="15"/>
                           <field id="_auditDate" type="text" start="194" len="8"/>
                           <field id="_accntCode" type="text" start="202" len="15"/>
                           <field id="_assetCode" type="text" start="217" len="15"/>
                           <field id="_assetSub" type="text" start="232" len="5"/>
                           <field id="_analM0" type="text" start="237" len="15"/>
                           <field id="_analM1" type="text" start="252" len="15"/>
                           <field id="_analM2" type="text" start="267" len="15"/>
                           <field id="_analM3" type="text" start="282" len="15"/>
                           <field id="_analM4" type="text" start="297" len="15"/>
                           <field id="_analM5" type="text" start="312" len="15"/>
                           <field id="_analM6" type="text" start="327" len="15"/>
                           <field id="_analM7" type="text" start="342" len="15"/>
                           <field id="_analM8" type="text" start="357" len="15"/>
                           <field id="_analM9" type="text" start="372" len="15"/>
                           <field id="_toAnalM0" type="text" start="387" len="15"/>
                           <field id="_toAnalM1" type="text" start="402" len="15"/>
                           <field id="_toAnalM2" type="text" start="417" len="15"/>
                           <field id="_toAnalM3" type="text" start="432" len="15"/>
                           <field id="_toAnalM4" type="text" start="447" len="15"/>
                           <field id="_toAnalM5" type="text" start="462" len="15"/>
                           <field id="_toAnalM6" type="text" start="477" len="15"/>
                           <field id="_toAnalM7" type="text" start="492" len="15"/>
                           <field id="_toAnalM8" type="text" start="507" len="15"/>
                           <field id="_toAnalM9" type="text" start="522" len="15"/>
                           <field id="_tips" type="text" start="537" len="200"/>
                       </MP>
            Case "MV", "SCHEMAMV"
                Return <MV>
                           <field id="_value1Name" type="text" start="1" len="15"/>
                           <field id="_value1" type="text" start="16" len="3" regex="^STD$|^LAT$|^AVE$|^C1$|^C2$|^C3$|^\s*$"/>
                           <field id="_value2Name" type="text" start="19" len="15"/>
                           <field id="_value2" type="text" start="34" len="3" regex="^STD$|^LAT$|^AVE$|^C1$|^C2$|^C3$|^\s*$"/>
                           <field id="_value3Name" type="text" start="37" len="15"/>
                           <field id="_value3" type="text" start="52" len="3" regex="^STD$|^LAT$|^AVE$|^C1$|^C2$|^C3$|^\s*$"/>
                       </MV>
            Case "ND", "SCHEMAND"
                Return <ND>
                           <field id="_category" type="text" start="1" len="2" required="Y"/>
                           <field id="_subCategory" type="text" start="3" len="1"/>
                           <field id="_lookUp" type="text" start="1" len="10"/>
                           <field id="_description" type="text" start="1" len="20" required="Y"/>
                           <field id="_shortDesc" type="text" start="21" len="5" required="Y"/>
                           <field id="_mask" type="text" start="26" len="15" regex="[\*,\s]*"/>
                           <field id="_nonValidated" type="text" start="41" len="1"/>
                           <field id="_amendable" type="text" start="42" len="1"/>
                           <field id="_updated" type="text" start="1" len="10"/>
                       </ND>
            Case "PD", "SCHEMAPD"
                Return <PD>
                           <field id="_Trans_Type" type="text" start="1" len="5" required="Y"/>
                           <field id="_Descriptn" type="text" start="1" len="30" required="Y"/>
                           <field id="_Ord_Ret" type="text" start="31" len="1" required="Y" regex="^[O,R]$"/>
                           <field id="_Ord_Inv_Entry" type="text" start="32" len="1" required="Y" regex="^[O,I]$"/>
                           <field id="_Grn" type="text" start="33" len="1"/>
                           <field id="_Update_Stock" type="text" start="34" len="1" regex="^[Y,N,O]$|^$"/>
                           <field id="_Due_Date_Per_Line" type="text" start="35" len="1"/>
                           <field id="_Accnt_Per_Line" type="text" start="36" len="1" regex="^[Y,A,M,O]$|^$"/>
                           <field id="_Inv_Addr_Code" type="text" start="37" len="10"/>
                           <field id="_Manual_Ref" type="text" start="47" len="1"/>
                           <field id="_Asset_Per_Line" type="text" start="181" len="1" regex="^[Y,A,M,O]$|^$"/>
                           <field id="_Direct_Print" type="text" start="57" len="1" regex="^[Y,N,S]$|^$"/>
                           <field id="_Receipt_Per_Line" type="text" start="182" len="1"/>
                           <field id="_Audit_Date" type="text" start="183" len="1"/>
                           <field id="_PO_Prefix" type="text" start="53" len="4"/>
                           <field id="_PO_Format" type="text" start="58" len="5"/>
                           <field id="_Update_Cost" type="text" start="221" len="1" regex="^[Y,P,V]$|^$"/>
                           <field id="_LI_Commitment" type="text" start="63" len="5"/>
                           <field id="_LI_Inventory" type="text" start="68" len="5" regex=" "/>
                           <field id="_LI_Unappr_Inv" type="text" start="73" len="5"/>
                           <field id="_LI_Appr_Inv" type="text" start="78" len="5" regex=" "/>
                           <field id="_LI_Cost_Adj" type="text" start="210" len="5" regex=" "/>
                           <field id="_Override_BPC" type="text" start="154" len="1" regex=" "/>
                           <field id="_Cal_01" type="text" start="103" len="3"/>
                           <field id="_Cal_02" type="text" start="106" len="3"/>
                           <field id="_Cal_03" type="text" start="109" len="3"/>
                           <field id="_Cal_04" type="text" start="112" len="3"/>
                           <field id="_Cal_05" type="text" start="115" len="3"/>
                           <field id="_Cal_06" type="text" start="118" len="3"/>
                           <field id="_Cal_07" type="text" start="121" len="3"/>
                           <field id="_Cal_08" type="text" start="124" len="3"/>
                           <field id="_Cal_09" type="text" start="127" len="3"/>
                           <field id="_Cal_10" type="text" start="130" len="3"/>
                           <field id="_Cal_11" type="text" start="133" len="3"/>
                           <field id="_Cal_12" type="text" start="136" len="3"/>
                           <field id="_Cal_13" type="text" start="139" len="3"/>
                           <field id="_Cal_14" type="text" start="142" len="3"/>
                           <field id="_Cal_15" type="text" start="145" len="3"/>
                           <field id="_Cal_16" type="text" start="148" len="3"/>
                           <field id="_Cal_17" type="text" start="151" len="3"/>
                           <field id="_H_Anal_M0" type="text" start="83" len="1"/>
                           <field id="_H_Anal_M1" type="text" start="84" len="1"/>
                           <field id="_H_Anal_M2" type="text" start="85" len="1"/>
                           <field id="_H_Anal_M3" type="text" start="86" len="1"/>
                           <field id="_H_Anal_M4" type="text" start="87" len="1"/>
                           <field id="_H_Anal_M5" type="text" start="88" len="1"/>
                           <field id="_H_Anal_M6" type="text" start="89" len="1"/>
                           <field id="_H_Anal_M7" type="text" start="90" len="1"/>
                           <field id="_H_Anal_M8" type="text" start="91" len="1"/>
                           <field id="_H_Anal_M9" type="text" start="92" len="1"/>
                           <field id="_D_Anal_M0" type="text" start="93" len="1"/>
                           <field id="_D_Anal_M1" type="text" start="94" len="1"/>
                           <field id="_D_Anal_M2" type="text" start="95" len="1"/>
                           <field id="_D_Anal_M3" type="text" start="96" len="1"/>
                           <field id="_D_Anal_M4" type="text" start="97" len="1"/>
                           <field id="_D_Anal_M5" type="text" start="98" len="1"/>
                           <field id="_D_Anal_M6" type="text" start="99" len="1"/>
                           <field id="_D_Anal_M7" type="text" start="100" len="1"/>
                           <field id="_D_Anal_M8" type="text" start="101" len="1"/>
                           <field id="_D_Anal_M9" type="text" start="102" len="1"/>
                       </PD>
            Case "SD", "SCHEMASD"
                Return <SD>
                           <field id="_transType" type="text" start="1" len="5" required="Y"/>
                           <field id="_descriptn" type="text" start="1" len="30" required="Y"/>
                           <field id="_credDeb" type="text" start="31" len="1" required="Y" regex="^[C,D]$"/>
                           <field id="_updateStock" type="text" start="33" len="1" regex="^[Y,N]$|^$"/>
                           <field id="_manualNos" type="text" start="32" len="1" regex="^[Y,Q]$|^$"/>
                           <field id="_quotationExpiryDays" type="text" start="199" len="3" regex="^[0-9]{0,3}$|^$"/>
                           <field id="_enterDelDueDate" type="text" start="34" len="1" regex="^[Y]$|^$"/>
                           <field id="_enterPmtDate" type="text" start="191" len="1" regex="^[O,A,P,D,I]$|^$"/>
                           <field id="_accntPerLine" type="text" start="192" len="1" regex="^[Y,O,D,I]$|^$"/>
                           <field id="_priceBaseDate" type="text" start="205" len="1" regex="^[O,U]$|^$"/>
                           <field id="_docPrefix" type="text" start="45" len="4"/>
                           <field id="_invInterface" type="text" start="49" len="5"/>
                           <field id="_stkInterface" type="text" start="166" len="5"/>
                           <field id="_cal01" type="text" start="54" len="3"/>
                           <field id="_cal02" type="text" start="57" len="3"/>
                           <field id="_cal03" type="text" start="60" len="3"/>
                           <field id="_cal04" type="text" start="63" len="3"/>
                           <field id="_cal05" type="text" start="66" len="3"/>
                           <field id="_cal06" type="text" start="69" len="3"/>
                           <field id="_cal07" type="text" start="72" len="3"/>
                           <field id="_cal08" type="text" start="75" len="3"/>
                           <field id="_cal09" type="text" start="78" len="3"/>
                           <field id="_cal10" type="text" start="81" len="3"/>
                           <field id="_cal11" type="text" start="84" len="3"/>
                           <field id="_cal12" type="text" start="87" len="3"/>
                           <field id="_cal13" type="text" start="90" len="3"/>
                           <field id="_cal14" type="text" start="93" len="3"/>
                           <field id="_cal15" type="text" start="96" len="3"/>
                           <field id="_cal16" type="text" start="99" len="3"/>
                           <field id="_cal17" type="text" start="102" len="3"/>
                           <field id="_cal18" type="text" start="105" len="3"/>
                           <field id="_cal19" type="text" start="108" len="3"/>
                           <field id="_cal20" type="text" start="111" len="3"/>
                           <field id="_cal01Override" type="text" start="126" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal02Override" type="text" start="127" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal03Override" type="text" start="128" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal04Override" type="text" start="129" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal05Override" type="text" start="130" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal06Override" type="text" start="131" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal07Override" type="text" start="132" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal08Override" type="text" start="133" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal09Override" type="text" start="134" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal10Override" type="text" start="135" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal11Override" type="text" start="136" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal12Override" type="text" start="137" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal13Override" type="text" start="138" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal14Override" type="text" start="139" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal15Override" type="text" start="140" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal16Override" type="text" start="141" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal17Override" type="text" start="142" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal18Override" type="text" start="143" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal19Override" type="text" start="144" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_cal20Override" type="text" start="145" len="1" regex="^[Y,I]$|^$"/>
                           <field id="_creditBalValue" type="text" start="123" len="3"/>
                           <field id="_salesQtyCode" type="text" start="202" len="3"/>
                           <field id="_salesQtyCode2" type="text" start="252" len="3"/>
                           <field id="_salesUnitCode" type="text" start="206" len="3"/>
                           <field id="_stkQtyCode" type="text" start="209" len="3"/>
                           <field id="_dAnalM0" type="text" start="171" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM1" type="text" start="173" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM2" type="text" start="175" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM3" type="text" start="177" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM4" type="text" start="179" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM5" type="text" start="181" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM6" type="text" start="183" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM7" type="text" start="185" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM8" type="text" start="187" len="2" regex="^M[0-9]$|^$"/>
                           <field id="_dAnalM9" type="text" start="189" len="2" regex="^M[0-9]$|^$"/>
                       </SD>
            Case "TA", "SCHEMATA"
                Return <TA>
                           <field id="_taCode" type="text" start="1" len="10"/>
                           <field id="_lookUp" type="text" start="1" len="10"/>
                           <field id="_jrnalType1F" type="text" start="1" len="5"/>
                           <field id="_jrnalType1T" type="text" start="6" len="5"/>
                           <field id="_jrnalType2F" type="text" start="11" len="5"/>
                           <field id="_jrnalType2T" type="text" start="16" len="5"/>
                           <field id="_jrnalType3F" type="text" start="21" len="5"/>
                           <field id="_jrnalType3T" type="text" start="26" len="5"/>
                           <field id="_invHolder" type="text" start="31" len="2" regex="^TR|T[0-9]$"/>
                           <field id="_taxHolder" type="text" start="33" len="2" regex="^TR$|^T[0-9]$"/>
                           <field id="_gross1F" type="text" start="35" len="10"/>
                           <field id="_gross1T" type="text" start="45" len="10"/>
                           <field id="_gross2F" type="text" start="55" len="10"/>
                           <field id="_gross2T" type="text" start="65" len="10"/>
                           <field id="_gross3F" type="text" start="75" len="10"/>
                           <field id="_gross3T" type="text" start="85" len="10"/>
                           <field id="_gross4F" type="text" start="95" len="10"/>
                           <field id="_gross4T" type="text" start="105" len="10"/>
                           <field id="_gross5F" type="text" start="115" len="10"/>
                           <field id="_gross5T" type="text" start="125" len="10"/>
                           <field id="_net1F" type="text" start="135" len="10"/>
                           <field id="_net1T" type="text" start="145" len="10"/>
                           <field id="_net2F" type="text" start="155" len="10"/>
                           <field id="_net2T" type="text" start="165" len="10"/>
                           <field id="_net3F" type="text" start="175" len="10"/>
                           <field id="_net3T" type="text" start="185" len="10"/>
                           <field id="_net4F" type="text" start="195" len="10"/>
                           <field id="_net4T" type="text" start="205" len="10"/>
                           <field id="_tax1F" type="text" start="215" len="10"/>
                           <field id="_tax1T" type="text" start="225" len="10"/>
                           <field id="_tax2F" type="text" start="235" len="10"/>
                           <field id="_tax2T" type="text" start="245" len="10"/>
                           <field id="_tax3F" type="text" start="255" len="10"/>
                           <field id="_tax3T" type="text" start="265" len="10"/>
                           <field id="_tax4F" type="text" start="275" len="10"/>
                           <field id="_tax4T" type="text" start="285" len="10"/>
                       </TA>
            Case "DOC", "SCHEMADOC"
                Return <DOC ClassName="DOC" merge2Table="N" DBTable="dbo.pbs_DOC">
                           <field id="Id" type="int" start="1" len="4" isPK="Y" regex="[0-9]+"/>
                           <field id="Url" type="String" start="1" len="512" isPK="N" required="Y"/>
                           <field id="Title" type="String" start="1" len="256" isPK="N" required="Y"/>
                           <field id="Category" type="String" start="1" len="50" isPK="N" required="Y"/>
                           <field id="Subject" type="String" start="1" len="256" isPK="N"/>
                           <field id="Author" type="String" start="1" len="50" isPK="N"/>
                           <field id="Keywords" type="String" start="1" len="50" isPK="N"/>
                           <field id="Comments" type="String" start="1" len="512" isPK="N"/>
                       </DOC>
            Case "DL", "SCHEMADL"
                Return <DL ClassName="DL" merge2Table="N" DBTable="dbo.pbs_DOCLINK">
                           <field id="Id" type="int" start="1" len="4" isPK="Y"/>
                           <field id="DocId" type="int" start="1" len="4" isPK="N" required="Y"/>
                           <field id="TransType" type="String" start="1" len="5" isPK="N"/>
                           <field id="Reference" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD0" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD1" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD2" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD3" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD4" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD5" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD6" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD7" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD8" type="String" start="1" len="15" isPK="N"/>
                           <field id="AnalD9" type="String" start="1" len="15" isPK="N"/>
                           <field id="Updated" type="int" start="1" len="4" isPK="N"/>
                           <field id="UpdatedBy" type="String" start="1" len="3" isPK="N"/>
                       </DL>
            Case "DMD", "SCHEMADMD"
                Return <DMD ClassName="DMD" merge2Table="N" DBTable="dbo.pbs_RFMSC">
                           <field id="MonitoringLoc" type="String" start="1" len="50" isPK="N" required="Y"/>
                           <field id="StorageLoc" type="String" start="1" len="50" isPK="N" required="Y"/>
                           <field id="Overwrite" type="String" start="1" len="1" isPK="N"/>
                           <field id="ClearSource" type="String" start="1" len="1" isPK="N"/>
                       </DMD>
            Case Else
                Return <Empty></Empty>
        End Select
    End Function
End Class
         