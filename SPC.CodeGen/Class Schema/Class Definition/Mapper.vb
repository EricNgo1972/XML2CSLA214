Namespace MetaData

    Public Class Mapper

        Shared Function FieldType411(pFieldType) As String

            Select Case pFieldType
                Case "SmartDate", "SD"
                    Return "SmartDate"

                Case "SmartTime", "ST"
                    Return "SmartTime"

                Case "SmartPeriod", "SP"
                    Return "SmartPeriod"

                Case "SmartFloat", "SF", "Numeric"
                    Return "SmartFloat"

                Case "SmartInt32", "SI"
                    Return "SmartInt32"

                Case "SmartInt16", "SI16"
                    Return "SmartInt"

                Case "Char", "String"
                    Return "string"

                Case "Boolean"
                    Return "bool"

                Case "Integer"
                    Return "int"

                Case "Decimal"
                    Return "decimal"

                Case "Int32", "Int16"
                    Return "int"

                Case Else
                    Return pFieldType

            End Select

        End Function

    End Class

End Namespace
