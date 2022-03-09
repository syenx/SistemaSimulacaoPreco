<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CtlOption.ascx.cs" Inherits="KS.SimuladorPrecos.AppControles.CtlOption" %>
<asp:UpdatePanel ID="upAlert" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlOption" runat="server" DefaultButton="btnNo" Visible="false">
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
                            ImageUrl="~/Imagens/Fechar.png" />
                    </div>
                </div>
                <div class="alert_body">
                    <div style="width: 100%; height: 100%; display: table; margin-top: 0px;">
                        <div style="width: 80%; height: 100%; display: table-cell; vertical-align: middle;">
                            <asp:Label ID="lblMessage" runat="server" />
                        </div>
                        <div id="dvAlertIcon" runat="server" style="width: 20%; height: 100%; display: table-cell; vertical-align: middle; text-align: center;">
                            <asp:Image ID="imgAlert" runat="server" Height="50%" Width="60%" ImageAlign="AbsMiddle" ImageUrl="~/Imagens/question.png" />
                        </div>
                    </div>
                </div>
                <div class="alert_bottom">
                    <div class="alert_bottom_button">
                        <div class="alert_bottom_button_left">
                            <asp:Button ID="btnYes" runat="server" AccessKey="S" SkinID="ButtonLeft" Text="<%$ Resources:Resource, lblSim %>" onclick="btnYes_Click"  />
                        </div>
                        <div class="alert_bottom_button_right">
                            <asp:Button ID="btnNo" runat="server" AccessKey="N" SkinID="ButtonRight" Text="<%$ Resources:Resource, lblNao %>" OnClick="btnNo_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
