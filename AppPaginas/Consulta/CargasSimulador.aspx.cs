using KS.SimuladorPrecos.AppBaseInfo;
using KS.SimuladorPrecos.DataEntities.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KS.SimuladorPrecos.AppPaginas.Consulta
{
    public partial class CargasSimulador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            RecarregarAsGrids();

            if (!Page.IsPostBack)
            {

                ClientScript.RegisterClientScriptBlock(GetType(), "isPostBack", String.Format("var isPostback = {0};", IsPostBack.ToString().ToLower()), true);


                fulArquivoCustos.Attributes.Add("OnClick", "javascript:document.forms[0].encoding = \"multipart/form-data\";");
                fullArquivosRegrasGerais.Attributes.Add("OnClick", "javascript:document.forms[0].encoding = \"multipart/form-data\";");
                fullRegrasST.Attributes.Add("OnClick", "javascript:document.forms[0].encoding = \"multipart/form-data\";");
                fullRegrasPE.Attributes.Add("OnClick", "javascript:document.forms[0].encoding = \"multipart/form-data\";");

                fullTrava.Attributes.Add("OnClick", "javascript:document.forms[0].encoding = \"multipart/form-data\";");
                fullConvenio.Attributes.Add("OnClick", "javascript:document.forms[0].encoding = \"multipart/form-data\";");
            }
        }


        private void RecarregarAsGrids()
        {
            gvConvenio.DataSource = null;
            gvEncargos.DataSource = null;
            gvTabelaCusto.DataSource = null;
            gvTabelaRegra.DataSource = null;
            gvTrava.DataSource = null;



            CarregaGridConvenio();
            CarregaGridEncargos();
            CarregaGridPrecoRegrasST();
            CarregaGridPrecoCusto();
            CarregaGridPrecoRegrasGerais();
            CarregaGridTrava();
        }


        #region Trava Principal

        protected void btnCargaTrava_Click(object sender, EventArgs e)
        {
            CarregarTrava(fullTrava.PostedFile);
        }

        private void CarregarTrava(HttpPostedFile file)
        {
            var user = (UserDataInfo)Session["USUARIO"];
            if (WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"] == null
            || string.IsNullOrEmpty(WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"].ToString()))
            {
                PopularModalErro("Caminho para arquivo não definido em Web.Config!");
                return;
            }

            try
            {
                string path = WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_CUSTO_IMPORTACAO"].ToString().Trim();

                string DiaHora = "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_";
                string myFileName = path + file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                string fileName = file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //sstring ext = ;
                if (file.FileName.Substring(file.FileName.IndexOf('.')).ToUpper() != ".CSV")
                {

                    PopularModalErro("A extensão do arquivo não é um .CSV");
                    return;
                }

                DataTable dtn = new DataTable();
                // salva arquivo no server
                fullTrava.PostedFile.SaveAs(myFileName);
                try
                {

                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("\r\n");
                    System.Text.RegularExpressions.Regex csv = new System.Text.RegularExpressions.Regex("\t");

                    StreamReader sr = new StreamReader(myFileName, System.Text.Encoding.GetEncoding(1252));

                    var dts = GetTabelaExcel(myFileName);
                    string conteudo = sr.ReadToEnd();
                    string[] myRows = rx.Split(conteudo);

                    sr.Close();

                    string[] Fields;
                    Fields = myRows[0].Split(new char[] { ';' });
                    int Cols = Fields.GetLength(0);

                    DataRow Row;

                    for (int i = 0; i < Cols; i++)
                        dtn.Columns.Add(Fields[i].ToString(), typeof(string));

                    for (int i = 0; i < myRows.Length; i++)
                    {
                        if (myRows[i].IndexOf("CATEGORIA") < 0)
                        {

                            if (!string.IsNullOrEmpty(myRows[i]))
                            {
                                Fields = myRows[i].Split(new char[] { ';' });
                                Row = dtn.NewRow();

                                for (int f = 0; f < Cols; f++)
                                    Row[f] = Fields[f];

                                dtn.Rows.Add(Row);
                            }
                        }
                    }

                    SimuladorPrecoTrava PrecoCusto = new SimuladorPrecoTrava();

                    if (PrecoCusto.Deletar())
                    {
                        if (PrecoCusto.InsertBulkCopy(
                                  ConvertToDataDTTrava(dtn),
                                  "KsSimuladorPrecoCargaTrava"))
                        {
                            var usuario = (UserDataInfo)Session["USUARIO"];
                            PrecoCusto.usuarioId = usuario.UserLogin;
                            PrecoCusto.UpdateLog();
                            CarregaGridTrava();
                        }

                    }
                }
                catch (Exception ex)
                {
                    PopularModalErro(ex.Message);
                }
            }
            catch (Exception exs)
            {
                PopularModalErro(exs.Message);
            }
        }

        public DataTable ConvertToDataDTTrava(DataTable dt)
        {
            DataTable dts = new DataTable();
            try
            {

                dts.Columns.Add("categoria", typeof(string));
                dts.Columns.Add("lista", typeof(string));
                dts.Columns.Add("trava", typeof(string));
                dts.Columns.Add("mva_st", typeof(string));
                dts.Columns.Add("mva_ts_reduzido", typeof(string));

                foreach (var item in dt.AsEnumerable())
                {
                    dts.Rows.Add(item[0].ToString()
                             , item[1].ToString()
                             , item[2].ToString()
                             , item[3].ToString()
                             , item[4].ToString());
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }
            return dts;
        }

        protected void gvTrava_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvTrava_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gvTrava_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvTrava.PageIndex = e.NewPageIndex;
            CarregaGridTrava();
        }

        public void CarregaGridTrava()
        {

            try
            {
                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorPrecoTrava oPd = new SimuladorPrecoTrava();


                if (Request.Form.Count > 0)
                {
                    oPd.categoria = Request.Form.GetValues("txtCategoria")[0].ToString();
                }

                if (Request.Form.Count > 0)
                {
                    oPd.lista = Request.Form.GetValues("txtLista")[0].ToString();
                }

                #endregion

                oDt = oPd.ObterDadosPorFiltro();

                if (oDt.Rows.Count > 0)
                {
                    this.gvTrava.PageSize = 20;
                    this.gvTrava.DataSource = oDt;
                    this.gvTrava.DataBind();
                    GroupGridView(gvTrava.Rows, 0, 1);
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }

        }

        protected void btnFiltroTravaPrincipal_Click(object sender, EventArgs e)
        {
            CarregaGridTrava();
        }

        #endregion

        #region CUSTOS CARGA

        protected void btnCarregarCustos_Click(object sender, EventArgs e)
        {
            //uppSimuladorImportacao.Visible = true;
            CarregarCustos(fulArquivoCustos.PostedFile);
            CarregaGridEncargos();
            Response.Redirect("../Consulta/CargasSimulador.aspx");


        }

        private void CarregarCustos(HttpPostedFile file)
        {
            var user = (UserDataInfo)Session["USUARIO"];
            if (WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"] == null
            || string.IsNullOrEmpty(WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"].ToString()))
            {
                PopularModalErro("Caminho para arquivo não definido em Web.Config!");
                return;
            }

            try
            {
                string path = WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_CUSTO_IMPORTACAO"].ToString().Trim();

                string DiaHora = "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_";
                string myFileName = path + file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                string fileName = file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //sstring ext = ;
                if (file.FileName.Substring(file.FileName.IndexOf('.')).ToUpper() != ".CSV")
                {

                    PopularModalErro("A extensão do arquivo não é um .CSV");
                    return;
                }

                DataTable dtn = new DataTable();
                // salva arquivo no server
                fulArquivoCustos.PostedFile.SaveAs(myFileName);
                try
                {

                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("\r\n");
                    System.Text.RegularExpressions.Regex csv = new System.Text.RegularExpressions.Regex("\t");

                    StreamReader sr = new StreamReader(myFileName, System.Text.Encoding.GetEncoding(1252));

                    var dts = GetTabelaExcel(myFileName);
                    string conteudo = sr.ReadToEnd();
                    string[] myRows = rx.Split(conteudo);

                    sr.Close();

                    string[] Fields;
                    Fields = myRows[0].Split(new char[] { ';' });
                    int Cols = Fields.GetLength(0);

                    DataRow Row;

                    for (int i = 0; i < Cols; i++)
                        dtn.Columns.Add(Fields[i].ToString(), typeof(string));

                    for (int i = 0; i < myRows.Length; i++)
                    {
                        if (myRows[i].IndexOf("estabelecimento") < 0)
                        {

                            if (!string.IsNullOrEmpty(myRows[i]))
                            {
                                Fields = myRows[i].Split(new char[] { ';' });
                                Row = dtn.NewRow();

                                for (int f = 0; f < Cols; f++)
                                    Row[f] = Fields[f];

                                dtn.Rows.Add(Row);
                            }
                        }
                    }

                    SimuladorPrecoCustos PrecoCusto = new SimuladorPrecoCustos();

                    if (PrecoCusto.DeletarPrecoCustosTemp())
                    {
                        if (PrecoCusto.InserirSimuladorPrecoCustosTempDaSimuladorPrecoCustos())
                        {
                            var dtss = CanvertToDataDT(dtn);
                            if (dtss != null)
                            {
                                if (dtss.Rows.Count > 0)
                                {
                                    if (PrecoCusto.DeletarPrecoCustos())
                                    {
                                        if (PrecoCusto.WriteToDatabase(dtss, "ksSimuladorPrecoCustos"))
                                        {
                                            if (PrecoCusto.InserirLogCustos(myFileName, fileName, DateTime.Now, user.UserID))
                                            {
                                                PopularModalErro("Dados Salvos com sucesso!");
                                                CarregaGridPrecoCusto();
                                            }
                                        }
                                        else
                                        {
                                            PrecoCusto.InserirSimuladorPrecoCustosRollBack();
                                            PopularModalErro("Problemas para inserir os novos dados, verifique as planilhas selecionada!");
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    PopularModalErro(ex.Message);
                }
            }
            catch (Exception exs)
            {
                PopularModalErro(exs.Message);
            }
        }

        public DataTable CanvertToDataDT(DataTable dt)
        {
            DataTable dts = new DataTable();

            try
            {
                SimuladorPrecoCustos PrecoCusto = new SimuladorPrecoCustos();

                dts.Columns.Add("estabelecimentoId", typeof(string));
                dts.Columns.Add("itemId", typeof(string));
                dts.Columns.Add("ufIdOrigem", typeof(string));
                dts.Columns.Add("itemDescricao", typeof(string));
                dts.Columns.Add("laboratorioNome", typeof(string));
                dts.Columns.Add("tipo", typeof(string));
                dts.Columns.Add("exclusivoHospitalar", typeof(string));
                dts.Columns.Add("NCM", typeof(string));
                dts.Columns.Add("listaDescricao", typeof(string));
                dts.Columns.Add("categoria", typeof(string));
                dts.Columns.Add("resolucao13", typeof(string));
                dts.Columns.Add("tratamentoICMSEstab", typeof(string));
                dts.Columns.Add("precoFabrica", typeof(decimal));
                dts.Columns.Add("descontoComercial", typeof(decimal));
                dts.Columns.Add("descontoAdicional", typeof(decimal));
                dts.Columns.Add("percRepasse", typeof(decimal));
                dts.Columns.Add("precoAquisicao", typeof(decimal));
                dts.Columns.Add("percReducaoBase", typeof(decimal));
                dts.Columns.Add("percICMSe", typeof(decimal));
                dts.Columns.Add("valorCreditoICMS", typeof(decimal));
                dts.Columns.Add("percIPI", typeof(decimal));
                dts.Columns.Add("valorIPI", typeof(decimal));
                dts.Columns.Add("percPisCofins", typeof(decimal));
                dts.Columns.Add("valorPisCofins", typeof(decimal));
                dts.Columns.Add("pmc17", typeof(decimal));
                dts.Columns.Add("descST", typeof(decimal));
                dts.Columns.Add("mva", typeof(decimal));
                dts.Columns.Add("valorICMSST", typeof(decimal));
                dts.Columns.Add("aplicacaoRepasse", typeof(string));
                dts.Columns.Add("reducaoST_MVA", typeof(string));
                dts.Columns.Add("estabelecimentoNome", typeof(string));
                dts.Columns.Add("estabelecimentoUf", typeof(string));
                dts.Columns.Add("custoPadrao", typeof(decimal));
                dts.Columns.Add("aliquotaInternaICMS", typeof(decimal));
                dts.Columns.Add("percPmc", typeof(decimal));
                dts.Columns.Add("itemControlado", typeof(bool));
                dts.Columns.Add("capAplicado", typeof(bool));
                dts.Columns.Add("capDescontoPrc", typeof(decimal));
                dts.Columns.Add("transfEs", typeof(decimal));
                dts.Columns.Add("vendaComST", typeof(bool));
                dts.Columns.Add("TransfDF", typeof(decimal));
                dts.Columns.Add("classificFiscal", typeof(string));

                bool itemControlado;

                DataTable dtPrecoCusto = PrecoCusto.GetPrecoCustos();

                Item itm = new Item();

                DataTable dtItem = itm.ListarFiltro();
                foreach (var item in dt.AsEnumerable())
                {
                    var dataRow = (from product in dtItem.AsEnumerable()
                                   where product.Field<string>("itemId").PadLeft(5, '0') == item["CodigoItem"].ToString().PadLeft(5, '0')
                                   select product).FirstOrDefault();

                    if (dataRow != null)
                    {
                        if (dtItem != null)
                        {
                            if (dtItem.Rows.Count > 0)
                            {
                                itemControlado = bool.Parse(dataRow["itemControlado"].ToString());
                            }
                            else
                                itemControlado = false;
                        }
                        else
                            itemControlado = false;
                    }
                    else
                    {
                        itemControlado = false;
                    }


                    dts.Rows.Add(item["Estabelecimento"].ToString()
                      , item["CodigoItem"].ToString().PadLeft(5, '0')
                      , item["UFOrigem"].ToString()
                      , item["ItemDescricao"].ToString()
                      , item["LaboratorioNome"].ToString()
                      , item["Tipo"].ToString()
                      , item["ExclusivoHospitalar"].ToString()
                      , item["NCM"].ToString()
                      , item["Lista"].ToString()
                      , item["Categoria"].ToString()
                      , item["Resolucao13"].ToString()
                      , item["TratamentoICMSespecificoestabelecimento"].ToString()
                      , decimal.Parse(item["PF17%"].ToString())
                      , decimal.Parse(item["DescontoComercial"].ToString())
                      , decimal.Parse(item["DescontosAdicionais"].ToString())
                      , decimal.Parse(item["Repasse(%)"].ToString())
                      , decimal.Parse(item["PrecoAquisicao"].ToString())
                      , decimal.Parse(item["ReducaoBase(%)"].ToString())
                      , decimal.Parse(item["ICMSe(%)"].ToString())
                      , decimal.Parse(item["CreditoICMS($)"].ToString())
                      , decimal.Parse(item["IPI(%)"].ToString())
                      , decimal.Parse(item["IPI($)"].ToString())
                      , decimal.Parse(item["PIS/COFINS(%)"].ToString())
                      , decimal.Parse(item["PIS/COFINS($)"].ToString())
                      , !string.IsNullOrEmpty(item["PMC17%"].ToString()) ? decimal.Parse(item["PMC17%"].ToString()) : 0
                      , !string.IsNullOrEmpty(item["DescparaST"].ToString().Replace("-", "")) ? decimal.Parse(item["DescparaST"].ToString().Replace("-", "")) : 0
                      , !string.IsNullOrEmpty(item["MVA"].ToString()) ? decimal.Parse(item["MVA"].ToString()) : 0
                      , decimal.Parse(item["ICMS-ST($)"].ToString())
                      , item["AplicacaodeRepasse"].ToString()
                      , item["ReducaoparaSTquandocalculadapeloMVA"].ToString()
                      , item["EstabelecimentoNome"].ToString()
                      , item["EstabelecimentoUF"].ToString()
                      , item["CustoPadrao"].ToString()
                      , decimal.Parse(item["AliquotaInternaICMS"].ToString())
                      , decimal.Parse(item["PercentualPMC"].ToString())
                      , itemControlado
                      , item["capAplicado"].ToString().Equals("SIM") ? true : false
                      , decimal.Parse(item["capDescontoPrc"].ToString())
                      , decimal.Parse(item["transfEs"].ToString())
                      , int.Parse(item["vendaComSt"].ToString())
                      , decimal.Parse(item["TransfDF"].ToString())
                      , item["classificFiscal"].ToString());

                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);

            }
            return dts;


        }

        protected void gvTabelaCusto_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvTabelaCusto_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvTabelaCusto.PageIndex = e.NewPageIndex;
            CarregaGridPrecoCusto();
        }

        protected void gvTabelaCusto_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName.Equals("Download"))
            {
                //  Download(e.CommandArgument.ToString());
            }
        }

        public void CarregaGridPrecoCusto()
        {

            try
            {
                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorPrecoCustos oPd = new SimuladorPrecoCustos();

                #endregion

                oDt = oPd.GetPrecoCustosLog();

                if (oDt.Rows.Count > 0)
                {
                    this.gvTabelaCusto.PageSize = 10;
                    this.gvTabelaCusto.DataSource = oDt;
                    this.gvTabelaCusto.DataBind();
                    GroupGridView(gvTabelaCusto.Rows, 0, 1);
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }

        }

        protected void btnpesquisarFiltroCusto_Click(object sender, EventArgs e)
        {

        }

        protected void btnFiltroCusto_Click(object sender, EventArgs e)
        {
            SimuladorPrecoCustos oPd = new SimuladorPrecoCustos();

            var custos = Request.Form.GetValues("txtDeAteCusto");

            if (custos[0] != "")
            {
                DateTime dataDe = DateTime.Parse(custos[0].Substring(0, 10));
                DateTime dataAte = DateTime.Parse(custos[0].Substring(13, 10));

                DataTable oDt = oPd.GetPrecoCustosLogFiltro(dataDe, dataAte);

                if (oDt.Rows.Count > 0)
                {
                    this.gvTabelaCusto.PageSize = 10;
                    this.gvTabelaCusto.DataSource = oDt;
                    this.gvTabelaCusto.DataBind();

                }

            }
        }

        #endregion

        #region REGRAS GERAIS
        protected void btnCarregaRegrasGerais_Click(object sender, EventArgs e)
        {
            GravaArquivoTabelaRegra(fullArquivosRegrasGerais.PostedFile);
            CarregaGridPrecoRegrasGerais();
        }

        public void GravaArquivoTabelaRegra(HttpPostedFile file)
        {
            if (WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"] == null
                || string.IsNullOrEmpty(WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"].ToString()))
            {
                PopularModalErro("Caminho para arquivo não definido em Web.Config!");

                return;
            }

            try
            {
                string path = WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_CUSTO_IMPORTACAO"].ToString().Trim();


                string DiaHora = "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_";
                string myFileName = path + file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                string fileName = file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                #region  cria caminho, se o mesmo não existe

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                #endregion

                #region valida se extensão é CSV

                //sstring ext = ;
                if (file.FileName.Substring(file.FileName.IndexOf('.')).ToUpper() != ".CSV")
                {
                    PopularModalErro("extensão do arquivo não é um .CSV");

                    return;
                }

                #endregion

                #region Salva no server e adiciona dados no em datatable
                DataTable dtn = new DataTable();
                // salva arquivo no server
                fullArquivosRegrasGerais.PostedFile.SaveAs(myFileName);
                try
                {



                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("\r\n");
                    System.Text.RegularExpressions.Regex csv = new System.Text.RegularExpressions.Regex("\t");

                    StreamReader sr = new StreamReader(myFileName, System.Text.Encoding.GetEncoding(1252));


                    var dts = GetTabelaExcel(myFileName);
                    string conteudo = sr.ReadToEnd();
                    string[] myRows = rx.Split(conteudo);

                    sr.Close();

                    string[] Fields;
                    Fields = myRows[0].Split(new char[] { ';' });
                    int Cols = Fields.GetLength(0);

                    DataRow Row;

                    #region Cabeçalho

                    for (int i = 0; i < Cols; i++)
                        dtn.Columns.Add(Fields[i].ToString(), typeof(string));

                    #endregion

                    #region Linhas

                    for (int i = 0; i < myRows.Length; i++)
                    {
                        if (myRows[i].IndexOf("Estabelecimento") < 0)
                        {

                            if (!string.IsNullOrEmpty(myRows[i]))
                            {
                                Fields = myRows[i].Split(new char[] { ';' });
                                Row = dtn.NewRow();

                                for (int f = 0; f < Cols; f++)
                                    Row[f] = Fields[f];

                                dtn.Rows.Add(Row);
                            }
                        }

                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    PopularModalErro(ex.Message);

                }

                DataTable dtExcel = dtn;
                if (dtExcel.Rows.Count.Equals(0))
                {
                    PopularModalErro("Não foi encontrado nenhum registro na planilha");
                    return;
                }

                #endregion


                SimuladorPrecoRegrasGerais RegrasGerais = new SimuladorPrecoRegrasGerais();


                if (RegrasGerais.DeletarPrecoCustosTemp())
                {
                    if (RegrasGerais.InserirPrecoRegraTempDePrecoRegra())
                    {
                        var dts = ConvertToDataDTRegras(dtExcel);

                        if (RegrasGerais.DeletarPrecoCustos())
                        {
                            if (RegrasGerais.InsertBulkCopy(dts, "ksSimuladorPrecoRegrasGerais"))
                            {
                                var usuario = (UserDataInfo)Session["USUARIO"];

                                RegrasGerais.usuarioId = usuario.UserLogin;
                                RegrasGerais.NomeArquivo = fileName;
                                RegrasGerais.pathArquivo = path;
                                RegrasGerais.InserirLog();
                                CarregaGridPrecoRegrasGerais();
                            }


                        }
                    }
                }


            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }
        }

        public DataTable ConvertToDataDTRegras(DataTable dt)
        {
            DataTable dts = new DataTable();
            try
            {


                SimuladorPrecoRegrasGerais RegrasGerais = new SimuladorPrecoRegrasGerais();

                dts.Columns.Add("estabelecimentoId", typeof(string));
                dts.Columns.Add("convenioId", typeof(string));
                dts.Columns.Add("ufDestino", typeof(string));
                dts.Columns.Add("perfilCliente", typeof(string));
                dts.Columns.Add("resolucaoId", typeof(string));
                dts.Columns.Add("icmsStValor", typeof(decimal));
                dts.Columns.Add("usoExclusivoHospitalar", typeof(string));
                dts.Columns.Add("icmsSobreVenda", typeof(decimal));
                dts.Columns.Add("icmsStSobreVenda", typeof(decimal));
                dts.Columns.Add("ajusteRegimeFiscal", typeof(decimal));



                DataTable dtRegrasGerais = RegrasGerais.GetPrecoRegrasGerais();


                foreach (var item in dt.AsEnumerable())
                {
                    dts.Rows.Add(item[0].ToString()
                             , item[1].ToString()
                             , item[2].ToString()
                             , item[3].ToString()
                             , item[4].ToString()
                             , !string.IsNullOrEmpty(item[5].ToString()) ? decimal.Parse(item[5].ToString()) : (decimal?)null
                             , item[6].ToString()
                             , decimal.Parse(item[7].ToString(), CultureInfo.InvariantCulture)
                             , decimal.Parse(item[8].ToString())
                             , decimal.Parse(item[9].ToString()));
                }





            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);

            }

            return dts;

        }

        public void CarregaGridPrecoRegrasGerais()
        {

            try
            {
                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorPrecoRegrasGerais oPd = new SimuladorPrecoRegrasGerais();

                #endregion

                oDt = oPd.GetPrecoRegrasGeraisLog();



                if (oDt.Rows.Count > 0)
                {

                    this.gvTabelaRegra.PageSize = 10;
                    this.gvTabelaRegra.DataSource = oDt;
                    this.gvTabelaRegra.DataBind();
                    GroupGridView(gvTabelaRegra.Rows, 0, 1);
                }

            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }

        }


        protected void gvTabelaRegra_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {



                }
                else if (e.Row.RowType.Equals(DataControlRowType.Header))
                {


                }
            }
        }

        protected void gvTabelaRegra_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Download"))
            {

            }
        }

        protected void gvTabelaRegra_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            this.gvTabelaRegra.PageIndex = e.NewPageIndex;
            CarregaGridPrecoRegrasGerais();
            updRegras.Update();
        }

        protected void btnFiltroRegras_Click(object sender, EventArgs e)
        {

            DateTime dataDe = new DateTime();
            DateTime dataAte = new DateTime();
            var regras = Request.Form.GetValues("txtDeAteRegrasGerais");
            SimuladorPrecoRegrasGerais oPd = new SimuladorPrecoRegrasGerais();

            if (regras[0] != "")
            {
                if (string.IsNullOrEmpty(txtNomeArquivoRegras.Text) && string.IsNullOrEmpty(txtEstabelecimentoRegras.Text))
                {
                    dataDe = DateTime.Parse(regras[0].ToString().Substring(0, 10));
                    dataAte = DateTime.Parse(regras[0].Substring(13, 10));
                }
            }

            //   DataTable oDt = oPd.GetPrecoCustosLogFiltro(dataDe, dataAte, txtEstabelecimentoRegras.Text, txtNomeArquivoRegras.Text);

            //if (oDt.Rows.Count > 0)
            //{
            //    this.gvTabelaRegra.PageSize = 10;
            //    this.gvTabelaRegra.DataSource = oDt;
            //    this.gvTabelaRegra.DataBind();

            //}




        }

        #endregion

        #region REGRAS ST

        protected void btnCarregarRegrasST_Click(object sender, EventArgs e)
        {
            GravaArquivoTabelaRegrasST(fullRegrasST.PostedFile);
            CarregaGridPrecoRegrasST();
        }

        public void GravaArquivoTabelaRegrasST(HttpPostedFile file)
        {
            if (WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"] == null
              || string.IsNullOrEmpty(WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"].ToString()))
            {
                PopularModalErro("Caminho para arquivo não definido em Web.Config!");
                return;
            }

            try
            {
                string path = WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_CUSTO_IMPORTACAO"].ToString().Trim();
                string DiaHora = "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_";
                string myFileName = path + file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                string fileName = file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                #region  cria caminho, se o mesmo não existe

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                #endregion

                #region valida se extensão é CSV

                //sstring ext = ;
                if (file.FileName.Substring(file.FileName.IndexOf('.')).ToUpper() != ".CSV")
                {
                    PopularModalErro("Extensão não é um .CSV");

                    return;
                }

                #endregion

                #region Salva no server e adiciona dados no em datatable
                DataTable dtn = new DataTable();
                // salva arquivo no server
                fullRegrasST.PostedFile.SaveAs(myFileName);
                try
                {
                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("\r\n");
                    System.Text.RegularExpressions.Regex csv = new System.Text.RegularExpressions.Regex("\t");

                    StreamReader sr = new StreamReader(myFileName, System.Text.Encoding.GetEncoding(1252));

                    var dts = GetTabelaExcel(myFileName);
                    string conteudo = sr.ReadToEnd();
                    string[] myRows = rx.Split(conteudo);

                    sr.Close();

                    string[] Fields;
                    Fields = myRows[0].Split(new char[] { ';' });
                    int Cols = Fields.GetLength(0);

                    DataRow Row;

                    #region Cabeçalho

                    for (int i = 0; i < Cols; i++)
                        dtn.Columns.Add(Fields[i].ToString(), typeof(string));

                    #endregion

                    #region Linhas

                    for (int i = 0; i < myRows.Length; i++)
                    {
                        if (myRows[i].IndexOf("cargaRegraStId") < 0)
                        {

                            if (!string.IsNullOrEmpty(myRows[i]))
                            {
                                Fields = myRows[i].Split(new char[] { ';' });
                                Row = dtn.NewRow();

                                for (int f = 0; f < Cols; f++)
                                    Row[f] = Fields[f];

                                dtn.Rows.Add(Row);
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    PopularModalErro(ex.Message);
                }

                // joga csv em datatable
                DataTable dtExcel = dtn; // Utility.GetCsvAsDataTable(myFileName);
                if (dtExcel.Rows.Count.Equals(0))
                {
                    PopularModalErro("Não foi encontrado nenhum registro na planilha");
                    return;
                }

                #endregion

                SimuladorRegrasST RegrasGerais = new SimuladorRegrasST();
                if (dtExcel != null)
                {
                    if (dtExcel.Rows.Count > 1)
                    {
                        if (RegrasGerais.DeletarSimuladorRegrasST())
                        {
                            if (RegrasGerais.InsertBulkCopy(
                                ConvertToDataDTRegraSTs(dtExcel),
                                                        "KSSimuladorCargaRegrasST"))
                            {
                                PopularModalErro("DADOS SALVOS COM SUCESSO!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }
        }

        public DataTable ConvertToDataDTRegraSTs(DataTable dt)
        {
            DataTable dts = new DataTable();
            try
            {
                SimuladorRegrasST RegrasGerais = new SimuladorRegrasST();
                dts.Columns.Add("estabelecimentoId", typeof(string));
                dts.Columns.Add("itemId", typeof(string));
                dts.Columns.Add("classeFiscal", typeof(string));
                dts.Columns.Add("perfilCliente", typeof(string));
                dts.Columns.Add("estadoDestino", typeof(string));
                dts.Columns.Add("aliquota", typeof(string));
                dts.Columns.Add("PMC", typeof(string));
                dts.Columns.Add("icmsInterno", typeof(string));
                dts.Columns.Add("PMC_CHEIO", typeof(string));
                dts.Columns.Add("PMPF", typeof(string));
                dts.Columns.Add("dataImportacao", typeof(string));
                dts.Columns.Add("usuarioId", typeof(string));

                var usuario = (UserDataInfo)Session["USUARIO"];
                foreach (var item in dt.AsEnumerable())
                {
                    dts.Rows.Add(
                     item[0]
                    , item["itemId"].ToString().PadLeft(5, '0')
                    , item[2]
                    , item[3]
                    , item[4]
                    , item[5]
                    , item[6]
                    , item[7]
                    , item[8]
                     , item[9]
                    , DateTime.Now
                   , usuario.UserLogin);
                }
            }
            catch
            {
            }

            return dts;
        }

        public void CarregaGridPrecoRegrasST()
        {

            try
            {
                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorRegrasST oPd = new SimuladorRegrasST();

                #endregion

                oDt = oPd.ObterTodos();

                foreach (var item in oDt.AsEnumerable())
                {
                    switch (item["perfilCliente"].ToString().ToUpper())
                    {
                        case "C":
                            item["perfilCliente"] = " Mercado Privado Contribuinte";
                            break;
                        case "0":
                            item["perfilCliente"] = "Mercado Público";
                            break;
                        case "1":
                            item["perfilCliente"] = "Mercado Privado Não Contribuinte";
                            break;
                        case "2":
                            item["perfilCliente"] = "Pessoa Física";
                            break;

                        default:
                            break;
                    }

                    item["aliquota"] = (decimal.Parse(item["aliquota"].ToString()) * 100).ToString() + "%";
                    item["icmsInterno"] = (decimal.Parse(item["icmsInterno"].ToString()) * 100).ToString() + "%";
                }

                if (oDt.Rows.Count > 0)
                {

                    this.gridRegrasST.PageSize = 10;
                    this.gridRegrasST.DataSource = oDt;
                    this.gridRegrasST.DataBind();
                    GroupGridView(gridRegrasST.Rows, 0, 1);
                }

            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }

        }

        protected void gridRegrasST_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gridRegrasST_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gridRegrasST_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridRegrasST.PageIndex = e.NewPageIndex;
            CarregaGridPrecoRegrasST();
            //uppSimuladorImportacao.Update();
        }

        protected void btnFiltroRegrasST_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region REGRAS PE
        protected void btnCarregarRegrasPE_Click(object sender, EventArgs e)
        {
            GravarArquivoRegraPE(fullRegrasPE.PostedFile);
            CarregaGridEncargos();
        }

        private bool GravarArquivoRegraPE(HttpPostedFile file)
        {
            if (WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"] == null
             || string.IsNullOrEmpty(WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_REGRA_IMPORTACAO"].ToString()))
            {
                PopularModalErro("Caminho para arquivo não definido em Web.Config!");
                return false;
            }

            try
            {
                string path = WebConfigurationManager.AppSettings["PATH_TABELA_PRECO_CUSTO_IMPORTACAO"].ToString().Trim();


                string DiaHora = "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_";
                string myFileName = path + file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                string fileName = file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                #region  cria caminho, se o mesmo não existe

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                #endregion

                #region valida se extensão é CSV

                //sstring ext = ;
                if (file.FileName.Substring(file.FileName.IndexOf('.')).ToUpper() != ".CSV")
                {
                    PopularModalErro("Extensão do arquivo não é um .CSV");
                    //  Alert(GetResourceValue("msgExtensaoNaoPermitida"));
                    return false;
                }

                #endregion

                #region Salva no server e adiciona dados no em datatable
                DataTable dtn = new DataTable();
                // salva arquivo no server
                fullRegrasPE.PostedFile.SaveAs(myFileName);
                try
                {

                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("\r\n");
                    System.Text.RegularExpressions.Regex csv = new System.Text.RegularExpressions.Regex("\t");

                    StreamReader sr = new StreamReader(myFileName, System.Text.Encoding.GetEncoding(1252));


                    var dts = GetTabelaExcel(myFileName);
                    string conteudo = sr.ReadToEnd();
                    string[] myRows = rx.Split(conteudo);

                    sr.Close();

                    string[] Fields;
                    Fields = myRows[0].Split(new char[] { ';' });
                    int Cols = Fields.GetLength(0);

                    DataRow Row;

                    #region Cabeçalho

                    for (int i = 0; i < Cols; i++)
                        dtn.Columns.Add(Fields[i].ToString(), typeof(string));

                    #endregion

                    #region Linhas

                    for (int i = 0; i < myRows.Length; i++)
                    {
                        if (myRows[i].IndexOf("estabelecimentoId") < 0)
                        {

                            if (!string.IsNullOrEmpty(myRows[i]))
                            {
                                Fields = myRows[i].Split(new char[] { ';' });
                                Row = dtn.NewRow();

                                for (int f = 0; f < Cols; f++)
                                    Row[f] = Fields[f];

                                dtn.Rows.Add(Row);
                            }
                        }

                    }

                    #endregion

                }
                catch (Exception ex)
                {

                    PopularModalErro(ex.Message);
                }

                // joga csv em datatable
                DataTable dtExcel = dtn; // Utility.GetCsvAsDataTable(myFileName);
                if (dtExcel.Rows.Count.Equals(0))
                {
                    PopularModalErro("Não foi encontrado nenhum registro na planilha");
                    return false;
                }

                #endregion

                SimuladorPrecoRegraPE pe = new SimuladorPrecoRegraPE();

                if (pe.DeletarSimuladorRegraPETemp())
                {
                    if (pe.InserirPrecoRegraTempDePE())
                    {
                        if (pe.DeletarSimuladorRegraPE())
                        {
                            if (pe.InsertBulkCopy(ConvertToDataDTRegraPE(dtExcel), "KsSimuladorPrecoRegraPE"))
                            {
                                return true;
                            }
                            else
                            {
                                pe.InserirPrecoRegraTempDePETemp();
                                return false;
                            }
                        }
                        else
                        {
                            PopularModalErro("Problema ao executar a operação");
                            return false;
                        }
                    }
                    else
                    {
                        PopularModalErro("Problema ao executar a operação");
                        pe.InserirPrecoRegraTempDePE();
                        return false;
                    }
                }
                else
                {
                    PopularModalErro("Problema ao executar a operação");
                    return false;
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
                return false;
            }
        }

        public DataTable ConvertToDataDTRegraPE(DataTable dt)
        {
            DataTable dts = new DataTable();
            try
            {
                dts.Columns.Add("estabelecimentoId", typeof(string));
                dts.Columns.Add("codigoItem", typeof(string));
                dts.Columns.Add("ufOrigemFornec", typeof(string));
                dts.Columns.Add("ufDestinoCliente", typeof(string));
                dts.Columns.Add("contribuinte", typeof(string));
                dts.Columns.Add("encargos", typeof(string));
                dts.Columns.Add("dataImportacao", typeof(string));
                dts.Columns.Add("usuarioId", typeof(string));

                var usuario = (UserDataInfo)Session["USUARIO"];

                foreach (var item in dt.AsEnumerable())
                {
                    dts.Rows.Add(
                        item[0]
                    , item[1].ToString().PadLeft(5, '0')
                    , item[2]
                    , item[3]
                    , item[4]
                    , item[5]
                    , DateTime.Now,
                    usuario.UserLogin
                 );


                }

            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
                //Utility.WriteLog(ex);
                //Alert(Utility.ERRORMESSAGE);
            }

            return dts;

        }

        public void CarregaGridEncargos()
        {

            try
            {
                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorPrecoRegraPE oPd = new SimuladorPrecoRegraPE();

                #endregion

                oDt = oPd.ObterTodosFiltro();



                if (oDt.Rows.Count > 0)
                {

                    this.gvEncargos.PageSize = 10;
                    this.gvEncargos.DataSource = oDt;
                    this.gvEncargos.DataBind();
                    GroupGridView(gvEncargos.Rows, 0, 1);
                }


            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }

        }


        protected void gvEncargos_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvEncargos_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gvEncargos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEncargos.PageIndex = e.NewPageIndex;
            CarregaGridEncargos();
            //uppSimuladorImportacao.Update();
        }

        protected void btnFiltroRegrasPE_Click(object sender, EventArgs e)
        {
            try
            {



                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorPrecoRegraPE oPd = new SimuladorPrecoRegraPE();
                oPd.codigoItem = txtxItemIDRegrasPE.Text;
                oPd.estabelecimentoId = txtEstabelecimentoRegrasPe.Text;
                oPd.usuarioId = txtUsuarioRegrasPE.Text;

                var regrasPe = Request.Form.GetValues("txtDeAteRegrasPE");

                if (regrasPe[0] != "")
                {
                    if (string.IsNullOrEmpty(txtxItemIDRegrasPE.Text) && string.IsNullOrEmpty(txtEstabelecimentoRegrasPe.Text) && string.IsNullOrEmpty(txtUsuarioRegrasPE.Text))
                    {
                        DateTime dataDe = DateTime.Parse(regrasPe[0].Substring(0, 10));
                        DateTime dataAte = DateTime.Parse(regrasPe[0].Substring(13, 10));

                        oPd.dataImportacaoInicio = dataDe;
                        oPd.dataImportacaoFim = dataAte;
                    }
                }


                #endregion

                oDt = oPd.ObterTodosFiltro();



                if (oDt.Rows.Count > 0)
                {

                    this.gvEncargos.PageSize = 10;
                    this.gvEncargos.DataSource = oDt;
                    this.gvEncargos.DataBind();
                    GroupGridView(gvEncargos.Rows, 0, 1);
                }


            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }
        }

        #endregion

        #region CONVENIOS

        public DataTable ConvertToDataDTConvenio(DataTable dt)
        {
            DataTable dts = new DataTable();
            try
            {

                dts.Columns.Add("item", typeof(string));
                dts.Columns.Add("origem", typeof(string));
                dts.Columns.Add("desitno", typeof(string));
                dts.Columns.Add("convenio", typeof(string));


                foreach (var item in dt.AsEnumerable())
                {
                    dts.Rows.Add(item[0].ToString()
                             , item[1].ToString()
                             , item[2].ToString()
                             , item[3].ToString());
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }
            return dts;
        }

        public void CarregaGridConvenio()
        {

            try
            {
                DataTable oDt;

                #region :: Get Filtros ::

                SimuladorPrecoConvenio oPd = new SimuladorPrecoConvenio();

                #endregion

                oDt = oPd.ObterDadosPorFiltro();

                if (oDt.Rows.Count > 0)
                {
                    this.gvConvenio.PageSize = 20;
                    this.gvConvenio.DataSource = oDt;
                    this.gvConvenio.DataBind();
                    GroupGridView(gvConvenio.Rows, 0, 1);
                }
            }
            catch (Exception ex)
            {
                PopularModalErro(ex.Message);
            }

        }


        private void CarregarConvenios(HttpPostedFile file)
        {
            var user = (UserDataInfo)Session["USUARIO"];
            if (WebConfigurationManager.AppSettings["PATH_TABELA_CONVENIOS"] == null
            || string.IsNullOrEmpty(WebConfigurationManager.AppSettings["PATH_TABELA_CONVENIOS"].ToString()))
            {
                PopularModalErro("Caminho para arquivo não definido em Web.Config!");
                return;
            }

            try
            {
                string path = WebConfigurationManager.AppSettings["PATH_TABELA_CONVENIOS"].ToString().Trim();

                string DiaHora = "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_";
                string myFileName = path + file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];
                string fileName = file.FileName.Split('.')[0] + DiaHora + "." + file.FileName.Split('.')[1];

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //sstring ext = ;
                if (file.FileName.Substring(file.FileName.IndexOf('.')).ToUpper() != ".CSV")
                {
                    PopularModalErro("A extensão do arquivo não é um .CSV");
                    return;
                }

                DataTable dtn = new DataTable();
                fullConvenio.PostedFile.SaveAs(myFileName);
                try
                {

                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("\r\n");
                    System.Text.RegularExpressions.Regex csv = new System.Text.RegularExpressions.Regex("\t");

                    StreamReader sr = new StreamReader(myFileName, System.Text.Encoding.GetEncoding(1252));

                    var dts = GetTabelaExcel(myFileName);
                    string conteudo = sr.ReadToEnd();
                    string[] myRows = rx.Split(conteudo);

                    sr.Close();

                    string[] Fields;
                    Fields = myRows[0].Split(new char[] { ';' });
                    int Cols = Fields.GetLength(0);

                    DataRow Row;

                    for (int i = 0; i < Cols; i++)
                        dtn.Columns.Add(Fields[i].ToString(), typeof(string));

                    for (int i = 0; i < myRows.Length; i++)
                    {
                        if (myRows[i].IndexOf("item") < 0)
                        {

                            if (!string.IsNullOrEmpty(myRows[i]))
                            {
                                Fields = myRows[i].Split(new char[] { ';' });
                                Row = dtn.NewRow();

                                for (int f = 0; f < Cols; f++)
                                    Row[f] = Fields[f];

                                dtn.Rows.Add(Row);
                            }
                        }
                    }

                    SimuladorPrecoConvenio PrecoConvenio = new SimuladorPrecoConvenio();

                    if (PrecoConvenio.Deletar())
                    {
                        if (PrecoConvenio.WriteToDatabase(
                                  ConvertToDataDTConvenio(dtn),
                                  "KsSimuladorPrecoCargaConvenio"))
                        {
                            var usuario = (UserDataInfo)Session["USUARIO"];
                            PrecoConvenio.usuarioId = usuario.UserLogin;
                            PrecoConvenio.InsertLog(myFileName);
                            CarregaGridConvenio();
                        }

                    }
                }
                catch (Exception ex)
                {
                    PopularModalErro(ex.Message);
                }
            }
            catch (Exception exs)
            {
                PopularModalErro(exs.Message);
            }
        }


        protected void btnConvenio_Click(object sender, EventArgs e)
        {
            CarregarConvenios(fullConvenio.PostedFile);
        }

        protected void btnFiltroConvenio_Click(object sender, EventArgs e)
        {

        }

        protected void gvConvenio_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvConvenio.PageIndex = e.NewPageIndex;
            CarregaGridConvenio();
        }

        protected void gvConvenio_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvConvenio_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        #endregion

        #region Metodos Privados

        private DataTable GetTabelaExcel(string arquivoExcel)
        {
            DataTable dt = new DataTable();

            //pega a extensão do arquivo
            string Ext = Path.GetExtension(arquivoExcel);

            //verifica a versão do Excel pela extensão

            return dt;
        }

        void GroupGridView(GridViewRowCollection gvrc, int startIndex, int total)
        {
            if (total == 0) return;
            int i, count = 1;
            System.Collections.ArrayList lst = new System.Collections.ArrayList();
            lst.Add(gvrc[0]);
            var ctrl = gvrc[0].Cells[startIndex];
            for (i = 1; i < gvrc.Count; i++)
            {
                TableCell nextCell = gvrc[i].Cells[startIndex];
                if (ctrl.Text == nextCell.Text)
                {
                    count++;
                    nextCell.Visible = false;
                    lst.Add(gvrc[i]);
                }
                else
                {
                    if (count > 1)
                    {
                        ctrl.RowSpan = count;
                        GroupGridView(new GridViewRowCollection(lst), startIndex + 1, total - 1);
                    }
                    count = 1;
                    lst.Clear();
                    ctrl = gvrc[i].Cells[startIndex];
                    lst.Add(gvrc[i]);
                }
            }
            if (count > 1)
            {
                ctrl.RowSpan = count;
                GroupGridView(new GridViewRowCollection(lst), startIndex + 1, total - 1);
            }
            count = 1;
            lst.Clear();
        }

        public void PopularModalErro(string textoErro)
        {
            // lblModalTitle.Text = "";
            lblModalBody.Text = textoErro;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();

        }



        #endregion



    }
}