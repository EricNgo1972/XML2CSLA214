Imports System.Text
Partial Class CSLAFactory

#Region " Business Properties and Methods "

    Friend Function GetProperties_Editable() As String
        Dim Template = <template>
                           
#Region "Property Changed"
        Protected Overrides Sub OnDeserialized(context As Runtime.Serialization.StreamingContext)
            MyBase.OnDeserialized(context)
            AddHandler Me.PropertyChanged, AddressOf BO_PropertyChanged
        End Sub

        Private Sub BO_PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
            'Select Case e.PropertyName

            '    Case "OrderType"
            '        If Not Me.GetOrderTypeInfo.ManualRef Then
            '            Me._orderNo = POH.AutoReference
            '        End If

            '    Case "OrderDate"
            '        If String.IsNullOrEmpty(Me.OrderPrd) Then Me._orderPrd.Text = Me._orderDate.Date.ToSunPeriod

            '    Case "SuppCode"
            '        For Each line In Lines
            '            line._suppCode = Me.SuppCode
            '        Next

            '    Case "ConvCode"
            '        If String.IsNullOrEmpty(Me.ConvCode) Then
            '            _convRate.Float = 0
            '        Else
            '            Dim conv = pbs.BO.LA.CVInfoList.GetConverter(Me.ConvCode, _orderPrd, String.Empty)
            '            If conv IsNot Nothing Then
            '                _convRate.Float = conv.DefaultRate
            '            End If
            '        End If

            '    Case Else

            'End Select

            pbs.BO.Rules.CalculationRules.Calculator(sender, e)
        End Sub
#End Region

#Region " Business Properties and Methods "

        Private _DTB As String = String.Empty
     
<%= If(String.IsNullOrEmpty(_schema._subTableCode), "", String.Format("       Public Const STR_{0} As String = ""{0}""{1}", _schema._subTableCode, Environment.NewLine)) %>

                           <%= From finfo In _schema.Members Where (finfo._id <> "DTB" AndAlso finfo._id <> "SITE") Select GetProperty_Editable(finfo) %>
                           
'Get ID
<%= GetIdValue() %>
'IComparable
<%= GetIComparable() %>                           
#End Region 'Business Properties and Methods
                       </template>
        Return Template.Value
    End Function

    Friend Function GetProperties_ReadOnly() As String
        Dim Template = <template>
#Region " Business Properties and Methods "

<%= From finfo In _schema.Members Select GetProperty_ReadOnly(finfo) %>
'Get ID
<%= GetIdValue() %>
'IComparable
<%= GetIComparable() %>

                           <%= Code_Property() %>
                           <%= LookUp_Property() %>
                           <%= Description_Property() %>
                           
    Public Sub InvalidateCache() Implements IInfo.InvalidateCache
            <%= _schema._className %>InfoList.InvalidateCache()
    End Sub
                          
                           
#End Region 'Business Properties and Methods
                       </template>
        Return Template.Value
    End Function

    Friend Function GetProperties_ReadOnlyList() As String
        Dim Template = <template>
#Region " Business Properties and Methods "
        Private Shared _DTB As String = String.Empty
        Const   _SUNTB As String = "<%= _schema._subTableCode %>"
        Private Shared _list As <%= _schema._className %>InfoList
#End Region 'Business Properties and Methods
                       </template>
        Return Template.Value
    End Function

    Private Function GetProperty_Editable(ByVal pField As FieldInfo) As StringBuilder
        Dim str = New StringBuilder

        Dim keyFieldTemplate = <template>
        Private [FieldName] As [FieldType] = [DefaultValue]
        &lt;System.ComponentModel.DataObjectField(True, <%= If(pField._isAutoNumber.ToBoolean, "True", "False") %>)&gt; _
        Public ReadOnly Property [PropertyName] As [PropertyType]
            Get
                Return [Property_Get]
            End Get
        End Property
                      </template>

        Dim notKeyFieldTemplate = <template>
        Private [FieldName] As [FieldType] = [DefaultValue] 
        &lt;CellInfo(GroupName:="",Hidden:=False)>
        Public Property [PropertyName] As [PropertyType]
            Get
                Return [Property_Get]
            End Get
            Set(ByVal value As [PropertyType])
                'CanWriteProperty(NameOf([PropertyName]), True)
                [Safe_Property_Set]
                If Not [FieldName].Equals(value) Then
                    [Property_Set]
                    PropertyHasChanged(NameOf([PropertyName]))
                End If
            End Set
        End Property
                                </template>

        If pField.IsPK Then
            str.Append(keyFieldTemplate.Value)
        Else
            str.Append(notKeyFieldTemplate.Value)
        End If

        'Search and Replace
        str.Replace("[FieldName]", pField._id.toMemberName)
        str.Replace("[PropertyName]", pField._id.toPropertyName)
        str.Replace("[FieldType]", pField.FieldType)
        If Not String.IsNullOrEmpty(pField.DefaultValue) Then
            str.Replace("= [DefaultValue]", String.Format(" = {0}", pField.DefaultValue))
        Else
            str.Replace("= [DefaultValue]", String.Empty)
        End If

        str.Replace("[PropertyType]", pField.PropertyType)
        str.Replace("[Property_Get]", pField.Property_Get)
        str.Replace("[Property_Set]", pField.Property_Set)
        str.Replace("[Safe_Property_Set]", pField.Safe_Property_Set)

        str.Replace(FieldInfo.STR_DELETED_LINE & System.Environment.NewLine, String.Empty)

        Return str

    End Function
    Private Function GetProperty_ReadOnly(ByVal pField As FieldInfo) As StringBuilder
        Dim str = New StringBuilder

        Dim FieldTemplate = <template>
        Private [FieldName] As [FieldType] = [DefaultValue] 
        Public ReadOnly Property [PropertyName] As [PropertyType]
            Get
                Return [Property_Get]
            End Get
        End Property
                                </template>

        str.Append(FieldTemplate.Value)

        'Search and Replace
        str.Replace("[FieldName]", pField._id.toMemberName)
        str.Replace("[PropertyName]", pField._id.toPropertyName)
        str.Replace("[FieldType]", pField.FieldType)
        If Not String.IsNullOrEmpty(pField.DefaultValue) Then
            str.Replace("= [DefaultValue]", String.Format(" = {0}", pField.DefaultValue))
        Else
            str.Replace("= [DefaultValue]", String.Empty)
        End If

        str.Replace("[PropertyType]", pField.PropertyTypeReadonly)
        str.Replace("[Property_Get]", pField.Property_GetReadonly)
        'str.Replace("[Property_Set]", pField.Property_Set(False))
        str.Replace("[Safe_Property_Set]", pField.Safe_Property_Set)

        str.Replace(FieldInfo.STR_DELETED_LINE & System.Environment.NewLine, String.Empty)

        Return str

    End Function
    Private Function Code_Property() As String

        Dim temp = <template>
    Public Readonly Property Code As String Implements IInfo.Code
        Get
            Return <%= _schema.MyCode %>
        End Get                       
    End Property
                   </template>

        Return temp.Value

    End Function
    Private Function LookUp_Property() As String

        Dim temp = <template>
    Public Readonly Property LookUp As String  Implements IInfo.LookUp
         Get
             Return <%= _schema.MyLookUp %>
         End Get
    End Property
                   </template>

        Return temp.Value

    End Function
    Private Function Description_Property() As String

        Dim temp = <template>
      Public Readonly Property Description As String  Implements IInfo.Description
         Get
             Return <%= _schema.MyDescription %>
         End Get
    End Property
                   </template>

        Return temp.Value

    End Function

    Private Function GetIdValue() As String

        Dim temp = <template>    Protected Overrides Function GetIdValue() As Object
        Return [IDArray]
    End Function
                   </template>

        Dim fortmatStr As String = String.Empty
        Dim varArr As String = String.Empty

        If _schema.Keys.Count = 1 Then
            Return temp.Value.Replace("[IDArray]", _schema.Keys(0)._id.toMemberName)
        Else
            For i = 0 To _schema.Keys.Count - 1
                fortmatStr += "{" & i & "}:"
                varArr += String.Format(", {0}{1}", _schema.Keys(i)._id.toMemberName, If(_schema.Keys(i)._fieldType.MatchesRegExp("String"), ".Trim", String.Empty))
            Next
            fortmatStr = System.Text.RegularExpressions.Regex.Replace(fortmatStr, ":$", String.Empty)

            Return temp.Value.Replace("[IDArray]", String.Format("String.Format(""{0}""{1})", fortmatStr, varArr))
        End If

    End Function

    Private Function Compare_Block() As String
        Dim _Compare_Block As New StringBuilder
        For Each f In _schema.Keys
            _Compare_Block.AppendFormat("If {0}{3} < p{1} Then Return -1 {2}", f._id.toMemberName, f._id.toPropertyName, System.Environment.NewLine, If(f._fieldType.MatchesRegExp("String"), ".Trim", ""))
            _Compare_Block.AppendFormat("If {0}{3} > p{1} Then Return 1 {2}", f._id.toMemberName, f._id.toPropertyName, System.Environment.NewLine, If(f._fieldType.MatchesRegExp("String"), ".Trim", ""))
        Next
        _Compare_Block.AppendFormat("Return 0")
        Return _Compare_Block.ToString
    End Function

    Private Function GetIComparable() As String
        Dim str = New StringBuilder
        Dim temp = <template>        Public Function CompareTo(ByVal IDObject) As Integer Implements System.IComparable.CompareTo
            Dim ID = IDObject.ToString
            <%= ParsingID_Block() %>
                       <%= Compare_Block() %>
        End Function    
                       </template>

        str.Append(temp.Value)

        Return str.ToString
    End Function
#End Region
End Class
