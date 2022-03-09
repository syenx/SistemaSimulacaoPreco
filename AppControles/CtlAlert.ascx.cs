using System;
using System.Web.UI;

namespace KS.SimuladorPrecos.AppControles
{
    /// <summary>
    /// Painel de mensagem de alerta personalizado
    /// </summary>
    public partial class CtlAlert : System.Web.UI.UserControl
    {
        #region :: Propriedades ::

        /// <summary>
        /// Informa se o botão close é apresentado no header
        /// </summary>
        public bool CloseButtonVisible
        {
            set { this.ibClose.Visible = value; }
        }

        #endregion

        #region :: Eventos ::

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ibClose_Click(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region :: Enum ::

        /// <summary>
        /// Enum de apresentação de imagem
        /// </summary>
        public enum Mark
        {
            /// <summary>
            /// Interrogação
            /// </summary>
            QuestionMark,
            /// <summary>
            /// Exclamação
            /// </summary>
            ExclamationMark,
            /// <summary>
            /// Erro
            /// </summary>
            ErrorMark,
            /// <summary>
            /// Sem ícone
            /// </summary>
            None
        }

        #endregion

        #region :: Métodos ::

        /// <summary>
        /// Fecha o Painel de Alerta
        /// </summary>
        private void Close()
        {
            this.pnlAlert.Visible = false;
        }

        /// <summary>
        /// Mostra a mensagem informando o texto do header
        /// </summary>
        /// <param name="sHeader">Texto a ser apresentado no header</param>
        /// <param name="sMsg">Texto da mensagem</param>
        public void Show(string sHeader, string sMsg)
        {
            this.lblHeaderText.Text = String.IsNullOrEmpty(sHeader) ? GetGlobalResourceObject("Resource", "lblAlertMessage").ToString() : sHeader;
            this.lblMessage.Text = sMsg;
            SetIconImage();
            this.pnlAlert.Visible = true;
            this.btnOk.Focus();
        }

        /// <summary>
        /// Mostra a mensagem
        /// </summary>
        /// <param name="sMsg">Texto da mensagem</param>
        /// <param name="Icone">Imagem</param>
        public void Show(string sMsg, Mark Icone)
        {
            this.lblHeaderText.Text = GetGlobalResourceObject("Resource", "lblAlertMessage").ToString();
            this.lblMessage.Text = sMsg;
            SetIconImage(Icone);
            this.pnlAlert.Visible = true;
            this.btnOk.Focus();
        }

        /// <summary>
        /// Mostra a mensagem de alerta
        /// </summary>
        /// <param name="sHeader">Cabeçalho a ser exibido</param>
        /// <param name="sMsg">Mensagem a ser exibida</param>
        /// <param name="Icone">Icone a ser exibido</param>
        public void Show(string sHeader, string sMsg, Mark Icone)
        {
            this.lblHeaderText.Text = String.IsNullOrEmpty(sHeader) ? GetGlobalResourceObject("Resource", "lblAlertMessage").ToString() : sHeader;
            this.lblMessage.Text = sMsg;
            SetIconImage(Icone);
            this.pnlAlert.Visible = true;
            this.btnOk.Focus();
        }

        /// <summary>
        /// Mostra a mensagem
        /// </summary>
        /// <param name="sMsg">Texto da mensagem</param>
        public void Show(string sMsg)
        {
            this.lblHeaderText.Text = GetGlobalResourceObject("Resource", "lblAlertMessage").ToString();
            this.lblMessage.Text = sMsg;
            SetIconImage();
            this.pnlAlert.Visible = true;
            this.btnOk.Focus();
        }

        /// <summary>
        /// Seta a imagem a ser exibida
        /// </summary>
        /// <param name="mark">Imagem a ser exibida</param>
        private void SetIconImage(Mark mark = Mark.ExclamationMark)
        {
            switch (mark)
            {
                case Mark.ErrorMark:
                    this.imgAlert.ImageUrl = "~/Imagens/_error.png";
                    break;

                case Mark.QuestionMark:
                    this.imgAlert.ImageUrl = "~/Imagens/question.png";
                    break;

                case Mark.None:
                    this.dvAlertIcon.Visible = false;
                    break;

                case Mark.ExclamationMark:
                default:
                    this.imgAlert.ImageUrl = "~/Imagens/exclamation.png";
                    break;
            }
        }

        #endregion
    }
}