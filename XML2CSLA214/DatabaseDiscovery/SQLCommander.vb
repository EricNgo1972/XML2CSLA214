Imports System.Data.SqlClient
Imports System.Data

Enum SQLMode
    Int = 0
    Dec = 1
    Str = 2
    DataTable = 3
    Update = 4
End Enum
''' <summary>
''' This Commander extend Phoebus Command object. Capable run SQL scripts to return a DataTable
''' or Integer/Decimal/String. Or even insert/update sql.
''' </summary>
''' <remarks></remarks>
<Serializable()> _
    Public Class PHOEBUS_SQLCommander

    Private _Table As DataTable
    Private _intReturnValue As Integer
    Private _decReturnValue As Decimal
    Private _strReturnValue As String = String.Empty
    Private _affected As Integer = 0

    Private _SQLString As String = String.Empty
    Private _SQLMode As SQLMode = SQLMode.DataTable

#Region " Factory Methods "
    Private Sub New(ByVal SQLString As String)
        _SQLString = SQLString
        _Table = New DataTable("DATA_TABLE")
    End Sub

    ''' <summary>
    ''' Run a query and return DataTable
    ''' </summary>
    ''' <param name="SQLString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Extract(ByVal SQLString As String) As DataTable
        Dim cmd As New PHOEBUS_SQLCommander(SQLString)
        cmd.DataPortal_Execute()
        Return cmd._Table
    End Function

    ''' <summary>
    ''' Run a scalar query and return Integer Value
    ''' </summary>
    ''' <param name="SQLString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetScalarInteger(ByVal SQLString As String) As Integer
        Dim cmd As New PHOEBUS_SQLCommander(SQLString)
        cmd._SQLMode = SQLMode.Int

        cmd.DataPortal_Execute()
        Return cmd._intReturnValue
    End Function

    ''' <summary>
    '''  Run a scalar query and return decimal Value
    ''' </summary>
    ''' <param name="SQLString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetScalarDecimal(ByVal SQLString As String) As Decimal
        Dim cmd As New PHOEBUS_SQLCommander(SQLString)
        cmd._SQLMode = SQLMode.Dec

        cmd.DataPortal_Execute()
        Return cmd._decReturnValue
    End Function

    ''' <summary>
    ''' Run a scalar query and return String Value
    ''' </summary>
    ''' <param name="SQLString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetScalarString(ByVal SQLString As String) As String
        Dim cmd As New PHOEBUS_SQLCommander(SQLString)
        cmd._SQLMode = SQLMode.Str

        cmd.DataPortal_Execute()
        Return cmd._strReturnValue
    End Function

    ''' <summary>
    ''' Run any SQL update/insert SQL script. Return number of affected Row.
    ''' </summary>
    ''' <param name="SQLString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RunInsertUpdate(ByVal SQLString As String) As Integer
        Dim cmd As New PHOEBUS_SQLCommander(SQLString)
        cmd._SQLMode = SQLMode.Update

        cmd.DataPortal_Execute()
        Return cmd._affected
    End Function

#End Region

#Region " Data Access  "
    Protected Sub DataPortal_Execute()
        Using cn As SqlConnection = New SqlConnection(Context.ConnectionStr)
            cn.Open()
            ExecuteFetch(cn)
        End Using
    End Sub

    Private Sub ExecuteFetch(ByVal cn As SqlConnection)
        Using cm As SqlCommand = cn.CreateCommand()
            cm.CommandType = CommandType.Text
            cm.CommandText = _SQLString
            cm.CommandTimeout = 1800

            Select Case _SQLMode
                Case SQLMode.DataTable
                    Using dr As SqlDataReader = cm.ExecuteReader()
                        FetchObject(dr)
                    End Using

                Case SQLMode.Dec
                    Dim ret As Object = cm.ExecuteScalar
                    _decReturnValue = CDec(DNz(ret, 0))

                Case SQLMode.Int
                    Dim ret As Object = cm.ExecuteScalar
                    _intReturnValue = CInt(DNz(ret, 0))

                Case SQLMode.Str
                    Dim ret As Object = cm.ExecuteScalar
                    _strReturnValue = DNz(ret, String.Empty)

                Case SQLMode.Update
                    Dim delimiter As String() = New String() {"GO_Splitter"} ', "GO ", "GO\t"}
                    Dim cmdTextArray As String() = cm.CommandText.Split(delimiter, StringSplitOptions.None)

                    For Each cmd In cmdTextArray
                        cm.CommandText = cmd
                        _affected += cm.ExecuteNonQuery()
                    Next

            End Select

        End Using
    End Sub

    Private Sub FetchObject(ByVal dr As SqlDataReader)
        _Table.Clear()
        _Table.Load(dr, LoadOption.OverwriteChanges)
    End Sub

#End Region

End Class

