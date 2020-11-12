Public Class CSLA411Factory

    Private _schema As Schema
    Public Sub New(ByVal pSchema As Schema)
        _schema = pSchema
    End Sub

#Region "EditableRFMSC"

    Friend Function Get_RFMSC411() As String
        Dim template = <template>
using System;
using System.Data;
using System.Text;
using SPC.Helper.Extension;
using Csla;
using SPC.Helper;
using SPC.SmartData;
using SPC.Interfaces;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SPC.API;
using SPC.Helper.Exceptions;       
using SmartDate = SPC.SmartData.SmartDate;
using System.Xml.Linq;
using SPC.Services;
using SPC.Data;

namespace  <%= _schema._nameSpace %>
{
    [Serializable]
    [PhoebusCommand(Desc = "<%= _schema._className %>")]
    [DB(TableName = "pbs_RFMSC/<%= _schema._subTableCode %>")]
    public partial class <%= _schema._className %> : Csla.BusinessBase&lt; <%= _schema._className %>>, IDocLink
    {
        public const string APICommand = "<%= $"{_schema._nameSpace}.{_schema._className}".Replace("SPC.", "pbs_").ToLower.Replace(".", "_") %>";

        public const string PBS_TB = "<%= _schema._subTableCode %>";

              </template>

        Dim ret = New List(Of String)

        ret.Add(template.Value)

        ret.AddRange(GetProperties_Editable)

        ret.AddRange(GetDocLinkInterface)

        ret.AddRange(Get_Editable_RulesSection)

        ret.AddRange(Get_Editable_FactorySection)

        ret.AddRange(Get_DataPortal_RFMSC411(False))

        ret.Add("}}")

        Return String.Join(Environment.NewLine, ret)
    End Function

#End Region

#Region "Editable"

    Friend Function Get_Editable() As String
        Dim template = <template>
using System;
using System.Data;
using System.Text;
using SPC.Helper.Extension;
using Csla;
using SPC.Helper;
using SPC.SmartData;
using SPC.Interfaces;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SPC.API;
using SPC.Helper.Exceptions;       
using SmartDate = SPC.SmartData.SmartDate;
using SPC.Data;

namespace  <%= _schema._nameSpace %>
{
    [Serializable]
    [PhoebusCommand(Desc = "<%= _schema._className %>")]
    [DB(TableName = "<%= _schema._DBTable %>")]
    public class <%= _schema._className %> : Csla.BusinessBase&lt; <%= _schema._className %>>, IDocLink, IEditable
    {
        public const string APICommand = "<%= $"{_schema._nameSpace}.{_schema._className}".Replace("SPC.", "pbs_").ToLower.Replace(".", "_") %>";
              </template>

        Dim ret = New List(Of String)

        ret.Add(template.Value)

        ret.AddRange(GetProperties_Editable)
        ret.AddRange(GetDocLinkInterface)

        ret.AddRange(Get_Editable_RulesSection)

        ret.AddRange(Get_Editable_FactorySection)
        ret.AddRange(Get_DataPortal_411(False))

        ret.Add("}}")

        Return String.Join(Environment.NewLine, ret)
    End Function

    Friend Function GetProperties_Editable() As List(Of String)

        Dim ret = New List(Of String)

        ret.Add("#region Business Properties")

        '     ret.Add("private string _DTB = string.Empty;")


        ret.Add(<text>

        public static readonly PropertyInfo&lt;string> DTBProperty = RegisterProperty&lt;string>(c => c.DTB, String.Empty);
        private string DTB
        {
            get { return ReadProperty(DTBProperty); }
            set { LoadProperty(DTBProperty, value); }
        }
                    </text>.Value.TrimStart)

        For Each finfo In _schema.Members

            If finfo._id <> "DTB" AndAlso finfo._id <> "SITE" Then
                ret.AddRange(finfo.GetOneProperty_Editable)
                ret.Add(vbNewLine)
            End If

        Next

        Dim ToStr = <code>
            public override string ToString()
            {
                return <%= _schema.GetCombinedKeyValues %>;
            }
                    </code>

        ret.Add(ToStr.Value.Trim)
        ret.Add(vbNewLine)

        ret.Add("#endregion Business Properties")
        ret.Add(vbNewLine)
        Return ret

    End Function

    Friend Function GetDocLinkInterface() As List(Of String)
        Dim ret As New List(Of String)

        ret.Add("#region IDoclink")

        ret.Add(<t> string IDocLink.GetDocLinkRef() { return String.Format("{0}#{1}", (this as IDocLink).GetDocType(), <%= _schema.Keys(0)._id.toPropertyName %>); }</t>.Value)

        ret.Add(vbNewLine)

        ret.Add(<t> string IDocLink.GetDocType() { return this.GetType().ToEditableClassName().Leaf(); }</t>.Value)

        ret.Add("#endregion IDoclink")
        ret.Add(vbNewLine)
        ret.Add(vbNewLine)

        Return ret
    End Function


#End Region

#Region "Readonly"
    Friend Function Get_RFMSCInfo() As String

        Dim ret = New List(Of String)
        Dim CLS = _schema._className

        Dim template = <template>
using System;
using System.Data;
using System.Text;
using SPC.Helper.Extension;
using Csla;
using SPC.Helper;
using SPC.SmartData;
using SPC.Interfaces;
using System.Text.RegularExpressions;
using System.Collections.Generic;       
using SmartDate = SPC.SmartData.SmartDate;
using System.Xml.Linq;
using SPC.Services;

namespace  <%= _schema._nameSpace %>
{
    [Serializable]
     public class <%= _schema._className %>Info : Csla.ReadOnlyBase&lt; <%= _schema._className %>Info>, IDocLink
    {
      
                       </template>

        ret.Add(template.Value)

        ret.AddRange(GetProperties_Readonly)

        ret.AddRange(GetDocLinkInterface)

        ret.AddRange(Get_Readonly_FactorySection)

        ret.Add(" #region Data Access")
        ret.Add(vbNewLine)

        ret.AddRange(Get_Fetch_SafeDataReader411(True))

        ret.Add(" #endregion Data Access")

        ret.Add("}}")

        Return String.Join(Environment.NewLine, ret.ToArray)
    End Function


    Friend Function Get_Readonly() As String

        Dim ret = New List(Of String)
        Dim CLS = _schema._className

        Dim template = <template>
using System;
using System.Data;
using System.Text;
using SPC.Helper.Extension;
using Csla;
using SPC.Helper;
using SPC.SmartData;
using SPC.Data;
using SPC.Interfaces;
using System.Text.RegularExpressions;
using System.Collections.Generic;       
using SmartDate = SPC.SmartData.SmartDate;

namespace  <%= _schema._nameSpace %>
{
    [Serializable]
     public class <%= _schema._className %>Info : Csla.ReadOnlyBase&lt; <%= _schema._className %>Info>, IDocLink
    {
      
                       </template>

        ret.Add(template.Value)

        ret.AddRange(GetProperties_Readonly)
        ret.AddRange(GetDocLinkInterface)
        ret.AddRange(Get_Readonly_FactorySection)

        ret.Add(" #region Data Access")
        ret.Add(vbNewLine)

        ret.AddRange(Get_Fetch_SafeDataReader411(True))

        ret.Add(" #endregion Data Access")

        ret.Add("}}")

        Return String.Join(Environment.NewLine, ret.ToArray)
    End Function

    Friend Function GetProperties_Readonly() As List(Of String)

        Dim ret = New List(Of String)

        ret.Add("#region Business Properties")
        ret.Add(vbNewLine)

        For Each finfo In _schema.Members

            If finfo._id <> "DTB" AndAlso finfo._id <> "SITE" Then
                ret.AddRange(finfo.GetOneProperty_Readonly)
                ret.Add(vbNewLine)
            End If
        Next

        Dim ToStr = <code>
            public override string ToString()
            {
            return <%= _schema.GetCombinedKeyValues %>;
            }
                    </code>

        ret.Add(ToStr.Value.Trim)
        ret.Add(vbNewLine)

        ret.Add("#endregion Business Properties")
        ret.Add(vbNewLine)
        Return ret

    End Function



#End Region

#Region "Readonly List 411"

    Friend Function Get_ReadonlyList() As String

        Dim ret = New List(Of String)

        Dim CLS = _schema._className

        Dim template = <template>
using Csla;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SPC.Helper;
using SPC.Helper.Extension;
using System.Data;

namespace <%= _schema._nameSpace %>
{
    [Serializable]
    public class <%= CLS %>InfoList : Csla.ReadOnlyListBase&lt;<%= CLS %>InfoList, <%= CLS %>Info>
    {
        
        private static string _DTB = string.Empty;

        #region Factory

        public <%= _schema._className %>InfoList()
        {
           _DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
        }

        public static async Task&lt;<%= CLS %>InfoList> GetInfoListAsync(Dictionary&lt;string, string> pFilters)
        {
            if (pFilters == null || pFilters.Count == 0) //use cached list
            {
                if (_list == null)
                { _list = await FetchInfoListAsync(pFilters); }

                return _list;
            }
            else
            {
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
                return await DataPortal.FetchAsync&lt;<%= CLS %>InfoList>(new QueryCriteria() { _sqlText = sqlText});
            }
        }

        public async static Task&lt;<%= CLS %>Info> Get<%= CLS %>InfoAsync(<%= _schema.GetParameterKeys(False) %>, bool EmptyInfo = false)
        {
            var dic = await GetInfoDicAsync();

            return dic.TryGetValue(<%= _schema.GetCombinedKeys %>, out <%= CLS %>Info ret) ? ret : EmptyInfo ? <%= CLS %>Info.Empty<%= CLS %>Info(<%= _schema.GetUsingParameterKeys %>) : null;

        }

        public async static Task&lt;bool> ContainsCodeAsync(<%= _schema.GetParameterKeys(False) %>)
        {
            var dic = await GetInfoDicAsync();

           return dic.ContainsKey(<%= _schema.GetCombinedKeys %>);
           
        }

        private static <%= CLS %>InfoList _list = null;
        internal static void InvalidateCache()
        {
            _list = null;
            _dic = null;
        }

        #endregion

        #region RESTAPI Access

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

        #endregion

        #region "Data Access"

        [Serializable]
        private class QueryCriteria
        {
            internal string _sqlText = String.Empty;

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

                        cm.CommandText = string.IsNullOrWhiteSpace(criteria._sqlText) ? $"SELECT * FROM <%= _schema._DBTable %>{_DTB} --WHERE '" : criteria._sqlText;

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

        #endregion
  
        #region Dictionary

        private static Dictionary&lt;string, <%= CLS %>Info> _dic;

        private async static Task&lt;Dictionary&lt;string, <%= CLS %>Info>> GetInfoDicAsync()
        {
            if (_dic == null || _DTB != Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)))
            {
                var thelist = await <%= CLS %>InfoList.GetInfoListAsync(null);
                _dic = new Dictionary&lt;string, <%= CLS %>Info>();

                foreach (var itm in thelist)
                {
                    if (!_dic.ContainsKey(itm.ToString()))
                        _dic.Add(itm.ToString(), itm);
                }
            }

            return _dic;
        }

        #endregion

        [Serializable]
        private class Query
        {
            internal static string BuildSQLText(Dictionary&lt;string, string> pFilters)
            {

                if (pFilters == null || (pFilters.Count == 0)) return string.Empty;
                
                var _qd = QD.QD.GetSystemQD("<%= CLS %>_QUERY");

                _qd.Descriptn = "System query";

                _qd.AnalQ0 = "<%= CLS %>"; //remember to add QDA file

                _qd.InsertUpdateFilterDictionary(pFilters);

                _qd.XL_DTB = pFilters.GetSystemKey("$Entity", string.Empty);

                _qd.AddSelectedField("*");
                
                return _qd.BuildSQL();
            }
        }
    }
}
                       </template>

        ret.Add(template.Value)


        Return String.Join(Environment.NewLine, ret.ToArray)
    End Function


    Friend Function Get_ReadonlyListNonAsync() As String

        Dim ret = New List(Of String)

        Dim CLS = _schema._className

        Dim template = <template>
using Csla;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SPC.Helper;
using SPC.Helper.Extension;
using System.Data;

namespace <%= _schema._nameSpace %>
{
    [Serializable]
    public class <%= CLS %>InfoList : Csla.ReadOnlyListBase&lt;<%= CLS %>InfoList, <%= CLS %>Info>
    {
        
        private static string _DTB = string.Empty;

        #region Factory

        public <%= _schema._className %>InfoList()
        {
           _DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
        }

        public static <%= CLS %>InfoList GetInfoList(Dictionary&lt;string, string> pFilters)
        {
            if (pFilters == null || pFilters.Count == 0) //use cached list
            {
                if (_list == null)
                { _list = FetchInfoList(pFilters); }

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
                return DataPortal.Fetch&lt;<%= CLS %>InfoList>(new QueryCriteria() { _sqlText = sqlText});
            }
        }

        public static <%= CLS %>Info Get<%= CLS %>Info(<%= _schema.GetParameterKeys(False) %>, bool EmptyInfo = false)
        {
            var dic = GetInfoDic();

            return dic.TryGetValue(<%= _schema.GetCombinedKeys %>, out <%= CLS %>Info ret) ? ret : EmptyInfo ? <%= CLS %>Info.Empty<%= CLS %>Info(<%= _schema.GetUsingParameterKeys %>) : null;

        }

        public static bool ContainsCode(<%= _schema.GetParameterKeys(False) %>)
        {
            var dic = GetInfoDic();

           return dic.ContainsKey(<%= _schema.GetCombinedKeys %>);
           
        }

        private static <%= CLS %>InfoList _list = null;
        internal static void InvalidateCache()
        {
            _list = null;
            _dic = null;
        }

        #endregion

        #region RESTAPI Access

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

        #endregion

        #region "Data Access"

        [Serializable]
        private class QueryCriteria
        {
            internal string _sqlText = String.Empty;

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

                        cm.CommandText = string.IsNullOrWhiteSpace(criteria._sqlText) ? $"SELECT * FROM <%= _schema._DBTable %>{_DTB} --WHERE '" : criteria._sqlText;

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

        #endregion
  
        #region Dictionary

        private static Dictionary&lt;string, <%= CLS %>Info> _dic;

        private static Dictionary&lt;string, <%= CLS %>Info> GetInfoDicAsync()
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
        }

        #endregion

        [Serializable]
        private class Query
        {
            internal static string BuildSQLText(Dictionary&lt;string, string> pFilters)
            {

                if (pFilters == null || (pFilters.Count == 0)) return string.Empty;
                
                var _qd = QD.QD.GetSystemQD("<%= CLS %>_QUERY");

                _qd.Descriptn = "System query";

                _qd.AnalQ0 = "<%= CLS %>"; //remember to add QDA file

                _qd.InsertUpdateFilterDictionary(pFilters);

                _qd.XL_DTB = pFilters.GetSystemKey("$Entity", string.Empty);

                _qd.AddSelectedField("*");
                
                return _qd.BuildSQL();
            }
        }
    }
}
                       </template>

        ret.Add(template.Value)


        Return String.Join(Environment.NewLine, ret.ToArray)
    End Function


    Friend Function Get_RFMSCReadonlyList() As String

        Dim ret = New List(Of String)

        Dim CLS = _schema._className

        Dim template = <template>
using Csla;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SPC.Helper;
using SPC.Helper.Extension;
using System.Data;
using SPC.Data;
using SPC.Database;

namespace <%= _schema._nameSpace %>
{
    [Serializable]
    public class <%= CLS %>InfoList : Csla.ReadOnlyListBase&lt;<%= CLS %>InfoList, <%= CLS %>Info>
    {
        
        private static string _DTB = string.Empty;

        #region Factory

        public <%= _schema._className %>InfoList()
        {
           _DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));
        }

        public static <%= CLS %>InfoList GetInfoList(Dictionary&lt;string, string> pFilters)
        {
           
            if (_list == null) _list = FetchInfoList(); 
            
            if(pFilters == null || pFilters.Count == 0)
                return _list;
            else
            {
                //var filteredByDAG = await Helper.Transform.DataFilter.FilterByDAG(nameof(DLS.DataAccessGroup), _list);
                //return Helper.Transform.DataFilter.CreateInfoListFrom(Helper.Transform.DataFilter.FilterInfoList_BySearchingCriteria(filteredByDAG, pFilters)) as <%= _schema._className %>InfoList;    

                return Helper.Transform.DataFilter.CreateInfoListFrom(Helper.Transform.DataFilter.FilterInfoList_BySearchingCriteria(_list, pFilters)) as <%= _schema._className %>InfoList;    
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

        public static <%= CLS %>Info Get<%= CLS %>Info(<%= _schema.GetParameterKeys(False) %>, bool EmptyInfo = false)
        {
            var dic = GetInfoDic();

            return dic.TryGetValue(<%= _schema.GetCombinedKeys %>, out <%= CLS %>Info ret) ? ret : EmptyInfo ? <%= CLS %>Info.Empty<%= CLS %>Info(<%= _schema.GetUsingParameterKeys %>) : null;

        }

        public static bool ContainsCode(<%= _schema.GetParameterKeys(False) %>)
        {
            var dic = GetInfoDic();

           return dic.ContainsKey(<%= _schema.GetCombinedKeys %>);
           
        }

        private static <%= CLS %>InfoList _list = null;
        internal static void InvalidateCache()
        {
            _list = null;
            _dic = null;
        }

        #endregion

        #region RESTAPI Access

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

        #endregion

        #region Data Access

        [Serializable]
        private class QueryCriteria
        {
            internal string _sqlText = String.Empty;

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

        }

        #endregion
  
        #region Dictionary

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
        }

        #endregion

        [Serializable]
        private class Query
        {
            internal static string BuildSQLText(Dictionary&lt;string, string> pFilters)
            {

                if (pFilters == null || (pFilters.Count == 0)) return string.Empty;
                
                var _qd = QD.QD.GetSystemQD("<%= CLS %>_QUERY");

                _qd.Descriptn = "System query";

                _qd.AnalQ0 = "<%= CLS %>"; //remember to add QDA file

                _qd.InsertUpdateFilterDictionary(pFilters);

                _qd.XL_DTB = pFilters.GetSystemKey("$Entity", string.Empty);

                _qd.AddSelectedField("*");
                
                return _qd.BuildSQL();
            }
        }
    }
}
                       </template>

        ret.Add(template.Value)


        Return String.Join(Environment.NewLine, ret.ToArray)
    End Function

#End Region

    Friend Function Get_Editable_RulesSection() As List(Of String)
        Dim ret = New List(Of String)

        Dim CLS = _schema._className

        ret.Add("#region Rules")


        ret.Add(<t>
        //Sample Rule class
        //private class URLMappingRule :Csla.Rules.BusinessRule
        //{
        //    public URLMappingRule(Csla.Core.IPropertyInfo primaryProperty) : base(primaryProperty)
        //    {
        //        InputProperties = new List&lt;Csla.Core.IPropertyInfo> { primaryProperty, InfoQdIdProperty, LinkedUrlProperty };
        //    }
        //    protected override void Execute(IRuleContext context)
        //    {
        //        if (!string.IsNullOrWhiteSpace(context.GetInputValue&lt;string>(InfoQdIdProperty)) &amp;&amp;  string.IsNullOrWhiteSpace(context.GetInputValue&lt;string>(InfoCodeProperty)))
        //        {
        //            context.AddErrorResult(InfoCodeProperty, Voca.Translate("You must select at least one column, which display the list code of the QD"));
        //            return;
        //        }
        //
        //        if (!string.IsNullOrWhiteSpace(context.GetInputValue&lt;string>(LinkedUrlProperty)) &amp;&amp; string.IsNullOrWhiteSpace(context.GetInputValue&lt;string>(InfoCodeProperty)))
        //        {
        //            context.AddErrorResult(InfoCodeProperty, Voca.Translate("You must select at least one column, which display the list code of URL"));
        //            return;
        //        }
//
  //          }
    //    }
     </t>.Value)

        '----------------AddSharedCommonRules
        ret.Add(vbNewLine)
        ret.Add(<t>
        private void AddSharedCommonRules()
        {
            // Sample simple custom rule

           // BusinessRules.AddRule(new URLMappingRule(InfoCodeProperty));
        }       </t>.Value)

        '----------------NewAsync
        ret.Add(vbNewLine)
        ret.Add(<t>
        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();

            foreach (Field _field in ClassSchema&lt;<%= CLS %>>._fieldList.Values)
            {
                if (_field.Required)
                    BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(FieldManager.GetRegisteredProperty(_field.PropertyName)));

                if (!string.IsNullOrEmpty(_field.RegexPattern))
                    BusinessRules.AddRule(new Csla.Rules.CommonRules.RegExMatch(FieldManager.GetRegisteredProperty(_field.PropertyName), _field.RegexPattern));
            }

            AddSharedCommonRules();
        }
                </t>.Value)


        ret.Add("#endregion")
        ret.Add(vbNewLine)

        Return ret
    End Function


#Region "Factory Methods"

    Friend Function Get_Editable_FactorySection() As List(Of String)
        Dim ret = New List(Of String)

        Dim CLS = _schema._className

        ret.Add("#region Factory Methods")
        ret.Add(vbNewLine)

        ret.Add(<t>public <%= CLS %>() {  DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)); }</t>.Value)

        '----------------NewAsync
        ret.Add(vbNewLine)
        ret.Add(<t>public static async Task&lt;<%= CLS %>> New<%= CLS %>Async(<%= _schema.GetParameterKeys(False) %>) 
                    { 
                        if (Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return await APIDataPortalCreateAsync(new Criteria(<%= _schema.GetUsingParameterKeys %>));
                        else
                        {
                            if (await ExistsAsync(<%= _schema.GetUsingParameterKeys %>))
                                ExceptionThower.BusinessRuleStop(ResStrConst.AlreadyExists(new <%= CLS %>() { <%= _schema.GetInitializeParameterKeys %> }));

                            return await DataPortal.CreateAsync&lt;<%= CLS %>>(new Criteria(<%= _schema.GetUsingParameterKeys %>));
                        }
                    }                
                </t>.Value)

        '----------------GetAsync

        ret.Add(<t>public static async Task&lt;<%= CLS %>> Get<%= CLS %>Async(<%= _schema.GetParameterKeys(False) %>) 
                    { 
                         if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return await APIDataPortalFetchAsync(new Criteria(<%= _schema.GetUsingParameterKeys %>));
                         else
                            return await DataPortal.FetchAsync&lt;<%= CLS %>>(new Criteria(<%= _schema.GetUsingParameterKeys %>));
                    }                
                </t>.Value)

        '-------------DeleteAsync

        ret.Add(<t>public static async Task&lt;string> Delete<%= CLS %>Async(<%= _schema.GetParameterKeys(False) %>)
                    {
                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                        return await APIDataPortalDeleteAsync(new Criteria(<%= _schema.GetUsingParameterKeys %>));
                        else
                        {
                        await DataPortal.DeleteAsync&lt;<%= CLS %>>(new Criteria(<%= _schema.GetUsingParameterKeys %>));
                        return <%= _schema.GetCombinedKeys(True) %>;
                        }
                    }       
                </t>.Value)

        '-------------ExistsAsync

        If _schema._subTableCode = "" Then

            ret.Add(<t>public static async Task&lt;bool> ExistsAsync(<%= _schema.GetParameterKeys(False) %>) 
                    { 
                        if (string.IsNullOrEmpty(<%= _schema.GetUsingParameterKeys %>)) return false;

                        string SqlText = $"SELECT COUNT(*) FROM <%= _schema._DBTable %> WHERE DTB='{Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>))}' AND <%= _schema.GetSQLParameterKeys %>";

                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return (await RESTDataPortal.GetScalarIntegerAsync(SqlText)) > 0;
                        else
                            return (await SPC.Database.SQLCommander.GetScalarIntegerAsync(SqlText)) > 0;
                    }                
                </t>.Value)

        ElseIf _schema._isSingleton Then
            ret.Add(<t>public static async Task&lt;bool> ExistsAsync(<%= _schema.GetParameterKeys(False) %>) 
                    { 
                        return true;
                    }                
                </t>.Value)
        Else
            ret.Add(<t>public static async Task&lt;bool> ExistsAsync(<%= _schema.GetParameterKeys(False) %>) 
                    { 
                        if (string.IsNullOrEmpty(<%= _schema.GetUsingParameterKeys %>)) return false;

                        string SqlText = $"SELECT COUNT(*) FROM <%= _schema._DBTable %> WHERE PBS_DB='{Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>))}' AND PBS_TB = '{PBS_TB}' AND KEY_FIELDS='{<%= _schema.GetUsingParameterKeys(True) %>}'";

                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return (await RESTDataPortal.GetScalarIntegerAsync(SqlText)) > 0;
                        else
                            return (await SPC.Database.SQLCommander.GetScalarIntegerAsync(SqlText)) > 0;
                    }                
                </t>.Value)

        End If

        '-------------Save

        If _schema._isSingleton OrElse (From k In _schema.Keys Where k._isAutoNumber.ToBoolean).Count > 0 Then
            ret.Add(<t> public async Task&lt;IEditable> SaveBOAsync()
                    {
                        if (!IsDirty) ExceptionThower.BusinessRuleStop(ResStrConst.NotDirty(this));
                        if (!IsSavable) ExceptionThower.BusinessRuleStop(ResStrConst.NotSavable(this));
            
                        this.ApplyEdit();

                        var ret = this;
                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            ret = await APIDataPortalPostAsync(this);
                        else
                            ret = await base.SaveAsync();

                        if (!Ctx.IsBatchSavingMode()) <%= CLS %>InfoList.InvalidateCache();

                        return ret as IEditable;
                    }

                </t>.Value)


        Else
            ret.Add(<t> public async Task&lt;IEditable> SaveBOAsync()
                    {
                        if (!IsDirty) ExceptionThower.BusinessRuleStop(ResStrConst.NotDirty(this));
                        if (!IsSavable) ExceptionThower.BusinessRuleStop(ResStrConst.NotSavable(this));

                        if (this.IsNew)
                            {
                                if (await ExistsAsync(<%= _schema.GetUsingParameterKeys(False) %>)) ExceptionThower.BusinessRuleStop(ResStrConst.AlreadyExists(this));
                                //Applying Numbering Rules and Auto Reference here
                            }
                        this.ApplyEdit();

                        var ret = this;
                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            ret = await APIDataPortalPostAsync(this);
                        else
                            ret = await base.SaveAsync();

                        if (!Ctx.IsBatchSavingMode()) <%= CLS %>InfoList.InvalidateCache();

                        return ret as IEditable;
                    }

                </t>.Value)
        End If



        '--------------Clone
        If _schema._isSingleton OrElse (From k In _schema.Keys Where k._isAutoNumber.ToBoolean).Count > 0 Then
            ret.Add(<t> public async Task&lt;<%= CLS %>> CopyBOAsync(<%= _schema.GetParameterKeys(False) %>)
                    {
                       
                        <%= CLS %> cloning = base.Clone();
   
                        cloning.DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));

                         <%= String.Join(Environment.NewLine, (From k In _schema.Keys Select k.AssigningKeysValueToClone).ToArray) %>
    
                        cloning.MarkNew();
                        cloning.BusinessRules.CheckRules();
                        cloning.ApplyEdit();
                        return cloning;
                    }

                </t>.Value)
        Else
            ret.Add(<t> public async Task&lt;<%= CLS %>> CopyBOAsync(<%= _schema.GetParameterKeys(False) %>)
                    {
                       if (await ExistsAsync(<%= _schema.GetUsingParameterKeys(True) %>))
                            ExceptionThower.BusinessRuleStop(string.Format(Voca.Translate(ResStrConst.CreateAlreadyExists), this.GetType().PhoebusCommandDesc()));

                        <%= CLS %> cloning = base.Clone();
   
                        cloning.DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));

                         <%= String.Join(Environment.NewLine, (From k In _schema.Keys Select k.AssigningKeysValueToClone).ToArray) %>
    
                        cloning.MarkNew();
                        cloning.BusinessRules.CheckRules();
                        cloning.ApplyEdit();
                        return cloning;
                    }

                </t>.Value)
        End If


        ret.Add("#endregion Factory Methods")
        ret.Add(vbNewLine)
        Return ret
    End Function

    Private Function GetCheckExists(Optional withP As Boolean = True) As String

        If _schema.Keys(0)._isAutoNumber.ToBoolean Then
            Return String.Empty
        ElseIf withP Then
            Return <t> if (await ExistsAsync(<%= _schema.GetUsingParameterKeys(withP) %>)) ExceptionThower.BusinessRuleStop(ResStrConst.AlreadyExists(new <%= _schema._className %>() { <%= _schema.GetInitializeParameterKeys %> })));</t>.Value
        Else
            Return <t> if (await ExistsAsync(<%= _schema.GetUsingParameterKeys(withP) %>)) ExceptionThower.BusinessRuleStop(ResStrConst.AlreadyExists(this));</t>.Value
        End If

    End Function

    Friend Function Get_Readonly_FactorySection() As List(Of String)
        Dim ret = New List(Of String)

        Dim CLS = _schema._className

        ret.Add("#region Factory Methods")
        ret.Add(vbNewLine)

        ret.Add(<t>public <%= CLS %>Info() {}</t>.Value)
        ret.Add(vbNewLine)

        '---------------EmptyLRQInfo
        ret.Add(<t>internal static <%= CLS %>Info Empty<%= CLS %>Info(<%= _schema.GetParameterKeys(True) %>) </t>.Value)
        ret.Add("{")
        ret.Add(<t><%= CLS %>Info info = new <%= CLS %>Info();</t>.Value)

        For Each key In _schema.Keys
            ret.Add($"info.{key.Property_Load411(True)};")
        Next

        ret.Add("return info;")

        ret.Add("}")

        '----------------GetInfo using datareader
        ret.Add(<t>internal static <%= _schema._className %>Info Get<%= _schema._className %>Info(SafeDataReader dr) 
                    { return new <%= _schema._className %>Info(dr); }                
                </t>.Value)


        '-------------API GETInfo using api

        ret.Add(<tx> internal static <%= CLS %>Info Get<%= CLS %>Info(Dictionary&lt;string, object> pDic)
                            {
                                var ret = new <%= CLS %>Info();
                                Csla.Data.DataMapper.Map(pDic, ret, true);
                                //Set Properies not in REST API
                                //ret.Description = pDic.GetObjectByKey("Descriptn", string.Empty).ToString();
                                return ret;
                            }
                </tx>.Value)



        ret.Add("#endregion Factory Methods")
        ret.Add(vbNewLine)

        Return ret
    End Function

#End Region

#Region "Data Portal"

    Friend Function Get_DataPortal_RFMSC411(pReadOnly As Boolean) As List(Of String)
        Dim CLS = _schema._className

        Dim ret = New List(Of String)

        ret.AddRange(GetCriteriaClass)

        ret.Add("#region RESTAPI")

        If _schema._isSingleton Then
            ret.Add(<tx>
                       private static async Task&lt;<%= CLS %>> APIDataPortalCreateAsync(Criteria criteria)
                        {
                            var ret = new <%= CLS %>();

                            var reader = await RESTDataPortal.CreateAsync(APICommand);

                            if (reader != null &amp;&amp; reader.Data != null) ret = <%= CLS %>.Fetch(reader.Data);
                            ret.MarkNew();
                            return ret;
                        }
                </tx>.Value)
        Else
            ret.Add(<tx>
                       private static async Task&lt;<%= CLS %>> APIDataPortalCreateAsync(Criteria criteria)
                        {
                            var ret = new <%= CLS %>();

                            var reader = await RESTDataPortal.CreateAsync(APICommand , criteria.GetFilter());

                            if (reader != null &amp;&amp; reader.Data != null) ret = <%= CLS %>.Fetch(reader.Data);
                            ret.MarkNew();
                            return ret;
                        }
                </tx>.Value)
        End If

        If _schema._isSingleton Then
            ret.Add(<tx>private static async Task&lt;<%= CLS %>> APIDataPortalFetchAsync(Criteria criteria)
                          {
                               var reader = await RESTDataPortal.FetchAsync&lt;SPC.API.APIBOReader>(APICommand);

                                if (reader != null &amp;&amp; reader.Data != null)
                                {
                                    var ret = <%= CLS %>.Fetch(reader.Data);
                                    ret.MarkOld();
                                    return ret;
                                }

                                return null;
                          }
                </tx>.Value)
        Else
            ret.Add(<tx>private static async Task&lt;<%= CLS %>> APIDataPortalFetchAsync(Criteria criteria)
                          {
                               var reader = await RESTDataPortal.FetchAsync&lt;SPC.API.APIBOReader>(APICommand, criteria.GetFilter());

                                if (reader != null &amp;&amp; reader.Data != null)
                                {
                                    var ret = <%= CLS %>.Fetch(reader.Data);
                                    ret.MarkOld();
                                    return ret;
                                }

                                return null;
                          }
                </tx>.Value)
        End If



        ret.Add(<tx> private static <%= CLS %> Fetch(Dictionary&lt;string, object> pDic)
                            {
                                var ret = new <%= CLS %>();
                                Csla.Data.DataMapper.Map(pDic, ret, true);
                                //Set Properies not in REST API
                                //ret.Description = pDic.GetObjectByKey("Descriptn", string.Empty).ToString();
                                return ret;
                            }
                </tx>.Value)

        ret.Add(<tx> private static async Task&lt;<%= CLS %>> APIDataPortalPostAsync(Csla.Core.BusinessBase pBO)
                            {
                                var reader = await RESTDataPortal.SaveAsync(APICommand, pBO);

                                if (reader != null &amp;&amp; reader.Data != null)
                                {
                                    var ret = <%= CLS %>.Fetch(reader.Data);
                                    ret.MarkOld();
                                    
                                    SPC.Services.UI.AlertService.Alert(reader.Message);

                                    return ret;
                                }
                                return pBO as <%= CLS %>;
                            }
                </tx>.Value)

        ret.Add(<tx> private static async Task&lt;string> APIDataPortalDeleteAsync(Criteria criteria)
                        {
                            var reader = await RESTDataPortal.DeleteAsync(APICommand, criteria.GetFilter());

                            if (reader != null )
                            {
                                SPC.Services.UI.AlertService.Toast(reader.Message);
                                return criteria.GetFilter().ToParametersString();
                            }
                            return string.Empty;

                        }
                </tx>.Value)

        ret.Add("#endregion RESTAPI")
        ret.Add(vbNewLine)
        ret.Add(vbNewLine)
        ret.Add("#region Data Access ")

        ret.Add(<tx>
                    #region Create

                    [RunLocal()]
                    private void DataPortal_Create(Criteria criteria)
                    {
                        using (BypassPropertyChecks)
                        {
                          <%= String.Join(Environment.NewLine, (From k In _schema.Keys Select k.AssigningKeysValueFromCriteria).ToArray) %>
                        }
                        BusinessRules.CheckRules();
                    }
                    #endregion
                </tx>.Value) 'Create

        ret.Add("#region Fetch")

        Dim theKeyProp = _schema.Keys(0)._id.toPropertyName

        ret.Add(<tx>
                    private void DataPortal_Fetch(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            ExecuteFetch(cn, criteria);
                        }
                    }

                    private void ExecuteFetch(IDbConnection cn, Criteria criteria)
                    {
                        using (var cm = cn.Get_RFMSCFetchCommand(DTB, PBS_TB, criteria.<%= theKeyProp %>))
                        {

                            using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
                            {
                                FetchObject(dr);
                            }

                        }
                    }

                    private void FetchObject(SafeDataReader dr)
                    {
                       if(dr.Read())
                        {
                       //missing extended
                       using (BypassPropertyChecks)
                       {

                            LoadProperty(<%= theKeyProp %>Property, dr.GetString("KEY_FIELDS").TrimEnd());

                             var _data = dr.GetString("PSM_DATA").TrimEnd();

                                if (!string.IsNullOrEmpty(_data))
                                {
                                    try
                                    {
                                        var xele = XElement.Parse(_data.Decompress());
                            
                                        <%= String.Join(Environment.NewLine, (From k In _schema.Members Select k.GetFetchPropertyRFMSC411).ToArray) %> 

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                                                  
                             LoadPropertyConvert&lt;SmartDate, string>(UpdatedProperty, dr.GetString("UPDATED"));
                        }
                      }
                    }        

         </tx>.Value)
        ret.Add("#endregion Fetch")

        ret.Add(vbNewLine)

        ret.Add("#region Insert Update")

        ret.Add(<tx>
         protected override void DataPortal_Insert()
        {
            using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
            {
                ExecuteInsert(cn);

                // update child object(s)

            }
        }

        private void ExecuteInsert(IDbConnection cn)
        {
            using (var cm = cn.Get_pbs_RFMSC_InsertUpdateCommand())
            {
                AddInsertParameters(cm);
                cm.ExecuteNonQuery();

            }
        }

        private void AddInsertParameters(IDbCommand cm)
        {
            var _data = new XElement("d");

            <%= String.Join(Environment.NewLine, (From k In _schema.Members Select k.InsertUpdateToDBRFMSC).ToArray) %>
                    
            cm.Parameters.AddWithValue("@PBS_DB", DTB);
            cm.Parameters.AddWithValue("@PBS_TB", PBS_TB);
            cm.Parameters.AddWithValue("@KEY_FIELDS", <%= theKeyProp %>.Trim());
            cm.Parameters.AddWithValue("@LOOKUP", String.Empty);
            cm.Parameters.AddWithValue("@UPDATED", DateTime.Today.ToSunDate());
            cm.Parameters.AddWithValue("@PSM_DATA", _data.ToString().Compress());
        }

        protected override void DataPortal_Update()
        {
               DataPortal_Insert();

        }  
         </tx>.Value)

        ret.Add("#endregion Insert Update")
        ret.Add(vbNewLine)

        ret.Add("#region Delete ")

        ret.Add(<t>private void DataPortal_Delete(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            using (var cm = cn.Get_RFMSC_DeleteCommand(DTB, PBS_TB, criteria.<%= theKeyProp %>))
                            {
                                cm.ExecuteNonQuery();
                            }

                        }
                    }
                </t>.Value)

        ret.Add("#endregion Delete ")

        ret.Add(vbNewLine)

        ret.Add("#endregion Data Access ")
        ret.Add(vbNewLine)
        Return ret
    End Function

    Friend Function Get_DataPortal_411(pReadOnly As Boolean) As List(Of String)
        Dim CLS = _schema._className

        Dim ret = New List(Of String)

        ret.AddRange(GetCriteriaClass)

        ret.Add("#region RESTAPI")

        ret.Add(<tx>
                       private static async Task&lt;<%= CLS %>> APIDataPortalCreateAsync(Criteria criteria)
                        {
                            var ret = new <%= CLS %>();

                            var reader = await RESTDataPortal.CreateAsync(APICommand , criteria.GetFilter());

                            if (reader != null &amp;&amp; reader.Data != null) ret = <%= CLS %>.Fetch(reader.Data);
                            ret.MarkNew();
                            return ret;
                        }
                </tx>.Value)

        ret.Add(<tx>private static async Task&lt;<%= CLS %>> APIDataPortalFetchAsync(Criteria criteria)
                          {
                               var reader = await RESTDataPortal.FetchAsync&lt;SPC.API.APIBOReader>(APICommand, criteria.GetFilter());

                                if (reader != null &amp;&amp; reader.Data != null)
                                {
                                    var ret = <%= CLS %>.Fetch(reader.Data);
                                    ret.MarkOld();
                                    return ret;
                                }

                                return null;
                          }
                </tx>.Value)

        ret.Add(<tx> private static <%= CLS %> Fetch(Dictionary&lt;string, object> pDic)
                            {
                                var ret = new <%= CLS %>();
                                Csla.Data.DataMapper.Map(pDic, ret, true);
                                //Set Properies not in REST API
                                //ret.Description = pDic.GetObjectByKey("Descriptn", string.Empty).ToString();
                                return ret;
                            }
                </tx>.Value)

        ret.Add(<tx> private static async Task&lt;<%= CLS %>> APIDataPortalPostAsync(Csla.Core.BusinessBase pBO)
                            {
                                var reader = await RESTDataPortal.SaveAsync(APICommand, pBO);

                                if (reader != null &amp;&amp; reader.Data != null)
                                {
                                    var ret = <%= CLS %>.Fetch(reader.Data);
                                    ret.MarkOld();
                                    
                                    SPC.Services.UI.AlertService.Alert(reader.Message);

                                    return ret;
                                }
                                return pBO as <%= CLS %>;
                            }
                </tx>.Value)

        ret.Add(<tx> private static async Task&lt;string> APIDataPortalDeleteAsync(Criteria criteria)
                        {
                            var reader = await RESTDataPortal.DeleteAsync(APICommand, criteria.GetFilter());

                            if (reader != null )
                            {
                                SPC.Services.UI.AlertService.Alert(reader.Message);
                                return criteria.GetFilter().ToParametersString();
                            }
                            return string.Empty;

                        }
                </tx>.Value)

        ret.Add("#endregion RESTAPI")
        ret.Add(vbNewLine)
        ret.Add("#region Data Access ")

        ret.Add(<tx>
                    #region Create

                    [RunLocal()]
                    private void DataPortal_Create(Criteria criteria)
                    {
                        using (BypassPropertyChecks)
                        {
                          <%= String.Join(Environment.NewLine, (From k In _schema.Keys Select k.AssigningKeysValueFromCriteria).ToArray) %>
                        }
                        BusinessRules.CheckRules();
                    }
        
                    #endregion

                </tx>.Value) 'Create

        ret.Add("#region Fetch")

        ret.Add(<tx>
                    private void DataPortal_Fetch(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            ExecuteFetch(cn, criteria);
                        }
                    }

                    private void ExecuteFetch(IDbConnection cn, Criteria criteria)
                    {
                        using (var cm = cn.CreateSQLCommand())
                        {
                            cm.SetDBCommand(CommandType.Text, $"SELECT * FROM <%= _schema._DBTable %> WHERE DTB='{DTB}' AND <%= _schema.GetSQLParameterKeys(True) %>");

                            using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
                            {
                                FetchObject(dr);
                            }

                        }
                    }

                    private void FetchObject(SafeDataReader dr)
                    {
                       if(dr.Read())
                            {
                               //missing extended
                               using (BypassPropertyChecks)
                               {
                                 <%= String.Join(Environment.NewLine, (From k In _schema.Members Select k.GetFetchProperty411).ToArray) %>
                               }
                            }
                     

                    }        

         </tx>.Value)
        ret.Add("#endregion Fetch")
        ret.Add(vbNewLine)
        ret.Add("#region Insert Update")

        ret.Add(<tx>
         protected override void DataPortal_Insert()
        {
            using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
            {
                ExecuteInsert(cn);

                // update child object(s)

            }
        }

        private void ExecuteInsert(IDbConnection cn)
        {
            using (var cm = cn.CreateSQLCommand())
            {
                cm.SetDBCommand(CommandType.StoredProcedure, $"<%= _schema._DBTable %>_{DTB}_Insert");
                <%= _schema.SetOuputSQLParameter %>

                AddInsertParameters(cm);
                cm.ExecuteNonQuery();

                <%= _schema.GetOuputSQLParameter %>
            }
        }

        private void AddInsertParameters(IDbCommand cm)
        {
             <%= String.Join(Environment.NewLine, (From k In _schema.Members Select k.InsertUpdateToDB).ToArray) %>
        }

        protected override void DataPortal_Update()
        {
                using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                    {
                        using (var cm = cn.CreateSQLCommand())
                        {
                            cm.SetDBCommand(CommandType.StoredProcedure, $"<%= _schema._DBTable %>_{DTB}_Update");

                            <%= _schema.SetOuputSQLParameter(False) %>

                            AddInsertParameters(cm);
                            cm.ExecuteNonQuery();

                        }

                        // update child object(s)

            }

        }  
         </tx>.Value)

        ret.Add("#endregion Insert Update")
        ret.Add(vbNewLine)
        ret.Add("#region Delete ")

        ret.Add(<t>private void DataPortal_Delete(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            using (var cm = cn.CreateSQLCommand())
                            {
                                cm.CommandText = $"DELETE FROM <%= _schema._DBTable %>{DTB} WHERE <%= _schema.GetSQLParameterKeys(True) %> {Environment.NewLine}";
                    
                                //cm.CommandText += $"DELETE FROM <%= _schema._DBTable %>{DTB} WHERE HEADER_NO= {criteria.LineNo}";

                                cm.ExecuteNonQuery();

                            }

                        }
                    }
                </t>.Value)

        ret.Add("#endregion Delete ")
        ret.Add(vbNewLine)
        ret.Add("#endregion Data Access ")
        ret.Add(vbNewLine)
        Return ret
    End Function

    Private Function GetCriteriaClass() As List(Of String)
        Dim ret = New List(Of String)
        ret.Add("[Serializable]")
        ret.Add("private class Criteria: CriteriaBase<Criteria>")
        ret.Add("{")

        For Each finfo In _schema.Keys

            If finfo._id <> "DTB" AndAlso finfo._id <> "SITE" Then
                ret.AddRange(finfo.GetOneProperty_Criteria)
                ret.Add(vbNewLine)
            End If

        Next

        ret.Add($"internal Criteria({_schema.GetParameterKeys(False)})")
        ret.Add("{")

        For Each f In _schema.Keys
            ret.Add($"{f._id.toPropertyName} = p{f._id.toPropertyName};")
        Next

        ret.Add("}")

        ret.Add(<t>
                     internal Dictionary&lt;string, string> GetFilter()
                     {
                            var pFilters = new Dictionary&lt;string, string>(StringComparer.OrdinalIgnoreCase);
                           <%= String.Join(Environment.NewLine, (From k In _schema.Keys Select k.AddingKeytoDictionary).ToArray) %>
                            return pFilters;
                      }
                    </t>.Value)

        ret.Add("}")

        ret.Add(vbNewLine)


        Return ret
    End Function

    'Private Function GetCriteriaClass() As List(Of String)
    '    Dim ret = New List(Of String)
    '    ret.Add("[Serializable]")
    '    ret.Add("private class Criteria: CORE.Criteria")
    '    ret.Add("{")

    '    For Each f In _schema.Keys
    '        ret.Add(f.GetPropertyDeclaringCriteria411(False))
    '    Next

    '    ret.Add($"internal Criteria({_schema.GetParameterKeys(False)})")
    '    ret.Add("{")

    '    For Each f In _schema.Keys
    '        ret.Add($"{f._id.toPropertyName} = p{f._id.toPropertyName};")
    '    Next

    '    ret.Add("}")

    '    ret.Add(<t>
    '             internal Dictionary&lt;string, string> GetFilter()
    '             {
    '                    var pFilters = new Dictionary&lt;string, string>(StringComparer.OrdinalIgnoreCase);
    '                   <%= String.Join(Environment.NewLine, (From k In _schema.Keys Select k.AddingKeytoDictionary).ToArray) %>
    '                    return pFilters;
    '              }
    '            </t>.Value)

    '    ret.Add("}")

    '    ret.Add(vbNewLine)
    '    Return ret
    'End Function

    Friend Function Get_Fetch_SafeDataReader411(pReadOnly As Boolean) As List(Of String)

        Dim ret = New List(Of String)

        ret.Add($" private {_schema._className}{If(pReadOnly, "Info", String.Empty)}(SafeDataReader dr)")

        ret.Add("{")

        If Not pReadOnly Then
            ret.Add("using (BypassPropertyChecks)")
            ret.Add("{")
        End If

        If _schema._subTableCode = "" Then
            For Each finfo In _schema.Members
                ret.Add(finfo.GetFetchProperty411)
            Next
        Else

            Dim theKeyProp = _schema.Keys(0)._id.toPropertyName

            Dim text = <text>
                LoadProperty(<%= theKeyProp %>Property, dr.GetString("KEY_FIELDS").TrimEnd());

                var _data = dr.GetString("PSM_DATA").TrimEnd();

                   if (!string.IsNullOrEmpty(_data)) 
                                {
                                    try
                                    {
                                        var xele = XElement.Parse(_data.Decompress());
                            
                                        <%= String.Join(Environment.NewLine, (From k In _schema.Members Select k.GetFetchPropertyRFMSC411).ToArray) %> 

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                                                  
                LoadPropertyConvert&lt;SmartDate, String > (UpdatedProperty, dr.GetString("UPDATED"));
                       </text>.Value

            ret.Add(text)

        End If

        If Not pReadOnly Then ret.Add("}")

        ret.Add("}")

        Return ret

    End Function



#End Region


End Class
