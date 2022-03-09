<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster/ABCPharma.Master" AutoEventWireup="true" CodeBehind="ABCPharmaDownloadPlanilhas.aspx.cs" Inherits="KS.SimuladorPrecos.AbcPharmaPages.ABCPharmaDownloadPlanilhas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ucc" %>
    <link href="../../Styles/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="../../Scripts/jquery-3.3.1.slim.min.js"></script>
    <script type="text/javascript" src="../../Scripts/popper.min.js"></script>
    <script type="text/javascript" src="../../Scripts/bootstrap.min.js"></script>
    <link href="../../Styles/bootstrap.css" rel="stylesheet" />

 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #D9DEE4">

                            <h4 class="modal-title">

                                <%--<asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label>--%></h4>
                            <img src="../../Imagens/Oncoprod_monocromatica.png" style="height: 30px" class="img-responsive" />
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

    



    <div class="card">
        <div class="card-header">
           BAIXAR AS METAS
        </div>
        <div class="card-body">
         
            <p style="margin:10px 10px 10px 10px" class="card-text">Funcionalidade para baixar as metas</p>
            <asp:Button ID="btnExcel" style="margin:10px 10px 10px 10px" OnClick="btnExcel_Click" Text="Baixar" SkinID="ButtonBootStrap" runat="server" />

        </div>
    </div>


</asp:Content>
