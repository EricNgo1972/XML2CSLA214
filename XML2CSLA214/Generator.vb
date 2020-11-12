Public Class Generator

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtSchema.Text = Schema._schema.ToString
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabCSLARfMsc.SelectedIndexChanged
        Try
            If Me.TabCSLARfMsc.SelectedTab IsNot Nothing Then
                Select Case Me.TabCSLARfMsc.SelectedTab.Text
                    Case "RFMSC411"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        rtbRFMSC.Text = factory.Get_RFMSC411

                    Case "RFMSCInfo"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        rtbRFMSCInfo.Text = factory.Get_RFMSCInfo

                    Case "RfMSCInfoList"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        rtbRFMSCInfoList.Text = factory.Get_RFMSCReadonlyList

                    Case "Editable411"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        rtbEditable411.Text = factory.Get_Editable



                    Case "Readonly411"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        rtbReadonly411.Text = factory.Get_Readonly

                    Case "ReadonlyList411"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        rtbReadonlyList411.Text = factory.Get_ReadonlyList

                    Case "ReadonlyList411Sync"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLA411Factory(_schema)

                        TextReadonlyList411Sync.Text = factory.Get_ReadonlyListNonAsync

                    Case "Editable"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLAFactory(_schema)

                        txtEditable.Text = factory.GetClassHeader_Editable
                        txtEditable.Text += factory.GetProperties_Editable
                        txtEditable.Text += factory.GetValidationRules
                        txtEditable.Text += factory.GetFactoryMethods_Editable
                        txtEditable.Text += factory.GetDataAccess_Editable
                        txtEditable.Text += factory.GetExists
                        txtEditable.Text += factory.GetIGenpart
                        txtEditable.Text += factory.GetIDoclink
                        txtEditable.Text += factory.GetClassFooter

                    Case "Child Editable"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLAFactory(_schema)

                        txtChildEditable.Text = factory.GetClassHeader_ChildEditable
                        txtChildEditable.Text += factory.GetProperties_Editable
                        txtChildEditable.Text += factory.GetValidationRules
                        txtChildEditable.Text += factory.GetFactoryMethods_ChildEditable
                        txtChildEditable.Text += factory.GetDataAccess_ChildEditable
                        txtChildEditable.Text += factory.GetClassFooter

                    Case "Readonly"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLAFactory(_schema)

                        txtReadonly.Text = factory.GetClassHeader_Readonly
                        txtReadonly.Text += factory.GetProperties_ReadOnly
                        txtReadonly.Text += factory.GetFactoryMethods_ReadOnly
                        txtReadonly.Text += factory.GetIDoclink
                        txtReadonly.Text += factory.GetClassFooter

                    Case "Readonly List"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLAFactory(_schema)

                        txtReadonlyList.Text = factory.GetClassHeader_ReadonlyList
                        txtReadonlyList.Text += factory.GetProperties_ReadOnlyList
                        txtReadonlyList.Text += factory.GetFactoryMethods_ReadonlyList
                        txtReadonlyList.Text += factory.GetDataAccess_ReadOnlyList
                        txtReadonlyList.Text += factory.GetDictionary
                        txtReadonlyList.Text += factory.GetClassFooter

                    Case "Child Editable List"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLAFactory(_schema)
                        txtChildEditableList.Text = factory.GetClassHeader_ChildEditableList
                        txtChildEditableList.Text += factory.GetBusinessMethod_ChildEditableList
                        txtChildEditableList.Text += factory.GetFactoryMethods_ChildEditableList
                        txtChildEditableList.Text += factory.GetDataAccess_ChildEditableList
                        txtChildEditableList.Text += factory.GetClassFooter

                    Case "Editable List"
                        Dim _schema = Schema.GetSchema(txtSchema.Text)
                        Dim factory = New CSLAFactory(_schema)
                        rtbEditableList.Text = factory.GetClassHeader_EditableList
                        rtbEditableList.Text += factory.GetBusinessMethod_EditableList
                        rtbEditableList.Text += factory.GetFactoryMethods_EditableList
                        rtbEditableList.Text += factory.GetDataAccess_EditableList
                        rtbEditableList.Text += factory.GetClassFooter

                        'Case "UIGenPart"
                        '    Dim _schema = Schema.GetSchema(txtSchema.Text)
                        '    Dim factory = New CSLAFactory(_schema)

                        '    txtUIGenPart.Text = factory.GetUIGenPart

                        'Case "BOSchema"

                        '    Dim _schema = Schema.GetSchema(txtSchema.Text)
                        '    Dim factory = New BOSchemaFactory(_schema)

                        '    txtBOSchema.Text = factory.GetBOSchema

                End Select

            End If
        Catch ex As Exception
            txtEditable.Text = ex.Message
        End Try

    End Sub

    Private Sub ReadTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReadTableToolStripMenuItem.Click
        Try
            Using dlg = New dlgSelectTable
                If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

                    txtSchema.Text = DBOInfo.GetTableXMLSchema(dlg._tableName)

                End If
            End Using

        Catch ex As Exception
            TextLogger.Log(ex)
            TextLogger.Flush()
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class
