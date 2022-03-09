<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="KS.SimuladorPrecos.Login" %>

<%@ Register Src="~/AppControles/CtlAlert.ascx" TagPrefix="CtlAlert" TagName="ControlAlert" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="shortcut icon" href="Imagens/Icone.ico" />
    <link href="Styles/Style.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scmngr" runat="server" AllowCustomErrorsRedirect="true" AsyncPostBackTimeout="600"
        ScriptMode="Debug" EnableScriptGlobalization="true" EnableScriptLocalization="true" />

    <%-- Controle Alert --%>
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

            <%-- BODY --%>
            <asp:Panel ID="pnlLogin" runat="server">
                <div class="login">
                    <div class="login_In">
                        <div class="Inner_Left">
                            <asp:Image ID="imgBody" runat="server" Width="400px" Height="200px" />
                        </div>
                        <div class="Inner_Center">
                            <div class="login_line">
                                <div class="login_label">
                                    <asp:Label ID="lblLogin" runat="server" />
                                </div>
                                <div class="login_text">
                                    <asp:TextBox ID="txtLogin" runat="server" SkinID="Filtro" />
                                </div>
                            </div>
                            <div class="login_line">
                                <div class="login_label">
                                    <asp:Label ID="lblPassword" runat="server" />
                                </div>
                                <div class="login_text">
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" SkinID="Filtro" />
                                </div>
                            </div>
                            <div class="login_line">
                                <asp:Button ID="btnLogin" runat="server" ValidationGroup="Validacao" OnClick="btnLogin_Click" />
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
                    Display="None" SetFocusOnError="true" Text="*" ValidationGroup="Validacao" />
                <asp:ValidationSummary ID="vsValidacao" runat="server" ShowMessageBox="true" ShowSummary="false"
                    ValidationGroup="Validacao" />
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- HEADER --%>
    <div class="header">
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
    <div class="footer">
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
