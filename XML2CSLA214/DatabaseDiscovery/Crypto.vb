Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports System.IO

Friend Class Crypto

    Friend Shared Function Str2Bytes(ByVal str As String) As Byte()
        Dim encoding As New System.Text.UTF8Encoding()
        Return encoding.GetBytes(str)
    End Function

    Friend Shared Function Bytes2Str(ByVal bytes As Byte()) As String
        Dim enc As New System.Text.UTF8Encoding()
        Return enc.GetString(bytes)

    End Function

    Friend Shared Function EncryptStr(ByVal sourceStr As String) As Byte()
        Dim fStream As New MemoryStream
        Try
            Dim key As Byte() = Str2Bytes("HugeRick".PadRight(32))
            Dim vector As Byte() = Str2Bytes("HugeRick".PadLeft(16))

            Dim RijndaelAlg As RijndaelManaged = RijndaelManaged.Create


            Using cStream As New CryptoStream(fStream, _
                                           RijndaelAlg.CreateEncryptor(key, vector), _
                                           CryptoStreamMode.Write)
                Using sWriter As New StreamWriter(cStream)
                    sWriter.WriteLine(sourceStr)
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Can not encrypt text")
        End Try

        Return fStream.ToArray()

    End Function

    Friend Shared Function DecryptBytes(ByVal encryptedBytes As Byte()) As String
        Dim retStr As String = String.Empty
        Try
            Dim key As Byte() = Str2Bytes("HugeRick".PadRight(32))
            Dim vector As Byte() = Str2Bytes("HugeRick".PadLeft(16))

            Using fStream As New MemoryStream(encryptedBytes)

                Dim RijndaelAlg As RijndaelManaged = RijndaelManaged.Create
                Using cStream As New CryptoStream(fStream, _
                                                RijndaelAlg.CreateDecryptor(key, vector), _
                                                CryptoStreamMode.Read)

                    Using sReader As New StreamReader(cStream)
                        retStr = sReader.ReadLine()
                    End Using

                End Using

            End Using
        Catch ex As Exception
            Throw New Exception("Can not decrypt text")
        End Try

        Return retStr

    End Function

    Friend Shared Function Hash(ByVal txt As String) As String
        Dim sha1CryptoService As SHA1CryptoServiceProvider = New SHA1CryptoServiceProvider()
        Dim byteValue() As Byte = encoding.UTF8.GetBytes(Nz(txt, String.Empty))
        Dim hashValue() As Byte = sha1CryptoService.ComputeHash(byteValue)

        Return Convert.ToBase64String(hashValue)

        'Return System.Text.Encoding.ASCII.GetString(hashValue)
    End Function

    Friend Shared Function GenerateHashDigest(ByVal source As String, ByVal algorithm As HashMethod) As String
        Dim salt As String = "HugeRick"
        Dim hashAlgorithm As HashAlgorithm = Nothing
        Select Case algorithm
            Case HashMethod.MD5
                hashAlgorithm = New MD5CryptoServiceProvider
            Case HashMethod.SHA1
                hashAlgorithm = New SHA1CryptoServiceProvider
            Case HashMethod.SHA384
                hashAlgorithm = New SHA384Managed
            Case Else
                ' Error case.
        End Select

        Dim byteValue() As Byte = encoding.UTF8.GetBytes(source & salt)
        Dim hashValue1() As Byte = hashAlgorithm.ComputeHash(byteValue)
        Dim hashValue2() As Byte = hashAlgorithm.ComputeHash(hashValue1)
        Return Convert.ToBase64String(hashValue2)
    End Function

End Class

Public Module pbsHash
    Public Function Hash(ByVal txt As String) As String
        Return Crypto.Hash(txt)
    End Function
End Module

Friend Enum HashMethod
    MD5
    SHA1
    SHA384
End Enum