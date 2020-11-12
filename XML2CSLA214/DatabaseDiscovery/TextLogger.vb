Imports System.Text

''' <summary>
''' usage : 
''' Log - to record message or exception to buffer.
''' Flush - to clear buffer and write buffer content to file PhoebusLog.xml
''' </summary>
''' <remarks></remarks>
Public Class TextLogger
    Private Shared _content As StringBuilder = Nothing
    Public Shared _fileName As String = "#DATE#_Log.txt"

    Private Shared Function Content() As StringBuilder
        If _content Is Nothing Then _content = New StringBuilder
        Return _content
    End Function

    Friend Shared Sub Flush()
        Dim thefileName = _fileName.Replace("#DATE#", Today.ToSunDate)
        thefileName = thefileName.Replace("#TIME#", String.Format("{0:00}{1:00}{2:00}", Now.Hour, Now.Minute, Now.Second))
        My.Computer.FileSystem.WriteAllText(thefileName, Content.ToString, True)
        _content = New StringBuilder
    End Sub

    Public Shared Function Text() As String
        Return Content.ToString
    End Function

    Public Shared Sub Log(ByVal msg As String)
        Content.AppendLine(Now.ToString & ": " & msg)
        Console.WriteLine(msg)
    End Sub

    Public Shared Sub Log(ByVal format As String, ByVal ParamArray params As Object())
        Content.AppendLine(Now.ToString & ": " & String.Format(format, params))
        Console.WriteLine(format, params)
    End Sub

    Public Shared Sub Write(ByVal format As String, ByVal ParamArray params As Object())
        Content.Append(Now.ToString & ": " & String.Format(format, params))
        Console.Write(format, params)
    End Sub

    Public Shared Sub Log(ByVal ex As Exception)
        Dim excMsg As String = String.Empty
        ex.Dump(excMsg)
        Content.AppendLine(Now.ToString & ": " & excMsg)
        Console.WriteLine(excMsg)
    End Sub

End Class
