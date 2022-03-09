<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster/KraftMaster.Master" AutoEventWireup="true" CodeBehind="SimuladroPrecoAll.aspx.cs" Inherits="KS.SimuladorPrecos.AppPaginas.Consulta.SimuladroPrecoAll" %>

<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ucc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="../../Scripts/jquery-3.3.1.slim.min.js"></script>

    <script type="text/javascript" src="../../Scripts/popper.min.js"></script>

    <script type="text/javascript" src="../../Scripts/bootstrap.min.js"></script>

    <link href="../../Styles/bootstrap.min.css" rel="stylesheet" />

    <link href="../../Styles/bootstrap.css" rel="stylesheet" />

    <style type="text/css">
        #dvPrecoVenda {
            background-color: #a2c5ef;
        }

        .card-header {
            background-color: #000066;
            color: #DBE5F1;
            height: 29px;
        }

        .tb-body-left {
            margin: 0 0 0 5px;
            font-size: 1vw;
            float: left;
        }

        .tb-body-left-color {
            margin: 0 0 0 5px;
            font-size: 1vw;
            float: left;
            background-color: #DBE5F1;
        }

        .tb-body-right {
            margin: 0 2px 0 0;
            font-size: 1vw;
            float: right;
        }

        .tb-body-right-color {
            font-size: 1vw;
            vertical-align: middle;
            text-align: right;
            background-color: #DBE5F1;
        }

        #BloCkTela {
            display: none;
            background: #CCCCFF url(../../Imagens/carregando.gif) center no-repeat;
            background-color: black;
            width: 100%;
            height: 100%;
            color: orange;
            position: fixed;
            top: 0;
            left: 0;
            z-index: 999;
            filter: alpha(opacity=50);
            opacity: 0.5;
            -moz-opacity: 0.5;
            -webkit-opacity: 0.5;
        }



        #screen {
            font-family: Calibri,Arial;
            font-weight: 300;
            font-size: 45px;
            width: 330px;
            height: 50px;
            color: gray;
            letter-spacing: 3px;
        }



        .emerald {
            border-radius: 5px;
            width: 80px;
            height: 30px;
            margin: 5px;
            border: 1px solid #384049;
            color: white;
            cursor: pointer;
            font-family: Calibri,Arial;
            font-weight: 300;
            outline: none;
            text-shadow: 0px -1px 1px black;
            text-transform: uppercase;
            transition: all 0.2s ease;
        }

        .emerald {
            background-image: linear-gradient(#3aad02,#2c6f05);
        }

            .emerald:active {
                background-image: linear-gradient(#2c6f05,#3aad02);
                text-shadow: 0px 1px 1px black;
            }
    </style>

</asp:Content>
<asp:Content ID="Content2" content="width=device-width, initial-scale=1.0" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <!-- Bootstrap Modal Dialog -->
    <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header" style="background-color: #D9DEE4">
                    <h4 class="modal-title">
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
        </div>
    </div>

    <%-- PROGRESS BACKGROUND --%>
    <div id="BloCkTela">
        <div class="chronometer" style="margin: 30% 0 0 43%;">
            <div id="screen">00 : 00 : 00</div>
        </div>
    </div>

    <%--PRINCIPAL PAGE--%>
    <div class="table-responsive-xl">
        <table class="table" style="margin: 0 0 0 0; background-color: #ecf0f3">
            <tr>
                <td style="width: 25%; border-top: 0; height: 170px; margin: 0 0 0 0">
                    <div class="card">
                        <div class="card-header ">
                            <label>Perfil Cliente</label>
                            <label style="color: red">*</label>
                        </div>
                        <div class="card-body" style="height: 125px; background-color: #ecf0f3">
                            <asp:RadioButtonList ID="rblPerfilCliente" CssClass="rbl" runat="server" RepeatColumns="1" AppendDataBoundItems="true" RepeatDirection="Vertical" AutoPostBack="true">
                                <asp:ListItem style="font-size: 1vw;" Value="0">   Mercado Público</asp:ListItem>
                                <asp:ListItem style="font-size: 1vw;" Value="C" Text=" Mercado Privado Contribuinte" />
                                <asp:ListItem style="font-size: 1vw;" Value="1" Text=" Mercado Privado Não Contribuinte" />
                                <asp:ListItem style="font-size: 1vw;" Value="2" Text=" Pessoa Física" />
                            </asp:RadioButtonList>

                        </div>
                        <div class="card-body" style="height: auto; font-size: 1vw; background-color: #ecf0f3; border: 1px inset black">
                            <asp:CheckBox ID="ckeckCalculaST" runat="server" />
                            <label for="coding">Regime Especial </label>
                        </div>
                    </div>
                </td>
                <td style="border-top: 0; background-color: #ecf0f3">
                    <table class="table" style="margin: 0 0 0 0; padding: 0 0 0 0 !important; background-color: #ecf0f3">
                        <tr>
                            <td style="width: 28.4%; border-top: 0; margin: 0 0 0 0; background-color: #ecf0f3">
                                <div class="card ">
                                    <div class="card-header">
                                        <label>Estado de Destino  </label>
                                    
                                    </div>
                                    <div class="card-body">
                                        <asp:DropDownList ID="ddlEstadoDestino" Height="45px" SkinID="DropDownListBootstrap" runat="server" AutoPostBack="true">
                                            <asp:ListItem style="font-size: 1vw;" Value="" Text="Selecione..." />
                                            <asp:ListItem style="font-size: 1vw;" Value="AC" Text="AC - Acre" />
                                            <asp:ListItem style="font-size: 1vw;" Value="AL" Text="AL - Alagoas" />
                                            <asp:ListItem style="font-size: 1vw;" Value="AP" Text="AP - Amapá" />
                                            <asp:ListItem style="font-size: 1vw;" Value="AM" Text="AM - Amazonas" />
                                            <asp:ListItem style="font-size: 1vw;" Value="BA" Text="BA - Bahia" />
                                            <asp:ListItem style="font-size: 1vw;" Value="CE" Text="CE - Ceará" />
                                            <asp:ListItem style="font-size: 1vw;" Value="DF" Text="DF - Distrito Federal" />
                                            <asp:ListItem style="font-size: 1vw;" Value="ES" Text="ES - Espírito Santo" />
                                            <asp:ListItem style="font-size: 1vw;" Value="GO" Text="GO - Goiás" />
                                            <asp:ListItem style="font-size: 1vw;" Value="MA" Text="MA - Maranhão" />
                                            <asp:ListItem style="font-size: 1vw;" Value="MT" Text="MT - Mato Grosso" />
                                            <asp:ListItem style="font-size: 1vw;" Value="MS" Text="MS - Mato Grosso do Sul" />
                                            <asp:ListItem style="font-size: 1vw;" Value="MG" Text="MG - Minas Gerais" />
                                            <asp:ListItem style="font-size: 1vw;" Value="PR" Text="PR - Paraná" />
                                            <asp:ListItem style="font-size: 1vw;" Value="PB" Text="PB - Paraíba" />
                                            <asp:ListItem style="font-size: 1vw;" Value="PA" Text="PA - Pará" />
                                            <asp:ListItem style="font-size: 1vw;" Value="PE" Text="PE - Pernambuco" />
                                            <asp:ListItem style="font-size: 1vw;" Value="PI" Text="PI - Piauí" />
                                            <asp:ListItem style="font-size: 1vw;" Value="RJ" Text="RJ - Rio de Janeiro" />
                                            <asp:ListItem style="font-size: 1vw;" Value="RN" Text="RN - Rio Grande do Norte" />
                                            <asp:ListItem style="font-size: 1vw;" Value="RS" Text="RS - Rio Grande do Sul" />
                                            <asp:ListItem style="font-size: 1vw;" Value="RO" Text="RO - Rondonia" />
                                            <asp:ListItem style="font-size: 1vw;" Value="RR" Text="RR - Roraima" />
                                            <asp:ListItem style="font-size: 1vw;" Value="SC" Text="SC - Santa Catarina" />
                                            <asp:ListItem style="font-size: 1vw;" Value="SE" Text="SE - Sergipe" />
                                            <asp:ListItem style="font-size: 1vw;" Value="SP" Text="SP - São Paulo" />
                                            <asp:ListItem style="font-size: 1vw;" Value="TO" Text="TO - Tocantins" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </td>
                            <td style="width: 24%; border-top: 0; margin: 0 0 0 0; padding: 0 0 0 0 !important;">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Desconto Adicional (%) </label>
                                    </div>
                                    <div class="card-body">
                                        <asp:TextBox runat="server" type="tel" Height="44px" SkinID="InputBootstrapMargem" ID="txtDescontoAdicional" />
                                    </div>
                                </div>
                            </td>
                            <td style="width: 24.4%; border-top: 0; margin: 0 0 0 0;">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Desconto Objetivo ($) </label>
                                        
                                    </div>
                                    <div class="card-body">
                                        <asp:TextBox runat="server" type="tel" Height="44px" SkinID="InputBootstrapMargem" ID="txtDescontoObjetivo" />
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>

                            <td style="width: 24.4%; border-top: 0; margin: 0 0 0 0">
                                <div class="card-group">
                                    <div class="card">
                                        <div class="card-header">
                                            <label>Margem Objetivo (%) </label>
                                            <label style="color: red">*</label>
                                        </div>
                                        <div class="card-group">
                                            <div class="card">
                                                <asp:TextBox Height="44px" type="tel" SkinID="InputBootstrapMargem" ID="txtMargemObjetivo" runat="server" AutoPostBack="true" />
                                            </div>

                                        </div>
                                    </div>
                                </div>

                            </td>
                            <td>
                                <div class="card-group">
                                    <div class="card">
                                        <div class="card-header">
                                            <label>Estabelecimento </label>
                                          
                                        </div>
                                        <div class="card-body">
                                            <asp:DropDownList ID="ddlEstabelecimentoId" Height="45px" SkinID="DropDownListBootstrap" runat="server" AutoPostBack="true">
                                                <asp:ListItem style="font-size: 1vw;" Value="" Text="Selecione..." />
                                                <asp:ListItem style="font-size: 1vw;" Value="2" Text="2  - DISTRIBUIDORA RS - ESTAB" />
                                                <asp:ListItem style="font-size: 1vw;" Value="3" Text="3  - DISTRIBUIDORA SP - ESTAB.  " />
                                                <asp:ListItem style="font-size: 1vw;" Value="4" Text="4  - SAR VIRTUAL SP - ESTAB.    " />
                                                <asp:ListItem style="font-size: 1vw;" Value="8" Text="8  - DROGARIA RS - ESTAB.      " />
                                                <asp:ListItem style="font-size: 1vw;" Value="11" Text="11 - NORPROD FORTALEZA - ESTAB. 	" />
                                                <asp:ListItem style="font-size: 1vw;" Value="12" Text="12 - DISTRIBUIDORA ES - ESTAB.  " />
                                                <asp:ListItem style="font-size: 1vw;" Value="13" Text="13 - DROGARIA ES - ESTAB.       " />
                                                <asp:ListItem style="font-size: 1vw;" Value="23" Text="23 - CD PE - ESTAB.            " />
                                                <asp:ListItem style="font-size: 1vw;" Value="20" Text="20 - HOSPLOG DF - ESTAB.  " />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div style="float: right">
        <asp:Button SkinID="ButtonBootStrap" Text="Simular" runat="server" ID="btnSimular" OnClientClick="return BloquearTela();" OnClick="btnSimular_Click"></asp:Button>
        <asp:Button SkinID="ButtonBootStrap" Text="Limpar" runat="server" ID="btnLimpar" OnClick="btnLimpar_Click"></asp:Button>
        <asp:Button SkinID="ButtonBootStrap" Text="Criar Excel" runat="server" ID="btnExportar" OnClick="btnExportar_Click1"></asp:Button>
    </div>
    <fieldset class="card" style="font-size: 12px; float: left; background-color: #ecf0f3">
        <asp:Literal ID="ltrDataAtualizacao" runat="server" />
    </fieldset>
    <div class="Detalhes CampoFixo" style="text-align: center; width: 149px; display: none;">
        <asp:Label runat="server" ID="lblDesconto"></asp:Label>
    </div>


    <fieldset class="card" style="font-size: 12px; float: left; background-color: #DBE5F1;">
        <asp:Literal ID="literarMsgFinal" runat="server" />
    </fieldset>

    <script type="text/javascript">
        (function ($) {
            $.fn.inputFilter = function (inputFilter) {
                return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
                    if (inputFilter(this.value)) {
                        this.oldValue = this.value;
                        this.oldSelectionStart = this.selectionStart;
                        this.oldSelectionEnd = this.selectionEnd;
                    } else if (this.hasOwnProperty("oldValue")) {
                        this.value = this.oldValue;
                        this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
                    }
                });
            };
        }(jQuery));

        $(document).ready(function () {
            $('.margem').inputFilter(function (value) {
                return /^-?\d*[.,]?\d*$/.test(value);
            });
        });

        function BloquearTela() {
            $("#BloCkTela").css("display", "block");
            start();

        };

        function DesBloquearTela() {
            $("#BloCkTela").css("display", "none");
        };


        window.onload = function () {
            pantalla = document.getElementById("screen");
        }
        var isMarch = false;
        var acumularTime = 0;

        function start() {
            if (isMarch == false) {
                timeInicial = new Date();
                control = setInterval(cronometro, 10);
                isMarch = true;
            }
        }
        function cronometro() {
            timeActual = new Date();
            acumularTime = timeActual - timeInicial;
            acumularTime2 = new Date();
            acumularTime2.setTime(acumularTime);
            cc = Math.round(acumularTime2.getMilliseconds() / 10);
            ss = acumularTime2.getSeconds();
            mm = acumularTime2.getMinutes();
            hh = acumularTime2.getHours() - 18;
            if (cc < 10) { cc = "0" + cc; }
            if (ss < 10) { ss = "0" + ss; }
            if (mm < 10) { mm = "0" + mm; }
            if (hh < 10) { hh = "0" + hh; }
            pantalla.innerHTML = mm + " : " + ss + " : " + cc;
        }

        function stop() {
            if (isMarch == true) {
                clearInterval(control);
                isMarch = false;
            }
        }

        function resume() {
            if (isMarch == false) {
                timeActu2 = new Date();
                timeActu2 = timeActu2.getTime();
                acumularResume = timeActu2 - acumularTime;

                timeInicial.setTime(acumularResume);
                control = setInterval(cronometro, 10);
                isMarch = true;
            }
        }

        function reset() {
            if (isMarch == true) {
                clearInterval(control);
                isMarch = false;
            }
            acumularTime = 0;
            pantalla.innerHTML = "00 : 00 : 00 : 00";
        }

    </script>
</asp:Content>
