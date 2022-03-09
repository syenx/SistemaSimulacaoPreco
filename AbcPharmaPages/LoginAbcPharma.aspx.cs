using KS.SimuladorPrecos.AppBaseInfo;
using KS.SimuladorPrecos.DataEntities;
using KS.SimuladorPrecos.DataEntities.Utility;
using OnConect.Dominio;
using OnConect.Repositorio.Factory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KS.SimuladorPrecos.AbcPharmaPages
{
    public partial class LoginAbcPharma : System.Web.UI.Page
    {

        private INaviLogRepositorio naviLogRepositorio;

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
            {
                this.txtLogin.Focus();
                this.imgHeader.ImageUrl = File.Exists(Server.MapPath("~/ImagemCliente/logo_cabecalho.png")) ? "~/ImagemCliente/logo_cabecalho.png" : "~/ImagemCliente/SemImagem.png";
                this.imgBody.ImageUrl = File.Exists(Server.MapPath("~/ImagemCliente/oncoprod.png")) ? "~/ImagemCliente/oncoprod.png" : "~/ImagemCliente/SemImagem.png";

                #region :: Acesso via KraftSales ::

                if (Request.Cookies["Simulador"] != null)
                {
                    if (!String.IsNullOrEmpty(Request.Cookies["Simulador"]["Login"]))
                    {
                        this.txtLogin.Text = Request.Cookies["Simulador"]["Login"];
                        this.txtPassword.Text = new PageBase().Decrypt(Server.UrlDecode(Request.Cookies["Simulador"]["Pwd"]));

                        EfetuaLogin();
                    }
                }

                #endregion


            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            EfetuaLogin();
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            RepositorioFactory.ProviderName = ConfigurationManager.
              ConnectionStrings["defaultConnection"].ProviderName;

            RepositorioFactory.ConnectionString = ConfigurationManager.
                ConnectionStrings["defaultConnection"].ConnectionString;

            RepositorioFactory.TipoRepositorio = (TipoRepositorio)
                Convert.ToInt32(ConfigurationManager.AppSettings["TipoRepositorio"]);

            naviLogRepositorio = RepositorioFactory.GetNaviLogRepositorio();

        }
        #endregion

        #region :: Métodos ::

      

        /// <summary>
        /// Carrega os Textos dos componentes da página web
        /// </summary>
        private void CarregaGlobalization()
        {
            this.Title = PageBase.GetResourceValueFromOutSide("titTitulo", false, "lblLogin");
            this.lblFooterVersion.Text = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            this.lblFooterCopiryght.Text = DateTime.Now.Year.ToString().Equals("2012") ?
                                           PageBase.GetResourceValueFromOutSide("lblCopyright", false, string.Empty) :
                                           PageBase.GetResourceValueFromOutSide("lblCopyright", true, " - " + DateTime.Now.Year.ToString());

            this.lblHeader.Text = PageBase.GetResourceValueFromOutSide("lblHeader");
            //   this.lblLogin.Text = string.Format("{0} / {1}:", PageBase.GetResourceValueFromOutSide("lblLogin"), PageBase.GetResourceValueFromOutSide("lblCodigo"));
            // this.lblPassword.Text = PageBase.GetResourceValueFromOutSide("lblSenha") + ":";
            this.lbEsqueciSenha.Text = PageBase.GetResourceValueFromOutSide("lblSenhaEsquecida");
            this.lbNaoCadastrado.Text = PageBase.GetResourceValueFromOutSide("lblNaoCadastrado");
            this.btnLogin.Text = PageBase.GetResourceValueFromOutSide("lblLogin");
            this.rfvLogin.ErrorMessage = PageBase.GetResourceValueFromOutSide("lblInformeDadosLogin");
            this.rfvSenha.ErrorMessage = PageBase.GetResourceValueFromOutSide("lblInformeDadosSenha");
        }

        /// <summary>
        /// Efetua o login na aplicação
        /// </summary>
        private void EfetuaLogin()
        {
            try
            {

                DataTable oDt = new DataTable();

                Usuario oUsr = new Usuario();


                oUsr.usuarioLogin = this.txtLogin.Text.ToUpper();
                oUsr.usuarioSenha = Utility.EncryptPassword(this.txtPassword.Text);


                oDt = oUsr.ListarLogin();

                if (oDt.Rows.Count > 0)
                {
                    #region :: Valida Acesso ::

                    if (String.IsNullOrEmpty(oDt.Rows[0]["usuarioSimuladorVisualizacao"].ToString()))
                    {
                        PopularModalErro(PageBase.GetResourceValueFromOutSide("msgAcessoNegado"));


                        return;
                    }

                    #endregion

                    #region :: Armazena as informações de acesso; IP e Data ::

                    UsuarioAcesso oUsrAcs = new UsuarioAcesso
                    {
                        usuarioId = oDt.Rows[0]["usuarioId"].ToString(),
                        usuarioAcessoData = DateTime.Now
                    };

                    if (!oUsrAcs.Incluir())
                    {
                        PopularModalErro(PageBase.GetResourceValueFromOutSide("lblUsuarioFalhaGravarAcesso"));

                        return;
                    }

                    #endregion

                    #region :: Recupera os dados do usuários ::

                    UserDataInfo userInfo = new UserDataInfo();

                    userInfo.UserContent = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}",
                                                          oDt.Rows[0]["usuarioId"].ToString(),
                                                          oDt.Rows[0]["usuarioLogin"].ToString(),
                                                          oDt.Rows[0]["usuarioSexo"].ToString(),
                                                          oDt.Rows[0]["usuarioAcessoData"].ToString(),
                                                          oDt.Rows[0]["usuarioNome"].ToString(),
                                                          oDt.Rows[0]["usuarioTipoId"].ToString(),
                                                          oDt.Rows[0]["perfilAcessoId"].ToString(),
                                                          oDt.Rows[0]["unidadeNegocioId"].ToString(),
                                                          oDt.Rows[0]["permiteAlterarRepreGrpCli"].ToString().ToLower(),
                                                          oDt.Rows[0]["usuarioSimuladorVisualizacao"].ToString(),
                                                          oDt.Rows[0]["usuarioSimuladorQuadro"].ToString(),
                                                          oDt.Rows[0]["usuarioNomeCompleto"].ToString());

                    Session.Add("USUARIO", userInfo);

                    if (naviLogRepositorio != null)
                    {
                        naviLogRepositorio.TracePage(oUsr.usuarioLogin, "Login Simulador efetuado em " + DateTime.Now.ToString());
                    }
                    Response.Redirect("~/AbcPharmaPages/ABCPharmaDownloadPlanilhas.aspx", false);

                    #endregion
                }
                else
                    PopularModalErro(PageBase.GetResourceValueFromOutSide("lblUsuarioNaoEncontrado"));

            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                
            }
        }

        private void Alert(string sMsg)
        {
            this.CtlAlert.Show(sMsg);
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