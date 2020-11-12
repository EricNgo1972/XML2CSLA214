Public Class Context
    Public Shared Function ProductCode() As String
        Return "Phoebus"
    End Function

#Region " Database "

    Private Shared _connectionStr As String
    Friend Shared Function ConnectionStr() As String
        If String.IsNullOrEmpty(_connectionStr) Then
            _connectionStr = CONInfo.GetDefaultConnection
        End If
        Return _connectionStr
    End Function

    Friend Shared Function DefaultDB() As String
        Dim server As String = System.Text.RegularExpressions.Regex.Match(Context.ConnectionStr, "(?<=Data Source=|Server=)[^;,^\s]+").Value
        Dim db As String = System.Text.RegularExpressions.Regex.Match(Context.ConnectionStr, "(?<=Initial Catalog=|Database=)[^;,^\s]+").Value
        Return String.Format("{0} - {1}", server, db)
    End Function


#End Region

End Class
