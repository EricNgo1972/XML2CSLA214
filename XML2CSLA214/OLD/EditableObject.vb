Imports System.Text
Public Class EditableObject
    Inherits CSLABuilder

    Dim template As XDocument = <?xml version="1.0"?>
                                <code>
Imports Csla
Imports Csla.Data
Imports Csla.Validation
Imports spc.Helper
Imports System.Text.RegularExpressions
Imports spc.BusinessContext
Imports spc.Authorizator

''' <summary>
''' Operator Definition
''' </summary>
''' <remarks></remarks>
                       &lt;Serializable()&gt; _
Public Class [objectClass]
    Inherits Csla.BusinessBase(Of [objectClass])
    Implements IGenPartObject

    Private Const STR_[objectClass] As String = "POD"
    Private _DTB As String

#Region "Business Properties and Methods"
    [PROPERTIES]
#End Region ' Business Properties and Methods

#Region "ID parsing"
    [IDPARSING]
#End Region ' ID parsing

#Region " Validation Rules "
    [VALIDATION_RULES]
#End Region ' Validation Rules

#Region " Authorization Rules "
    [AUTHORIZATION]
#End Region ' Authorization Rules

#Region " Factory Methods "
    [FACTORY]
#End Region ' Factory Methods

#Region " Data Access "
    [CRITERIA]
    [CREATE]
    [FETCH]
    [INSERT]
    [UPDATE]
    [DELETE]
#End Region 'Data Access

#Region " Exists "
  [EXISTS]
#End Region

#Region " IGenPart "
   [IGENPART]
#End Region

End Class
                   </code>

    Public Function GenCode() As String
        Dim _gencode As New StringBuilder
        _gencode.Append(template...<code>.Value)

        _gencode.Replace("[PROPERTIES]", MyBase.Build_Properties)
        _gencode.Replace("[IDPARSING]", MyBase.Build_IDParsing)
        _gencode.Replace("[VALIDATION_RULES]", MyBase.Build_ValidationRules)
        _gencode.Replace("[AUTHORIZATION]", MyBase.Build_AuthorizationRules)
        _gencode.Replace("[FACTORY]", MyBase.Build_Factory)
        If _keyCount > 1 Then
            _gencode.Replace("[CRITERIA]", MyBase.Build_ClassCriteria)
        Else
            _gencode.Replace("[CRITERIA]", String.Empty)
        End If
        _gencode.Replace("[CRITERIA]", MyBase.Build_ClassCriteria)
        _gencode.Replace("[CREATE]", MyBase.Build_DataPortal_Create)
        _gencode.Replace("[FETCH]", MyBase.Build_DataPortal_Fetch)
        _gencode.Replace("[INSERT]", MyBase.Build_DataPortal_Insert)
        _gencode.Replace("[UPDATE]", MyBase.Build_DataPortal_Update)
        _gencode.Replace("[DELETE]", MyBase.Build_DataPortal_Delete)
        _gencode.Replace("[EXISTS]", MyBase.Build_Exists)
        _gencode.Replace("[IGENPART]", MyBase.Build_IGenPart)

        _gencode.Replace("[objectClass]", MyBase._objectClass)

        Return _gencode.ToString
    End Function

End Class
