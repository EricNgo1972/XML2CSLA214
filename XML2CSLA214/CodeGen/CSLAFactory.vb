Imports System.Text
Public Class CSLAFactory
    Private _schema As Schema
    Public Sub New(ByVal pSchema As Schema)
        _schema = pSchema
    End Sub

#Region "Class Header/Footer"
    Friend Function GetClassHeader_EditableList() As String
        Dim template = <template>Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Csla
Imports Csla.Data
Imports pbs.Helper
Imports pbs.BO.BusinessRules

<%= If(_schema.Keys.Count > 1, "Imports System.Text.RegularExpressions" & vbCrLf, vbCrLf) %>

                           <%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>

    &lt;Serializable()&gt; _
    Public Class <%= _schema._className %>List
        Inherits Csla.BusinessListBase(Of <%= _schema._className %>List, <%= _schema._className %>)
                            </template>
        Return template.Value
    End Function

    Friend Function GetBusinessMethod_EditableList() As String
        Dim template = <template>
#Region " Business Methods"

        private _DTB As String = String.Empty

        Protected Overrides Function AddNewCore() As Object
            If _parent IsNot Nothing Then
                Dim theNewLine = <%= _schema._className %>.New<%= _schema._className %>([UsingKeyParameters])
                AddNewLine(theNewLine)
                theNewLine.CheckRules()
                Return theNewLine
            Else
                Return MyBase.AddNewCore
            End If
        End Function

        Friend Sub AddNewLine(ByVal _line As <%= _schema._className %>)
            If _line Is Nothing Then Exit Sub

            'get the next line number
            Dim nextnumber As Integer = 1
            If Me.Count > 0 Then
                Dim allNumbers = (From line In Me Select CInt(Nz(Regex.Match(line.{LINE_NO}, "[0-9]").Value, 0))).ToList
                nextnumber = allNumbers.Max + 1
            End If

            _line.{LINE_NO} = String.Format("{0:00000}", nextnumber)

           'Populate _line with info from parent here

            Me.Add(_line)

        End Sub

#End Region
                        </template>.Value

        Return template.Replace("[UsingKeyParameters]", UsingKeyParameters)
    End Function

    Friend Function GetClassHeader_ChildEditableList() As String
        Dim template = <template>Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Csla
Imports Csla.Data
Imports pbs.Helper
<%= If(_schema.Keys.Count > 1, "Imports System.Text.RegularExpressions" & vbCrLf, vbCrLf) %>

                           <%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>

    &lt;Serializable()&gt; _
    Public Class <%= _schema._className %>s
        Inherits Csla.BusinessListBase(Of <%= _schema._className %>s, <%= _schema._className %>)
                            </template>
        Return template.Value
    End Function

    Friend Function GetBusinessMethod_ChildEditableList() As String
        Dim template = <template>
#Region " Business Methods"
        Friend _parent As <%= _schema._parentClassName %> = Nothing

        Protected Overrides Function AddNewCore() As Object
            If _parent IsNot Nothing Then
                Dim theNewLine = <%= _schema._className %>.New<%= _schema._className %>(_parent.GetIdValue.ToString, String.Empty)
                AddNewLine(theNewLine)
                theNewLine.CheckRules()
                Return theNewLine
            Else
                Return MyBase.AddNewCore
            End If
        End Function

        Friend Sub AddNewLine(ByVal _line As <%= _schema._className %>)
            If _line Is Nothing Then Exit Sub
         
            'get the next line number
            Dim nextnumber As Integer = 1
            If Me.Count > 0 Then
                Dim allNumbers = (From line In Me Select CInt(Nz(Regex.Match(line.{LINE_NO}, "[0-9]").Value, 0))).ToList
                nextnumber = allNumbers.Max + 1
            End If

            _line.{LINE_NO} = String.Format("{0:00000}", nextnumber)

           'Populate _line with info from parent here

            Me.Add(_line)

        End Sub

#End Region
                        </template>
        Return template.Value
    End Function

    Friend Function GetClassHeader_Editable() As String
        Dim template = <template>Imports pbs.Helper
Imports System.Data.SqlClient
Imports Csla.Data
Imports Csla.Validation
Imports pbs.BO.DataAnnotations
Imports pbs.BO.Script

<%= If(_schema.Keys.Count > 1, "Imports System.Text.RegularExpressions" & vbCrLf, vbCrLf) %>
                           <%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>

    &lt;Serializable()&gt; _
    Public Class <%= _schema._className %>
        Inherits Csla.BusinessBase(Of <%= _schema._className %>)
        Implements Interfaces.IGenPartObject
        Implements IComparable
        Implements IDocLink

                       </template>
        Return template.Value
    End Function

    Friend Function GetClassHeader_ChildEditable() As String
        Dim template = <template>Imports pbs.Helper
Imports System.Data
Imports System.Data.SqlClient
Imports Csla
Imports Csla.Data
Imports Csla.Validation
<%= If(_schema.Keys.Count > 1, "Imports System.Text.RegularExpressions" & vbCrLf, vbCrLf) %>
                           <%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>

    &lt;Serializable()&gt; _
    Public Class <%= _schema._className %>
        Inherits Csla.BusinessBase(Of <%= _schema._className %>)
        Implements IComparable

                       </template>
        Return template.Value
    End Function

    Friend Function GetClassHeader_Readonly() As String
        Dim template = <template>
Imports pbs.Helper
Imports pbs.Helper.Interfaces
Imports System.Data
Imports Csla
Imports Csla.Data
Imports Csla.Validation
<%= If(_schema.Keys.Count > 1, "Imports System.Text.RegularExpressions" & vbCrLf, vbCrLf) %>
                           <%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>

    &lt;Serializable()&gt; _
    Public Class <%= _schema._className %>Info
        Inherits Csla.ReadOnlyBase(Of <%= _schema._className %>Info)
        Implements IComparable
        Implements IInfo
        Implements IDocLink
        'Implements IInfoStatus
        
                       </template>
        Return template.Value
    End Function

    Friend Function GetClassHeader_ReadonlyList() As String
        Dim template = <template>
Imports Csla.Data
Imports pbs.Helper

<%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>

    &lt;Serializable()&gt; _
    Public Class <%= _schema._className %>InfoList
        Inherits Csla.ReadOnlyListBase(Of <%= _schema._className %>InfoList, <%= _schema._className %>Info)

                       </template>
        Return template.Value
    End Function

    Friend Function GetDictionary() As String
        Dim template = <template>
#Region "{XX} Dictionary"

        Private Shared _{xx}Dic As Dictionary(Of String, {XX}Info)

        Private Shared Function Get{XX}Dic() As Dictionary(Of String, {XX}Info)
            If _{xx}Dic Is Nothing OrElse _DTB &lt;&gt; Context.CurrentBECode Then
                _{xx}Dic = New Dictionary(Of String, {XX}Info)

                For Each itm In {XX}InfoList.Get{XX}InfoList
                    _{xx}Dic.Add(itm.Code, itm)
                Next
            End If

            Return _{xx}Dic

        End Function

#End Region

        Private Class Query

            Friend Shared Function BuildQD(ByVal pFilters As Dictionary(Of String, String)) As QD

            Dim _QD As QD = QD.SysNewQD("SYSTEMQD")
            _QD.AnalQ0 = "XXXX"
            _QD.Descriptn = "XXXX"

            'output
            _QD.AddSelectedField("*")

            'filters
             _QD.AddFilterDictionary(pFilters)

            If _QD.Filters.Count = 0 Then
                _QD.AddFilter("MISA_INV\RefID", "", "&lt;ALL>")
            End If

            Return _QD

            End Function
        
        End Class
             
                       </template>

        If _schema.Keys.Count = 1 Then
            Dim ret = template.Value.Replace("{XX}", _schema._className)
            ret = ret.Replace("{xx}", _schema._className.ToLower)
            Return ret
        Else
            Return String.Empty
        End If
    End Function

    Friend Function GetClassFooter() As String
        Dim template = <template>
    End Class

<%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "End Namespace ") %>
                       </template>
        Return template.Value
    End Function

#End Region

#Region "Validation Rules"
    Friend Function GetValidationRules() As String
        Dim template = <template>
#Region "Validation Rules"

        Private Sub AddSharedCommonRules()
            'Sample simple custom rule
            'ValidationRules.AddRule(AddressOf LDInfo.ContainsValidPeriod, "Period", 1)           
          
            'Sample dependent property. when check one , check the other as well
            'ValidationRules.AddDependantProperty("AccntCode", "AnalT0")
        End Sub

        Protected Overrides Sub AddBusinessRules()
            AddSharedCommonRules()

            For Each _field As ClassField In ClassSchema(Of <%= _schema._className %>)._fieldList 
                If _field.Required Then
                    ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, _field.FieldName, 0)
                End If
                If Not String.IsNullOrEmpty(_field.RegexPattern) Then
                    ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.RegExMatch, New RegExRuleArgs(_field.FieldName, _field.RegexPattern), 1)
                End If
                '----------using lookup, if no user lookup defined, fallback to predefined by developer----------------------------
                If CATMAPInfoList.ContainsCode(_field) Then
                    ValidationRules.AddRule(AddressOf LKUInfoList.ContainsLiveCode, _field.FieldName, 2)
                'Else
                '    Select Case _field.FieldName
                '        Case "LocType"
                '            ValidationRules.AddRule(Of LOC, AnalRuleArg)(AddressOf LOOKUPInfoList.ContainsSysCode, New AnalRuleArg(_field.FieldName, SysCats.LocationType))
                '        Case "Status"
                '            ValidationRules.AddRule(Of LOC, AnalRuleArg)(AddressOf LOOKUPInfoList.ContainsSysCode, New AnalRuleArg(_field.FieldName, SysCats.LocationStatus))
                '    End Select
                End If
            Next
            pbs.BO.Rules.BusinessRules.RegisterBusinessRules(Me)
            MyBase.AddBusinessRules()
        End Sub
#End Region ' Validation
                       </template>
        Return template.Value
    End Function
#End Region

#Region " Authorization Rules "
    Friend Function GetAuthorizationRules() As String
        Dim template = <template>
#Region " Authorization Rules "

    Protected Overrides Sub AddAuthorizationRules()
<%= From f In _schema.Members Select String.Format("'AuthorizationRules.AllowRead(""{0}"", ""ReadGroup"") {1}", f._id.toPropertyName, vbCrLf) %>
    End Sub

    Public Shared Function CanGetObject() As Boolean
        Return pbs.Security.Permission.isPermited(GetType(<%= _schema._className %>).ToString)
    End Function
    Public Shared Function CanAddObject() As Boolean
        Return pbs.Security.Permission.isPermited(String.Format("{0}.{1}", GetType(<%= _schema._className %>).ToString, Action._Create.ToUpper))
    End Function
    Public Shared Function CanEditObject() As Boolean
        Return pbs.Security.Permission.isPermited(String.Format("{0}.{1}", GetType(<%= _schema._className %>).ToString, Action._Amend.ToUpper))
    End Function
    Public Shared Function CanDeleteObject() As Boolean
        Return pbs.Security.Permission.isPermited(String.Format("{0}.{1}", GetType(<%= _schema._className %>).ToString, Action._Delete.ToUpper))
    End Function

#End Region ' Authorization Rules
                       </template>
        Return template.Value
    End Function

#End Region ' Authorization Rules

#Region "Factory Methods"

    Friend Function GetFactoryMethods_Editable() As String
        Dim str = New StringBuilder

        Dim template = <template>
#Region " Factory Methods "

        Private Sub New()
           _DTB = Context.CurrentBECode
        End Sub

        Public Shared Function Blank<%= _schema._className %>() As <%= _schema._className %>
            Return New <%= _schema._className %>
        End Function

        Public Shared Function New<%= _schema._className %>([DeclareKeyParameters]) As <%= _schema._className %>
            'Delete this line if the object uses an auto key
            If KeyDuplicated([UsingKeyParameters]) Then ExceptionThower.BusinessRuleStop(String.Format(ResStr(ResStrConst.NOACCESS), ResStr("<%= _schema._className %>")))
            Return DataPortal.Create(Of <%= _schema._className %>)(New Criteria([UsingKeyParameters]))
        End Function

        Public Shared Function NewBO(ByVal ID As String) As <%= _schema._className %>
             [ParsingID_Block]
             Return New<%= _schema._className %>([UsingKeyParameters])
        End Function

        Public Shared Function Get<%= _schema._className %>([DeclareKeyParameters]) As <%= _schema._className %>
            Return DataPortal.Fetch(Of <%= _schema._className %>)(New Criteria([UsingKeyParameters]))
        End Function

        Public Shared Function GetBO(ByVal ID As String) As <%= _schema._className %>
            [ParsingID_Block]
            Return Get<%= _schema._className %>([UsingKeyParameters])
        End Function

        Public Shared Sub Delete<%= _schema._className %>([DeclareKeyParameters])
            DataPortal.Delete(New Criteria([UsingKeyParameters]))
        End Sub

        Public Overrides Function Save() As <%= _schema._className %>
            If Not IsDirty Then ExceptionThower.NotDirty(ResStr(ResStrConst.NOTDIRTY))
            If Not IsSavable Then Throw New Csla.Validation.ValidationException(String.Format(ResStr(ResStrConst.INVALID), ResStr("<%= _schema._className %>")))
            
            Me.ApplyEdit()
           
            Dim Ret = MyBase.Save()
            If Not Context.IsBatchSavingMode Then  <%= _schema._className %>InfoList.InvalidateCache()
            return Ret
        End Function

       Public Function Clone<%= _schema._className %>([DeclareKeyParameters]) As <%= _schema._className %>
           
           If <%= _schema._className %>.KeyDuplicated([UsingKeyParameters]) Then ExceptionThower.BusinessRuleStop(ResStr(ResStrConst.CreateAlreadyExists), Me.GetType.ToString.Leaf.Translate)                           

            Dim cloning<%= _schema._className %> As <%= _schema._className %> = MyBase.Clone
            [Cloning_Block]            
           'Todo:Remember to reset status of the new object here 
            cloning<%= _schema._className %>.MarkNew()
            cloning<%= _schema._className %>.ApplyEdit()

            cloning<%= _schema._className %>.ValidationRules.CheckRules()

            Return cloning<%= _schema._className %>
        End Function

#End Region ' Factory Methods
                    </template>

        str.Append(template.Value)
        str.Replace("[DeclareKeyParameters]", DeclareKeyParameters)
        str.Replace("[UsingKeyParameters]", UsingKeyParameters)
        str.Replace("[ParsingID_Block]", ParsingID_Block)
        str.Replace("[Cloning_Block]", Cloning_Block)

        Return str.ToString
    End Function

    Friend Function GetFactoryMethods_ChildEditable() As String
        Dim str = New StringBuilder

        Dim template = <template>
#Region " Factory Methods - Child"

        Friend Shared Function NewChild<%= _schema._className %>([DeclareKeyParameters]) As <%= _schema._className %>
            Return New <%= _schema._className %>([UsingKeyParameters])
        End Function

        Friend Shared Function GetChild<%= _schema._className %>(ByVal dr As SafeDataReader, Optional ByVal SuppressValidation As Boolean = False) As <%= _schema._className %>
            Dim ret = New <%= _schema._className %>(dr, SuppressValidation)
            ret.MarkOld()
            ret.MarkAsChild()
            Return ret
        End Function

        'Private Sub New([DeclareKeyParameters])
        '    [AssignKeys_Block]
        '    ValidationRules.CheckRules() '--if validation depend on parent data.delay the checking until inserting line to collection
        '    MarkAsChild()
        'End Sub

        Private Sub New(ByVal dr As SafeDataReader, Optional ByVal SuppressValidation As Boolean = False)
            MarkAsChild()
            FetchObject(dr)
            If Not SuppressValidation Then ValidationRules.CheckRules()
        End Sub

#End Region ' Factory Methods
                    </template>

        str.Append(template.Value)
        str.Replace("[DeclareKeyParameters]", DeclareKeyParameters)
        str.Replace("[UsingKeyParameters]", UsingKeyParameters)
        str.Replace("[AssignKeys_Block]", AssignKeys_Block)

        Return str.ToString
    End Function

#Region "Editable List"
    Friend Function GetFactoryMethods_EditableList() As String
        Dim pclassName As String = _schema._className
        Dim template = <template>
                           
#Region " Factory Methods "

      <%= EL_New() %>
                           <%= EL_EmptyList(pclassName) %>
                           <%= EL_GetListByQD(pclassName) %>
                           <%= EL_DecorateAnalysisDescription(pclassName) %>
#End Region ' Factory Methods
                    </template>

        Return template.Value.Trim
    End Function

    Private Function EL_New() As String
        Dim template = <template>
        Private Sub New()
            ' require use of factory method 
            _DTB = Context.CurrentBECode
        End Sub
                    </template>.Value.Trim
        Return template
    End Function

    Private Function EL_EmptyList(pClassName As String) As String
        Dim template = <template>
 
        Public Shared Function Empty<%= pClassName %>List() As <%= pClassName %>List
            Return New <%= pClassName %>List
        End Function
                    </template>.Value.Trim
        Return template
    End Function

    Private Function EL_GetListByQD(pClassName As String) As String
        Dim template = <template>
 
          Public Shared Function GetByQD(ByVal _QD As QD, Optional ByVal dic As Dictionary(Of String, String) = Nothing) As <%= pClassName %>List

            _QD.Selected.Clear()
            QDFactory.MakeUp_<%= pClassName %>Query(_QD)
            _QD.AddFilterDictionary(dic)

            Dim sqlText As String = _QD.BuildSQL()

            Dim ret = DataPortal.Fetch(Of <%= pClassName %>List)(New FilterCriteria(sqlText))

            'DecorateAnalysisDescription(ret)

            Return ret
        End Function

                    </template>.Value.Trim
        Return template
    End Function

    Private Function EL_DecorateAnalysisDescription(pClassName As String) As String
        Dim template = <template>

         Private Shared Sub DecorateAnalysisDescription(ByVal ret As <%= pClassName %>List)
            If ret Is Nothing OrElse ret.Count = 0 Then Exit Sub

            Dim limit = pbs.Helper.InfoListSize
            If limit > 0 AndAlso not ret.Count > limit Then
        Dim _T0Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT0")
        Dim _T1Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT1")
        Dim _T2Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT2")
        Dim _T3Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT3")
        Dim _T4Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT4")
        Dim _T5Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT5")
        Dim _T6Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT6")
        Dim _T7Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT7")
        Dim _T8Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT8")
        Dim _T9Dic = LinkSources.DetectInfoDictionaryBySourceCode("AnalT9")

        For Each info In ret

            If _T0Dic.ContainsKey(info.AnalT0) Then info._analDescT0 = _T0Dic(info.AnalT0)
            If _T1Dic.ContainsKey(info.AnalT1) Then info._analDescT1 = _T1Dic(info.AnalT1)
            If _T2Dic.ContainsKey(info.AnalT2) Then info._analDescT2 = _T2Dic(info.AnalT2)
            If _T3Dic.ContainsKey(info.AnalT3) Then info._analDescT3 = _T3Dic(info.AnalT3)
            If _T4Dic.ContainsKey(info.AnalT4) Then info._analDescT4 = _T4Dic(info.AnalT4)
            If _T5Dic.ContainsKey(info.AnalT5) Then info._analDescT5 = _T5Dic(info.AnalT5)
            If _T6Dic.ContainsKey(info.AnalT6) Then info._analDescT6 = _T6Dic(info.AnalT6)
            If _T7Dic.ContainsKey(info.AnalT7) Then info._analDescT7 = _T7Dic(info.AnalT7)
            If _T8Dic.ContainsKey(info.AnalT8) Then info._analDescT8 = _T8Dic(info.AnalT8)
            If _T9Dic.ContainsKey(info.AnalT9) Then info._analDescT9 = _T9Dic(info.AnalT9)

        Next
            End If

    End Function

                    </template>.Value.Trim
        Return template
    End Function

#End Region

    Friend Function GetFactoryMethods_ChildEditableList() As String
        Dim template = <template>
#Region " Factory Methods "

        Friend Shared Function New<%= _schema._className %>s(ByVal pParent As <%= _schema._parentClassName %>) As <%= _schema._className %>s
            Return New <%= _schema._className %>s(pParent)
        End Function

        Friend Shared Function Get<%= _schema._className %>s(ByVal dr As SafeDataReader, ByVal parent As <%= _schema._parentClassName %>) As <%= _schema._className %>s
            Dim ret =  New <%= _schema._className %>s(dr, parent)
            ret.MarkAsChild()
            Return ret
        End Function

        Private Sub New(ByVal parent As <%= _schema._parentClassName %>)
            _parent = parent
            MarkAsChild()
        End Sub

        Private Sub New(ByVal dr As SafeDataReader, ByVal parent As <%= _schema._parentClassName %>)
            _parent = parent
            MarkAsChild()
            Fetch(dr)
        End Sub

#End Region ' Factory Methods
                    </template>

        Return template.Value.Trim
    End Function

    Friend Function GetFactoryMethods_ReadOnly() As String
        Dim str = New StringBuilder

        Dim template = <template>
#Region " Factory Methods "

        Friend Shared Function Get<%= _schema._className %>Info(ByVal dr As SafeDataReader) As <%= _schema._className %>Info
            Return New <%= _schema._className %>Info(dr)
        End Function

        Friend Shared Function Empty<%= _schema._className %>Info([DeclareKeyParameters_Optional]) As <%= _schema._className %>Info
            Dim info As <%= _schema._className %>Info = New <%= _schema._className %>Info
            With info
               [Fill_KeyMembers_Criteria]
            End With
            Return info
        End Function

        Private Sub New(ByVal dr As SafeDataReader)
             <%= Fill_MembersFrom_Dr() %>
        End Sub

        Private Sub New()
        End Sub


#End Region ' Factory Methods
                    </template>

        str.Append(template.Value)

        str.Replace("[DeclareKeyParameters_Optional]", DeclareKeyParameters_Optional)
        str.Replace("[Fill_KeyMembers_Criteria]", Fill_KeyMembers_Criteria(True))
        str.Replace("[Fill_Members]", Fill_Members)
        str.Replace("[Fill_Merged_Members]", Fill_Merged_Members)

        Return str.ToString
    End Function

    Friend Function GetFactoryMethods_ReadonlyList() As String
        Dim str = New StringBuilder

        Dim MultiKeytemplate = <template>
#Region " Factory Methods "

        Private Sub New()
            _DTB = Context.CurrentBECode
        End Sub

        Public Shared Function Get<%= _schema._className %>Info([DeclareKeyParameters]) As <%= _schema._className %>Info
            Dim Info As <%= _schema._className %>Info = <%= _schema._className %>Info.Empty<%= _schema._className %>Info([UsingKeyParameters])
            Dim ID = Info.ToString
            ContainsID(ID, Info)
            Return Info
        End Function

        'Public Shared Function GetDescription(ByVal pCode As String) As String
        '   Return Get<%= _schema._className %>Info(pCode).Description
        'End Function

        Public Shared Function Get<%= _schema._className %>InfoList() As <%= _schema._className %>InfoList
            If _list Is Nothing OrElse _DTB &lt;&gt; Context.CurrentBECode Then

            _DTB = Context.CurrentBECode
                _list = DataPortal.Fetch(Of <%= _schema._className %>InfoList)(New FilterCriteria())

        End If
        Return _list
    End Function

    Public Shared Sub InvalidateCache()
        _list = Nothing
    End Sub

    Public Shared Function ContainsCode([DeclareKeyParameters], Optional ByRef RetInfo As <%= _schema._className %>Info = Nothing) As Boolean
          Dim EmptyInfo = <%= _schema._className %>Info.Empty<%= _schema._className %>Info([UsingKeyParameters])
          Dim fl = From info In Get<%= _schema._className %>InfoList() Where info.CompareTo([UsingKeyParameters]) = 0
          For Each info As <%= _schema._className %>Info In fl
              RetInfo = info
              Return True
          Next
          RetInfo = EmptyInfo
    End Function

    Public Shared Function ContainsID(Byval ID as String, Optional ByRef RetInfo As <%= _schema._className %>Info = Nothing) As Boolean

            Dim EmptyInfo = <%= _schema._className %>Info.Empty<%= _schema._className %>Info()

            Dim fl = From info In Get<%= _schema._className %>InfoList() Where info.CompareTo(ID) = 0 

            For Each info As <%= _schema._className %>Info In fl
            RetInfo = info
            Return True
        Next

        RetInfo = EmptyInfo
    End Function

#End Region ' Factory Methods
                    </template>

        Dim SingleKeytemplate = <template>
#Region " Factory Methods "

        Private Sub New()
            _DTB = Context.CurrentBECode
        End Sub

        Public Shared Function Get<%= _schema._className %>Info([DeclareKeyParameters]) As <%= _schema._className %>Info
            Dim Info As <%= _schema._className %>Info = <%= _schema._className %>Info.Empty<%= _schema._className %>Info([UsingKeyParameters])
            ContainsCode([UsingKeyParameters], Info)
            Return Info
        End Function

        Public Shared Function GetDescription([DeclareKeyParameters]) As String
            Return Get<%= _schema._className %>Info([UsingKeyParameters]).Description
        End Function

        Public Shared Function GetInfoList(pFilters As Dictionary(Of String, String)) As <%= _schema._className %>InfoList
            Dim _qd = Query.BuildQD(pFilters)
            Dim _sqlText = _qd.BuildSQL

            Return DataPortal.Fetch(Of <%= _schema._className %>InfoList)(New FilterCriteria() With {._sqlText = _sqlText})
        End Function

        Public Shared Function Get<%= _schema._className %>InfoList() As <%= _schema._className %>InfoList
            If _list Is Nothing Or _DTB &lt;&gt; Context.CurrentBECode Then

                _DTB = Context.CurrentBECode
                _list = DataPortal.Fetch(Of <%= _schema._className %>InfoList)(New FilterCriteria())

            End If
            Return _list
        End Function

        Public Shared Sub InvalidateCache()
            _list = Nothing
            [invalidateDictionary]
        End Sub
                      
        Public Shared Function ContainsCode([DeclareKeyParameters], Optional ByRef RetInfo As <%= _schema._className %>Info = Nothing) As Boolean

           RetInfo = <%= _schema._className %>Info.Empty<%= _schema._className %>Info([UsingKeyParameters])
            If Get<%= _schema._className %>Dic.ContainsKey([UsingKeyParameters]) Then
                RetInfo =  Get<%= _schema._className %>Dic([UsingKeyParameters])
                Return True
            EndIf
            Return False

        End Function

        Public Shared Function ContainsCode(ByVal Target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim value As String = CType(CallByName(Target, e.PropertyName, CallType.Get), String)
            'no thing to check
            If String.IsNullOrEmpty(value) Then Return True

            If ContainsCode(value) Then
                Return True
            Else
                e.Description = String.Format(ResStr(Msg.NOSUCHITEM), ResStr("<%= _schema._className %>"), value)
                Return False
            End If
        End Function

#End Region ' Factory Methods
                    </template>

        Dim template As XElement
        If _schema.Keys.Count > 1 Then
            template = MultiKeytemplate
        ElseIf _schema.Keys.Count = 1 Then
            template = SingleKeytemplate
        Else
            template = SingleKeytemplate
        End If

        str.Append(template.Value)
        str.Replace("[DeclareKeyParameters]", DeclareKeyParameters)
        str.Replace("[UsingKeyParameters]", UsingKeyParameters)
        str.Replace("[UsingKeyFields]", UsingKeyFields)

        str.Replace("[invalidateDictionary]", <inv>_<%= _schema._className.ToLower %>Dic = Nothing</inv>.Value)

        Return str.ToString
    End Function

    Private Function DeclareKeyParameters() As String
        Dim _GetParamsBlock As New StringBuilder
        If _schema.Keys.Count = 0 Then Return String.Empty

        For Each f In _schema.Keys
            _GetParamsBlock.AppendFormat("ByVal  p{0} as {1},", f._id.toPropertyName, f._fieldType)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function
    Private Function DeclareKeyParameters_Optional() As String
        Dim _GetParamsBlock As New StringBuilder
        If _schema.Keys.Count = 0 Then Return String.Empty

        For Each f In _schema.Keys
            _GetParamsBlock.AppendFormat("Optional ByVal  p{0} as {1} = """",", f._id.toPropertyName, f._fieldType)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function
    Private Function UsingKeyParameters() As String
        Dim _GetParamsBlock As New StringBuilder

        If _schema.Keys.Count = 0 Then Return String.Empty

        For Each f In _schema.Keys
            _GetParamsBlock.AppendFormat("p{0},", f._id.toPropertyName)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(_GetParamsBlock.ToString, ",$", String.Empty)
    End Function
    Private Function ParsingID_Block() As String
        Dim str As New StringBuilder
        If _schema.Keys.Count > 1 Then
            str.AppendLine(" 'Dim m As MatchCollection = Regex.Match(ID, pbsRegex.AlphaNumericExt)")
            For i = 0 To _schema.Keys.Count - 1
                Dim nextMatch = String.Empty
                If i > 0 Then
                    For matchIdx = 1 To i
                        nextMatch += ".NextMatch"
                    Next
                End If
                str.AppendFormat("Dim p{0} as {1} = Regex.Match(ID, pbsRegex.AlphaNumericExt){2}.Value.Trim{3}{4}", _schema.Keys(i)._id.toPropertyName, _schema.Keys(i)._fieldType, nextMatch, System.Environment.NewLine, If(_schema.Keys(i)._fieldType.MatchesRegExp("Integer"), ".ToInteger", ""))
            Next
        ElseIf _schema.Keys.Count = 1 Then
            str.AppendFormat("Dim p{0} as {1} = id.Trim{3}{2}", _schema.Keys(0)._id.toPropertyName, _schema.Keys(0)._fieldType, System.Environment.NewLine, If(_schema.Keys(0)._fieldType.MatchesRegExp("Integer"), ".ToInteger", ""))
        End If

        Return str.ToString

    End Function
    Private Function Cloning_Block() As String
        Dim str As New StringBuilder
        If _schema.Keys.Count = 0 Then Return String.Empty
        For Each f In _schema.Keys
            str.AppendFormat("cloning{0}.{1}{2} = p{3}{4}", _
                             _schema._className, _
                             f._id.toMemberName, _
                             If(f.FieldType.MatchesRegExp("^Smart"), ".Text", String.Empty), _
                             f._id.toPropertyName, _
                             System.Environment.NewLine)
        Next

        Return str.ToString
    End Function
    Private Function AssignKeys_Block() As String
        Dim str As New StringBuilder
        If _schema.Keys.Count = 0 Then Return String.Empty
        For Each f In _schema.Keys
            str.AppendFormat("{0}{1} = p{2}{3}", _
                              f._id.toMemberName, _
                             If(f.FieldType.MatchesRegExp("^Smart"), ".Text", String.Empty), _
                             f._id.toPropertyName, _
                             System.Environment.NewLine)
        Next

        Return str.ToString
    End Function

#End Region

#Region "Exists"

    Friend Function GetExists() As String
        Dim ExistQuery As String = "<SqlText>SELECT COUNT(*) FROM _DBTable WHERE DTB='<%= Context.CurrentBECode %>' CRITERIA</SqlText>.Value.Trim".Replace("_DBTable", _schema._DBTable).Replace("CRITERIA", WHERECriteria(True))
        Dim str As New StringBuilder

        Dim template = <template>
#Region " Exists "
        Public Shared Function Exists(ByVal <%= If(_schema.Keys.Count = 1, UsingKeyParameters(), "ID") %> As String) As Boolean
            'Return <%= _schema._className %>InfoList.<%= If(_schema.Keys.Count > 1, "ContainsID", "ContainsCode") %>(<%= If(_schema.Keys.Count = 1, UsingKeyParameters(), "ID") %>)
            Return KeyDuplicated(<%= If(_schema.Keys.Count = 1, UsingKeyParameters(), "ID") %>)
        End Function

        Public Shared Function KeyDuplicated(<%= DeclareKeyParameters() %>) As Boolean
        
            'Dim theLineNo as Integer = pLineNo.ToInterger
            'If theLineNo &lt;=0 then Return False

            Dim SqlText = <%= ExistQuery %>
            Return SQLCommander.GetScalarInteger(SqlText) > 0
        End Function
#End Region
                       </template>

        str.Append(template.Value)
        ' str.Replace("[DeclareKeyParameters]", DeclareKeyParameters)
        ' str.Replace("[UsingKeyParameters]", UsingKeyParameters)
        ' str.Replace("[Declare_Criteria_Members_Block]", Declare_Criteria_Members_Block)
        ' str.Replace("[Fill_KeyMembers_Criteria]", Fill_KeyMembers_Criteria)

        Return str.ToString

    End Function

#End Region

#Region "IGenpart"
    Friend Function GetIGenpart() As String

        Dim template = <template>
#Region " IGenpart "

         Public Function CloneBO(ByVal id As String) As Object Implements Interfaces.IGenPartObject.CloneBO
            Return Clone<%= _schema._className %>(id)
        End Function

        Public Function getBO1(ByVal id As String) As Object Implements Interfaces.IGenPartObject.getBO
            Return GetBO(id)
        End Function

        Public Function myCommands() As String() Implements Interfaces.IGenPartObject.myCommands
            Return pbs.Helper.Action.StandardReferenceCommands
        End Function

        Public Function myFullName() As String Implements Interfaces.IGenPartObject.myFullName
            Return GetType(<%= _schema._className %>).ToString
        End Function

        Public Function myName() As String Implements Interfaces.IGenPartObject.myName
            Return GetType(<%= _schema._className %>).ToString.Leaf
        End Function

        Public Function myQueryList() As IList Implements Interfaces.IGenPartObject.myQueryList
            Return <%= _schema._className %>InfoList.Get<%= _schema._className %>InfoList
        End Function
#End Region                           
                       </template>

        Return template.Value
    End Function
#End Region

#Region "IDoclink"
    Friend Function GetIDoclink() As String

        Dim template = <template>
#Region "IDoclink"
        Public Function Get_DOL_Reference() As String Implements IDocLink.Get_DOL_Reference
            Return String.Format("{0}#{1}", Get_TransType,<%= _schema.Keys(0)._id.toMemberName %>)
        End Function

        Public Function Get_TransType() As String Implements IDocLink.Get_TransType
            Return Me.GetType.ToClassSchemaName.Leaf
        End Function
#End Region                          
                       </template>

        Return template.Value
    End Function
#End Region

#Region "UIGenPart"

    Private Function UsingKeyTextBox() As String
        Dim str As New StringBuilder

        If _schema.Keys.Count = 0 Then Return String.Empty

        For Each f In _schema.Keys
            str.AppendFormat("{0}KryptonTextBox.Text.Trim(),", f._id.toPropertyName)
        Next
        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(str.ToString, ",$", String.Empty)
    End Function 'array of all keys, splitted by commas

    Friend Function GetUIGenPart() As String
        Dim str = New StringBuilder
        Dim template = <template>
Imports System.Windows.Forms
Imports Csla.Data
Imports pbs.Helper
Imports pbs.BO.<%= _schema._nameSpace %>
Imports pbs.BO
Imports System.XML

<%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "Namespace " & _schema._nameSpace) %>
    Public Class <%= _schema._className %>UI
        Inherits UIGenPart

        Public Overrides ReadOnly Property PartTitle() As String
            Get
                Return ResStr("<%= _schema._className %>")
            End Get
        End Property
        Private Property _<%= _schema._className %>() As <%= _schema._className %>
            Get
                Return CType(_obj, <%= _schema._className %>)
            End Get
            Set(ByVal value As <%= _schema._className %>)
                _obj = value
            End Set
        End Property

        Public Sub New()
            Me.InitializeComponent()
            Me.ErrorProvider1.RightToLeft = True
            _<%= _schema._className %> = <%= _schema._className %>.Blank<%= _schema._className %>
        End Sub

        Public Sub New(ByVal Args As pbsCmdArgs)
            Me.InitializeComponent()
            Me.ErrorProvider1.RightToLeft = True
            _<%= _schema._className %> = <%= _schema._className %>.Blank<%= _schema._className %>

            If Args Is Nothing Then Exit Sub
            If String.IsNullOrEmpty(args.ID) Then Exit Sub
            Try
                _<%= _schema._className %> = <%= _schema._className %>.Get<%= _schema._className %>(args.ID)
            Catch ex As Exception
                SendMsg(String.Format(ResStr("NoSuchItem"), ResStr("<%= _schema._className %>"), Args.ID))
            End Try
        End Sub

        Private Sub <%= _schema._className %>UI_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            BindBO()
            Me.UIReset()
        End Sub

#Region " BO engine overrides "
        Private Sub CommitCommand() Handles MyBase.onCommandCommit
            Dim cmd = Me.WP.Tag
            'confirm data accept
            If _state = State.DataAccept Then
                If Save<%= _schema._className %>() Then Complete(cmd)
                Exit Sub
            End If

            'Key fileds entered , Excecute command with parameter
            Me.Execute(cmd)
        End Sub

        Protected Overrides Sub InvalidateInfoList(ByVal cmd As String)
               If cmd = Action._Create OrElse cmd = Action._Delete OrElse cmd = Action._Copy OrElse cmd = Action._Amend _
                                Then <%= _schema._className %>InfoList.InvalidateCache()
        End Sub

#End Region

#Region "Must override functions "

        Protected Overrides Function BOExists() As Boolean
            Return <%= _schema._className %>.Exists([UsingKeyTextBox])
        End Function

        Protected Overrides Sub Copy()
            Try
                Dim _<%= _schema._className %>Copy As <%= _schema._className %> = _<%= _schema._className %>.Clone<%= _schema._className %>([UsingKeyTextBox])
                _<%= _schema._className %> = _<%= _schema._className %>Copy
                BindBO()
                Me.UIAmend()
            Catch ex As Exception
                SendMsg(ex)
            End Try

        End Sub

        Protected Overrides Function NewBO() As Boolean
            Dim ret As Boolean = False
            Try
                _<%= _schema._className %> = <%= _schema._className %>.New<%= _schema._className %>([UsingKeyTextBox])
                ret = True
                BindBO()
            Catch ex As Exception
                SendMsg(ex)
            End Try
            Return ret

        End Function

        Protected Overrides Function GetBO() As Boolean
            Dim ret As Boolean = False
            Try
                _<%= _schema._className %> = <%= _schema._className %>.Get<%= _schema._className %>([UsingKeyTextBox])
                ret = True
                BindBO()

            Catch ex As Exception
                SendMsg(ex)
            End Try
            Return ret

        End Function

        Protected Overrides Function DeleteBO() As Boolean
            Dim ret As Boolean = False
            Try
                <%= _schema._className %>.Delete<%= _schema._className %>([UsingKeyTextBox])
                ret = True
            Catch ex As Exception
                SendMsg(ex)
            End Try
            Return ret
        End Function

        Private Function Save<%= _schema._className %>() As Boolean
            Dim ret As Boolean = False
            Try
                _<%= _schema._className %>.ApplyEdit()
                _<%= _schema._className %>.Save()
                ret = True
                BindBO()
            Catch ex As Exception
                SendMsg(ex)
            End Try
            Return ret
        End Function

        Protected Overrides Sub BindBO()
            Me.<%= _schema._className %>BindingSource.DataSource = _<%= _schema._className %>
        End Sub

        Protected Overrides Sub FocusFirstKey()
            Me.<%= _schema.Keys(0)._id.toPropertyName %>KryptonTextBox.Focus()
        End Sub

        Protected Overrides Sub FocusDataFields()
            Me.<%= (From f In _schema.Members Where Not f.IsPK Select f._id.toPropertyName Take 1).SingleOrDefault %>KryptonTextBox.Focus()
        End Sub

        Protected Overrides Sub CancelEdit()
            _<%= _schema._className %>.CancelEdit()
        End Sub

        Protected Overrides Function GetMyReportDataset() As List(Of DataTable)
            Return <%= _schema._className %>InfoList.GetMyReportDataset
        End Function

        Protected Overrides Sub TransferIn()
            Try
                Dim td As New pbs.UI.TIDL(Of <%= _schema._className %>) _
                (<%= String.Join(",", (From f In _schema.Keys Select """" & f._id.toPropertyName & """").ToArray) %>, <%= _schema._className %>.Blank<%= _schema._className %>, ResStr(WIP.TransferInPrompt))
                td.ShowDialog()
            Catch ex As Exception
                SendMsg(ex)
            End Try
        End Sub
#End Region

    End Class

<%= If(String.IsNullOrEmpty(_schema._nameSpace), String.Empty, "End Namespace ") %>
                       </template>
        str.Append(template.Value)

        str.Replace("[UsingKeyTextBox]", UsingKeyTextBox)
        Return str.ToString
    End Function

#End Region

End Class
