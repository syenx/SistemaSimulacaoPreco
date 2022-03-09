using System;

namespace KS.SimuladorPrecos.AppControles
{
    /// <summary>
    /// Classe com as propriedades do controle Option(CtlOption)
    /// </summary>
    public class OptionBody
    {
        #region :: Enum ::

        /// <summary>
        /// Informa a opção selecionada
        /// </summary>
        public enum Option
        {
            /// <summary>
            /// Sim
            /// </summary>
            Yes,
            /// <summary>
            /// Não
            /// </summary>
            No
        }

        #endregion

        #region :: Propriedades ::

        /// <summary>
        /// Opção selecionada
        /// </summary>
        public Option Decision { get; set; }

        /// <summary>
        /// Texto de comando
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Dados passados como parâmetro
        /// </summary>
        public object[] RetrieveParameters { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class CtlOption : System.Web.UI.UserControl
    {
        #region :: ViewState ::

        /// <summary>
        /// Texto de comando
        /// </summary>
        private string CommandText
        {
            get { return this.ViewState["CommandText"] != null ? !String.IsNullOrEmpty(this.ViewState["CommandText"].ToString()) ? this.ViewState["CommandText"].ToString() : "Default" : "Default"; }
            set { this.ViewState["CommandText"] = value; }
        }

        /// <summary>
        /// Recupera os parâmetros armazenados
        /// </summary>
        private object[] RetrieveParameters
        {
            get { return this.ViewState["RetrieveParameters"] != null ? (object[])this.ViewState["RetrieveParameters"] : null; }
            set { this.ViewState["RetrieveParameters"] = value; }
        }

        #endregion

        #region :: Delegate & Event ::

        /// <summary>
        /// Delegate da decisão
        /// </summary>
        /// <param name="sender">Botão</param>
        /// <param name="e">Detalhes</param>
        public delegate void Selection(OptionBody e);

        /// <summary>
        /// Evento da decisão
        /// </summary>
        public event Selection OnSelection;

        #endregion

        #region :: Enum ::

        /// <summary>
        /// Opções possíveis
        /// </summary>
        private enum Option
        {
            /// <summary>
            /// Sim
            /// </summary>
            Yes,
            /// <summary>
            /// Não
            /// </summary>
            No
        }

        #endregion

        #region :: Eventos ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnYes_Click(object sender, EventArgs e)
        {
            OnOptionSelection(Option.Yes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNo_Click(object sender, EventArgs e)
        {
            OnOptionSelection();
        }

        #endregion

        #region :: Métodos ::

        /// <summary>
        /// Fecha o painel de opção
        /// </summary>
        private void Close()
        {
            this.pnlOption.Visible = false;
        }

        /// <summary>
        /// Apresenta o painel e seta o focu no botão NO.
        /// </summary>
        private void ShowDialog()
        {
            this.pnlOption.Visible = true;
            this.btnNo.Focus();
        }

        /// <summary>
        /// Apresenta o painel de opções Yes/No
        /// </summary>
        /// <param name="_message">Mensagem a ser apresentada</param>
        /// <param name="_commandText">Texto de comando para o controle</param>
        /// <param name="_header">Texto a ser apresentado no header</param>
        public void Show(string _message = "", string _commandText = "Default", string _header = "")
        {
            this.CommandText = _commandText;

            this.lblHeaderText.Text = 
                !String.IsNullOrEmpty(_header) ? _header :
                    GetGlobalResourceObject("Resource", "lblConfirmar").ToString();

            this.lblMessage.Text = 
                !String.IsNullOrEmpty(_message) ? _message : 
                    GetGlobalResourceObject("Resource", "lblConfirmaOperacao").ToString();

            ShowDialog();
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        public void Show(params object[] _storeParameters)
        {
            this.RetrieveParameters = _storeParameters;

            this.lblHeaderText.Text =
                GetGlobalResourceObject("Resource", "lblConfirmar").ToString();

            this.lblMessage.Text =
                    GetGlobalResourceObject("Resource", "lblConfirmaOperacao").ToString();

            ShowDialog();
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_commandText">Texto de comando</param>
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        public void Show(string _commandText, params object[] _storeParameters)
        {
            this.CommandText = _commandText;
            this.RetrieveParameters = _storeParameters;            

            this.lblHeaderText.Text = 
                GetGlobalResourceObject("Resource", "lblConfirmar").ToString();

            this.lblMessage.Text = 
                    GetGlobalResourceObject("Resource", "lblConfirmaOperacao").ToString();

            ShowDialog();
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_message">Mensagem a ser apresentada</param>
        /// <param name="_commandText">Texto de comando</param>
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        public void Show(string _message, string _commandText, params object[] _storeParameters)
        {
            this.CommandText = _commandText;
            this.RetrieveParameters = _storeParameters;

            this.lblHeaderText.Text =
                GetGlobalResourceObject("Resource", "lblConfirmar").ToString();

            this.lblMessage.Text =
                !String.IsNullOrEmpty(_message) ? _message :
                    GetGlobalResourceObject("Resource", "lblConfirmaOperacao").ToString();

            ShowDialog();
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_header">Texto a ser apresentado no header</param>
        /// <param name="_message">Mensagem a ser apresentada</param>
        /// <param name="_comandText">Texto de comando</param>        
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        public void Show(string _header, string _message, string _comandText, params object[] _storeParameters)
        {
            this.CommandText = _comandText;
            this.RetrieveParameters = _storeParameters;

            this.lblHeaderText.Text =
                !String.IsNullOrEmpty(_header) ? _header :
                    GetGlobalResourceObject("Resource", "lblConfirmar").ToString();

            this.lblMessage.Text =
                !String.IsNullOrEmpty(_message) ? _message :
                    GetGlobalResourceObject("Resource", "lblConfirmaOperacao").ToString();

            ShowDialog();
        }

        /// <summary>
        /// Lança o evento com a decisão do usuário.
        /// </summary>
        /// <param name="_option">Opção selecionada</param>
        private void OnOptionSelection(Option _option = Option.No)
        {
            try
            {
                switch (_option)
                {
                    case Option.Yes:

                        if (OnSelection != null)
                            OnSelection(new OptionBody
                                            {
                                                Decision = OptionBody.Option.Yes,
                                                CommandText = this.CommandText,
                                                RetrieveParameters = this.RetrieveParameters
                                            }
                                       );

                        break;

                    default:

                        if (OnSelection != null)
                            OnSelection(new OptionBody
                                            {
                                                Decision = OptionBody.Option.No,
                                                CommandText = this.CommandText,
                                                RetrieveParameters = this.RetrieveParameters
                                            }
                                       );

                        break;
                }
            }
            finally
            {
                this.CommandText = string.Empty;
                this.RetrieveParameters = null;
                Close();
            }
        }

        #endregion
    }
}