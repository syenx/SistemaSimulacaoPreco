using System;
using KS.SimuladorPrecos.AppBaseInfo;

namespace KS.SimuladorPrecos.AppPaginas.Home
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Home : PageBase
    {
        #region :: Eventos ::

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/AppPaginas/Consulta/SimuladorPreco.aspx", true);


        }

        #endregion

        protected void btnLogin_Click(object sender, EventArgs e)
        {

        }
    }
}