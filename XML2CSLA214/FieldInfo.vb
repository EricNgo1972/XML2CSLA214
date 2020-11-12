Imports System.Text.RegularExpressions
Public Class FieldInfo
    Public Const STR_DELETED_LINE As String = "'DELETED_ME"

    Friend _id As String = String.Empty
    Friend _secondID As String = String.Empty
    Friend _fieldType As String = String.Empty
    Friend _propertyType As String = String.Empty
    Friend _DbType As String = String.Empty 'user for detect the length if _len is empty

    Friend _len As Integer
    Friend _isPK As String = String.Empty
    Friend _isAutoNumber As String = String.Empty
    Friend _infoPosition As String = String.Empty

    Friend ReadOnly Property IsPK() As Boolean
        Get
            If String.IsNullOrEmpty(_isPK) Then Return False
            If _isPK.Equals("Y", StringComparison.OrdinalIgnoreCase) Then Return True
            If _isPK.Equals("True", StringComparison.OrdinalIgnoreCase) Then Return True
            Return False
        End Get
    End Property

#Region " Business Members Decorators "

    Function DefaultValue() As String
        Select Case _fieldType
            Case "SmartDate", "SD"
                Return "new pbs.Helper.SmartDate(true)"
            Case "SmartTime", "ST"
                Return "new pbs.Helper.SmartTime()"
            Case "SmartPeriod", "SP"
                Return "new pbs.Helper.SmartPeriod()"
            Case "SmartFloat", "SF"
                Return "new pbs.Helper.SmartFloat(0)"
            Case "SmartInt32", "SI"
                Return "new pbs.Helper.SmartInt32(0)"
            Case "SmartInt16", "SI16"
                Return "new pbs.Helper.SmartInt16(0)"
            Case "String", "Char"
                Return "String.Empty"
            Case Else
                Return String.Empty
        End Select
    End Function

    Function GetDefaultParameterValue() As String
        Select Case Me.PropertyType
            Case "SmartDate", "SD"
                Return "new SmartDate(true)"
            Case "SmartTime", "ST"
                Return "new SmartTime()"
            Case "SmartPeriod", "SP"
                Return "new SmartPeriod()"
            Case "SmartFloat", "SF"
                Return "new SmartFloat(0)"
            Case "SmartInt32", "SI"
                Return "new SmartInt32(0)"
            Case "SmartInt16", "SI16"
                Return "new SmartInt16(0)"
            Case "String", "Char"
                Return <t>""</t>.Value
            Case "int", "int32", "integer", "decimal"
                Return "0"
            Case Else
                Return <t>""</t>.Value
        End Select
    End Function

    Function FieldType() As String
        Select Case _fieldType
            Case "SmartDate", "SD"
                Return "pbs.Helper.SmartDate"
            Case "SmartTime", "ST"
                Return "pbs.Helper.SmartTime"
            Case "SmartPeriod", "SP"
                Return "SmartPeriod"
            Case "SmartFloat", "SF", "Numeric"
                Return "pbs.Helper.SmartFloat"
            Case "SmartInt32", "SI"
                Return "pbs.Helper.SmartInt32"
            Case "SmartInt16", "SI16"
                Return "pbs.Helper.SmartInt16"
            Case "Char", "String"
                Return "string"
            Case "Int32", "Int16"
                Return "int"
            Case Else
                Return _fieldType
        End Select
    End Function

    Friend Function IsReadonly() As Boolean
        If _id.toPropertyName.MatchesRegExp("Updated|UpdatedBy") Then
            Return True
        End If
        Return False
    End Function

    Function PropertyType() As String
        If Not String.IsNullOrEmpty(_propertyType) Then
            Return _propertyType
        Else
            Select Case _fieldType
                Case "SmartDate", "SmartPeriod", "SmartTime", "ST", "SD", "SP"
                    Return "string"
                Case "SmartFloat", "SmartInt32", "SmartInt16", "SF", "SI"
                    Return "string"
                Case "Char", "String", "varchar"
                    Return "string"
                Case Else
                    Return _fieldType
            End Select
        End If
    End Function

    Function PropertyTypeReadonly() As String
        If Not String.IsNullOrEmpty(_propertyType) Then
            Return _propertyType
        Else
            Select Case _fieldType

                Case "SmartDate", "SmartPeriod", "SmartTime", "ST", "SD", "SP"
                    Return "string"

                Case "SmartFloat"
                    Return "decimal"
                Case "SmartInt32", "SmartInt16", "SF", "SI"
                    Return "string"
                Case "Char", "varchar"
                    Return "string"
                Case Else
                    Return _fieldType
            End Select
        End If
    End Function

    Function Property_DB() As String
        Dim tails As String = String.Empty
        If _fieldType.MatchesRegExp("Smart") Then
            tails = ".DBValue()"
        ElseIf _fieldType.MatchesRegExp("String") Then
            tails = ".Trim()"
        End If
        Return String.Format("{0}{1}", _id.toMemberName, tails)
    End Function

    Function Property_Get() As String
        Select Case _fieldType

            Case "SmartDate", "SmartPeriod", "SmartTime", "ST", "SD", "SP"

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.Text", _id.toMemberName)
                Else
                    Return _id.toMemberName
                End If

            Case "SmartFloat", "SmartInt32", "SmartInt16", "SF", "SI" 'member smart

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.Text", _id.toMemberName)
                ElseIf _propertyType = "Decimal" Then
                    Return String.Format("{0}.Float", _id.toMemberName)
                ElseIf _propertyType = "Integer" Then
                    Return String.Format("{0}.Int", _id.toMemberName)
                ElseIf _propertyType = "SmallInt" Then
                    Return String.Format("{0}.Int", _id.toMemberName)
                End If

            Case "String" 'member string - property Boolean
                If Not String.IsNullOrEmpty(_propertyType) Then
                    If _propertyType.Equals("Boolean", StringComparison.OrdinalIgnoreCase) Then

                        Return String.Format("{0}.Equals(""Y"", StringComparison.OrdinalIgnoreCase)", _id.toMemberName)

                    ElseIf _propertyType.Equals("Boolean?", StringComparison.OrdinalIgnoreCase) Then

                        Return String.Format("If(String.IsNullOrEmpty({0}) = True, Nothing, {0}.Equals(""Y"", StringComparison.OrdinalIgnoreCase))", _id.toMemberName)

                    End If
                End If
        End Select

        Return _id.toMemberName

    End Function

    Function Property_GetReadonly() As String
        Select Case _fieldType

            Case "SmartDate", "SD"

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.DateViewFormat", _id.toMemberName)
                Else
                    Return _id.toMemberName
                End If

            Case "SmartPeriod", "SP"

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.PeriodViewFormat", _id.toMemberName)
                Else
                    Return _id.toMemberName
                End If

            Case "SmartTime", "ST"

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.Text", _id.toMemberName)
                Else
                    Return _id.toMemberName
                End If

            Case "SmartFloat"

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.Float", _id.toMemberName)
                ElseIf _propertyType = "Decimal" Then
                    Return String.Format("{0}.Float", _id.toMemberName)
                End If

            Case "SmartInt32", "SmartInt16", "SF", "SI" 'member smart

                If String.IsNullOrEmpty(_propertyType) Then
                    Return String.Format("{0}.DocNumber", _id.toMemberName)
                ElseIf _propertyType = "Integer" Then
                    Return String.Format("{0}.Int", _id.toMemberName)
                ElseIf _propertyType = "SmallInt" Then
                    Return String.Format("{0}.Int", _id.toMemberName)
                End If

            Case "String" 'member string - property Boolean
                If Not String.IsNullOrEmpty(_propertyType) Then
                    If _propertyType.Equals("Boolean", StringComparison.OrdinalIgnoreCase) Then

                        Return String.Format("{0}.ToBoolean", _id.toMemberName)

                    ElseIf _propertyType.Equals("Boolean?", StringComparison.OrdinalIgnoreCase) Then

                        Return String.Format("If(String.IsNullOrEmpty({0}), Nothing, {0}.ToBoolean)", _id.toMemberName)

                    End If
                End If
        End Select

        Return _id.toMemberName

    End Function

    Function Property_Load411(Optional noConvert As Boolean = False) As String
        If FieldType411() <> PropertyType() AndAlso Not noConvert Then
            Return $"LoadPropertyConvert<{FieldType411()}, {PropertyType()}>({_id.toPropertyName}Property, p{_id.toPropertyName})"
        Else
            Return $"LoadProperty({_id.toPropertyName}Property, p{_id.toPropertyName})"
        End If

    End Function
    Function Property_Set411() As String
        If FieldType411() <> PropertyType() Then
            Return $"SetPropertyConvert<{FieldType411()}, {PropertyType()}>({_id.toPropertyName}Property, p{_id.toPropertyName})"
        Else
            Return $"SetProperty({_id.toPropertyName}Property, p{_id.toPropertyName})"
        End If

    End Function

    Function Property_Set(Optional ByVal RaiseChangeEvent As Boolean = True) As String

        If _fieldType = PropertyType() Then Return String.Format("{0} = value", _id.toMemberName)

        Select Case _fieldType

            Case "Boolean"

                If String.IsNullOrEmpty(_propertyType) Then 'empty means string
                    Return String.Format("{0} = value", _id.toMemberName)
                End If

            Case "SmartDate", "SmartPeriod", "SmartTime", "ST", "SD", "SP"

                If String.IsNullOrEmpty(_propertyType) Then 'empty means string
                    Return String.Format("{0}.Text = value", _id.toMemberName)
                End If

            Case "SmartFloat", "SF"  'member smart

                If String.IsNullOrEmpty(_propertyType) Then 'default is string
                    Return String.Format("{0}.Text = value", _id.toMemberName)
                ElseIf _propertyType = "Decimal" Then
                    Return String.Format("{0}.Float = value ", _id.toMemberName)
                ElseIf _propertyType = "Integer" Then
                    Return String.Format("{0}.Float = value", _id.toMemberName)
                End If

            Case "SmartInt32", "SmartInt16", "SI"  'member smart

                If String.IsNullOrEmpty(_propertyType) Then 'default is string
                    Return String.Format("{0}.Text = value", _id.toMemberName)
                ElseIf _propertyType = "Integer" Then
                    Return String.Format("{0}.Int = value", _id.toMemberName)
                End If

            Case "String" 'member string - property smart
                If Not String.IsNullOrEmpty(_propertyType) Then

                    Select Case _propertyType
                        Case "SmartDate", "SmartPeriod", "SD", "SP", "SmartInt32", "SI"
                            Return String.Format("{0} = value.DBValue.ToString", _id.toMemberName)

                        Case "Boolean"
                            'Return String.Format("{0} = value.DBValue.ToString", _id)
                            Dim t = <template>
                            If Not _convCntrl.ToBoolean = value Then
                                If value Then _convCntrl = if(value,"Y","N")
                                <%= If(RaiseChangeEvent, "PropertyHasChanged(""ConvCntrl"")", String.Empty) %> 
                            End If
                                    </template>
                            Return t.Value.Replace("_convCntrl", _id.toMemberName).Replace("ConvCntrl", _id.toPropertyName)

                        Case "Boolean?"
                            Dim t = <template>
                            'Todo : Delete outside if clause
                             If value Is Nothing Then
                                If Not String.IsNullOrEmpty(_convCntrl) Then
                                    _convCntrl = String.Empty
                                   <%= If(RaiseChangeEvent, "PropertyHasChanged(""ConvCntrl"")", String.Empty) %> 
                                End If
                             Else
                                If Not _convCntrl.ToBoolean = value Then
                                    If value Then _convCntrl = if(value,"Y","N")
                                    <%= If(RaiseChangeEvent, "PropertyHasChanged(""ConvCntrl"")", String.Empty) %> 
                                End If
                            End If
                                    </template>
                            Return t.Value.Replace("_convCntrl", _id.toMemberName).Replace("ConvCntrl", _id.toPropertyName)
                    End Select

                End If
        End Select

        Return String.Format("{0} = value", _id.toMemberName)
    End Function

    Function Safe_Property_Set() As String
        Select Case PropertyType()
            Case "String"
                Return "If value Is Nothing Then value = String.Empty"

            Case "SmartDate", "SD"
                Return "If value Is Nothing Then value = new pbs.Helper.SmartDate()"

            Case "SmartTime", "ST"
                Return "If value Is Nothing Then value = new pbs.Helper.SmartTime()"

            Case "SmartPeriod", "SP"
                Return "If value Is Nothing Then value = new pbs.Helper.SmartPeriod()"

            Case "SmartInt32", "SI"
                Return "If value Is Nothing Then value = new pbs.Helper.SmartInt32(0)"

            Case Else
                Return String.Empty
                ' Return STR_DELETED_LINE

        End Select
    End Function

    Function Safe_Read_Field() As String
        If _DbType Is Nothing Then
            Return <txt>dr.GetString("<%= _id %>").TrimEnd</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "CHAR", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetString("<%= _id %>").TrimEnd</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "uniqueidentifier", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetGuid("<%= _id %>").ToString</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "SmallInt", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetInt16("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "tinyint", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetByte("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "INT", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetInt32("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "Decimal", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetDecimal("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "Numeric", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetDecimal("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "DateTime", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetDateTime("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "bit", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetBoolean("<%= _id %>")</txt>.Value

        Else
            Return <txt>dr.GetString("<%= _id %>").TrimEnd</txt>.Value
        End If
    End Function

    Function SQL_Read_Field() As String


        If _DbType Is Nothing Then
            Return <txt>dr.GetString("<%= _id %>").TrimEnd()</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "CHAR", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetString("<%= _id %>").TrimEnd()</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "uniqueidentifier", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetGuid("<%= _id %>").ToString()</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "SmallInt", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetInt16("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "tinyint", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetByte("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "INT", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetInt32("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "Decimal", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetDecimal("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "Numeric", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetDecimal("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "DateTime", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetDateTime("<%= _id %>")</txt>.Value

        ElseIf Regex.IsMatch(_DbType, "bit", RegexOptions.IgnoreCase) Then
            Return <txt>dr.GetBoolean("<%= _id %>")</txt>.Value

        Else
            Return <txt>dr.GetString("<%= _id %>").TrimEnd()</txt>.Value
        End If
    End Function

    Function Safe_Read_FieldType() As String
        If _DbType Is Nothing Then
            Return "string"

        ElseIf Regex.IsMatch(_DbType, "CHAR", RegexOptions.IgnoreCase) Then
            Return "string"

        ElseIf Regex.IsMatch(_DbType, "uniqueidentifier", RegexOptions.IgnoreCase) Then
            Return "string"

        ElseIf Regex.IsMatch(_DbType, "SmallInt", RegexOptions.IgnoreCase) Then
            Return "int"

        ElseIf Regex.IsMatch(_DbType, "tinyint", RegexOptions.IgnoreCase) Then
            Return "int"

        ElseIf Regex.IsMatch(_DbType, "INT", RegexOptions.IgnoreCase) Then
            Return "int"

        ElseIf Regex.IsMatch(_DbType, "Decimal", RegexOptions.IgnoreCase) Then
            Return "decimal"

        ElseIf Regex.IsMatch(_DbType, "Numeric", RegexOptions.IgnoreCase) Then
            Return "decimal"

        ElseIf Regex.IsMatch(_DbType, "DateTime", RegexOptions.IgnoreCase) Then
            Return "DateTime"

        ElseIf Regex.IsMatch(_DbType, "bit", RegexOptions.IgnoreCase) Then
            Return "bool"

        Else
            Return "string"
        End If
    End Function



#End Region

#Region "411"

    Function FieldType411() As String
        Select Case _fieldType
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
            Case "Int32", "Int16"
                Return "int"
            Case Else
                Return _fieldType
        End Select
    End Function



    ''' <summary>
    ''' private string EmplCode = ""
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetPropertyDeclaringCriteria411(Optional isPrivate As Boolean = True) As String
        Return $"{If(isPrivate, "private", "internal")} {Me.FieldType} {_id.toPropertyName} = {GetDefaultParameterValue()};"
    End Function

    Friend Function GetFetchPropertyRFMSC411() As String
        'Dim ret = New List(Of String)

        ' Name = xele.GetString("NAME").TrimEnd();


        Dim theFieldType = FieldType411()
        Dim theFieldName = _id.toPropertyName

        Dim thePropType = PropertyType()
        Dim thePropName = _id.toPropertyName

        Dim theDBColumnName = Property_DB()
        Dim theDBType = Safe_Read_FieldType()

        If IsPK Then Return String.Empty

        If thePropName.Equals("Dtb") Then Return String.Empty
        If thePropName.Equals("Updated") Then Return String.Empty

        Return <d><%= thePropName %> = xele.GetString(nameof(<%= thePropName %>)).TrimEnd();</d>.Value

        'Return ret
    End Function

    Friend Function GetFetchProperty411() As String
        'Dim ret = New List(Of String)

        Dim theFieldType = FieldType411()
        Dim theFieldName = _id.toPropertyName

        Dim thePropType = PropertyType()
        Dim thePropName = _id.toPropertyName

        Dim theDBColumnName = Property_DB()
        Dim theDBType = Safe_Read_FieldType()

        'Return $"  {thePropName} = {pField.SQL_Read_Field};"
        If theFieldType <> theDBType Then
            Return $"  LoadPropertyConvert<{theFieldType}, {theDBType}>({thePropName}Property, {SQL_Read_Field()});"
        Else
            Return $"  LoadProperty({thePropName}Property, {SQL_Read_Field()});"
        End If


        'Return ret
    End Function

    Friend Function GetOneProperty_Readonly() As List(Of String)
        Dim ret = New List(Of String)

        Dim theFieldType = FieldType411()
        Dim theFieldName = _id.toPropertyName

        Dim thePropType = PropertyType()
        Dim thePropName = _id.toPropertyName

        Dim theDefault = DefaultValue.Replace("pbs.Helper.Smart", "Smart")


        ret.Add($"public static readonly PropertyInfo<{theFieldType}> {thePropName}Property = RegisterProperty<{theFieldType}>(c => c.{thePropName},{theDefault});")

        If IsPK Then ret.Add($"[System.ComponentModel.DataObjectField(true,{_isAutoNumber.ToBoolean.ToString.ToLower})]")

        ret.Add($"public {thePropType} {thePropName}")
        ret.Add("{")

        If thePropType = theFieldType Then
            ret.Add($" get {{ return GetProperty({thePropName}Property); }}")

        ElseIf thePropType.MatchesRegExp("bool") AndAlso theFieldType.MatchesRegExp("string|char") Then
            ret.Add($" get {{ return GetProperty({thePropName}Property).ToBoolean(); }}")

        Else
            ret.Add($" get {{ return GetPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property); }}")
        End If

        If thePropType <> theFieldType AndAlso thePropType.MatchesRegExp("bool") AndAlso theFieldType.MatchesRegExp("string|char") Then
            ret.Add($"private set {{ LoadProperty({thePropName}Property, value?""Y"":""N""); }}")

        ElseIf thePropType <> theFieldType Then
            ret.Add($"private set {{ LoadPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property, value); }}")
        Else
            ret.Add($"private set {{ LoadProperty({thePropName}Property, value); }}")
        End If


        ret.Add("}")

        Return ret

    End Function

    Friend Function GetOneProperty_Criteria() As List(Of String)
        Dim ret = New List(Of String)

        Dim theFieldType = FieldType411()
        Dim theFieldName = _id.toPropertyName

        Dim thePropType = PropertyType()
        Dim thePropName = _id.toPropertyName

        Dim theDefault = DefaultValue.Replace("pbs.Helper.Smart", "Smart")


        ret.Add($"public static readonly PropertyInfo<{theFieldType}> {thePropName}Property = RegisterProperty<{theFieldType}>(c => c.{thePropName},{theDefault});")

        ret.Add($"public {thePropType} {thePropName}")
        ret.Add("{")

        If thePropType = theFieldType Then
            ret.Add($" get {{ return ReadProperty({thePropName}Property); }}")
        Else
            ret.Add($" get {{ return ReadPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property); }}")
        End If

        If thePropType <> theFieldType Then
            ret.Add($"private set {{ LoadPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property, value); }}")
        Else
            ret.Add($"private set {{ LoadProperty({thePropName}Property, value); }}")
        End If


        ret.Add("}")

        Return ret

    End Function

    Friend Function GetOneProperty_Editable() As List(Of String)
        Dim ret = New List(Of String)

        Dim theFieldType = FieldType411()
        Dim theFieldName = _id.toPropertyName

        Dim thePropType = PropertyType()
        Dim thePropName = _id.toPropertyName

        Dim theDefault = DefaultValue.Replace("pbs.Helper.Smart", "Smart")


        ret.Add($"public static readonly PropertyInfo<{theFieldType}> {thePropName}Property = RegisterProperty<{theFieldType}>(c => c.{thePropName},{theDefault});")

        If IsPK Then ret.Add($"[System.ComponentModel.DataObjectField(true,{_isAutoNumber.ToBoolean.ToString.ToLower})]")

        ret.Add($"public {thePropType} {thePropName}")
        ret.Add("{")

        If thePropType = theFieldType Then
            ret.Add($" get {{ return GetProperty({thePropName}Property); }}")

        ElseIf thePropType.MatchesRegExp("bool") AndAlso theFieldType.MatchesRegExp("string|char") Then
            ret.Add($" get {{ return GetProperty({thePropName}Property).ToBoolean(); }}")

        Else
            ret.Add($" get {{ return GetPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property); }}")
        End If

        If IsPK Then
            If thePropType <> theFieldType Then
                ret.Add($"private set {{ LoadPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property, value); }}")
            Else
                ret.Add($"private set {{ LoadProperty({thePropName}Property, value); }}")
            End If

        Else
            If IsReadonly() Then
                If thePropType <> theFieldType AndAlso thePropType.MatchesRegExp("bool") AndAlso theFieldType.MatchesRegExp("string|char") Then
                    ret.Add($"private set {{ LoadProperty({thePropName}Property, value?""Y"":""N""); }}")

                ElseIf thePropType <> theFieldType Then
                    ret.Add($"private set {{ LoadPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property, value); }}")
                Else
                    ret.Add($"private set {{ LoadProperty({thePropName}Property, value); }}")
                End If
            Else
                If thePropType <> theFieldType AndAlso thePropType.MatchesRegExp("bool") AndAlso theFieldType.MatchesRegExp("string|char") Then
                    ret.Add($"   set {{ SetProperty({thePropName}Property, value?""Y"":""N""); }}")

                ElseIf thePropType <> theFieldType Then
                    ret.Add($"  set {{ SetPropertyConvert<{theFieldType},{thePropType}>({thePropName}Property, value); }}")
                Else
                    ret.Add($"  set {{ SetProperty({thePropName}Property, value); }}")
                End If
            End If


        End If

        ret.Add("}")

        Return ret

    End Function

    Friend Function InsertUpdateToDBRFMSC() As String

        Dim thePropName = _id.toPropertyName

        If thePropName.Equals("Dtb") Then Return String.Empty

        Return <t>  _data.AddWithValue("@<%= _id %>", <%= thePropName %>.TrimEnd()); </t>.Value

    End Function

    Friend Function InsertUpdateToDB() As String
        Dim theFieldType = FieldType411()

        Dim thePropName = _id.toPropertyName

        Dim theDBType = Safe_Read_FieldType()

        'Return $"  {thePropName} = {pField.SQL_Read_Field};"
        If theFieldType <> theDBType Then
            Return <t> cm.Parameters.AddWithValue("@<%= _id %>", ReadPropertyConvert&lt;<%= theFieldType %>, <%= theDBType %>>(<%= thePropName %>Property)); </t>.Value
        Else
            ' cm.Parameters.AddWithValue("@FIRST_NAME", FirstName.Trim());
            Return <t>  cm.Parameters.AddWithValue("@<%= _id %>", ReadProperty(<%= thePropName %>Property)); </t>.Value
        End If
    End Function

    Friend Function AddingKeytoDictionary() As String
        If _DbType = "int" Then
            Return <t>pFilters.Add(nameof(<%= _id.toPropertyName %>), <%= _id.toPropertyName %>.ToString()); </t>.Value
        Else
            Return <t>pFilters.Add(nameof(<%= _id.toPropertyName %>), <%= _id.toPropertyName %>); </t>.Value
        End If

    End Function

    Friend Function AssigningKeysValueFromCriteria() As String

        If _DbType = "int" Then
            Return <t><%= _id.toPropertyName %> = criteria.<%= _id.toPropertyName %>.ToString(); </t>.Value
        Else
            Return <t><%= _id.toPropertyName %> = criteria.<%= _id.toPropertyName %>; </t>.Value
        End If


    End Function

    Friend Function AssigningKeysValueToClone() As String

        Return <t>cloning.<%= _id.toPropertyName %> = p<%= _id.toPropertyName %>; </t>.Value

    End Function

#End Region

End Class
