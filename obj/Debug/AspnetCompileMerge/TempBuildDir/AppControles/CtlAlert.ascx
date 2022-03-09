<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CtlAlert.ascx.cs" Inherits="KS.SimuladorPrecos.AppControles.CtlAlert" %>
<asp:UpdatePanel ID="upAlert" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlAlert" runat="server" DefaultButton="btnOk" Visible="false">
            <div class="OpacityBackGround" style="z-index: 7;">
            </div>
            <div class="alert">
                <div class="alert_header">
                    <div class="alert_header_message">
                        <asp:Label ID="lblHeaderText" runat="server" ForeColor="White" />
                    </div>
                    <div class="alert_header_icon">
                        <asp:ImageButton ID="ibClose" runat="server" ImageAlign="AbsMiddle" Visible="false"
                            Width="30px" Height="30px" 
                            ImageUrl="~/Imagens/Fechar.png"
                            onclick="ibClose_Click" />
                    </div>
                </div>
                <div class="alert_body">
                    <div style="width: 100%; height: 100%; display: table; margin-top: 0px;">
                        <div style="width: 80%; height: 100%; display: table-cell; vertical-align: middle;">
                            <asp:Label ID="lblMessage" runat="server" />
                        </div>
                        <div id="dvAlertIcon" runat="server" style="width: 20%; height: 100%; display: table-cell; vertical-align: middle; text-align: center;">
                            <asp:Image ID="imgAlert" runat="server" Height="50%" Width="60%" ImageAlign="AbsMiddle" ImageUrl="~/Imagens/_error.png" />
                        </div>
                    </div>
                </div>
                <div class="alert_bottom">
                    <div class="alert_bottom_button">
                        <asp:Button ID="btnOk" runat="server" AccessKey="O" Text="<%$ Resources:Resource, lblOk %>" onclick="btnOk_Click"  />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
