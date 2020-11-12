Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Private Function GenerateEditableRFMSCCode() As String

            Dim sections = New List(Of String)

            sections.Add(BuildEditableProperties)

            sections.Add(BuildEditableDoclink)

            sections.Add(BuildEditableRules)

            sections.Add(BuildEditableCriteria)

            sections.Add(BuildEditableFactories)

            sections.Add(BuildEditableRESTAPI)

            sections.Add(BuildEditableDataAccess)

            If Tables.Count > 0 Then sections.Add(BuildChildTables())

            Dim theCode = String.Join(Environment.NewLine, sections.ToArray)

            theCode = WrappWithEditableClassName(theCode)

            theCode = WrappWithNameSpace(theCode)

            theCode = theCode.Insert(0, Environment.NewLine)

            theCode = theCode.Insert(0, BuildEditableUsings)

            Return theCode

        End Function

#Region "Services"

        Private Function RFMSCExistAsync() As String
            Return <t>public static async Task&lt;bool> ExistsAsync(<%= GetParameterKeys(False) %>)
                    {
                        if (<%= GetCheckNullParameters() %>) return false;

                        var criteria = new Criteria(<%= GetUsingParameterKeys(True) %>);

                        string SqlText = $"SELECT COUNT(*) FROM pbs_RFMSC WHERE PBS_DB='{Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= ClassName %>))}' AND PBS_TB = '{PBS_TB}' AND KEY_FIELDS='{criteria.ToString()}'";

                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= ClassName %>)))
                            return (await RESTDataPortal.GetScalarIntegerAsync(SqlText)) > 0;
                        else
                            return (await SPC.Database.SQLCommander.GetScalarIntegerAsync(SqlText)) > 0;
                    }              
                </t>.Value
        End Function

        Private Function SSRFMSCExistAsync() As String
            Return <t>public static async Task&lt;bool> ExistsAsync(<%= GetParameterKeys(False) %>)
                    {
                        if (<%= GetCheckNullParameters() %>) return false;

                         var criteria = new Criteria(<%= GetUsingParameterKeys(True) %>);

                         string SqlText = $"SELECT COUNT(*) FROM SSRFMSC WHERE SUN_DB='{Ctx.EntityInfo.GetMasterEntityCode(typeof(<%= ClassName %>))}' AND SUN_TB = '{SUN_TB}' AND KEY_FIELDS='{criteria.ToString()}'";

                        if (SPC.Ctx.AppConfig.UseAPIDataPortal(typeof(<%= ClassName %>)))
                            return (await RESTDataPortal.GetScalarIntegerAsync(SqlText)) > 0;
                        else
                            return (await SPC.Database.SQLCommander.GetScalarIntegerAsync(SqlText)) > 0;
                    }              
                </t>.Value
        End Function

        Private Function RFMSCFetching() As String

            Return <tx>
                        private void DataPortal_Fetch(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            ExecuteFetch(cn, criteria);
                        }
                    }

                    private void ExecuteFetch(IDbConnection cn, Criteria criteria)
                    {
                        using (var cm = cn.Get_RFMSCFetchCommand(DTB, PBS_TB, criteria.ToString()))
                        {

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
                                  <%= LoadKeysFromKEY_FIELDS() %>

                                   var _data = dr.GetString("PSM_DATA").TrimEnd();

                                   if (!string.IsNullOrEmpty(_data)) 
                                    {
                                        try
                                            {
                                                var xele = XElement.Parse(_data.Decompress());
                            
                                                <%= String.Join(Environment.NewLine, (From k In SingleFields() Select k.GetFetchPropertyRFMSC411).ToArray) %> 

                                            }
                                        catch (Exception){}
                                    }
                                 
                                 //LoadPropertyConvert&lt;SmartDate, string>(UpdatedProperty, dr.GetString("UPDATED"));

                               }
                            }
                     }       
                   </tx>.Value

        End Function

        Private Function SSRFMSCFetching() As String

            Return <tx>

                    private void DataPortal_Fetch(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            ExecuteFetch(cn, criteria);
                        }
                    }

                    private void ExecuteFetch(IDbConnection cn, Criteria criteria)
                    {
                        using (var cm = cn.Get_SSRFMSCFetchCommand(DTB, SUN_TB, criteria.ToString()))
                        {

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
                                  <%= LoadKeysFromKEY_FIELDS() %>

                                   var _data = dr.GetString("SUN_DATA").TrimEnd();

                                   if (!string.IsNullOrEmpty(_data)) 
                                    {
                                        try
                                            {
                                                var xele = XElement.Parse(_data.Decompress());
                            
                                                <%= String.Join(Environment.NewLine, (From k In SingleFields() Select k.GetFetchPropertyRFMSC411).ToArray) %> 

                                            }
                                        catch (Exception){}
                                    }
                                 
                                 //LoadPropertyConvert&lt;SmartDate, string>(UpdatedProperty, dr.GetString("UPDATED"));

                               }
                            }
                     }       
                   </tx>.Value

        End Function

        Private Function RFMSCInsertUpdate() As String

            Return <tx>
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
            using (var cm = cn.Get_pbs_RFMSC_InsertUpdateCommand())
            {
                AddInsertParameters(cm);
                cm.ExecuteNonQuery();
            }
        }

        private void AddInsertParameters(IDbCommand cm)
        {
            var _data = new XElement("d");

            <%= String.Join(Environment.NewLine, (From k In Fields Select k.InsertUpdateToDBRFMSC).ToArray) %>
                    
            cm.Parameters.AddWithValue("@PBS_DB", DTB);
            cm.Parameters.AddWithValue("@PBS_TB", PBS_TB);
            cm.Parameters.AddWithValue("@KEY_FIELDS", this.ToString());
            cm.Parameters.AddWithValue("@LOOKUP", String.Empty);
            cm.Parameters.AddWithValue("@UPDATED", DateTime.Today.ToSunDate());
            cm.Parameters.AddWithValue("@PSM_DATA", _data.ToString().Compress());
        }

        protected override void DataPortal_Update()
        {
               DataPortal_Insert();

        }  
         </tx>.Value

        End Function

        Private Function SSRFMSCInsertUpdate() As String

            Return <tx>
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
            using (var cm = cn.Get_SSRFMSC_InsertUpdateCommand())
            {
                AddInsertParameters(cm);
                cm.ExecuteNonQuery();
            }
        }

        private void AddInsertParameters(IDbCommand cm)
        {
            var _data = new XElement("d");

            <%= String.Join(Environment.NewLine, (From k In Fields Select k.InsertUpdateToDBRFMSC).ToArray) %>
                    
            cm.Parameters.AddWithValue("@SUN_DB", DTB);
            cm.Parameters.AddWithValue("@SUN_TB", SUN_TB);
            cm.Parameters.AddWithValue("@KEY_FIELDS", this.ToString());
            cm.Parameters.AddWithValue("@LOOKUP", String.Empty);
            cm.Parameters.AddWithValue("@UPDATED", DateTime.Today.ToSunDate());
            cm.Parameters.AddWithValue("@SUN_DATA", _data.ToString().Compress());
        }

        protected override void DataPortal_Update()
        {
               DataPortal_Insert();

        }  
         </tx>.Value

        End Function

        Private Function RFMSCDelete() As String
            Return <tx>private void DataPortal_Delete(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            using (var cm = cn.Get_RFMSC_DeleteCommand(DTB, PBS_TB, criteria.ToString()))
                            {
                                cm.ExecuteNonQuery();
                            }

                        }
                    }
                   </tx>.Value
        End Function

        Private Function SSRFMSCDelete() As String
            Return <tx>private void DataPortal_Delete(Criteria criteria)
                    {
                        using (var cn = SPC.Database.ConnectionFactory.GetDBConnection(true))
                        {
                            using (var cm = cn.Get_SSRFMSC_DeleteCommand(DTB, SUN_TB, criteria.ToString()))
                            {
                                cm.ExecuteNonQuery();
                            }

                        }
                    }
                   </tx>.Value
        End Function

        Private Function LoadKeysFromKEY_FIELDS() As String

            If Keys() IsNot Nothing AndAlso Keys.Count = 1 Then

                Return <t> LoadProperty(<%= Keys(0).FieldName %>Property, dr.GetString("KEY_FIELDS").TrimEnd());</t>.Value

            ElseIf Keys() IsNot Nothing AndAlso Keys.Count > 1 Then
                Dim ret = New List(Of String)

                ret.Add("var theKey = dr.GetString(""KEY_FIELDS"").TrimEnd();")
                ret.Add("var thekeys = theKey.Split(new char[] {':'});")

                For idx = 0 To Keys.Count - 1

                    ret.Add($"if (thekeys.Length > {idx}) LoadProperty({Keys(idx).FieldName}Property, thekeys[{idx}]); else LoadProperty({Keys(idx).FieldName}Property, """");")
                Next

                Return String.Join(Environment.NewLine, ret.ToArray())

            End If

            Return String.Empty
        End Function



#End Region

    End Class

End Namespace
