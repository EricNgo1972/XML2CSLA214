Imports System.Text.RegularExpressions

Public Class Schema

    Friend Shared _using_PHOEBUS_COLUMN_FORMAT As Boolean = True

#Region " Construction "

    Friend Shared _schema As XDocument = <?xml version="1.0"?>
                                         <schema NameSpace="HR" ClassName="EMP" DBTable="pbs_EMPLOYEE" ParentClassName="" Merge2Table="N" SubTableCode="" PHOEBUS_COLUMN_FORMAT="Y" Singleton="N">
                                             <Column Name="DTB" Type="System.String" DbType="CHAR(3)" IsPrimaryKey="true" CanBeNull="false"/>

                                             <Column Name="EMPL_CODE" Type="System.String" DbType="CHAR(15)" IsReadOnly="true" IsPrimaryKey="true" CanBeNull="false" Identity="N"/>
                                             <Column Name="NAME" Type="System.String" DbType="CHAR(50)" CanBeNull="false"/>
                                             <Column Name="OTHER_NAME" Type="System.String" DbType="CHAR(50)" IsReadOnly="true" CanBeNull="false"/>
                                             <Column Name="FIRST_NAME" Type="System.String" DbType="CHAR(20)" CanBeNull="false"/>
                                             <Column Name="MID_NAME" Type="System.String" DbType="CHAR(20)" CanBeNull="false"/>
                                             <Column Name="LAST_NAME" Type="System.String" DbType="CHAR(20)" CanBeNull="false"/>
                                             <Column Name="GENDER" Type="System.String" DbType="CHAR(1)" CanBeNull="false"/>
                                             <Column Name="DOB" Type="System.DateTime" DbType="INT" CanBeNull="false"/>
                                             <Column Name="PLACE_OF_BIRTH" Type="System.String" DbType="CHAR(20)" CanBeNull="false"/>
                                             <Column Name="MARITAL_STATUS" Type="System.String" DbType="CHAR(1)" CanBeNull="false"/>
                                             <Column Name="ETHNIC_GROUP" Type="System.String" DbType="CHAR(15)" CanBeNull="false"/>
                                             <Column Name="NATIONALITY" Type="System.String" DbType="CHAR(3)" CanBeNull="false"/>
                                             <Column Member="UPDATED" Type="System.DateTime" CanBeNull="false"/>
                                             <Column Member="CLOSE" Type="String" propertyType="Boolean" CanBeNull="false"/>
                                         </schema>
    Friend _DBTable As String = String.Empty
    Friend _DBMergeMode As Boolean = False
    Friend _nameSpace As String = String.Empty
    Friend _className As String = String.Empty
    Friend _parentClassName As String = String.Empty
    Friend _subTableCode As String = String.Empty

    Friend _isSingleton As Boolean = False

    Private _keys As List(Of FieldInfo)
    Friend ReadOnly Property Keys() As List(Of FieldInfo)
        Get
            If _keys Is Nothing Then
                _keys = (From f In _schema...<Column> Where f.@IsPrimaryKey = "true" AndAlso f.@Name <> "DTB" AndAlso f.@Name <> "SITE"
                         Select New FieldInfo With {._id = f.@Name, ._secondID = f.@Member, ._fieldType = f.@Type, ._len = f.@len, ._propertyType = f.@propertyType, ._DbType = f.@DbType,
                                                    ._isPK = f.@IsPrimaryKey, ._infoPosition = f.@infoPosition, ._isAutoNumber = f.@Identity}).ToList
            End If

            For Each k In _keys
                If String.IsNullOrEmpty(k._id) Then k._id = k._secondID

                k._fieldType = k._fieldType.Replace("System.", String.Empty)
                If k._fieldType = "DateTime" Then k._fieldType = "SmartDate"
            Next

            Return _keys
        End Get
    End Property

    Private _members As List(Of FieldInfo)
    Friend ReadOnly Property Members() As List(Of FieldInfo)
        Get
            If _members Is Nothing Then
                _members = (From f In _schema...<Column>
                            Select New FieldInfo With {._id = f.@Name, ._secondID = f.@Member, ._fieldType = f.@Type, ._len = f.@len, ._propertyType = f.@propertyType, ._DbType = f.@DbType,
                                                    ._isPK = f.@IsPrimaryKey, ._infoPosition = f.@infoPosition, ._isAutoNumber = f.@Identity}).ToList
            End If

            For Each m In _members
                'set Id if Member used instead of Name
                If String.IsNullOrEmpty(m._id) Then m._id = m._secondID

                'Set the short format of field type
                m._fieldType = m._fieldType.Replace("System.", String.Empty)
                If m._fieldType = "DateTime" Then m._fieldType = "SmartDate"


                'set length from field type
                If m._len = 0 Then
                    Select Case m._fieldType
                        Case "String", "Char"
                            If m._DbType IsNot Nothing Then
                                m._len = nz(Regex.Match(m._DbType, "[0-9]+").Value, 0)
                            End If
                        Case "SmartDate"
                            m._len = 10
                        Case "SmartPeriod"
                            m._len = 8
                        Case "SmartTime"
                            m._len = 6
                        Case "SmartFloat"
                            m._len = 25
                    End Select
                End If

            Next

            Return _members
        End Get
    End Property

    Private Function nz(ByVal original As String, ByVal value As String) As String
        If String.IsNullOrEmpty(original) Then Return value Else Return original
    End Function

    Friend Sub New(ByVal pSchema As XDocument)
        _schema = pSchema
        _DBMergeMode = (_schema.Root.@Merge2Table.Equals("Y", StringComparison.OrdinalIgnoreCase))
        _DBTable = _schema.Root.@DBTable
        _nameSpace = _schema.Root.@NameSpace
        _className = _schema.Root.@ClassName
        _subTableCode = _schema.Root.@SubTableCode
        _parentClassName = _schema.Root.@ParentClassName

        _isSingleton = DNz(_schema.Root.@Singleton, "N").ToBoolean

        _using_PHOEBUS_COLUMN_FORMAT = DNz(_schema.Root.@PHOEBUS_COLUMN_FORMAT, "Y").toBoolean
    End Sub

    Friend Shared Function GetSchema(ByVal pSchema As String) As Schema
        Dim xdoc = XDocument.Parse(pSchema)
        Return New Schema(xdoc)
    End Function

    ReadOnly Property MyCode() As String
        Get
            Return (From f In Members Where f._infoPosition = "Code" Select f._id Take 1).SingleOrDefault
        End Get
    End Property
    ReadOnly Property MyLookUp() As String
        Get
            Return (From f In Members Where f._infoPosition = "LookUp" Select f._id Take 1).SingleOrDefault
        End Get
    End Property
    ReadOnly Property MyDescription() As String
        Get
            Return (From f In Members Where f._infoPosition = "Description" Select f._id Take 1).SingleOrDefault
        End Get
    End Property

#End Region


#Region "CSLA411"

    ''' <summary>
    ''' string pKey1,int pKey2
    ''' </summary>
    ''' <param name="OptionalValue"></param>
    ''' <returns></returns>
    Friend Function GetParameterKeys(OptionalValue As Boolean) As String

        If Me._isSingleton Then Return String.Empty

        Dim retStrings As New List(Of String)

        If Keys.Count = 0 Then Return String.Empty

        For Each f In Keys
            If OptionalValue Then
                retStrings.Add($"{f.PropertyType} p{f._id.toPropertyName} = {f.GetDefaultParameterValue}")
            Else
                retStrings.Add($"{f.PropertyType} p{f._id.toPropertyName}")
            End If
        Next

        Return String.Join(",", retStrings.ToArray)
    End Function

    ''' <summary>
    ''' pKEy1,pKey2
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetCombinedKeys(Optional withP As Boolean = True) As String
        Dim retStrings As New List(Of String)

        If Keys.Count = 0 Then Return String.Empty

        For Each f In Keys
            If withP Then
                retStrings.Add($"p{f._id.toPropertyName}")
            Else
                retStrings.Add($"{f._id.toPropertyName}")
            End If

        Next

        If retStrings.Count = 1 Then
            Return retStrings(0)
        Else
            Return <text>String.Format("{0}:{1}", <%= String.Join(",", retStrings.ToArray) %>)</text>.Value
        End If

    End Function

    ''' <summary>
    ''' pKEy1,pKey2
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetUsingParameterKeys(Optional withP As Boolean = True) As String
        Dim retStrings As New List(Of String)

        If Keys.Count = 0 Then Return String.Empty

        For Each f In Keys
            If withP Then
                retStrings.Add($"p{f._id.toPropertyName}")
            Else
                retStrings.Add($"{f._id.toPropertyName}")
            End If

        Next

        If retStrings.Count = 1 Then
            Return retStrings(0)
        Else
            Return String.Join(",", retStrings.ToArray)
        End If

    End Function

    ''' <summary>
    ''' KEY1='{pKey1}' AND KEY2='{pKey2}'
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetSQLParameterKeys(Optional withCriteria As Boolean = False) As String
        Dim retStrings As New List(Of String)

        If Keys.Count = 0 Then Return String.Empty

        For Each f In Keys
            If withCriteria Then
                retStrings.Add(<t><%= f._id %> = '{criteria.<%= f._id.toPropertyName %>}'</t>.Value)
            Else
                retStrings.Add(<t><%= f._id %> = '{p<%= f._id.toPropertyName %>}'</t>.Value)
            End If


        Next

        Return String.Join(" AND ", retStrings.ToArray)
    End Function

    ''' <summary>
    ''' KEy1=pKey1,pKey2=pKey2
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetInitializeParameterKeys() As String
        Dim retStrings As New List(Of String)

        If Keys.Count = 0 Then Return String.Empty

        For Each f In Keys

            retStrings.Add($"{f._id.toPropertyName}=p{f._id.toPropertyName}")

        Next

        Return String.Join(",", retStrings.ToArray)
    End Function

    ''' <summary>
    ''' pKey1:pKey2
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetCombinedKeys() As String
        Dim retStrings As New List(Of String)

        If Keys.Count = 0 Then Return String.Empty

        For Each f In Keys

            retStrings.Add($"p{f._id.toPropertyName}")

        Next
        If retStrings.Count = 1 Then
            Return retStrings(0)
        Else
            Return <tx>String.Format("{0}:{1}", <%= String.Join(",", retStrings.ToArray) %>).TrimEnd()</tx>.Value
        End If
        '      Return String.Join(":", retStrings.ToArray)
    End Function

    ''' <summary>
    ''' value of  pKey1:pKey2 , used as ToString
    ''' or pKey1 if there is only one parameter
    ''' </summary>
    ''' <returns></returns>
    Friend Function GetCombinedKeyValues() As String


        If Keys.Count = 0 Then Return String.Empty

        If Keys.Count = 1 Then

            Dim ret = Keys(0)
            If ret.PropertyType.MatchesRegExp("string") Then
                Return ret._id.toPropertyName + ".TrimEnd()"
            Else
                Return ret._id.toPropertyName
            End If
        Else
            Dim retStrings As New List(Of String)
            For Each f In Keys
                retStrings.Add(<k>{<%= f._id.toPropertyName %>}</k>.Value)
            Next
            Return <tx> $"<%= String.Join(":", retStrings.ToArray) %>".TrimEnd(); </tx>.Value
        End If


    End Function

    Friend Function SetOuputSQLParameter(Optional withDicrection As Boolean = True) As String

        If Keys(0)._isAutoNumber.ToBoolean Then
            If withDicrection Then
                Return <t>cm.Parameters.AddWithValue("@<%= Keys(0)._id %>", ReadPropertyConvert&lt;<%= Keys(0).FieldType411 %>, <%= Keys(0)._DbType %>>(<%= Keys(0)._id.toPropertyName %>Property)).Direction= ParameterDirection.Output;</t>.Value
            Else
                Return <t>cm.Parameters.AddWithValue("@<%= Keys(0)._id %>", ReadPropertyConvert&lt;<%= Keys(0).FieldType411 %>, <%= Keys(0)._DbType %>>(<%= Keys(0)._id.toPropertyName %>Property));</t>.Value
            End If

        End If
        Return String.Empty
    End Function

    Friend Function GetOuputSQLParameter() As String
        If Keys(0)._isAutoNumber.ToBoolean Then
            Return <t><%= Keys(0)._id.toPropertyName %> = cm.GetReturnIdAsInteger("@<%= Keys(0)._id %>").ToString();</t>.Value
        End If
        Return String.Empty

    End Function

    Friend Function DataportalUpdate() As String
        If Not Keys(0)._isAutoNumber.ToBoolean Then
            Return "DataPortal_Insert();"
        Else
            Return <t>
            using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
            {
                using (var cm = cn.CreateSQLCommand())
                {
                    cm.SetDBCommand(CommandType.StoredProcedure, $"<%= Me._DBTable %>{DTB}_Update");

                     <%= SetOuputSQLParameter(False) %>

                    AddInsertParameters(cm);
                    cm.ExecuteNonQuery();

                }

                // update child object(s)

            }</t>.Value
        End If
    End Function


#End Region

End Class
