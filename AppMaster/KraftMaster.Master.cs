using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using System.Web.UI;
using KS.SimuladorPrecos.AppBaseInfo;

namespace KS.SimuladorPrecos.AppMaster
{
    /// <summary>
    /// 
    /// </summary>
    public partial class KraftMaster : System.Web.UI.MasterPage
    {
        #region :: Propriedades ::



        private bool alteraMenuPMPF
        {
            get; set;
        }

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

        
        private void ShowHidePanelConfirm(bool visible)
        {
            
            if (visible) this.btnConfirmNo.Focus();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "exampleModal", "$('#exampleModal').modal();", true);
        
        }


        private void Credenciais()
        {
            Session.Abandon();

            Response.Redirect("~/Login.aspx", true);
        }

    
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

        protected void checkIdAtivaPMPF_CheckedChanged(object sender, EventArgs e)
        {
            ConfigurationManager.AppSettings.Set("AtivaPMPF", (checkIdAtivaPMPF.Checked) ? true.ToString() : false.ToString());
        }
    }
}