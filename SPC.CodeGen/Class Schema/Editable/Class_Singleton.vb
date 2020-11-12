Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Private Function GenerateEditableSingletonCode() As String

            Dim sections = New List(Of String)

            sections.Add(BuildEditableProperties)

            sections.Add(BuildEditableRules)

            sections.Add(BuildEditableSingletonFactories)

            sections.Add(BuildEditableCriteria)

            sections.Add(BuildEditableSingletonRESTAPI)

            sections.Add(BuildEditableDataAccess)

            sections.Add(BuildCacheGetSingleton)

            If Tables.Count > 0 Then sections.Add(BuildChildTables())

            Dim theCode = String.Join(Environment.NewLine, sections.ToArray)

            theCode = WrappWithEditableSingletonClassName(theCode)

            theCode = WrappWithNameSpace(theCode)

            theCode = theCode.Insert(0, Environment.NewLine)

            theCode = theCode.Insert(0, BuildEditableUsings)

            Return theCode

        End Function

#Region "Services"

        Private Function CloneSingletonAsync() As String

            Return <t> public Task&lt;<%= ClassName %>> CopyBOAsync()
                    {                      
                        return Task.FromResult(this);
                    }
                </t>.Value

        End Function

        Private Function SaveSingletonAsync() As String

            Return <t> public async Task&lt;IEditable> SaveBOAsync()
                    {
                        if (!IsDirty) ExceptionThower.BusinessRuleStop(ResStrConst.NotDirty(this));
                        if (!IsSavable) ExceptionThower.BusinessRuleStop(ResStrConst.NotSavable(this));
            
                        this.ApplyEdit();

                        var ret = this;
                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= ClassName %>)))
                            ret = await APIDataPortalPostAsync(this);
                        else
                            ret = await base.SaveAsync();

                        InvalidateCache();

                        return ret as IEditable;
                    }

                </t>.Value

        End Function

        Private Function SingletonExistAsync() As String
            Return <t>public static async Task&lt;bool> ExistsAsync(<%= GetParameterKeys(False) %>) 
                    { 
                        return true;
                    }                
                   </t>.Value
        End Function

        Private Function WrappWithEditableSingletonClassName(pCode As String) As String

            Return <code>
    [Serializable]
    [PhoebusCommand(Desc = "<%= Description %>")]
    [DB(TableName = "<%= DBTableName %>/<%= SubTable %>")]
    public class <%= ClassName %> : Csla.BusinessBase&lt; <%= ClassName %>>, IEditable , ISingleton<%= If(Scripts.Count > 0, ", ISupportScripts", "") %>
    {    
       public const string STR_<%= ClassName %> = "<%= ClassName %>";

        <%= pCode %>
    
    }

                   </code>.Value.Trim



        End Function


        Private Function BuildEditableSingletonFactories() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName

            ret.Add(<t>public <%= CLS %>() {}</t>.Value)

            '----------------NewAsync
            ret.Add(vbNewLine)
            ret.Add(<t>public static <%= CLS %> New<%= CLS %>() 
                    {                       
                      return DataPortal.Create&lt;<%= CLS %>>(new Criteria());                        
                    }                
                </t>.Value)

            '----------------GetAsync

            ret.Add(<t>public static <%= CLS %> Fetch<%= CLS %>() 
                    { 
                         if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= CLS %>)))
                            return APIDataPortalFetch(new Criteria());
                         else
                            return DataPortal.Fetch&lt;<%= CLS %>>(new Criteria());
                    }                
                </t>.Value)

            '-------------DeleteAsync
            ret.Add(<t>public static string Delete<%= CLS %>()
                    {
                        return string.Empty;
                    }       
                </t>.Value)

            '-------------ExistsAsync

            ret.Add(<t>public static bool Exists() 
                    { 
                        return true;
                    }                
             </t>.Value)


            '-------------Save
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

                       InvalidateCache();

                        return ret as IEditable;
                    }

                </t>.Value)


            '--------------Clone

            ret.Add(<t> public Task&lt;<%= CLS %>> CopyBOAsync(<%= GetParameterKeys(False) %>)
                    {                       
                      return Task.FromResult(this);
                    }

                </t>.Value)



            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Factory Methods")

            Return retStr

        End Function

        Private Function BuildEditableSingletonRESTAPI() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName


            ret.Add(<tx>
                       private static <%= CLS %> APIDataPortalCreate(Criteria criteria)
                        {
                           return DataPortal.Create&lt;<%= CLS %>>(criteria);
                        }
                </tx>.Value)

            ret.Add(<tx>private static <%= CLS %> APIDataPortalFetch(Criteria criteria)
                          {
                               var reader = RESTDataPortal.Fetch&lt;SPC.API.APIBOReader>(APICommand);

                                if (reader != null &amp;&amp; reader.Data != null)
                                {
                                    var ret = <%= CLS %>.Fetch(reader.Data);
                                    ret.MarkOld();
                                    return ret;
                                }

                                return New<%= CLS %>() ;
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

            ret.Add(<tx> private static Task&lt;string> APIDataPortalDeleteAsync(Criteria criteria)
                        {
                            return Task.FromResult("");
                        }
                </tx>.Value)

            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "RESTAPI")

            Return retStr

        End Function

        Private Function BuildCacheGetSingleton()

            Dim ret = New List(Of String)

            Dim CLS = ClassName

            ret.Add(<t>   
        
        [NonSerialized]
        private static <%= CLS %> _settings = null;
        internal static <%= CLS %> Get<%= CLS %>()
        {
            if (_settings == null)
                _settings = Fetch<%= CLS %>();

            return _settings;
        }

        public static void InvalidateCache()
        {
            _settings = null;
        }
                </t>.Value)



            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Cache Singleton")

            Return retStr

        End Function

#End Region

    End Class

End Namespace
