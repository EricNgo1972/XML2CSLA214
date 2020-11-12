Imports System.Text
Partial Class CSLAFactory

#Region "Data Access"

    Private Function Fill_KeyMembers_Criteria(Optional ByVal withDot As Boolean = False) As String
        Dim str As New StringBuilder

        If _schema.Keys.Count = 0 Then Return String.Empty
        For Each f In _schema.Keys
            str.AppendFormat("{3}{0} = p{1}{2}", _
                             f._id.toMemberName, _
                             f._id.toPropertyName, _
                             System.Environment.NewLine, _
                             If(withDot, ".", String.Empty))
        Next

        Return str.ToString
    End Function 'populate members of criteria
    Private Function Fill_Members_FromParameters() As String
        Dim str As New StringBuilder

        If _schema.Keys.Count = 0 Then Return String.Empty
        For Each f In _schema.Members
            str.AppendFormat("{0} = p{1}{2}", f._id.toMemberName, f._id.toPropertyName, System.Environment.NewLine)
        Next

        Return str.ToString
    End Function 'populate members of criteria
    Private Function Declare_Criteria_Members_Block() As String
        Dim str As New StringBuilder
        For Each f In _schema.Keys
            str.AppendFormat("Public {0} as {1} = [DefaultValue]{2}", f._id.toMemberName, f._fieldType, System.Environment.NewLine)
            If Not String.IsNullOrEmpty(f.DefaultValue) Then
                str.Replace("= [DefaultValue]", String.Format(" = {0}", f.DefaultValue))
            Else
                str.Replace("= [DefaultValue]", String.Empty)
            End If
        Next
        Return str.ToString
    End Function
    Private Function Fill_KeyMembers() As String
        Dim str As New StringBuilder

        If _schema.Keys.Count = 0 Then Return String.Empty
        For Each f In _schema.Keys
            str.AppendFormat("{0} = criteria.{0}{1}", f._id.toMemberName, System.Environment.NewLine)
        Next

        Return str.ToString
    End Function 'populate members of BO

    Private Function Fill_Members(Optional ByVal withDot As Boolean = False) As String
        Dim str As New StringBuilder

        If _schema.Members.Count = 0 Then Return String.Empty
        For Each f In _schema.Members
            '_transdate.Text = _nc.TRANS_DATE.Trim
            str.AppendFormat("{0}{1}{2} = _{3}.{4}{5}{6}",
                             If(withDot, ".", String.Empty),
                             f._id.toMemberName,
                             If(f._fieldType.MatchesRegExp("^Smart"), ".Text", String.Empty),
                             "row",
                             f._id.toPropertyName.DBColumnName,
                             If(f.FieldType.MatchesRegExp("String|Text|Smart"), ".Trim", String.Empty),
                             System.Environment.NewLine)
        Next

        Return str.ToString
    End Function 'fetch data 

    Private Function Fill_Merged_Members(Optional ByVal withDot As Boolean = False) As String
        Dim str As New StringBuilder

        If _schema.Members.Count = 0 Then Return String.Empty
        For Each f In _schema.Members
            str.AppendFormat("{5}{0}{1} = ClassSchema(Of {2}).GetSubData(""{0}"",_{2}.{3}).TrimEnd{4}", _
                                f._id.toMemberName, _
                                If(f._fieldType.MatchesRegExp("^Smart"), ".Text", String.Empty), _
                                _schema._className, _
                                If(f.IsPK, "KEY_FIELDS", "SUN_DATA"), _
                                System.Environment.NewLine, _
                                If(withDot, ".", String.Empty))
        Next
        Return str.ToString
    End Function 'fetch data

    Private Function Fill_MembersFrom_Dr(Optional ByVal withDot As Boolean = False) As String
        Dim str As New StringBuilder

        If _schema.Members.Count = 0 Then Return String.Empty
        For Each f In _schema.Members
            If f._id <> "DTB" AndAlso f._id <> "SITE" Then
                '_employeeDate.Text =  dr.GetInt32("EmployeeDate")

                str.AppendFormat("{0}{1}{2} = {3}{4}",
                                 If(withDot, ".", String.Empty),
                                 f._id.toMemberName,
                                 If(f._fieldType.MatchesRegExp("^Smart"), ".Text", String.Empty),
                                 f.Safe_Read_Field,
                                 System.Environment.NewLine)
            End If
        Next

        Return str.ToString
    End Function 'fetch data from safe data reader

    Private Function Fill_InsertUpdate_Merged_Fields() As String
        Dim str As New StringBuilder

        If _schema.Members.Count = 0 Then Return String.Empty
        For Each f In _schema.Members
            If Not f.IsPK Then
                str.AppendFormat("ClassSchema(Of {2}).SetSubData(""{0}"",{0}{1},_data){3}", _
                                    f._id.toMemberName, _
                                    If(f._fieldType.MatchesRegExp("^Smart"), ".DBValue", String.Empty), _
                                    _schema._className, _
                                    System.Environment.NewLine)
            Else
                str.AppendFormat("ClassSchema(Of {2}).SetSubData(""{0}"",{0}{1},_key){3}", _
                                 f._id.toMemberName, _
                                 If(f._fieldType.MatchesRegExp("^Smart"), ".DBValue", String.Empty), _
                                 _schema._className, _
                                 System.Environment.NewLine)
            End If
        Next
        Return str.ToString
    End Function 'parameters for InsertUpdate Stored Procedures

    ''' <summary>
    ''' Style 3.5. use sql linq. this is not safe. call stored procedure from linq is not safe
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertUpdate_Parameters() As String
        Dim str As New StringBuilder

        If _schema.Members.Count = 0 Then Return String.Empty

        For Each f In _schema.Members
            Dim tails As String = String.Empty
            If f._fieldType.MatchesRegExp("Smart") Then
                tails = ".DBValue"
            ElseIf f._fieldType.MatchesRegExp("String") Then
                tails = ".Trim"
            End If
            str.AppendFormat("{0}{1},", f._id.toMemberName, tails)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(str.ToString, ",$", String.Empty)

    End Function 'parameters for InsertUpdate Stored Procedures,array of all members, splitted by commas

    Private Function Add_InsertUpdate_Parameters() As String
        Dim str As New StringBuilder

        If _schema.Members.Count = 0 Then Return String.Empty
        str.AppendLine(<code>   Private Sub AddInsertParameters(ByVal cm As SqlCommand)</code>.Value)

        For Each f In _schema.Members
            If f._isAutoNumber.ToBoolean Then

                str.AppendFormat(<code> cm.Parameters.AddWithValue("@<%= f._id %>", <%= f.Property_DB %>, ParameterDirection.Output)</code>.Value)
                str.AppendLine()
                str.AppendFormat(<code><%= f._id %> = cm.GetReturnIdAsInteger(@<%= f.Property_DB %>)</code>.Value)
                str.AppendLine("'Move this to insert")
                str.AppendLine()
            Else
                str.AppendFormat(<code> cm.Parameters.AddWithValue("@<%= f._id %>", <%= f.Property_DB %>)</code>.Value)
            End If

            str.AppendLine()
        Next
        str.AppendLine(<code>   End Sub</code>.Value)

        Return str.ToString
    End Function

    Private Function KeysFetchWithCriteria() As String
        Dim str As New StringBuilder
        If Not String.IsNullOrEmpty(_schema._subTableCode) Then
            str.AppendLine(<Code>AndAlso _<%= _schema._className %>.PBS_TB = <%= _schema._subTableCode %></Code>.Value)
        End If
        For i = 0 To _schema.Keys.Count - 1
            str.AppendLine(<Code>   AndAlso _<%= _schema._className %>.<%= _schema.Keys(i)._id %> = criteria.<%= _schema.Keys(i)._id.toMemberName %></Code>.Value)
        Next
        Return str.ToString
    End Function

    Private Function WHERECriteria(Optional sharedMode As Boolean = False) As String
        Dim str As New StringBuilder

        If Not String.IsNullOrEmpty(_schema._subTableCode) Then
            If _schema.Keys.Count = 1 Then
                str.AppendLine(<Code> AND PBS_TB='<%= _schema._subTableCode %>' AND KEY_FIELDS='{criteria.<%= _schema.Keys(0)._id.toMemberName %>}'</Code>.Value)
            Else
                str.AppendLine(<Code> AND PBS_TB='<%= _schema._subTableCode %>' AND KEY_FIELDS='{criteria.DBKey}'</Code>.Value)
            End If
        Else
            For i = 0 To _schema.Keys.Count - 1
                str.Append(<Code> AND <%= _schema.Keys(i)._id %>= '{criteria.<%= _schema.Keys(i)._id.toMemberName %>}'</Code>.Value)
            Next
        End If

        Dim ret = str.ToString.Replace("{", "<%= ")
        ret = ret.Replace("}", " %>")
        If sharedMode Then ret = ret.Replace("criteria._", "p")
        Return ret
    End Function

    Private Function SubTableFetchWithCriteria() As String
        Dim str As New StringBuilder
        If Not String.IsNullOrEmpty(_schema._subTableCode) Then
            str.AppendLine(<Code>AndAlso _row.PBS_TB = <%= _schema._subTableCode %></Code>.Value)
        End If
        Return str.ToString
    End Function

    Private Function UsingKeyFields() As String
        Dim str As New StringBuilder

        If _schema.Keys.Count = 0 Then Return String.Empty

        For Each f In _schema.Keys
            str.AppendFormat("{0},", f._id.toMemberName)
        Next

        'eliminate ending ,
        Return System.Text.RegularExpressions.Regex.Replace(str.ToString, ",$", String.Empty)
    End Function 'array of all keys, splitted by commas
    Private Function GetDataAccess_StandAloneTable() As String
        Dim str As New StringBuilder

        Dim DELETEQuery As String = "<SqlText>DELETE FROM _DBTable WHERE PBS_DB='<%= _DTB %>' CRITERIA </SqlText>.Value.Trim".Replace("_DBTable", _schema._DBTable).Replace("CRITERIA", WHERECriteria)
        Dim SelectQuery As String = "<SqlText>SELECT * FROM _DBTable WHERE PBS_DB='<%= _DTB %>' CRITERIA </SqlText>.Value.Trim".Replace("_DBTable", _schema._DBTable).Replace("CRITERIA", WHERECriteria)

        Dim template = <template>
#Region " Data Access "

        &lt;Serializable()&gt; _
        Private Class Criteria
           <%= Declare_Criteria_Members_Block() %>
            Public Sub New(<%= DeclareKeyParameters() %>)
               <%= Fill_KeyMembers_Criteria() %>
            End Sub
        End Class

        &lt;RunLocal()&gt; _
        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)
            <%= Fill_KeyMembers() %>
            ValidationRules.CheckRules()
        End Sub

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
           Using ctx = ConnectionFactory.GetDBConnection
                Using cm =  ctx.CreateSQLCommand
                    cm.CommandType = CommandType.Text
                    cm.CommandText = <%= SelectQuery %>

        Using dr As New SafeDataReader(cm.ExecuteReader)
            If dr.Read Then
                FetchObject(dr)
                MarkOld()
            End If
        End Using

                End Using
            End Using
        End Sub

    Private Sub FetchObject(ByVal dr As SafeDataReader)
           <%= Fill_MembersFrom_Dr() %>
        End Sub

    Private Shared _lockObj As New Object
    Protected Overrides Sub DataPortal_Insert()
        SyncLock _lockObj
            Using ctx = ConnectionFactory.GetDBConnection(true)
                Using cm  = ctx.CreateSQLCommand

                    cm.CommandType = CommandType.StoredProcedure
                    cm.CommandText = "pbs_<%= _schema._className %>_InsertUpdate"
                                                  
                    AddInsertParameters(cm)
                    cm.ExecuteNonQuery()

                End Using
            End Using
        End SyncLock
    End Sub

      <%= Add_InsertUpdate_Parameters() %>   

    Protected Overrides Sub DataPortal_Update()
        Using ctx = ConnectionFactory.GetDBConnection(true)
                Using cm  = ctx.CreateSQLCommand

                    cm.CommandType = CommandType.StoredProcedure
                    cm.CommandText = "pbs_<%= _schema._className %>_InsertUpdate"

                    AddInsertParameters(cm)
                    cm.ExecuteNonQuery()

                End Using
            End Using
    End Sub

    Protected Overrides Sub DataPortal_DeleteSelf()
        DataPortal_Delete(New Criteria(<%= UsingKeyFields() %>))
    End Sub

    Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
        Using ctx = ConnectionFactory.GetDBConnection(true)
            Using cm = ctx.CreateSQLCommand

                cm.CommandType = CommandType.Text
                cm.CommandText = <%= DELETEQuery %>
                cm.ExecuteNonQuery()

            End Using
        End Using

    End Sub
                   
#End Region 'Data Access                           
                       </template>

        str.Append(template.Value)

        str.Replace("[Fill_KeyMembers_Criteria]", Fill_KeyMembers_Criteria)
        str.Replace("[Declare_Criteria_Members_Block]", Declare_Criteria_Members_Block)
        str.Replace("[DeclareKeyParameters]", DeclareKeyParameters)
        str.Replace("[Fill_KeyMembers]", Fill_KeyMembers)
        str.Replace("[Fill_KeyMembers_Criteria]", Fill_KeyMembers_Criteria)
        str.Replace("[Fill_Members]", Fill_Members)
        str.Replace("[InsertUpdate_Parameters]", InsertUpdate_Parameters)
        str.Replace("[UsingKeyParameters]", UsingKeyParameters)
        str.Replace("[UsingKeyFields]", UsingKeyFields)

        Return str.ToString

    End Function
    Private Function GetDataAccess_Merged() As String
        Dim str As New StringBuilder
        Dim template = <template>
#Region " Data Access "

        &lt;Serializable()&gt; _
        Private Class Criteria
           [Declare_Criteria_Members_Block]
            Public Sub New([DeclareKeyParameters])
               [Fill_KeyMembers_Criteria]
            End Sub
        End Class

        &lt;RunLocal()&gt; _
        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)
            [Fill_KeyMembers]
            ValidationRules.CheckRules()
        End Sub

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
            Using ctx = ContextManager(Of <%= _schema._nameSpace %>DataContext).GetDataContext
                'todo: Manual edit LINQ 
                Dim qry = From _<%= _schema._className %> In ctx.DataContext.<%= _schema._DBTable %>s _
                          Where _<%= _schema._className %>._DTB = _DTB  <%= KeysFetchWithCriteria() %>

                For Each _<%= _schema._className %> In qry
                    _data = _<%= _schema._className %>._DATA.TrimEnd
                   [Fill_Merged_Members]
                Next

            End Using

        End Sub

        Protected Overrides Sub DataPortal_Insert()
            Using ctx = ContextManager(Of <%= _schema._nameSpace %>DataContext).GetDataContext
                dim _key as string = String.Empty
                [Fill_InsertUpdate_Merged_Fields]
                ctx.DataContext.<%= _schema._DBTable %>_InsertUpdate(_DTB,_PBS_TB,_key,_lookup,ToDay().ToSunDate,_data)
            End Using
        End Sub

        Protected Overrides Sub DataPortal_Update()
           DataPortal_Insert
        End Sub

        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria([UsingKeyFields]))
        End Sub

        Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
            Using ctx = ContextManager(Of <%= _schema._nameSpace %>DataContext).GetDataContext
                'Todo: Manual edit LINQ
                Dim qry = From _<%= _schema._className %> In ctx.DataContext.<%= _schema._DBTable %>s _
                          Where _<%= _schema._className %>.SUN_DB = _DTB <%= KeysFetchWithCriteria() %>

                ctx.DataContext.<%= _schema._DBTable %>s.DeleteAllOnSubmit(qry)

                ctx.DataContext.SubmitChanges()

            End Using

        End Sub

        Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As Csla.DataPortalEventArgs)
            If Csla.ApplicationContext.ExecutionLocation = ExecutionLocations.Server Then
                <%= _schema._className %>InfoList.InvalidateCache()
            End If
        End Sub


#End Region 'Data Access                           
                       </template>

        str.Append(template.Value)

        str.Replace("[Fill_KeyMembers_Criteria]", Fill_KeyMembers_Criteria)
        str.Replace("[Declare_Criteria_Members_Block]", Declare_Criteria_Members_Block)
        str.Replace("[Fill_KeyMembers]", Fill_KeyMembers)
        str.Replace("[Fill_KeyMembers_Criteria]", Fill_KeyMembers_Criteria)
        str.Replace("[Fill_Members]", Fill_Members)
        str.Replace("[InsertUpdate_Parameters]", InsertUpdate_Parameters)
        str.Replace("[UsingKeyParameters]", UsingKeyParameters)
        str.Replace("[DeclareKeyParameters]", DeclareKeyParameters)
        str.Replace("[UsingKeyFields]", UsingKeyFields)
        str.Replace("[Fill_Merged_Members]", Fill_Merged_Members)
        str.Replace("[Fill_InsertUpdate_Merged_Fields]", Fill_InsertUpdate_Merged_Fields)
        Return str.ToString

    End Function

    Friend Function GetDataAccess_Editable() As String
        If _schema._DBMergeMode Then
            Return GetDataAccess_Merged()
        Else
            Return GetDataAccess_StandAloneTable()
        End If
    End Function

    Friend Function GetDataAccess_ChildEditable() As String
        Dim template = <txt>
#Region " Data Access - children"

       ' &lt;Serializable()&gt; _
       'Private Class Criteria
       '     Public Sub New()
       '     End Sub
       ' End Class

#Region " Data Access - Fetch"

        'Private Sub Fetch(ByVal dr As SafeDataReader)
        '    FetchObject(dr)
        '    MarkOld()
        'End Sub

        Private Sub FetchObject(ByVal dr As SafeDataReader)
           <%= Fill_MembersFrom_Dr() %>
        End Sub

#End Region ' Data Access - Fetch

#Region " Data Access - Insert "

        Friend Sub Insert(ByVal cn As IDbConnection)
            If Not IsDirty Then Return
            ExecuteInsert(cn)
            MarkOld()
        End Sub

        Private Sub ExecuteInsert(ByVal cn As IDbConnection)
            Using cm = cn.CreateSQLCommand()
                cm.CommandType = CommandType.StoredProcedure
                cm.CommandText = "pbs_<%= _schema._className %>_InsertUpdate"
                AddInsertParameters(cm)
  'cm.Parameters.AddWithValue("@LINE_NO", _lineNo).Direction = ParameterDirection.Output              
                cm.ExecuteNonQuery()
  '_lineNo = cm.Parameters("@LINE_NO").Value.ToString.ToInteger
            End Using
        End Sub

   <%= Add_InsertUpdate_Parameters() %>

        Friend Sub Update(ByVal cn As IDbConnection)
            If Not IsDirty Then Return
            ExecuteUpdate(cn)
            MarkOld()
        End Sub

 Private Sub ExecuteUpdate(ByVal cn As IDbConnection)
            Using cm = cn.CreateSQLCommand()
                cm.CommandType = CommandType.StoredProcedure
                cm.CommandText = "pbs_<%= _schema._className %>_InsertUpdate"
                AddInsertParameters(cm)
  'cm.Parameters.AddWithValue("@LINE_NO", _lineNo)             
                cm.ExecuteNonQuery()
 
            End Using
        End Sub


#End Region ' Data Access - Insert Update

#Region " Data Access - Delete "

    Friend Sub DeleteSelf(ByVal cn As IDbConnection)
            If Not IsDirty Then Return
            If IsNew Then Return
            Dim _DTB = Context.CurrentBECode
            Dim sqlText = &lt;Script&gt;DELETE SCRIPT &lt;/Script&gt;.Value
        Using cm = cn.CreateCommand()
            cm.CommandType = CommandType.Text
            cm.CommandText = sqlText.Trim

            cm.ExecuteNonQuery()
        End Using

        MarkNew()
    End Sub

    'Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
    '    Using ctx = ContextManager(Of <%= _schema._nameSpace %>DataContext).GetDataContext

    '            Dim qry = From _<%= _schema._className %> In ctx.DataContext.<%= _schema._DBTable %>s _
    '                      Where _<%= _schema._className %>.DTB = _DTB AndAlso _<%= _schema._className %>.SITE = _SITE <%= KeysFetchWithCriteria() %>

    '            ctx.DataContext.<%= _schema._DBTable %>s.DeleteAllOnSubmit(qry)

    '        ctx.DataContext.SubmitChanges()

    '    End Using
    '    MarkNew()
    'End Sub

#End Region ' Data Access - Delete

#End Region 'Data Access     
                       </txt>


        Return template.Value
    End Function

    Friend Function GetDataAccess_ChildEditableList() As String
        Dim temp = <template>
#Region " Data Access "

        Private Sub Fetch(ByVal dr As SafeDataReader)
            RaiseListChangedEvents = False

            Dim suppressChildValidation = true
            While dr.Read()
                Dim Line = <%= _schema._className %>.Get<%= _schema._className %>(dr, suppressChildValidation)
                Me.Add(Line)
            End While

            RaiseListChangedEvents = True
        End Sub

        Friend Sub Update(ByVal cn As SqlConnection, ByVal parent As <%= _schema._parentClassName %>)
            RaiseListChangedEvents = False

            ' loop through each deleted child object
            For Each deletedChild As <%= _schema._className %> In DeletedList
                deletedChild._DTB = parent._DTB
                deletedChild.DeleteSelf(cn)
            Next
            DeletedList.Clear()

            ' loop through each non-deleted child object
            For Each child As <%= _schema._className %> In Me
                child._DTB = parent._DTB
                'child.OrderNo = parent.OrderNo
                If child.IsNew Then
                    child.Insert(cn)
                Else
                    child.Update(cn)
                End If
            Next

            RaiseListChangedEvents = True
        End Sub

#End Region ' Data Access                   </template>

        Return temp.Value
    End Function

    Friend Function GetDataAccess_ReadOnlyList() As String
        'If _schema._DBMergeMode Then
        '    Return GetDataAccess_ReadOnlyList_Merged()
        'Else
        Return GetDataAccess_ReadOnlyList_StandAloneTable()
        'End If
    End Function

    '    Friend Function GetDataAccess_ReadOnlyList_Merged() As String

    '        Dim temp = <template>
    '#Region " Data Access "

    '#Region " Filter Criteria "

    '        &lt;Serializable()&gt; _
    '        Private Class FilterCriteria
    '            Public Sub New()
    '            End Sub
    '        End Class

    '#End Region
    '        Private Shared _lockObj As New Object
    '        Private Overloads Sub DataPortal_Fetch(ByVal criteria As FilterCriteria)
    '         SyncLock _lockObj
    '            RaiseListChangedEvents = False
    '            IsReadOnly = False
    '            Using ctx = ContextManager(Of <%= _schema._nameSpace %>DataContext).GetDataContext
    '                Dim qry = From _<%= _schema._className %> In ctx.DataContext.<%= _schema._DBTable %>s _
    '                          Where _<%= _schema._className %>.DTB = _DTB AndAlso _<%= _schema._className %>.SITE = _SITE <%= SubTableFetchWithCriteria() %>

    '                For Each row In qry
    '                    Dim info = <%= _schema._className %>Info.Get<%= _schema._className %>Info(row)
    '                    'If pbs.Security.DAG.CanAccess(info.DataAccessGroup) Then
    '                        Me.Add(info)
    '                    'End If
    '                Next

    '            End Using
    '            IsReadOnly = True
    '            RaiseListChangedEvents = True
    '         End SyncLock
    '        End Sub

    '#End Region ' Data Access                   </template>

    '        Return temp.Value
    '    End Function

    Friend Function GetDataAccess_ReadOnlyList_StandAloneTable() As String
        Dim SelectAllQuery As String = "<SqlText>SELECT * FROM _DBTable WHERE DTB='<%= _DTB %>'</SqlText>.Value.Trim".Replace("_DBTable", _schema._DBTable)

        Dim temp = <template>
#Region " Data Access "

#Region " Filter Criteria "

        &lt;Serializable()&gt; _
        Private Class FilterCriteria
            Friend _sqlText as String = String.Empty
            Public Sub New()
            End Sub
        End Class

#End Region
        Private Shared _lockObj As New Object
      
        Private Overloads Sub DataPortal_Fetch(ByVal criteria As FilterCriteria)
         SyncLock _lockObj
            RaiseListChangedEvents = False
            IsReadOnly = False

            Using cn = ConnectionFactory.GetDBConnection(true)

                Using cm = cn.CreateSQLCommand
                    cm.CommandType = CommandType.Text
                    
                    If Not String.IsNullOrEmpty(criteria._sqlText) Then
                        cm.CommandText = criteria._sqlText
                    Else
                        cm.CommandText = <%= SelectAllQuery %>
                    End If

                     Using dr As New SafeDataReader(cm.ExecuteReader)
                        While dr.Read 
                            dim info = <%= _schema._className %>Info.Get<%= _schema._className %>Info(dr)
                            Me.Add(Info)
                        End While
                     End Using
               
                 End Using     
                 
            End Using
            IsReadOnly = True
            RaiseListChangedEvents = True
         End SyncLock
        End Sub

#End Region ' Data Access                   </template>

        Return temp.Value
    End Function

#Region "Editable List"
    Friend Function GetDataAccess_EditableList() As String
        Dim pclassName As String = _schema._className
        Dim template = <template>
#Region "Data Access"

<%= EL_GetFilterCriteria() %>
                           <%= EL_GetFetch(pclassName) %>
                           <%= EL_GetDataPortalUpdate(pclassName) %>    
     
#End Region ' Data Access
                    </template>

        Return template.Value.Trim
    End Function

    Private Function EL_GetFilterCriteria() As String
        Dim template = <template>
#Region " Filter Criteria "

        &lt;Serializable()&gt; _
        Private Class FilterCriteria
            Public _sqlText As String = String.Empty

            Public Sub New(ByVal sqlText As String)
                _sqlText = sqlText
            End Sub
        End Class

#End Region                           
                       </template>.Value.Trim
        Return template
    End Function

    Private Function EL_GetFetch(pClassName As String) As String

        Dim template = <template>
       
      Private Overloads Sub DataPortal_Fetch(ByVal criteria As FilterCriteria)
            RaiseListChangedEvents = False
            Using cn = New SqlConnection(Database.PhoebusConnection)
                cn.Open()
                ExecuteFetch(cn, criteria)
            End Using
            RaiseListChangedEvents = True
        End Sub     

      Private Sub ExecuteFetch(ByVal cn As SqlConnection, ByVal criteria As FilterCriteria)
            Using cm As SqlCommand = cn.CreateCommand()
                cm.CommandType = CommandType.Text
                cm.CommandText = criteria._sqlText

                Using dr As SafeDataReader = New SafeDataReader(cm.ExecuteReader())
                    While dr.Read()
                        'Todo: implement DAG here
                        Dim _Info As <%= pClassName %> = <%= pClassName %>.Get<%= pClassName %>(dr)
                        'If pbs.UsrMan.DAG.CanAccess(_Info.DataAccessGroup) Then 
                        Me.Add(_Info)
                         'End If
                    End While
                End Using
            End Using
        End Sub              
                       </template>.Value.Trim
        Return template
    End Function

    Private Function EL_GetDataPortalUpdate(pClassName As String) As String

        Dim template = <template>

    Private _locObj As New Object
    Protected Overrides Sub DataPortal_Update()
            RaiseListChangedEvents = False
            Using cn = New SqlConnection(Database.PhoebusConnection)
                cn.Open()

                ' loop through each deleted child object
                For Each deletedChild As LQ In DeletedList
                    deletedChild.DeleteSelf(cn)
                Next
                DeletedList.Clear()

                Dim NewJETrans As New List(Of LQ)
                For Each child As LQ In Me

                    If child._isTotalLine Then Continue For

                    If child.IsNew Then
                        child.Insert(cn, Me)
                    Else
                        child.Update(cn, Me)
                    End If
                Next

        End Using

        RaiseListChangedEvents = True
    End Function
                       </template>.Value.Trim
        Return template
    End Function
#End Region

#End Region

End Class
