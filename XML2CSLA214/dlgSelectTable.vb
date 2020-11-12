Public Class dlgSelectTable

    Public _tableName As String


    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDoubleClick

        Dim pt = New Point(e.X, e.Y)
        Dim itm = ListBox1.Items(ListBox1.IndexFromPoint(pt))
        If itm IsNot Nothing Then

            _tableName = itm.ToString

            DialogResult = Windows.Forms.DialogResult.OK

            Me.Close()

        End If

    End Sub

    Private Sub dlgSelectTable_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim Db_Tables = DBOInfo.GetUsersTable
        Me.ListBox1.DataSource = Db_Tables
    End Sub
End Class