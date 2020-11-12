Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Private Function GenerateScriptsCode() As String


            If Scripts.Count > 0 Then

                Dim ret = New List(Of String)

                For Each scr In Scripts
                    ret.Add(scr.GenerateScriptsCode())
                Next

                Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

                retStr = WrappWithRegion(retStr, "ISupportScript")

                Return retStr

            End If

            Return String.Empty

        End Function


    End Class

End Namespace
