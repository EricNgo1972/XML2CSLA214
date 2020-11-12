Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Public Module StrUtils

    Public Function Nz(ByVal source As String, ByVal valueIfEmpty As String) As String
        If String.IsNullOrEmpty(source) Then
            Return valueIfEmpty
        Else
            Return source
        End If
    End Function

    Public Function DNz(ByVal source As Object, ByVal valueIfEmpty As String) As String
        If source Is Nothing OrElse IsDBNull(source) Then
            Return valueIfEmpty
        Else
            Return source.ToString
        End If
    End Function

    ''' <summary>
    ''' Y/True/T/1
    ''' </summary>
    ''' <param name="BooleanStr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToBoolean(ByVal BooleanStr As String) As Boolean

        If String.IsNullOrEmpty(BooleanStr) Then Return False

        Return BooleanStr.Trim.MatchesRegExp("^Y$|^True$|^1$|^T$")

    End Function

    <Extension()> _
   Public Function ToSunDate(ByVal _Date As Date) As String
        Return String.Format("{0:0000}{1:00}{2:00}", _Date.Year, _Date.Month, _Date.Day)
    End Function

#Region "Exception"
    <Extension()> _
    Public Sub Dump(ByVal ex As Exception, ByRef msg As String)

        If Not ex Is Nothing Then
            msg += ex.Message + System.Environment.NewLine
        Else
            Exit Sub
        End If

        Dump(ex.InnerException, msg)

    End Sub
#End Region

    <Extension()> _
    Public Function MatchesRegExp(ByVal the_string As String, ByVal regular_expression As String) As Boolean
        Dim reg_exp As New Regex(regular_expression, RegexOptions.IgnoreCase)

        Return reg_exp.IsMatch(the_string)
    End Function

    ''' <summary>
    ''' Fill DataStr to DB_Str.  From position StartFillingAt. With len = Filling Len
    ''' If DataStr is shorter then FillingLen. the Gap will be filled with the filler
    ''' Which default value is blank
    ''' </summary>
    ''' <param name="TargetString">Where you want to fill</param>
    ''' <param name="StartFillingAt">1 is the first character</param>
    ''' <param name="FillingLen"></param>
    ''' <param name="DataStr">What you want to fill</param>
    ''' <param name="filler"></param>
    ''' <remarks></remarks>
    Public Sub Fill(ByRef TargetString As String, ByVal StartFillingAt As Integer, ByVal FillingLen As Integer, ByVal DataStr As String, Optional ByVal filler As Char = " "c)

        If FillingLen = 0 Then Exit Sub

        Dim maxlen = Math.Max(StartFillingAt + FillingLen - 1, TargetString.Length)


        TargetString = TargetString.PadRight(maxlen)


        Dim _len As Integer = Math.Min(TargetString.Length - StartFillingAt + 1, FillingLen)

        TargetString = TargetString.Remove(StartFillingAt - 1, _len)
        TargetString = TargetString.Insert(StartFillingAt - 1, Fit(Nz(DataStr, ""), _len, filler))

    End Sub

    Private Function Fit(ByVal Str As String, ByVal len As Integer, Optional ByVal filler As Char = " "c) As String
        Return Str.PadRight(len, filler).Substring(0, len)
    End Function

End Module
