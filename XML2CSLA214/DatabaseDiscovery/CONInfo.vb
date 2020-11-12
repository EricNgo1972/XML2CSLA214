Imports System.Text.RegularExpressions
Imports System.Web

Public Class CONInfo
  
    Friend Shared Function GetDefaultConnection() As String

        Try
            Dim constr As String = String.Empty

            Dim reg As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software\SPC-Technology\Phoebus", False)
            constr = Crypto.DecryptBytes(reg.GetValue("C1"))

            Return constr
        Catch ex As Exception
            Throw New Exception("can not get Connection string. Please run setup", ex)
        End Try


    End Function

End Class
