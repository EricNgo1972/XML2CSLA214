Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Private Function GenerateReadonlyListRFMSCCode() As String

            Dim sections = New List(Of String)

            sections.Add(BuildReadonlyListRFMSCFactories)

            sections.Add(BuildReadonlyListRFMSCRESTAPI)

            sections.Add(BuildReadonlyListDataAccessRFMSC)

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

        Private Function BuildReadonlyListRFMSCFactories() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        public <%= ClassName %>InfoList()
        {
           //_DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
        }

        public static <%= CLS %>InfoList GetInfoList(Dictionary&lt;string, string> pFilters)
        {
            if (_list == null || _DTB != Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)))
            {
                _DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
                _list = FetchInfoList(); 
            }
            
            if(pFilters == null || pFilters.Count == 0)
                return _list;
            else
            {
                //var filteredByDAG = await Helper.Transform.DataFilter.FilterByDAG(nameof(<%= CLS %>.<%= DAGField %>), _list);
                //return SPC.Helper.Transform.DataFilter.CreateInfoListFrom(Helper.Transform.DataFilter.FilterInfoList_BySearchingCriteria(filteredByDAG, pFilters)) as <%= CLS %>InfoList;    

                return SPC.Helper.Transform.DataFilter.CreateInfoListFrom(SPC.Helper.Transform.DataFilter.FilterInfoList_BySearchingCriteria(_list, pFilters)) as <%= CLS %>InfoList;    
            }           

        }

       private static <%= CLS %>InfoList FetchInfoList()
        {
            if (Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>).ToString()))
            {
                return APIDataPortalFetch();
            }
            else
            {
               return DataPortal.Fetch&lt;<%= CLS %>InfoList>(new QueryCriteria());
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
       
        [NonSerialized]
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

        Private Function BuildReadonlyListRFMSCRESTAPI() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
       private static <%= CLS %>InfoList APIDataPortalFetch()
        {
            var ret = new <%= CLS %>InfoList();

            var reader = SPC.API.RESTDataPortal.Fetch&lt;SPC.API.APIListReader>(<%= CLS %>.APICommand);

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

        Private Function BuildReadonlyListDataAccessRFMSC() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        [Serializable]
        private class QueryCriteria: CriteriaBase&lt;QueryCriteria>
        {
            public static readonly PropertyInfo&lt;string> SqlTextProperty = RegisterProperty&lt;string>(c => c.SqlText);
            public string SqlText
            {
                get { return ReadProperty(SqlTextProperty); }
                set { LoadProperty(SqlTextProperty, value); }
            }
        }

       [NonSerialized,NotUndoable]
        private static object _lockObj = new object();

        private void DataPortal_Fetch(QueryCriteria criteria)
        {
            lock(_lockObj)
            {
                RaiseListChangedEvents = false;
                IsReadOnly = false;

                    using (var cn = ConnectionFactory.GetDBConnection(true))
                    {
                          using (var cm = cn.Get_RFMSC_FetchAllCommand(_DTB, <%= CLS %>.PBS_TB))
                            {
                              using(var dr = new SPC.Data.SafeDataReader(cm.ExecuteReader()))
                                {
                                    while (dr.Read())
                                    {
                                        var info = <%= CLS %>Info.Get<%= CLS %>Info(dr);
                                        this.Add(info);
                                 }   }

                            }

                    }

                IsReadOnly = true;
                RaiseListChangedEvents = true;
            }

        }</template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Data Access Methods")

            Return retStr

        End Function

#End Region




    End Class

End Namespace
