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
using System.Configuration;
using System.Data.SqlTypes;
using System.Xml;
using System.IO;

namespace KS.SimuladorPrecos.AppPaginas.Consulta
{

    [Serializable]
    public class SimuladorSumarizacao
    {
        public string estabelecimentoId { get; set; }
        public decimal precoVenda { get; set; }
        public decimal valorMargem { get; set; }
    }


    public partial class SimuladorPreco : PageBase
    {

        SimuladorPrecoCustosCalculos _simuladorPrecoCustos = new SimuladorPrecoCustosCalculos();
        CalculoDeCustoPadrao _calculoDeCustoPadrao = new CalculoDeCustoPadrao();
        CalculoSobreVenda _calculoSobreVenda = new CalculoSobreVenda();


        #region :: ViewState ::

        private bool CapAplicado
        {
            get { return this.ViewState["CapAplicado"] != null ? bool.Parse(this.ViewState["CapAplicado"].ToString()) : false; }
            set { this.ViewState["CapAplicado"] = value; }
        }

        private bool VendaComST
        {
            get { return this.ViewState["VendaComST"] != null ? bool.Parse(this.ViewState["VendaComST"].ToString()) : false; }
            set { this.ViewState["VendaComST"] = value; }
        }

        private decimal transf
        {
            get { return this.ViewState["transf"] != null ? decimal.Parse(this.ViewState["transf"].ToString()) : decimal.MinValue; }
            set { this.ViewState["transf"] = value; }
        }



        private List<SimuladorPrecoCustos> lstCustos
        {
            get { return this.ViewState["lstCustos"] != null ? ((List<SimuladorPrecoCustos>)this.ViewState["lstCustos"]) : null; }
            set { this.ViewState["lstCustos"] = value; }
        }

        #endregion

        #region :: Propriedades ::

        /// <summary>
        /// Verifica se é um perfil contribuinte de ICMS ou não
        /// </summary>
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

        /// <summary>
        /// Informa se é simulação para o setor público
        /// </summary>
        private bool IsSetorPublico
        {
            get { return rblPerfilCliente.SelectedValue.Equals("0"); }
        }

        public decimal ValorIcmsSobreVenda { get; private set; }

        #endregion

        #region :: Campos ::

        /// <summary>
        /// List dos valores calculos para apresentação da Grid de sumarização
        /// </summary>
        List<SimuladorSumarizacao> lstItens = null;
        private decimal vendaLiquidaComICMSTS;
        private decimal vendaLiquidaPE;
        private decimal ValorPISCofins;
        private decimal margemVlrValorComST;

        #endregion

        #region :: Eventos ::

        #region :: Eventos de Página ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            this.Title = GetResourceValue("titTitulo", true, "lblSimuladorPreco");

            if (!Page.IsPostBack)
            {
                ckeckCalculaST.Enabled = false;

                AddScripts();
                DataAtualizacao();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


            #region :: Grid Sumarizacao ::

            if (lstCustos != null)
                if (dlSimulacoes.Items.Count.Equals(lstCustos.Count))
                    dlSimulacoes_ItemCreated(new object(), new DataListItemEventArgs(new DataListItem(0, ListItemType.Item)));

            #endregion
        }

        #endregion

        #region :: Botões ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnItem_Click(object sender, EventArgs e)
        {
            GetItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSimular_Click(object sender, EventArgs e)
        {
            Simular();
        }

        #endregion

        #region :: TextBox ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtItem_TextChanged(object sender, EventArgs e)
        {

            dlSimulacoes.DataSource = null;
            dlSimulacoes.DataBind();

            lstCustos = null;
            lstItens = null;
            vendaLiquidaComICMSTS = 0;
            vendaLiquidaPE = 0;
            ValorPISCofins = 0;
            margemVlrValorComST = 0;




            int numero = 0;
            if (((TextBox)sender).Text.Contains("|"))
                ((TextBox)sender).Text = ((TextBox)sender).Text.Split('|')[0].Trim();
            else
            {
                if (int.TryParse(txtItem.Text, out numero))
                {
                    ((TextBox)sender).Text = numero.ToString();
                }
                else
                {
                    PopularModalErro("Selecione o item do filtro ou insira o codigo do Item! ");
                    return;
                }
            }

            GetItem();
        }

        #endregion

        #region :: DataList ::


        protected void dlSimulacoes_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item.ItemType.Equals(ListItemType.Item) || e.Item.ItemType.Equals(ListItemType.AlternatingItem))
                {
                    SimuladorPrecoCustos oCst = new SimuladorPrecoCustos();

                    #region :: Margem Percentual ::
                    decimal mva = decimal.Parse(DataBinder.Eval(e.Item.DataItem, "mva").ToString()) * 100;

                    decimal precoAquisicao = new decimal();
                    decimal redutorBase = new decimal();
                    transf = new decimal();
                    decimal TransfEs = decimal.Parse(DataBinder.Eval(e.Item.DataItem, "transfEs").ToString());
                    decimal calcula = decimal.Parse(DataBinder.Eval(e.Item.DataItem, "transfDF").ToString());

                    if (UserDataInfo.TipoOperacao.Equals("PJ"))
                    {
                        if (DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString().Equals("30"))
                        {
                            precoAquisicao = GetAquisicaoPorEstabelecimento("3");
                            redutorBase = GetRedutorPorEstabelecimento("3");

                            ((Label)e.Item.FindControl("lblHeader")).Text = "Hosplog DF - Estab. 20 Transf";
                        }
                        else
                        {
                            precoAquisicao = GetAquisicaoPorEstabelecimento("3");
                            redutorBase = GetRedutorPorEstabelecimento("3");
                        }
                    }
                    else
                    {
                        if (DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString().Equals("30"))
                        {
                            precoAquisicao = GetAquisicaoPorEstabelecimento("4");
                            redutorBase = GetRedutorPorEstabelecimento("4");

                            ((Label)e.Item.FindControl("lblHeader")).Text = "Hosplog DF - Estab. 20 Transf";
                        }
                        else
                        {
                            precoAquisicao = GetAquisicaoPorEstabelecimento("4");
                            redutorBase = GetRedutorPorEstabelecimento("4");
                        }
                    }



                    transf = RegraTransfESCalculoGerais(DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                                this.rblPerfilCliente.SelectedValue, TransfEs, calcula, precoAquisicao, redutorBase);

                    ((Literal)e.Item.FindControl("ltrTransESValor")).Text = string.Format("{0:n2}", transf);

                    ((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text =
                        string.Format("{0:n2}%",
                            GetPercentualMargem(GetPrecoVendaDesconto(DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()),
                                                DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                                DataBinder.Eval(e.Item.DataItem, "tipo").ToString(),
                                                rblPerfilCliente.SelectedValue,
                                                DataBinder.Eval(e.Item.DataItem, "resolucao13").ToString(),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percICMSe").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()),
                                                DataBinder.Eval(e.Item.DataItem, "descST").ToString(), mva,
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()),
                                                e,
                                                DataBinder.Eval(e.Item.DataItem, "listaDescricao").ToString(),
                                                DataBinder.Eval(e.Item.DataItem, "categoria").ToString(),
                                                DataBinder.Eval(e.Item.DataItem, "ufIdOrigem").ToString(),
                                                DataBinder.Eval(e.Item.DataItem, "exclusivoHospitalar").ToString(),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "valorICMSST").ToString()),
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "reducaoST_MVA").ToString()),
                                                int.Parse(DataBinder.Eval(e.Item.DataItem, "itemCodigoOrigem").ToString()),
                                                DataBinder.Eval(e.Item.DataItem, "tratamentoICMSEstab").ToString(),
                                                transf));





                    #endregion

                    #region :: Quadros Sumarizado/Resumido ::

                    if (UserDataInfo.VisualizacaoTipo.Equals(VisualizacaoTipo.Resumida))
                    {
                        if (UserDataInfo.VisualizacaoQuadro.Equals(VisualizacaoQuadro.Resumida))
                            EnableDisableLine(false,
                                ((HtmlGenericControl)e.Item.FindControl("dvDescontoComercial")),
                                ((HtmlGenericControl)e.Item.FindControl("dvDescontoAdicionalValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvRepasseValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPrecoAquisicao")),
                                ((HtmlGenericControl)e.Item.FindControl("dvReducaoBaseValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSSEValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvCreditoICMSValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvIPIPrc")),
                                ((HtmlGenericControl)e.Item.FindControl("dvIPIValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPISCofinsPrc")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPISCofins")),
                                ((HtmlGenericControl)e.Item.FindControl("dvDescParaSTValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvMVA")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSST")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSAliquota")),
                                ((HtmlGenericControl)e.Item.FindControl("dvTransES")),
                                ((HtmlGenericControl)e.Item.FindControl("dvCustoPadrao")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSSobreVenda")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSSTSobreVenda")),
                                ((HtmlGenericControl)e.Item.FindControl("dvAjusteRegimeFiscal")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPISCofinsSobreVenda")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPrecoVendaLiquido")),
                                ((HtmlGenericControl)e.Item.FindControl("dvCustoPadraoVenda")),
                                ((HtmlGenericControl)e.Item.FindControl("dvLinhaVaga")),
                                ((HtmlGenericControl)e.Item.FindControl("dvMargemValor")));
                        else if (UserDataInfo.VisualizacaoQuadro.Equals(VisualizacaoQuadro.Sumarizada))
                            EnableDisableLine(false,
                                ((HtmlGenericControl)e.Item.FindControl("dvDescontoAdicionalValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvDescontoComercial")),
                                ((HtmlGenericControl)e.Item.FindControl("dvRepasseValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPrecoAquisicao")),
                                ((HtmlGenericControl)e.Item.FindControl("dvReducaoBaseValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSSEValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvCreditoICMSValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvIPIPrc")),
                                ((HtmlGenericControl)e.Item.FindControl("dvIPIValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPISCofinsPrc")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPISCofins")),
                                ((HtmlGenericControl)e.Item.FindControl("dvDescParaSTValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvMVA")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSST")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSAliquota")),
                                 ((HtmlGenericControl)e.Item.FindControl("dvTransES")),
                                ((HtmlGenericControl)e.Item.FindControl("dvCustoPadrao")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSSobreVenda")),
                                ((HtmlGenericControl)e.Item.FindControl("dvICMSSTSobreVenda")),
                                ((HtmlGenericControl)e.Item.FindControl("dvAjusteRegimeFiscal")),
                                ((HtmlGenericControl)e.Item.FindControl("dvLinhaVaga")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPrecoVendaLiquido")),
                                ((HtmlGenericControl)e.Item.FindControl("dvMargemValor")),
                                ((HtmlGenericControl)e.Item.FindControl("dvPISCofinsSobreVenda")));
                    }

                    #endregion

                    #region :: Store Data ::

                    decimal precoVendaComST = !String.IsNullOrEmpty(((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text) ? decimal.Parse(((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text) : new decimal();
                    if (precoVendaComST > 0)
                    {

                        lstItens.Add(
                            new SimuladorSumarizacao
                            {

                                estabelecimentoId = ((Label)e.Item.FindControl("lblHeader")).Text.ToUpper(),
                                precoVenda = !String.IsNullOrEmpty(((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text) ? decimal.Parse(((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text) : new decimal(),
                                valorMargem = !String.IsNullOrEmpty(((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text) ? decimal.Parse(((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text.Remove(((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text.Length - 1)) : new decimal()
                            });

                    }
                    else
                    {

                        lstItens.Add(
                        new SimuladorSumarizacao
                        {

                            estabelecimentoId = ((Label)e.Item.FindControl("lblHeader")).Text.ToUpper(),
                            precoVenda = !String.IsNullOrEmpty(((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text) ? decimal.Parse(((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text) : new decimal(),
                            valorMargem = !String.IsNullOrEmpty(((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text) ? decimal.Parse(((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text.Remove(((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text.Length - 1)) : new decimal()
                        });
                    }
                    #endregion


                    Literal lbl = (Literal)(e.Item.FindControl("ltrICMSeValor"));

                    lbl.Text = string.Format("{0:n2}%", GetPercICMSe2(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percICMSe").ToString()),
                                                                        DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                                                        DataBinder.Eval(e.Item.DataItem, "ufIdOrigem").ToString(),
                                                                        DataBinder.Eval(e.Item.DataItem, "resolucao13").ToString(), DataBinder.Eval(e.Item.DataItem, "tipo").ToString()
                                                                                                              ));
                    Literal ltrCreditoICMS = (Literal)(e.Item.FindControl("ltrCreditoICMSValor"));

                    ltrCreditoICMS.Text = string.Format("{0:n2}",
                                        GetCreditoICMS(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()),
                                                        Utility.VLR_CUSTOPADRAOICMSE,
                                                        decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                        GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                        decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                        decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()),
                                                        int.Parse(DataBinder.Eval(e.Item.DataItem, "itemCodigoOrigem").ToString()),
                                                        DataBinder.Eval(e.Item.DataItem, "tratamentoICMSEstab").ToString(),
                                                        DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                                        DataBinder.Eval(e.Item.DataItem, "ufIdOrigem").ToString(),
                                                        DataBinder.Eval(e.Item.DataItem, "resolucao13").ToString(),
                                                        DataBinder.Eval(e.Item.DataItem, "tipo").ToString()));

                    if (DataBinder.Eval(e.Item.DataItem, "tratamentoICMSEstab").ToString().Equals("Sem ICMS"))
                    {
                        ltrCreditoICMS.Text = string.Format("{0:n2}", 0);
                    }







                    ((Literal)e.Item.FindControl("ltrDescontoComercialValor")).Text =
                    string.Format("{0:n2}%",
                                                        GetDescontoComercial
                                                            (
                                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString()) < 1 ?
                                                            (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString()) * 100) :
                                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())
                                                            ));

                    ((Literal)e.Item.FindControl("ltrDescontoAdicionalValor")).Text = string.Format("{0:n2}%",
                                                                                GetDescontoAdicional(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString())) < 1 ?
                                                                                    (GetDescontoAdicional(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString())) * 100) :
                                                                                        GetDescontoAdicional(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString())));



                    ((Literal)e.Item.FindControl("ltrRepasseValor")).Text = string.Format("{0:n2}%",
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()) < 1 ?
                                                (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()) * 100) :
                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()
                                            ));



                    ((Literal)e.Item.FindControl("ltrPrecoAquisicaoValor")).Text = string.Format("{0:n2}",
                                                                       GetPrecoAquisicao(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                                        GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                                        decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                                        decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString())
                                                                    ));

                    ((Literal)e.Item.FindControl("ltrReducaoBaseValor")).Text = string.Format("{0:n2}%",
                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()) < 1 ?
                                                   (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()) * 100) :
                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()
                                                ));

                    ((Literal)e.Item.FindControl("ltrIPIPrcValor")).Text = string.Format("{0:n2}%",
                                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()) < 1 ?
                                                            (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()) * 100) :
                                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()
                                                            ));

                    ((Literal)e.Item.FindControl("ltrIPIVlrValor")).Text = string.Format("{0:n2}",
                                                    GetValorIPI(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()),
                                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                                GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString())
                                                                ));
                    ((Literal)e.Item.FindControl("ltrPISCofinsPrcValor")).Text = string.Format("{0:n2}%",
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()) < 1 ?
                                            (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()) * 100) :
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()
                                        ));



                    ((Literal)e.Item.FindControl("ltrPISCofinsVlrValor")).Text = string.Format("{0:n2}",
                                                                GetValorPISCofins(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()),
                                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                                                    GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString())
                                                                                ));


                    ((Literal)e.Item.FindControl("ltrPMCValor")).Text = string.Format("{0:n2}",
                                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()) < 1 ?
                                                            (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()) * 100) :
                                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()
                                                            ));

                    ((Literal)e.Item.FindControl("ltrDescParaSTValor")).Text = DataBinder.Eval(e.Item.DataItem, "descST").ToString().Equals("-") ?
                                                    DataBinder.Eval(e.Item.DataItem, "descST").ToString() :
                                                    string.Format("{0:n2}%",
                                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descST").ToString()) < 1 ?
                                                                (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descST").ToString()) * 100) :
                                                                decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descST").ToString()
                                                                )).ToString();

                    ((Literal)e.Item.FindControl("ltrMVAValor")).Text = string.Format("{0:n2}%", mva);

                    ((Literal)e.Item.FindControl("ltrPMC")).Text = string.Format(GetResourceValue("lblPMC18"), decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPmc").ToString()) < 1 ? (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPmc").ToString()) * 100) : decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPmc").ToString()));
                    //  aliquotaInternaICMS

                    ((Literal)e.Item.FindControl("ltrICMSSTValor")).Text = string.Format("{0:n2}",
                                                    GetValorICMS_ST(DataBinder.Eval(e.Item.DataItem, "descST").ToString(),
                                                                    mva,
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS").ToString()),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                                    GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "valorICMSST").ToString()),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()),
                                                                    decimal.Parse(DataBinder.Eval(e.Item.DataItem, "reducaoST_MVA").ToString())
                                                                    ));

                    ((Literal)e.Item.FindControl("ltrAliquotaICMSValor")).Text = string.Format("{0:n2}%",
                                        decimal.Parse(DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS").ToString()) < 1 ?
                                            (decimal.Parse(DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS").ToString()) * 100) :
                                                DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS"));



                    ((Literal)e.Item.FindControl("ltrCustoPadraoValor")).Text = string.Format("{0:n2}",
                        GetValorCustoPadrao(
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percICMSe").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()),
                                            DataBinder.Eval(e.Item.DataItem, "descST").ToString(),
                                           mva,
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                            GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "valorICMSST").ToString()),
                                            decimal.Parse(DataBinder.Eval(e.Item.DataItem, "reducaoST_MVA").ToString()),
                                            DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                            DataBinder.Eval(e.Item.DataItem, "tipo").ToString(),
                                            int.Parse(DataBinder.Eval(e.Item.DataItem, "itemCodigoOrigem").ToString()),
                                            DataBinder.Eval(e.Item.DataItem, "tratamentoICMSEstab").ToString(),
                                            DataBinder.Eval(e.Item.DataItem, "ufIdOrigem").ToString(),
                                            DataBinder.Eval(e.Item.DataItem, "resolucao13").ToString(),
                                            transf
                                            ));


                    ((Literal)e.Item.FindControl("ltrCustoPadraoVendaValor")).Text =
                                                     string.Format("{0:n2}",
                                                     GetValorCustoPadrao(
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percICMSe").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percIPI").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percPisCofins").ToString()),
                                                     DataBinder.Eval(e.Item.DataItem, "descST").ToString(),
                                                    mva,
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "aliquotaInternaICMS").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "pmc17").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                     GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "valorICMSST").ToString()),
                                                     decimal.Parse(DataBinder.Eval(e.Item.DataItem, "reducaoST_MVA").ToString()),
                                                     DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                                     DataBinder.Eval(e.Item.DataItem, "tipo").ToString(),
                                                     int.Parse(DataBinder.Eval(e.Item.DataItem, "itemCodigoOrigem").ToString()),
                                                     DataBinder.Eval(e.Item.DataItem, "tratamentoICMSEstab").ToString(),
                                                     DataBinder.Eval(e.Item.DataItem, "ufIdOrigem").ToString(),
                                                     DataBinder.Eval(e.Item.DataItem, "resolucao13").ToString(),
                                                     transf
                                                     ));




                    ((Literal)e.Item.FindControl("ltrCreditoICMSValor")).Text = string.Format("{0:n2}",
                                                                                                               GetCreditoICMS(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percReducaoBase").ToString()),
                                                                                                                              decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percICMSe").ToString()),
                                                                                                                              decimal.Parse(DataBinder.Eval(e.Item.DataItem, "precoFabrica").ToString()),
                                                                                                                              GetDescontoComercial(decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoComercial").ToString())),
                                                                                                                              decimal.Parse(DataBinder.Eval(e.Item.DataItem, "descontoAdicional").ToString()),
                                                                                                                              decimal.Parse(DataBinder.Eval(e.Item.DataItem, "percRepasse").ToString()),
                                                                                                                                int.Parse(DataBinder.Eval(e.Item.DataItem, "itemCodigoOrigem").ToString()),
                                                                                                                                 DataBinder.Eval(e.Item.DataItem, "tratamentoICMSEstab").ToString(),
                                                                                                                                 DataBinder.Eval(e.Item.DataItem, "estabelecimentoId").ToString(),
                                                                                                                                 DataBinder.Eval(e.Item.DataItem, "ufIdOrigem").ToString(),
                                                                                                                                 DataBinder.Eval(e.Item.DataItem, "resolucao13").ToString(),
                                                                                                                                  DataBinder.Eval(e.Item.DataItem, "tipo").ToString()
                                                                                                                              ));


                    ((Literal)e.Item.FindControl("ltrTransESValor")).Text = string.Format("{0:n2}", transf);

                }
                else if (e.Item.ItemType.Equals(ListItemType.Header))
                {
                    Session.Add("GvSumarizacao", ((GridView)e.Item.FindControl("gvSumarizacao")));
                }
            }
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

        #endregion

        #endregion

        #region :: Métodos ::

        #region :: Gerais ::

        /// <summary>
        /// Adiciona scripts aos componentes da página web
        /// </summary>
        private void AddScripts()
        {
            this.rfvItem.ErrorMessage = GetResourceValue("msgInformeItem");

            AddCurrencyScript(this.txtDescontoAdicional,
                              this.txtMargemObjetivo,
                              this.txtPrecoObjetivo,
                              this.txtCapAplicado,
                              this.txtDescontoObjetivo);

            AddVerifyDropScript(this.txtItem, new object[,] { });

            #region :: Check ::

            AddVerifyDropScript(this.txtItem, new object[,] { });

            #endregion

            AddSetAutoCompleteNewParameterScript(ddlaceItem, txtItem);
        }

        /// <summary>
        /// Recupera os dados referentes ao item
        /// </summary>
        private void GetItem()
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtItem.Text))
                {
                    //Alert(GetResourceValue("msgInformeCodigoItem"));
                    PopularModalErro(GetResourceValue("msgInformeCodigoItem"));
                    return;
                }

                SimuladorPrecoCustos sPrc =
                    new SimuladorPrecoCustos
                    {
                        itemId = this.txtItem.Text.PadLeft(5, '0')
                    }.GetItem();


                if (sPrc != null)
                {

                    if (sPrc.itemObsoleto)
                    {
                        //Alert(GetResourceValue("msgItemObsoleto"));
                        PopularModalErro(GetResourceValue("msgItemObsoleto"));
                        return;

                    }

                    Clean(
                        lblUfOrigem,
                        lblFornecedor,
                        lblLista,
                        lbltipo,
                        lblNCM,
                        lblMedicamento,
                        lblUsoExclusivo,
                        lblResolucao13,
                        txtPrecoObjetivo,
                        txtCapAplicado,
                        txtDescontoObjetivo,
                        txtMargemObjetivo,
                        txtDescontoAdicional,


                        lblDescricao,
                          lblUfOrigem,
                          lblFornecedor,
                          lblLista,
                          lblCategoria,
                          lbltipo,
                          lblClassificFiscal,
                          lblNCM,
                          lblUsoExclusivo,
                          lblResolucao13,
                          lblDesconto,
                          lblMedicamento,
                          txtCapAplicado);

                    if (sPrc != null)
                    {
                        #region :: ViewState ::

                        this.CapAplicado = sPrc.capAplicado;

                        #endregion

                        this.lblDescricao.Text = sPrc.itemDescricao;
                        this.lblUfOrigem.Text = sPrc.ufIdOrigem;
                        this.lblFornecedor.Text = sPrc.laboratorioNome;
                        this.lblLista.Text = sPrc.listaDescricao;
                        this.lblCategoria.Text = sPrc.categoria;
                        this.lbltipo.Text = sPrc.tipo;
                        this.lblClassificFiscal.Text = sPrc.classificFiscal;

                        this.lblNCM.Text = sPrc.NCM;

                        this.txtCapAplicado.Text =
                            string.Format("{0:n2}", (sPrc.capDescontoPrc > 1) ? sPrc.capDescontoPrc : ((sPrc.capDescontoPrc) * 100));

                        this.lblUsoExclusivo.Text =
                            new Regex(Utility.FormataStringPesquisa(sPrc.exclusivoHospitalar), RegexOptions.IgnoreCase).IsMatch("NÃO") ?
                                GetResourceValue("lblNao") :
                                    GetResourceValue("lblSim");

                        this.lblResolucao13.Text =
                            IsEquals(sPrc.resolucao13, "NÃO") ?
                                GetResourceValue("lblNao") :
                                    GetResourceValue("lblSim");

                        this.lblDesconto.Text = string.Format("{0:n2}%",
                                                        sPrc.descontoComercial < 1 ?
                                                            (sPrc.descontoComercial * 100) :
                                                                sPrc.descontoComercial);

                        this.lblMedicamento.Text =
                            sPrc.itemControlado ? GetResourceValue("lblSim") : GetResourceValue("lblNao");
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

        private bool ValidarPreencimentoCampos()
        {

            List<int> count = new List<int>();

            if (String.IsNullOrEmpty(this.txtItem.Text))
            {
                PopularModalErro(GetResourceValue("msgInformeItem"));
                this.txtItem.Focus();
                return false;
            }

            if (!HasItemSelected(rblPerfilCliente.Items))
            {
                //Alert(GetResourceValue("msgInformePerfilCliente"));
                PopularModalErro(GetResourceValue("msgInformePerfilCliente"));
                this.rblPerfilCliente.Focus();
                return false;
            }

            if (txtDescontoObjetivo.Text == "" && txtMargemObjetivo.Text == "" && txtPrecoObjetivo.Text == "")
            {
                PopularModalErro("Por favor informe o Desconto Objetivo, Margem Objetivo ou Preço Objetivo");
                this.txtDescontoObjetivo.Focus();
                Clean(
                      txtDescontoAdicional,
                      txtDescontoObjetivo,
                      txtPrecoObjetivo,
                      txtCalculaPrePrecoObjetivo,
                      txtMargemObjetivo,
                      ddlaceItem,
                      rblPerfilCliente);

                return false;
            }


            if (txtDescontoObjetivo.Text != "")
                count.Add(1);
            if (txtMargemObjetivo.Text != "")
                count.Add(1);
            if (txtPrecoObjetivo.Text != "")
                count.Add(1);

            if (count.Count > 1)
            {
                PopularModalErro("Por favor informe o Desconto Objetivo, Margem Objetivo ou Preço Objetivo");
                this.txtDescontoObjetivo.Focus();

                Clean(txtDescontoAdicional, txtDescontoObjetivo, txtPrecoObjetivo, txtMargemObjetivo);

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

        /// <summary>
        /// Efetuar a simulação de preços
        /// </summary>
        private void Simular()
        {
            try
            {
                if (ValidarPreencimentoCampos())
                {

                    List<SimuladorPrecoCustos> _lst =
                        lstCustos =
                            new SimuladorPrecoCustos
                            {
                                _isAdm = UserDataInfo.IsAdm,
                                itemId = this.txtItem.Text.PadLeft(5, '0'),
                                unidadeNegocioId = UserDataInfo.UserUnidadeNegocio,
                                usuarioId = UserDataInfo.UserID

                            }.GetCustos();


                    foreach (var item in _lst)
                    {
                        if (item.estabelecimentoId.Equals("4") && item.VendaComST.Equals(false) && ddlEstadoDestino.SelectedValue.Equals("SP"))
                        {
                            _lst.Remove(item);
                            break;
                        }
                        else if (item.estabelecimentoId.Equals("4") && item.VendaComST.Equals(true) && !ddlEstadoDestino.SelectedValue.Equals("SP"))
                        {
                            _lst.Remove(item);
                            break;

                        }



                    }

                    ValidacaoEstabelecimentoTransferencia(_lst);

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

                    //foreach (var item in _lst)
                    //{
                    //    if (rblPerfilCliente.SelectedValue.Equals("C") || rblPerfilCliente.SelectedValue.Equals("2"))
                    //    {
                    //        if (item.transfDf > item.transfEs)
                    //        {
                    //            transf = item.transfDf;
                    //        }

                    //    }
                    //}

                    //foreach (var item in _lst)
                    //{
                    //    if (item.estabelecimentoId.Equals("12") && ddlEstadoDestino.SelectedValue.Equals("SP") && !rblPerfilCliente.SelectedValue.Equals("0"))
                    //    {
                    //        _lst.Remove(item);
                    //        break;
                    //    }
                    //}

                    foreach (var item in _lst)
                    {
                        if (rblPerfilCliente.SelectedValue.Equals("0"))
                        {
                            item.transfEs = new decimal();
                        }
                    }

                    if (_lst != null)
                    {
                        if (_lst.Count > 0)
                        {
                            lstItens = new List<SimuladorSumarizacao>();

                            Clean(dlSimulacoes);

                            dlSimulacoes.DataSource = _lst;

                            dlSimulacoes.DataBind();
                        }
                        else
                            Clean(dlSimulacoes);
                    }
                    else
                        Clean(dlSimulacoes);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                PopularModalErro(GetResourceValue(Utility.ERRORMESSAGE));
                //Alert(Utility.ERRORMESSAGE);
            }
        }

        private static void ValidacaoEstabelecimentoTransferencia(List<SimuladorPrecoCustos> _lst)
        {
            bool removerEstav20 = false;

            foreach (var item in _lst)
            {
                if (item.estabelecimentoId.Equals("30"))
                {
                    removerEstav20 = true;
                }
            }
            foreach (var item in _lst)
            {
                if (item.estabelecimentoId.Equals("20"))
                {

                    if (removerEstav20)
                    {
                        _lst.Remove(item);
                        break;
                    }

                }
            }





        }

        /// <summary>
        /// Recupera o perfil selecionado
        /// </summary>
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

        /// <summary>
        /// Habilita/Desabilita linha(s).
        /// </summary>
        /// <param name="flag">true/false</param>
        /// <param name="dvs">Div's</param>
        private void EnableDisableLine(bool flag, params HtmlGenericControl[] dvs)
        {
            foreach (HtmlGenericControl dv in dvs)
            {
                if (dv != null)
                {
                    dv.Visible = flag;
                }
            }

        }

        #endregion

        #region :: Cálculos de custo padrão ::

        /// <summary>
        /// Recupera o valor do desconto comercial
        /// </summary>
        /// <param name="_descontoComercial">Valor do desconto comercial</param>
        /// <returns></returns>
        protected decimal GetDescontoComercial(decimal _descontoComercial)
        {
            try
            {
                decimal _descontoInformado =
                    !String.IsNullOrEmpty(this.txtDescontoAdicional.Text) ?
                        decimal.Parse(this.txtDescontoAdicional.Text) :
                            new decimal();
                //return ((_descontoComercial > 1 ? _descontoComercial : (_descontoComercial + _descontoInformado)));
                return ((_descontoComercial > 1 ? _descontoComercial : (_descontoComercial * 100)) + _descontoInformado);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }

        /// <summary>
        /// Realiza o cálculo dos descontos adicionais
        /// </summary>
        /// <param name="_descontoAdicional">Valor recuperado na carga</param>
        /// <returns>Valor formatado</returns>
        protected decimal GetDescontoAdicional(decimal _descontoAdicional)
        {
            try
            {
                decimal _descontoCap = new decimal();

                if (IsSetorPublico)
                    _descontoCap =
                        !CapAplicado ? new decimal() :
                            !String.IsNullOrEmpty(this.txtCapAplicado.Text) ?
                                decimal.Parse(this.txtCapAplicado.Text) : new decimal();

                return ((_descontoAdicional > 1 ? _descontoAdicional : (_descontoAdicional * 100)) + ((_descontoCap > 1 ? _descontoCap : (_descontoCap * 100))));
                //return ((_descontoAdicional > 1 ? _descontoAdicional : (_descontoAdicional * 100)) + (_descontoCap / 100));
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }

        /// <summary>
        /// Realiza o cálculo do preço de aquisição
        /// </summary>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto adicional</param>
        /// <param name="_repasse">Valor do repasse</param>
        /// <returns>Cálculo com o valor formatado</returns>
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

        /// <summary>
        /// Calcula o valor do crédito de ICMS
        /// </summary>
        /// <param name="_reducaoBase">Redução base</param>
        /// <param name="_ICMSe">ICMSe</param>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto adicional</param>
        /// <param name="_repasse">Repasse</param>
        /// <returns>Cálculo formato</returns>
        //protected decimal GetCreditoICMS(decimal _reducaoBase, decimal _ICMSe, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse)
        //{
        //    try
        //    {
        //        return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

        //    }
        //    catch (Exception ex)
        //    {
        //        Utility.WriteLog(ex);
        //        return 0;
        //    }
        //}




        protected decimal GetCreditoICMS2(decimal _reducaoBase, decimal _ICMSe, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, int CodigoOrigem, string tratamentoICMSEstab, string _estabelecimento, string UFOrigem, string _resolucao, string ConvenioId)
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

        /// <summary>
        /// Calcula o valor do crédito de ICMS
        /// </summary>
        /// <param name="_reducaoBase">Redução base</param>
        /// <param name="_ICMSe">ICMSe</param>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto adicional</param>
        /// <param name="_repasse">Repasse</param>
        /// <returns>Cálculo formato</returns>
        protected decimal GetCreditoICMS(decimal _reducaoBase, decimal _ICMSe, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, int CodigoOrigem, string tratamentoICMSEstab, string _estabelecimento, string UFOrigem, string _resolucao, string ConvenioId)
        {
            try
            {
                if (_estabelecimento == "12" || _estabelecimento == "13")
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

                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("ES"))
                    {



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
                    else if (_resolucao.Equals("SIM"))
                    {
                        _ICMSe = 4;
                        return (GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse) * (1 - (_reducaoBase > 1 ? (_reducaoBase / 100) : _reducaoBase)) * (_ICMSe > 1 ? (_ICMSe / 100) : _ICMSe));

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
                                    //_ICMSe = 12;
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

        /// <summary>
        /// Calcula o valor do IPI
        /// </summary>
        /// <param name="_percIPI">Percentual IPI</param>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto Adicional</param>
        /// <param name="_repasse">Valor do repasse</param>
        /// <returns>Cálculo formatado</returns>
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

        /// <summary>
        /// Calcula o valor do PIS/Cofins
        /// </summary>
        /// <param name="_percPISCofins">Percentual do PIS/Cofins</param>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto adicional</param>
        /// <param name="_repasse">Valor do repase</param>
        /// <returns>Cálculo formatado</returns>
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

        /// <summary>
        /// Calculo do valor do ICMS-ST
        /// </summary>
        /// <param name="_descST">Descrição ST</param>
        /// <param name="_mva">Valor MVA</param>
        /// <param name="_aliquotaIntICMS">Aliquota do ICMS</param>
        /// <param name="_pmc17">PMC</param>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto Adicional</param>
        /// <param name="_repasse">Valor do repasse</param>
        /// <param name="_valorICMSST">Valor do IcmsSt</param>
        /// <returns>Cálculo formatado</returns>
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

        /// <summary>
        /// Calcula o valor do custo padrão
        /// </summary>
        /// <param name="_reducaoBase">Redução base</param>
        /// <param name="_ICMSe">ICMSe</param>
        /// <param name="_percIPI">Percentual IPI</param>
        /// <param name="_percPISCofins">Percentual PIS/Cofins</param>
        /// <param name="_descST">DescST</param>
        /// <param name="_mva">MVA</param>
        /// <param name="_aliquotaIntICMS">Alíquota ICMS</param>
        /// <param name="_pmc17">PMC</param>
        /// <param name="_precoFabrica">Preço fábrica</param>
        /// <param name="_descontoComercial">Desconto comercial</param>
        /// <param name="_descontoAdicional">Desconto Adicional</param>
        /// <param name="_repasse">Valor repasse</param>
        /// <param name="_valorICMSST">Valor IcmsSt</param>
        /// <param name="_reducaoST_MVA"></param>
        /// <param name="_estabelecimento"></param>
        /// <param name="_convenio"></param>
        /// <returns>Cálculo formatado</returns>
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

                        //if (_estabelecimento.Equals("20"))
                        //{
                        //    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                        //    {
                        //        _ICMSe = Utility.VLR_CUSTOPADRAOICMSE;


                        //    }
                        //    else
                        //    {
                        //        if (!IsContribuinte)
                        //            _ICMSe = Utility.VLR_CUSTOPADRAOICMSE;
                        //    }
                        //}

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

        protected decimal GetPercICMSe2(decimal percICMSe, string estabelecimento, string UFOrigem, string _resolucao, string ConvenioId)
        {
            if (estabelecimento == "12" || estabelecimento == "13")
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


                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals(UFOrigem))
                    {

                        percICMSe = 17;
                        return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

                    }

                    else if (_resolucao.Equals("SIM") && regiaoNO)
                    {
                        percICMSe = 4;
                        return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

                    }
                    else if (_resolucao.Equals("NAO") && regiaoNO)
                    {
                        percICMSe = 12;
                        return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

                    }

                    else if (_resolucao.Equals("SIM") && regiaoSUL)
                    {
                        percICMSe = 4;
                        return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

                    }
                    else if (_resolucao.Equals("NAO") && regiaoSUL)
                    {
                        percICMSe = 7;
                        return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

                    }
                }
                else if (_resolucao.Equals("SIM"))
                {
                    percICMSe = 4;
                    return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

                }
            }
            else
            {


                if (estabelecimento == "20")
                {
                    if (this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                    {
                        if (ConvenioId.ToUpper().Equals("REGIME NORMAL"))
                        {
                            return percICMSe < 1 ? (percICMSe * 100) : percICMSe;
                        }
                        else
                        {
                            return percICMSe < 1 ? (percICMSe * 100) : percICMSe;
                        }
                    }
                    else if (!this.ddlEstadoDestino.SelectedValue.ToUpper().Equals("DF"))
                    {

                        // 0 - Mercado Público                            1 - Mercado Privado Não Contribuinte
                        if (rblPerfilCliente.SelectedValue.Equals("0") || rblPerfilCliente.SelectedValue.Equals("1") || rblPerfilCliente.SelectedValue.Equals("2") || rblPerfilCliente.SelectedValue.Equals("C"))
                        {

                            if (ConvenioId.Equals("Convênio 118") || ConvenioId.Equals("Convênio 57"))
                            {
                                percICMSe = 0;
                                return percICMSe < 1 ? (percICMSe * 100) : percICMSe;
                            }
                        }
                        else
                        {
                            return percICMSe < 1 ? (percICMSe * 100) : percICMSe;
                        }

                    }
                    else
                    {
                        return percICMSe < 1 ? (percICMSe * 100) : percICMSe;
                    }
                }
                else
                {
                    return percICMSe < 1 ? (percICMSe * 100) : percICMSe;
                }
            }




            return percICMSe < 1 ? (percICMSe * 100) : percICMSe;

        }


        #endregion

        #region :: Cálculos sobre venda ::

        protected decimal GetAquisicaoPorEstabelecimento(string _estabelecimento)
        {


            SimuladorPrecoCustos oCst =
                          lstCustos
                              .Where(x =>
                                      x.itemId.Equals(this.txtItem.Text.PadLeft(5, '0')) &&
                                      x.estabelecimentoId.Equals(_estabelecimento)
                                    )
                              .SingleOrDefault();
            // return ((oCst.precoFabrica) - ((oCst.precoFabrica) * (_descontoObjetivo)));

            return GetPrecoAquisicao(oCst.precoFabrica, oCst.descontoComercial, oCst.descontoAdicional, oCst.percRepasse);

        }

        protected decimal GetRedutorPorEstabelecimento(string _estabelecimento)
        {


            SimuladorPrecoCustos oCst =
                          lstCustos
                              .Where(x =>
                                      x.itemId.Equals(this.txtItem.Text.PadLeft(5, '0')) &&
                                      x.estabelecimentoId.Equals(_estabelecimento)
                                    )
                              .SingleOrDefault();
            return oCst.percReducaoBase;

        }

        protected string GetPrecoVendaDesconto(string _estabelecimentoId)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtDescontoObjetivo.Text))
                {
                    decimal _descontoObjetivo =
                        decimal.Parse(this.txtDescontoObjetivo.Text) > 1 ?
                            ((decimal.Parse(this.txtDescontoObjetivo.Text)) / 100) :
                                decimal.Parse(this.txtDescontoObjetivo.Text);

                    SimuladorPrecoCustos oCst =
                        lstCustos
                            .Where(x =>
                                    x.itemId.Equals(this.txtItem.Text.PadLeft(5, '0')) &&
                                    x.estabelecimentoId.Equals(_estabelecimentoId)
                                  )
                            .SingleOrDefault();

                    switch (_estabelecimentoId)
                    {
                        #region :: Distribuidoras ::

                        case "2":
                        case "3":
                        case "5":
                        case "10":
                        case "11":
                        case "20":
                        case "30":
                        case "31":
                        case "40":
                        case "90":

                            return (
                                    (oCst.precoFabrica)
                                        -
                                    (
                                        (oCst.precoFabrica)
                                            *
                                        (_descontoObjetivo)
                                    )
                                   ).ToString();

                        #endregion

                        #region :: Drogarias ::

                        case "4":
                        case "7":
                        case "8":
                        case "91":

                            return (
                                    (oCst.pmc17)
                                        -
                                    (
                                        (oCst.pmc17)
                                            *
                                        (_descontoObjetivo)
                                    )
                                   ).ToString();

                        #endregion

                        default:
                            return this.txtPrecoObjetivo.Text;
                    }
                }
                else
                    return this.txtPrecoObjetivo.Text;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return this.txtPrecoObjetivo.Text;
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


        protected decimal GetICMSSobreVenda(string _valorVenda, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, string _usoExclusivo, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, decimal _valorICMSST, decimal _percIPI, decimal _reducaoST_MVA, string ufIdOrigem)
        {
            try
            {
                #region :: Versão anterior ::

                /*
                SimuladorPrecoRegrasGerais oRg =
                    new SimuladorPrecoRegrasGerais
                        {
                            estabelecimentoId = _estabelecimento,
                            convenioId = _convenioId,
                            perfilCliente = _perfil,
                            resolucaoId = _resolucao,
                            ufDestino = this.ddlEstadoDestino.SelectedValue,
                            usoExclusivoHospitalar = Utility.FormataStringPesquisa(_usoExclusivo)

                        }.GetRegras();

                if (oRg != null)
                    return ((oRg.icmsSobreVenda > 1 ? (oRg.icmsSobreVenda / 100) : oRg.icmsSobreVenda) * GetPrecoVenda(_valorVenda));
                else
                    return 0;
                */

                #endregion

                List<SimuladorPrecoRegrasGerais> oRg =
                    new SimuladorPrecoRegrasGerais
                    {
                        estabelecimentoId = _estabelecimento,
                        convenioId = Utility.FormataStringPesquisa(_convenioId),
                        perfilCliente = _perfil,
                        resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                        ufDestino = this.ddlEstadoDestino.SelectedValue,
                        usoExclusivoHospitalar = Utility.FormataStringPesquisa(_usoExclusivo)

                    }.GetRegrasICMSVenda();

                if (oRg != null)
                {

                    if (_estabelecimento.Equals("20"))
                    {

                        if (_convenioId.Equals("Convênio 118"))
                        {
                        }
                    }


                    SimuladorPrecoRegrasGerais _oRg = null;

                    #region :: Valida o cálculo de ICMS de saída de acordo com a ST de entreda ::

                    if (
                        GetValorICMS_ST(_descST,
                                        _mva,
                                        _aliquotaIntICMS,
                                        _pmc17,
                                        _precoFabrica,
                                        _descontoComercial,
                                        _descontoAdicional,
                                        _repasse,
                                        _valorICMSST,
                                        _percIPI,
                                        _reducaoST_MVA
                                       ) > 0
                       )
                        _oRg = oRg.Where(x => x._icmsStValor.GetValueOrDefault(-1).Equals(0)).SingleOrDefault();
                    else
                        _oRg = oRg.Where(x => x._icmsStValor == null).SingleOrDefault();

                    #endregion



                    return ((_oRg.icmsSobreVenda > 1 ? (_oRg.icmsSobreVenda / 100) : _oRg.icmsSobreVenda) * GetPrecoVenda(_valorVenda));
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

        public decimal GetIcmsSobreVendaRegraPE(string ItemId, string contribuinte, string estabelecimentoId, string UFDestino, string UfOrigem, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse)
        {
            decimal IcmsSobrePrecoAquisicao = 0;
            try
            {
                SimuladorPrecoRegraPE RegraPE = new SimuladorPrecoRegraPE();
                RegraPE.codigoItem = ItemId;
                RegraPE.contribuinte = contribuinte;
                RegraPE.estabelecimentoId = estabelecimentoId;
                RegraPE.uFOrigemFornec = UfOrigem;
                RegraPE.ufDestinoCliente = UFDestino;

                DataTable dt = RegraPE.ObterTodosFiltro();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        decimal Encargos = !string.IsNullOrEmpty(dt.Rows[0]["encargos"].ToString()) ? decimal.Parse(dt.Rows[0]["encargos"].ToString()) : 0;
                        IcmsSobrePrecoAquisicao = Encargos * GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse);

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


        public decimal GetIcmsSobreVendaRegraSpin(string ItemId, string contribuinte, string estabelecimentoId, string UFDestino, string UfOrigem, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse)
        {
            decimal IcmsSobrePrecoAquisicao = 0;
            try
            {
                SimuladorPrecoRegraPE regraSpinraza = new SimuladorPrecoRegraPE();
                regraSpinraza.codigoItem = ItemId;
                regraSpinraza.estabelecimentoId = estabelecimentoId;
                regraSpinraza.ufDestinoCliente = UFDestino;
                regraSpinraza.contribuinte = contribuinte;
                DataTable dt = regraSpinraza.ObterRegrasSPINRAZA();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        decimal Encargos = !string.IsNullOrEmpty(dt.Rows[0]["encargos"].ToString()) ? decimal.Parse(dt.Rows[0]["encargos"].ToString()) : 0;
                        IcmsSobrePrecoAquisicao = Encargos * _precoFabrica;

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


        protected decimal GetICMSSTSobreVenda(string _valorVenda, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, string _lista, string _categoria, string _uf, decimal _pmc17, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, decimal _percPISCofins, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, decimal _valorICMSST, decimal _reducaoST_MVA, string _usoExclusivo, bool _hasData, SimuladorPrecoRegrasGerais _oRg, bool embutirICMSST)
        {
            try
            {
                SimuladorPrecoRegrasGerais oRg = null;

                if (!_hasData)
                {
                    oRg =
                        new SimuladorPrecoRegrasGerais
                        {
                            estabelecimentoId = _estabelecimento,
                            convenioId = Utility.FormataStringPesquisa(_convenioId),
                            perfilCliente = _perfil,
                            resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                            ufDestino = this.ddlEstadoDestino.SelectedValue

                        }.GetRegras();
                }
                else
                    oRg = _oRg;

                if (oRg != null)
                {
                    #region :: Estabelecimento 02 ::

                    if (_estabelecimento.PadLeft(2, '0').Equals("02"))
                    {
                        switch (this.ddlEstadoDestino.SelectedValue.ToUpper())
                        {
                            case "RS":

                                if (IsEquals(_convenioId.Trim(), "REGIME NORMAL") &&
                                       IsEquals(GetPerfil(_perfil.Trim()), "CONTRIBUINTE"))
                                {
                                    #region :: PMC17 > 0 ::

                                    if (_pmc17 > 0)
                                    {
                                        switch (GetEquality(_categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                        {
                                            case "SIMILAR":

                                                if (oRg.icmsStSobreVenda != 0)
                                                {
                                                    return (
                                                            (_pmc17
                                                                *
                                                             Utility.VLR_STRSSIMILAR
                                                                *
                                                             (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                -
                                                            GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                           );
                                                }
                                                else
                                                    return oRg.icmsStSobreVenda;
                                            case "GENÉRICO":
                                                if (oRg.icmsStSobreVenda != 0)
                                                {
                                                    return (
                                                            (_pmc17
                                                                *
                                                             Utility.VLR_STRSGENERICO
                                                                *
                                                             (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                -
                                                            GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                           );
                                                }
                                                else
                                                    return oRg.icmsStSobreVenda;
                                            default:
                                                if (oRg.icmsStSobreVenda != 0)
                                                {
                                                    return (
                                                            (_pmc17
                                                                *
                                                             Utility.VLR_STRSNAOSIMILAR
                                                                *
                                                             (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                -
                                                            GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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

                                        if (IsEquals(_lista.Trim(), "POSITIVA"))
                                        {
                                            switch (GetEquality(_categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        decimal valor1 = 0;

                                                        valor1 = (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVASIMILAR
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVASIMILARPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                );

                                                        decimal icmsSobVenda = GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf);
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
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVAGENERICO
                                                                        *
                                                                    Utility.VLR_STRSPOSITIVAGENERICOPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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
                                                                     GetPrecoVenda(_valorVenda)
                                                                         *
                                                                     Utility.VLR_STRSPOSITIVAOUTROS
                                                                         *
                                                                     Utility.VLR_STRSPOSITIVAOUTROSPRC
                                                                         *
                                                                     (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                 )
                                                                         -
                                                                     GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                                );


                                                        return valor;
                                                    }

                                                    else
                                                        return oRg.icmsStSobreVenda;


                                                    //(
                                                    //    (
                                                    ////GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                    //        GetPrecoVenda(_valorVenda)
                                                    //            *
                                                    //        Utility.VLR_STRSPOSITIVAOUTROS
                                                    //            *
                                                    //        Utility.VLR_STRSPOSITIVAOUTROSPRC
                                                    //            *
                                                    //        (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                    //    )
                                                    //            -
                                                    //        GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA)
                                                    //   );
                                            }
                                        }

                                        #endregion

                                        #region :: Negativa ::

                                        else if (IsEquals(_lista.Trim(), "NEGATIVA"))
                                        {
                                            switch (GetEquality(_categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {

                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVASIMILAR
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVASIMILARPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAGENERICO
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAGENERICOPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAOUTROS
                                                                        *
                                                                    Utility.VLR_STRSNEGATIVAOUTROSPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                               );
                                                    }
                                                    else return oRg.icmsStSobreVenda;
                                            }
                                        }

                                        #endregion

                                        #region :: Neutra ::

                                        else if (IsEquals(_lista.Trim(), "NEUTRA"))
                                        {
                                            switch (GetEquality(_categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    return (
                                                            (
                                                                //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                GetPrecoVenda(_valorVenda)
                                                                    *
                                                                Utility.VLR_STRSNEUTRASIMILAR
                                                                    *
                                                                Utility.VLR_STRSNEUTRASIMILARPRC
                                                                    *
                                                                (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                    -
                                                                GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                           );

                                                case "GENÉRICO":
                                                    return (
                                                            (
                                                                //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                GetPrecoVenda(_valorVenda)
                                                                    *
                                                                Utility.VLR_STRSNEUTRAGENERICO
                                                                    *
                                                                Utility.VLR_STRSNEUTRAGENERICOPRC
                                                                    *
                                                                (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                    -
                                                                GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                           );

                                                default:
                                                    return (
                                                            (
                                                                //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                GetPrecoVenda(_valorVenda)
                                                                    *
                                                                Utility.VLR_STRSNEUTRAOUTROS
                                                                    *
                                                                Utility.VLR_STRSNEUTRAOUTROSPRC
                                                                    *
                                                                (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                            )
                                                                    -
                                                                GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                           );
                                            }
                                        }

                                        #endregion

                                        #region :: Sem Lista ::

                                        else
                                        {
                                            switch (GetEquality(_categoria.Trim(), "SIMILAR", "GENÉRICO"))
                                            {
                                                case "SIMILAR":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTASIMILAR
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTASIMILARPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                               );
                                                    }
                                                    else return oRg.icmsStSobreVenda;

                                                case "GENÉRICO":
                                                    if (oRg.icmsStSobreVenda != 0)
                                                    {
                                                        return (
                                                                (
                                                                    //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAGENERICO
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAGENERICOPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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
                                                                    GetPrecoVenda(_valorVenda)
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAOUTROS
                                                                        *
                                                                    Utility.VLR_STRSSEMLISTAOUTROSPRC
                                                                        *
                                                                    (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                                )
                                                                        -
                                                                    GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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

                    else if (_estabelecimento.Equals("31"))
                    {
                        if (embutirICMSST)
                        {
                            switch (this.ddlEstadoDestino.SelectedValue.ToUpper())
                            {
                                case "RJ":

                                    if ((IsEquals(_convenioId.Trim(), "REGIME NORMAL", "DERMOCOSMÉTICOS"))
                                                &&
                                            IsEquals(GetPerfil(_perfil.Trim()), "CONTRIBUINTE"))
                                    {
                                        if (IsEquals(_lista.Trim(), "POSITIVA"))

                                            if (oRg.icmsStSobreVenda != 0)
                                            {
                                                return (
                                                        (
                                                         //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                         GetPrecoVenda(_valorVenda)
                                                            *
                                                         Utility.VLR_STRJLISTAPOSITIVA
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                        GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                       );
                                            }
                                            else
                                                return oRg.icmsStSobreVenda;

                                        else if (IsEquals(_lista.Trim(), "NEGATIVA"))
                                            if (oRg.icmsStSobreVenda != 0)
                                            {

                                                return (
                                                        (
                                                         //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                         GetPrecoVenda(_valorVenda)
                                                            *
                                                         Utility.VLR_STRJLISTANEGATIVA
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                        GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                                       );
                                            }
                                            else
                                                return oRg.icmsStSobreVenda;

                                        else if (IsEquals(_lista.Trim(), "NEUTRA"))
                                            if (oRg.icmsStSobreVenda != 0)
                                            {
                                                return (
                                                        (
                                                         //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                         GetPrecoVenda(_valorVenda)
                                                            *
                                                         Utility.VLR_STRJLISTANEUTRA
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                        GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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
                                                         //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                                         GetPrecoVenda(_valorVenda)
                                                            *
                                                         Utility.VLR_STRJLISTASEM
                                                            *
                                                         (oRg.icmsStSobreVenda > 1 ? (oRg.icmsStSobreVenda / 100) : oRg.icmsStSobreVenda)
                                                        )
                                                            -
                                                        GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _usoExclusivo, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
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


        protected decimal GetLei5005(decimal precoVenda, string convenio, string perfil, decimal precoAquisicao, decimal icmsSobreVenda, decimal icmsSE, string estadoDestino, string resolucao, bool regraTransferencia)
        {
            try
            {


                if (regraTransferencia)
                {

                    precoAquisicao = GetAquisicaoPorEstabelecimento("3");

                }


                decimal retorno = new decimal();

                //REGRAS PARA ICMS SOBRE VENDA
                if (!estadoDestino.Equals("DF") && resolucao.ToUpper().Equals("SIM"))
                {
                    icmsSobreVenda = 4;
                    icmsSobreVenda = icmsSobreVenda / 100;

                }
                else if (estadoDestino.Equals("DF"))
                {
                    icmsSobreVenda = icmsSobreVenda;
                }
                else
                {
                    icmsSobreVenda = (12);

                    icmsSobreVenda = icmsSobreVenda / 100;
                }


                //REGRA PARA ICMS DE COMPRA 
                if (resolucao.ToUpper().Equals("SIM"))
                {
                    icmsSE = 4;
                    icmsSE = icmsSE / 100;
                }
                else
                {
                    icmsSE = 7;
                    icmsSE = icmsSE / 100;
                }

                decimal calculo12 = 12;
                decimal calculo13 = 13;


                if (convenio.ToUpper().Equals("REGIME NORMAL") && !perfil.Equals("2"))
                {

                    decimal part1 = (precoVenda * (icmsSobreVenda));
                    decimal part2 = (precoAquisicao * (icmsSE));
                    decimal part3 = (precoVenda * (calculo13 / 100));
                    decimal part4 = (precoAquisicao * (calculo12 / 100));


                    retorno = ((part1 - part2) - (part3 - part4));

                    return retorno;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }


        /// <summary>
        /// Calcula o ajuste do regime fiscal sobre venda
        /// </summary>
        /// <param name="_valorVenda">Valor venda</param>
        /// <param name="_estabelecimento">Estabelecimento</param>
        /// <param name="_convenioId">Convênio</param>
        /// <param name="_perfil">Perfil</param>
        /// <param name="_resolucao">Resolução 13</param>
        /// <returns>Cálculo formatado</returns>
        public decimal GetAjusteRegimeFiscalSobreVenda(string _valorVenda, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, decimal _percPISCofins, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, string _uf, string _exclusivoHospitalar, decimal _valorICMSST, decimal _reducaoST_MVA)
        {
            try
            {
                SimuladorPrecoRegrasGerais oRg =
                    new SimuladorPrecoRegrasGerais
                    {
                        estabelecimentoId = _estabelecimento,
                        convenioId = Utility.FormataStringPesquisa(_convenioId),
                        perfilCliente = _perfil,
                        resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                        ufDestino = this.ddlEstadoDestino.SelectedValue,
                        usoExclusivoHospitalar = Utility.FormataStringPesquisa(_exclusivoHospitalar)

                    }.GetRegras();

                if (oRg != null)
                {
                    switch (GetEquality(_convenioId.Trim(), "DERMOCOSMETICOS", "REGIME NORMAL"))
                    {
                        case "DERMOCOSMETICOS":
                        case "REGIME NORMAL":

                            return
                                    !_estabelecimento.Equals("11") ?

                                    (
                                       //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                       GetPrecoVenda(_valorVenda)

                                            *
                                        //(Utility.VLR_BASEAJUSTEREGIMEFISCAL)
                                        //  *
                                        (oRg.ajusteRegimeFiscal > 1 ? (oRg.ajusteRegimeFiscal / 100) : oRg.ajusteRegimeFiscal)
                                    )
                                        :
                                    (
                                        (
                                            //GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPISCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId)
                                            GetPrecoVenda(_valorVenda)
                                                *
                                            //(Utility.VLR_BASEAJUSTEREGIMEFISCAL)
                                            //   *
                                            (oRg.ajusteRegimeFiscal > 1 ? (oRg.ajusteRegimeFiscal / 100) : oRg.ajusteRegimeFiscal)
                                        )
                                            *
                                        (-1)
                                    );
                        default:
                            return ((oRg.ajusteRegimeFiscal > 1 ? (oRg.ajusteRegimeFiscal / 100) : oRg.ajusteRegimeFiscal) * GetPrecoVenda(_valorVenda)); //return 0;
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


        protected decimal GetPrecoVendaLiquido(string _valorVenda, decimal _percPisCofins, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, string _uf, string _exclusivoHospitalar, decimal _valorICMSST, decimal _reducaoST_MVA, string _lista, string _categoria)
        {
            try
            {
                SimuladorPrecoRegrasGerais oRg =
                    new SimuladorPrecoRegrasGerais
                    {
                        estabelecimentoId = _estabelecimento,
                        convenioId = Utility.FormataStringPesquisa(_convenioId),
                        perfilCliente = _perfil,
                        resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                        ufDestino = this.ddlEstadoDestino.SelectedValue

                    }.GetRegras();

                if (oRg != null)
                {
                    return (
                            GetPrecoVenda(_valorVenda)
                                -
                            GetICMSSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _exclusivoHospitalar, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf)
                                -
                            GetICMSSTSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _lista, _categoria, _uf, _pmc17, _reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _exclusivoHospitalar, false, null, false)
                                +
                            GetAjusteRegimeFiscalSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA)
                                -
                          (_percPisCofins > 1 ? (_percPisCofins / 100) : _percPisCofins) * GetPrecoVenda(_valorVenda) // ((_percPisCofins / 100) * GetPrecoVenda(_valorVenda))
                           );
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


        protected decimal GetValorMagem(string _valorVenda, decimal _percPisCofins, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, decimal _valorICMSST, decimal _reducaoST_MVA, string _uf, string _exclusivoHospitalar, string _lista, string _categoria, int CodigoOrigem, string tratamentoICMSEstab, decimal transfES)
        {
            try
            {
                return (
                        (GetPrecoVendaLiquido(_valorVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria))
                            -
                        (GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, CodigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES))
                       );
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }


        protected decimal GetValorMagemSpin(string _valorVenda, decimal _percPisCofins, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI,
            string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, decimal _valorICMSST, decimal _reducaoST_MVA, string _uf, string _exclusivoHospitalar, string _lista, string _categoria, int CodigoOrigem, string tratamentoICMSEstab, decimal transfES, decimal _custoPadrao, string contribuinte)
        {
            try
            {
                return (
                        (decimal.Parse(_valorVenda) - GetIcmsSobreVendaRegraSpin(txtItem.Text, contribuinte, _estabelecimento, ddlEstadoDestino.SelectedValue, "", decimal.Parse(_valorVenda), _descontoComercial, _descontoAdicional, _repasse) - ValorPISCofins)
                            -
                        _custoPadrao
                       );
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }

        protected decimal? GetPercentualMargem(string _valorVenda, decimal _percPisCofins, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, decimal
            _reducaoBase, decimal _ICMSe, decimal _percIPI, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial,
            decimal _descontoAdicional, decimal _repasse, DataListItemEventArgs e, string _lista, string _categoria, string _uf, string _exclusivoHospitalar, decimal _valorICMSST,
            decimal _reducaoST_MVA, int codigoOrigem, string tratamentoICMSEstab, decimal transfES)
        {
            decimal cemPercent = 100;
            decimal? _margem = decimal.Zero;

            try
            {
                bool regraTransferencia = false;
                if (_estabelecimento.Equals("30"))
                {
                    _estabelecimento = "20";
                    regraTransferencia = true;
                }


                //CHAVE PMPF
                bool AtivaPMPF = bool.Parse(ConfigurationManager.AppSettings["AtivaPMPF"].ToString());

                decimal _margemObjetivo = new decimal();
                decimal lei5005 = new decimal();
                bool regimeEspecial = this.ValidaRegimeEspecial(_categoria);
                string ItemId = this.txtItem.Text.PadLeft(5, '0');
                decimal icmsSobreVenda = new decimal();
                string contribuinte = this.rblPerfilCliente.SelectedValue;
                string estabelecimentoId = _estabelecimento;
                string UFDestino = this.ddlEstadoDestino.SelectedValue;
                string UfOrigem = _uf;
                decimal _vlrIcmsSTVenda = new decimal();
                decimal ValorComST = new decimal();
                decimal icmsPEValor = new decimal();
                bool PossuiST = false;
                decimal _custoPadrao = this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES);
                decimal PisCofins = (_percPisCofins > decimal.One ? _percPisCofins / new decimal(100) : _percPisCofins);

                //CALCULA POR MARGEM
                if (!string.IsNullOrEmpty(this.txtMargemObjetivo.Text))
                {

                    _margemObjetivo = decimal.Parse(this.txtMargemObjetivo.Text);
                    decimal _vlrVenda = new decimal();
                    decimal _vlrIcms = new decimal();

                    if (this.txtMargemObjetivo.Text.Contains("-"))
                    {
                        decimal result = new decimal();
                        if (decimal.TryParse(this.txtMargemObjetivo.Text.Replace('.', ','), out result))
                        {
                            if (!result.Equals(decimal.Zero))
                            {
                                _margemObjetivo = (result * decimal.MinusOne) * decimal.MinusOne;
                            }
                        }
                    }

                    SimuladorPrecoRegrasGerais oRg = null;

                    if (_margemObjetivo != null)
                    {
                        oRg = (new SimuladorPrecoRegrasGerais()
                        {
                            estabelecimentoId = _estabelecimento,
                            convenioId = _convenioId,
                            perfilCliente = _perfil,
                            resolucaoId = _resolucao,
                            ufDestino = this.ddlEstadoDestino.SelectedValue
                        }).GetRegras();
                        List<SimuladorPrecoRegrasGerais> oRgSv = (new SimuladorPrecoRegrasGerais()
                        {
                            estabelecimentoId = _estabelecimento,
                            convenioId = _convenioId,
                            perfilCliente = _perfil,
                            resolucaoId = _resolucao,
                            ufDestino = this.ddlEstadoDestino.SelectedValue,
                            usoExclusivoHospitalar = _exclusivoHospitalar
                        }).GetRegrasICMSVenda();
                        if (oRgSv == null)
                        {
                            _vlrIcms = new decimal();
                        }
                        else
                        {
                            SimuladorPrecoRegrasGerais _oRg = null;
                            _oRg = (this.GetValorICMS_ST(_descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA) <= decimal.Zero ? (
                                from x in oRgSv
                                where !x._icmsStValor.HasValue
                                select x).SingleOrDefault<SimuladorPrecoRegrasGerais>() : (
                                from x in oRgSv
                                where x._icmsStValor.GetValueOrDefault(decimal.MinusOne).Equals(decimal.Zero)
                                select x).SingleOrDefault<SimuladorPrecoRegrasGerais>());
                            icmsSobreVenda = _oRg.icmsSobreVenda;
                            if (ItemId.Equals("110219"))
                            {
                                icmsPEValor = this.GetIcmsSobreVendaSPINRAZA(ItemId, _custoPadrao, PisCofins, _estabelecimento, UFDestino, contribuinte, icmsSobreVenda);
                            }
                            else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !estabelecimentoId.Equals("23")))
                            {
                                if (VerificaseItemTemCalculoST(this.ddlEstadoDestino.SelectedValue, estabelecimentoId, ItemId, _convenioId, _perfil))
                                {
                                    _vlrIcms = BuscaIcmsST(this.ddlEstadoDestino.SelectedValue, estabelecimentoId, ItemId, _convenioId, _perfil);
                                }
                                else
                                {
                                    _vlrIcms = (_oRg.icmsSobreVenda > decimal.One ? _oRg.icmsSobreVenda / new decimal(100) : _oRg.icmsSobreVenda);

                                }
                            }
                            else
                            {
                                icmsPEValor = this.GetIcmsSobreVendaRegraPE(ItemId, contribuinte, estabelecimentoId, UFDestino, UfOrigem, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse);
                            }
                        }

                        if (!this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                        {
                            _vlrVenda = (_custoPadrao / (decimal.One - ((_margemObjetivo / 100) + (PisCofins) + (_vlrIcms))));
                        }
                        else
                        {
                            _vlrIcms = this.GetIcmsSpintPerc(this.txtItem.Text.PadLeft(5, '0'), _estabelecimento, this.ddlEstadoDestino.SelectedValue, icmsSobreVenda, contribuinte);
                            _vlrVenda = (_custoPadrao / (decimal.One - ((_margemObjetivo / 100) + _vlrIcms)));
                        }

                        if (estabelecimentoId.Equals("23") && ddlEstadoDestino.SelectedValue.Equals("PE"))
                        {
                            var margemValor = (((_custoPadrao + icmsPEValor) / (1 - (_margemObjetivo / 100))) * (_margemObjetivo / 100));
                            _vlrVenda = _custoPadrao + icmsPEValor + margemValor;
                        }


                        SimuladorRegrasST RegraST = new SimuladorRegrasST()
                        {
                            itemId = this.txtItem.Text.PadLeft(5, '0'),
                            estadoDestino = this.ddlEstadoDestino.SelectedValue,
                            estabelecimentoId = _estabelecimento,
                            classeFiscal = _convenioId,
                            perfilCliente = _perfil
                        };
                        DataTable dtRegraST = new DataTable();
                        if (regimeEspecial)
                        {
                            dtRegraST = RegraST.ObterTodosComFiltro();
                        }
                        if (dtRegraST == null)
                        {
                            ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                            ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                            ValorComST = new decimal();
                            _vlrIcmsSTVenda = new decimal();
                            _margem = new decimal?((this.GetPrecoVendaLiquidoMemoria(oRg, _vlrVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms) - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES)) / this.GetPrecoVendaLiquidoMemoria(oRg, _vlrVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms));
                        }
                        else if (dtRegraST.Rows.Count <= 0)
                        {
                            ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                            ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                            if (this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                            {
                                if (PisCofins >= decimal.Zero)
                                {
                                    this.vendaLiquidaPE = _vlrVenda - (icmsPEValor / (decimal.One - PisCofins));
                                }
                                else
                                {
                                    this.vendaLiquidaPE = (_vlrVenda - (icmsPEValor / (decimal.One - PisCofins))) * PisCofins;
                                }
                                this.ValorPISCofins = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / ((decimal.One - _percPisCofins) - this.GetIcmsSpintPerc(this.txtItem.Text.PadLeft(5, '0'), _estabelecimento, this.ddlEstadoDestino.SelectedValue, icmsSobreVenda, contribuinte))) * _percPisCofins;
                                ValorComST = new decimal();
                                _vlrIcmsSTVenda = new decimal();
                                _margem = new decimal?(this.vendaLiquidaPE - _custoPadrao);
                            }
                            else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !_estabelecimento.Equals("23")))
                            {



                                ValorComST = new decimal();
                                _vlrIcmsSTVenda = new decimal();
                                decimal vendaLiquida = new decimal();


                                if (!PisCofins.Equals(decimal.Zero))
                                {
                                    vendaLiquida = ((_vlrVenda) * ((_vlrVenda * _vlrIcms) + (decimal.One - PisCofins))) + lei5005;
                                }
                                else
                                {
                                    vendaLiquida = ((_vlrVenda) - ((_vlrVenda * _vlrIcms))) + lei5005;
                                }

                                _margem = (vendaLiquida - _custoPadrao);

                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", vendaLiquida);
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", _margem);

                            }
                            else
                            {
                                this.vendaLiquidaPE = (_vlrVenda - icmsPEValor) - (((this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES) / (decimal.One - (decimal.Parse(this.txtMargemObjetivo.Text) / new decimal(100)))) / (decimal.One - _percPisCofins)) * _percPisCofins);
                                this.ValorPISCofins = ((this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES) / (decimal.One - (decimal.Parse(this.txtMargemObjetivo.Text) / new decimal(100)))) / (decimal.One - _percPisCofins)) * _percPisCofins;
                                ValorComST = new decimal();
                                _vlrIcmsSTVenda = new decimal();




                                _margem = new decimal?(this.vendaLiquidaPE - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES));
                            }
                        }
                        else
                        {
                            decimal aliquota = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["aliquota"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["aliquota"].ToString()) : decimal.Zero);
                            decimal PMC = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC"].ToString()) : decimal.Zero);
                            decimal PMC_Cheio = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) : decimal.Zero);
                            decimal PMPF = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMPF"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMPF"].ToString()) : decimal.Zero);





                            decimal icmsInterno = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["icmsInterno"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["icmsInterno"].ToString()) : decimal.Zero);
                            string baseValor = string.Empty;
                            bool verificaSeAssumeBase = false;



                            if (estabelecimentoId != "3")
                            {
                                ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                            }
                            else
                            {
                                string novaBaseSt = CalulaNovaBase(_categoria, this.lblLista.Text, _vlrVenda, PMC_Cheio, PMPF);



                                if (string.IsNullOrEmpty(novaBaseSt))
                                {
                                    ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                    ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                                }
                                else
                                {
                                    ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = string.Concat("(+) ", novaBaseSt.Split(new char[] { ';' })[0].ToString());
                                    ((Literal)e.Item.FindControl("ltrBaseValor")).Text = string.Format("{0:n2}", decimal.Parse(novaBaseSt.Split(new char[] { ';' })[1].ToString()));
                                    baseValor = novaBaseSt.Split(new char[] { ';' })[1].ToString();
                                    if (!novaBaseSt.Split(';')[0].ToString().Equals("Base - 1"))
                                    {
                                        verificaSeAssumeBase = (!novaBaseSt.Split(new char[] { ';' })[0].ToString().Equals("Base ST") ? true : false);
                                    }
                                    else
                                    {
                                        verificaSeAssumeBase = true;
                                    }
                                }
                            }
                            icmsInterno = (icmsInterno > decimal.One ? icmsInterno / new decimal(100) : icmsInterno);
                            PossuiST = true;
                            if (this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                            {
                                _vlrIcmsSTVenda = this.GetPrecoVendaComSTSPIN(_vlrVenda, icmsInterno, icmsInterno, aliquota, PMC, icmsPEValor);
                                ValorComST = _vlrIcmsSTVenda + _vlrVenda;

                                this.vendaLiquidaPE = ((_custoPadrao + icmsPEValor) / (decimal.One - ((_margemObjetivo / new decimal(100)) + _percPisCofins)) - icmsPEValor);


                                this.ValorPISCofins = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - _percPisCofins)) * _percPisCofins;
                                _margem = new decimal?(this.vendaLiquidaPE - _custoPadrao);
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsPEValor);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.vendaLiquidaPE);
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", _margem);

                                _margem = (1 - ((_custoPadrao + icmsPEValor) / _vlrVenda));
                            }
                            else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !_estabelecimento.Equals("23")))
                            {
                                decimal ValorIcmsInterno = new decimal();
                                decimal ValorIcmsSTSobreVenda = new decimal();
                                _vlrVenda = _custoPadrao / (1 - (icmsInterno + (_margemObjetivo / 100) + PisCofins));
                                this.ValorIcmsSobreVenda = this.GetValorIcmsSobreVenda(_vlrVenda, icmsInterno);
                                if (!verificaSeAssumeBase)
                                {
                                    ValorIcmsInterno = this.GetValorIcmsInterno(this.GetValorBaseST(_vlrVenda, aliquota, PMC), icmsInterno);
                                    ValorIcmsSTSobreVenda = ValorIcmsInterno - this.ValorIcmsSobreVenda;
                                    ValorComST = this.GetPrecoVendaComST(_vlrVenda, icmsInterno, icmsInterno, aliquota, PMC, decimal.Zero);
                                }
                                else
                                {
                                    ValorIcmsInterno = this.GetValorIcmsInterno(decimal.Parse(baseValor), icmsInterno);
                                    ValorIcmsSTSobreVenda = ValorIcmsInterno - this.ValorIcmsSobreVenda;
                                    ValorComST = _vlrVenda + ValorIcmsSTSobreVenda;
                                }
                                this.vendaLiquidaComICMSTS = this.GetPrecoVendaLiquidoMemoria(oRg, _vlrVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, icmsInterno);
                                this.margemVlrValorComST = this.vendaLiquidaComICMSTS - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES);
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", this.ValorIcmsSobreVenda);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.vendaLiquidaComICMSTS);
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", this.margemVlrValorComST);
                                _vlrIcmsSTVenda = ValorIcmsSTSobreVenda;
                                //_margem = new decimal?((this.GetPrecoVendaLiquidoMemoria(oRg, _vlrVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, icmsInterno)
                                //    - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES)) / this.GetPrecoVendaLiquidoMemoria(oRg, _vlrVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, icmsInterno));


                                _margem = (1 - ((_custoPadrao + (ValorIcmsSobreVenda) + (PisCofins * _vlrVenda)) / _vlrVenda));

                            }
                            else
                            {
                                if (!this.rblPerfilCliente.SelectedValue.Equals("C"))
                                {
                                    ValorComST = this.GetPrecoVendaComSTPE(_vlrVenda, icmsInterno, icmsInterno, aliquota, PMC);

                                    this.vendaLiquidaPE = ((_custoPadrao + icmsPEValor) / (decimal.One - ((_margemObjetivo / new decimal(100)) + _percPisCofins)) - icmsPEValor);

                                    _vlrIcmsSTVenda = (_vlrVenda * (decimal.One + aliquota)) * icmsInterno;
                                    _margem = (1 - ((_custoPadrao + icmsPEValor) / _vlrVenda));
                                }
                                else
                                {
                                    _vlrIcmsSTVenda = this.GetICMS_STContribuinte(_vlrVenda, aliquota, PMC, icmsInterno);
                                    ValorComST = _vlrIcmsSTVenda + _vlrVenda;
                                    this.vendaLiquidaPE = ((_custoPadrao + icmsPEValor) / (decimal.One - ((_margemObjetivo / new decimal(100)) + _percPisCofins)) - icmsPEValor);
                                    _margem = (1 - ((_custoPadrao + icmsPEValor) / _vlrVenda));


                                }
                                var margemValor = (((_custoPadrao + icmsPEValor) / (1 - (_margemObjetivo / 100))) * (_margemObjetivo / 100));
                                //    var margemValor = ((_custoPadrao / (1 - (_margemObjetivo / 100))) * (_margemObjetivo / 100));
                                this.ValorPISCofins = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - _percPisCofins)) * _percPisCofins;
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsPEValor);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.vendaLiquidaPE);
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", margemValor);
                            }
                        }
                    }


                    ////    CALCULO POR PRECO OBJETIVO ////    CALCULO POR PRECO OBJETIVO ////    CALCULO POR PRECO OBJETIVO

                    ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", _vlrVenda);
                    ((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text = string.Format("{0:n2}", ValorComST);



                    if (PossuiST)
                    {
                        PossuiST = false;
                    }
                    else
                    {
                        ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                        ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                        if (this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                        {
                            ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsPEValor);
                            ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.vendaLiquidaPE);
                            ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", _margem);
                        }
                        else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !_estabelecimento.Equals("23")))
                        {
                            ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", this.GetICMSSobreVenda(_vlrVenda.ToString(), _estabelecimento, _convenioId, _perfil, _resolucao, _exclusivoHospitalar, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf));
                            ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, _vlrVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms));
                            ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:0.##}", _margem);


                        }
                        else
                        {
                            ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsPEValor);
                            ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.vendaLiquidaPE);
                            ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", _margem);
                        }
                    }
                    ((Literal)e.Item.FindControl("ltrICMSSobreVendaValorComST")).Text = string.Format("{0:n2}", _vlrIcmsSTVenda);


                    if (!estabelecimentoId.Equals("20"))
                    {
                        ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", this.GetAjusteRegimeFiscalSobreVenda(_vlrVenda.ToString(), _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA));

                    }

                    if (this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                    {
                        ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.ValorPISCofins);
                    }
                    else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !_estabelecimento.Equals("23")))
                    {
                        ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.GetPISCofinsSobreVenda(_vlrVenda.ToString(), _percPisCofins));
                    }
                    else
                    {
                        ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.ValorPISCofins);
                    }




                    if (ValorComST.Equals(decimal.Zero))
                    {
                        if (_margem.HasValue)
                        {
                            var icmsSobreVendaNv = ((_vlrVenda * _vlrIcms) + (_vlrVenda * _percPisCofins));
                            var custoCalculado = (_custoPadrao + icmsSobreVendaNv);
                            _margem = ((1 - (custoCalculado / _vlrVenda)) * cemPercent);
                        }
                    }
                    else
                    {
                        _margem = _margem * 100;
                    }


                    SimuladorPrecoRegrasGerais oRg_lei =
                    new SimuladorPrecoRegrasGerais
                    {
                        estabelecimentoId = _estabelecimento,
                        convenioId = Utility.FormataStringPesquisa(_convenioId),
                        perfilCliente = _perfil,
                        resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                        ufDestino = this.ddlEstadoDestino.SelectedValue

                    }.GetRegras();


                    if (estabelecimentoId.Equals("20") && _convenioId.ToUpper().Equals("REGIME NORMAL") && !rblPerfilCliente.SelectedValue.Equals("2"))
                    {
                        CalcularLei5005PorMargemObjetivo(_convenioId, oRg_lei.icmsSobreVenda, _precoFabrica, e, regraTransferencia, _margemObjetivo, lei5005, estabelecimentoId, UFDestino, _custoPadrao, PisCofins, _vlrIcms, oRg,
                            GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse));
                    }

                }
                else
                {
                    ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                    ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                    if (!this.txtItem.Text.Equals("110219"))
                    {
                        icmsPEValor = this.GetICMSSobreVenda(_valorVenda.ToString(), _estabelecimento,
                             _convenioId, _perfil, _resolucao, _exclusivoHospitalar, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional,
                             _repasse, _valorICMSST, _percIPI, _reducaoST_MVA, _uf);

                        ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsPEValor);
                    }
                    else
                    {
                        icmsPEValor = this.GetIcmsSobreVendaRegraSpin(this.txtItem.Text, this.rblPerfilCliente.SelectedValue, _estabelecimento, this.ddlEstadoDestino.SelectedValue, "", decimal.Parse(_valorVenda), _descontoComercial, _descontoAdicional, _repasse);

                        ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsPEValor);
                    }


                    if (!estabelecimentoId.Equals("20"))
                    {
                        ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", this.GetAjusteRegimeFiscalSobreVenda(_valorVenda, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA));
                    }

                    SimuladorRegrasST RegraST = new SimuladorRegrasST()
                    {
                        itemId = this.txtItem.Text.PadLeft(5, '0'),
                        estadoDestino = this.ddlEstadoDestino.SelectedValue,
                        estabelecimentoId = _estabelecimento,
                        classeFiscal = _convenioId,
                        perfilCliente = _perfil
                    };
                    DataTable dtRegraST = new DataTable();
                    if (regimeEspecial)
                    {
                        dtRegraST = RegraST.ObterTodosComFiltro();
                    }
                    if (dtRegraST != null)
                    {
                        if (dtRegraST.Rows.Count <= 0)
                        {



                            ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text =
                                    string.Format("{0:n2}", this.GetPISCofinsSobreVenda(_valorVenda.ToString(), _percPisCofins));
                            if (!this.txtItem.Text.Equals("110219"))
                            {
                                var valorVenda = decimal.Parse(_valorVenda.Replace(".",""));



                                SimuladorPrecoRegrasGerais oRg =
                                new SimuladorPrecoRegrasGerais
                                {
                                    estabelecimentoId = _estabelecimento,
                                    convenioId = Utility.FormataStringPesquisa(_convenioId),
                                    perfilCliente = _perfil,
                                    resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                                    ufDestino = this.ddlEstadoDestino.SelectedValue

                                }.GetRegras();

                                if (estabelecimentoId.Equals("20") && _convenioId.ToUpper().Equals("REGIME NORMAL") && !rblPerfilCliente.SelectedValue.Equals("2"))
                                {


                                    decimal _precoAquisicao = GetPrecoAquisicao(_precoFabrica, _descontoComercial, _descontoAdicional, _repasse);


                                    lei5005 = GetLei5005(valorVenda, _convenioId, rblPerfilCliente.SelectedValue, _precoAquisicao, oRg.icmsSobreVenda, _ICMSe, UFDestino, lblResolucao13.Text, regraTransferencia);// item: 4407.

                                    ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscal")).Text = "(-) Lei 5005 (%)";
                                    ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", lei5005);
                                }
                                else if ((estabelecimentoId.Equals("20")))
                                {

                                    ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscal")).Text = "(-) Lei 5005 (%)";
                                    ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", 0);

                                }
                                else
                                {
                                    ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", 0);
                                }

                                var icmsSobreVendalocal = valorVenda * oRg.icmsSobreVenda;
                                var vendaLiquida = valorVenda - icmsPEValor + lei5005;
                                var margemValor = vendaLiquida - _custoPadrao;

                                _margem = ((1 - ((_custoPadrao + ((oRg.icmsSobreVenda * (valorVenda)) + (PisCofins * (valorVenda)) - lei5005)) / (valorVenda))) * 100);


                                ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", valorVenda);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", vendaLiquida);
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", margemValor);

                            }
                            else
                            {
                                var valorVenda = decimal.Parse(_valorVenda);

                                SimuladorPrecoRegrasGerais oRg =
                                    new SimuladorPrecoRegrasGerais
                                    {
                                        estabelecimentoId = _estabelecimento,
                                        convenioId = Utility.FormataStringPesquisa(_convenioId),
                                        perfilCliente = _perfil,
                                        resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                                        ufDestino = this.ddlEstadoDestino.SelectedValue,
                                        usoExclusivoHospitalar = lblUsoExclusivo.Text
                                    }.GetRegras();


                                var icmsSobreVendalocal = valorVenda * oRg.icmsSobreVenda;
                                var vendaLiquida = valorVenda - icmsSobreVendalocal;

                                _margem = (1 - ((_custoPadrao + icmsSobreVendalocal) / valorVenda)) * 100;
                                var margemValor = vendaLiquida - _custoPadrao;


                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", margemValor);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", vendaLiquida);

                                if (icmsPEValor.Equals(decimal.Zero))
                                {
                                    ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsSobreVendalocal);
                                }

                            }
                        }
                        else
                        {
                            decimal _vlrVenda = new decimal();
                            decimal _vlrIcms = new decimal();
                            SimuladorPrecoRegrasGerais oRg = null;
                            oRg = (new SimuladorPrecoRegrasGerais()
                            {
                                estabelecimentoId = _estabelecimento,
                                convenioId = Utility.FormataStringPesquisa(_convenioId),
                                perfilCliente = _perfil,
                                resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                                ufDestino = this.ddlEstadoDestino.SelectedValue
                            }).GetRegras();
                            if (this.txtItem.Text.Equals("110219"))
                            {
                                _vlrIcms = this.GetIcmsSobreVendaRegraSpin(this.txtItem.Text, this.rblPerfilCliente.SelectedValue, _estabelecimento,
                                    this.ddlEstadoDestino.SelectedValue, "", decimal.Parse(_valorVenda), _descontoComercial, _descontoAdicional, _repasse);

                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", _vlrIcms);
                            }
                            List<SimuladorPrecoRegrasGerais> oRgSv = (new SimuladorPrecoRegrasGerais()
                            {
                                estabelecimentoId = _estabelecimento,
                                convenioId = Utility.FormataStringPesquisa(_convenioId),
                                perfilCliente = _perfil,
                                resolucaoId = Utility.FormataStringPesquisa(_resolucao),
                                ufDestino = this.ddlEstadoDestino.SelectedValue,
                                usoExclusivoHospitalar = Utility.FormataStringPesquisa(_exclusivoHospitalar)
                            }).GetRegrasICMSVenda();
                            if (oRgSv == null)
                            {
                                _vlrIcms = new decimal();
                            }
                            else
                            {
                                SimuladorPrecoRegrasGerais _oRg = null;
                                _oRg = (this.GetValorICMS_ST(_descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST,
                                    _percIPI, _reducaoST_MVA) <= decimal.Zero ? (
                                    from x in oRgSv
                                    where !x._icmsStValor.HasValue
                                    select x).SingleOrDefault<SimuladorPrecoRegrasGerais>() : (
                                    from x in oRgSv
                                    where x._icmsStValor.GetValueOrDefault(decimal.MinusOne).Equals(decimal.Zero)
                                    select x).SingleOrDefault<SimuladorPrecoRegrasGerais>());
                                ItemId = this.txtItem.Text.PadLeft(5, '0');
                                icmsSobreVenda = _oRg.icmsSobreVenda;
                                contribuinte = this.rblPerfilCliente.SelectedValue;
                                estabelecimentoId = _estabelecimento;
                                UFDestino = this.ddlEstadoDestino.SelectedValue;
                                UfOrigem = _uf;
                                if (this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                                {
                                    _vlrIcms = this.GetIcmsSpintPerc(ItemId, estabelecimentoId, UFDestino, icmsSobreVenda, contribuinte);
                                }
                                else if (!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !estabelecimentoId.Equals("23"))
                                {
                                    _vlrIcms = (_oRg.icmsSobreVenda > decimal.One ? _oRg.icmsSobreVenda / new decimal(100) : _oRg.icmsSobreVenda);
                                }
                                else
                                {
                                    _vlrIcms = this.GetIcmsSobreVendaRegraPE(ItemId, contribuinte, estabelecimentoId, UFDestino, UfOrigem, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse);
                                }
                            }
                            decimal aliquota = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["aliquota"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["aliquota"].ToString()) : decimal.Zero);
                            decimal PMC = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC"].ToString()) : decimal.Zero);
                            decimal PMC_Cheio = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) : decimal.Zero);


                            decimal icmsInterno = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["icmsInterno"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["icmsInterno"].ToString()) : decimal.Zero);
                            aliquota = (aliquota > decimal.One ? aliquota / new decimal(100) : aliquota);
                            icmsInterno = (icmsInterno > decimal.One ? icmsInterno / new decimal(100) : icmsInterno);
                            decimal PMPF = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMPF"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMPF"].ToString()) : decimal.Zero);

                            PossuiST = true;
                            string baseValor = "0";
                            decimal valorVenda = new decimal();
                            double valor = 0.000111;
                            double valorNega = -100;
                            bool flagCalculado = false;

                            if (!estabelecimentoId.Equals("23") && !ddlEstadoDestino.SelectedValue.Equals("PE"))
                            {
                                _vlrIcms = icmsInterno;
                            }

                            if (this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                            {
                                _vlrVenda = _custoPadrao / ((decimal.One - _vlrIcms) - PisCofins);
                                while (ValorComST < decimal.Parse(_valorVenda))
                                {
                                    _margemObjetivo += Convert.ToDecimal(valor);
                                    valorVenda = _custoPadrao / (1 - (_vlrIcms + _margemObjetivo + PisCofins));

                                    ValorComST = this.GetPrecoVendaComST(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC, decimal.Zero);
                                    if (flagCalculado.Equals(false))
                                    {
                                        if (ValorComST > decimal.Parse(_valorVenda))
                                        {
                                            flagCalculado = true;
                                            _margemObjetivo = Convert.ToDecimal(valorNega);

                                            valorVenda = _custoPadrao / (1 - (_vlrIcms + _margemObjetivo + PisCofins));

                                            ValorComST = this.GetPrecoVendaComST(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC, decimal.Zero);
                                        }
                                    }
                                }
                                _vlrIcmsSTVenda = this.GetValorIcmsSobreVenda(_vlrVenda, _vlrIcms);
                                decimal ValorIcmsInterno = this.GetValorIcmsInterno(this.GetValorBaseST(valorVenda, aliquota, PMC), icmsInterno);
                                decimal ValorIcmsSobreVenda = this.GetValorIcmsSobreVenda(valorVenda, _vlrIcms);
                                _vlrIcmsSTVenda = ValorIcmsInterno - ValorIcmsSobreVenda;
                                ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", valorVenda);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text = string.Format("{0:n2}", ValorComST);
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValorComST")).Text = string.Format("{0:n2}", _vlrIcmsSTVenda);
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", ValorIcmsSobreVenda);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, ValorComST
                                    - _vlrIcmsSTVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS,
                                    _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria,
                                    _exclusivoHospitalar, _vlrIcms));
                                ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                                ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.GetPISCofinsSobreVenda(valorVenda.ToString(), _percPisCofins));
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, ValorComST - _vlrIcmsSTVenda,
                                    _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17,
                                    _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria,
                                    _exclusivoHospitalar, _vlrIcms) - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS,
                                    _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem,
                                    tratamentoICMSEstab, _uf, _resolucao, transfES));
                            }
                            else if ((!this.ddlEstadoDestino.SelectedValue.Equals("PE") ? true : !_estabelecimento.Equals("23")))
                            {
                                _vlrVenda = _custoPadrao / ((decimal.One - _vlrIcms) - PisCofins);
                                while (ValorComST < decimal.Parse(_valorVenda))
                                {
                                    _margemObjetivo += Convert.ToDecimal(valor);

                                    valorVenda = _custoPadrao / (1 - (_vlrIcms + _margemObjetivo + PisCofins));
                                    ValorComST = this.GetPrecoVendaComST(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC, decimal.Zero);
                                    if (flagCalculado.Equals(false))
                                    {
                                        if (ValorComST > decimal.Parse(_valorVenda))
                                        {
                                            flagCalculado = true;
                                            _margemObjetivo = Convert.ToDecimal(valorNega);
                                            valorVenda = _custoPadrao / (1 - (_vlrIcms + _margemObjetivo + PisCofins));

                                            ValorComST = this.GetPrecoVendaComST(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC, decimal.Zero);
                                        }
                                    }
                                }

                                #region CALCULO DA BASE ST SOMENTE PARA SÃO PAULO

                                if (estabelecimentoId != "3")
                                {
                                    ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                    ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                                }
                                else
                                {
                                    string novaBaseSt = string.Empty;


                                    if (string.IsNullOrEmpty(txtCalculaPrePrecoObjetivo.Text))
                                    {

                                        novaBaseSt = SimuladorPreco.CalulaNovaBase(_categoria, this.lblLista.Text, valorVenda, PMC_Cheio, PMPF);

                                    }
                                    else
                                    {
                                        novaBaseSt = SimuladorPreco.CalulaNovaBase(_categoria, this.lblLista.Text, decimal.Parse(txtCalculaPrePrecoObjetivo.Text), PMC_Cheio, PMPF);

                                    }

                                    if (string.IsNullOrEmpty(novaBaseSt))
                                    {
                                        ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                        ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                                    }
                                    else
                                    {
                                        ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = string.Concat("(+) ", novaBaseSt.Split(new char[] { ';' })[0].ToString());
                                        ((Literal)e.Item.FindControl("ltrBaseValor")).Text = string.Format("{0:n2}", decimal.Parse(novaBaseSt.Split(new char[] { ';' })[1].ToString()));
                                        baseValor = novaBaseSt.Split(new char[] { ';' })[1].ToString();
                                    }


                                }

                                #endregion

                                #region VERIFICA SE TEM PMC CHEIO. 

                                if (PMC_Cheio > decimal.Zero || PMPF > decimal.Zero)
                                {
                                    flagCalculado = false;
                                    ValorComST = new decimal();
                                    valorVenda = new decimal();
                                    _margemObjetivo = new decimal();
                                    while (ValorComST < decimal.Parse(_valorVenda))
                                    {
                                        _margemObjetivo += Convert.ToDecimal(valor);

                                        valorVenda = _custoPadrao / (1 - (_vlrIcms + _margemObjetivo + PisCofins));

                                        ValorComST = this.GetPrecoVendaComST(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC, decimal.Parse(baseValor));
                                        if (flagCalculado.Equals(false))
                                        {
                                            if (ValorComST > decimal.Parse(_valorVenda))
                                            {
                                                flagCalculado = true;
                                                _margemObjetivo = Convert.ToDecimal(valorNega);

                                                valorVenda = _custoPadrao / (1 - (_vlrIcms + _margemObjetivo + PisCofins));
                                                ValorComST = this.GetPrecoVendaComST(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC, decimal.Parse(baseValor));
                                            }
                                        }
                                    }

                                    _vlrIcmsSTVenda = this.GetValorIcmsSobreVenda(_vlrVenda, _vlrIcms);
                                    decimal ValorIcmsInterno = this.GetValorIcmsInterno(decimal.Parse(baseValor), icmsInterno);
                                    decimal ValorIcmsSobreVenda = this.GetValorIcmsSobreVenda(valorVenda, _vlrIcms);
                                    _vlrIcmsSTVenda = ValorIcmsInterno - ValorIcmsSobreVenda;

                                    ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", valorVenda);
                                    ((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text = string.Format("{0:n2}", ValorComST);
                                    ((Literal)e.Item.FindControl("ltrICMSSobreVendaValorComST")).Text = string.Format("{0:n2}", _vlrIcmsSTVenda);
                                    ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", ValorIcmsSobreVenda);
                                    ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, ValorComST - _vlrIcmsSTVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms));
                                    ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.GetPISCofinsSobreVenda(valorVenda.ToString(), _percPisCofins));
                                    ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, ValorComST - _vlrIcmsSTVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms) - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES));
                                }

                                #endregion
                                else
                                {
                                    _vlrIcmsSTVenda = this.GetValorIcmsSobreVenda(_vlrVenda, _vlrIcms);
                                    decimal ValorIcmsInterno = this.GetValorIcmsInterno(this.GetValorBaseST(valorVenda, aliquota, PMC), icmsInterno);
                                    decimal ValorIcmsSobreVenda = this.GetValorIcmsSobreVenda(valorVenda, _vlrIcms);
                                    _vlrIcmsSTVenda = ValorIcmsInterno - ValorIcmsSobreVenda;
                                    ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", valorVenda);
                                    ((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text = string.Format("{0:n2}", ValorComST);
                                    ((Literal)e.Item.FindControl("ltrICMSSobreVendaValorComST")).Text = string.Format("{0:n2}", _vlrIcmsSTVenda);
                                    ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", ValorIcmsSobreVenda);
                                    ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, ValorComST
                                        - _vlrIcmsSTVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva,
                                        _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST,
                                        _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms));
                                    ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                    ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                                    ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.GetPISCofinsSobreVenda(valorVenda.ToString(), _percPisCofins));
                                    ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", this.GetPrecoVendaLiquidoMemoria(oRg, ValorComST
                                        - _vlrIcmsSTVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST,
                                        _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST,
                                        _reducaoST_MVA, _lista, _categoria, _exclusivoHospitalar, _vlrIcms) - this.GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins,
                                        _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA,
                                        _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES));
                                }
                            }
                            else
                            {
                                decimal percPisCofins = (_percPisCofins > decimal.One ? _percPisCofins / new decimal(100) : _percPisCofins);
                                if (!this.rblPerfilCliente.SelectedValue.Equals("C"))
                                {
                                    _vlrVenda = ((_custoPadrao / decimal.One) / (decimal.One - percPisCofins)) + _vlrIcms;
                                    while (ValorComST < decimal.Parse(_valorVenda))
                                    {
                                        _margemObjetivo += Convert.ToDecimal(valor);
                                        valorVenda = (((_custoPadrao + _vlrIcms) / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - percPisCofins));
                                        ValorComST = this.GetPrecoVendaComSTPE(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC);
                                        if (flagCalculado.Equals(false))
                                        {
                                            if (ValorComST > decimal.Parse(_valorVenda))
                                            {
                                                flagCalculado = true;
                                                _margemObjetivo += Convert.ToDecimal(valorNega);
                                                valorVenda = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - percPisCofins)) + _vlrIcms;
                                                ValorComST = this.GetPrecoVendaComSTPE(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC);
                                            }
                                        }
                                    }
                                    this.vendaLiquidaPE = this.CalcularVendaLiquidaPernanbuco(_percPisCofins, _estabelecimento, _convenioId, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _valorICMSST, _reducaoST_MVA, codigoOrigem, tratamentoICMSEstab, transfES, _margemObjetivo, _vlrIcms, valorVenda);
                                    this.ValorPISCofins = this.CalcularPisCofins(_percPisCofins, _estabelecimento, _convenioId, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _valorICMSST, _reducaoST_MVA, codigoOrigem, tratamentoICMSEstab, transfES, _margemObjetivo);
                                    _vlrIcmsSTVenda = (valorVenda * (decimal.One + aliquota)) * icmsInterno;
                                }
                                else
                                {
                                    _vlrVenda = ((_custoPadrao / decimal.One) / (decimal.One - percPisCofins)) + _vlrIcms;
                                    while (ValorComST < decimal.Parse(_valorVenda))
                                    {
                                        _margemObjetivo += Convert.ToDecimal(valor);
                                        valorVenda = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - percPisCofins)) + _vlrIcms;
                                        _vlrIcmsSTVenda = this.GetICMS_STContribuintePO(valorVenda, aliquota, PMC, icmsInterno);
                                        ValorComST = valorVenda + _vlrIcmsSTVenda;
                                        if (flagCalculado.Equals(false))
                                        {
                                            if (ValorComST > decimal.Parse(_valorVenda))
                                            {
                                                flagCalculado = true;
                                                _margemObjetivo += Convert.ToDecimal(valorNega);
                                                valorVenda = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - percPisCofins)) + _vlrIcms;
                                                ValorComST = this.GetPrecoVendaComSTPE(valorVenda, _vlrIcms, icmsInterno, aliquota, PMC);
                                            }
                                        }
                                    }
                                    this.vendaLiquidaPE = this.CalcularVendaLiquidaPernanbuco(_percPisCofins, _estabelecimento, _convenioId, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _valorICMSST, _reducaoST_MVA, codigoOrigem, tratamentoICMSEstab, transfES, _margemObjetivo, _vlrIcms, valorVenda);
                                    this.ValorPISCofins = this.CalcularPisCofins(_percPisCofins, _estabelecimento, _convenioId, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _valorICMSST, _reducaoST_MVA, codigoOrigem, tratamentoICMSEstab, transfES, _margemObjetivo);
                                }
                                ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", valorVenda);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaValorComST")).Text = string.Format("{0:n2}", ValorComST);
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValorComST")).Text = string.Format("{0:n2}", _vlrIcmsSTVenda);
                                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", _vlrIcms);
                                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", this.vendaLiquidaPE);
                                ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.ValorPISCofins);
                                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", this.vendaLiquidaPE - _custoPadrao);
                                ((Literal)e.Item.FindControl("ltrBaseLabel")).Text = "(+) Base ST";
                                ((Literal)e.Item.FindControl("ltrBaseValor")).Text = "0,00";
                            }
                            if (!this.txtItem.Text.PadLeft(5, '0').Equals("110219"))
                            {
                                if (!estabelecimentoId.Equals("23") && !ddlEstadoDestino.SelectedValue.Equals("PE"))
                                {
                                    if (!valorVenda.Equals(decimal.Zero))
                                    {
                                        _vlrVenda = valorVenda;
                                    }

                                    if (_margem.HasValue)
                                    {
                                        var icmsSobreVendaNv = ((_vlrVenda * _vlrIcms) + (_vlrVenda * _percPisCofins));
                                        var custoCalculado = (_custoPadrao + icmsSobreVendaNv);
                                        _margem = ((1 - (custoCalculado / _vlrVenda)) * cemPercent) * 1;
                                    }
                                }
                                else
                                {
                                    if (!this.rblPerfilCliente.SelectedValue.Equals("C"))
                                    {
                                        ValorComST = this.GetPrecoVendaComSTPE(valorVenda, icmsInterno, icmsInterno, aliquota, PMC);
                                        if (_percPisCofins <= decimal.Zero)
                                        {
                                            this.vendaLiquidaPE = _custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)));
                                        }
                                        else
                                        {
                                            this.vendaLiquidaPE = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - _percPisCofins)) * _percPisCofins;
                                        }
                                        _vlrIcmsSTVenda = (valorVenda * (decimal.One + aliquota)) * icmsInterno;
                                        _margem = (1 - ((_custoPadrao + _vlrIcms) / valorVenda)) * 100;
                                    }
                                    else
                                    {
                                        _vlrIcmsSTVenda = this.GetICMS_STContribuinte(valorVenda, aliquota, PMC, icmsInterno);
                                        ValorComST = _vlrIcmsSTVenda + valorVenda;
                                        if (_percPisCofins <= decimal.Zero)
                                        {
                                            this.vendaLiquidaPE = _custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)));
                                        }
                                        else
                                        {
                                            this.vendaLiquidaPE = ((_custoPadrao / (decimal.One - (_margemObjetivo / new decimal(100)))) / (decimal.One - _percPisCofins)) * _percPisCofins;
                                        }
                                        _vlrIcmsSTVenda = (valorVenda * (decimal.One + aliquota)) * icmsInterno;
                                        _margem = (1 - ((_custoPadrao + _vlrIcms) / valorVenda)) * 100;
                                    }
                                }
                            }
                            else
                            {
                                if (!valorVenda.Equals(decimal.Zero))
                                {
                                    _vlrVenda = valorVenda;
                                }

                                if (_margem.HasValue)
                                {
                                    var icmsSobreVendaNv = ((_vlrVenda * _vlrIcms) + _percPisCofins);
                                    var custoCalculado = (_custoPadrao + icmsSobreVendaNv) - lei5005;
                                    _margem = ((1 - (custoCalculado / _vlrVenda)) * cemPercent);
                                }
                            }
                        }
                    }
                    else if (!this.txtItem.Text.Equals("110219"))
                    {
                        ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.GetPISCofinsSobreVenda(_valorVenda.ToString(), _percPisCofins));
                        _margem = new decimal?((this.GetValorMagem(_valorVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _uf, _exclusivoHospitalar, _lista, _categoria, codigoOrigem, tratamentoICMSEstab, transfES) / this.GetPrecoVendaLiquido(_valorVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria)) * new decimal(100));
                    }
                    else
                    {
                        ((Literal)e.Item.FindControl("ltrPISCofinsSobreVendaValor")).Text = string.Format("{0:n2}", this.GetPISCofinsSobreVenda(_valorVenda.ToString(), _percPisCofins));
                        ((Literal)e.Item.FindControl("ltrICMSSTSobreVendaValor")).Text = string.Format("{0:n2}", this.GetIcmsSobreVendaRegraSpin(this.txtItem.Text, this.rblPerfilCliente.SelectedValue, _estabelecimento, this.ddlEstadoDestino.SelectedValue, "", _precoFabrica, _descontoComercial, _descontoAdicional, _repasse));
                        _margem = new decimal?((this.GetValorMagemSpin(_valorVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _uf, _exclusivoHospitalar, _lista, _categoria, codigoOrigem, tratamentoICMSEstab, transfES, _custoPadrao, this.rblPerfilCliente.SelectedValue) / this.GetPrecoVendaLiquido(_valorVenda, _percPisCofins, _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA, _lista, _categoria)) * new decimal(100));
                    }
                }



            }
            catch (Exception exception)
            {
                Utility.WriteLog(exception);
                _margem = new decimal();
            }
            return _margem;
        }

        private decimal CalcularLei5005PorMargemObjetivo(string _convenioId, decimal _ICMSe, decimal _precoFabrica, DataListItemEventArgs e, bool regraTransferencia, decimal _margemObjetivo, decimal lei5005, string estabelecimentoId, string UFDestino, decimal _custoPadrao, decimal PisCofins, decimal _vlrIcms, SimuladorPrecoRegrasGerais oRg, decimal precoAquisicao)
        {





            if (estabelecimentoId.Equals("20") && _convenioId.ToUpper().Equals("REGIME NORMAL") && !rblPerfilCliente.SelectedValue.Equals("2"))
            {
                decimal margemBase = 0.001M;
                double contador = 0.0001;
                decimal vendaLiquidaCalculado = _precoFabrica;
                decimal icmsValorCalculadoPF = 0;
                decimal _margemValor = 0;
                decimal valorVenda = 0;
                decimal margemPercent = 0;
                bool flagCalculado = false;



                valorVenda = (_custoPadrao / (1 - (_vlrIcms + margemBase + PisCofins)));

                lei5005 = GetLei5005(valorVenda, _convenioId, rblPerfilCliente.SelectedValue, precoAquisicao, oRg.icmsSobreVenda, _ICMSe, UFDestino, lblResolucao13.Text, regraTransferencia);

                icmsValorCalculadoPF = (valorVenda * (_vlrIcms));

                vendaLiquidaCalculado = (valorVenda - icmsValorCalculadoPF);

                _margemValor = ((vendaLiquidaCalculado - _custoPadrao) + lei5005);

                margemPercent = (_margemValor / valorVenda);
                decimal _margemObjetivoNOvo = (_margemObjetivo / 100);


                while (margemPercent < _margemObjetivoNOvo)
                {
                    margemBase += decimal.Parse(contador.ToString());

                    valorVenda = (_custoPadrao / (1 - (_vlrIcms + margemBase + PisCofins)));

                    lei5005 = GetLei5005(valorVenda, _convenioId, rblPerfilCliente.SelectedValue, precoAquisicao, oRg.icmsSobreVenda, _ICMSe, UFDestino, lblResolucao13.Text, regraTransferencia);

                    icmsValorCalculadoPF = (valorVenda * (_vlrIcms));

                    vendaLiquidaCalculado = (valorVenda - icmsValorCalculadoPF);

                    _margemValor = ((vendaLiquidaCalculado - _custoPadrao) + lei5005);

                    margemPercent = (_margemValor / valorVenda);
                    flagCalculado = true;
                }

                while (margemPercent > _margemObjetivoNOvo)
                {
                    margemBase -= decimal.Parse(contador.ToString());

                    valorVenda = (_custoPadrao / (1 - (_vlrIcms + margemBase + PisCofins)));

                    lei5005 = GetLei5005(valorVenda, _convenioId, rblPerfilCliente.SelectedValue, precoAquisicao, oRg.icmsSobreVenda, _ICMSe, UFDestino, lblResolucao13.Text, regraTransferencia);

                    icmsValorCalculadoPF = (valorVenda * (_vlrIcms));

                    vendaLiquidaCalculado = (valorVenda - icmsValorCalculadoPF);

                    _margemValor = ((vendaLiquidaCalculado - _custoPadrao) + lei5005);

                    margemPercent = (_margemValor / valorVenda);
                    flagCalculado = true;
                }




                string teste = "";

                ((Literal)e.Item.FindControl("ltrPrecoVendaValor")).Text = string.Format("{0:n2}", valorVenda);
                ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscal")).Text = "(-) Lei 5005 (%)";
                ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", lei5005);
                ((Literal)e.Item.FindControl("ltrMargemVlrValor")).Text = string.Format("{0:n2}", _margemValor);
                ((Literal)e.Item.FindControl("ltrMargemPrcValor")).Text = string.Format("{0:n2}", margemPercent);
                ((Literal)e.Item.FindControl("ltrPrecoVendaLiquidoValor")).Text = string.Format("{0:n2}", vendaLiquidaCalculado + lei5005);
                ((Literal)e.Item.FindControl("ltrICMSSobreVendaValor")).Text = string.Format("{0:n2}", icmsValorCalculadoPF);



            }
            else if ((estabelecimentoId.Equals("20")))
            {

                ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscal")).Text = "(-) Lei 5005 (%)";
                ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", 0);

            }
            else
            {
                ((Literal)e.Item.FindControl("ltrAjusteRegimeFiscalValor")).Text = string.Format("{0:n2}", 0);
            }

            return lei5005;
        }

        private static decimal RegraTransfESCalculoGerais(string _estabelecimento, string _perfil, decimal transfES, decimal calcula, decimal precoAquisicao, decimal redutorBase)
        {




            string repassefixo = "7";
            decimal icmsCompraFixo = 7;
            decimal icmsBeneficioFixo = 12;

            string valor116POrcentro = "1,16";
            decimal calculoBase = (precoAquisicao / (1 - (decimal.Parse(repassefixo) / 100)));

            if (_perfil.Equals("2") && _estabelecimento.Equals("20"))
            {
                transfES = 0;
            }
            else
            {
                if (calcula == 1)
                {
                    decimal part1 = (calculoBase * (icmsBeneficioFixo / 100));
                    decimal part2 = ((precoAquisicao * (1 - redutorBase)) * (icmsCompraFixo / 100)); ;
                    decimal part3 = (calculoBase * (decimal.Parse(valor116POrcentro) / 100));


                    transfES = part1 - part2 - part3;

                }

            }

            if (_perfil.Equals("0"))
            {
                transfES = 0;
            }


            return transfES;
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


        private decimal CalcularPisCofins(decimal _percPisCofins, string _estabelecimento, string _convenioId, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, string _uf, decimal _valorICMSST, decimal _reducaoST_MVA, int codigoOrigem, string tratamentoICMSEstab, decimal transfES, decimal _margemObjetivo)
        {
            return ((GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES))
                                                                     / (1 - _margemObjetivo / 100) / (1 - _percPisCofins)) * _percPisCofins;
        }

        private decimal GetICMS_STContribuintePO(decimal vlrVenda, decimal aliquotaBaseST, decimal PMC, decimal icmsST)
        {
            if (PMC > 0)
            {
                return (PMC * icmsST) - (vlrVenda * icmsST);
            }
            else
            {
                var Aliquota = aliquotaBaseST > 1 ? (aliquotaBaseST / 100) : aliquotaBaseST;
                if (Aliquota.Equals(decimal.Zero))
                {
                    return (vlrVenda * icmsST);
                }
                else
                {
                    return ((vlrVenda * (Aliquota)) * icmsST) - (vlrVenda * icmsST);
                }

            }
        }

        private decimal CalcularVendaLiquidaPernanbuco(decimal _percPisCofins, string _estabelecimento, string _convenioId, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, string _uf, decimal _valorICMSST, decimal _reducaoST_MVA, int codigoOrigem, string tratamentoICMSEstab, decimal transfES, decimal _margemObjetivo, decimal _vlrIcms, decimal valorVenda)
        {
            return (valorVenda - _vlrIcms - ((GetValorCustoPadrao(_reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _estabelecimento, _convenioId, codigoOrigem, tratamentoICMSEstab, _uf, _resolucao, transfES))
                             / (1 - _margemObjetivo / 100) / (1 - _percPisCofins)) * _percPisCofins);
        }

        private decimal GetPrecoVendaLiquidoMemoria(SimuladorPrecoRegrasGerais oRg, decimal _vlrVenda, decimal _percPisCofins, string _estabelecimento, string _convenioId, string _perfil, string _resolucao, decimal _reducaoBase, decimal _ICMSe, decimal _percIPI, string _descST, decimal _mva, decimal _aliquotaIntICMS, decimal _pmc17, decimal _precoFabrica, decimal _descontoComercial, decimal _descontoAdicional, decimal _repasse, string _uf, string _exclusivoHospitalar, decimal _valorICMSST, decimal _reducaoST_MVA, string _lista, string _categoria, string _usoExclusivo, decimal _vlrIcms)
        {
            try
            {
                if (!oRg._AliquotasCalculadas)
                {
                    oRg._AliquotasCalculadas = true;
                    oRg._ajusteRegimeFiscal = GetAjusteRegimeFiscalSobreVenda(_vlrVenda.ToString(), _estabelecimento, _convenioId, _perfil, _resolucao, _reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _pmc17, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _uf, _exclusivoHospitalar, _valorICMSST, _reducaoST_MVA);
                }

                return (
                        GetPrecoVenda(_vlrVenda.ToString())
                            -
                        (_vlrIcms * GetPrecoVenda(_vlrVenda.ToString()))
                            -
                        GetICMSSTSobreVenda(_vlrVenda.ToString(), _estabelecimento, _convenioId, _perfil, _resolucao, _lista, _categoria, _uf, _pmc17, _reducaoBase, _ICMSe, _percIPI, _percPisCofins, _descST, _mva, _aliquotaIntICMS, _precoFabrica, _descontoComercial, _descontoAdicional, _repasse, _valorICMSST, _reducaoST_MVA, _exclusivoHospitalar, true, oRg, false)
                            +
                        oRg._ajusteRegimeFiscal
                            -
                       ((_percPisCofins > 1 ? (_percPisCofins / 100) : _percPisCofins) * GetPrecoVenda(_vlrVenda.ToString()))
                       //  ((_percPisCofins / 100) * GetPrecoVenda(_vlrVenda.ToString()))
                       );
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return 0;
            }
        }

        private decimal GetICMS_STContribuinte(decimal vlrVenda, decimal aliquotaBaseST, decimal PMC, decimal icmsST)
        {

            if (PMC > 0)
            {
                return (PMC * icmsST) - (vlrVenda * icmsST);
            }
            else
            {
                var Aliquota = aliquotaBaseST > 1 ? (aliquotaBaseST / 100) : aliquotaBaseST;
                if (Aliquota.Equals(decimal.Zero))
                {
                    return (vlrVenda * icmsST);
                }
                else
                {
                    return ((vlrVenda * (Aliquota)) * icmsST) - (vlrVenda * icmsST);
                }

            }
        }

        #region Venda com ST




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

        private decimal GetValorIcmsSobreVenda(decimal _vlrVenda, decimal _vlrIcms)
        {
            decimal Icms = _vlrIcms > 1 ? (_vlrIcms / 100) : _vlrIcms;
            return (_vlrVenda * Icms);
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
        #endregion

        #endregion

        #region  ::Data Ultima Atualização ::

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

        public bool ValidaItemImportado(int CodigoOrigem)
        {
            bool retorno = false;

            switch (CodigoOrigem)
            {
                case 0:
                case 4:
                case 5:
                    retorno = false;
                    break;
                case 1:
                case 2:
                case 3:
                case 6:
                case 7:
                    retorno = true;
                    break;

                default:
                    break;
            }

            return retorno;

        }
        #endregion


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



        #endregion

        #region Calculo da base!
        private static decimal CacularBase1(decimal pmc, decimal redutor)
        {
            return pmc * (1 - redutor / 100); ;
        }

        private static decimal CacularBase2(decimal precoVenda, decimal mvaST)
        {
            return precoVenda * (1 + mvaST / 100);
        }



        public static string CalulaNovaBase(string categoria, string lista, decimal liquido, decimal PMC, decimal PMPF)
        {


            decimal baseCalculado = new decimal();
            string retorno = string.Empty;
            SimuladorPrecoTrava simuladorPrecoTrava = new SimuladorPrecoTrava();

            simuladorPrecoTrava.categoria = categoria;
            simuladorPrecoTrava.lista = lista;

            bool AtivaPMPF = bool.Parse(ConfigurationManager.AppSettings["AtivaPMPF"].ToString());

            var dt = simuladorPrecoTrava.ObterDadosPorFiltro();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    decimal trava = decimal.Parse(dt.Rows[0]["trava"].ToString().Replace('%', ' ')) / 100;
                    decimal mva = decimal.Parse(dt.Rows[0]["mva_st"].ToString().Replace('%', ' '));
                    decimal redutor = decimal.Parse(dt.Rows[0]["mva_ts_reduzido"].ToString().Replace('%', ' '));

                    decimal liqMVA = (liquido * (1 + mva / 100));


                    ///CALCULA BASE ANTIGA ST
                    if (!AtivaPMPF)
                    {
                        decimal base1 = new decimal();
                        decimal base2 = new decimal();

                        base1 = CacularBase1(PMC, redutor);

                        if (base1 > 0)
                        {
                            var calculoNovaBase = liqMVA / base1;
                            if (calculoNovaBase >= trava)
                            {
                                base2 = CacularBase2(liquido, mva);

                                if (base2 <= PMC)
                                {
                                    retorno = "Base - 2;" + base2.ToString();
                                    return retorno;
                                }
                                else
                                {
                                    retorno = "PMC_cheio - 2;" + PMC.ToString();
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
                    else          // CALCULA BASE NOVA ST
                    {
                        ///REGRAS 1
                        ///
                        if (PMC > 0 && PMPF > 0)
                        {
                            if ((PMPF * trava) < liquido)
                            {
                                baseCalculado = (PMPF * trava);
                                retorno = "PMP + Trava F ;" + baseCalculado.ToString();
                                return retorno;
                            }
                            else
                            {
                                if (PMPF < PMC)
                                {
                                    baseCalculado = PMPF;
                                    retorno = "PMPF ;" + baseCalculado.ToString();
                                    return retorno;
                                }
                                else
                                {
                                    baseCalculado = PMC;
                                    retorno = "PMC ;" + baseCalculado.ToString();
                                    return retorno;
                                }
                            }
                        }
                        else
                    ///REGRAS 2
                    if (PMC > 0 && PMPF <= 0)
                        {
                            if (liqMVA < PMC)
                            {
                                baseCalculado = liqMVA;
                                retorno = "Liquido + MVA;" + baseCalculado.ToString();
                                return retorno;
                            }
                            else
                            {
                                baseCalculado = PMC;
                                retorno = "PMC ;" + baseCalculado.ToString();
                                return retorno;
                            }
                        }
                        else
                    ///REGRAS 3
                    if (PMC <= 0 && PMPF > 0)
                        {
                            if ((PMPF * trava) < liquido)
                            {
                                baseCalculado = liqMVA;
                                retorno = "Liquido + MVA;" + baseCalculado.ToString();
                                return retorno;
                            }
                            else
                            {
                                baseCalculado = PMPF;
                                retorno = "PMPF ;" + baseCalculado.ToString();
                                return retorno;
                            }
                        }
                        else
                    ///REGRAS 4
                    if (PMC <= 0 && PMPF <= 0)
                        {
                            baseCalculado = liqMVA;
                            retorno = "Liquido + MVA;" + baseCalculado.ToString();
                            return retorno;
                        }
                    }


                }
            }
            return retorno;
        }

        #endregion

        public void PopularModalErro(string textoErro)
        {
            lblModalBody.Text = textoErro;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();
            pnlPrincipal.Visible = true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblModalBody.Text = "This is modal body";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();
        }

        protected void btnLimpar_Click(object sender, EventArgs e)
        {

            Response.Redirect("~/AppPaginas/Consulta/SimuladorPreco.aspx");
            Clean(txtItem,
                txtDescontoAdicional,
                txtDescontoObjetivo,
                txtPrecoObjetivo,
                txtMargemObjetivo,
                ddlaceItem,
                ddlEstadoDestino,
                rblPerfilCliente,
                pnlSimulacao,
                lblDescricao,
                   lblUfOrigem,
                   lblFornecedor,
                   lblLista,
                   lblCategoria,
                   lbltipo,
                   lblNCM,
                   lblUsoExclusivo,
                   lblResolucao13,
                   lblDesconto,
                   lblMedicamento,
                   txtCapAplicado);
        }

        protected void ddlEstadoDestino_TextChanged(object sender, EventArgs e)
        {
            this.Clean((Control)this.txtPrecoObjetivo, (Control)this.dlSimulacoes, (Control)this.txtCalculaPrePrecoObjetivo);
            if (this.ddlEstadoDestino.SelectedValue.Equals("SP") || this.ddlEstadoDestino.SelectedValue.Equals("RS"))
            {
                this.ckeckCalculaST.Enabled = true;
            }
            else
            {
                this.ckeckCalculaST.Checked = false;
                this.ckeckCalculaST.Enabled = false;
            }
            if (this.ddlEstadoDestino.SelectedValue.Equals("SP") && this.rblPerfilCliente.SelectedValue.Equals("C"))
            {
                this.txtCalculaPrePrecoObjetivo.Enabled = true;
                this.txtPrecoObjetivo.Enabled = false;
            }
            else
            {
                this.txtCalculaPrePrecoObjetivo.Enabled = false;
                this.txtPrecoObjetivo.Enabled = true;
            }
            DataTable dataTable = new SimuladorPrecoConvenio()
            {
                item = this.txtItem.Text,
                destino = this.ddlEstadoDestino.SelectedValue
            }.ObterListaConvenio();
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                    this.lbltipo.Text = dataTable.Rows[0]["convenio"].ToString();
                else
                    this.lbltipo.Text = new SimuladorPrecoCustos().GetConvenioTabelaCusto(this.txtItem.Text);
            }
            else
                this.lbltipo.Text = new SimuladorPrecoCustos().GetConvenioTabelaCusto(this.txtItem.Text);
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

        protected void txtCalculaPrePrecoObjetivo_TextChanged(object sender, EventArgs e)
        {

            //CHAVE LIGA E DESLIGA REGRA PMPF
            bool AtivaPMPF = bool.Parse(ConfigurationManager.AppSettings["AtivaPMPF"].ToString());

            base.Clean(new Control[] { this.txtPrecoObjetivo, this.dlSimulacoes });
            if (!string.IsNullOrEmpty(this.txtCalculaPrePrecoObjetivo.Text))
            {
                if ((!this.ddlEstadoDestino.SelectedValue.Equals("SP") ? false : this.rblPerfilCliente.SelectedValue.Equals("C")))
                {
                    decimal valorVendaCalculado = decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text);
                    SimuladorPrecoCustos simuladorPrecoCustos = (new SimuladorPrecoCustos()
                    {
                        itemId = this.txtItem.Text.PadLeft(5, '0')
                    }).GetItem();
                    DataTable dtRegraST = (new SimuladorRegrasST()
                    {
                        itemId = this.txtItem.Text.PadLeft(5, '0'),
                        estadoDestino = this.ddlEstadoDestino.SelectedValue,
                        estabelecimentoId = "3",
                        classeFiscal = simuladorPrecoCustos.tipo,
                        perfilCliente = this.rblPerfilCliente.SelectedValue
                    }).ObterTodosComFiltro();
                    if (dtRegraST != null)
                    {
                        if (dtRegraST.Rows.Count > 0)
                        {
                            SimuladorPrecoRegrasGerais oRgSv = (new SimuladorPrecoRegrasGerais()
                            {
                                estabelecimentoId = "3",
                                convenioId = Utility.FormataStringPesquisa(simuladorPrecoCustos.tipo),
                                perfilCliente = this.rblPerfilCliente.SelectedValue,
                                resolucaoId = Utility.FormataStringPesquisa(simuladorPrecoCustos.resolucao13),
                                ufDestino = this.ddlEstadoDestino.SelectedValue,
                                usoExclusivoHospitalar = Utility.FormataStringPesquisa(simuladorPrecoCustos.exclusivoHospitalar)
                            }).GetRegrasICMSVenda()[0];
                            if (oRgSv != null)
                            {

                                decimal ValorIcmsInterno = new decimal();
                                decimal ValorIcmsSTSobreVenda = new decimal();
                                decimal _vlrIcms = (oRgSv.icmsSobreVenda > decimal.One ? oRgSv.icmsSobreVenda / new decimal(100) : oRgSv.icmsSobreVenda);
                                decimal aliquota = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["aliquota"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["aliquota"].ToString()) : decimal.Zero);
                                decimal PMC = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC"].ToString()) : decimal.Zero);
                                decimal PMC_Cheio = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMC_CHEIO"].ToString()) : decimal.Zero);
                                decimal icmsInterno = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["icmsInterno"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["icmsInterno"].ToString()) : decimal.Zero);
                                string baseValor = string.Empty;

                                decimal PMPF = (!string.IsNullOrEmpty(dtRegraST.Rows[0]["PMPF"].ToString()) ? decimal.Parse(dtRegraST.Rows[0]["PMPF"].ToString()) : decimal.Zero);

                                aliquota = (aliquota > decimal.One ? aliquota / new decimal(100) : aliquota);
                                icmsInterno = (icmsInterno > decimal.One ? icmsInterno / new decimal(100) : icmsInterno);
                                string novaBaseSt = CalulaNovaBase(simuladorPrecoCustos.categoria, this.lblLista.Text, decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text), PMC_Cheio, PMPF);

                                if (!string.IsNullOrEmpty(novaBaseSt))
                                {
                                    baseValor = novaBaseSt.Split(new char[] { ';' })[1].ToString();

                                    ValorIcmsInterno = this.GetValorIcmsInterno(decimal.Parse(baseValor), icmsInterno);
                                    if (!ValorIcmsInterno.Equals(decimal.Zero))
                                    {
                                        this.ValorIcmsSobreVenda = this.GetValorIcmsSobreVenda(decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text), icmsInterno);
                                        ValorIcmsInterno = this.GetValorIcmsInterno(decimal.Parse(baseValor), icmsInterno);
                                        ValorIcmsSTSobreVenda = ValorIcmsInterno - this.ValorIcmsSobreVenda;
                                        valorVendaCalculado = decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) + ValorIcmsSTSobreVenda;
                                    }
                                    else
                                    {
                                        ValorIcmsInterno = decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) * icmsInterno;
                                        ValorIcmsSTSobreVenda = ((decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) * (decimal.One + aliquota)) * icmsInterno) - ValorIcmsInterno;
                                        valorVendaCalculado = decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) + ValorIcmsSTSobreVenda;
                                    }
                                }
                                else
                                {
                                    ValorIcmsInterno = decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) * icmsInterno;
                                    ValorIcmsSTSobreVenda = ((decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) * (decimal.One + aliquota)) * icmsInterno) - ValorIcmsInterno;
                                    valorVendaCalculado = decimal.Parse(this.txtCalculaPrePrecoObjetivo.Text) + ValorIcmsSTSobreVenda;
                                }
                            }
                        }
                        this.txtPrecoObjetivo.Text = string.Format("{0:n2}", valorVendaCalculado);
                    }
                }
            }
        }

        protected void rblPerfilCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clean(txtPrecoObjetivo, dlSimulacoes, txtCalculaPrePrecoObjetivo);

            if (!string.IsNullOrEmpty(ddlEstadoDestino.SelectedValue))
            {

                if (ddlEstadoDestino.SelectedValue.Equals("SP") || ddlEstadoDestino.SelectedValue.Equals("RS"))
                {
                    ckeckCalculaST.Enabled = true;
                }
                else
                {
                    ckeckCalculaST.Checked = false;
                    ckeckCalculaST.Enabled = false;
                }


                if (ddlEstadoDestino.SelectedValue.Equals("SP") && rblPerfilCliente.SelectedValue.Equals("C"))
                {
                    txtCalculaPrePrecoObjetivo.Enabled = true;
                    txtPrecoObjetivo.Enabled = false;
                }
                else
                {
                    txtCalculaPrePrecoObjetivo.Enabled = false;
                    txtPrecoObjetivo.Enabled = true;
                }
            }

        }
    }
}