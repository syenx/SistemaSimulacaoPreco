using KS.SimuladorPrecos.AppBaseInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KS.SimuladorPrecos.AppMaster
{
    public partial class ABCPharma : System.Web.UI.MasterPage
    {
        #region :: Propriedades ::

        /// <summary>
        /// Informa se o usuário está logado no sistema
        /// </summary>
        private bool IsAuthenticated
        {
            get { return new PageBase().User.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// Informações da classe base da aplicação
        /// </summary>
        private PageBase AppBase
        {
            get { return new PageBase(); }
        }

        /// <summary>
        /// ClientID do LinkButton Menu
        /// </summary>
        public string sCtlClientID = string.Empty;

        #endregion

        #region :: Eventos ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            CarregaGlobalization();

            if (!Page.IsPostBack)
                CarregaImagemHeader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbLogOnLogOff_Click(object sender, EventArgs e)
        {
            if (Session["USUARIO"] != null)
                ShowHidePanelConfirm(true);
            else
                Credenciais();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmOk_Click(object sender, EventArgs e)
        {
            Credenciais();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmNo_Click(object sender, EventArgs e)
        {
            ShowHidePanelConfirm(false);
        }

        #endregion

        #region :: Métodos ::

        /// <summary>
        /// Mostra/Esconde o painel para a confirmação do login
        /// </summary>
        /// <param name="visible">Informa se mostra ou esconde o painel de confirmação</param>
        private void ShowHidePanelConfirm(bool visible)
        {
            // this.pnlConfirma.Visible = visible;
            if (visible) this.btnConfirmNo.Focus();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "exampleModal", "$('#exampleModal').modal();", true);

        }

        /// <summary>
        /// Efetua o Login caso o Usuário Não Seja Credenciado ou Logout Caso Seja
        /// </summary>
        private void Credenciais()
        {
            Session.Abandon();

            if (Request.Cookies["Simulador"] != null)
            {
                if (!String.IsNullOrEmpty(Request.Cookies["Simulador"]["Login"]))
                {
                    if (WebConfigurationManager.AppSettings["KraftSales"] != null)
                    {
                        if (!String.IsNullOrEmpty(WebConfigurationManager.AppSettings["KraftSales"].ToString()))
                            Response.Redirect(WebConfigurationManager.AppSettings["KraftSales"].ToString());
                        else
                            Response.Redirect("~/AbcPharmaPages/LoginAbcPharma.aspx", true);
                    }
                    else
                        Response.Redirect("~/AbcPharmaPages/LoginAbcPharma.aspx", true);
                }
                else
                    Response.Redirect("~/AbcPharmaPages/LoginAbcPharma.aspx", true);
            }
            else
                Response.Redirect("~/AbcPharmaPages/LoginAbcPharma.aspx", true);
        }

        private void Alert(string sMsg)
        {
            this.CtlAlert.Show(sMsg);
        }

        /// <summary>
        /// Carrega os textos dos componentes da página web
        /// </summary>
        private void CarregaGlobalization()
        {
            #region :: Header ::

            //this.lblHeader.Text = PageBase.GetResourceValueFromOutSide("lblHeader");
            this.lblNomeUsuario.Text = PageBase.GetResourceValueFromOutSide("lblUsuarioNome", true, Session["USUARIO"] != null ? ((UserDataInfo)Session["USUARIO"]).UserName.ToUpper().Split(' ')[0].ToString() : "lblUsuario");
            this.lblNomeUsuario.ToolTip = Session["USUARIO"] != null ? ((UserDataInfo)Session["USUARIO"]).UserNameComplete.ToUpper() : PageBase.GetResourceValueFromOutSide("lblUsuario");
            this.lbLogOnLogOff.Text = PageBase.GetResourceValueFromOutSide(Session["USUARIO"] != null ? "lblLogout" : "lblLogin");

            this.lblAcesso.Text = Session["USUARIO"] != null ? ((UserDataInfo)Session["USUARIO"]).UserIsFisrtAccess ? string.Empty :
                                        PageBase.GetResourceValueFromOutSide("lblAcesso", true, ((UserDataInfo)Session["USUARIO"]).UserUltimoAcesso.Value.Day.ToString().PadLeft(2, '0'),
                                                PageBase.GetMonthFromOutSide(((UserDataInfo)Session["USUARIO"]).UserUltimoAcesso.Value.Month),
                                                            ((UserDataInfo)Session["USUARIO"]).UserUltimoAcesso.Value.Year.ToString(),
                                                                ((UserDataInfo)Session["USUARIO"]).UserUltimoAcesso.Value.ToShortTimeString()) : string.Empty;

            #endregion



            #region :: Painel Confirmação ::

            this.lblConfirmHeader.Text = PageBase.GetResourceValueFromOutSide("lblAlertMessage");
            this.lblConfirmBody.Text = PageBase.GetResourceValueFromOutSide("msgLogout");
            this.btnConfirmNo.Text = PageBase.GetResourceValueFromOutSide("lblNao");
            this.btnConfirmOk.Text = PageBase.GetResourceValueFromOutSide("lblSim");

            #endregion

            #region :: Menu ::

            //this.lbHome.Text = PageBase.GetResourceValueFromOutSide("lblHome");

            #region :: Cadastros ::

            /*this.lbCadastros.Text = PageBase.GetResourceValueFromOutSide("lblCadastro") + "S";
            this.lbCadUsuario.Text = PageBase.GetResourceValueFromOutSide("lblUsuario");
            this.lbCadUnidadeNegocio.Text = PageBase.GetResourceValueFromOutSide("lblUnidadeNegocio");
            this.lbCadFamiliaComercial.Text = PageBase.GetResourceValueFromOutSide("lblFamiliaComercial");
            this.lbCadCondicaoPagamento.Text = PageBase.GetResourceValueFromOutSide("lblCondicaoPagamento");
            this.lbCadCanalVendas.Text = PageBase.GetResourceValueFromOutSide("lblCanalVenda");
            this.lbCadNaturezaOperacao.Text = PageBase.GetResourceValueFromOutSide("lblNaturezaOperacao");
            this.lbCadClienteEstabelecimento.Text = PageBase.GetResourceValueFromOutSide("lbClienteEstabelecimento");
            this.lbCadUnidadeMedida.Text = PageBase.GetResourceValueFromOutSide("lblUnidadeMedida");
            this.lbCadDominio.Text = PageBase.GetResourceValueFromOutSide("lblDominio");
            this.lbCadRepresentante.Text = PageBase.GetResourceValueFromOutSide("lblRepresentante");
            this.lbCadEmpresa.Text = PageBase.GetResourceValueFromOutSide("lblEmpresa");
            this.lbCadCondicaoPagamentoParcela.Text = PageBase.GetResourceValueFromOutSide("lblCondicaoPagamentoParcela");
            this.lbCadClienteGrupoFinanceiro.Text = PageBase.GetResourceValueFromOutSide("lblClienteGrupoFinanceiro");
            this.lbCadClienteGrupoComercial.Text = PageBase.GetResourceValueFromOutSide("lblClienteGrupoComercial");
            this.lbCadPais.Text = PageBase.GetResourceValueFromOutSide("lblCadPais");
            this.lbCadUf.Text = PageBase.GetResourceValueFromOutSide("lblUf");
            this.lbCadGrupoEstoque.Text = PageBase.GetResourceValueFromOutSide("lblGrupoEstoque");
            this.lbCadFamiliaMaterial.Text = PageBase.GetResourceValueFromOutSide("lblFamiliaMaterial");
            this.lbCadMercado.Text = PageBase.GetResourceValueFromOutSide("lblMercado");
            this.lbCadDepartamento.Text = PageBase.GetResourceValueFromOutSide("lblDepartamento");
            this.lbCadTipoDocumento.Text = PageBase.GetResourceValueFromOutSide("lblTipoDocumento");
            this.lbCadTipoEndereco.Text = PageBase.GetResourceValueFromOutSide("lblTipoEndereco");
            this.lbCadClassificacaoFiscal.Text = PageBase.GetResourceValueFromOutSide("lblClassificacaoFiscal");*/

            #endregion

            #region :: Consultas ::

            //this.lbConsultas.Text = PageBase.GetResourceValueFromOutSide("lblConsulta") + "S";

            #endregion

            #region :: Relatórios ::

            //this.lbRelatorios.Text = PageBase.GetResourceValueFromOutSide("lblRelatorio") + "S";

            #endregion

            #endregion

            #region :: Rodapé ::

            this.lblFooterCopiryght.Text = DateTime.Now.Year.ToString().Equals("2012") ?
                                           PageBase.GetResourceValueFromOutSide("lblCopyright", false, string.Empty) :
                                           PageBase.GetResourceValueFromOutSide("lblCopyright", true, " - " + DateTime.Now.Year.ToString());

            this.lblFooterVersion.Text = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            #endregion
        }




        /// <summary>
        /// Carrega as imagens do cliente
        /// </summary>
        private void CarregaImagemHeader()
        {
            //this.imgHeader.ImageUrl = File.Exists(Server.MapPath("~/ImagemCliente/logo_cabecalho.png")) ? "~/ImagemCliente/logo_cabecalho.png" : "~/ImagemCliente/SemImagem.png";
            //    this.imgUsuario.ImageUrl = Session["USUARIO"] == null ? PageBase.GetResourceValueFromOutSide("imgIconeNaoLogado") : ((UserDataInfo)Session["USUARIO"]).UserGenero.Equals(PageBase.Genero.M) ? PageBase.GetResourceValueFromOutSide("imgIconeLogadoHomem") : PageBase.GetResourceValueFromOutSide("imgIconeLogadoMulher");
        }

        #endregion
    }
}