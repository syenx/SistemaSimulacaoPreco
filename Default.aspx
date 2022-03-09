<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KS.SimuladorPrecos.Default" %>


<%@ Register Src="~/AppControles/CtlAlert.ascx" TagPrefix="CtlAlert" TagName="ControlAlert" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scmngr" runat="server" AllowCustomErrorsRedirect="true" AsyncPostBackTimeout="600"
            ScriptMode="Debug" EnableScriptGlobalization="true" EnableScriptLocalization="true" />
        <CtlAlert:ControlAlert ID="CtlAlert" runat="server" />
        <%-- PROGRESS BACKGROUND --%>
        <asp:UpdatePanel ID="upLoadLogin" runat="server" UpdateMode="Conditional">
            <Triggers>
                <%--<asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />--%>
            </Triggers>
            <ContentTemplate>
                <asp:UpdateProgress ID="upLoading" runat="server" AssociatedUpdatePanelID="upLoadLogin">
                    <ProgressTemplate>
                        <div class="ProgressBackGround">
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <div class="header" style="position: relative; z-index: 1; margin: 0 0 0 0">
                    <div class="Inner_header">
                        <div class="Inner_Left">
                  <img src="../../Imagens/Oncoprod_monocromatica.png" style="height: 30px" class="img-responsive" />
                                         </div>
                        <div class="Inner_Center">
                            <label class="control-label" style="font-size: 2vw; color: #485d74; margin: 0 0 0 20%">Ferramentas Oncoprod</label>
                        </div>
                        <div class="Inner_Right">
                            <div style="position: static; width: 380px; text-align: right;">
                                <asp:Label ID="lblNomeUsuario" runat="server" Style="font-size: 1vw;" Font-Bold="true" />
                                <asp:LinkButton ID="lbLogOnLogOff" Style="font-size: 1vw;" runat="server" ForeColor="Black" data-target="#exampleModal" data-toggle="exampleModal" Font-Bold="true" />
                            </div>
                            <div style="position: static; width: 380px; text-align: right; display: none">
                                <asp:Label ID="lblAcesso" runat="server" ForeColor="White" Font-Bold="true" Font-Size="Smaller" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="container" style="margin: 3% 10% 0 10%; width: 100%; align-content: center">
                    <h3></h3>
                    <ul class="nav nav-tabs">
                        <li id="prod" class="active"><a href="#">PRODUÇÃO</a></li>
                        <li id="homolog" ><a href="#" onclick="OcultarProducao">HOMOLOGAÇÃO</a></li>
                    </ul>
                    <br>
                    <div id="backGroud">
                        <div id="divProd" style="margin: 10px 35px 20px 35px;">
                            <h3>PRODUÇÃO</h3>
                            <div class="row">
                                <div class="col-sm-4 ">
                                    <div class="card">

                                        <a target="_blank" rel="noopener noreferrer" href="http://simulador.oncoprod.com.br:8080/SimuladordePrecos/Login.aspx">
                                            <div class="card-header ">
                                                <label>Simulador de Preços</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/logo-oncosales-topo1.png" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px">
                                                    Simulação de Preços com produtos Oncoprod
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="AbcPharmaPages/LoginAbcPharma.aspx">
                                            <div class="card-header ">
                                                <label>ABC Farma</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px">
                                                    Importação de tabela de preços ABC Farma
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://portaltabpreco.oncoprod.com.br:8086/">
                                            <div class="card-header ">
                                                <label>Tabela de Preços</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                  <label style="margin:0 2px 2px 2px">
                                                    Tabela para consulta de preços Oncoprod
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>

                            </div>
                            <div class="row" style="padding-top: 20px">
                                <div class="col-sm-4 ">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://portalvendasksales.oncoprod.com.br/KraftSales/Login.aspx">
                                            <div class="card-header ">
                                                <label>Kraft</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px">
                                                     Sistema de Vendas OncoSales Kraft
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://onconect.oncoprod.com.br:8082/">
                                            <div class="card-header ">
                                                <label>OnConect</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Onconect2.png" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px">
                                                    Sistema de apoio OncoSales Kraft
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="https://portalacessar.stcruz.com.br/acessar/login">
                                            <div class="card-header ">
                                                <label>Portal Acessar</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/logo-acessar_web.png" />
                                            </div>
                                            <div class="card-body">
                                                  <label style="margin:0 2px 2px 2px">
                                                    Portal de Vendas para seguradoras(PF)
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>

                            </div>
                            <div class="row" style="padding-top: 20px">
                                <div class="col-sm-4 ">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://portalvendasksales.oncoprod.com.br/PortalVendas/Login.aspx">
                                            <div class="card-header ">
                                                <label>Portal e-OncoSales</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                <label style="margin:0 2px 2px 2px">
                                                    Portal de Vendas para clínicas (PJ)
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div id="divHomolog" style="margin: 10px 35px 20px 35px; display: none">
                            <h3>HOMOLOGAÇÃO</h3>
                            <div class="row">
                                <div class="col-sm-4 ">
                                    <div class="card">

                                        <a target="_blank" rel="noopener noreferrer" href="http://10.1.0.46/SimuladorPrecosteste/Login.aspx">
                                            <div class="card-header ">
                                                <label>Simulador de Preços</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/logo-oncosales-topo1.png" />
                                            </div>
                                            <div class="card-body">
                                                <label style="margin:0 2px 2px 2px">
                                                   Simulação de Preços com produtos Oncoprod
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card"  >
                                        <a target="_blank"  rel="noopener noreferrer" >
                                            <div class="card-header ">
                                                <label>ABC Farma</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px; color:#337ac7">
                                                   Importação de tabela de preços ABC Farma
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://10.1.58.6:8089/Usuario/Login">
                                            <div class="card-header ">
                                                <label>Tabela de Preços</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px">
                                                   Tabela para consulta de preços Oncoprod
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>

                            </div>
                            <div class="row" style="padding-top: 20px">
                                <div class="col-sm-4 ">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://10.1.58.6:8280/Login.aspx">
                                            <div class="card-header ">
                                                <label>Kraft </label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                  <label style="margin:0 2px 2px 2px">
                                                    Sistema de Vendas OncoSales Kraft
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" >
                                            <div class="card-header ">
                                                <label>OnConect </label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Onconect2.png" />
                                            </div>
                                            <div class="card-body">
                                                  <label style="margin:0 2px 2px 2px; color:#337ac7">
                                                   Sistema de apoio OncoSales Kraft
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="https://portalacessarhom.stcruz.com.br/acessar_homolog/login">
                                            <div class="card-header ">
                                                <label>Portal Acessar </label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/logo-acessar_web.png" />
                                            </div>
                                            <div class="card-body">
                                                  <label style="margin:0 2px 2px 2px">
                                                   Portal de Vendas para seguradoras(PF)
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>

                            </div>
                            <div class="row" style="padding-top: 20px">
                                <div class="col-sm-4 ">
                                    <div class="card">
                                        <a target="_blank" rel="noopener noreferrer" href="http://10.1.0.46/PortalVendasTeste/Login.aspx">
                                            <div class="card-header ">
                                                <label>Portal e-OncoSales</label>
                                                <img class="img-responsive" style="float: right; height: 30px" src="Imagens/Logo-oncosales-login.jpg" />
                                            </div>
                                            <div class="card-body">
                                                 <label style="margin:0 2px 2px 2px">
                                                   Portal de Vendas para clínicas (PJ)
                                                </label>

                                            </div>
                                        </a>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <script type="text/javascript">
                    $("#prod").click(function () {
                        $("#prod").addClass("active");
                        $("#homolog").removeClass("active");

                        $("#divProd").css("display", "block");
                        $("#divProd").css("background-color", "#ffff");
                        $("#divHomolog").css("display", "none")
                    });

                    $("#homolog").click(function () {
                        $("#divHomolog").css("background-color", "##cd7537");
                        $("#homolog").addClass("active");
                        $("#prod").removeClass("active");

                        $("#divHomolog").css("display", "block")
                        $("#divProd").css("display", "none")
                    });

                </script>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
