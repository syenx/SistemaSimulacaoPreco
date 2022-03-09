<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="LoginAbcPharma.aspx.cs" Inherits="KS.SimuladorPrecos.AbcPharmaPages.LoginAbcPharma" %>

<%@ Register Src="~/AppControles/CtlAlert.ascx" TagPrefix="CtlAlert" TagName="ControlAlert" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">


    <title></title>
    <link rel="shortcut icon" href="../Imagens/logo-oncosales-topo1.png" />
    <link href="Styles/Style.css" rel="Stylesheet" type="text/css" />
 
       <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.css">
    <link href="../../Styles/bootstrap.min.css" rel="stylesheet" />
    <link type="text/css" rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <link href="../../css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js"></script>
    <script type="text/javascript" src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>


     <link href="../../Styles/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript"  src="../../Scripts/jquery-3.3.1.slim.min.js"></script>
    <script type="text/javascript" src="../../Scripts/popper.min.js"></script>
    <script type="text/javascript"  src="../../Scripts/bootstrap.min.js"></script>
    <link href="../../Styles/bootstrap.css" rel="stylesheet" />
    <%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ucc" %>
    <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scmngr" runat="server" AllowCustomErrorsRedirect="true" AsyncPostBackTimeout="600"
            ScriptMode="Debug" EnableScriptGlobalization="true" EnableScriptLocalization="true" />

        <%-- Controle ////Alert --%>
        <CtlAlert:ControlAlert ID="CtlAlert" runat="server" />

        <%-- PROGRESS BACKGROUND --%>
        <asp:UpdatePanel ID="upLoadLogin" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <asp:UpdateProgress ID="upLoading" runat="server" AssociatedUpdatePanelID="upLoadLogin">
                    <ProgressTemplate>
                        <div class="ProgressBackGround">
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>

                <div class="modal show" style="background-color: #ECF0F3">
                    <div class="modal-dialog-centered " style=" margin:0% 33% 20% 33%  ">
                        <div class="modal-content" >
                            <div class="modal-body" >
                                <div class="form-group text-center">
                                    <img src="../Imagens/Oncoprod_monocromatica.png"  class="img-responsive " />
                                </div>
                                <div class="modal-header" style="border: 0 none; margin-bottom:0;padding:0">
                                    <h2 class="text-center " style="margin-bottom:0""  >ABC Pharma</h2>

                                </div>
                                <div class="modal-header" style="border: 0 none;  padding:0">
                                    <h4 class="text-center  "  >Por favor, digite o seu usuário e senha.</h4>
                                </div>

                                <div class="form-group" style="margin: 5px 30px 10px 30px; height: 40px;">

                                    <asp:TextBox ID="txtLogin" CssClass="input-lg" placeholder="Usuário"  SkinID="InputBootstrap" runat="server" />
                                </div>
                                <div class="form-group" style="margin: 10px 30px 10px 30px; height: 40px;">

                                    <asp:TextBox ID="txtPassword" placeholder="Senha" runat="server" CssClass="input-lg"  SkinID="InputBootstrap" TextMode="Password" />
                                </div>

                                <div class="form-group center-block" style="margin: 5px 30px 10px 30px; height: 40px;"">
                                  
                                        <%--<input type="submit" value="Login" class="btn btn-primary btn-lg btn-block" />--%>
                                       <asp:Button ID="btnLogin"   Height="100%" Width="100%" runat="server" ValidationGroup="Validacao" SkinID="ButtonBootStrap" OnClick="btnLogin_Click" />
                                    </div>

         
                        </div>
                    </div>

                                     <!-- Bootstrap Modal Dialog -->
            <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="modal-content">
                                <div class="modal-header" style="background-color: #D9DEE4">

                                    <h4 class="modal-title">

                                        <%--<asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label>--%></h4>
                                    <img src="../../Imagens/Oncoprod_monocromática.png" style="height: 30px" class="img-responsive" />
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>

                                </div>
                                <div class="modal-body">
                                    <asp:Label ID="lblModalBody" Font-Size="Medium" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="modal-footer">
                                    <button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Fechar</button>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
                          <script type="text/javascript">

                              var input = document.getElementById('txtLogin');

                              input.onkeyup = function () {
                                  this.value = this.value.toUpperCase();
                              }


    </script>
                    <%-- BODY --%>
                    <asp:Panel ID="pnlLogin" runat="server">
                        <div class="login" style="display: none">
                            <div class="login_In">
                                <div class="Inner_Left">
                                    <asp:Image ID="imgBody" runat="server" Width="400px" Height="200px" />
                                </div>
                                <div class="Inner_Center">
                                    <div class="login_line">
                                        <div class="login_label">
                                        </div>
                                        <div class="login_text">
                                        </div>
                                    </div>
                                    <div class="login_line">
                                        <div class="login_label">
                                        </div>
                                        <div class="login_text">
                                        </div>
                                    </div>
                                    <div class="login_line">
                                       
                                    </div>
                                    <div class="login_line" style="display: none;">
                                        <asp:LinkButton ID="lbEsqueciSenha" runat="server" SkinID="LinkButtonLogin" />
                                        <b><font color="13213C">| </font></b>
                                        <asp:LinkButton ID="lbNaoCadastrado" runat="server" SkinID="LinkButtonLogin" PostBackUrl="~/AppPaginas/Cadastros/CadUsuario.aspx" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="txtLogin" Enabled="false"
                            Display="None" SetFocusOnError="true" Text="*" ValidationGroup="Validacao" />
                        <asp:RequiredFieldValidator ID="rfvSenha" runat="server" ControlToValidate="txtPassword" Enabled="false"
                            Display="None" SetFocusOnError="true"  Text="*" ValidationGroup="Validacao" />
                        <asp:ValidationSummary ID="vsValidacao" runat="server" ShowMessageBox="true" ShowSummary="false"
                            ValidationGroup="Validacao" />
                    </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <%-- HEADER --%>
        <div class="header" style="display: none">
            <div class="Inner_header">
                <div class="Inner_Left">
                    <asp:Image ID="imgHeader" runat="server" ImageAlign="AbsMiddle" Width="180px" Height="50px" />
                </div>
                <div class="Inner_Center">
                    <asp:Label ID="lblHeader" runat="server" ForeColor="White" />
                </div>
                <div class="Inner_Right">
                </div>
            </div>
        </div>


        <%-- FOOTER --%>
        <div class="footer" style="display: none">
            <div class="Inner_footer">
                <div class="Inner_Left">
                    <asp:Label ID="lblFooterVersion" runat="server" Font-Bold="true" ForeColor="White"
                        Visible="true" />
                </div>
                <div class="Inner_Right">
                    <asp:Label ID="lblFooterCopiryght" runat="server" ForeColor="White" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>

