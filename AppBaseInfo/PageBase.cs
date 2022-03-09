using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using KS.SimuladorPrecos.AppControles;
using KS.SimuladorPrecos.DataEntities.Utility;
using KS.SimuladorPrecos.DataEntities.Utility.Security;
using ajax = AjaxControlToolkit;

namespace KS.SimuladorPrecos.AppBaseInfo
{
    /// <summary>
    /// Classe base da aplicação, onde todas as outras herdam as funcionalidades básicas para a generalização da aplicação.
    /// </summary>
    public class PageBase : System.Web.UI.Page
    {

       

        #region :: Declarações Globais ::

        /// <summary>
        /// Declaração Global de Acesso a Struct Usuário Logado
        /// </summary>
        public UserDataInfo UserDataInfo;
        //public static UserDataInfo UserDataInfoOutSide;

        #endregion

        #region :: Campos ::

        /// <summary>
        /// Informa qual Menu Receberá o Focu
        /// </summary>
        public static int MENUFOCUS = new int();

        /// <summary>
        /// Define o perfil do pedido quando selecionado pelo administrador
        /// </summary>
        public static string PerfilPedido = string.Empty;

        #endregion

        #region :: Propriedades ::

        /// <summary>
        /// Acesso ao controle 'Alert' da MasterPage
        /// </summary>
        private CtlAlert Alerta
        {
            get { return ((CtlAlert)Master.FindControl("CtlAlert")); }
        }

        /// <summary>
        /// Acesso ao controle 'Option' da MasterPage
        /// </summary>
        private CtlOption Option
        {
            get { return ((CtlOption)Master.FindControl("CtlOption")); }
        }

        #endregion

        #region :: Enum ::

        /// <summary>
        /// Enum de idêntificação do Menu
        /// </summary>
        public enum Menu
        {
            Home = 1,
            Cadastros = 2
        }

        /// <summary>
        /// Gênero do usuário logado
        /// </summary>
        public enum Genero
        {
            /// <summary>
            /// Masculino
            /// </summary>
            M,
            /// <summary>
            /// Feminino
            /// </summary>
            F
        }

        /// <summary>
        /// Tipos de usuário
        /// </summary>
        public enum TipoUsuario
        {
            /// <summary>
            /// Administrador
            /// </summary>
            ADM,
            /// <summary>
            /// Gerente
            /// </summary>
            GER,
            /// <summary>
            /// Supervisor
            /// </summary>
            SUP,
            /// <summary>
            /// Atendente
            /// </summary>
            ATD,
            /// <summary>
            /// Representante
            /// </summary>
            REP            
        }

        /// <summary>
        /// Tipo do perfil do usuário ogado
        /// </summary>
        public enum TipoPerfil
        {
            /// <summary>
            /// BackOffice conferência de cadastro
            /// </summary>
            BCC,
            /// <summary>
            /// BackOffice financeiro
            /// </summary>
            BF,
            /// <summary>
            /// BackOffice receita
            /// </summary>
            BR,
            /// <summary>
            /// Cadastra clientes
            /// </summary>
            C,
            /// <summary>
            /// Cadastra clientes e emite pedidos
            /// </summary>
            CP,
            /// <summary>
            /// Emite pedidos
            /// </summary>
            P,
            /// <summary>
            /// Acesso total
            /// </summary>
            T,
            /// <summary>
            /// BackOffice financeiro PJ 
            /// </summary>
            BFPJ
        }

        /// <summary>
        /// TIPO DE CLIENTE
        /// </summary>
        public enum tipoCliente
        {
            J,
            F,
            E
        }

        /// <summary>
        /// Tipo de unidade de negócio
        /// </summary>
        public enum TipoUnidadeNegocio
        {
            /// <summary>
            /// Pessoa física
            /// </summary>
            PF,
            /// <summary>
            /// Pessoa jurídica privado
            /// </summary>
            PJPrivado,
            /// <summary>
            /// Pessoa jurídica pública
            /// </summary>
            PJPublico,
            /// <summary>
            /// Setor de compras
            /// </summary>
            Compras,
            /// <summary>
            /// Setor de eCommerce
            /// </summary>
            eCommerce
        }

        /// <summary>
        /// Período de entrega
        /// </summary>
        public enum PeriodoEntrega
        {
            /// <summary>
            /// Manhã
            /// </summary>
            M,
            /// <summary>
            /// Tarde
            /// </summary>
            T,
            /// <summary>
            /// Horário comercial
            /// </summary>
            H,
            /// <summary>
            /// Horário diferenciado
            /// </summary>
            Outros
        }

        /// <summary>
        /// Tipo de visualização
        /// </summary>
        public enum VisualizacaoTipo
        {
            Completa,
            Resumida
        }

        /// <summary>
        /// Tipo de visualização aos quadros do simulador
        /// </summary>
        public enum VisualizacaoQuadro
        {
            Sumarizada,
            Resumida
        }

        #endregion

        #region :: Eventos ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_InitComplete(object sender, EventArgs e)
        {
            if (Session["USUARIO"] != null)
                this.UserDataInfo = (UserDataInfo)Session["USUARIO"];
            else
                Response.Redirect("~/Login.aspx", true);

            #region :: try - Para a utilização em páginas onde não existem o controle ::

            try
            {
                Option.OnSelection += new CtlOption.Selection(OnSelection);
            }
            catch
            {
            }

            #endregion
        }

        #endregion

        #region :: Métodos ::

        #region :: Construtor ::

        /// <summary>
        /// Construtor
        /// </summary>
        public PageBase()
        {
            UserDataInfo = new UserDataInfo();
        }

        #endregion

        #region :: Outros ::

        /// <summary>
        /// Método lançado na escolha da opção SIM/NÃO
        /// </summary>
        /// <param name="e">Dados retornados na decisão</param>
        protected virtual void OnSelection(OptionBody e)
        {
            return;
        }

        /// <summary>
        /// Método utilizado para o redirecionamento centralizado
        /// </summary>
        /// <param name="sPath">Caminho da página a ser direcionada</param>
        protected void Redirect(string sPath)
        {
            try
            {
                Response.Redirect(sPath, true);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
            }
        }

        /// <summary>
        /// Retorna o nome do mês
        /// </summary>
        /// <param name="mes">número do mês</param>
        /// <returns>O nome do mês</returns>
        protected string GetMonthName(int mes)
        {
            switch (mes)
            {
                case 1:
                    return "Janeiro";

                case 2:
                    return "Fevereiro";

                case 3:
                    return "Marco";

                case 4:
                    return "Abril";

                case 5:
                    return "Maio";

                case 6:
                    return "Junho";

                case 7:
                    return "Julho";

                case 8:
                    return "Agosto";

                case 9:
                    return "Setembro";

                case 10:
                    return "Outubro";

                case 11:
                    return "Novembro";

                case 12:
                    return "Dezembro";

                default:
                    return "Janeiro";
            }
        }

        /// <summary>
        /// Retorna a descrição do tipo do usuário
        /// </summary>
        /// <param name="usuarioTipo">Tipo do usuário</param>
        /// <returns></returns>
        protected string GetTipoUsuario(string usuarioTipo)
        {
            switch (usuarioTipo.ToLower())
            {
                case "admin":
                    return GetResourceValue("lblAdmin");

                case "gerente":
                    return GetResourceValue("lblGerente");

                case "supervisor":
                    return GetResourceValue("lblSupervisor");

                case "apoioVenda":
                    return GetResourceValue("lblApoioVenda");

                case "representante":
                    return GetResourceValue("lblRepresentante");

                case "vendedor":
                    return GetResourceValue("lblVendedor");

                case "cliente":
                    return GetResourceValue("lblCliente");

                default:
                    return GetResourceValue("lblVendedor");
            }
        }

        /// <summary>
        /// Apresenta ou Oculta um Menu 
        /// </summary>
        /// <param name="oImb">Botão Chamador</param>
        /// <param name="oPnl">Painel do Menu Selecionado</param>
        protected void ShowHideMenu(ImageButton oImb, Panel oPnl)
        {
            oPnl.Visible = oPnl.Visible ? false : true;
            oImb.ImageUrl = oPnl.Visible ? "~/Imagens/Colapse.png" : "~/Imagens/Expand.png";
            oImb.ToolTip = oPnl.Visible ? GetResourceValue("lblContrair") : GetResourceValue("lblExpandir");
        }

        /// <summary>
        /// Compara a igualdade das strings, independente de Case e/ou acentuação.
        /// </summary>
        /// <param name="_value">Valor à ser comparado</param>
        /// <param name="_compare">Valor com o qual comparar</param>
        /// <returns>true: Se igual; false: Se diferente;</returns>
        protected bool IsEquals(string _value, params string[] _compare)
        {
            try
            {
                if (String.IsNullOrEmpty(_value))
                    return false;

                foreach (string s in _compare)
                    if (new Regex(Utility.FormataStringPesquisa(_value), RegexOptions.IgnoreCase).IsMatch(s))
                        return true;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Compara a igualdade das strings, independente de Case e/ou acentuação e retorna o valor validado.
        /// </summary>
        /// <param name="_value">Valor à ser comparado</param>
        /// <param name="_compare">Valores com o qual se comparar</param>
        /// <returns>Retorna a chave na qual o valor se enquadra</returns>
        protected string GetEquality(string _value, params string[] _compare)
        {
            string _returnValue = GetResourceValue("lblNaoConsta");

            try
            {
                if (String.IsNullOrEmpty(_value))
                    return _returnValue;

                foreach (string s in _compare)
                    if (new Regex(Utility.FormataStringPesquisa(_value), RegexOptions.IgnoreCase).IsMatch(s))
                        return s;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return _returnValue;
            }

            return _returnValue;
        }

        #endregion

        #region :: Show Confirm Option ::

        /// <summary>
        /// Apresenta o painel de opções Yes/No
        /// </summary>
        /// <param name="_message">Mensagem a ser apresentada</param>
        /// <param name="_commandText">Texto de comando para o controle</param>
        /// <param name="_header">Texto a ser apresentado no header</param>
        protected void Confirm(string _message = "", string _commandText = "Default", string _header = "")
        {
            Option.Show(_message, _commandText, _header);
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        protected void Confirm(params object[] _storeParameters)
        {
            Option.Show(_storeParameters);
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_commandText">Texto de comando</param>
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        protected void Confirm(string _commandText, params object[] _storeParameters)
        {
            Option.Show(_commandText, _storeParameters);
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_message">Mensagem a ser apresentada</param>
        /// <param name="_commandText">Texto de comando</param>
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        protected void Confirm(string _message, string _commandText, params object[] _storeParameters)
        {
            Option.Show(_message, _commandText, _storeParameters);
        }

        /// <summary>
        /// Apresenta o painel de confirmação
        /// </summary>
        /// <param name="_header">Texto a ser apresentado no header</param>
        /// <param name="_message">Mensagem a ser apresentada</param>
        /// <param name="_comandText">Texto de comando</param>        
        /// <param name="_storeParameters">Lista de objetos a serem armazenados</param>
        protected void Confirm(string _header, string _message, string _comandText, params object[] _storeParameters)
        {
            Option.Show(_header, _message, _comandText, _storeParameters);
        }

        #endregion

        #region :: Bind de DataSources em componentes ::

        /// <summary>
        /// Carrega o DropDownList com a Fonte de Dados Passada como Parâmetro
        /// </summary>
        /// <param name="oDDl">Drop a ser Carregado</param>
        /// <param name="oSource">Fonte de Dados</param>
        /// <param name="value">Valor a ser fixado na propriedade DataValueField</param>
        /// <param name="text">Valor a ser fixado na propriedade DataTextField</param>
        protected void CarregaDrop(DropDownList oDDl, object oSource, string text = "Descricao", string value = "Id")
        {
            try
            {
                oDDl.Items.Clear();
                oDDl.DataValueField = value;
                oDDl.DataTextField = text;                
                oDDl.DataSource = oSource;
                oDDl.DataBind();
                oDDl.Items.Insert(0, new ListItem(this.GetResourceValue("lblSelecione"), string.Empty));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Carrega o DropDownList com a Fonte de Dados Passada como Parâmetro
        /// </summary>
        /// <param name="oDDl">Drop a ser carregado</param>
        /// <param name="oSource">Fonte de dados</param>
        protected void CarregaDrop(object oSource, params DropDownList[] oDDl)
        {
            try
            {
                foreach (DropDownList ddl in oDDl)
                {
                    ddl.Items.Clear();
                    ddl.DataTextField = "Descricao";
                    ddl.DataValueField = "Id";
                    ddl.DataSource = oSource;
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem(this.GetResourceValue("lblSelecione"), string.Empty));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Carrega o RadioList com a Fonte de Dados Passada como Parâmetro
        /// </summary>
        /// <param name="oRlst">RadioList a ser Carregado</param>
        /// <param name="oSource">Fonte de Dados</param>
        /// <param name="value">Valor a ser fixado na propriedade DataValueField</param>
        /// <param name="text">Valor a ser fixado na propriedade DataTextField</param>
        protected void CarregaRadioList(RadioButtonList oRlst, object oSource, string text = "Descricao", string value = "Id")
        {
            try
            {
                oRlst.Items.Clear();
                oRlst.DataValueField = value;
                oRlst.DataTextField = text;                
                oRlst.DataSource = oSource;
                oRlst.DataBind();
                oRlst.Items[0].Selected = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Carrega o RadioList com a Fonte de Dados Passada como Parâmetro
        /// </summary>
        /// <param name="oChkLst">RadioList a ser Carregado</param>
        /// <param name="oSource">Fonte de Dados</param>
        /// /// <param name="value">Valor a ser fixado na propriedade DataValueField</param>
        /// <param name="text">Valor a ser fixado na propriedade DataTextField</param>
        protected void CarregaChkList(CheckBoxList oChkLst, object oSource, string text = "Descricao", string value = "Id")
        {
            try
            {
                oChkLst.Items.Clear();
                oChkLst.DataValueField = value;
                oChkLst.DataTextField = text;
                oChkLst.DataSource = oSource;
                oChkLst.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Carrega ListBox
        /// </summary>
        /// <param name="oLstBox">ListBox</param>
        /// <param name="oSource">Fonte de dados</param>
        /// <param name="value">Valor a ser fixado na propriedade DataValueField</param>
        /// <param name="text">Valor a ser fixado na propriedade DataTextField</param>
        protected void CarregaListBox(ListBox oLstBox, object oSource, string text = "Descricao", string value = "Id")
        {
            try
            {
                oLstBox.Items.Clear();
                oLstBox.DataValueField = value;
                oLstBox.DataTextField = text;
                oLstBox.DataSource = oSource;
                oLstBox.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region :: Cleaning Data ::

        /// <summary>
        /// Retorna os Componentes ao seu Valor inicial
        /// </summary>
        /// <param name="oCtls">Controles a Serem Inicializados</param>
        protected void Clean(params Control[] oCtls)
        {
            foreach (Control oCtl in oCtls)
            {
                if (oCtl is TextBox)
                    ((TextBox)oCtl).Text = string.Empty;
                else if (oCtl is Label)
                {
                    ((Label)oCtl).Text = string.Empty;
                    ((Label)oCtl).ToolTip = string.Empty;
                }
                else if (oCtl is DropDownList)
                    ((DropDownList)oCtl).SelectedValue = string.Empty;
                else if (oCtl is HiddenField)
                    ((HiddenField)oCtl).Value = string.Empty;
                else if (oCtl is Panel)
                {
                    ((Panel)oCtl).Visible = false;
                    ((Panel)oCtl).ToolTip = string.Empty;
                }
                else if (oCtl is CheckBox)
                    ((CheckBox)oCtl).Checked = false;
                else if (oCtl is RadioButtonList)
                    ((RadioButtonList)oCtl).Items[0].Selected = true;
                else if (oCtl is Image)
                    ((Image)oCtl).ToolTip = string.Empty;
                else if (oCtl is CheckBoxList)
                {
                    foreach (ListItem item in ((CheckBoxList)oCtl).Items)
                        item.Selected = false;
                }
                else if (oCtl is GridView)
                {
                    ((GridView)oCtl).DataSource = null;
                    ((GridView)oCtl).DataBind();
                }
                else if (oCtl is ListBox)
                {
                    ((ListBox)oCtl).Items.Clear();
                    ((ListBox)oCtl).DataSource = null;
                    ((ListBox)oCtl).DataBind();
                }
                else if (oCtl is DataList)
                {
                    ((DataList)oCtl).DataSource = null;
                    ((DataList)oCtl).DataBind();
                }
            }
        }

        /// <summary>
        /// Esvazia os controles informados
        /// </summary>
        /// <param name="oCtls">Lista de controles</param>
        protected void EmptyDrop(params Control[] oCtls)
        {
            foreach (Control oCtl in oCtls)
            {
                if (oCtl is DropDownList)
                {
                    ((DropDownList)oCtl).Items.Clear();
                    ((DropDownList)oCtl).DataSource = null;
                    ((DropDownList)oCtl).DataBind();
                }
                else if (oCtl is ListBox)
                {
                    ((ListBox)oCtl).Items.Clear();
                    ((ListBox)oCtl).DataSource = null;
                    ((ListBox)oCtl).DataBind();
                }
            }
        }

        #endregion

        #region :: Formatações e validações ::

        /// <summary>
        /// Formata a string para a saída do bloco utilizado como filtro em negrito
        /// </summary>
        /// <param name="texto">Texto</param>
        /// <param name="filtro">Filtro de pesquisa</param>
        /// <returns>String formatada</returns>
        protected string FormataStringSaida(object texto, object filtro)
        {
            try
            {
                texto = new Regex(filtro.ToString(), RegexOptions.IgnoreCase).Replace(texto.ToString(), string.Format("<b>{0}</b>", filtro.ToString()));
            }
            catch
            {
                return texto.ToString().ToUpperInvariant();
            }

            return texto.ToString().ToUpperInvariant();
        }

        /// <summary>
        /// Verifica se existe item selecionado no controlado
        /// </summary>
        /// <param name="itens">Itens da lista</param>
        /// <returns>true;false</returns>
        protected bool HasItemSelected(ListItemCollection itens)
        {
            try
            {
                if (itens.Cast<ListItem>().Any(x => x.Selected))
                    return true;
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                return false;
            }

            return false;
        }

        #endregion

        #region :: Alertas ::

        /// <summary>
        /// Mensagem de alert padrão do navegador
        /// </summary>
        /// <param name="sMsg">Mensagem a ser apresentada</param>
        protected void AlertDefault(string sMsg)
        {
            try
            {
                if (!ClientScript.IsClientScriptBlockRegistered("Alert"))
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>window.alert('" + sMsg + "');</script>", false);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMsg"></param>
        protected void AlertAjax(string sMsg)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Alert", "<script>window.alert('" + sMsg + "');</script>", false);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
            }
        }

        /// <summary>
        /// Mostra a mensagem de alerta
        /// </summary>
        /// <param name="sMsg">Mensagem</param>
        protected void Alert(string sMsg)
        {
            Alerta.Show(sMsg);
        }

        /// <summary>
        /// Mostra a mensagem de alerta
        /// </summary>
        /// <param name="sMsg">Mensagem a ser exibida</param>
        /// <param name="mark">Icone a ser exibido</param>
        protected void Alert(string sMsg, CtlAlert.Mark mark)
        {
            Alerta.Show(sMsg, mark);
        }

        /// <summary>
        /// Mostra a mensagem de alerta
        /// </summary>
        /// <param name="sHeader">Texto a ser apresentado no header da caixa de mensagem</param>
        /// <param name="sMsg">Mensagem</param>
        protected void Alert(string sHeader, string sMsg)
        {
            Alerta.Show(sHeader, sMsg);
        }

        /// <summary>
        /// Mostra a mensagem de alerta
        /// </summary>
        /// <param name="sHeader">Cabeçalho a ser exibido</param>
        /// <param name="sMsg">Mensagem a ser exibida</param>
        /// <param name="mark">Icone a ser exibido</param>
        protected void Alert(string sHeader, string sMsg, CtlAlert.Mark mark)
        {
            Alerta.Show(sHeader, sMsg, mark);
        }

        /// <summary>
        /// Apresenta a mensagem e fecha a tela
        /// </summary>
        protected void AlertAndClose(string sMsg)
        {
            if (!ClientScript.IsClientScriptBlockRegistered("AlertAndClose"))
                ClientScript.RegisterClientScriptBlock(this.GetType(), "AlertAndClose", "<script>windo.alert('" + sMsg + "');self.close();</script>", true);
        }

        /// <summary>
        /// Fecha o popup 
        /// </summary>
        protected void Close()
        {
            if (!ClientScript.IsClientScriptBlockRegistered("Close"))
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "<script>self.close();</script>", true);
        }

        #endregion

        #region :: Métodos Estáticos ::

        /// <summary>
        /// Retorna o nome do mês
        /// </summary>
        /// <param name="mes">número do mês</param>
        /// <returns>O nome do mês</returns>
        public static string GetMonthFromOutSide(int mes)
        {
            return new PageBase().GetMonthName(mes);
        }

        /// <summary>
        /// Retorna o texto de acordo com a chave informada
        /// </summary>
        /// <param name="Key">Nome da chave</param>
        /// <returns>O texto solicitado</returns>
        public static string GetResourceValueFromOutSide(string Key)
        {
            return new PageBase().GetResourceValue(Key);
        }

        /// <summary>
        /// Retorna o texto de acordo com a chave informada
        /// </summary>
        /// <param name="Key">Chave do texto principal</param>
        /// <param name="ignoreKey">Informa se deve ser retornado um resource de erro padrão caso a chave não exista,
        /// ou se deve ser apresentado texto literal passado com parâmetro</param>
        /// <param name="KeyCollection">Chave dos textos que aparecerão nos indices do texto principal</param>
        /// <returns>O texto formatado</returns>
        public static string GetResourceValueFromOutSide(string Key, bool ignoreKey, params string[] KeyCollection)
        {
            return new PageBase().GetResourceValue(Key, ignoreKey, KeyCollection);
        }

        /// <summary>
        /// Mostra a mensagem de alerta
        /// </summary>
        /// <param name="sMsg">Mensagem</param>
        public static void AlertDefaultFromOutSide(string sMsg)
        {
            //new PageBase().AlertAjax(sMsg);
            new PageBase().AlertDefault(sMsg);
        }

        #endregion

        #region :: Resources ::

        /// <summary>
        /// Retorna o texto de acordo com a chave informada
        /// </summary>
        /// <param name="Key">Nome da chave</param>
        /// <returns>O texto solicitado</returns>
        protected string GetResourceValue(string Key)
        {
            string chave = string.Empty;

            try
            {
                chave = Key;

                return GetGlobalResourceObject("Resource", Key).ToString();
            }
            catch(Exception)
            {
                return string.Format(GetGlobalResourceObject("Resource", "ResourceError").ToString(), String.IsNullOrEmpty(chave) ? GetGlobalResourceObject("Resource", "ResourceErrorHelper").ToString() : chave);
            }
        }

        /// <summary>
        /// Retorna o texto de acordo com a chave informada
        /// </summary>
        /// <param name="Key">Chave do texto principal</param>
        /// <param name="ignoreKey">Informa se deve ser retornado um resource de erro padrão caso a chave não exista,
        /// ou se deve ser apresentado texto literal passado com parâmetro</param>
        /// <param name="KeyColleciotn">Chave dos textos que aparecerão nos indices do texto principal</param>
        /// <returns>O texto formatado</returns>
        protected string GetResourceValue(string Key, bool ignoreKey, params string[] KeyColleciotn)
        {
            string chave = string.Empty;

            try
            {
                string[] values = new string[KeyColleciotn.Length];

                for (int ind = 0; ind < KeyColleciotn.Length; ind++)
                {
                    chave = KeyColleciotn[ind];

                    if (ignoreKey)
                    {
                        try
                        {
                            values[ind] = String.IsNullOrEmpty(chave) ? string.Empty : GetGlobalResourceObject("Resource", KeyColleciotn[ind]).ToString();
                        }
                        catch
                        {
                            values[ind] = chave;
                        }
                    }
                    else
                        values[ind] = String.IsNullOrEmpty(chave) ? string.Empty : GetGlobalResourceObject("Resource", KeyColleciotn[ind]).ToString();
                }

                return string.Format(GetGlobalResourceObject("Resource", Key).ToString(), values);
            }
            catch 
            {
                return string.Format(GetGlobalResourceObject("Resource", "ResourceError").ToString(), String.IsNullOrEmpty(chave) ? GetGlobalResourceObject("Resource", "ResourceErrorHelper").ToString() : chave);
            }
        }

        #endregion

        #region :: Adiciona Scripts ::

        /// <summary>
        /// Bloqueia o retorno à pagina anterior quando pressinado BackSpace em um TextBox com ReadOnly true.
        /// </summary>
        /// <param name="oTxts">Controles TextBox</param>
        //protected void AddBlockBackSpaceScript(params TextBox[] oTxts)
        //{
        //    foreach (TextBox oTxt in oTxts)
        //        oTxt.Attributes.Add("onkeydown", "javascript:BlockBackSpace(this, event);");
        //}

        /// <summary>
        /// Adiciona script para que o componente aceite somente números
        /// </summary>
        /// <param name="oTxts">Coleção de caixas de texto</param>
        //protected void AddOnlyNumberScript(params TextBox[] oTxts)
        //{
        //    foreach (TextBox oTxt in oTxts)
        //        oTxt.Attributes.Add("onkeypress", "javascript:return OnlyNumber(event);");
        //}

        /// <summary>
        /// Adiciona o script para formatação monetária
        /// </summary>
        /// <param name="oTxt">TextBox</param>
        protected void AddCurrencyScript(params TextBox[] oTxt)
        {
            //foreach (TextBox txt in oTxt)
            //    txt.Attributes.Add("onkeypress", "javascript:GetSelectedText(this);return CurrencyFormat(this, event);");
                
            //txt.Attributes.Add("onkeypress", "javascript:return moeda(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de Cnpj
        /// </summary>
        /// <param name="oTxts">Coleção de caixas de texto</param>
        protected void AddCnpjFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return CNPJFormat(this, event);");
        }

        /// <summary>
        /// Adiciona script de fromatação de cpf
        /// </summary>
        /// <param name="oTxts">Conjunto de caixas de texto</param>
        protected void AddCpfFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return FormataCpf(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de data
        /// </summary>
        /// <param name="oTxts"></param>
        protected void AddDateFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
            {
                oTxt.Attributes.Add("onkeypress", "javascript:return DateMask(this, event);");
                oTxt.Attributes.Add("onblur", "javascript:fnValidaData(this, event);");
            }
        }

        /// <summary>
        /// Adiciona Script de Formatação de Hora
        /// </summary>
        /// <param name="oTxts"></param>
        protected void AddHourFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
            {
                oTxt.Attributes.Add("onkeypress", "javascript:return HourMask(this, event);");
                oTxt.Attributes.Add("onblur", "javascript:fnValidaHora(this, event);");
            }
        }

        /// <summary>
        /// Adiciona script de formatação de CEP
        /// </summary>
        /// <param name="oTxts"></param>
        protected void AddCepFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return FormataCEP(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de telefone
        /// </summary>
        /// <param name="oTxts">TextBox</param>
        protected void AddFoneFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return formatTelefone(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de telefone com DDD
        /// </summary>
        /// <param name="oTxts">TextBox</param>
        protected void AddFoneDDDFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return formatDDDTelefone(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de telefone com DDD
        /// </summary>
        /// <param name="oTxts">TextBox</param>
        protected void AddCelularDDDFormatOncoScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return formatDDDCelularNovo(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de telefone celular com DDD
        /// </summary>
        /// <param name="oTxts">TextBox</param>
        protected void AddCelularDDDFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return formatDDDCelular(this, event);");
        }

        /// <summary>
        /// Adiciona script de formatação de placa de veículo
        /// </summary>
        /// <param name="oTxts">TextBox</param>
        protected void AddPlacaFormatScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return fnFrmPlaca(this, event);");
        }

        /// <summary>
        /// Torna a primeira letra sempre maiúscula
        /// </summary>
        /// <param name="oTxts"></param>
        protected void AddFirstLetterUpperScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return FirstLetter(this, event);");
        }

        /// <summary>
        /// Verifica o tamanho máximo de caracteres
        /// </summary>
        /// <param name="oTxts"></param>
        protected void AddCheckMaxlengthScript(params TextBox[] oTxts)
        {
            foreach (TextBox oTxt in oTxts)
                oTxt.Attributes.Add("onkeypress", "javascript:return CheckMaxLength(this," + oTxt.MaxLength + ");");
        }

        /// <summary>
        /// Adiciona script para a seleção de todos os checkboxes no CheckBoxList
        /// </summary>
        /// <param name="oChks">Conjunto de CheckBoxes.
        /// Passagem dos Parâmetros: new Control[,] { { CheckBox, CheckBoxList } }
        /// </param>
        protected void AddCheckUnCheckAllScript(Control[,] oChks)
        {
            try
            {
                for (int iOut = 0; iOut < oChks.GetLength(0); iOut++)
                    if (oChks[iOut, 0] is CheckBox &&
                            oChks[iOut, 1] is CheckBoxList)
                    {
                        ((CheckBox)oChks[iOut, 0]).Attributes.Add("onclick", "javascript:CheckBoxListCheckAll('" +
                                                                              ((CheckBox)oChks[iOut, 0]).ClientID + "', '" +
                                                                              ((CheckBoxList)oChks[iOut, 1]).ClientID + "');");

                        ((CheckBoxList)oChks[iOut, 1]).Attributes.Add("onclick", "javascript:CheckBoxListUnCheckFather('" +
                                                                                 ((CheckBox)oChks[iOut, 0]).ClientID + "','" +
                                                                                 ((CheckBoxList)oChks[iOut, 1]).ClientID + "');");
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona o script de marcação e desmarcação do checkbox no header da grid. 
        /// </summary>
        /// <param name="oCtls">Controles CheckBox e GridView.
        /// Passagem dos Parâmetros: new Control[,] { CheckBoxItem, CheckBoxHeader, GridView }
        /// </param>
        /// <param name="coluna">Coluna onde estão os controles Checkbox</param>
        protected void AddCheckUnCheckAllInGrid(Control[,] oCtls, int coluna)
        {
            try
            {
                for (int iOut = 0; iOut < oCtls.GetLength(0); iOut++)
                    if (oCtls[iOut, 0] is CheckBox &&
                            oCtls[iOut, 1] is CheckBox &&
                                oCtls[iOut, 2] is GridView)
                    {
                        ((CheckBox)oCtls[iOut, 1]).Attributes.Clear();
                        ((CheckBox)oCtls[iOut, 1]).Attributes.Add("onclick",
                                                                  "javascript:SelectAll('" + ((CheckBox)oCtls[iOut, 1]).ClientID + "', '" +
                                                                                             ((GridView)oCtls[iOut, 2]).ClientID + "', " + coluna + ");");

                        ((CheckBox)oCtls[iOut, 0]).Attributes.Add("onclick",
                                                                  "javascript:CheckUncheckFather('" + ((CheckBox)oCtls[iOut, 1]).ClientID + "', '" +
                                                                                                      ((GridView)oCtls[iOut, 2]).ClientID + "', " + coluna + ");");
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// O painel é iniciado como invisivél
        /// </summary>
        /// <param name="oChk">CheckBox Ativador</param>
        /// <param name="oPnl">Painel</param>
        protected void AddShowHidePanelScript(CheckBox oChk, Panel oPnl)
        {
            try
            {
                oPnl.Attributes.Add("style", "visibility:hidden");

                oChk.Attributes.Add("onclick", "javascript:ShowHidePanel('" + oChk.ClientID + "', '" +
                                                                              oPnl.ClientID + "');");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona o Script para pular para o próximo componente da página
        /// </summary>
        /// <param name="sFormName">Nome do formulário em questão: this.Form.ClientID</param>
        /// <param name="oCtls">Caixas de Textos</param>
        protected void AddJumpNextScript(string sFormName, params TextBox[] oCtls)
        {
            try
            {
                foreach (TextBox oTxt in oCtls)
                    ((TextBox)oTxt).Attributes.Add("onkeyup", "javascript:JumpNext('" + ((TextBox)oTxt).MaxLength.ToString() + "', '" +
                                                                                        ((TextBox)oTxt).ClientID + "', '" +
                                                                                        ((TextBox)oTxt).TabIndex + "', '" +
                                                                                        sFormName + "');");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona o Script de Seleção de Opção a Todos os DropDownLists da Grid
        /// </summary>
        /// <param name="oDDl">DropDownList Header</param>
        /// <param name="oGv">GridView</param>
        /// <param name="iColumn">Coluna onde estão Localizados os Controles DropDownLists no GridView</param>
        protected void AddSetSelectedItemForAllDropDownListsInGridScript(DropDownList oDDl, GridView oGv, int iColumn)
        {
            try
            {
                oDDl.Attributes.Clear();
                oDDl.Attributes.Add("onChange", "javascript:SetScriptsForAllDropDownListsInGrid('" + oDDl.ClientID + "', '" +
                                                                                                     oGv.ClientID + "', '" +
                                                                                                     iColumn + "');");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona o script para Mostrar/Esconder painel
        /// </summary>
        /// <param name="oCtl">Controle</param>
        /// <param name="oPnl">Painel</param>
        /// <param name="value">Valor</param>
        protected void AddShowHidePanelScript(Control oCtl, Panel oPnl, string value)
        {
            try
            {
                if (oCtl is DropDownList)
                    ((DropDownList)oCtl).Attributes.Add("onChange", "javascript:ShowHidePanel('" + oCtl.ClientID + "', '" +
                                                                                                   oPnl.ClientID + "', '" + 
                                                                                                   value + "');");
                else if (oCtl is CheckBox)
                    ((CheckBox)oCtl).Attributes.Add("onclick", "javascript:ShowHidePanel('" + oCtl.ClientID + "', '" +
                                                                                              oPnl.ClientID + "', '" + 
                                                                                              value + "');");
                else if (oCtl is RadioButtonList)
                    ((RadioButtonList)oCtl).Attributes.Add("onclick", "javascript:ShowHidePanel('" + oCtl.ClientID + "', '" +
                                                                                                     oPnl.ClientID + "', '" + 
                                                                                                     value + "');");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Adiciona o script para a verificação do valor do dropdownlist
        /// </summary>
        /// <param name="oBtn">Botão que chamará a função para validar os DropDownList's</param>
        /// <param name="oDDl">
        /// Os controles DDL e suas mensagens de erro.
        /// Passagem do parâmetro:         /// 
        /// Quando somente 01: new object[,] { { string, DropDownList } }
        /// - Posição 0: string = mensagem de erro
        /// - Posição 1: DropDownList = Componente a ser tratado
        /// Quando 02 ou mais: new object[,] { { string, DropDownList }, { string, DropDownList },... }
        /// </param>
        protected void AddVerifyDropScript(Control oBtn, object[,] oDDl)
        {
            try
            {
                string[] sMsg = new string[oDDl.GetLength(0)];
                string[] sDrop = new string[oDDl.GetLength(0)];

                for (int iOut = 0; iOut < oDDl.GetLength(0); iOut++)
                {
                    if (oDDl[iOut, 0] is string &&
                            oDDl[iOut, 1] is DropDownList)
                    {
                        sMsg[iOut] = oDDl[iOut, 0].ToString();
                        sDrop[iOut] = ((DropDownList)oDDl[iOut, 1]).ClientID;
                    }
                }

                if (oBtn is Button)
                    ((Button)oBtn).Attributes.Add("onclick", "javascript:ValidaDrop('" + string.Join("|", sMsg) + "','" +
                                                                               string.Join("|", sDrop) + "' );");
                else if (oBtn is ImageButton)
                    ((ImageButton)oBtn).Attributes.Add("onclick", "javascript:ValidaDrop('" + string.Join("|", sMsg) + "','" +
                                                                           string.Join("|", sDrop) + "' );");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona script para verificar se nenhum check foi selecionado
        /// </summary>
        /// <param name="oBtn">Botão de controle</param>
        /// <param name="oChkLst">CheckBoxList</param>
        protected void AddVerifyIfCheckScript(Button oBtn, CheckBoxList oChkLst)
        {
            try
            {
                oBtn.Attributes.Add("onclick", "javascript:GetCheckBoxListValues('" + oChkLst.ClientID + "', '" + GetResourceValue("msgNenhumRegistroSelecionado") + "');");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Verifica se existem checks selecionados no grid
        /// </summary>
        /// <param name="oBtn">Botão</param>
        /// <param name="oGv">GridView</param>
        /// <param name="coluna">Número da coluna onde estão os CheckBoxes</param>
        /// <param name="msg">Mensagem a ser apresentada</param>
        protected void AddVerifyIfCheckedInGridScript(Control oBtn, GridView oGv, int coluna, string msg)
        {
            try
            {
                if (oBtn is Button)
                {
                    ((Button)oBtn).Attributes.Add("onclick", "javascript:VerifyIfCheckInGrid('" + oGv.ClientID + "', " + 
                                                                                                  coluna + ", '" + 
                                                                                                  msg + "');");
                }
                else if (oBtn is ImageButton)
                {
                    ((ImageButton)oBtn).Attributes.Add("onclick", "javascript:VerifyIfCheckInGrid('" + oGv.ClientID + "', " +
                                                                                                       coluna + ", '" +
                                                                                                       msg + "');");
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Informa os controles que serão utilizados para a recuperação de parâmetros extras para o 
        /// componente AutoCompleteExtender.
        /// Os valores dos componentes serão separados pelo caracter | na ordem em que forem passados como parâmetro.
        /// </summary>
        /// <param name="ace">AutoCompleteExtender</param>
        /// <param name="oTxt">TextBox</param>
        /// <param name="oCtls">Controles que serão utilizados como parâmetros</param>
        protected void AddSetAutoCompleteNewParameterScript(ajax.AutoCompleteExtender ace, TextBox oTxt, params Control[] oCtls)
        {
            try
            {
                string[] _clienteId = new string[oCtls.Length];
                int _indice = 0;

                foreach (Control oCtl in oCtls)
                {
                    if (oCtl is TextBox)
                        _clienteId[_indice] = ((TextBox)oCtl).ClientID;
                    else if (oCtl is DropDownList)
                        _clienteId[_indice] = ((DropDownList)oCtl).ClientID;
                    else if (oCtl is CheckBox)
                        _clienteId[_indice] = ((CheckBox)oCtl).ClientID;

                    _indice++;
                }

          
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex);
                throw ex;
            }
        }

        #endregion

        #region :: Encrypt/Decrypt ::

        /// <summary>
        /// Criptografa uma valor
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public string Encrypt(string sValue)
        {
            return Server.UrlEncode(Cryptography.Encrypt(sValue, "KsOnco", Cryptography.AESCryptographyLevel.AES_256));
        }

        /// <summary>
        /// Descriptografa um valor 
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public string Decrypt(string sValue)
        {
            return Server.UrlDecode(Cryptography.Decrypt(sValue, "KsOnco", Cryptography.AESCryptographyLevel.AES_256));
        }

        #endregion

        #endregion
    }
}