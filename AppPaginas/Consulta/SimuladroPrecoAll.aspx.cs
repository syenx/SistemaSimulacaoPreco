using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using KS.SimuladorPrecos.AppBaseInfo;
using KS.SimuladorPrecos.DataEntities.Entidades;
using KS.SimuladorPrecos.DataEntities.Utility;
using System.Data;
using KS.SimuladorPrecos.DataEntities.Business;
using System.Net;
using System.Text;
using ClosedXML.Excel;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Web;

namespace KS.SimuladorPrecos.AppPaginas.Consulta
{
    public partial class SimuladroPrecoAll : PageBase
    {
        private List<SimuladorPrecoRetorno> listaRetorno
        {
            get { return this.ViewState["listaRetorno"] != null ? (List<SimuladorPrecoRetorno>)ViewState["listaRetorno"] : new List<SimuladorPrecoRetorno>(); }
            set { this.ViewState["listaRetorno"] = value; }
        }
        private string itemID
        {
            get { return this.ViewState["itemID"].ToString(); }
            set { this.ViewState["itemID"] = value; }
        }
        private bool CapAplicado
        {
            get { return this.ViewState["CapAplicado"] != null ? bool.Parse(this.ViewState["CapAplicado"].ToString()) : false; }
            set { this.ViewState["CapAplicado"] = value; }
        }
        private List<string> ListaEstadoDestino()
        {
            return new List<string> { "AC", "AL", "AP", "AM", "BA", "CE", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO", "DF" };

        }
        private bool IsContribuinte
        {
            get
            {
                switch (this.rblPerfilCliente.SelectedValue.ToUpper())
                {
                    case "C":
                        return true;

                    default:
                        return false;
                }
            }
        }
        private bool IsSetorPublico
        {
            get { return rblPerfilCliente.SelectedValue.Equals("0"); }
        }
        public decimal ValorIcmsSobreVenda { get; private set; }
        List<SimuladorSumarizacao> lstItens = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            //   this.Title = GetResourceValue("titTitulo", true, "lblSimuladorPreco");

            if (!Page.IsPostBack)
            {
                ckeckCalculaST.Enabled = false;
                listaRetorno = new List<SimuladorPrecoRetorno>();

                AddScripts();
                DataAtualizacao();

                RemoverBloqueio();
            }
            RemoverBloqueio();
        }
        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            #region :: Visualização dos Painéis ::

            if (UserDataInfo.IsAdm)
                rblPerfilCliente.Items.Cast<ListItem>().ToList().ForEach(x => x.Enabled = true);
            else if (UserDataInfo.TipoOperacao.Equals(TipoUnidadeNegocio.PJPublico))
                rblPerfilCliente.Items.Cast<ListItem>().Where(x => !x.Value.Equals("0")).ToList().ForEach(y => y.Enabled = false);
            else if (UserDataInfo.TipoOperacao.Equals(TipoUnidadeNegocio.PJPrivado))
                rblPerfilCliente.Items.Cast<ListItem>().Where(x => !x.Value.Equals("C") && !x.Value.Equals("1")).ToList().ForEach(y => y.Enabled = false);
            else
                rblPerfilCliente.Items.Cast<ListItem>().Where(x => !x.Value.Equals("1") && !x.Value.Equals("2")).ToList().ForEach(y => y.Enabled = false);

            #endregion
            AddScripts();

            RemoverBloqueio();
            #region :: Grid Sumarizacao ::



            #endregion
        }
        protected void btnItem_Click(object sender, EventArgs e)
        {
            GetItem();
        }
        protected void btnSimular_Click(object sender, EventArgs e)
        {

            Simular();
        }
        protected void dlSimulacoes_ItemDataBound(object sender, DataListItemEventArgs e)
        {

        }
        protected void dlSimulacoes_ItemCreated(object sender, DataListItemEventArgs e)
        {
            if (Session["GvSumarizacao"] != null)
            {

                ((GridView)Session["GvSumarizacao"]).RowDataBound += new GridViewRowEventHandler(GvSumarizacao_RowDataBound);
                ((GridView)Session["GvSumarizacao"]).DataSource = lstItens;
                ((GridView)Session["GvSumarizacao"]).DataBind();
            }
        }
        protected void GvSumarizacao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.RowType.Equals(DataControlRowType.Header))
                {

                }

                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {

                }


            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblModalBody.Text = "This is modal body";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);

        }
        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AppPaginas/Consulta/SimuladorPreco.aspx");
        }
        private void Simular()
        {
            listaRetorno = new List<SimuladorPrecoRetorno>();
            try
            {
                if (ValidarPreencimentoCampos())
                {
                    List<SimuladorPrecoCustosNovo> _lst = BuscarListaItensPorEstabelecimento();
                    if (_lst.Count > 0)
                    {

                        foreach (var item in _lst)
                        {
                            SimuladorPrecoConvenio convenio = new SimuladorPrecoConvenio();
                            convenio.item = item.itemId;
                            convenio.destino = ddlEstadoDestino.SelectedValue;
                            DataTable listaConvenio = convenio.ObterListaConvenio();


                            if (listaConvenio != null)
                            {
                                if (listaConvenio.Rows.Count > 0)
                                {
                                    foreach (DataRow itemConvenio in listaConvenio.AsEnumerable())
                                    {
                                        if (int.Parse(item.estabelecimentoId).Equals(int.Parse(itemConvenio["origem"].ToString())))
                                        {
                                            item.tipo = itemConvenio["convenio"].ToString();
                                        }
                                    }
                                }
                            }
                        }

                        CriarListaRetorno(listaRetorno, _lst);

                        RemoverBloqueio();

                        literarMsgFinal.Text = " \n \n - ARQUIVO GERADO COM SUCESSO!";

                    }
                    else
                    {
                        RemoverBloqueio();
                        PopularModalErro("ESTABELECIMENTO DESTINO INCORRETO");
                    }


                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                PopularModalErro(GetResourceValue(Utility.ERRORMESSAGE));
                throw;
            }
        }
        private void RemoverBloqueio()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "YourUniqueScriptKey", "return DesBloquearTela();", true);
        }
        private List<SimuladorPrecoCustosNovo> BuscarListaItensPorEstabelecimento()
        {
            try
            {
                return new SimuladorPrecoCustosNovo
                {
                    estabelecimentoId = ddlEstabelecimentoId.SelectedValue,
                    _isAdm = UserDataInfo.IsAdm,
                    unidadeNegocioId = UserDataInfo.UserUnidadeNegocio,
                    usuarioId = UserDataInfo.UserID

                }.GetCustosAll(rblPerfilCliente.SelectedValue);

            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                PopularModalErro(GetResourceValue(Utility.ERRORMESSAGE));
                throw;
            }

        }
        private void CriarListaRetorno(List<SimuladorPrecoRetorno> listaRetorno, List<SimuladorPrecoCustosNovo> _lst)
        {
            try
            {
                if (_lst != null && _lst.Count > 0)
                {
                    _lst = _lst.Distinct().Where(f => f.itemId != null || f.itemId != string.Empty).ToList();

                    if (string.IsNullOrEmpty(ddlEstadoDestino.SelectedValue))
                    {
                        foreach (var itemEstado in ListaEstadoDestino())
                        {
                            foreach (var item in _lst)
                            {
                                ddlEstadoDestino.SelectedValue = itemEstado;
                                listaRetorno.Add(GetPercentualMargem(item));
                            }
                        }
                        ddlEstadoDestino.SelectedIndex = 0;
                    }
                    else
                    {
                        foreach (var item in _lst)
                        {
                            listaRetorno.Add(GetPercentualMargem(item));
                        }
                    }
                }
                listaRetorno = listaRetorno.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                PopularModalErro(GetResourceValue(Utility.ERRORMESSAGE));
            }

        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            try
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();

                foreach (PropertyDescriptor prop in properties)
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                return table;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                PopularModalErro(GetResourceValue(Utility.ERRORMESSAGE));
                throw;
            }


        }
        private bool ValidarPreencimentoCampos()
        {
            List<int> count = new List<int>();

            if (!HasItemSelected(rblPerfilCliente.Items))
            {
                //Alert(GetResourceValue("msgInformePerfilCliente"));
                PopularModalErro(GetResourceValue("msgInformePerfilCliente"));
                this.rblPerfilCliente.Focus();
                return false;
            }

            //if (String.IsNullOrEmpty(this.ddlEstadoDestino.SelectedValue))
            //{
            //    PopularModalErro(GetResourceValue("msgInformeEstadoDestino"));
            //    //Alert(GetResourceValue("msgInformeEstadoDestino"));
            //    this.ddlEstadoDestino.Focus();
            //    return false;
            //}

            if (txtDescontoObjetivo.Text == "" && txtMargemObjetivo.Text == "")
            {
                PopularModalErro("Por favor informe o Desconto Objetivo, Margem Objetivo ou Preço Objetivo");
                this.txtDescontoObjetivo.Focus();
                Clean(
                      txtDescontoAdicional,
                      txtDescontoObjetivo,
                      txtMargemObjetivo,
                      rblPerfilCliente);

                return false;
            }

            if (!string.IsNullOrEmpty(txtMargemObjetivo.Text))
            {
                var margemObjetivo = decimal.Parse(txtMargemObjetivo.Text);

                if (margemObjetivo < -5)
                {
                    PopularModalErro("Cálculo não permitido para margem menor que -5%");
                    this.txtMargemObjetivo.Focus();
                    return false;
                }
            }


            return true;
        }
        private void GetItem()
        {
            try
            {

                SimuladorPrecoCustos sPrc =
                    new SimuladorPrecoCustos
                    {
                        //itemId = this.itemID.PadLeft(5, '0')
                    }.GetItem();


                if (sPrc != null)
                {

                    if (sPrc.itemObsoleto)
                    {
                        //Alert(GetResourceValue("msgItemObsoleto"));
                        PopularModalErro(GetResourceValue("msgItemObsoleto"));
                        return;

                    }

                    if (sPrc != null)
                    {
                        #region :: ViewState ::

                        this.CapAplicado = sPrc.capAplicado;

                        #endregion
                    }
                }
                else
                {
                    //Alert(GetResourceValue("msgNenhumRegistro"));
                    PopularModalErro(GetResourceValue("msgNenhumRegistro"));
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                //Alert(Utility.ERRORMESSAGE);
                PopularModalErro(GetResourceValue(Utility.ERRORMESSAGE));
            }
        }
        private void AddScripts()
        {

            AddCurrencyScript(this.txtDescontoAdicional,
                              this.txtMargemObjetivo,


                              this.txtDescontoObjetivo);
        }
        protected SimuladorPrecoRetorno GetPercentualMargem(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                SimuladorPrecoRetorno simuladorPrecoRetorno = new SimuladorPrecoRetorno();



                bool regimeEspecial = ValidaRegimeEspecial(custo.categoria);
                custo.perfilCliente = GetPerfil(rblPerfilCliente.SelectedValue);
                this.itemID = custo.itemId;
                string itemId = custo.itemId;
                decimal vendaLiquidaPE = 0;
                decimal icmsSobreVenda = 0;
                string contribuinte = rblPerfilCliente.SelectedValue;
                string estabelecimentoId = custo.estabelecimentoId;
                string UFDestino = this.ddlEstadoDestino.SelectedValue;
                string UfOrigem = custo.ufIdOrigem;

                PreencherSimuladroRetorno(custo, simuladorPrecoRetorno);

                #region :: Declaração de Auxiliares ::

                decimal _margemObjetivo = decimal.Parse(this.txtMargemObjetivo.Text);
                custo.valorVenda = 0;
                decimal _vlrIcms = 0;

                if (txtMargemObjetivo.Text.Contains("-"))
                {

                    decimal result = 0;

                    if (decimal.TryParse(txtMargemObjetivo.Text.Replace('.', ','), out result))
                    {
                        decimal resultado = result;
                        if (!resultado.Equals(0))
                        {
                            _margemObjetivo = ((result * -1)) * -1;
                        }
                    }
                }

                #endregion
                SimuladorPrecoRegrasGerais _oRg = null;
                SimuladorPrecoRegrasGerais oRg = (new SimuladorPrecoRegrasGerais()
                {
                    estabelecimentoId = custo.estabelecimentoId,
                    convenioId = custo.convenioId,
                    perfilCliente = rblPerfilCliente.SelectedValue,
                    resolucaoId = custo.resolucao13,
                    ufDestino = this.ddlEstadoDestino.SelectedValue
                }).GetRegras();

                List<SimuladorPrecoRegrasGerais> oRgSv = (new SimuladorPrecoRegrasGerais()
                {
                    estabelecimentoId = custo.estabelecimentoId,
                    convenioId = custo.convenioId,
                    perfilCliente = rblPerfilCliente.SelectedValue,
                    resolucaoId = custo.resolucao13,
                    ufDestino = this.ddlEstadoDestino.SelectedValue,
                    usoExclusivoHospitalar = custo.exclusivoHospitalar
                }).GetRegrasICMSVenda();

                if (oRgSv == null)
                {
                    _vlrIcms = new decimal();
                }
                else
                {

                    _oRg = (this.GetValorICMS_ST(custo) <= decimal.Zero ? (
                        from x in oRgSv
                        where !x._icmsStValor.HasValue
                        select x).SingleOrDefault<SimuladorPrecoRegrasGerais>() : (
                        from x in oRgSv
                        where x._icmsStValor.GetValueOrDefault(decimal.MinusOne).Equals(decimal.Zero)
                        select x).SingleOrDefault<SimuladorPrecoRegrasGerais>());
                    icmsSobreVenda = _oRg.icmsSobreVenda;


                    if (custo.itemId.Equals("110219"))
                    {
                        _vlrIcms = this.GetIcmsSobreVendaSPINRAZA(custo.itemId, custo.custoPadrao, custo.valorPisCofins, custo.estabelecimentoId, UFDestino, contribuinte, icmsSobreVenda);
                    }
                    else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !estabelecimentoId.Equals("23")))
                    {


                        if (VerificaseItemTemCalculoST(this.ddlEstadoDestino.SelectedValue, estabelecimentoId, custo.itemId, custo.convenioId, rblPerfilCliente.SelectedValue))
                        {
                            _vlrIcms = BuscaIcmsST(this.ddlEstadoDestino.SelectedValue, estabelecimentoId, custo.itemId, custo.convenioId, rblPerfilCliente.SelectedValue);
                        }
                        else
                        {
                            _vlrIcms = (_oRg.icmsSobreVenda > decimal.One ? _oRg.icmsSobreVenda / new decimal(100) : _oRg.icmsSobreVenda);
                        }
                    }
                    else
                    {
                        _vlrIcms = this.GetIcmsSobreVendaRegraPE(custo);
                    }
                }

                if (this.itemID.PadLeft(5, '0').Equals("110219"))
                {
                    var icmsPercent = GetIcmsSpintPerc(this.itemID.PadLeft(5, '0'), custo.estabelecimentoId, this.ddlEstadoDestino.SelectedValue, icmsSobreVenda, contribuinte);

                    if (icmsPercent.Equals(decimal.Zero))
                    {
                        custo.valorVenda = (custo.custoPadrao / (1 - _margemObjetivo / 100)) / (1 - simuladorPrecoRetorno.PIS_COFINS_PER - GetIcmsSpintPerc(this.itemID.PadLeft(5, '0'), custo.estabelecimentoId, this.ddlEstadoDestino.SelectedValue, icmsSobreVenda, contribuinte));
                        simuladorPrecoRetorno.PRECO_DE_VENDA = custo.valorVenda;
                    }
                    else
                    {
                        var margemValor = ((custo.custoPadrao + _vlrIcms) / (1 - _margemObjetivo / 100)) * (_margemObjetivo / 100);
                        custo.valorVenda = custo.custoPadrao + _vlrIcms + margemValor;
                        simuladorPrecoRetorno.PRECO_DE_VENDA = custo.valorVenda;
                    }

                }
                else if (this.ddlEstadoDestino.SelectedValue.Equals("PE") && custo.estabelecimentoId.Equals("23"))
                {

                    var margemValor = (custo.custoPadrao / (1 - _margemObjetivo / 100)) * _margemObjetivo / 100;

                    custo.valorVenda = custo.custoPadrao + _vlrIcms + margemValor;
                    simuladorPrecoRetorno.PRECO_DE_VENDA = custo.valorVenda;

                    simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = (1 - (custo.custoPadrao / (custo.valorVenda - _vlrIcms))) * 100;
                }
                else
                {
                    if (simuladorPrecoRetorno.PIS_COFINS_PER.Equals(decimal.Zero))
                    {
                        custo.valorVenda = custo.custoPadrao / (1 - ((_margemObjetivo / 100) + _vlrIcms + simuladorPrecoRetorno.PIS_COFINS_PER));

                        simuladorPrecoRetorno.PRECO_DE_VENDA = custo.valorVenda;
                    }
                    else
                    {

                        custo.valorVenda = custo.custoPadrao / (1 - ((_margemObjetivo / 100) + _vlrIcms + simuladorPrecoRetorno.PIS_COFINS_PER));

                        simuladorPrecoRetorno.PRECO_DE_VENDA = custo.valorVenda;
                    }
                }

                SimuladorRegrasST RegraST = new SimuladorRegrasST()
                {
                    itemId = custo.itemId.PadLeft(5, '0'),
                    estadoDestino = this.ddlEstadoDestino.SelectedValue,
                    estabelecimentoId = custo.estabelecimentoId,
                    classeFiscal = custo.tipo,
                    perfilCliente = contribuinte
                };
                DataTable dtRegraST = new DataTable();
                if (regimeEspecial)
                {
                    dtRegraST = RegraST.ObterTodosComFiltro();
                }

                if (dtRegraST != null)
                {
                    if (dtRegraST.Rows.Count > 0)
                    {
                        decimal aliquota = !string.IsNullOrEmpty(dtRegraST.Rows[0]["aliquota"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["aliquota"].ToString()) : 0;
                        decimal PMC = !string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC"].ToString()) : 0;
                        decimal PMC_Cheio = !string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) : 0;
                        decimal icmsST = !string.IsNullOrEmpty(dtRegraST.Rows[0]["icmsInterno"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["icmsInterno"].ToString()) : 0;
                        string baseValor = string.Empty;
                        bool verificaSeAssumeBase = false;

                        simuladorPrecoRetorno.ICMS_ST = icmsST;
                        simuladorPrecoRetorno.PMC = PMC;

                        #region Definir BaseST

                        if (estabelecimentoId == "3")
                        {
                            var novaBaseSt = CalulaNovaBase(custo.categoria, "", PMC, custo.valorVenda, PMC_Cheio);
                            if (!string.IsNullOrEmpty(novaBaseSt))
                            {
                                baseValor = novaBaseSt.Split(';')[1].ToString();

                                if (novaBaseSt.Split(';')[0].ToString().Equals("Base - 1"))
                                {
                                    verificaSeAssumeBase = true;
                                }
                                else if (novaBaseSt.Split(';')[0].ToString().Equals("Base ST"))
                                {
                                    verificaSeAssumeBase = false;
                                }
                                else
                                {
                                    verificaSeAssumeBase = true;
                                }
                            }
                        }
                        #endregion

                        icmsST = (icmsST > 1 ? (icmsST / 100) : icmsST);

                        if (this.itemID.PadLeft(5, '0').Equals("110219"))
                        {
                            simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = GetPrecoVendaComSTSPIN(custo.valorVenda, icmsST, icmsST, aliquota, PMC, _vlrIcms);

                            simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA + custo.valorVenda;

                            if (custo.percPisCofins > 0)
                            {
                                vendaLiquidaPE = (custo.custoPadrao / (1 - _margemObjetivo / 100) / (1 - custo.percPisCofins)) * custo.percPisCofins;
                            }
                            else
                            {
                                vendaLiquidaPE = ((custo.custoPadrao + _vlrIcms) / (1 - _margemObjetivo / 100)) - _vlrIcms;//(custo.custoPadrao / (1 - _margemObjetivo / 100));
                            }

                            simuladorPrecoRetorno.PIS_COFINS_VAL = (custo.custoPadrao / (1 - _margemObjetivo / 100) / (1 - custo.percPisCofins)) * custo.percPisCofins;
                            simuladorPrecoRetorno.MARGEM_OBJETIVO_VAL = (vendaLiquidaPE - custo.custoPadrao);

                            simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((1 - (custo.custoPadrao + _vlrIcms) / simuladorPrecoRetorno.PRECO_DE_VENDA) * 100) * 1;

                        }
                        else if (this.ddlEstadoDestino.SelectedValue.Equals("PE") && custo.estabelecimentoId.Equals("23"))
                        {
                            if (rblPerfilCliente.SelectedValue.Equals("C"))
                            {
                                simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = GetICMS_STContribuinte(custo.valorVenda, aliquota, PMC, icmsST);

                                simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA + custo.valorVenda;

                                if (custo.percPisCofins > 0)
                                {
                                    vendaLiquidaPE = (custo.custoPadrao / (1 - _margemObjetivo / 100) / (1 - custo.percPisCofins)) * custo.percPisCofins;
                                }
                                else
                                {
                                    vendaLiquidaPE = (custo.custoPadrao / (1 - _margemObjetivo / 100));
                                }
                                simuladorPrecoRetorno.MARGEM_OBJETIVO_VAL = vendaLiquidaPE - custo.custoPadrao;
                            }
                            else
                            {
                                simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = GetPrecoVendaComSTPE(custo.valorVenda, icmsST, icmsST, aliquota, PMC);

                                if (custo.percPisCofins > 0)
                                {
                                    vendaLiquidaPE = (custo.custoPadrao / (1 - _margemObjetivo / 100) / (1 - custo.percPisCofins)) * custo.percPisCofins;
                                }
                                else
                                {
                                    vendaLiquidaPE = (custo.custoPadrao / (1 - _margemObjetivo / 100));
                                }

                                simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = (custo.valorVenda * (1 + aliquota)) * icmsST;

                                simuladorPrecoRetorno.MARGEM_OBJETIVO_VAL = vendaLiquidaPE - custo.custoPadrao;

                            }
                            simuladorPrecoRetorno.PIS_COFINS_VAL = (custo.custoPadrao / (1 - _margemObjetivo / 100) / (1 - custo.percPisCofins)) * custo.percPisCofins;
                        }
                        else
                        {

                            decimal ValorIcmsInterno = 0;
                            decimal ValorIcmsSTSobreVenda = 0;

                            custo.valorVenda = custo.custoPadrao / (1 - ((_margemObjetivo / 100) + icmsST + simuladorPrecoRetorno.PIS_COFINS_PER));

                            simuladorPrecoRetorno.ICMS_SOBRE_VENDA = GetValorIcmsSobreVenda(custo.valorVenda, icmsST);
                            simuladorPrecoRetorno.PRECO_DE_VENDA = custo.valorVenda;
                            #region VERIFICA SE ASSUME BASE TS

                            if (verificaSeAssumeBase)
                            {
                                ValorIcmsInterno = GetValorIcmsInterno(decimal.Parse(baseValor), icmsST);

                                ValorIcmsSTSobreVenda = ValorIcmsInterno - simuladorPrecoRetorno.ICMS_SOBRE_VENDA;
                                simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = custo.valorVenda + ValorIcmsSTSobreVenda;
                                simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = ValorIcmsSTSobreVenda;
                            }
                            else
                            {
                                ValorIcmsInterno = GetValorIcmsInterno(GetValorBaseST(custo.valorVenda, aliquota, PMC), icmsST);

                                ValorIcmsSTSobreVenda = ValorIcmsInterno - simuladorPrecoRetorno.ICMS_SOBRE_VENDA;
                                simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = GetPrecoVendaComST(custo.valorVenda, icmsST, icmsST, aliquota, PMC, 0);
                                simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = ValorIcmsSTSobreVenda;


                            }
                            #endregion


                            decimal vendaLiquida = 0;

                            if (!simuladorPrecoRetorno.PIS_COFINS_PER.Equals(decimal.Zero))
                            {
                                vendaLiquida = simuladorPrecoRetorno.CUSTO_PADRAO / ((simuladorPrecoRetorno.PIS_COFINS_PER / 100) + (1 - _margemObjetivo / 100)) + (simuladorPrecoRetorno.ICMS_SOBRE_VENDA / 100);

                                simuladorPrecoRetorno.MARGEM_OBJETIVO_VAL = vendaLiquida - simuladorPrecoRetorno.CUSTO_PADRAO;
                                simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = ValorIcmsSTSobreVenda;
                                //  simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((vendaLiquida) - (simuladorPrecoRetorno.CUSTO_PADRAO)) / vendaLiquida;


                                var icmsSobreVendaNv = ((simuladorPrecoRetorno.PRECO_DE_VENDA * _vlrIcms) + (simuladorPrecoRetorno.PRECO_DE_VENDA * simuladorPrecoRetorno.PIS_COFINS_PER));
                                var custoCalculado = (simuladorPrecoRetorno.CUSTO_PADRAO + icmsSobreVendaNv);
                                simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((1 - (custoCalculado / simuladorPrecoRetorno.PRECO_DE_VENDA)) * 100) * 1;

                            }
                            else
                            {
                                vendaLiquida = (simuladorPrecoRetorno.PRECO_DE_VENDA - simuladorPrecoRetorno.ICMS_SOBRE_VENDA);

                                simuladorPrecoRetorno.MARGEM_OBJETIVO_VAL = vendaLiquida - simuladorPrecoRetorno.CUSTO_PADRAO;
                                simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = ValorIcmsSTSobreVenda;
                                //    simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((vendaLiquida) - (simuladorPrecoRetorno.CUSTO_PADRAO)) / vendaLiquida;


                                var icmsSobreVendaNv = ((simuladorPrecoRetorno.PRECO_DE_VENDA * _vlrIcms) + (simuladorPrecoRetorno.PRECO_DE_VENDA * simuladorPrecoRetorno.PIS_COFINS_PER));
                                var custoCalculado = (simuladorPrecoRetorno.CUSTO_PADRAO + icmsSobreVendaNv);
                                simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((1 - (custoCalculado / simuladorPrecoRetorno.PRECO_DE_VENDA)) * 100) * 1;
                            }



                        }
                    }
                    else
                    {

                        if (this.itemID.PadLeft(5, '0').Equals("110219"))
                        {

                            if (simuladorPrecoRetorno.PIS_COFINS_PER < 0)
                            {
                                vendaLiquidaPE = (custo.valorVenda - simuladorPrecoRetorno.ICMS_SOBRE_VENDA / (1 - simuladorPrecoRetorno.PIS_COFINS_PER)) * simuladorPrecoRetorno.PIS_COFINS_PER;

                            }
                            else
                            {
                                vendaLiquidaPE = (custo.valorVenda - simuladorPrecoRetorno.ICMS_SOBRE_VENDA / (1 - simuladorPrecoRetorno.PIS_COFINS_PER));

                            }


                            simuladorPrecoRetorno.PIS_COFINS_VAL = (custo.custoPadrao / (1 - _margemObjetivo / 100))
                                / (1 - custo.percPisCofins -
                                GetIcmsSpintPerc(this.itemID.PadLeft(5, '0'), custo.estabelecimentoId, this.ddlEstadoDestino.SelectedValue, icmsSobreVenda, contribuinte)) * custo.percPisCofins;
                            simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = 0;
                            simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = 0;

                            simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = (1 - (custo.custoPadrao + _vlrIcms) / simuladorPrecoRetorno.PRECO_DE_VENDA) * 100;

                        }
                        else if (this.ddlEstadoDestino.SelectedValue.Equals("PE") && custo.estabelecimentoId.Equals("23"))
                        {
                            vendaLiquidaPE = simuladorPrecoRetorno.CUSTO_PADRAO / ((simuladorPrecoRetorno.PIS_COFINS_PER / 100) + (1 - _margemObjetivo / 100)) + (simuladorPrecoRetorno.ICMS_SOBRE_VENDA / 100);

                            simuladorPrecoRetorno.PIS_COFINS_VAL = ((GetValorCustoPadrao(custo))
                                                / (1 - decimal.Parse(txtMargemObjetivo.Text) / 100) / (1 - custo.percPisCofins)) * custo.percPisCofins;
                            simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = 0;
                            simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = 0;

                            //simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = vendaLiquidaPE - simuladorPrecoRetorno.CUSTO_PADRAO;

                            var icmsSobreVendaNv = ((simuladorPrecoRetorno.PRECO_DE_VENDA * _vlrIcms) + simuladorPrecoRetorno.PIS_COFINS_PER);
                            var custoCalculado = (simuladorPrecoRetorno.CUSTO_PADRAO + icmsSobreVendaNv);
                            simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((1 - (custoCalculado / simuladorPrecoRetorno.PRECO_DE_VENDA)) * 100) * 1;

                        }
                        else
                        {



                            var icmsSobreVendaNv = ((simuladorPrecoRetorno.PRECO_DE_VENDA * _vlrIcms) + simuladorPrecoRetorno.PIS_COFINS_PER);
                            simuladorPrecoRetorno.ICMS_SOBRE_VENDA = icmsSobreVendaNv;

                            decimal vendaLiquida = simuladorPrecoRetorno.PRECO_DE_VENDA - (simuladorPrecoRetorno.ICMS_SOBRE_VENDA);


                            simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = 0;

                            simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = 0;
                            simuladorPrecoRetorno.MARGEM_OBJETIVO_VAL = vendaLiquida - simuladorPrecoRetorno.CUSTO_PADRAO;
                            //    simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((vendaLiquida) - (simuladorPrecoRetorno.CUSTO_PADRAO)) / vendaLiquida;



                            var custoCalculado = (simuladorPrecoRetorno.CUSTO_PADRAO + icmsSobreVendaNv);
                            simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((1 - (custoCalculado / simuladorPrecoRetorno.PRECO_DE_VENDA)) * 100) * 1;


                        }
                    }

                }





                else
                {

                    simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = 0;
                    simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = 0;
                    //simuladorPrecoRetorno.MARGEM_OBJETIVO_PER =

                    //          (
                    //                (
                    //                    (GetPrecoVendaLiquidoMemoria(_oRg, custo))
                    //                        -
                    //                    (GetValorCustoPadrao(custo))
                    //                )
                    //                    /
                    //                (GetPrecoVendaLiquidoMemoria(_oRg, custo))
                    //            );



                    var icmsSobreVendaNv = ((simuladorPrecoRetorno.PRECO_DE_VENDA * _vlrIcms) + simuladorPrecoRetorno.PIS_COFINS_PER);
                    var custoCalculado = (simuladorPrecoRetorno.CUSTO_PADRAO + icmsSobreVendaNv);
                    simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = ((1 - (custoCalculado / simuladorPrecoRetorno.PRECO_DE_VENDA)) * 100) * 1;
                }

                simuladorPrecoRetorno.TRANSF_ES =
                                     rblPerfilCliente.SelectedValue.Equals("C") || rblPerfilCliente.SelectedValue.Equals("2") ?
                                             decimal.Parse(custo.transfDF) > 0 ?
                                                "DF_" + decimal.Parse(custo.transfDF).ToString() :
                                                "ES_" + decimal.Parse(custo.transfEs).ToString() :
                                                "ES_" + decimal.Parse(custo.transfEs).ToString();



                return simuladorPrecoRetorno;

            }
            catch (Exception ex)
            {

                Utility.WriteLog(ex);
                return null;
            }


        }
        private void PreencherSimuladroRetorno(SimuladorPrecoCustosNovo custo, SimuladorPrecoRetorno simuladorPrecoRetorno)
        {
            simuladorPrecoRetorno.ESTABELECIMENTO_DESTINO = custo.estabelecimentoId;
            simuladorPrecoRetorno.MARGEM_OBJETIVO_PER = 0;
            simuladorPrecoRetorno.CATEGORIA = custo.categoria;
            simuladorPrecoRetorno.CLASS_FISCAL = custo.tipo;
            simuladorPrecoRetorno.FORNECEDOR = custo.laboratorioNome;
            simuladorPrecoRetorno.LISTA = custo.listaDescricao;
            simuladorPrecoRetorno.PRECO_DE_VENDA_COM_ST = 0;
            simuladorPrecoRetorno.ICMS_ST_SOBRE_VENDA = 0;
            simuladorPrecoRetorno.ITEM = custo.itemId;
            simuladorPrecoRetorno.DESCRICAO = custo.itemDescricao;
            simuladorPrecoRetorno.UF_ORIGEM = custo.ufIdOrigem;
            simuladorPrecoRetorno.NCM = custo.NCM;
            simuladorPrecoRetorno.MED_CONTROLADO_USO = custo.itemControlado ? "SIM" : "NAO";
            simuladorPrecoRetorno.EX_HOSPITALAR = custo.exclusivoHospitalar;
            simuladorPrecoRetorno.RESOLUCAO_13 = custo.resolucao13;
            simuladorPrecoRetorno.PERFIL_DE_CLIENTE = custo.perfilCliente;
            simuladorPrecoRetorno.ESTADO_DESTINO = ddlEstadoDestino.SelectedValue;
            simuladorPrecoRetorno.DESCONTO_ADICIONAL = custo.descontoAdicional.ToString();
            simuladorPrecoRetorno.REPASSE = custo.percRepasse.ToString();
            simuladorPrecoRetorno.IPI_VAL = custo.percIPI;
            simuladorPrecoRetorno.TRANSF_ES = custo.transfEs.ToString();
            simuladorPrecoRetorno.PIS_COFINS_PER = (custo.percPisCofins > decimal.One ? (custo.percPisCofins / 100) : custo.percPisCofins);

            custo.custoPadrao = GetValorCustoPadrao(custo);

            simuladorPrecoRetorno.CUSTO_PADRAO = custo.custoPadrao;
            simuladorPrecoRetorno.PRECO_FABRICA = custo.precoFabrica;
            simuladorPrecoRetorno.DESCONTO_COMERCIAL = custo.descontoComercial;
            simuladorPrecoRetorno.PIS_COFINS_VAL = custo.valorPisCofins;
            simuladorPrecoRetorno.MVA = custo.mva;
            simuladorPrecoRetorno.HORA_PROCESS = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            simuladorPrecoRetorno.CREDITO_ICMS_VAL = custo.valorCreditoICMS;
            simuladorPrecoRetorno.CREDITO_ICMS_PER = custo.percICMSe;



        }
        protected decimal GetDescontoAdicional(decimal _descontoAdicional)
        {
            try
            {
                decimal _descontoCap = new decimal();

                if (IsSetorPublico)
                    _descontoCap =
                        !CapAplicado ? new decimal() :
                            !String.IsNullOrEmpty("") ?
                                decimal.Parse("") : new decimal();

                return ((_descontoAdicional > 1 ? _descontoAdicional : (_descontoAdicional * 100)) + ((_descontoCap > 1 ? _descontoCap : (_descontoCap * 100))));
                //return ((_descontoAdicional > 1 ? _descontoAdicional : (_descontoAdicional * 100)) + (_descontoCap / 100));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected string GetPerfil(string _perfil)
        {
            switch (this.rblPerfilCliente.SelectedValue.ToUpper())
            {
                case "C":
                    return "contribuinte";

                default:
                    return "não contribuinte";
            }
        }
        protected decimal GetPrecoVenda(string _valorVenda)
        {
            try
            {
                if (!String.IsNullOrEmpty(_valorVenda))
                    return decimal.Parse(_valorVenda);

                return 0;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        public decimal GetIcmsSobreVendaSPINRAZA(string ItemId, decimal custoPadrao, decimal pisCofins, string estabelecimentoId, string ufDestino, string contribuinte, decimal icms)
        {
            decimal icmsSobreCusto = 0;
            try
            {
                SimuladorPrecoRegraPE regraSperaza = new SimuladorPrecoRegraPE();
                regraSperaza.codigoItem = ItemId;
                regraSperaza.estabelecimentoId = estabelecimentoId;
                regraSperaza.ufDestinoCliente = ufDestino;
                regraSperaza.contribuinte = contribuinte;
                DataTable dt = regraSperaza.ObterRegrasSPINRAZA();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        decimal icmsSpin = !string.IsNullOrEmpty(dt.Rows[0]["encargos"].ToString()) ? decimal.Parse(dt.Rows[0]["encargos"].ToString()) : 0;

                        icmsSobreCusto = (custoPadrao * icmsSpin);
                    }
                    else
                    {
                        icmsSobreCusto = (custoPadrao / (1 - ((decimal.Parse(txtMargemObjetivo.Text) / 100) + icms + pisCofins))) * icms;
                    }
                }
                else
                {
                    icmsSobreCusto = (custoPadrao / (1 - ((decimal.Parse(txtMargemObjetivo.Text) / 100) + icms + pisCofins))) * icms;
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }

            return icmsSobreCusto;
        }
        public decimal GetIcmsSpintPerc(string ItemId, string establelecimentoId, string ufDestino, decimal icms, string contribuinte)
        {
            decimal IcmsSobrePrecoAquisicao = 0;
            try
            {
                SimuladorPrecoRegraPE regraSperaza = new SimuladorPrecoRegraPE();
                regraSperaza.codigoItem = ItemId;
                regraSperaza.estabelecimentoId = establelecimentoId;
                regraSperaza.ufDestinoCliente = ufDestino;
                regraSperaza.contribuinte = contribuinte;
                DataTable dt = regraSperaza.ObterRegrasSPINRAZA();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        IcmsSobrePrecoAquisicao = !string.IsNullOrEmpty(dt.Rows[0]["encargos"].ToString()) ? decimal.Parse(dt.Rows[0]["encargos"].ToString()) : 0;
                    }
                    else
                    {
                        IcmsSobrePrecoAquisicao = icms;
                    }
                }
                else
                {
                    IcmsSobrePrecoAquisicao = icms;
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }

            return IcmsSobrePrecoAquisicao;
        }
        private decimal GetICMS_STContribuinte(decimal vlrVenda, decimal aliquotaBaseST, decimal PMC, decimal icmsST)
        {

            if (PMC > 0)
            {
                return (PMC * icmsST) - (vlrVenda * icmsST);
            }
            else
            {
                //Aliquota = aliquotaBaseST > 1 ? (aliquotaBaseST / 100) : aliquotaBaseST;
                return ((vlrVenda * (1 + aliquotaBaseST / 100)) * icmsST) - (vlrVenda * icmsST);
            }
        }
        private decimal GetValorBaseST(decimal vlrVenda, decimal aliquotaBaseST, decimal PMC)
        {

            if (PMC > 0)
            {
                return PMC;
            }
            else
            {
                //Aliquota = aliquotaBaseST > 1 ? (aliquotaBaseST / 100) : aliquotaBaseST;
                return vlrVenda * (1 + aliquotaBaseST);
            }
        }
        private decimal GetPrecoVendaComSTPE(decimal _vlrVenda, decimal _vlrIcms, decimal IcmsInterno, decimal aliquotaBaseST, decimal PMC)
        {
            decimal ValorBaseST = GetValorBaseST(_vlrVenda, aliquotaBaseST, PMC);
            decimal ValorIcmsInterno = GetValorIcmsInterno(ValorBaseST, IcmsInterno);
            return ValorIcmsInterno + _vlrVenda;
        }
        private decimal GetPrecoVendaComSTSPIN(decimal _vlrVenda, decimal _vlrIcms, decimal IcmsInterno, decimal aliquotaBaseST, decimal PMC, decimal icmsSpin)
        {
            decimal ValorBaseST = GetValorBaseST(_vlrVenda, aliquotaBaseST, PMC);
            decimal ValorIcmsInterno = GetValorIcmsInterno(ValorBaseST, IcmsInterno);
            return ValorIcmsInterno - icmsSpin;
        }
        private decimal GetPrecoVendaComST(decimal _vlrVenda, decimal _vlrIcms, decimal IcmsInterno, decimal aliquotaBaseST, decimal PMC, decimal Base_ST)
        {
            decimal ValorBaseST = 0;
            if (Base_ST <= 0)
            {
                ValorBaseST = GetValorBaseST(_vlrVenda, aliquotaBaseST, PMC);
            }
            else
            {
                ValorBaseST = Base_ST;
            }
            //ValorBaseST = GetValorBaseST(_vlrVenda, aliquotaBaseST, PMC);

            decimal ValorIcmsInterno = GetValorIcmsInterno(ValorBaseST, IcmsInterno);
            decimal ValorIcmsSobreVenda = GetValorIcmsSobreVenda(_vlrVenda, _vlrIcms);
            decimal ValorIcmsSTSobreVenda = ValorIcmsInterno - ValorIcmsSobreVenda;

            //decimal Icms = IcmsInterno > 1 ? (IcmsInterno / 100) : IcmsInterno;
            return _vlrVenda + ValorIcmsSTSobreVenda;
        }
        private decimal GetValorIcmsInterno(decimal ValorBaseST, decimal IcmsInterno)
        {

            decimal Icms = IcmsInterno > 1 ? (IcmsInterno / 100) : IcmsInterno;

            return (ValorBaseST * Icms);

        }
        protected bool ValidaRegimeEspecial(string _categoria)
        {
            if (ckeckCalculaST.Checked)
            {
                switch (_categoria)
                {
                    case "GENERICO":
                        return false;
                    case "REFERENCIA":
                        return false;
                    case "SIMILAR":
                        return false;
                    case "VACINAS":
                        return false;
                    case "OUTROS":
                        return false;
                    default:
                        return true;
                }
            }
            else return true;
        }
        public decimal GetIcmsSobreVendaRegraPE(SimuladorPrecoCustosNovo custo)
        {
            decimal IcmsSobrePrecoAquisicao = 0;
            try
            {
                SimuladorPrecoRegraPE RegraPE = new SimuladorPrecoRegraPE();
                RegraPE.codigoItem = custo.itemId;
                RegraPE.contribuinte = rblPerfilCliente.SelectedValue;
                RegraPE.estabelecimentoId = custo.estabelecimentoId;
                RegraPE.uFOrigemFornec = custo.ufIdOrigem;
                RegraPE.ufDestinoCliente = ddlEstadoDestino.SelectedValue;

                DataTable dt = RegraPE.ObterTodosFiltro();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        decimal Encargos = !string.IsNullOrEmpty(dt.Rows[0]["encargos"].ToString()) ? decimal.Parse(dt.Rows[0]["encargos"].ToString()) : 0;
                        IcmsSobrePrecoAquisicao = Encargos * custo.precoAquisicao;

                    }
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }

            return IcmsSobrePrecoAquisicao;
        }
        protected decimal GetPISCofinsSobreVenda(string _valorVenda, decimal _percPisCofins)
        {
            try
            {
                return (
                        (_percPisCofins > 1 ? (_percPisCofins / 100) : _percPisCofins)
                            *
                        GetPrecoVenda(_valorVenda)
                       );
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetAjusteRegimeFiscalSobreVenda(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                SimuladorPrecoRegrasGerais oRg =
                    new SimuladorPrecoRegrasGerais
                    {
                        estabelecimentoId = custo.estabelecimentoId,
                        convenioId = Utility.FormataStringPesquisa(custo.convenioId),
                        perfilCliente = custo.perfilCliente,
                        resolucaoId = Utility.FormataStringPesquisa(custo.resolucao13),
                        ufDestino = this.ddlEstadoDestino.SelectedValue,
                        usoExclusivoHospitalar = Utility.FormataStringPesquisa(custo.exclusivoHospitalar)

                    }.GetRegras();



                if (oRg != null)
                {
                    switch (GetEquality(custo.tipo.Trim(), "DERMOCOSMETICOS", "REGIME NORMAL"))
                    {
                        case "DERMOCOSMETICOS":
                        case "REGIME NORMAL":

                            return
                                    !custo.estabelecimentoId.Equals("11") ?

                                    (
                                       //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                       GetPrecoVenda(custo.valorVenda.ToString())

                                            *
                                        //(Utility.VLR_BASEAJUSTEREGIMEFISCAL)
                                        //  *
                                        (oRg.ajusteRegimeFiscal > 1 ? (oRg.ajusteRegimeFiscal / 100) : oRg.ajusteRegimeFiscal)
                                    )
                                        :
                                    (
                                        (
                                            //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                            GetPrecoVenda(custo.valorVenda.ToString())
                                                *
                                            //(Utility.VLR_BASEAJUSTEREGIMEFISCAL)
                                            //   *
                                            (oRg.ajusteRegimeFiscal > 1 ? (oRg.ajusteRegimeFiscal / 100) : oRg.ajusteRegimeFiscal)
                                        )
                                            *
                                        (-1)
                                    );
                        default:
                            return ((oRg.ajusteRegimeFiscal > 1 ? (oRg.ajusteRegimeFiscal / 100) : oRg.ajusteRegimeFiscal) * GetPrecoVenda(custo.valorVenda.ToString())); //return 0;
                    }
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetICMSSTSobreVenda(SimuladorPrecoCustosNovo custo, SimuladorPrecoRegrasGerais _oRg)
        {
            try
            {
                SimuladorPrecoRegrasGerais oRg = null;

                if (!custo.hasData)
                {
                    oRg =
                        new SimuladorPrecoRegrasGerais
                        {
                            estabelecimentoId = custo.estabelecimentoId,
                            convenioId = Utility.FormataStringPesquisa(custo.convenioId),
                            perfilCliente = custo.perfilCliente,
                            resolucaoId = Utility.FormataStringPesquisa(custo.resolucao13),
                            ufDestino = this.ddlEstadoDestino.SelectedValue

                        }.GetRegras();
                }
                else
                    oRg = _oRg;

                if (oRg != null)
                {
                    #region :: Estabelecimento 02 ::

                    if (custo.estabelecimentoId.PadLeft(2, '0').Equals("02"))
                    {
                        switch (this.ddlEstadoDestino.SelectedValue.ToUpper())
                        {
                            case "RS":

                                if (IsEquals(custo.convenioId.Trim(), "REGIME NORMAL") &&
                                       IsEquals(GetPerfil(custo.perfilCliente.Trim()), "CONTRIBUINTE"))
                                {
                                    #region :: PMC17 > 0 ::

                                    if (custo.pmc17 > 0)
                                    {
                                        switch (GetEquality(custo.categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                        {
                                            case "SIMILAR":

                                                if (oRg.icmsStSobreVenda != 0)
                                                {
                                                    return (
                                                            (custo.pmc17
                                                                *
                                                             Utility.VLR_STRSSIMILAR
                                                                *
                                                             (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                -
                                                            GetICMSSobreVenda(custo)
                                                           );
                                                }
                                                else
                                                    return oRg.icmsStSobreVenda;
                                            case "GENÉRICO":
                                                if (oRg.icmsStSobreVenda != 0)
                                                {
                                                    return (
                                                            (custo.pmc17
                                                                *
                                                             Utility.VLR_STRSGENERICO
                                                                *
                                                             (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                -
                                                              GetICMSSobreVenda(custo)
                                                           );
                                                }
                                                else
                                                    return oRg.icmsStSobreVenda;
                                            default:
                                                if (oRg.icmsStSobreVenda != 0)
                                                {
                                                    return (
                                                            (custo.pmc17
                                                                *
                                                             Utility.VLR_STRSNAOSIMILAR
                                                                *
                                                             (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                -
                                                             GetICMSSobreVenda(custo)
                                                           );
                                                }
                                                else
                                                    return oRg.icmsStSobreVenda;

                                        }
                                    }

                                    #endregion

                                    #region :: PMC17 = 0 ::

                                    else
                                    {
                                        #region :: Positiva ::

                                        if (IsEquals(custo.listaDescricao.Trim(), "POSITIVA"))
                                        {
                                            switch (GetEquality(custo.categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        decimal valor1 = 0;

                                                        valor1 = (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVASIMILAR
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVASIMILARPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                );

                                                        decimal icmsSobVenda = GetICMSSobreVenda(custo);
                                                        return (
                                                               valor1
                                                                        -
                                                                   icmsSobVenda
                                                               );
                                                    }
                                                    else
                                                        return oRg.icmsStSobreVenda;

                                                case "GENÉRICO":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVAGENERICO
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVAGENERICOPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                       GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else
                                                        return oRg.icmsStSobreVenda;

                                                default:
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {

                                                        decimal valor = (
                                                                 (
                                                                     //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                     GetPrecoVenda(custo.valorVenda.ToString())
                                                                         *
                                                                     Utility.VLR_STRSPOSITIVAOUTROS
                                                                         *
                                                                     Utility.VLR_STRSPOSITIVAOUTROSPRC
                                                                         *
                                                                     (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                 )
                                                                         -
                                                                       GetICMSSobreVenda(custo)
                                                                );


                                                        return valor;
                                                    }

                                                    else
                                                        return oRg.icmsStSobreVenda;



                                            }
                                        }

                                        #endregion

                                        #region :: Negativa ::

                                        else if (IsEquals(custo.listaDescricao.Trim(), "NEGATIVA"))
                                        {
                                            switch (GetEquality(custo.categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {

                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVASIMILAR
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVASIMILARPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                      GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else
                                                        return oRg.icmsStSobreVenda;

                                                case "GENÉRICO":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAGENERICO
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAGENERICOPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                   GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else
                                                        return oRg.icmsStSobreVenda;

                                                default:
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAOUTROS
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAOUTROSPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                  GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else return oRg.icmsStSobreVenda;
                                            }
                                        }

                                        #endregion

                                        #region :: Neutra ::

                                        else if (IsEquals(custo.listaDescricao.Trim(), "NEUTRA"))
                                        {
                                            switch (GetEquality(custo.categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    return (
                                                            (
                                                                //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                GetPrecoVenda(custo.valorVenda.ToString())
                                                                    *
                                                                Utility.VLR_STRSNEUTRASIMILAR
                                                                    *
                                                                Utility.VLR_STRSNEUTRASIMILARPRC
                                                                    *
                                                                (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                    -
                                                                GetICMSSobreVenda(custo)
                                                           );

                                                case "GENÉRICO":
                                                    return (
                                                            (
                                                                //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                GetPrecoVenda(custo.valorVenda.ToString())
                                                                    *
                                                                Utility.VLR_STRSNEUTRAGENERICO
                                                                    *
                                                                Utility.VLR_STRSNEUTRAGENERICOPRC
                                                                    *
                                                                (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                    -
                                                                  GetICMSSobreVenda(custo)
                                                           );

                                                default:
                                                    return (
                                                            (
                                                                //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                GetPrecoVenda(custo.valorVenda.ToString())
                                                                    *
                                                                Utility.VLR_STRSNEUTRAOUTROS
                                                                    *
                                                                Utility.VLR_STRSNEUTRAOUTROSPRC
                                                                    *
                                                                (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                    -
                                                                  GetICMSSobreVenda(custo)
                                                           );
                                            }
                                        }

                                        #endregion

                                        #region :: Sem Lista ::

                                        else
                                        {
                                            switch (GetEquality(custo.categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTASIMILAR
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTASIMILARPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                   GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else return oRg.icmsStSobreVenda;

                                                case "GENÉRICO":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAGENERICO
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAGENERICOPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                      GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else
                                                        return oRg.icmsStSobreVenda;
                                                default:

                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {

                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(custo.valorVenda.ToString())
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAOUTROS
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAOUTROSPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(custo)
                                                               );
                                                    }
                                                    else
                                                        return oRg.icmsStSobreVenda;

                                            }
                                        }

                                        #endregion
                                    }

                                    #endregion
                                }
                                else
                                    return new decimal();

                            default:
                                return new decimal();
                        }
                    }

                    #endregion

                    #region :: Estabelecimento 31 ::

                    else if (custo.estabelecimentoId.Equals("31"))
                    {
                        if (custo.embutirICMSST)
                        {
                            switch (this.ddlEstadoDestino.SelectedValue.ToUpper())
                            {
                                case "RJ":

                                    if ((IsEquals(custo.convenioId.Trim(), "REGIME NORMAL", "DERMOCOSMÉTICOS"))
                                                &&
                                            IsEquals(GetPerfil(custo.perfilCliente.Trim()), "CONTRIBUINTE"))
                                    {
                                        if (IsEquals(custo.listaDescricao.Trim(), "POSITIVA"))

                                            if (oRg.icmsStSobreVenda != 0)
                                            {
                                                return (
                                                        (

                                                         GetPrecoVenda(custo.valorVenda.ToString())
                                                            *
                                                         Utility.VLR_STRJLISTAPOSITIVA
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                         GetICMSSobreVenda(custo)
                                                       );
                                            }
                                            else
                                                return oRg.icmsStSobreVenda;

                                        else if (IsEquals(custo.listaDescricao.Trim(), "NEGATIVA"))
                                            if (oRg.icmsStSobreVenda != 0)
                                            {

                                                return (
                                                        (
                                                         //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                         GetPrecoVenda(custo.valorVenda.ToString())
                                                            *
                                                         Utility.VLR_STRJLISTANEGATIVA
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                       GetICMSSobreVenda(custo)
                                                       );
                                            }
                                            else
                                                return oRg.icmsStSobreVenda;

                                        else if (IsEquals(custo.listaDescricao.Trim(), "NEUTRA"))
                                            if (oRg.icmsStSobreVenda != 0)
                                            {
                                                return (
                                                        (
                                                         GetPrecoVenda(custo.valorVenda.ToString())
                                                            *
                                                         Utility.VLR_STRJLISTANEUTRA
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                       GetICMSSobreVenda(custo)
                                                       );
                                            }
                                            else
                                                return oRg.icmsStSobreVenda;

                                        else
                                        {
                                            if (oRg.icmsStSobreVenda != 0)
                                            {
                                                return (
                                                        (
                                                         GetPrecoVenda(custo.valorVenda.ToString())
                                                            *
                                                         Utility.VLR_STRJLISTASEM
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                         GetICMSSobreVenda(custo)
                                                       );
                                            }
                                            else
                                                return oRg.icmsStSobreVenda;

                                        }
                                    }
                                    else
                                        return new decimal();

                                default:
                                    return new decimal();
                            }
                        }

                        else
                        {
                            return new decimal();
                        }
                    }

                    #endregion

                    else
                        return new decimal();
                }
                else
                    return new decimal();

                #region :: Regras de cálculo de ST ::

                /*
                 * ST somente para estabelecimentos 2 e 31
                 *
                 *   regras estabelecimento 2
                 *
                 *   Somente RS
                 *
                 *   saida = RS
                 *   - regime normal 
                 *   - contribuinte
                 *   - produto similar
                 *   - pmc17 * 0,75 * icmsst
                 *
                 *   saida = RS
                 *   - regime normal 
                 *   - contribuinte
                 *   - produto NAO similar
                 *   - pmc17 * 0,80 * icmsst
                 *
                 *   default = 0
                 *
                 *   Regras estabelecimento 31
                 *
                 *   Somente RJ
                 *
                 *   Saida = RJ
                 *   - Regime normal e/ou Dermocosmeticos
                 *   - contribuinte
                 *   - lista Positiva
                 *   - custoPadrao * 1,3824(variável) * icmsst
                 *
                 *   Saida = RJ
                 *   - Regime normal e/ou Dermocosmeticos
                 *   - contribuinte
                 *   - lista Negativa
                 *   - custoPadrao * 1,3293(variável) * icmsst
                 *
                 *   Saida = RJ
                 *   - Regime normal e/ou Dermocosmeticos
                 *   - contribuinte
                 *   - lista Neutra
                 *   - custoPadrao * 1,4142(variável) * icmsst
                 *
                 *   Saida = RJ
                 *   - Regime normal e/ou Dermocosmeticos
                 *   - contribuinte
                 *   - lista Sem Lista
                 *   - custoPadrao * 1,4142(variável) * icmsst
                 *
                 *   default 0
                 *
                 */

                #endregion
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetICMSSobreVenda(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                #region :: Versão anterior ::


                #endregion

                List<SimuladorPrecoRegrasGerais> oRg =
                    new SimuladorPrecoRegrasGerais
                    {
                        estabelecimentoId = custo.estabelecimentoId,
                        convenioId = Utility.FormataStringPesquisa(custo.convenioId),
                        perfilCliente = custo.perfilCliente,
                        resolucaoId = Utility.FormataStringPesquisa(custo.resolucao13),
                        ufDestino = this.ddlEstadoDestino.SelectedValue,
                        usoExclusivoHospitalar = Utility.FormataStringPesquisa(custo.exclusivoHospitalar)

                    }.GetRegrasICMSVenda();

                if (oRg != null)
                {

                    if (custo.estabelecimentoId.Equals("20"))
                    {

                        if (custo.convenioId.Equals("Convênio 118"))
                        {
                        }
                    }


                    SimuladorPrecoRegrasGerais _oRg = null;

                    #region :: Valida o cálculo de ICMS de saída de acordo com a ST de entreda ::

                    if (
                        GetValorICMS_ST(custo) > 0
                       )
                        _oRg = oRg.Where(x => x._icmsStValor.GetValueOrDefault(-1).Equals(0)).SingleOrDefault();
                    else
                        _oRg = oRg.Where(x => x._icmsStValor == null).SingleOrDefault();

                    #endregion



                    return ((_oRg.icmsSobreVenda > 1 ? (_oRg.icmsSobreVenda / 100) : _oRg.icmsSobreVenda) * GetPrecoVenda(custo.valorVenda.ToString()));
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorICMS_ST(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                if (custo.descST.Trim().Equals("-") && !custo.mva.Equals(decimal.Zero))
                {
                    var ipi = custo.valorIPI;
                    var precoAquisicao = custo.precoAquisicao;
                    var mva = custo.mva > 1 ? (1 + custo.mva / 100) : custo.mva;
                    var reducaoST = custo.percReducaoBase > decimal.One ? (1 - custo.percReducaoBase / 100) : custo.percReducaoBase;
                    var icmsAliquotaICMS = (custo.aliquotaInternaICMS > decimal.One ? (custo.aliquotaInternaICMS / 100) : custo.aliquotaInternaICMS);


                    mva = !mva.Equals(decimal.Zero) ? mva : 1;
                    reducaoST = !reducaoST.Equals(decimal.Zero) ? reducaoST : 1;
                    icmsAliquotaICMS = icmsAliquotaICMS.Equals(decimal.Zero) ? 1 : icmsAliquotaICMS;

                    var valorICMS = ((((precoAquisicao + ipi) * mva) * reducaoST) * icmsAliquotaICMS);

                    return valorICMS;

                }
                else
                {
                    return new decimal();
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetPrecoAquisicao(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                var precoAquisicao = (custo.precoFabrica * (1 - (custo.descontoComercial > 1 ? (custo.descontoComercial / 100) : custo.descontoComercial)) * (1 - (GetDescontoAdicional(custo.descontoAdicional) / 100)) * (1 - (custo.percRepasse > 1 ? (custo.percRepasse / 100) : custo.percRepasse)));
                return precoAquisicao;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorCustoPadrao(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                #region :: Aplicação da Regra ::
                decimal precoAquisicao = new decimal();
                string _convenio = GetEquality(custo.convenioId.Trim(), "CONVÊNIO 118", "CONVÊNIO 57");

                switch (_convenio.ToUpper())
                {
                    case "CONVÊNIO 118":

                        if (custo.estabelecimentoId.Equals("20"))
                        {
                            if (!this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                                if (!IsContribuinte)
                                    custo.percICMSe = custo.percICMSe.Equals(0) ? new decimal() : Utility.VLR_CUSTOPADRAOICMSE;
                        }

                        break;

                    default:

                        if (_convenio.ToUpper().Equals("CONVÊNIO 57"))
                            break;

                        break;
                }


                #endregion

                if (custo.estabelecimentoId == "12" || custo.estabelecimentoId == "13")
                {
                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("ES"))
                    {
                        custo.resolucao13 = RemoveSpecialCharactersAccents(custo.resolucao13);
                        return (
                              (GetPrecoAquisicao(custo) - decimal.Parse(custo.transfEs))
                                   -
                               GetCreditoICMS(custo)
                                   +
                               GetValorIPI(custo)
                                   -
                               GetValorPISCofins(custo)
                                   +
                               GetValorICMS_ST(custo)
                              );
                    }
                }
                else if (custo.estabelecimentoId == "11")
                {
                    decimal VariaVelFix = new decimal(1.6);
                    var precoFabrica = custo.precoFabrica;
                    var desconto = precoFabrica * custo.descontoComercial;
                    var repasse = (custo.precoFabrica - desconto) * custo.percRepasse;
                    precoAquisicao = precoFabrica - desconto - repasse;
                    var creditoIMCS = ((precoAquisicao * (1 + custo.mva)) * VariaVelFix) / 100;
                    var custoPadrao = precoAquisicao + creditoIMCS;

                    return custoPadrao;
                }
                else
                {
                    decimal VariaVelFix = new decimal(1.6);
                    var precoFabrica = custo.precoFabrica;
                    var desconto = precoFabrica * custo.descontoComercial;
                    var repasse = (custo.precoFabrica - desconto) * custo.percRepasse;
                    precoAquisicao = precoFabrica - desconto - repasse;
                }



                return (
                (precoAquisicao - decimal.Parse(custo.transfEs))
                       -
                   GetCreditoICMS(custo)
                       +
                   GetValorIPI(custo)
                       -
                   GetValorPISCofins(custo)
                       +
                   GetValorICMS_ST(custo)
                  );








            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorIPI(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                return (custo.precoAquisicao * (custo.percIPI > 1 ? (custo.percIPI / 100) : custo.percIPI));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetCreditoICMS(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                if (custo.estabelecimentoId == "12" || custo.estabelecimentoId == "13")
                {
                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("ES"))
                    {

                        custo.resolucao13 = RemoveSpecialCharactersAccents(custo.resolucao13);

                        bool regiaoSUL = false;
                        bool regiaoNO = false;
                        switch (custo.ufIdOrigem)
                        {
                            case "RS":
                            case "SC":
                            case "PR":
                            case "SP":
                            case "RJ":
                            case "ES":
                            case "MG":
                                regiaoSUL = true;
                                break;
                            default:
                                regiaoNO = true;
                                break;
                        }

                        if (custo.tratamentoICMSEstab.Equals("Sem ICMS"))
                        {
                            return new decimal();
                        }

                        if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals(custo.ufIdOrigem))
                        {

                            custo.percICMSe = 17;
                            return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));

                        }

                        else if (custo.resolucao13.Equals("SIM") && regiaoNO)
                        {
                            custo.percICMSe = 4;
                            return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));

                        }
                        else if (custo.resolucao13.Equals("NAO") && regiaoNO)
                        {
                            custo.percICMSe = 12;
                            return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));

                        }

                        else if (custo.resolucao13.Equals("SIM") && regiaoSUL)
                        {
                            custo.percICMSe = 4;
                            return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));

                        }
                        else if (custo.resolucao13.Equals("NAO") && regiaoSUL)
                        {
                            custo.percICMSe = 12;

                            var calculoIcms = (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                            return calculoIcms;

                        }

                    }
                }
                else
                {

                    if (custo.estabelecimentoId == "20")
                    {
                        if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                        {

                            if (custo.convenioId.ToUpper().Equals("REGIME NORMAL"))
                            {

                                custo.percICMSe = 12;
                                return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                            }
                            else
                            {
                                return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));

                            }
                        }
                        else if (!this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                        {
                            if (rblPerfilCliente.SelectedValue.Equals("0") || rblPerfilCliente.SelectedValue.Equals("1") || rblPerfilCliente.SelectedValue.Equals("2") || rblPerfilCliente.SelectedValue.Equals("C"))
                            {
                                if (custo.convenioId.Equals("Convênio 118") || custo.convenioId.Equals("Convênio 57"))
                                {
                                    custo.percICMSe = 0;
                                    return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                                }
                                else
                                {
                                    custo.percICMSe = 12;
                                    return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                                }
                            }
                            else
                            {
                                return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                            }
                        }
                        else
                        {
                            return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                        }
                    }
                    else
                    {
                        var icmaCalculado = (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
                        return icmaCalculado;
                    }
                }
                return (custo.precoAquisicao * (1 - (custo.percReducaoBase > 1 ? (custo.percReducaoBase / 100) : custo.percReducaoBase)) * (custo.percICMSe > 1 ? (custo.percICMSe / 100) : custo.percICMSe));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorPISCofins(SimuladorPrecoCustosNovo custo)
        {
            try
            {
                return (custo.precoAquisicao * (custo.percPisCofins > 1 ? (custo.percPisCofins / 100) : custo.percPisCofins));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        public static string CalulaNovaBase(string categoria, string lista, decimal pmc, decimal vlVenda, decimal PMC_cheio)
        {
            string retorno = string.Empty;
            SimuladorPrecoTrava simuladorPrecoTrava = new SimuladorPrecoTrava();

            simuladorPrecoTrava.categoria = categoria;
            simuladorPrecoTrava.lista = lista;

            decimal base1 = new decimal();
            decimal base2 = new decimal();
            var dt = simuladorPrecoTrava.ObterDadosPorFiltro();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    decimal trava = decimal.Parse(dt.Rows[0]["trava"].ToString().Replace('%', ' ')) / 100;
                    decimal mva_st = decimal.Parse(dt.Rows[0]["mva_st"].ToString().Replace('%', ' '));
                    decimal redutor = decimal.Parse(dt.Rows[0]["mva_ts_reduzido"].ToString().Replace('%', ' '));
                    base1 = CacularBase1(PMC_cheio, redutor);

                    if (base1 > 0)
                    {
                        var calculoNovaBase = vlVenda / base1;
                        if (calculoNovaBase >= trava)
                        {
                            base2 = CacularBase2(vlVenda, mva_st);

                            if (base2 <= PMC_cheio)
                            {
                                retorno = "Base - 2;" + base2.ToString();
                                return retorno;
                            }
                            else
                            {
                                retorno = "PMC_cheio - 2;" + PMC_cheio.ToString();
                                return retorno;
                            }
                        }
                        else
                        {
                            retorno = "Base - 1;" + base1.ToString();
                            return retorno;
                        }
                    }
                    else
                    {
                        retorno = "Base ST;" + base1.ToString();
                        return retorno;
                    }
                }
                else
                {
                    return retorno;
                }
            }
            else
            {
                return retorno;
            }

        }
        private static decimal CacularBase1(decimal pmc, decimal redutor)
        {
            return pmc * (1 - redutor / 100); ;
        }
        private static decimal CacularBase2(decimal precoVenda, decimal mvaST)
        {
            return precoVenda * (1 + mvaST / 100);
        }
        private decimal GetValorIcmsSobreVenda(decimal _vlrVenda, decimal _vlrIcms)
        {
            decimal Icms = _vlrIcms > 1 ? (_vlrIcms / 100) : _vlrIcms;
            return (_vlrVenda * Icms);
        }
        public void DataAtualizacao()
        {

            SimuladorPrecoCustos oSP = new SimuladorPrecoCustos();

            DataTable dt = oSP.GetDataUltimoUpdate();
            string Texto = string.Empty;
            if (dt != null)
            {

                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Texto += dt.Rows[i]["Tipo"].ToString() + ": " + String.Format("{0:d/M/yyyy HH:mm:ss}", dt.Rows[i]["DataUpdate"].ToString()) + " - ";
                    }


                    ltrDataAtualizacao.Text = "Última atualização: " + Texto.Remove(Texto.Length - 2, 2);
                }

            }

        }
        public static string RemoveSpecialCharactersAccents(string texto, bool allowSpace = true)
        {

            string ret;

            if (allowSpace)
                ret = System.Text.RegularExpressions.Regex.Replace(texto, @"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\s,]+?", string.Empty);
            else
                ret = System.Text.RegularExpressions.Regex.Replace(texto, @"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ]+?", string.Empty);


            if (string.IsNullOrEmpty(ret))
                return String.Empty;
            else
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(ret);
                ret = System.Text.Encoding.UTF8.GetString(bytes);
            }


            return ret;

        }
        public void PopularModalErro(string textoErro)
        {

            // lblModalTitle.Text = "";
            lblModalBody.Text = textoErro;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);


        }
        protected void btnExportar_Click1(object sender, EventArgs e)
        {
            if (listaRetorno.Count() != 0)
            {
                DataTable odt = ConvertToDataTable(listaRetorno);

                if (odt != null && odt.Rows.Count > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(odt, "CalculoSimuladorTotal");

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=CalculoSimuladorTotal" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
                else
                {
                    PopularModalErro("POR FAVOR SIMULAR PRIMEIRO!");
                }
            }
            else
            {
                PopularModalErro("POR FAVOR SIMULAR PRIMEIRO!");
            }
        }
        private bool VerificaseItemTemCalculoST(string estabelecimentoDestino, string estabelecimentoId, string itemId, string _convenioId, string _perfil)
        {
            SimuladorRegrasST RegraST = new SimuladorRegrasST()
            {
                itemId = itemId.PadLeft(5, '0'),
                estadoDestino = estabelecimentoDestino,
                estabelecimentoId = estabelecimentoId,
                classeFiscal = _convenioId,
                perfilCliente = _perfil
            };

            DataTable dt = RegraST.ObterTodosComFiltro();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        private decimal BuscaIcmsST(string estabelecimentoDestino, string estabelecimentoId, string itemId, string _convenioId, string _perfil)
        {
            SimuladorRegrasST RegraST = new SimuladorRegrasST()
            {
                itemId = itemId.PadLeft(5, '0'),
                estadoDestino = estabelecimentoDestino,
                estabelecimentoId = estabelecimentoId,
                classeFiscal = _convenioId,
                perfilCliente = _perfil
            };

            DataTable dt = RegraST.ObterTodosComFiltro();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    return decimal.Parse(dt.Rows[0]["icmsInterno"].ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
        #region metodos antigos 
        protected decimal GetValorCustoPadrao(decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, decimal _percPISCofins, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, decimal _valorICMSST, decimal _reducaoST_MVA, string _estabelecimento, string _convenioId, int CodigoOrigem, string tratamentoICMSEstab, string uforigem, string resolucao, decimal TransfES)
        {
            try
            {
                #region :: Aplicação da Regra ::

                string _convenio = GetEquality(_convenioId.Trim(), "CONVÊNIO 118", "CONVÊNIO 57");

                switch (_convenio.ToUpper())
                {
                    case "CONVÊNIO 118":

                        if (_estabelecimento.Equals("20"))
                        {
                            if (!this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                                if (!IsContribuinte)
                                    _ICMSe = _ICMSe.Equals(0) ? new decimal() : Utility.VLR_CUSTOPADRAOICMSE;
                            //_ICMSe = Utility.VLR_CUSTOPADRAOICMSE;
                        }

                        break;

                    default:

                        if (_convenio.ToUpper().Equals("CONVÊNIO 57"))
                            break;

                        break;
                }


                #endregion

                if (_estabelecimento == "12" || _estabelecimento == "13")
                {
                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("ES"))
                    {
                        resolucao = RemoveSpecialCharactersAccents(resolucao);
                        return (
                              (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) - TransfES)
                                   -
                               GetCreditoICMS(_reducaoBase, _ICMSe, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, CodigoOrigem, tratamentoICMSEstab, _estabelecimento, uforigem, resolucao, _convenioId)
                                   +
                               GetValorIPI(_percIPI, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse)
                                   -
                               GetValorPISCofins(_percPISCofins, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse)
                                   +
                               GetValorICMS_ST(_descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA)
                              );
                        //-
                        //GetCreditoICMS(_reducaoBase,
                        //              Utility.VLR_CUSTOPADRAOICMSE,
                        //              _precoFabrica,
                        //              GetDescontoComercial(_descontoComercial),
                        //              _descontoAdicional,
                        //              _repasse,
                        //              CodigoOrigem,
                        //              tratamentoICMSEstab,
                        //              _estabelecimento,
                        //              uforigem, resolucao
                        //              );

                    }
                }






                return (
                     (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) - TransfES)  // GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse)
                            -
                        GetCreditoICMS(_reducaoBase, _ICMSe, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, CodigoOrigem, tratamentoICMSEstab, _estabelecimento, uforigem, resolucao, _convenioId)
                            +
                        GetValorIPI(_percIPI, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse)
                            -
                        GetValorPISCofins(_percPISCofins, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse)
                            +
                        GetValorICMS_ST(_descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA)
                       );

                #region :: Definição das Regras de cálculo ::

                /*
                 * 
                 * Convênio 57
                 *  - Normal
                 *  
                 * Convênio 118
                 *  - Loja <> 20
                 *   - Normal
                 *                 
                 * Convênio 118
                 *  - Loja 20
                 *   - Destino <> DF
                 *    - Não Contribuinte
                 *     - Se tiver ICMS
                 *      - ICMSe Sempre 12%
                 *
                 * * Convênio 118
                 *  - Loja 20
                 *   - Destino <> DF
                 *    - Não Contribuinte
                 *     - Se NÃO tiver ICMS
                 *      - 0
                 * 
                 * Convênio 118
                 *  - Loja 20
                 *   - Destino <> DF
                 *    - Contribuinte
                 *     - Normal
                 *     
                 * Convênio 118
                 *  - Loja 20
                 *   - Destino = DF
                 *    - Normal
                 *
                 * Default
                 *  - Loja <> 20
                 *   - Normal
                 *  
                 * Default
                 *  - Loja 20
                 *   - Destino = DF
                 *    - ICMSe Sempre 12%
                 *    
                 * Default
                 *  - Loja 20
                 *   - Destino <> DF
                 *    - Não Contribuinte
                 *     - ICMSe Sempre 12%
                 *     
                 * Default
                 *  - Loja 20
                 *   - Destino <> DF
                 *    - Contribuinte
                 *     - Normal
                 */

                #endregion
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetPrecoAquisicao(decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse)
        {
            try
            {

                return (_precoFabrica * (1 - (_descontoComercial > 1 ? (_descontoComercial / 100) : _descontoComercial)) * (1 - (GetDescontoAdicional(_descontoAdicional) / 100)) * (1 - (_repasse > 1 ? (_repasse / 100) : _repasse)));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetCreditoICMS(decimal _reducaoBase, decimal _ICMSe, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, int CodigoOrigem, string tratamentoICMSEstab, string _estabelecimento, string UFOrigem, string _resolucao, string ConvenioId)
        {
            try
            {
                if (_estabelecimento == "12" || _estabelecimento == "13")
                {
                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("ES"))
                    {

                        _resolucao = RemoveSpecialCharactersAccents(_resolucao);

                        bool regiaoSUL = false;
                        bool regiaoNO = false;
                        switch (UFOrigem)
                        {
                            case "RS":
                            case "SC":
                            case "PR":
                            case "SP":
                            case "RJ":
                            case "ES":
                            case "MG":
                                regiaoSUL = true;
                                break;
                            default:
                                regiaoNO = true;
                                break;
                        }

                        if (tratamentoICMSEstab.Equals("Sem ICMS"))
                        {
                            return new decimal();
                        }

                        if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals(UFOrigem))
                        {

                            _ICMSe = 17;
                            return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                        }

                        else if (_resolucao.Equals("SIM") && regiaoNO)
                        {
                            _ICMSe = 4;
                            return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                        }
                        else if (_resolucao.Equals("NAO") && regiaoNO)
                        {
                            _ICMSe = 12;
                            return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                        }

                        else if (_resolucao.Equals("SIM") && regiaoSUL)
                        {
                            _ICMSe = 4;
                            return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                        }
                        else if (_resolucao.Equals("NAO") && regiaoSUL)
                        {
                            _ICMSe = 7;
                            return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                        }

                    }
                }
                else
                {

                    if (_estabelecimento == "20")
                    {
                        if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                        {

                            if (ConvenioId.ToUpper().Equals("REGIME NORMAL"))
                            {

                                _ICMSe = 12;
                                return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                            }
                            else
                            {
                                return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                            }
                        }
                        else if (!this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                        {



                            // 0 - Mercado Público                            1 - Mercado Privado Não Contribuinte
                            if (rblPerfilCliente.SelectedValue.Equals("0") || rblPerfilCliente.SelectedValue.Equals("1") || rblPerfilCliente.SelectedValue.Equals("2") || rblPerfilCliente.SelectedValue.Equals("C"))
                            {

                                if (ConvenioId.Equals("Convênio 118") || ConvenioId.Equals("Convênio 57"))
                                {
                                    _ICMSe = 0;
                                    return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                                }
                                else
                                {
                                    _ICMSe = 12;
                                    return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                                }


                                //if (_resolucao.Equals("SIM"))
                                //{

                                //    _ICMSe = 12;
                                //    return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                                //}
                                //else
                                //{
                                //    return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                                //    //_ICMSe = 4;
                                //    //return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

                                //}

                            }
                            else
                            {
                                return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                                //_ICMSe = 4;
                                //return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                            }

                        }
                        else
                        {
                            return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                            //_ICMSe = 4;
                            //return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                        }


                    }
                    else
                    {
                        return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));
                    }

                }
                return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorIPI(decimal _percIPI, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse)
        {
            try
            {
                return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (_percIPI > 1 ? (_percIPI / 100) : _percIPI));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorICMS_ST(string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, decimal _valorICMSST, decimal _percIPI, decimal _reducaoST_MVA)
        {
            try
            {
                if (_descST.Trim().Equals("-") && _mva.Equals(0))
                    return new decimal();
                else if (_descST.Trim().Equals("-") && !(_mva.Equals(0)))
                    return ((GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) + GetValorIPI(_percIPI, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse)) * (1 + (_mva > 1 ? (_mva / 100) : _mva)) * (1 - (_reducaoST_MVA > 1 ? (_reducaoST_MVA / 100) : _reducaoST_MVA)) * (_aliquotaIntICMS > 1 ? (_aliquotaIntICMS / 100) : _aliquotaIntICMS));
                else
                    return (_pmc17 * (1 - (decimal.Parse(_descST) > 1 ? (decimal.Parse(_descST) / 100) : decimal.Parse(_descST))) * (_aliquotaIntICMS > 1 ? (_aliquotaIntICMS / 100) : _aliquotaIntICMS));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        protected decimal GetValorPISCofins(decimal _percPISCofins, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse)
        {
            try
            {
                return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (_percPISCofins > 1 ? (_percPISCofins / 100) : _percPISCofins));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }
        #endregion
    }
}