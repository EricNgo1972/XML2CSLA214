

Public Class CodeFactory

    Public Shared Function GenerateCode(pClassMeta As MetaData.EditableDefinition) As String

        Dim files = New List(Of String)

        Dim CLS = pClassMeta.ClassName

        Dim theCode = pClassMeta.GenerateEditableCode

        Dim FileName = System.IO.Path.Combine(pbs.Helper.FileRepository.GetSubFolder("CSharp_Code"), $"{CLS}.cs")

        My.Computer.FileSystem.WriteAllText(FileName, theCode, False)

        files.Add(FileName)

        theCode = pClassMeta.GenerateReadonlyCode
        If Not String.IsNullOrEmpty(theCode) Then
            FileName = System.IO.Path.Combine(pbs.Helper.FileRepository.GetSubFolder("CSharp_Code"), $"{CLS}Info.cs")

            My.Computer.FileSystem.WriteAllText(FileName, theCode, False)

            files.Add(FileName)

        End If

        theCode = pClassMeta.GenerateReadonlyListCode
        If Not String.IsNullOrEmpty(theCode) Then

            FileName = System.IO.Path.Combine(pbs.Helper.FileRepository.GetSubFolder("CSharp_Code"), $"{CLS}InfoList.cs")

            My.Computer.FileSystem.WriteAllText(FileName, theCode, False)

            files.Add(FileName)

        End If

        theCode = pClassMeta.GenerateQDADD
        If Not String.IsNullOrEmpty(theCode) Then
            FileName = System.IO.Path.Combine(pbs.Helper.FileRepository.GetSubFolder("CSharp_Code"), $"{pClassMeta.QDADD}.xml")

            My.Computer.FileSystem.WriteAllText(FileName, theCode, False)

            files.Add(FileName)

        End If

        Dim zipfileName = System.IO.Path.Combine(pbs.Helper.FileRepository.GetBackupFolder, $"{pClassMeta.ClassNameSpace}.{CLS}.zip")
        pbs.Helper.FileCompressor.ZipFiles(files.ToArray, zipfileName)

        Try
            System.IO.Directory.Delete(pbs.Helper.FileRepository.GetSubFolder("CSharp_Code"), True)
        Catch ex As Exception
        End Try


        Return zipfileName

    End Function

End Class
