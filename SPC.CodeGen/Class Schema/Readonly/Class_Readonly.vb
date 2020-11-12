Imports pbs.Helper

Namespace MetaData

    Partial Public Class EditableDefinition

        Public Function GenerateReadonlyCode() As String

            Dim sections = New List(Of String)

            sections.Add(BuildReadonlyProperties)

            sections.Add(BuildEditableDoclink)

            sections.Add(BuildReadonlyFactories)

            sections.Add(BuildReadonlyDataAccess)

            If Tables.Count > 0 Then sections.Add(BuildChildTables(True))

            Dim theCode = String.Join(Environment.NewLine, sections.ToArray)

            theCode = WrappWithReadonlyClassName(theCode)

            theCode = WrappWithNameSpace(theCode)

            theCode = theCode.Insert(0, Environment.NewLine)

            theCode = theCode.Insert(0, BuildReadonlyUsings)

            Return theCode

        End Function

#Region "Business Properties"

        Private Function BuildReadonlyProperties() As String
            Dim sections = New List(Of String)

            'SingleFields
            For Each prop In Fields
                If Not prop.IsChildCollection Then sections.Add(prop.BuildReadonlyProperty)
            Next

            sections.Add(BuildToString)

            Dim ret = String.Join(Environment.NewLine, sections.ToArray)

            ret = WrappWithRegion(ret, "Business Properties")

            Return ret
        End Function


#End Region

#Region "Factories"

        Private Function BuildReadonlyFactories() As String
            Dim ret = New List(Of String)

            Dim CLS = ClassName


            ret.Add(<t>public <%= CLS %>Info() {}</t>.Value)

            '---------------EmptyLRQInfo
            ret.Add(<t>internal static <%= CLS %>Info Empty<%= CLS %>Info(<%= GetParameterKeys(True) %>) </t>.Value)
            ret.Add("{")
            ret.Add(<t><%= CLS %>Info info = new <%= CLS %>Info();</t>.Value)

            For Each key In Keys()
                ret.Add($"info.{key.GetFetchProperty411FromCriteria()};")
            Next

            ret.Add("return info;")

            ret.Add("}")

            '----------------GetInfo using datareader
            ret.Add(<t>internal static <%= ClassName %>Info Get<%= ClassName %>Info(SafeDataReader dr) 
                    { return new <%= ClassName %>Info(dr); }                
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


            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Factory Methods")

            Return retStr

        End Function

#End Region



#Region "DataAccess"

        Private Function BuildReadonlyDataAccess() As String
            Dim CLS = ClassName

            Dim ret = New List(Of String)

            ret.Add($" private {ClassName}Info(SafeDataReader dr)")

            ret.Add("{")


            If SubTable = "" Then
                For Each finfo In SingleFields()
                    ret.Add(finfo.GetFetchProperty411)
                Next
            Else

                If Keys() Is Nothing OrElse Keys.Count = 0 Then
                    'no key
                Else
                    Dim theKeyProp = Keys(0)?.FieldName
                    ret.Add(<txt> LoadProperty(<%= theKeyProp %>Property, dr.GetString("KEY_FIELDS").TrimEnd());</txt>.Value)
                End If

                Dim text = <text>var _data = dr.GetString("PSM_DATA").TrimEnd();

                   if (!string.IsNullOrEmpty(_data)) 
                                {
                                    try
                                    {
                                        var xele = XElement.Parse(_data.Decompress());
                            
                                        <%= String.Join(Environment.NewLine, (From k In SingleFields() Select k.GetFetchPropertyRFMSC411).ToArray) %> 

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                                                  
                LoadPropertyConvert&lt;SmartDate, String > (UpdatedProperty, dr.GetString("UPDATED"));
                       </text>.Value

                ret.Add(text)

            End If


            ret.Add("}")



            Dim retStr = String.Join(Environment.NewLine, ret.ToArray)

            retStr = WrappWithRegion(retStr, "Data Access")

            Return retStr

        End Function

#End Region

#Region "Class Header"

        Private Function BuildReadonlyUsings() As String
            Return $"
using System;
using System.Data;
using System.Text;
using SPC.Helper.Extension;
using Csla;
using SPC.Helper;
using SPC.SmartData;
using SPC.Data;
using SPC.Interfaces;
using SPC.Services;
using System.Text.RegularExpressions;
using System.Collections.Generic;       
using SmartDate = SPC.SmartData.SmartDate;
using System.Xml.Linq;

"

        End Function


        Private Function WrappWithReadonlyClassName(pCode As String) As String

            Return <code>
    [Serializable]
    public class <%= ClassName %>Info : Csla.ReadOnlyBase&lt; <%= ClassName %>Info>, IDocLink 
    {    
    
    <%= pCode %>
    
    }

                   </code>.Value.Trim



        End Function


#End Region


    End Class

End Namespace
