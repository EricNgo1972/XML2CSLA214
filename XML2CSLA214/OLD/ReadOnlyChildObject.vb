Imports System.Text
Public Class ReadonlyChildObject
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
''' Operator Definition Info
''' </summary>
''' <remarks></remarks>
                       &lt;Serializable()&gt; _
Public Class [objectClass]Info
    Inherits Csla.ReadOnlyBase(Of [objectClass]Info)

    Private Const STR_[objectClass] As String = "POD"

#Region "Business Properties and Methods"
    [INFO_PROPERTIES]
#End Region ' Business Properties and Methods

#Region "ID parsing"
    [IDPARSING]
   Public Overrides Function ToString() As String
    Return GetIdValue
  End Function
#End Region ' ID parsing

    [NEWCHILD]

End Class
                   </code>

    Public Function GenCode() As String
        Dim _gencode As New StringBuilder
        _gencode.Append(template...<code>.Value)

        _gencode.Replace("[INFO_PROPERTIES]", MyBase.Build_InfoProperties)
        _gencode.Replace("[IDPARSING]", MyBase.Build_IDParsing)
        _gencode.Replace("[NEWCHILD]", MyBase.Build_NEWCHILD)
        
        _gencode.Replace("[objectClass]", MyBase._objectClass)

        Return _gencode.ToString
    End Function

End Class
