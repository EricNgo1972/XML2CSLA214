Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Public Function GenerateReadonlyListCode() As String

            If IsSingleton Then Return String.Empty

            If DBTableName.MatchesRegExp("pbs_RFMSC") Then

                Return GenerateReadonlyListRFMSCCode()

            ElseIf UseSyncInfoList Then

                Return GenerateReadonlyListSyncCode()

            Else

                Return GenerateReadonlyListAsyncCode()

            End If

        End Function

        Private Function GenerateReadonlyListAsyncCode() As String

            Dim sections = New List(Of String)

            sections.Add(BuildReadonlyListFactories)

            sections.Add(BuildReadonlyListRESTAPI)

            sections.Add(BuildReadonlyListDataAccess)

            sections.Add(BuildReadonlyListDictionary)

            sections.Add(BuildQuery)

            Dim theCode = String.Join(Environment.NewLine, sections.ToArray)

            theCode = WrappWithReadonlyListClassName(theCode)

            theCode = WrappWithNameSpace(theCode)

            theCode = theCode.Insert(0, Environment.NewLine)

            theCode = theCode.Insert(0, BuildReadonlyListUsings)

            Return theCode

        End Function


#Region "Factories"

        Private Function BuildReadonlyListFactories() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        public <%= ClassName %>InfoList()
        {
           //_DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
        }

        public static async Task&lt;<%= CLS %>InfoList> GetInfoListAsync(Dictionary&lt;string, string> pFilters)
        {
            if (pFilters == null || pFilters.Count == 0) //use cached list
            {
                if (_list == null || _DTB != Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)))
                { 
                    _DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
                    _list = await FetchInfoListAsync(pFilters); 
                }

                // var filteredByDAG = Helper.Transform.DataFilter.FilterByDAG(nameof(<%= CLS %>.<%= DAGField %>), _list);
                // return Helper.Transform.DataFilter.CreateInfoListFrom(filteredByDAG) as <%= CLS %>InfoList;    

                return _list;
            }
            else
            {
                // var filteredByDAG = Helper.Transform.DataFilter.FilterByDAG(nameof(<%= CLS %>.<%= DAGField %>), await FetchInfoListAsync(pFilters));
                // return Helper.Transform.DataFilter.CreateInfoListFrom(filteredByDAG) as <%= CLS %>InfoList;    
                
                return await FetchInfoListAsync(pFilters);
            }
        }

        private static async Task&lt;<%= CLS %>InfoList> FetchInfoListAsync(Dictionary&lt;string, string> pFilters)
        {
            if (Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>).ToString()))
            {
                return await APIDataPortalFetchAsync(pFilters);
            }
            else
            {
                string sqlText = Query.BuildSQLText(pFilters);
                return await DataPortal.FetchAsync&lt;<%= CLS %>InfoList>(new QueryCriteria() { SqlText = sqlText});
            }
        }

        public async static Task&lt;<%= CLS %>Info> Get<%= CLS %>InfoAsync(<%= GetParameterKeys(False) %>, bool EmptyInfo = false)
        {
            var dic = await GetInfoDicAsync();

            return dic.TryGetValue(<%= GetCombinedKeys() %>, out <%= CLS %>Info ret) ? ret : EmptyInfo ? <%= CLS %>Info.Empty<%= CLS %>Info(<%= GetUsingParameterKeys() %>) : null;

        }

        public async static Task&lt;bool> ContainsCodeAsync(<%= GetParameterKeys(False) %>)
        {
            var dic = await GetInfoDicAsync();

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

            retStr = WrappWithRegion(retStr, "Factory Async Methods")

            Return retStr

        End Function

        Private Function BuildReadonlyListRESTAPI() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        private static async Task&lt;<%= CLS %>InfoList> APIDataPortalFetchAsync(Dictionary&lt;string, string> pFilters)
        {
            var ret = new <%= CLS %>InfoList();

            var reader = await SPC.API.RESTDataPortal.FetchAsync&lt;SPC.API.APIListReader>(<%= CLS %>.APICommand, pFilters);

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

        Private Function BuildReadonlyListDataAccess() As String
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

                using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                {
                    using (var cm = cn.CreateSQLCommand())
                    {
                        cm.CommandType = CommandType.Text;

                        cm.CommandText = string.IsNullOrWhiteSpace(criteria.SqlText) ? $"SELECT * FROM <%= DBTableName %>{_DTB} --WHERE '" : criteria.SqlText;

                          using (var dr = new SPC.Data.SafeDataReader(cm.ExecuteReader()))
                            {
                                while (dr.Read())
                                {
                                    var info = <%= CLS %>Info.Get<%= CLS %>Info(dr);
                                    this.Add(info);
                                }
                            }
                    }
                }

                IsReadOnly = true;
                RaiseListChangedEvents = true;
            }

        }
        </template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Data Access Methods")

            Return retStr

        End Function


        Private Function BuildReadonlyListDictionary() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>
        [NonSerialized]
        private static Dictionary&lt;string, <%= CLS %>Info> _dic;

        private async static Task&lt;Dictionary&lt;string, <%= CLS %>Info>> GetInfoDicAsync()
        {
            if (_dic == null || _DTB != Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)))
            {
                var thelist = await <%= CLS %>InfoList.GetInfoListAsync(null);
                _dic = new Dictionary&lt;string, <%= CLS %>Info>(StringComparer.OrdinalIgnoreCase);

                foreach (var itm in thelist)
                {
                    if (!_dic.ContainsKey(itm.ToString()))   _dic.Add(itm.ToString(), itm);
                }
            }

            return _dic;
        }
                       </template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Cache Dictionary")

            Return retStr

        End Function

        Private Function BuildQuery() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            Dim template = <template>     
        [Serializable]
        private class Query
        {
            internal static string BuildSQLText(Dictionary&lt;string, string> pFilters)
            {

                if (pFilters == null || (pFilters.Count == 0)) return string.Empty;
                
                var _qd = QD.GetSystemQD("<%= CLS %>_QUERY");

                _qd.Descriptn = "System query";

                _qd.AnalQ0 = "<%= QDADD %>"; //remember to add QDA file

                _qd.InsertUpdateFilterDictionary(pFilters);

                _qd.XL_DTB = pFilters.GetSystemKey("$Entity", string.Empty);

                _qd.AddSelectedField("*");
                
                return _qd.BuildSQL();
            }
        }
                       </template>

            ret.Add(template.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "List Query")

            Return retStr

        End Function

#End Region

#Region "Class Header"

        Private Function BuildReadonlyListUsings() As String
            Return $"

using Csla;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPC;
using SPC.QD;
using SPC.Helper;
using SPC.Helper.Extension;
using System.Data;
using SPC.Data;
using SPC.Database;

"

        End Function


        Private Function WrappWithReadonlyListClassName(pCode As String) As String

            Return <code>
    [Serializable]
    public class <%= ClassName %>InfoList : Csla.ReadOnlyListBase&lt; <%= ClassName %>InfoList, <%= ClassName %>Info> 
    {    

     private static string _DTB = string.Empty;
    
    <%= pCode %>
    
    }

                   </code>.Value.Trim



        End Function


#End Region


    End Class

End Namespace
