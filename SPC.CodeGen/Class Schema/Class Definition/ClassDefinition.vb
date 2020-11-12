Imports pbs.Helper

Namespace MetaData

    Public Class EditableDefinition

        Private _className As String
        Public Property ClassName() As String
            Get
                Return _className
            End Get
            Set(ByVal value As String)
                _className = value
            End Set
        End Property

        Private _description As String
        Public Property Description() As String
            Get
                Return Nz(_description, ClassName.Translate())
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Private _classNameSpace As String
        Public Property ClassNameSpace() As String
            Get
                Return _classNameSpace
            End Get
            Set(ByVal value As String)
                _classNameSpace = value
            End Set
        End Property

        Private _isSingleton As String
        Public Property IsSingleton() As Boolean
            Get
                Return _isSingleton.ToBoolean()
            End Get
            Set(ByVal value As Boolean)
                _isSingleton = If(value, "Y", "N")
            End Set
        End Property

        Private _useSyncInfoList As String
        Public Property UseSyncInfoList() As Boolean
            Get
                Return _useSyncInfoList.ToBoolean()
            End Get
            Set(ByVal value As Boolean)
                _useSyncInfoList = If(value, "Y", "N")
            End Set
        End Property

        Friend _dagField As String
        Public Property DAGField() As String
            Get
                Return Nz(_dagField, "DAG")
            End Get
            Set(ByVal value As String)
                _dagField = value
            End Set
        End Property

#Region "Database"

        Private _QDADD As String
        Public Property QDADD As String
            Get
                Return Nz(_QDADD, ClassName.Leaf())
            End Get
            Set(ByVal value As String)
                _QDADD = value
            End Set
        End Property

        Private _DBTableName As String
        Public Property DBTableName() As String
            Get
                Return _DBTableName
            End Get
            Set(ByVal value As String)
                _DBTableName = value
            End Set
        End Property

        Private _insertSPC As String
        Public Property InsertSP() As String
            Get
                Return _insertSPC
            End Get
            Set(ByVal value As String)
                _insertSPC = value
            End Set
        End Property

        Private _updateSP As String
        Public Property UpdateSP() As String
            Get
                Return _updateSP
            End Get
            Set(ByVal value As String)
                _updateSP = value
            End Set
        End Property

        Private _RFMSCSubTable As String
        Public Property SubTable() As String
            Get
                Return _RFMSCSubTable
            End Get
            Set(ByVal value As String)
                _RFMSCSubTable = value
            End Set
        End Property

#End Region

#Region "Fields"

        Private _fields As List(Of ClassField)
        Public ReadOnly Property Fields() As List(Of ClassField)
            Get

                If _fields Is Nothing Then
                    _fields = New List(Of ClassField)
                End If

                Return _fields
            End Get

        End Property

        Function SingleFields() As List(Of ClassField)
            Return (From f In Fields Where Not f.IsChildCollection).ToList
        End Function

        Function Keys() As List(Of ClassField)
            Return (From f In Fields Where f.IsPrimaryKey).ToList
        End Function

        Function Tables() As List(Of ClassField)
            Return (From f In Fields Where f.IsChildCollection).ToList
        End Function

#End Region

#Region "Scripts"

        Property Scripts As List(Of ActionScript) = New List(Of ActionScript)

        Class ActionScript
            Property Code As String
            Property Image As String
            Property Caption As String
            Property Location As pbs.BO.Script.ScriptButtonLocation

            Function GenerateScriptsCode() As String

                Return <txt>
        private UITasks <%= Code %>_Imp()
        {
            var scripts = new UITasks(this);

            scripts.IconName = "<%= Image %>";

            scripts.CaptionKey = "<%= Caption %>";

            scripts.ButtonLocation = <%= Location.ToString %>;

            scripts.AddCallMethod(10, "<%= Code %>");

            return scripts;
        }

        private async void <%= Code %>()
        {
            ///if (_current != null)
            ///{
            ///    var arg = new CmdArg($"{typeof(SPC.QD.QD).ToString()}?QdId={_current.QdId}&amp;$action=Run");
            ///
            ///   await Services.UI.RunURLService.RunAsync(arg);
            ///}
        }
        </txt>.Value

            End Function

        End Class

#End Region
    End Class

End Namespace
