Imports System.Text
Public Class ReadonlyRootList
    Inherits CSLABuilder

    Dim template As XDocument = <?xml version="1.0"?>
                                <code>
Imports Csla
Imports Csla.Data
Imports System.XML
Imports System.XML.Xsl
Imports spc.Helper
Imports spc.BusinessContext
Imports spc.Authorizator

                                    &lt;Serializable()&gt; _
Public Class [objectClass]InfoList
    Inherits Csla.ReadOnlyListBase(Of [objectClass]InfoList, [objectClass]Info)

    Private Shared _list As [objectClass]InfoList
    Private Shared _DTB As String

#Region " Transfer out "

    Public Shared Function TransferOut(ByVal Code_From As String, ByVal Code_To As String, ByVal FileName As String) As Integer
        Dim _dt As New DataTable(GetType([objectClass]).ToString)
        Dim oa As New ObjectAdapter()

        For Each info As [objectClass]Info In [objectClass]InfoList.Get[objectClass]InfoList
            If info.Code &gt;= Code_From And info.Code &lt;= Code_To Then
                oa.Fill(_dt, [objectClass].GetBO(info.MyID))
            End If
        Next

        Try
            _dt.Columns.Remove("IsNew")
            _dt.Columns.Remove("IsValid")
            _dt.Columns.Remove("IsSavable")
            _dt.Columns.Remove("IsDeleted")
            _dt.Columns.Remove("IsDirty")
            _dt.Columns.Remove("BrokenRulesCollection")
        Catch ex As Exception
        End Try

        For Each col As DataColumn In _dt.Columns
                col.ColumnMapping = MappingType.Attribute
        Next

        _dt.WriteXml(FileName)
        Return _dt.Rows.Count
    End Function

#End Region

#Region " Factory Methods "

    Private Sub New()
        'require use of factory method
        _DTB = Context.CurrentBusinessEntityCode
    End Sub

    Public Shared Function GetDescription(ByVal [ID] as string) As String
        Dim info As [objectClass]Info = Nothing
        If Contains[ID]([ID], info) Then
            Return info.Description
        End If
        Return String.Empty
    End Function
    Public Shared Function Get[objectClass]Info(ByVal [ID] As String) As CAInfo
            Dim Info As [objectClass]Info = [objectClass]Info.Empty[objectClass]Info([ID])
            Contains[ID]([ID], Info)
            Return Info
    End Function

    Public Shared Function Get[objectClass]InfoList() As [objectClass]InfoList
         If _list Is Nothing Orelse _DTB &lt;&gt; Context.CurrentBusinessEntityCode Then
            _DTB = Context.CurrentBusinessEntityCode
            _list = DataPortal.Fetch(Of [objectClass]InfoList)()
        End If
        Return _list
    End Function

    Public Shared Sub InvalidateCache()
        _list = Nothing
    End Sub

    Public Shared Function ContainsCode(ByVal [ID] As String, Optional ByRef RetInfo As [objectClass]Info = Nothing) As Boolean
        Dim ret = From info In Get[objectClass]InfoList() _
                  Where info.MyID.ToLower.Trim = [ID].ToLower.Trim _
                  Take 1 _
                  Select info

        If ret.Count > 0 Then
            RetInfo = ret.Single
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function ContainsCode(ByVal Target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim value As String = CType(CallByName(Target, e.PropertyName, CallType.Get), String)
        If ContainsCode(value) Then
            Return True
        Else
            e.Description = String.Format(Localizator.Str(Msg.NOSUCHITEM), Localizator.Str("[objectClass]"), value)
            Return False
        End If
    End Function

#End Region ' Factory Methods

#Region " Data Access "

    Private Overloads Sub DataPortal_Fetch()
        RaiseListChangedEvents = False
        Using ctx = ContextManager(Of AuthorizatorDataContext).GetManager(Context.ConnectionString, False)
            'todo: check this
            Dim data = From r In ctx.DataContext.[DBTable]s _
                       Where r.SUN_DB = _DTB _ 
                       Select New [objectClass]Info(r)

            IsReadOnly = False
            Me.AddRange(data)
            IsReadOnly = True
        End Using
        RaiseListChangedEvents = True

    End Sub

#End Region ' Data Access

End Class
                   </code>

    Public Function GenCode() As String
        Dim theCode As New StringBuilder
        theCode.Append(template...<code>.Value)

        If _keyCount = 1 Then
            theCode.Replace("[ID]", "Code")
        ElseIf _keyCount > 1 Then
            theCode.Replace("[ID]", "ID")
        End If
        
        theCode.Replace("[objectClass]", MyBase._objectClass)
        theCode.Replace("[DBTable]", MyBase._DBTable)

        Return theCode.ToString
    End Function

End Class
