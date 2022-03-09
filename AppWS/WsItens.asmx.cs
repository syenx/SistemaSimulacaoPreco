using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;
using KS.SimuladorPrecos.DataEntities.Entidades;
using KS.SimuladorPrecos.DataEntities.Utility;

namespace KS.SimuladorPrecos.AppWS
{
    /// <summary>
    /// Summary description for WsItens
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WsItens : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod]
        public string[] GetItens(string prefixText)
        {
            try
            {
                DataTable oDt = new DataTable();
                string[] itens = null;


                oDt =
                    new SimuladorPrecoCustos
                        {
                            itemId = Utility.IsNumber(prefixText) ? prefixText : string.Empty,
                            itemDescricao = !Utility.IsNumber(prefixText) ? prefixText : string.Empty
                        }.GetItensIdOuDescri();


                if (oDt != null)
                {
                    if (oDt.Rows.Count > 0)
                    {
                        itens = new string[oDt.Rows.Count];
                        int i = new int();

                        foreach (DataRow row in oDt.Rows)
                            itens[i++] =
                                string.Format("{0}|{1}",
                                               row["itemId"].ToString(),
                                               row["itemDescricao"].ToString());
                    }
                    else
                        return new string[] { };
                }
                else
                    return new string[] { };

                return itens;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return new string[] { };
            }
        }
    }
}
