Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Private Function GenerateReadonlyListSyncCode() As String

            Dim sections = New List(Of String)

            sections.Add(BuildReadonlyListSyncFactories)

            sections.Add(BuildReadonlyListSyncRESTAPI)

            sections.Add(BuildReadonlyListDataAccess)

            sections.Add(BuildReadonlyListDictionarySync)

            sections.Add(BuildQuery)

            Dim theCode = String.Join(Environment.NewLine, sections.ToArray)

            theCode = WrappWithReadonlyListClassName(theCode)

            theCode = WrappWithNameSpace(theCode)

            theCode = theCode.Insert(0, Environment.NewLine)

            theCode = theCode.Insert(0, BuildReadonlyListUsings)

            Return theCode

        End Function


#Region "Factories"

        Private Function BuildReadonlyListSyncFactories() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        public <%= ClassName %>InfoList()
        {
           //_DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
        }

        public static <%= CLS %>InfoList GetInfoList(Dictionary&lt;string, string> pFilters)
        {
            if (pFilters == null || pFilters.Count == 0) //use cached list
            {
               if (_list == null || _DTB != Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)))
                { 
                    _DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
                    _list = FetchInfoList(pFilters); 
                }
                
                // var filteredByDAG = Helper.Transform.DataFilter.FilterByDAG(nameof(<%= CLS %>.<%= DAGField %>), _list);
                // return Helper.Transform.DataFilter.CreateInfoListFrom(filteredByDAG) as <%= CLS %>InfoList;     
                
                return _list;
            }
            else
            {
                return FetchInfoList(pFilters);
            }

        }

        private static <%= CLS %>InfoList FetchInfoList(Dictionary&lt;string, string> pFilters)
        {
            if (Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>).ToString()))
            {
                return APIDataPortalFetch(pFilters);
            }
            else
            {
                string sqlText = Query.BuildSQLText(pFilters);
                return DataPortal.Fetch&lt;<%= CLS %>InfoList>(new QueryCriteria() { SqlText = sqlText});
            }
        }

        public static <%= CLS %>Info Get<%= CLS %>Info(<%= GetParameterKeys(False) %>, bool EmptyInfo = false)
        {
            var dic = GetInfoDic();

            return dic.TryGetValue(<%= GetCombinedKeys() %>, out <%= CLS %>Info ret) ? ret : EmptyInfo ? <%= CLS %>Info.Empty<%= CLS %>Info(<%= GetUsingParameterKeys() %>) : null;

        }

        public static bool ContainsCode(<%= GetParameterKeys(False) %>)
        {
            var dic = GetInfoDic();

           return dic.ContainsKey(<%= GetCombinedKeys() %>);
           
        }

        private static <%= CLS %>InfoList _list = null;
        internal static void InvalidateCache()
        {
            _list = null;
            _dic = null;
        }            </template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Factory Methods")

            Return retStr

        End Function

        Private Function BuildReadonlyListSyncRESTAPI() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        private static <%= CLS %>InfoList APIDataPortalFetch(Dictionary&lt;string, string> pFilters)
        {
            var ret = new <%= CLS %>InfoList();

            var reader = SPC.API.RESTDataPortal.Fetch&lt;SPC.API.APIListReader>(<%= CLS %>.APICommand, pFilters);

            if (reader != null &amp;&amp; reader.Data != null)
            {
                ret.IsReadOnly = false;
                ret.RaiseListChangedEvents = false;

                foreach (var itm in reader.Data)
                {
                    var info = <%= CLS %>Info.Get<%= CLS %>Info(itm);
                    ret.Add(info);
                }
                
                ret.IsReadOnly = true;
                ret.RaiseListChangedEvents = true;
            }

            return ret;
        }
                       </template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "REST API Methods")

            Return retStr

        End Function

#End Region

#Region "DataAccess"

        'Private Function BuildReadonlyListDataAccessSync() As String
        '    Dim ret = New List(Of String)

        '    Dim CLS = ClassName

        '    Dim template = <template>
        '[Serializable]
        'private class QueryCriteria: CriteriaBase&lt;QueryCriteria>
        '{
        '    public static readonly PropertyInfo&lt;string> SqlTextProperty = RegisterProperty&lt;string>(c => c.SqlText);
        '    public string SqlText
        '    {
        '        get { return ReadProperty(SqlTextProperty); }
        '        set { LoadProperty(SqlTextProperty, value); }
        '    }
        '}

        '[NonSerialized,NotUndoable]
        'private static object _lockObj = new object();

        'private void DataPortal_Fetch(QueryCriteria criteria)
        '{
        '    lock(_lockObj)
        '    {
        '        RaiseListChangedEvents = false;
        '        IsReadOnly = false;

        '        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
        '        {
        '            using (var cm = cn.CreateSQLCommand())
        '            {
        '                cm.CommandType = CommandType.Text;

        '                cm.CommandText = string.IsNullOrWhiteSpace(criteria.SqlText) ? $"SELECT * FROM <%= DBTableName %>{_DTB} --WHERE '" : criteria.SqlText;

        '                  using (var dr = new SPC.Data.SafeDataReader(cm.ExecuteReader()))
        '                    {
        '                        while (dr.Read())
        '                        {
        '                            var info = <%= CLS %>Info.Get<%= CLS %>Info(dr);
        '                            this.Add(info);
        '                        }
        '                    }
        '            }
        '        }

        '        IsReadOnly = true;
        '        RaiseListChangedEvents = true;
        '    }

        '}</template>

        '    ret.Add(template.Value)

        '    Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

        '    retStr = WrappWithRegion(retStr, "Data Access Methods")

        '    Return retStr

        'End Function

        Private Function BuildReadonlyListDictionarySync() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        [NonSerialized]
        private static Dictionary&lt;string, <%= CLS %>Info> _dic;

        private static Dictionary&lt;string, <%= CLS %>Info> GetInfoDic()
        {
            if (_dic == null || _DTB != Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)))
            {
                var thelist = <%= CLS %>InfoList.GetInfoList(null);
                _dic = new Dictionary&lt;string, <%= CLS %>Info>();

                foreach (var itm in thelist)
                {
                    if (!_dic.ContainsKey(itm.ToString()))
                        _dic.Add(itm.ToString(), itm);
                }
            }

            return _dic;
        }              </template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Cache Dictionary")

            Return retStr

        End Function

#End Region




    End Class

End Namespace
