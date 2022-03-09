using System;
//using KRAFTSALES.DATAENTITIES.Entidades;

namespace KS.SimuladorPrecos.AppBaseInfo
{
    /// <summary>
    /// Estrutura Responsável por Armazenar os Dados do Usuário Logado
    /// </summary>
    public class UserDataInfo
    {
        /// <summary>
        /// Mantém dados do Usuário logado
        /// </summary>
        public string UserContent { get; set; }

        /// <summary>
        /// Retorna o ID do Usuário Logado
        /// </summary>
        public string UserID
        {
            get { return (UserContent.Split(';'))[0].ToString(); }
        }

        /// <summary>
        /// Retorna o nome de login do usuário autênticado
        /// </summary>
        public string UserLogin
        {
            get { return String.IsNullOrEmpty(UserContent) ? PageBase.GetResourceValueFromOutSide("lblUsuario") : (UserContent.Split(';'))[1].ToString(); }
        }

        /// <summary>
        /// Retorna o nome do usuário autênticado
        /// </summary>
        public string UserName
        {
            get { return String.IsNullOrEmpty(UserContent) ? PageBase.GetResourceValueFromOutSide("lblUsuario") : (UserContent.Split(';'))[4].ToString(); }
        }

        /// <summary>
        /// Nome completo do usuário logado
        /// </summary>
        public string UserNameComplete
        {
            get { return String.IsNullOrEmpty(UserContent) ? PageBase.GetResourceValueFromOutSide("lblUsuario") : (UserContent.Split(';'))[11].ToString(); }
        }

        /// <summary>
        /// Verifica se é o primeiro acesso do usuário ao sistema
        /// </summary>
        public bool UserIsFisrtAccess
        {
            get { return String.IsNullOrEmpty(UserContent) ? true : String.IsNullOrEmpty((UserContent.Split(';'))[3].ToString()) ? true : false; }
        }

        /// <summary>
        /// Informa o gênero(sexo) do usuário logado
        /// </summary>
        public PageBase.Genero UserGenero
        {
            get { return String.IsNullOrEmpty(UserContent) ? PageBase.Genero.M : (UserContent.Split(';'))[2].ToString().Equals((PageBase.Genero.F).ToString()) ? PageBase.Genero.F : PageBase.Genero.M; }
        }

        /// <summary>
        /// Data do último acesso realizado pelo usuário
        /// </summary>
        public DateTime? UserUltimoAcesso
        {
            get { return String.IsNullOrEmpty(UserContent) ? new Nullable<DateTime>() : String.IsNullOrEmpty(UserContent.Split(';')[3].ToString()) ? new Nullable<DateTime>() : DateTime.Parse(UserContent.Split(';')[3].ToString()); }
        }

        /// <summary>
        /// Retorna o tipo do usuário logado
        /// </summary>
        public PageBase.TipoUsuario TipoUsuario
        {
            get
            {
                if (String.IsNullOrEmpty(UserContent))
                    return PageBase.TipoUsuario.ATD;
                else if (String.IsNullOrEmpty(UserContent.Split(';')[5]))
                    return PageBase.TipoUsuario.ATD;
                else
                {
                    switch (UserContent.Split(';')[5])
                    {
                        case "ADM":
                            return PageBase.TipoUsuario.ADM;

                        case "GER":
                            return PageBase.TipoUsuario.GER;

                        case "SUP":
                            return PageBase.TipoUsuario.SUP;

                        case "REP":
                            return PageBase.TipoUsuario.REP;

                        case "ATD":
                            return PageBase.TipoUsuario.ATD;

                        default:
                            return PageBase.TipoUsuario.ATD;
                    }
                }
            }
        }

        /// <summary>
        /// Informa o perfil de acesso do usuário logado
        /// </summary>
        public string UserPerfilAcessoNome
        {
            get { return String.IsNullOrEmpty(UserContent) ? PageBase.GetResourceValueFromOutSide("lblPerfilNaoRelacionado") : (UserContent.Split(';'))[6].ToString(); }
        }

        /// <summary>
        /// Perfil do usuário logado
        /// </summary>
        public PageBase.TipoPerfil TipoPerfil
        {
            get
            {
                switch (UserPerfilAcessoNome)
                {
                    case "BCC":
                        return PageBase.TipoPerfil.BCC;
                    case "BF":
                        return PageBase.TipoPerfil.BF;
                    case "BR":
                        return PageBase.TipoPerfil.BR;
                    case "C":
                        return PageBase.TipoPerfil.C;
                    case "CP":
                        return PageBase.TipoPerfil.CP;
                    case "P":
                        return PageBase.TipoPerfil.P;
                    case "T":
                        return PageBase.TipoPerfil.T;
                    case "BFPJ":
                        return PageBase.TipoPerfil.BFPJ;
                    default:
                        return PageBase.TipoPerfil.C;
                }
            }
        }

        /// <summary>
        /// Número de casas decimais a se utilizar na aplicação
        /// </summary>
        public int UserCasasDecimais
        {
            get { return 2; }
        }

        /// <summary>
        /// Verifica a permissão de manutenção de pedidos
        /// </summary>
        public bool IsUsuarioAutorizado
        {
            get
            {
                switch (TipoUsuario)
                {
                    case PageBase.TipoUsuario.ADM:
                    case PageBase.TipoUsuario.GER:
                    case PageBase.TipoUsuario.SUP:                    
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Informa se o usuário logado é administrador
        /// </summary>
        public bool IsAdm
        {
            get
            {
                switch (TipoUsuario)
                {
                    case PageBase.TipoUsuario.ADM:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Informa o código da unidade de negócio do usuário
        /// </summary>
        public string UserUnidadeNegocio
        {
            get
            {
                if (IsAdm)
                    return !String.IsNullOrEmpty(PageBase.PerfilPedido) ? PageBase.PerfilPedido : String.IsNullOrEmpty(UserContent) ? string.Empty : (UserContent.Split(';'))[7];
                else
                    return String.IsNullOrEmpty(UserContent) ? string.Empty : (UserContent.Split(';'))[7];
            }
        }

        /// <summary>
        /// Tipo da unidade de negócio do usuário logado
        /// </summary>
        public PageBase.TipoUnidadeNegocio TipoOperacao
        {
            get
            {
                switch (UserUnidadeNegocio.ToUpper())
                {
                    case "PJP":
                        return PageBase.TipoUnidadeNegocio.PJPublico;

                    case "COMPRAS":
                    case "PJD":
                        return PageBase.TipoUnidadeNegocio.PJPrivado;

                    case "ECM":
                        return PageBase.TipoUnidadeNegocio.eCommerce;

                    default:
                        return PageBase.TipoUnidadeNegocio.PF;
                }
            }
        }

        /// <summary>
        /// Informa se faz parte do ADM Vendas
        /// </summary>
        public bool IsAdmVendas
        {
            get { return String.IsNullOrEmpty(UserContent) ? false : String.IsNullOrEmpty((UserContent.Split(';'))[8].ToString()) ? false : bool.Parse((UserContent.Split(';'))[8].ToString()) ? true : false; }
        }

        /// <summary>
        /// Retorna o tipo de unidade de negócios
        /// </summary>
        /// <param name="unidadeNegocioId">Id da unidade de negócios</param>
        /// <returns>PageBase.TipoUnidadeNegocio</returns>
        public PageBase.TipoUnidadeNegocio GetUnidadeNegocio(string unidadeNegocioId)
        {
            switch (unidadeNegocioId.ToUpper())
            {
                case "PJP":
                    return PageBase.TipoUnidadeNegocio.PJPublico;

                case "COMPRAS":
                case "PJD":
                    return PageBase.TipoUnidadeNegocio.PJPrivado;

                default:
                    return PageBase.TipoUnidadeNegocio.PF;
            }
        }

        /// <summary>
        /// Tipo de visualização ao simulador
        /// </summary>
        public PageBase.VisualizacaoTipo VisualizacaoTipo
        {
            get
            {
                if (String.IsNullOrEmpty(UserContent))
                    return PageBase.VisualizacaoTipo.Completa;
                else if ((UserContent.Split(';')[9]).ToString().Trim().Equals("CP"))
                    return PageBase.VisualizacaoTipo.Completa;
                else
                    return PageBase.VisualizacaoTipo.Resumida;
            }
        }

        /// <summary>
        /// Tipo de visualização aos quadros do simulador
        /// </summary>
        public PageBase.VisualizacaoQuadro VisualizacaoQuadro
        {
            get
            {
                if (String.IsNullOrEmpty(UserContent))
                    return PageBase.VisualizacaoQuadro.Sumarizada;
                else if ((UserContent.Split(';')[10]).ToString().Trim().Equals("SM"))
                    return PageBase.VisualizacaoQuadro.Sumarizada;
                else
                    return PageBase.VisualizacaoQuadro.Resumida;
            }
        }
    }
}