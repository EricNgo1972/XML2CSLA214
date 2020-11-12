Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions

Public Class CSLABuilder

    Public XMLtext As System.Xml.Linq.XDocument = <?xml version="1.0"?>
                                                  <schema nameSpace="PS" ClassName="OD1" DBTable="pbs_RFMSC" merge2Table="Y">
                                                      <field id="_liCode" type="String" len="5" regex="A-Z" isPK="Y"/>
                                                      <field id="_lineNo" type="String" len="3" isPK="Y"/>
                                                      <field id="_lookUp" type="String" len="10"/>
                                                      <field id="_data" type="string"/>
                                                      <field id="_updated" type="SmartDate"/>
                                                  </schema>

    Public backedField As Boolean = True
    Public _keyCount As Integer
    Protected _objectClass As String = "Test"
    Private _merge2Table As Boolean = False
    Protected _DBTable As String = "Test"
    Private _nameSpace As String = ""

    Private _pkKeys As New List(Of XElement)
    Private _fields As New List(Of XElement)

    Private Sub setFactoryParameters()
        Dim behaviour = (From sc In XMLtext...<schema>).Single

        _DBTable = behaviour.@DBTable
        _objectClass = behaviour.@ClassName
        _merge2Table = If(behaviour.@merge2Table Is Nothing OrElse behaviour.@merge2Table.Trim.ToUpper <> "Y", False, True)
        _nameSpace = behaviour.@nameSpace

        Dim pkFields = From f In XMLtext...<Column> _
                     Where f.@IsPrimaryKey IsNot Nothing AndAlso f.@IsPrimaryKey.Trim.ToUpper = "Y"

        _keyCount = pkFields.Count
        If _keyCount >= 1 Then _pkKeys = pkFields.ToList

        Dim Fields = From f In XMLtext...<Column> _
        Where (f.@IsPrimaryKey Is Nothing OrElse f.@IsPrimaryKey.Trim.ToUpper <> "Y")
        _fields = Fields.ToList

    End Sub

    Public Sub LoadXMLFile(ByVal XMLFileName As String)
        XMLtext = XDocument.Load(XMLFileName)
        setFactoryParameters()
    End Sub

    Public Sub LoadXmlString(ByVal _XmlString As String)
        '  Dim sr As StringReader = New StringReader(_XmlString)
        ' XMLtext = XDocument.Load(sr)
        XMLtext = XDocument.Parse(_XmlString)
        setFactoryParameters()
    End Sub

    Public Sub New()
        setFactoryParameters()
    End Sub

    Public Sub New(ByVal _XDocument As XDocument)
        XMLtext = _XDocument
        setFactoryParameters()
    End Sub

    Public Sub New(ByVal _XmlString As String)
        Dim sr As StringReader = New StringReader(_XmlString)
        XMLtext = XDocument.Load(sr)
        setFactoryParameters()
    End Sub

    Public Sub New(ByVal uri As String, ByVal dummy As Boolean)
        XMLtext = XDocument.Load(uri)
        setFactoryParameters()
    End Sub

    Public Function Build_Properties() As String

        Dim retString As String = String.Empty

        Dim fields = XMLtext...<Column>

        For Each node In fields
            BuildProperty(node, retString)
        Next

        If _merge2Table Then
            retString += System.Environment.NewLine + " private _data as String = String.empty"
        End If

        Return retString
    End Function

    Public Function Build_InfoProperties() As String

        Dim retString As String = String.Empty

        Dim fields = XMLtext...<Column>

        For Each node In fields
            BuildInfoProperty(node, retString)
        Next

        Return retString
    End Function

    Public Function Build_IDParsing() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <IDSection>
                                           <Code>
    Protected Overrides Function GetIdValue() As Object
        Return [ID_Array]
    End Function
    Public Function MyID() As String
        Return [ID_Array]
    End Function
      [Combine_DB_Keys_Block]
    Public Function CompareTo(ByVal ID as String) As Integer
        [ParsingID_Block] 
        [Compare_Block]
        Return 0
    End Function  
                                      </Code>
                                       </IDSection>

        Dim theCode As New StringBuilder

        Dim IDSection = xmlTemplate...<IDSection>
        For Each codeElement In IDSection
            theCode.AppendLine(codeElement.<Code>.Value)
        Next
        theCode.Replace("[ParsingID_Block]", ParsingID_Block)
        theCode.Replace("[Compare_Block]", Compare_Block)
        theCode.Replace("[ID_Array]", ID_Array)
        theCode.Replace("[Combine_DB_Keys_Block]", Combine_DB_Keys_Block)
        theCode.Replace("[objectClass]", _objectClass)
        Return theCode.ToString
    End Function

    Private Function Compare_Block() As String
        Dim _Compare_Block As New StringBuilder
        For Each node In _pkKeys
            _Compare_Block.AppendFormat("If _{0} < p{0} Then Return -1 {1}", node.@Name, System.Environment.NewLine)
            _Compare_Block.AppendFormat("If _{0} > p{0} Then Return 1 {1}", node.@Name, System.Environment.NewLine)
        Next
        _Compare_Block.AppendFormat("Return 0")
        Return _Compare_Block.ToString
    End Function

    Private Function ParsingID_Block() As String
        Dim _ParsingID_Block As New StringBuilder
        If _keyCount > 1 Then
            _ParsingID_Block.AppendLine(" Dim m As MatchCollection = Regex.Matches(ID, spc.Helper.pbsRegex.AlphaNumericExt)")
            For i = 0 To _pkKeys.Count - 1
                _ParsingID_Block.AppendFormat("Dim p{0} as {1} = m({2}).Value{3}", _pkKeys(i).@Name, _pkKeys(i).@Type, i, System.Environment.NewLine)
            Next
        ElseIf _keyCount = 1 Then
            _ParsingID_Block.AppendFormat("Dim p{0} as {1} = id.toString{2}", _pkKeys(0).@Name, _pkKeys(0).@Type, System.Environment.NewLine)
        End If

        Return _ParsingID_Block.ToString

    End Function

    Private Function ID_Array() As String

        Dim fortmatStr As String = String.Empty
        Dim varArr As String = String.Empty

        If _keyCount = 1 Then Return "_" & _pkKeys(0).@Name

        For i = 0 To _pkKeys.Count - 1
            fortmatStr += "{" & i & "}:"
            varArr += ", _" & _pkKeys(i).@Name
        Next
        fortmatStr = System.Text.RegularExpressions.Regex.Replace(fortmatStr, ":$", String.Empty)

        Return String.Format("String.Format(""{0}""{1})", fortmatStr, varArr)

    End Function

    Private Function ID_pArray() As String

        Dim fortmatStr As String = String.Empty
        Dim varArr As String = String.Empty
        For i = 0 To _pkKeys.Count - 1
            fortmatStr += "{" & i & "}:"
            varArr += ",p" & _pkKeys(i).@Name
        Next
        fortmatStr = System.Text.RegularExpressions.Regex.Replace(fortmatStr, ":$", String.Empty)

        Return String.Format("String.Format(""{0}""{1})", fortmatStr, varArr)

    End Function

    Private Function Combine_DB_Keys_Block() As String
        Dim _Combine_DB_Keys As New StringBuilder

        If Not _merge2Table Then Return String.Empty

        _Combine_DB_Keys.AppendLine("Private Function DB_KEYS() As String")
        _Combine_DB_Keys.AppendLine(" Dim pkfield As String = String.Empty")
        For Each node In _pkKeys
            _Combine_DB_Keys.AppendFormat("ClassSchema(Of [objectClass]).SetSubData(""{0}"", _{0}, pkfield){1}", node.@Name, System.Environment.NewLine)
        Next
        _Combine_DB_Keys.AppendLine("Return pkfield")
        _Combine_DB_Keys.AppendLine("End Function")
        Return _Combine_DB_Keys.ToString

    End Function

    Public Function Build_ValidationRules() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Validation>
                                           <Code>
    Protected Overrides Sub AddBusinessRules()

        For Each _field As ClassField In ClassSchema(Of [objectClass])._fieldList
            If _field.Required Then
                ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, _field.FieldName)
            End If
            If Not String.IsNullOrEmpty(_field.RegexPattern) Then
                ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.RegExMatch, New RegExRuleArgs(_field.FieldName, _field.RegexPattern))
            End If
        Next

    End Sub
                                      </Code>
                                       </Validation>

        Dim theCode As New StringBuilder

        Dim validation = xmlTemplate...<Validation>
        For Each codeElement In validation
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[objectClass]", _objectClass)

        Return theCode.ToString
    End Function

    Public Function Build_AuthorizationRules() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Authorization>
                                           <Code>
    Protected Overrides Sub AddAuthorizationRules()
        'TODO: Define authorization rules in OD
    [PropertiesAuthorizationBlock]
        'AuthorizationRules.AllowRead("UserId", "ODReadGroup")
    End Sub

    Public Shared Function CanGetObject() As Boolean
        Return Authorizator.Allowed(GetType([objectName]).ToString)
    End Function

    Public Shared Function CanAddObject() As Boolean
        Return Authorizator.Allowed(String.Format("{0}.{1}", GetType([objectName]).ToString, Action._Create))
    End Function

    Public Shared Function CanEditObject() As Boolean
        Return Authorizator.Allowed(String.Format("{0}.{1}", GetType([objectName]).ToString, Action._Amend))
    End Function

    Public Shared Function CanDeleteObject() As Boolean
          Return Authorizator.Allowed(String.Format("{0}.{1}", GetType([objectName]).ToString, Action._Delete))
    End Function </Code>
                                       </Authorization>

        Dim theCode As New StringBuilder

        Dim Authorization = xmlTemplate...<Authorization>
        For Each codeElement In Authorization
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[objectName]", _objectClass)

        ' Build Properties Authorization block ------------------
        Dim PropertiesAuthorizationBlock As New StringBuilder
        Dim fields = XMLtext...<Column>

        For Each node In fields
            PropertiesAuthorizationBlock.AppendFormat(" 'Authorizator.AllowRead(""{0}""){1}", node.@Name, System.Environment.NewLine)
        Next
        ' -------------------------------------------------------

        theCode.Replace("[PropertiesAuthorizationBlock]", PropertiesAuthorizationBlock.ToString)
        Return theCode.ToString
    End Function

    Public Function Build_Factory() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Factory>
                                           <Code>
    Private Sub New()
        ' require use of factory method 
        _DTB = Context.CurrentBusinessEntityCode
    End Sub

    Public Shared Function Blank[objectClass]() As [objectClass]
        Return New [objectClass]
    End Function

    Public Shared Function New[objectClass]([DeclareParamsBlock]) As [objectClass]
        If Not CanAddObject() Then ExceptionThower.NoAdding("[objectClass]")
        Return DataPortal.Create(Of [objectClass])([NEW_CRITERIA_BLOCK])
    End Function

    Public Shared Function Get[objectClass]([DeclareParamsBlock]) As [objectClass]
        If Not CanGetObject() Then ExceptionThower.NoAccess("[objectClass]")
        Return DataPortal.Fetch(Of [objectClass])([NEW_CRITERIA_BLOCK])
    End Function

    Public Shared Sub Delete[objectClass]([DeclareParamsBlock])
        If Not CanDeleteObject() Then ExceptionThower.NoDelete("[objectClass]")
        DataPortal.Delete([NEW_CRITERIA_BLOCK])
    End Sub

    Public Overrides Function Save() As [objectClass]
        If IsDeleted AndAlso Not CanDeleteObject() Then
            ExceptionThower.NoDelete("[objectClass]")
        ElseIf IsNew AndAlso Not CanAddObject() Then
            ExceptionThower.NoAdding("[objectClass]")
        ElseIf Not IsNew And Not CanEditObject() Then
            ExceptionThower.NoEdit("[objectClass]")
        End If
        If Not IsDirty Then
            ExceptionThower.NotDirty(Me.ToString)
        End If
        If Not IsSavable Then
            ExceptionThower.Invalid(Me.ToString)
        End If

        [objectClass]InfoList.InvalidateCache()

        Return MyBase.Save()
    End Function

    Public Function Clone[objectClass]([DeclareParamsBlock]) As [objectClass]
        Dim cloning[objectClass] As [objectClass] = MyBase.Clone
        [Fill_PKMemberCloning_Block]       
        cloning[objectClass].MarkNew()
        cloning[objectClass].ValidationRules.CheckRules()
        Return cloning[objectClass]
    End Function
                                          </Code>
                                       </Factory>

        Dim theCode As New StringBuilder

        Dim Authorization = xmlTemplate...<Factory>
        For Each codeElement In Authorization
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[NEW_CRITERIA_BLOCK]", NEW_CRITERIA_BLOCK)
        theCode.Replace("[DeclareParamsBlock]", DeclareParamsBlock)
        theCode.Replace("[ParamsBlock]", ParamsBlock)
        theCode.Replace("[Fill_PKMemberCloning_Block]", Fill_PKMemberCloning_Block)
        theCode.Replace("[objectClass]", _objectClass)
        Return theCode.ToString
    End Function

    Public Function Build_NEWCHILD() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <NEW_CHILD>
                                           <Code>
  Friend Sub New([Declare_child_Creation_Params_Block])
    [Fill_Members_Block]
  End Sub
  Friend Sub New(ByVal rawData As [DBTable])
    [Fill_Members_FromRaw_Block]
  End Sub
  Private Sub New()
  End Sub
  Public Shared Function Empty[objectClass]Info as [objectClass]Info
     return new [objectClass]Info
  end function
</Code>
                                       </NEW_CHILD>

        Dim theCode As New StringBuilder

        Dim NEW_CHILD = xmlTemplate...<NEW_CHILD>
        For Each codeElement In NEW_CHILD
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[Declare_child_Creation_Params_Block]", Declare_child_Creation_Params_Block)
        theCode.Replace("[Fill_Members_Block]", Fill_Members_Block)
        theCode.Replace("[Fill_Members_FromRaw_Block]", Fill_Members_FromRaw_Block)
        theCode.Replace("[DBTable]", _DBTable)
        Return theCode.ToString

    End Function

    Public Function Build_ClassCriteria() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <ClassCriteria>
                                           <Code>&lt;Serializable()&gt;Private Class [objectClass]Criteria
    [Declare_CriteriaMembers_Block]
    Public Sub New([DeclareParamsBlock])
        [Fill_PKMembers_Block]
    End Sub
    [Combine_DB_Keys_Block]
End Class
                                           </Code>
                                       </ClassCriteria>

        Dim theCode As New StringBuilder

        Dim ClassCriteria = xmlTemplate...<ClassCriteria>
        For Each codeElement In ClassCriteria
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[Declare_CriteriaMembers_Block]", Declare_CriteriaMembers_Block)
        theCode.Replace("[Combine_DB_Keys_Block]", Combine_DB_Keys_Block)
        theCode.Replace("[DeclareParamsBlock]", DeclareParamsBlock)
        theCode.Replace("[Fill_PKMembers_Block]", Fill_PKMembers_Block)
        theCode.Replace("[objectClass]", _objectClass)
        Return theCode.ToString
    End Function

    Public Function Build_DataPortal_Create() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Create>
                                           <Code>
    &lt;RunLocal()&gt; _
    Private Overloads Sub DataPortal_Create([criteriaParams_Block])
        [Fill_PKMembers_From_Criteria_Block]
        ValidationRules.CheckRules()
    End Sub
                                           </Code>
                                       </Create>

        Dim theCode As New StringBuilder

        Dim Create = xmlTemplate...<Create>
        For Each codeElement In Create
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[criteriaParams_Block]", criteriaParams_Block)
        theCode.Replace("[Fill_PKMembers_From_Criteria_Block]", Fill_PKMembers_From_Criteria_Block)
        theCode.Replace("[ParamsBlock]", ParamsBlock)
        theCode.Replace("[objectClass]", _objectClass)
        Return theCode.ToString
    End Function

    Public Function Build_DataPortal_Fetch() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Fetch>
                                           <Code>
    Private Overloads Sub DataPortal_Fetch([criteriaParams_Block])

    Using ctx = ContextManager(Of AuthorizatorDataContext).GetManager(Context.ConnectionString,false)
      
      Dim data = (From r In ctx.DataContext.[DBTable]s [Where_Block] _
                  Select r).Single
      [LOAD_PROPERTIES_BLOCK]
      ' get child data
      ' LoadProperty(Of ProjectResources)(ResourcesProperty, _
      '  ProjectResources.GetProjectResources(data.Assignments.ToArray))
    End Using

  End Sub
                                           </Code>
                                       </Fetch>

        Dim theCode As New StringBuilder

        Dim Fetch = xmlTemplate...<Fetch>
        For Each codeElement In Fetch
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[criteriaParams_Block]", criteriaParams_Block)
        theCode.Replace("[Fill_PKMembers_From_Criteria_Block]", Fill_PKMembers_From_Criteria_Block)
        theCode.Replace("[LOAD_PROPERTIES_BLOCK]", LoadProperties_Block)
        theCode.Replace("[Where_Block]", Where_Block)
        theCode.Replace("[DBTable]", _DBTable)
        theCode.Replace("[objectClass]", _objectClass)

        Return theCode.ToString
    End Function

    Public Function Build_DataPortal_Insert() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Insert>
                                           <Code1>
    '&lt;Transactional(TransactionalTypes.TransactionScope)&gt; _
    Protected Overrides Sub DataPortal_Insert()
        PrepareUpdatedData()
        Using ctx = ContextManager(Of AuthorizatorDataContext).GetManager(Context.ConnectionString, False)

        'todo : check this 
        ctx.DataContext.pbs_[DBTable]_InsertUpdate(STR_[objectClass], keys, _data)
        'DataPortal.UpdateChild(ReadProperty(Of ChildType)(ChildProperty), Me)
        End Using

    End Sub

    Private Sub PrepareUpdatedData()
       [Build_Raw_InsertUpdate_Data]
    End Sub
                                           </Code1>
                                           <Code2>
    '&lt;Transactional(TransactionalTypes.TransactionScope)&gt; _
    Protected Overrides Sub DataPortal_Insert()
        Using ctx = ContextManager(Of AuthorizatorDataContext).GetManager(Context.ConnectionString, False)

        'todo : check this 
        ctx.DataContext.pbs_[DBTable]_InsertUpdate([InsertUpdate_Params_Block])

        'DataPortal.UpdateChild(ReadProperty(Of ChildType)(ChildProperty), Me)
        End Using
    End Sub
                                          </Code2>
                                       </Insert>

        Dim theCode As New StringBuilder
        Dim ClassCriteria = xmlTemplate...<Insert>

        If _merge2Table Then
            For Each codeElement In ClassCriteria
                theCode.AppendLine(codeElement.<Code1>.Value)
            Next
            theCode.Replace("[Build_Raw_InsertUpdate_Data]", Build_Raw_InsertUpdate_Data)
        Else
            For Each codeElement In ClassCriteria
                theCode.AppendLine(codeElement.<Code2>.Value)
            Next
            theCode.Replace("[InsertUpdate_Params_Block]", InsertUpdate_Params_Block)
        End If

        theCode.Replace("[DBTable]", _DBTable)
        theCode.Replace("[objectClass]", _objectClass)

        Return theCode.ToString
    End Function

    Public Function Build_DataPortal_Update() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Update>
                                           <Code>
   '&lt;Transactional(TransactionalTypes.TransactionScope)&gt; _
    Protected Overrides Sub DataPortal_Update()
        DataPortal_Insert()
    End Sub
                                          </Code>
                                       </Update>

        Dim theCode As New StringBuilder
        Dim Update = xmlTemplate...<Update>
        For Each codeElement In Update
            theCode.AppendLine(codeElement.<Code>.Value)
        Next
        Return theCode.ToString
    End Function

    Public Function Build_DataPortal_Delete() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Delete>
                                           <Code>Protected Overrides Sub DataPortal_DeleteSelf()
        DataPortal_Delete([NEW_CRITERIA_BLOCK_SELF])
    End Sub

    '&lt;Transactional(TransactionalTypes.TransactionScope)&gt; _
    Private Overloads Sub DataPortal_Delete([criteriaParams_Block])
        Using ctx = ContextManager(Of AuthorizatorDataContext).GetManager(Context.ConnectionString, False)
            ctx.DataContext.pbs_[DBTable]_Delete([usingCriteriaParams_Block])
        End Using
    End Sub                            </Code>
                                       </Delete>

        Dim theCode As New StringBuilder
        Dim Delete = xmlTemplate...<Delete>
        For Each codeElement In Delete
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[UsingParamsBlock]", UsingParamsBlock)
        theCode.Replace("[NEW_CRITERIA_BLOCK_SELF]", NEW_CRITERIA_BLOCK_SELF)
        theCode.Replace("[usingCriteriaParams_Block]", usingCriteriaParams_Block)
        theCode.Replace("[criteriaParams_Block]", criteriaParams_Block)
        theCode.Replace("[ParamsBlockSelf]", ParamsBlockSelf)
        theCode.Replace("[DBTable]", _DBTable)
        theCode.Replace("[objectClass]", _objectClass)

        Return theCode.ToString
    End Function

    Public Function Build_IGenPart() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <IGenPart>
                                           <Code>
    Public Function CommandOf(ByVal shortcut As String) As String Implements IGenPartObject.CommandOf
        Return Action.CommandOf(shortcut)
    End Function

    Public Function getBO(ByVal id As String) As Object Implements IGenPartObject.getBO
        [ParsingID_Block] 
        Return Get[objectClass]([ParamsBlock])
    End Function

    Public Function myCommands() As String() Implements IGenPartObject.myCommands
        Return Action.StandardReferenceCommands
    End Function

    Public Function myFullName() As String Implements IGenPartObject.myFullName
        Return Me.GetType.ToString
    End Function

    Public Function myName() As String Implements IGenPartObject.myName
        Return "[objectClass]"
    End Function

    Public Function myQueryList() As Object Implements IGenPartObject.myQueryList
        Return [objectClass]InfoList.Get[objectClass]InfoList
    End Function                                               
                                           </Code>
                                       </IGenPart>

        Dim theCode As New StringBuilder

        Dim IGenPart = xmlTemplate...<IGenPart>
        For Each codeElement In IGenPart
            theCode.AppendLine(codeElement.<Code>.Value)
        Next

        theCode.Replace("[ParsingID_Block] ", ParsingID_Block)
        theCode.Replace("[ParamsBlock]", ParamsBlock)
        theCode.Replace("[objectClass]", _objectClass)
        Return theCode.ToString
    End Function

    Public Function Build_Exists() As String
        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Exists>
                                           <Code>
    Public Shared Function Exists(ByVal [ID] As String) As Boolean
        Return [objectClass]InfoList.Contains[ID]([ID])
    End Function
    [MultiCriteria_Exists]                                       
                                           </Code>
                                       </Exists>

        Dim theCode As New StringBuilder

        Dim Exists = xmlTemplate...<Exists>
        For Each codeElement In Exists
            theCode.AppendLine(codeElement.<Code>.Value)
        Next
        If _keyCount >= 1 Then
            theCode.Replace("[MultiCriteria_Exists] ", MultiCriteria_Exists)
            theCode.Replace("[ID_pArray]", ID_pArray)
        End If
        If _keyCount = 1 Then
            theCode.Replace("[ID]", "Code")
        ElseIf _keyCount > 1 Then
            theCode.Replace("[ID]", "ID")
        End If
        theCode.Replace("[DeclareParamsBlock]", DeclareParamsBlock)
        theCode.Replace("[ParamsBlock]", ParamsBlock)
        theCode.Replace("[objectClass]", _objectClass)
        Return theCode.ToString
    End Function

#Region " Services Functions "

    Private Function MultiCriteria_Exists() As String
        If _pkKeys.Count <= 1 Then Return String.Empty

        Dim _MultiCriteria_Exists As New StringBuilder
        _MultiCriteria_Exists.AppendLine("Public Shared Function Exists([DeclareParamsBlock]) As Boolean")
        _MultiCriteria_Exists.AppendLine("  Dim ID as string = [ID_pArray]")
        _MultiCriteria_Exists.AppendLine("  Return Exists(ID)")
        _MultiCriteria_Exists.AppendLine(" End Function")

        Return _MultiCriteria_Exists.ToString
    End Function

    Private Function InsertUpdate_Params_Block() As String
        If _fields.Count = 0 Then Return String.Empty

        Dim prepareblock As New StringBuilder
        For Each node In _pkKeys
            prepareblock.AppendFormat("_{0},", _
                                      If(node.@Type.Contains("Smart") OrElse node.@Type.Contains("SunDate"), node.@Name & ".DBValue", node.@Name))
        Next
        For Each node In _fields
            prepareblock.AppendFormat("_{0},", _
                                      If(node.@Type.Contains("Smart") OrElse node.@Type.Contains("SunDate"), node.@Name & ".DBValue", node.@Name))
        Next
        'eliminate ending andalso
        Return System.Text.RegularExpressions.Regex.Replace(prepareblock.ToString, ",$", String.Empty)
    End Function

    Private Function Declare_child_Creation_Params_Block() As String
        If _fields.Count = 0 Then Return String.Empty

        Dim _Declare_child_Creation_Params_Block As New StringBuilder
        For Each node In _pkKeys
            _Declare_child_Creation_Params_Block.AppendFormat("ByVal  p{0} as {1},", node.@Name, node.@Type)
        Next
        For Each node In _fields
            _Declare_child_Creation_Params_Block.AppendFormat("ByVal  p{0} as {1},", node.@Name, node.@Type)
        Next
        'eliminate ending andalso
        Return System.Text.RegularExpressions.Regex.Replace(_Declare_child_Creation_Params_Block.ToString, ",$", String.Empty)
    End Function

    Private Function Build_Raw_InsertUpdate_Data() As String
        If _fields.Count = 0 Then Return String.Empty

        Dim prepareblock As New StringBuilder
        For Each node In _fields
            prepareblock.AppendFormat("ClassSchema(Of {0}).SetSubData(""_{1}"", _{1}, _data){2}", _objectClass, node.@Name, System.Environment.NewLine)
        Next
        'eliminate ending andalso
        Return System.Text.RegularExpressions.Regex.Replace(prepareblock.ToString, ",$", String.Empty)
    End Function

    Private Function Where_Block() As String
        If _pkKeys.Count = 0 Then Return String.Empty
        Dim _where_Block As New StringBuilder("Where ")
        If _pkKeys.Count = 1 Then
            _where_Block.AppendFormat("r.{0} = criteria.Value ", _pkKeys(0).@Name)
        ElseIf _merge2Table Then
            _where_Block.AppendFormat("r.{0} = criteria.DB_KEYS ", _pkKeys(0).@Name)
        Else
            For Each node In _pkKeys
                _where_Block.AppendFormat(" r.{0} = criteria._{0} andalso", node.@Name)
            Next
        End If
        'eliminate ending andalso
        Return System.Text.RegularExpressions.Regex.Replace(_where_Block.ToString, "andalso$", " ")
    End Function 'used in fetch 

    Private Function LoadProperties_Block() As String
        Dim loadProp As New StringBuilder

        If Not _merge2Table Then
            Dim loads = From f In XMLtext...<Column> _
                Select String.Format("LoadProperty(Of {0})({1}Property, data.{2})", f.@Type, f.@Name, f.@Name.DBColumnName)
            For Each load As String In loads
                loadProp.AppendLine(load)
            Next
        Else
            Dim loads = From f In XMLtext...<Column> _
                Select String.Format("LoadProperty(Of {0})({1}Property, ClassSchema(Of {2}).GetSubData(""_{1}"", data.{1}))", f.@Type, f.@Name, _objectClass)
            For Each load As String In loads
                loadProp.AppendLine(load)
            Next
            loadProp.AppendLine("_data = Data.RAW_DATA")

        End If
        Return loadProp.ToString
    End Function

    Private Sub BuildProperty(ByVal field As System.Xml.Linq.XElement, ByRef properties As String)
        If properties Is Nothing Then properties = String.Empty

        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Properties>
                                           <Code>Private Shared [fieldname]Property As PropertyInfo(Of [type]) = RegisterProperty(Of [type]) _
            (GetType([objectName]), New PropertyInfo(Of [type])("[fieldname]", {defaultvalue}))
      [BACKED_FIELD]{PK_ATTRIBUTE}
      Public {ReadOnly} Property [fieldname]() As String
       {GET}{SET}
      End Property
    </Code>
                                       </Properties>

        Dim template As New StringBuilder
        Dim Prop = xmlTemplate...<Properties>
        For Each codeElement In Prop
            template.AppendLine(codeElement.<Code>.Value)
        Next

        If Not field.@IsPrimaryKey Is Nothing AndAlso field.@IsPrimaryKey.ToString.Trim.ToLower = "y" Then
            template.Replace("{ReadOnly}", "ReadOnly")
            template.Replace("{PK_ATTRIBUTE}", "[LB]<System.ComponentModel.DataObjectField(True, True)> _")
            template.Replace("{SET}", String.Empty)
        Else
            template.Replace("{ReadOnly}", String.Empty)
            template.Replace("{PK_ATTRIBUTE}", String.Empty)
        End If

        'backed field ----------------------
        If Not backedField Then
            template.Replace("[BACKED_FIELD]", String.Empty)
        Else
            template.Replace("[BACKED_FIELD]", "Private _[fieldname] As [type] = [fieldname]Property.DefaultValue")
        End If

        '--------Getter section ------------
        Dim getter As String = String.Empty
        If field.@Type.ToString.Contains("Smart") OrElse field.@Type.ToString.Contains("SunDate") Then
            template.Replace("{defaultvalue}", " New [type]")
            getter = "Return GetProperty(Of [type],String)([fieldname]Property[BACKED_FIELD])"
        Else
            template.Replace("{defaultvalue}", "String.Empty")
            getter = "Return GetProperty(Of [type])([fieldname]Property[BACKED_FIELD])"
        End If

        If Not backedField Then
            getter = getter.Replace("[BACKED_FIELD]", String.Empty)
        Else
            getter = getter.Replace("[BACKED_FIELD]", ",_[fieldname]")
        End If

        getter = String.Format("Get [LB][TAB]{0}[LB] End Get", getter)

        template.Replace("{GET}", getter)
        '------- end getter -------------

        '--------Setter section ------------
        Dim setter As String = String.Empty

        If field.@Type.ToString.Contains("Smart") OrElse field.@Type.ToString.Contains("SunDate") Then
            template.Replace("{defaultvalue}", " New [type]")
            setter = "SetProperty(Of [type],String)([fieldname]Property [BACKED_FIELD], value)"
        Else
            setter = "SetProperty(Of [type])([fieldname]Property [BACKED_FIELD], value)"
        End If

        If Not backedField Then
            setter = setter.Replace("[BACKED_FIELD]", String.Empty)
        Else
            setter = setter.Replace("[BACKED_FIELD]", ",_[fieldname]")
        End If

        setter = String.Format("[LB]Set(ByVal value As String)[LB][TAB]{0}[LB] End Set", setter)

        template.Replace("{SET}", setter)
        '------- end setter -------------

        template.Replace("[type]", field.@Type)
        template.Replace("[fieldname]", field.@Name)
        template.Replace("[objectName]", _objectClass)
        template.Replace("[LB]", System.Environment.NewLine)
        template.Replace("[TAB]", Chr(Keys.Tab))
        properties += template.ToString

    End Sub

    Private Sub BuildInfoProperty(ByVal field As System.Xml.Linq.XElement, ByRef properties As String)
        If properties Is Nothing Then properties = String.Empty

        Dim xmlTemplate As XDocument = <?xml version="1.0"?>
                                       <Properties>
                                           <Code>
   Private _[fieldname] As [type]
   Public Property [fieldname]() As [type]
    Get
      Return _[fieldname]
    End Get
    Friend Set(ByVal value As  [type])
      _[fieldname] = value
    End Set
  End Property 
</Code>
                                       </Properties>

        Dim template As New StringBuilder
        Dim Prop = xmlTemplate...<Properties>
        For Each codeElement In Prop
            template.AppendLine(codeElement.<Code>.Value)
        Next

        template.Replace("[type]", field.@Type)
        template.Replace("[fieldname]", field.@Name)
        properties += template.ToString

    End Sub

    Private Function ParamsBlock() As String
        Dim _GetParamsBlock As New StringBuilder

        If _pkKeys.Count = 0 Then Return String.Empty

        For Each node In _pkKeys
            _GetParamsBlock.AppendFormat("p{0},", node.@Name)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function

    Private Function ParamsBlockSelf() As String
        Dim _GetParamsBlock As New StringBuilder

        If _pkKeys.Count = 0 Then Return String.Empty

        For Each node In _pkKeys
            _GetParamsBlock.AppendFormat("_{0},", node.@Name)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function

    Private Function UsingParamsBlock() As String
        Dim _GetParamsBlock As New StringBuilder

        If _pkKeys.Count = 0 Then Return String.Empty

        For Each node In _pkKeys
            _GetParamsBlock.AppendFormat("_{0},", node.@Name)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function

    Private Function DeclareParamsBlock() As String
        Dim _GetParamsBlock As New StringBuilder
        If _pkKeys.Count = 0 Then Return String.Empty

        For Each node In _pkKeys
            _GetParamsBlock.AppendFormat("ByVal  p{0} as {1},", node.@Name, node.@Type)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function

    Private Function criteriaParams_Block() As String
        Select Case _keyCount
            Case 0
                Return String.Empty
            Case 1
                Return String.Format("ByVal criteria As SingleCriteria(Of [objectClass],{0})", _pkKeys(0).@Type)
            Case Else
                Return "ByVal criteria As [objectClass]Criteria"
        End Select

    End Function

    Private Function usingCriteriaParams_Block() As String
        Select Case _keyCount
            Case 0
                Return String.Empty
            Case 1
                Return "Criteria.Value"
            Case Else

                If _merge2Table Then
                    Return "STR_[objectClass],Criteria.DB_KEYS"
                Else

                    Dim _usingCriteriaParams_Block As New StringBuilder
                    For Each node In _pkKeys
                        _usingCriteriaParams_Block.AppendFormat("Criteria._{0},", node.@Name)
                    Next
                    'eliminate ending ,
                    Return System.Text.RegularExpressions.Regex.Replace(_usingCriteriaParams_Block.ToString, ",$", String.Empty)
                End If

        End Select

    End Function

    Private Function NEW_CRITERIA_BLOCK() As String
        Dim _GetNEW_CRITERIA_BLOCK As New StringBuilder

        If _pkKeys.Count = 0 Then Return String.Empty

        If _pkKeys.Count = 1 Then

            _GetNEW_CRITERIA_BLOCK.AppendFormat("New SingleCriteria(Of [objectClass],{0})(p{1})", _pkKeys(0).@Type, _pkKeys(0).@Name)

        Else

            _GetNEW_CRITERIA_BLOCK.Append("New [objectClass]Criteria([ParamsBlock])")

        End If

        Return _GetNEW_CRITERIA_BLOCK.ToString
    End Function
    Private Function NEW_CRITERIA_BLOCK_SELF() As String
        Dim _GetNEW_CRITERIA_BLOCK As New StringBuilder

        If _pkKeys.Count = 0 Then Return String.Empty

        If _pkKeys.Count = 1 Then

            _GetNEW_CRITERIA_BLOCK.AppendFormat("New SingleCriteria(Of [objectClass],{0})(_{1})", _pkKeys(0).@Type, _pkKeys(0).@Name)

        Else

            _GetNEW_CRITERIA_BLOCK.Append("New [objectClass]Criteria([ParamsBlockSelf])")

        End If

        Return _GetNEW_CRITERIA_BLOCK.ToString
    End Function

    Private Function Declare_CriteriaMembers_Block() As String
        Dim _CriteriaMembers_Block As New StringBuilder
        Dim pkFields = From f In XMLtext...<Column> _
                       Where f.@IsPrimaryKey IsNot Nothing AndAlso f.@IsPrimaryKey.Trim.ToUpper = "Y"

        If _keyCount = 0 Then Return String.Empty

        For Each node In _pkKeys
            _CriteriaMembers_Block.AppendFormat("Public _{0} as {1}{2}", node.@Name, node.@Type, System.Environment.NewLine)
        Next

        Return _CriteriaMembers_Block.ToString
    End Function

    Private Function Fill_PKMemberCloning_Block() As String
        Dim _GetParamsBlock As New StringBuilder

        If _keyCount = 0 Then Return String.Empty

        For Each node In _pkKeys
            _GetParamsBlock.AppendFormat("cloning[objectClass]._{0} = p{0}{1}", node.@Name, System.Environment.NewLine)
        Next

        Return _GetParamsBlock.ToString
    End Function
    Private Function Fill_PKMembers_From_Criteria_Block() As String
        Dim _GetParamsBlock As New StringBuilder

        If _keyCount = 0 Then Return String.Empty
        If _keyCount = 1 Then
            _GetParamsBlock.AppendFormat("_{0} = Criteria.Value{1}", _pkKeys(0).@Name, System.Environment.NewLine)
        Else
            For Each node In _pkKeys
                _GetParamsBlock.AppendFormat("_{0} = Criteria._{0}{1}", node.@Name, System.Environment.NewLine)
            Next

        End If

        Return _GetParamsBlock.ToString
    End Function
    Private Function Fill_PKMembers_Block() As String
        Dim _GetParamsBlock As New StringBuilder

        If _keyCount = 0 Then Return String.Empty
        For Each node In _pkKeys
            _GetParamsBlock.AppendFormat("_{0} = p{0}{1}", node.@Name, System.Environment.NewLine)
        Next

        Return _GetParamsBlock.ToString
    End Function
    Private Function Fill_Members_Block() As String
        Dim _Fill_Members_Block As New StringBuilder

        If _keyCount = 0 Then Return String.Empty
        For Each node In _pkKeys
            _Fill_Members_Block.AppendFormat("_{0} = p{0}{1}", node.@Name, System.Environment.NewLine)
        Next
        For Each node In _fields
            _Fill_Members_Block.AppendFormat("_{0} = p{0}{1}", node.@Name, System.Environment.NewLine)
        Next

        Return _Fill_Members_Block.ToString
    End Function
    Private Function Fill_Members_FromRaw_Block() As String
        Dim _Fill_Members_Block As New StringBuilder

        If _keyCount = 0 Then Return String.Empty

        If Not _merge2Table Then

            For Each node In _pkKeys
                _Fill_Members_Block.AppendFormat("_{0} = rawData.{1}{2}", _
                                                  If(node.@Type.Contains("Smart") OrElse node.@Type.Contains("SunDate"), node.@Name & ".Text", node.@Name), _
                                                  node.@Name.DBColumnName, _
                                                  System.Environment.NewLine)
            Next
            For Each node In _fields
                _Fill_Members_Block.AppendFormat("_{0} = rawData.{1}{2}", _
                                                  If(node.@Type.Contains("Smart") OrElse node.@Type.Contains("SunDate"), node.@Name & ".Text", node.@Name), _
                                                  node.@Name.DBColumnName, _
                                                  System.Environment.NewLine)
            Next
        Else
            For Each node In _pkKeys
                _Fill_Members_Block.AppendFormat(" _{0} = ClassSchema(Of {1}).GetSubData(""_{0}"", rawData.Colum){2}", _
                                                  node.@Name, _objectClass, System.Environment.NewLine)
            Next
            For Each node In _fields
                _Fill_Members_Block.AppendFormat(" _{0} = ClassSchema(Of {1}).GetSubData(""_{0}"", rawData.Colum){2}", _
                                                  node.@Name, _objectClass, System.Environment.NewLine)
            Next
        End If

        Return _Fill_Members_Block.ToString

    End Function

#End Region
End Class
