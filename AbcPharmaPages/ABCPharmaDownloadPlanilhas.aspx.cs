using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Xml;


namespace KS.SimuladorPrecos.AbcPharmaPages
{
    public partial class ABCPharmaDownloadPlanilhas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 17; i++)
            {
                var cli = new WebClient();

                string url = "https://webserviceabcfarma.org.br/webservice/";

                Console.WriteLine("Carregando pagina :" + i + ".... Aguarde");
                var parametros = "cnpj_cpf=61940292004981&senha=admin123&cnpj_sh=61940292004981&pagina=" + i; cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                var response = cli.UploadString(url, parametros);


                XmlNode xml = JsonConvert.DeserializeXmlNode("{record:{record:" + response + "}}");
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml.InnerXml);
                XmlReader xmlReader = new XmlNodeReader(xml);
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(xmlReader);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=abcPharma_page"+ i +"data"+ DateTime.Now.ToString() + @".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {

                        for (int j = 0; j < dataSet.Tables.Count; j++)
                        {
                            wb.Worksheets.Add(dataSet.Tables[j], dataSet.Tables[j].TableName);
                        }

                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
        public static void ExportDataSetToExcel(DataSet ds, int pagina)
        {
            string AppLocation = "";
            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            AppLocation = AppLocation.Replace("file:\\", "");
            string date = DateTime.Now.ToShortDateString();
            date = date.Replace("/", "_");
            string filepath = AppLocation + "\\ExcelFiles\\" + "ABCFARMA Pagina-" + pagina + " " + date + ".xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    wb.Worksheets.Add(ds.Tables[i], ds.Tables[i].TableName);
                }
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.SaveAs(filepath);
            }
        }
    }

}
