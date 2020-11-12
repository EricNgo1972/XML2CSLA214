Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module Pascal2DBName
    <Extension()> _
   Public Function DBColumnName(ByVal PascalString As String) As String

        Dim dest As String = PascalString

        Dim mCol As MatchCollection = Regex.Matches(PascalString, "[A-Z]{1}")

        For i As Integer = 1 To mCol.Count
            Dim m As Match = mCol(mCol.Count - i)
            If m.Index = 0 Then Continue For
            dest = dest.Remove(m.Index, 1)
            dest = dest.Insert(m.Index, "_" & m.Value)
        Next

        Return dest.ToUpper
    End Function

    ''' <summary>
    ''' Database Name to Property Name
    ''' </summary>
    ''' <param name="UPPER_COLUMN_NAME"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function toPropertyName(ByVal UPPER_COLUMN_NAME As String) As String

        Return PascalConvert(UPPER_COLUMN_NAME)

    End Function

    ''' <summary>
    ''' Database Name to member Name
    ''' </summary>
    ''' <param name="UPPER_COLUMN_NAME"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function toMemberName(ByVal UPPER_COLUMN_NAME As String) As String

        Return CamelConvert(UPPER_COLUMN_NAME)

    End Function

    Private Function PascalConvert(ByVal UPPER_COLUMN_NAME As String) As String

        If Schema._using_PHOEBUS_COLUMN_FORMAT Then
            Dim dest As String = UPPER_COLUMN_NAME.ToLower

            Dim mCol As MatchCollection = Regex.Matches(UPPER_COLUMN_NAME, "^.{1}|_.{1}")
            For Each m As Match In mCol
                dest = dest.Remove(m.Index, m.Length)
                dest = dest.Insert(m.Index, m.Value.ToUpper)
            Next

            Return dest.Replace("_", String.Empty)

        Else
            Return UPPER_COLUMN_NAME
        End If

    End Function

    Private Function CamelConvert(ByVal source As String) As String

        If Schema._using_PHOEBUS_COLUMN_FORMAT Then

            Dim dest As String = source.ToLower

            Dim mCol As MatchCollection = Regex.Matches(source, "_.{1}")
            For Each m As Match In mCol
                dest = dest.Remove(m.Index, m.Length)
                dest = dest.Insert(m.Index, m.Value.ToUpper)
            Next

            dest = dest.Replace("_", String.Empty)

            Return "_" + dest

        Else

            Return Regex.Replace(source, "^.{1}", "_" & source.Substring(0, 1).ToLower)

        End If

    End Function


End Module
