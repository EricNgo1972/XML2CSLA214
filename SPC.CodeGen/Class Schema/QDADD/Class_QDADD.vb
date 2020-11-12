Imports pbs.Helper
Imports pbs.BO.DB

Namespace MetaData

    Partial Public Class EditableDefinition

        Public Function GenerateQDADD() As String

            Try
                If String.IsNullOrEmpty(DBTableName) Then Return String.Empty
                If String.IsNullOrEmpty(QDADD) Then Return String.Empty

                Dim scripts = New List(Of String)

                Dim currentTable = New SimpleInfo With {._Code = DBTableName}
                Dim tb = New DBTable(currentTable, "")

                scripts.Add(tb.MySchemaText(QDADD))
                scripts.Add(tb.MySourceText(QDADD))

                Dim body = <SCHEMA SCHEMA_ID=<%= QDADD %> DESCRIPTN=<%= DBTableName %>>
                            thebody
                       </SCHEMA>.ToString

                body = body.Replace("thebody", String.Join(Environment.NewLine, scripts.ToArray))

                Dim xele = XElement.Parse(body)

                For Each ele In xele...<row>
                    Dim Col = ele.GetStringAttribute("node")

                    If String.IsNullOrEmpty(Col) Then Continue For

                    For Each itm In Fields
                        If itm.DatabaseFieldName = Col Then
                            If itm.FieldType.MatchesRegExp("SmartDate") Then
                                Dim theType = ele.Attribute("type")
                                If theType IsNot Nothing Then

                                    If theType.Value.MatchesRegExp("N") Then ele.Attribute("type").SetValue("SDN") Else ele.Attribute("type").SetValue("SD")

                                End If
                                Continue For
                            ElseIf itm.FieldType.MatchesRegExp("SmartPeriod") Then
                                Dim theType = ele.Attribute("type")
                                If theType IsNot Nothing Then

                                    If theType.Value.MatchesRegExp("N") Then ele.Attribute("type").SetValue("SPN") Else ele.Attribute("type").SetValue("SP")

                                End If
                                Continue For
                            ElseIf itm.FieldType.MatchesRegExp("SmartTime") Then
                                Dim theType = ele.Attribute("type")
                                If theType IsNot Nothing Then

                                    If theType.Value.MatchesRegExp("N") Then ele.Attribute("type").SetValue("STN") Else ele.Attribute("type").SetValue("ST")

                                End If
                                Continue For
                            End If
                        End If
                    Next

                Next

                Return xele.ToString.Replace("""strin""", """""")

            Catch ex As Exception
                Return ex.Message
            End Try

        End Function


    End Class

End Namespace
