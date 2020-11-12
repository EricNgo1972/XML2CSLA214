Imports System.Data.SqlClient
Imports System.Data
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' Detect DB informations related to an object
''' </summary>
''' <remarks></remarks>
Public Class DBOInfo
    Public Shared Function GetPrimaryKeys(ByVal TableName As String) As List(Of String)

        Dim theList As New List(Of String)
        If String.IsNullOrEmpty(TableName) Then Return theList

        Dim SQLtext As String = _
" select kcu.TABLE_SCHEMA, kcu.TABLE_NAME, kcu.CONSTRAINT_NAME, tc.CONSTRAINT_TYPE, kcu.COLUMN_NAME, kcu.ORDINAL_POSITION " & _
"  from INFORMATION_SCHEMA.TABLE_CONSTRAINTS as tc " & _
"  join INFORMATION_SCHEMA.KEY_COLUMN_USAGE as kcu " & _
"    on kcu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA " & _
"   and kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME " & _
"   and kcu.TABLE_SCHEMA = tc.TABLE_SCHEMA " & _
"   and kcu.TABLE_NAME = tc.TABLE_NAME " & _
"   AND kcu.TABLE_NAME = '{@TableName}'	" & _
" where tc.CONSTRAINT_TYPE in ( 'PRIMARY KEY', 'UNIQUE' ) " & _
" order by kcu.TABLE_SCHEMA, kcu.TABLE_NAME, tc.CONSTRAINT_TYPE, kcu.CONSTRAINT_NAME, kcu.ORDINAL_POSITION" & _
""
        SQLtext = SQLtext.Replace("{@TableName}", TableName)

        Dim _dt As DataTable = PHOEBUS_SQLCommander.Extract(SQLtext)

        If _dt.Rows IsNot Nothing Then
            For Each row As DataRow In _dt.Rows
                theList.Add(DNz(row("COLUMN_NAME"), String.Empty))
            Next
        End If

        Return theList
    End Function

    Public Shared Function GetTableColumns(ByVal TableName As String) As List(Of String)
        Dim theList As New List(Of String)
        If String.IsNullOrEmpty(TableName) Then Return theList

        Dim SQLtext As String = _
"select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH " & _
"        from INFORMATION_SCHEMA.COLUMNS " & _
"where [TABLE_NAME] = '{@TableName}' " & _
"order by ORDINAL_POSITION "

        SQLtext = SQLtext.Replace("{@TableName}", TableName)

        Dim _dt As DataTable = PHOEBUS_SQLCommander.Extract(SQLtext)

        If _dt.Rows IsNot Nothing Then
            For Each row As DataRow In _dt.Rows
                theList.Add(DNz(row("COLUMN_NAME"), String.Empty))
            Next
        End If

        Return theList

    End Function

    Public Shared Function GetTableSchema(ByVal TableName As String) As DataTable

        Dim SQLtext As String = <script>
select * from INFORMATION_SCHEMA.COLUMNS where [TABLE_NAME] = '<%= TableName %>' order by ORDINAL_POSITION
                                </script>.Value

        Return PHOEBUS_SQLCommander.Extract(SQLtext)

    End Function

#Region "analyse table to xml"
    Public Shared Function GetTableXMLSchema(TableName As String) As String
        Dim dt = GetTableSchema(TableName)

        Dim pkeys = GetPrimaryKeys(TableName)

        Return <schema NameSpace="" ClassName=<%= Regex.Match(TableName, "[^_]+$") %> DBTable=<%= TableName %> ParentClassName="" Merge2Table="N" SubTableCode="" PHOEBUS_COLUMN_FORMAT="Y">

                   <%= From r As DataRow In dt.Rows Where Include(r) Select datarow2xml(r, pkeys) %>

               </schema>.ToString
    End Function

    Private Shared Function Include(row As DataRow) As Boolean
        'If row("COLUMN_NAME") = "DTB" Then Return False
        If row("DATA_TYPE") = "timestamp" Then Return False
        Return True
    End Function

    Private Shared Function datarow2xml(row As DataRow, pks As List(Of String)) As XElement
        Return <Column Name=<%= row("COLUMN_NAME") %> Type=<%= DetectBusinessType(row) %> DbType=<%= row("DATA_TYPE") %> IsPrimaryKey=<%= pks.Contains(row("COLUMN_NAME")) %> CanBeNull=<%= row("IS_NULLABLE") = "YES" %> LEN=<%= DNz(row("CHARACTER_MAXIMUM_LENGTH"), "") %> unicode=<%= DNz(row("CHARACTER_MAXIMUM_LENGTH"), "") = "UNICODE" %>/>
    End Function

    Private Shared Function DetectBusinessType(row As DataRow) As String
        If row("DATA_TYPE").ToString.MatchesRegExp("char|varchar|text") Then
            Return "String"

        ElseIf row("DATA_TYPE").ToString.MatchesRegExp("uniqueidentifier") Then
            Return "String"

        ElseIf row("DATA_TYPE").ToString.MatchesRegExp("smallint") Then
            Return "SmartInt16"

        ElseIf row("DATA_TYPE").ToString.MatchesRegExp("bit") Then
            Return "Boolean"

        ElseIf row("DATA_TYPE").ToString.MatchesRegExp("^date") Then
            Return "SmartDate"

        ElseIf row("DATA_TYPE").ToString.MatchesRegExp("^int") Then

            If row("COLUMN_NAME").ToString.MatchesRegExp("date") Then
                Return "SmartDate"
            ElseIf row("COLUMN_NAME").ToString.MatchesRegExp("period|prd") Then
                Return "SmartPeriod"
            ElseIf row("COLUMN_NAME").ToString.MatchesRegExp("time") Then
                Return "SmartTime"
            Else
                Return "SmartInt32"
                'Return "Integer"
            End If
        ElseIf row("DATA_TYPE").ToString.MatchesRegExp("numeric|decimal") Then
            Return "SmartFloat"

        Else
            Return row("DATA_TYPE").ToString
        End If
    End Function

#End Region

    Public Shared Function GetUsersTable() As List(Of String)
        Dim thelist As New List(Of String)

        Dim SQLtext As String = "SELECT [name] FROM dbo.sysobjects WHERE type = 'U' ORDER BY [name]"

        Dim dt = PHOEBUS_SQLCommander.Extract(SQLtext)
        For Each dr As DataRow In dt.Rows
            thelist.Add(dr("name").ToString.Trim)
        Next

        Return thelist
    End Function

    Public Shared Function GetParameters(ByVal StoredProcedureName As String) As List(Of String)

        Dim theList As New List(Of String)
        If String.IsNullOrEmpty(StoredProcedureName) Then Return theList

        Dim SQLtext As String = _
"select ORDINAL_POSITION,PARAMETER_NAME , PARAMETER_MODE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH " & _
"        from(INFORMATION_SCHEMA.PARAMETERS) " & _
"WHERE [SPECIFIC_NAME] = '{@StoredProcedureName}' " & _
"ORDER BY ORDINAL_POSITION " & _
""
        SQLtext = SQLtext.Replace("{@StoredProcedureName}", StoredProcedureName)

        Dim _dt As DataTable = PHOEBUS_SQLCommander.Extract(SQLtext)

        If _dt.Rows IsNot Nothing Then
            For Each row As DataRow In _dt.Rows
                theList.Add(DNz(row("PARAMETER_NAME"), String.Empty))
            Next
        End If

        Return theList
    End Function

    ''' <summary>
    ''' Detect an object exist in database 
    ''' </summary>
    ''' <param name="DBObjectName">Object Name</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ObjectExists(ByVal DBObjectName As String) As Boolean

        Dim SQLtext As String = "SELECT count(*) FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{DBObjectName}]')"
        SQLtext = SQLtext.Replace("{DBObjectName}", DBObjectName)

        Return PHOEBUS_SQLCommander.GetScalarInteger(SQLtext) > 0

    End Function
    Public Shared Function SPExists(ByVal SPName As String) As Boolean

        Dim SQLtext As String = "SELECT count(*) FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{DBObjectName}]') AND type = 'P'"
        SQLtext = SQLtext.Replace("{DBObjectName}", SPName)

        Return PHOEBUS_SQLCommander.GetScalarInteger(SQLtext) > 0

    End Function

    Public Shared Function ObjectExistsScript(ByVal DBObjectName As String) As String
        Return "SELECT count(*) FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{DBObjectName}]')".Replace("{DBObjectName}", DBObjectName)
    End Function

    Public Shared Function GetDelTableScript(ByVal table As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("Select * , getdate() as UpdatedTime into @table_DEL from @table Where 0=1")
        sb.Replace("@table", table.Trim)
        Return sb.ToString
    End Function
    Public Shared Function GetDelTriggerScript(ByVal table As String) As String
        Dim keys = DBOInfo.GetPrimaryKeys(table.Trim)

        Dim delSql = String.Empty

        If keys.Count > 0 Then
            Dim criteria = (From key In keys Select String.Format(" {0} IN (SELECT {0} FROM DELETED) ", key)).ToArray
            delSql = <Script>
                        DELETE FROM @table_DEL WHERE
                        <%= String.Join(" AND ", criteria) %>
                     </Script>.Value.Trim
        End If

        Dim trigger = <Script>
CREATE  trigger @table_DEL_TRIGGER on [dbo].[@table] FOR DELETE AS
<%= delSql %>
insert into [dbo].[@table_DEL] select * ,getdate() as UpdatedTime  from deleted                        
                      </Script>.Value.Trim
        Return trigger.Replace("@table", table.Trim)

    End Function
    Public Shared Function GetDelTableRemovingScript(ByVal table As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("Drop TABLE @table_DEL")
        sb.Replace("@table", table.Trim)
        Return sb.ToString
    End Function
    Public Shared Function GetDelTriggerRemovingScript(ByVal table As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("DROP trigger @table_DEL_TRIGGER")
        sb.Replace("@table", table.Trim)
        Return sb.ToString
    End Function

    Public Shared Function GetInsertUpdateTableScript(ByVal table As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("Select * , getdate() as UpdatedTime into @table_IU from [@table] Where 0=1")
        sb.Replace("@table", table.Trim)
        Return sb.ToString
    End Function

    Public Shared Function GetInsertUpdateTriggerScript(ByVal table As String) As String
        Dim keys = DBOInfo.GetPrimaryKeys(table.Trim)

        Dim delSql = String.Empty

        If keys.Count > 0 Then
            Dim criteria = (From key In keys Select String.Format(" {0} in (SELECT {0} FROM INSERTED) ", key)).ToArray
            delSql = <Script>
                        DELETE FROM @table_IU WHERE
                        <%= String.Join(" AND ", criteria) %>
                     </Script>.Value.Trim
        End If

        Dim trigger = <Script>
CREATE trigger @table_IU_TRIGGER on [dbo].[@table] FOR INSERT, UPDATE AS  
<%= delSql %>
insert into [dbo].[@table_IU] select * , getdate() as UpdatedTime from inserted                          
                      </Script>.Value.Trim
        Return trigger.Replace("@table", table.Trim)
    End Function

    Public Shared Function GetInsertUpdateTableRemovingScript(ByVal table As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("Drop TABLE @table_IU")
        sb.Replace("@table", table.Trim)
        Return sb.ToString

    End Function
    Public Shared Function GetInsertUpdateTriggerRemovingScript(ByVal table As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("DROP trigger @table_IU_TRIGGER")
        sb.Replace("@table", table.Trim)
        Return sb.ToString

    End Function
End Class

