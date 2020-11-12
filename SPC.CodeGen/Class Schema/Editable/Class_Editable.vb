Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Public Function GenerateEditableCode() As String
            Try

                If IsSingleton Then

                    Return GenerateEditableSingletonCode()

                ElseIf DBTableName = "pbs_RFMSC" Then

                    Return GenerateEditableRFMSCCode()

                Else

                    Dim sections = New List(Of String)

                    sections.Add(BuildEditableProperties)

                    sections.Add(BuildEditableDoclink)

                    sections.Add(BuildEditableRules)

                    sections.Add(BuildEditableCriteria)

                    sections.Add(BuildEditableFactories)

                    sections.Add(BuildEditableRESTAPI)

                    sections.Add(BuildEditableDataAccess)

                    Dim scriptCode = GenerateScriptsCode()
                    If Not String.IsNullOrEmpty(scriptCode) Then sections.Add(scriptCode)

                    If Tables.Count > 0 Then sections.Add(BuildChildTables())

                    Dim theCode = String.Join(Environment.NewLine, sections.ToArray)

                    theCode = WrappWithEditableClassName(theCode)

                    theCode = WrappWithNameSpace(theCode)

                    theCode = theCode.Insert(0, Environment.NewLine)

                    theCode = theCode.Insert(0, BuildEditableUsings)

                    Return theCode

                End If
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

#Region "Business Properties"

        Private Function BuildEditableProperties() As String
            Dim sections = New List(Of String)

            sections.Add(<p>public const string APICommand = "<%= $"{Me.ClassNameSpace}.{ClassName}".Replace("SPC.", "pbs_").ToLower.Replace(".", "_") %>";</p>.Value)

            If DBTableName = "pbs_RFMSC" Then
                If SubTable = "PRT" Then
                    sections.Add(<p> public const string PBS_TB = RFMSC_TB.Repository;</p>.Value)
                Else
                    sections.Add(<p> public const string PBS_TB = "<%= SubTable %>" ;//RFMSC_TB.PurchaseOrderType;</p>.Value)
                End If
            ElseIf DBTableName = "SSRFMSC" Then
                sections.Add(<p> public const string SUN_TB = "<%= SubTable %>" ;//RFMSC_TB.;</p>.Value)
            End If
            'DTB
            sections.Add(<text>

        public static readonly PropertyInfo&lt;string> DTBProperty = RegisterProperty&lt;string>(c => c.DTB, String.Empty);
        private string DTB
        {
            get { return ReadProperty(DTBProperty); }
            set { LoadProperty(DTBProperty, value); }
        }
                    </text>.Value.TrimStart)

            'SingleFields
            For Each prop In Fields
                If Not prop.IsChildCollection Then sections.Add(prop.BuildProperty)
            Next

            sections.Add(BuildToString)

            Dim ret = String.Join(Environment.NewLine, sections.ToArray)

            ret = WrappWithRegion(ret, "Business Properties")

            Return ret
        End Function

        Private Function BuildToString() As String
            If IsSingleton Then

                Return <code>
            public override string ToString()
            {
                return STR_<%= ClassName.Leaf %>;
            }
                    </code>.Value

            Else

                Return <code>
            public override string ToString()
            {
                return <%= GetCombinedKeyValues() %>;
            }
                    </code>.Value
            End If

        End Function

#End Region

#Region "Doclink"

        Private Function BuildEditableDoclink() As String

            If IsSingleton Then Return String.Empty

            If Keys() Is Nothing OrElse Keys.Count = 0 Then Return String.Empty

            Dim ret As New List(Of String)

            ret.Add(<t> string IDocLink.GetDocLinkRef() { return String.Format("{0}#{1}", (this as IDocLink).GetDocType(), <%= Keys(0).FieldName %>); }</t>.Value)

            ret.Add(<t> string IDocLink.GetDocType() { return this.GetType().ToEditableClassName().Leaf(); }</t>.Value)


            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "IDoclink")

            Return retStr

        End Function

#End Region

#Region "Rules"

        Private Function BuildEditableRules() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

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
        //        }
        //    }
     </t>.Value)

            '----------------AddSharedCommonRules
            ret.Add(<t>private void AddSharedCommonRules()
        {
            // Sample simple custom rule

           // BusinessRules.AddRule(new URLMappingRule(InfoCodeProperty));
        }       </t>.Value)

            '----------------NewAsync
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
            }</t>.Value)


            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Business Rules")

            Return retStr
        End Function

#End Region

#Region "Factories"

        Private Function BuildEditableFactories() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            ret.Add(<t>public <%= CLS %>() {  DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>)); }</t>.Value)

            '----------------NewAsync
            ret.Add(vbNewLine)
            ret.Add(<t>public static async Task&lt;<%= CLS %>> New<%= CLS %>Async(<%= GetParameterKeys(False) %>) 
                    { 
                        if (Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return await APIDataPortalCreateAsync(new Criteria(<%= GetUsingParameterKeys() %>));
                        else
                        {
                            if (await ExistsAsync(<%= GetUsingParameterKeys() %>))
                                ExceptionThower.BusinessRuleStop(ResStrConst.AlreadyExists(new <%= CLS %>() { <%= GetInitializeParameterKeys() %> }));

                            return await DataPortal.CreateAsync&lt;<%= CLS %>>(new Criteria(<%= GetUsingParameterKeys() %>));
                        }
                    }                
                </t>.Value)

            '----------------GetAsync

            ret.Add(<t>public static async Task&lt;<%= CLS %>> Get<%= CLS %>Async(<%= GetParameterKeys(False) %>) 
                    { 
                         if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return await APIDataPortalFetchAsync(new Criteria(<%= GetUsingParameterKeys() %>));
                         else
                            return await DataPortal.FetchAsync&lt;<%= CLS %>>(new Criteria(<%= GetUsingParameterKeys() %>));
                    }                
                </t>.Value)

            '-------------DeleteAsync


            ret.Add(<t>public static async Task&lt;string> Delete<%= CLS %>Async(<%= GetParameterKeys(False) %>)
                    {
                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                        return await APIDataPortalDeleteAsync(new Criteria(<%= GetUsingParameterKeys() %>));
                        else
                        {
                        await DataPortal.DeleteAsync&lt;<%= CLS %>>(new Criteria(<%= GetUsingParameterKeys() %>));
                        return <%= GetCombinedKeys() %>  ;
                        }
                    }       
                </t>.Value)

            '-------------ExistsAsync

            If IsSingleton Then

                ret.Add(SingletonExistAsync)

            ElseIf DBTableName = "pbs_RFMSC" Then

                ret.Add(RFMSCExistAsync)

            ElseIf DBTableName = "SSRFMSC" Then

                ret.Add(SSRFMSCExistAsync)

            ElseIf SubTable = "" Then

                ret.Add(<t>public static async Task&lt;bool> ExistsAsync(<%= GetParameterKeys(False) %>) 
                    { 
                        if (<%= GetCheckNullParameters() %>) return false;

                        string SqlText = $"SELECT COUNT(*) FROM <%= DBTableName %> WHERE DTB='{Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>))}' AND <%= GetSQLParameterKeys() %>";

                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return (await RESTDataPortal.GetScalarIntegerAsync(SqlText)) > 0;
                        else
                            return (await SPC.Database.SQLCommander.GetScalarIntegerAsync(SqlText)) > 0;
                    }                
                </t>.Value)

            Else
                ret.Add(<t>public static async Task&lt;bool> ExistsAsync(<%= GetParameterKeys(False) %>) 
                    { 
                        if (<%= GetCheckNullParameters() %>) return false;

                        string SqlText = $"SELECT COUNT(*) FROM <%= DBTableName %> WHERE PBS_DB='{Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>))}' AND PBS_TB = '{PBS_TB}' AND KEY_FIELDS='{<%= GetUsingParameterKeys(True) %>}'";

                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return (await RESTDataPortal.GetScalarIntegerAsync(SqlText)) > 0;
                        else
                            return (await SPC.Database.SQLCommander.GetScalarIntegerAsync(SqlText)) > 0;
                    }                
                </t>.Value)

            End If

            '-------------Save

            If IsSingleton Then

                ret.Add(SaveSingletonAsync)

            ElseIf (From k In Keys() Where k.IsAutoGenerated).Count > 0 Then

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
                                if (await ExistsAsync(<%= GetUsingParameterKeys(False) %>)) ExceptionThower.BusinessRuleStop(ResStrConst.AlreadyExists(this));
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
            If IsSingleton Then
                ret.Add(CloneSingletonAsync)

            ElseIf (From k In Keys() Where k.IsAutoGenerated).Count > 0 Then
                ret.Add(<t> public async Task&lt;<%= CLS %>> CopyBOAsync(<%= GetParameterKeys(False) %>)
                    {
                       
                        <%= CLS %> cloning = base.Clone();
   
                        cloning.DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));

                         <%= String.Join(Environment.NewLine, (From k In Keys() Select k.AssigningKeysValueToClone).ToArray) %>
    
                        cloning.MarkNew();
                        cloning.BusinessRules.CheckRules();
                        cloning.ApplyEdit();
                        return cloning;
                    }

                </t>.Value)
            Else
                ret.Add(<t> public async Task&lt;<%= CLS %>> CopyBOAsync(<%= GetParameterKeys(False) %>)
                    {
                       if (await ExistsAsync(<%= GetUsingParameterKeys(True) %>))
                            ExceptionThower.BusinessRuleStop(string.Format(Voca.Translate(ResStrConst.CreateAlreadyExists), this.GetType().PhoebusCommandDesc()));

                        <%= CLS %> cloning = base.Clone();
   
                        cloning.DTB = Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= CLS %>));

                         <%= String.Join(Environment.NewLine, (From k In Keys() Select k.AssigningKeysValueToClone).ToArray) %>
    
                        cloning.MarkNew();
                        cloning.BusinessRules.CheckRules();
                        cloning.ApplyEdit();
                        return cloning;
                    }

                </t>.Value)
            End If


            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Factory Methods")

            Return retStr

        End Function

#End Region

#Region "API"

        Private Function BuildEditableRESTAPI() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName


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

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "RESTAPI")

            Return retStr


        End Function

#End Region

#Region "DataAccess"

        Private Function BuildEditableCriteria() As String
            Dim ret = New List(Of String)

            ret.Add("[Serializable]")
            ret.Add("private class Criteria: CriteriaBase<Criteria>")
            ret.Add("{")

            For Each finfo In Keys()

                If finfo.FieldName <> "DTB" AndAlso finfo.DatabaseFieldName <> "SITE" Then
                    ret.AddRange(finfo.BuildCriteriaProperty)
                    ret.Add(vbNewLine)
                End If

            Next

            ret.Add($"internal Criteria({GetParameterKeys(False)})")
            ret.Add("{")

            For Each f In Keys()
                ret.Add($"{f.FieldName} = p{f.FieldName};")
            Next

            ret.Add("}")

            ret.Add(<t>
                     internal Dictionary&lt;string, string> GetFilter()
                     {
                            var pFilters = new Dictionary&lt;string, string>(StringComparer.OrdinalIgnoreCase);
                           <%= String.Join(Environment.NewLine, (From k In Keys() Select k.AddingKeytoDictionary).ToArray) %>
                            return pFilters;
                      }
                    </t>.Value)

            ret.Add(BuildToString())

            ret.Add("}")


            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Criteria")

            Return retStr


        End Function

        Private Function BuildEditableDataAccess() As String
            Dim CLS = ClassName

            Dim ret = New List(Of String)

            ret.Add(<tx>
                    #region Create

                    [RunLocal()]
                    private void DataPortal_Create(Criteria criteria)
                    {
                        using (BypassPropertyChecks)
                        {
                          <%= String.Join(Environment.NewLine, (From k In Keys() Select k.AssigningKeysValueFromCriteria).ToArray) %>

                        <%= String.Join(Environment.NewLine, (From k In Fields Where Not k.IsPrimaryKey Select k.AssigningValueFromDefaultValue).ToArray) %>

                        }
                        BusinessRules.CheckRules();
                    }
        
                    #endregion

                </tx>.Value) 'Create

            ret.Add("#region Fetch")

            If DBTableName = "pbs_RFMSC" Then
                ret.Add(RFMSCFetching)
            ElseIf DBTableName = "SSRFMSC" Then
                ret.Add(SSRFMSCFetching)
            Else
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
                            cm.SetDBCommand(CommandType.Text, $"SELECT * FROM <%= DBTableName %> WHERE DTB='{DTB}' AND <%= GetSQLParameterKeys(True) %>").Replace("{XXX}", DTB).Replace("XXX", DTB);

                            using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
                            {
                                FetchObject(dr);
                            }

                        }
                       <%= FetchChildTable() %>

                    }

                    private void FetchObject(SafeDataReader dr)
                    {
                       if(dr.Read())
                            {
                               //missing extended
                               using (BypassPropertyChecks)
                               {
                                 <%= String.Join(Environment.NewLine, (From k In SingleFields() Select k.GetFetchProperty411).ToArray) %>
                               }
                            }
                     }        
         </tx>.Value)
            End If

            ret.Add("#endregion Fetch")




            ret.Add(vbNewLine)



            ret.Add("#region Insert Update")

            If DBTableName = "pbs_RFMSC" Then
                ret.Add(RFMSCInsertUpdate())
            ElseIf DBTableName = "SSRFMSC" Then
                ret.Add(SSRFMSCInsertUpdate())
            Else

                ret.Add(<tx>
         protected override void DataPortal_Insert()
        {
            using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
            {
                ExecuteInsert(cn);
                            
              <%= InsertUpdateChildTable() %>

            }
        }

        private void ExecuteInsert(IDbConnection cn)
        {
            using (var cm = cn.CreateSQLCommand())
            {
                cm.SetDBCommand(CommandType.StoredProcedure, "<%= If(Not String.IsNullOrEmpty(InsertSP), InsertSP, $"{DBTableName}_{DTB}_Insert") %>".Replace("{XXX}", DTB).Replace("XXX", DTB));
                <%= SetOuputSQLParameter() %>

                AddInsertParameters(cm);
                cm.ExecuteNonQuery();

                <%= GetOuputSQLParameter() %>
            }
        }

        private void AddInsertParameters(IDbCommand cm)
        {
             <%= String.Join(Environment.NewLine, (From k In SingleFields() Select k.InsertUpdateToDB).ToArray) %>
        }

        protected override void DataPortal_Update()
        {
                using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                    {
                        using (var cm = cn.CreateSQLCommand())
                        {
                            
                            cm.SetDBCommand(CommandType.StoredProcedure, "<%= If(Not String.IsNullOrEmpty(UpdateSP), UpdateSP, $"{DBTableName}_{DTB}_Update") %>".Replace("{XXX}", DTB).Replace("XXX", DTB));

                            <%= SetOuputSQLParameter(False) %>

                            AddInsertParameters(cm);
                            cm.ExecuteNonQuery();

                        }

                    <%= InsertUpdateChildTable() %>

                    }

        }  
         </tx>.Value)
            End If

            ret.Add("#endregion Insert Update")
            ret.Add(vbNewLine)
            ret.Add("#region Delete ")

            If DBTableName = "pbs_RFMSC" Then
                ret.Add(RFMSCDelete)
            ElseIf DBTableName = "SSRFMSC" Then
                ret.Add(SSRFMSCDelete)
            Else

                ret.Add(<t>private void DataPortal_Delete(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            using (var cm = cn.CreateSQLCommand())
                            {
                                cm.CommandText = $"DELETE FROM <%= DBTableName %> WHERE <%= GetSQLParameterKeys(True) %> {Environment.NewLine}".Replace("{XXX}", DTB).Replace("XXX", DTB);
                    
                               <%= DeleteChildTableScript() %>
                                
                                cm.ExecuteNonQuery();

                            }

                        }
                    }
                </t>.Value)
            End If

            ret.Add("#endregion Delete ")
            ret.Add(vbNewLine)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Data Access")

            Return retStr

        End Function

#End Region

#Region "Class Header"

        Private Function BuildEditableUsings() As String
            Return $"
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
using SPC.Services;
using SPC.Helper.Exceptions;       
using SmartDate = SPC.SmartData.SmartDate;
using SPC;
using SPC.Data;
using SPC.CORE.Annotation;
using System.Xml.Linq;

"

        End Function

        Private Function WrappWithNameSpace(pCode As String) As String

            Return <code>namespace  <%= Me.ClassNameSpace %>
                        {
                          <%= pCode %>
                        }

                   </code>.Value.Trim



        End Function

        Private Function WrappWithEditableClassName(pCode As String) As String

            Return <code>
    [Serializable]
    [PhoebusCommand(Desc = "<%= Description %>")]
    [DB(TableName = "<%= DBTableName %><%= If(Not String.IsNullOrEmpty(SubTable), $"/{SubTable}", "") %>")]
    public class <%= ClassName %> : Csla.BusinessBase&lt; <%= ClassName %>>, IEditable <%= If(IsSingleton, ", ISingleton", ", IDocLink") %><%= If(Scripts.Count > 0, ", ISupportScripts", "") %>
    {    
        <%= pCode %>
    
    }

                   </code>.Value.Trim



        End Function

        Private Shared Function WrappWithRegion(pCode As String, pRegionName As String) As String

            Return <region>
                      #region <%= pRegionName %><%= Environment.NewLine %><%= Environment.NewLine %>

                       <%= pCode %>

                       #endregion <%= pRegionName %>
                   </region>

        End Function
#End Region

#Region "Children List"
        Private Function BuildChildTables(Optional NoIsValidOrDirty As Boolean = False) As String
            Dim ret = New List(Of String)

            For Each kid In Tables()
                ret.Add(kid.BuildChildProperty())
            Next

            If NoIsValidOrDirty Then

            Else
                Dim ValidClause = String.Join(" && ", From k In Tables() Select $"{k.FieldName}.IsValid")
                ret.Add(<isValid>[System.ComponentModel.Browsable(false)]
        public override bool IsValid => base.IsValid &amp;&amp; <%= ValidClause %>;</isValid>.Value)

                Dim DirtyClause = String.Join(" || ", From k In Tables() Select $"{k.FieldName}.IsDirty")
                ret.Add(<isDirty>[System.ComponentModel.Browsable(false)]
        public override bool IsDirty => base.IsDirty || <%= DirtyClause %>;</isDirty>.Value)

            End If
            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Childrens")

            Return retStr

        End Function

        Private Function FetchChildTable() As String
            If Tables.Count <= 0 Then Return String.Empty

            Dim ret = New List(Of String)

            ret.Add("// fetching child object(s)")

            For Each kid In Tables()
                ret.Add(kid.BuildChildFetching(Keys(0)?.FieldName))
            Next

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            Return retStr

        End Function

        Private Function InsertUpdateChildTable() As String
            If Tables.Count <= 0 Then Return String.Empty

            Dim ret = New List(Of String)

            ret.Add("// update child object(s)")
            For Each kid In Tables()
                ret.Add(kid.BuildChildInsertUpdate)
            Next

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)


            Return retStr

        End Function

        Private Function DeleteChildTableScript() As String
            If Tables.Count <= 0 Then Return String.Empty

            Dim ret = New List(Of String)

            ret.Add("// Delete Children Table(s)")
            For Each kid In Tables()
                ret.Add(kid.DeleteChildTableScript(Keys(0)?.FieldName))
            Next

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)


            Return retStr

        End Function

#End Region


#Region "Services"

        ''' <summary>
        ''' value of  pKey1:pKey2 , used as ToString
        ''' or pKey1 if there is only one parameter
        ''' </summary>
        ''' <returns></returns>
        Private Function GetCombinedKeyValues() As String

            If Keys().Count = 0 Then Return String.Empty

            If Keys().Count = 1 Then

                Dim ret = Keys(0)
                If ret.PropType.MatchesRegExp("string") Then
                    Return ret.FieldName + ".TrimEnd()"
                Else
                    Return ret.FieldName
                End If

            Else

                Dim retStrings As New List(Of String)
                For Each f In Keys()
                    retStrings.Add(<k>{<%= f.FieldName %>}</k>.Value)
                Next

                Return <tx> $"<%= String.Join(":", retStrings.ToArray) %>".TrimEnd()</tx>.Value

            End If


        End Function


        ''' <summary>
        ''' string pKey1,int pKey2
        ''' </summary>
        ''' <param name="WithDefaultParameterValue"></param>
        ''' <returns></returns>
        Friend Function GetParameterKeys(WithDefaultParameterValue As Boolean) As String

            If Me.IsSingleton Then Return String.Empty

            Dim retStrings As New List(Of String)

            If Keys.Count = 0 Then Return String.Empty

            For Each f In Keys()

                If WithDefaultParameterValue Then
                    retStrings.Add($"{f.PropType} p{f.FieldName} = {f.GetDefaultParameterValue}")
                Else
                    retStrings.Add($"{f.PropType} p{f.FieldName}")
                End If
            Next

            Return String.Join(",", retStrings.ToArray)

        End Function


#Region "CSLA411"

        Private Function GetCheckNullParameters() As String
            If Keys.Count = 0 Then Return String.Empty

            If Keys.Count = 1 Then Return $"String.IsNullOrEmpty(p{Keys(0).FieldName})"

            Dim ret = New List(Of String)
            If Keys.Count > 1 Then
                For Each key In Keys()
                    ret.Add($"String.IsNullOrEmpty(p{key.FieldName})")
                Next

                Return String.Join(" && ", ret.ToArray())
            End If

            Return String.Empty
        End Function

        ''' <summary>
        ''' pKey1,pKey2
        ''' </summary>
        ''' <returns></returns>
        Friend Function GetUsingParameterKeys(Optional withP As Boolean = True) As String
            Dim retStrings As New List(Of String)

            If Keys.Count = 0 Then Return String.Empty

            For Each f In Keys()

                If withP Then
                    retStrings.Add($"p{f.FieldName}")
                Else
                    retStrings.Add($"{f.FieldName}")
                End If

            Next

            If retStrings.Count = 1 Then
                Return retStrings(0)
            Else
                Return String.Join(",", retStrings.ToArray)
            End If

        End Function

        ''' <summary>
        ''' KEY1='{pKey1}' AND KEY2='{pKey2}'
        ''' </summary>
        ''' <returns></returns>
        Friend Function GetSQLParameterKeys(Optional withCriteria As Boolean = False) As String
            Dim retStrings As New List(Of String)

            If Keys.Count = 0 Then Return String.Empty

            For Each f In Keys()

                If withCriteria Then
                    retStrings.Add(<t><%= f.DatabaseFieldName %> = '{criteria.<%= f.FieldName %>}'</t>.Value)
                Else
                    retStrings.Add(<t><%= f.DatabaseFieldName %> = '{p<%= f.FieldName %>}'</t>.Value)
                End If


            Next

            Return String.Join(" AND ", retStrings.ToArray)
        End Function

        ''' <summary>
        ''' KEy1=pKey1,pKey2=pKey2
        ''' </summary>
        ''' <returns></returns>
        Friend Function GetInitializeParameterKeys() As String
            Dim retStrings As New List(Of String)

            If Keys.Count = 0 Then Return String.Empty

            For Each f In Keys()

                retStrings.Add($"{f.FieldName}=p{f.FieldName}")

            Next

            Return String.Join(",", retStrings.ToArray)
        End Function

        ''' <summary>
        ''' pKey1:pKey2
        ''' </summary>
        ''' <returns></returns>
        Friend Function GetCombinedKeys() As String
            Dim retStrings As New List(Of String)

            If Keys.Count = 0 Then Return String.Empty

            For Each f In Keys()
                retStrings.Add($"p{f.FieldName}")
            Next

            If retStrings.Count = 1 Then
                Return retStrings(0)
            Else

                Dim arr = (From str In retStrings Select "{" & str & "}").ToArray

                ' Return String.Join(":", arr)

                Return <t>$"<%= String.Join(":", arr) %>"</t>.Value

            End If

        End Function

        '''' <summary>
        '''' value of  pKey1:pKey2 , used as ToString
        '''' or pKey1 if there is only one parameter
        '''' </summary>
        '''' <returns></returns>
        'Friend Function GetCombinedKeyValues() As String

        '    If Keys.Count = 0 Then Return String.Empty

        '    If Keys.Count = 1 Then

        '        Dim ret = Keys(0)
        '        If ret.PropType.MatchesRegExp("string") Then
        '            Return ret.FieldName + ".TrimEnd()"
        '        Else
        '            Return ret.FieldName
        '        End If

        '    Else
        '        Dim retStrings As New List(Of String)
        '        For Each f In Keys()
        '            retStrings.Add(<k>{<%= f.FieldName %>}</k>.Value)
        '        Next
        '        Return <tx> $"<%= String.Join(":", retStrings.ToArray) %>".TrimEnd(); </tx>.Value
        '    End If


        'End Function

        Friend Function SetOuputSQLParameter(Optional isOutput As Boolean = True) As String

            If Keys(0).IsAutoGenerated Then
                If isOutput Then
                    Return <t>cm.Parameters.AddWithValue("@<%= Keys(0).DatabaseFieldName %>", ReadPropertyConvert&lt;<%= Mapper.FieldType411(Keys(0).FieldType) %>, <%= Keys(0).FieldType %>>(<%= Keys(0).FieldName %>Property)).Direction= ParameterDirection.Output;</t>.Value
                Else
                    Return <t>cm.Parameters.AddWithValue("@<%= Keys(0).DatabaseFieldName %>", ReadPropertyConvert&lt;<%= Mapper.FieldType411(Keys(0).FieldType) %>, <%= Keys(0).FieldType %>>(<%= Keys(0).FieldName %>Property));</t>.Value
                End If

            End If
            Return String.Empty
        End Function

        Friend Function GetOuputSQLParameter() As String
            If Keys(0).IsAutoGenerated Then
                Return <t><%= Keys(0).FieldName %> = cm.GetReturnIdAsInteger("@<%= Keys(0).DatabaseFieldName %>").ToString();</t>.Value
            End If
            Return String.Empty

        End Function

        Friend Function DataportalUpdate() As String
            If Not Keys(0).IsAutoGenerated Then
                Return "DataPortal_Insert();"
            Else
                Return <t>
            using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
            {
                using (var cm = cn.CreateSQLCommand())
                {
                    cm.SetDBCommand(CommandType.StoredProcedure, $"<%= Me.UpdateSP %>.Replace("{XXX}", DTB)");

                    <%= SetOuputSQLParameter(False) %>

                    AddInsertParameters(cm);

                    cm.ExecuteNonQuery();

                }

                // update child object(s)

            }</t>.Value
            End If
        End Function


#End Region

#End Region

    End Class

End Namespace
