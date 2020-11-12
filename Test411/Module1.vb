Imports System.IO
Imports pbs.Helper

Module Module1

    Sub Main()

        Console.WriteLine("Phoebus CodeGen")

        RegisterAddins()

        Dim sample = spc.CodeGen.MetaData.ClassDefinitionFactory.GetClassDefinition("pbs.BO.HR.EMP", "VSA")
        sample.InsertSP = "pbs_EMP_InserUpdate"
        sample.UpdateSP = "pbs_EMP_InserUpdate"

        sample.UseSyncInfoList = True

        Dim zipFileName = spc.CodeGen.CodeFactory.GenerateCode(sample)

        Process.Start(zipfileName)

        Console.ReadKey()

    End Sub

    Private Sub RegisterAddins()



        pbs.UsrMan.ModuleSettings._register = True
        pbs.BO.Azure.Settings.RegisterModule()

        pbs.BO.LA.NAInfoList.InvalidateCache()

        pbs.BO.SQLBuilder.ModuleSettings._register = True
        pbs.BO.Authentication.CloudLogin._azureToken = Nothing ' add assembly to the list
        pbs.BO.TBOSettings._register = True ' add assembly to the list
        pbs.BO.PO.Settings.RegisterModule()
        pbs.BO.SO.Settings.RegisterModule()
        pbs.BO.ClientDocuments.Settings.RegisterModule()
        pbs.BO.eInvoice.Settings.RegisterModule()
        pbs.BO.CRM.Settings.RegisterModule()
        'pbs.BO.EXT.VUS.Settings.RegisterModule()
        'pbs.BO.EXT.VUS2018.Settings.RegisterModule()
        pbs.BO.SM.Settings.RegisterModule()
        pbs.BO.FI.Settings.RegisterModule()
        pbs.BO.PM.Settings.RegisterModule()
        pbs.BO.LA.Settings.RegisterModule()
        pbs.BO.PI.Settings.RegisterModule()
        pbs.BO.RE.Settings.RegisterModule()
        pbs.BO.MC.Settings.RegisterModule()
        pbs.BO.HR.Settings.RegisterModule()
        pbs.BO.LM.Settings.RegisterModule()
        'pbs.BO.MXM.Settings.RegisterModule()
        'pbs.BO.Sun5.Settings.RegisterModule()
        pbs.BO.EAM.Settings.RegisterModule()



        pbs.BO.HSP.Settings.RegisterModule()
        'pbs.BO.FAST.Settings.RegisterModule()
        'pbs.BO.MISA.Settings.RegisterModule()
        'pbs.BO.MIMOSA.Settings.RegisterModule()
        pbs.BO.OG.Settings.RegisterModule()
        pbs.BO.DM.Settings.RegisterModule()

        'pbs.BO.EXT.VUS2019.Settings.RegisterModule()

        'pbs.BO.Xero.Settings.RegisterModule()

        'pbs.BO.TWINCRM.Settings.RegisterModule()

        Dim loadedAssy = pbs.Helper.PhoebusAssemblies.GetBOAssemblyNames

        Dim addinFiles = My.Computer.FileSystem.GetFiles(pbs.Helper.FileRepository.GetAddInsFolder, FileIO.SearchOption.SearchTopLevelOnly)

        Dim _notLoadeds As New List(Of String)
        Dim loaded As Integer = 0

        For Each addinfile In addinFiles
            Try
                If loadedAssy.Contains(addinfile.FileName, StringComparer.OrdinalIgnoreCase) Then
                    Dim msg = String.Format("Assembly '{0}' is a built-in. Add-ins can not have this name", addinfile.FileName)
                    _notLoadeds.Add(msg)
                    Continue For
                End If

                Dim info = My.Computer.FileSystem.GetFileInfo(addinfile)
                If Path.GetExtension(addinfile).Equals(".dll", StringComparison.OrdinalIgnoreCase) Then

                    Dim ai_assy = System.Reflection.Assembly.LoadFile(addinfile)
                    For Each cl In ai_assy.DefinedTypes
                        If cl.Name = "Settings" Then
                            pbs.Helper.CallSharedMethodIfImplemented(cl, "RegisterModule")
                            Exit For
                        End If
                    Next

                    loaded += 1

                End If

            Catch ex As Exception
                Dim msg As String = String.Empty
                Dim rtle = TryCast(ex, Reflection.ReflectionTypeLoadException)
                If rtle IsNot Nothing Then

                    For Each txt In rtle.LoaderExceptions
                        msg += txt.Message
                    Next

                Else
                    msg = ex.Message
                End If
                _notLoadeds.Add(String.Format(ResStr("Can not load addin {0}. {1}"), addinfile, msg))
            End Try
        Next
        'AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CheckLoaded

        'If _notLoadeds.Count > 0 Then
        '    MessageBox.Show(String.Join(Environment.NewLine, _notLoadeds.ToArray), "Info", MessageBoxButtons.OK)
        'End If


        PhoebusAssemblies.InvalidateCahed()

        PhoebusAssemblies.GetBOAssemblies()



        'RegisterOCRSDK()

    End Sub

End Module
