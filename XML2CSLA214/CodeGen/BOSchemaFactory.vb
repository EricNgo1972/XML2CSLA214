Friend Class BOSchemaFactory

    Private _schema As Schema
    Public Sub New(ByVal pSchema As Schema)
        _schema = pSchema
    End Sub


    Public Function GetBOSchema() As String
        Dim xmlSchema =
<___BO___>
    <%= From f In _schema.Members
        Select <field id=<%= f._id.toMemberName %> type=<%= f._fieldType %> start="1" len=<%= f._len %> required=<%= If(f.IsPK, "Y", "") %>/> %>
</___BO___>

        Dim txt = xmlSchema.ToString

        Return txt.Replace("___BO___", _schema._className)
    End Function



End Class
