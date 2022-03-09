<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster/KraftMaster.Master" AutoEventWireup="true" CodeBehind="SimuladorPreco.aspx.cs" Inherits="KS.SimuladorPrecos.AppPaginas.Consulta.SimuladorPreco" %>

<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ucc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ucc" %>



    <link href="../../Styles/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="../../Scripts/jquery-3.3.1.slim.min.js"></script>
    <script type="text/javascript" src="../../Scripts/popper.min.js"></script>
    <script type="text/javascript" src="../../Scripts/bootstrap.min.js"></script>



    <link href="../../Styles/bootstrap.css" rel="stylesheet" />
    <script type="text/javascript" src="/Scripts/scriptMenuFlutuante.js"></script>


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

        desabilitaHomolog {
            display: none;
        }

        habilitaHomolog {
            display: block;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" content="width=device-width, initial-scale=1.0" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="uppSimulacao" runat="server" UpdateMode="Conditional">
        <Triggers>

            <asp:AsyncPostBackTrigger ControlID="btnSimular" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <!-- Bootstrap Modal Dialog -->
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

            <%-- PROGRESS BACKGROUND --%>
            <asp:UpdateProgress ID="upLoading" runat="server" AssociatedUpdatePanelID="uppSimulacao">
                <ProgressTemplate>
                    <div class="ProgressBackGround"></div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:Panel ID="pnlPrincipal" runat="server" DefaultButton="btnSimular">


                <div class="table-responsive-xl">
                    <table class="table" style="margin: 40px 0 0 0; background-color: #ecf0f3">
                        <tr>
                            <td style="width: 15%; border: 0;">
                                <div class="card ">
                                    <div class="card-header">
                                        <label>Item</label>
                                        <label style="color: red">*</label>
                                    </div>
                                    <div class="card-body-pesquisa" style="padding: 0 0 0 0; background-color: #ecf0f3">
                                        <asp:Panel ID="pnlItem" runat="server">
                                            <asp:TextBox ID="txtItem" CssClass="ajustarCamposPadrao" Height="26px" runat="server" SkinID="InputBootstrap" placeholder="Item ID" AutoPostBack="true" OnTextChanged="txtItem_TextChanged" />

                                            <ucc:AutoCompleteExtender ID="ddlaceItem" runat="server"
                                                CompletionListCssClass="autocomplete_list2"
                                                CompletionListItemCssClass="autocomplete_listitem2"
                                                CompletionListHighlightedItemCssClass="autocomplete_highlighted_listitem2"
                                                CompletionInterval="100"
                                                EnableCaching="false"
                                                MinimumPrefixLength="1"
                                                TargetControlID="txtItem"
                                                ServiceMethod="GetItens"
                                                ServicePath="../../AppWS/WsItens.asmx" />

                                        </asp:Panel>
                                    </div>
                                </div>
                            </td>
                            <td style="width: 70%; border: 0;">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Descrição</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label ForeColor="black" runat="server" ID="lblDescricao"></asp:Label>
                                    </div>
                                </div>
                            </td>
                            <td style="width: 70%; border: 0;">
                                <div class="card">
                                    <div class="card-header">
                                        <label>UF Origem</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label CssClass="Label" ForeColor="black" runat="server" ID="lblUfOrigem"></asp:Label>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <table class="table" style="margin: 0 0 0 0">
                        <tr>
                            <td style="width: 25%; border-top: 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Fornecedor</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblFornecedor"></asp:Label>
                                    </div>
                                </div>
                            </td>
                            <td style="width: 25%; border-top: 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Lista</label>
                                    </div>

                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblLista"></asp:Label>

                                    </div>
                                </div>
                            </td>
                            <td style="width: 20%; border-top: 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Categoria</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblCategoria"></asp:Label>
                                    </div>
                                </div>
                            </td>
                            <td style="width: 30%; border-top: 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Classific Fiscal</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblClassificFiscal"></asp:Label>
                                    </div>
                                </div>

                            </td>

                            <td style="width: 15%; border-top: 0; display:none" >
                                <div class="card">
                                    <div class="card-header">
                                        <label>EX Classif</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lbltipo"></asp:Label>
                                    </div>
                                </div>

                            </td>
                        </tr>
                    </table>
                    <table class="table" style="margin: 0 0 0 0; background-color: #ecf0f3">
                        <tr>
                            <td style="width: 25%; border-top: 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>NCM</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblNCM"></asp:Label>
                                    </div>
                                </div>

                            </td>
                            <td style="width: 25%; border-top: 0; margin: 0 0 0 0">

                                <div class="card">
                                    <div class="card-header">
                                        <label>Medicamento Controlado</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblMedicamento"></asp:Label>
                                    </div>
                                </div>

                            </td>
                            <td style="width: 25%; border-top: 0; margin: 0 0 0 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Uso Exclusivo Hospitalar</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblUsoExclusivo"></asp:Label>
                                    </div>
                                </div>

                            </td>
                            <td style="width: 25%; border-top: 0; margin: 0 0 0 0">
                                <div class="card">
                                    <div class="card-header">
                                        <label>Resolução 13</label>
                                    </div>
                                    <div class="card-body-pesquisa">
                                        <asp:Label runat="server" ForeColor="black" ID="lblResolucao13"></asp:Label>
                                    </div>
                                </div>

                            </td>
                        </tr>
                    </table>
                    <table class="table" style="margin: 0 0 0 0; background-color: #ecf0f3">
                        <tr>
                            <td style="width: 25%; border-top: 0; height: 170px; margin: 0 0 0 0">
                                <div class="card">
                                    <div class="card-header ">
                                        <label>Perfil Cliente</label>
                                        <label style="color: red">*</label>
                                    </div>
                                    <div class="card-body" style="height: 125px; background-color: #ecf0f3">
                                        <asp:RadioButtonList ID="rblPerfilCliente" CssClass="rbl" runat="server" RepeatColumns="1" AppendDataBoundItems="true" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="rblPerfilCliente_SelectedIndexChanged">
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
                                                    <label style="color: red">*</label>
                                                </div>
                                                <div class="card-body">
                                                    <asp:DropDownList ID="ddlEstadoDestino" Height="45px" SkinID="DropDownListBootstrap" runat="server" AutoPostBack="true" OnTextChanged="ddlEstadoDestino_TextChanged">
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
                                        <td style="width: 28.7%; border-top: 0; margin: 0 0 0 0">
                                            <div class="card">
                                                <div class="card-header">

                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <label>Preço Venda </label>
                                                            </td>

                                                            <td>
                                                                <label style="padding: 0 0 0 71px;">Preço Venda c/ ST </label>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                </div>
                                                <div class="card-body">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox runat="server" type="tel" Height="45px" Width="100%" SkinID="InputBootstrapMargem" ID="txtCalculaPrePrecoObjetivo" AutoPostBack="true" OnTextChanged="txtCalculaPrePrecoObjetivo_TextChanged" /></td>
                                                            <td>
                                                                <asp:TextBox runat="server" type="tel" Height="45px" Width="100%" SkinID="InputBootstrapMargem" Enabled="false" ID="txtPrecoObjetivo" />
                                                            </td>

                                                        </tr>
                                                    </table>


                                                </div>
                                            </div>
                                        </td>
                                        <td style="width: 27.8%; border-top: 0; margin: 0 0 0 0; padding: 0 0 0 0 !important;">
                                            <div class="card">
                                                <div class="card-header">
                                                    <label>CAP Aplicado</label>
                                                </div>
                                                <div class="card-body">
                                                    <asp:TextBox runat="server" type="tel" Height="45px" ID="txtCapAplicado" SkinID="InputBootstrap" Enabled="false" />
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 24.4%; border-top: 0; margin: 0 0 0 0;">
                                            <div class="card">
                                                <div class="card-header">
                                                    <label>Desconto Objetivo ($) </label>
                                                    <label style="color: red">*</label>
                                                </div>
                                                <div class="card-body">
                                                    <asp:TextBox runat="server" type="tel" Height="44px" SkinID="InputBootstrapMargem" ID="txtDescontoObjetivo" />
                                                </div>
                                            </div>
                                        </td>
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

                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>

                </div>

                <div style="float: right; margin: 0 0 40px 0">
                    <asp:Button SkinID="ButtonBootStrap" Text="Simular" runat="server" ID="btnSimular" OnClick="btnSimular_Click"></asp:Button>
                    <asp:Button SkinID="ButtonBootStrap" Text="Limpar" runat="server" ID="btnLimpar" OnClick="btnLimpar_Click"></asp:Button>

                </div>
                <fieldset class="card" style="font-size: 12px; float: left; background-color: #ecf0f3">
                    <asp:Literal ID="ltrDataAtualizacao" runat="server" />
                </fieldset>
                <div class="Detalhes CampoFixo" style="text-align: center; width: 149px; display: none;">
                    <asp:Label runat="server" ID="lblDesconto"></asp:Label>
                </div>


            </asp:Panel>

            <asp:Panel runat="server" ID="pnlSimulacao">

                <asp:DataList ID="dlSimulacoes" runat="server" Style="width: 100%; margin: 0 4px 10px 0; padding: 0 !important"
                    RepeatDirection="Horizontal" RepeatColumns="4"
                    OnItemDataBound="dlSimulacoes_ItemDataBound">
                    <ItemTemplate>
                        <div id="columEstabielecimento" runat="server" style="margin: 5px 10px 10px 0; border: solid 1px #000000; background-color: #ecf0f3">
                            <div style="background-color: #13213c;">
                                <asp:Label ID="lblHeader" Style="color: #fff; margin: 0 0 0 7px;" runat="server" Text='<%# Eval("estabelecimentoNome") %>' />
                            </div>
                            <div id="divPrecoFabrica" runat="server" style="display: table; width: 100%; border: solid 1px #000000;">

                                <div class="tb-body-left " style="">
                                    <asp:Literal ID="ltrPrecoFabrica" runat="server" Text='<%$ Resources:Resource, lblPrecoFabrica %>' />
                                </div>
                                <div class="tb-body-right ">
                                    <asp:Literal ID="ltrPrecoFabricaValor" runat="server" Text='<%# Eval("precoFabrica", "{0:n2}") %>' />
                                </div>
                            </div>


                            <div id="dvDescontoComercial" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrDescontoComercial" runat="server" Text='<%$ Resources:Resource, lblDescontoComercial %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrDescontoComercialValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvDescontoAdicionalValor" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrDescontoAdicional" runat="server" Text='<%$ Resources:Resource, lblDescontoAdicional %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrDescontoAdicionalValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvRepasseValor" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrRepasse" runat="server" Text='<%$ Resources:Resource, lblRepasse %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrRepasseValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPrecoAquisicao" runat="server" style="display: table; width: 100%; border: solid 1px #000000;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPrecoAquisicao" runat="server" Text='<%$ Resources:Resource, lblPrecoAquisicao %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrPrecoAquisicaoValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvReducaoBaseValor" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrReducaoBase" runat="server" Text='<%$ Resources:Resource, lblReducaoBase %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrReducaoBaseValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvICMSSEValor" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrICMSe" runat="server" Text='<%$ Resources:Resource, lblICMSe %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrICMSeValor" runat="server" />

                                </div>
                            </div>
                            <div id="dvCreditoICMSValor" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrCreditoICMS" runat="server" Text='<%$ Resources:Resource, lblICMSCreditoVlr %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrCreditoICMSValor" runat="server" />

                                </div>
                            </div>
                            <div id="dvIPIPrc" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrIPIPrc" runat="server" Text='<%$ Resources:Resource, lblIPIPrc %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrIPIPrcValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvIPIValor" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrIPIVlr" runat="server" Text='<%$ Resources:Resource, lblIPIVlr %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrIPIVlrValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPISCofinsPrc" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPISCofinsPrc" runat="server" Text='<%$ Resources:Resource, lblPISCofinsPrc %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrPISCofinsPrcValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPISCofins" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPISCofinsVlr" runat="server" Text='<%$ Resources:Resource, lblPISCofinsVlr %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrPISCofinsVlrValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPMC" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrPMC" runat="server" Text='<%# string.Format(GetResourceValue("lblPMC18"), decimal.Parse(Eval("percPmc").ToString()) < 1 ? (decimal.Parse(Eval("percPmc").ToString()) * 100) : decimal.Parse(Eval("percPmc").ToString()))  %>' />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrPMCValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvDescParaSTValor" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrDescParaST" runat="server" Text='<%$ Resources:Resource, lblDscST %>' />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrDescParaSTValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvMVA" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrMVA" runat="server" Text='<%$ Resources:Resource, lblMVA %>' />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrMVAValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvICMSST" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrICMSST" runat="server" Text='<%$ Resources:Resource, lblISMSSTVlr %>' />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrICMSSTValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvICMSAliquota" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrAliquotaICMS" runat="server" Text='<%$ Resources:Resource, lblAliquotaInternaICMS %>' />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrAliquotaICMSValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvTransES" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrTransES" runat="server" Text="Transf.ES" />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrTransESValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvCustoPadrao" runat="server" style="display: table; width: 100%; margin: 0 0 10px 0; border: solid 1px #000000;">
                                <div class="tb-body-left-color ">
                                    <asp:Literal ID="ltrCustoPadrao" runat="server" Text='<%$ Resources:Resource, lblCustoPadrao %>' />
                                </div>
                                <div class="tb-body-right-color ">
                                    <asp:Literal ID="ltrCustoPadraoValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPrecoVendaComST" runat="server" style="display: table; width: 100%; margin: 0 0 0 0; background-color: #8DB4E3;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPrecoVendaComST" runat="server" Text="Preço Venda C/ ST" />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrPrecoVendaValorComST" runat="server" />
                                </div>
                            </div>
                            <div id="dvICMSSobreVendaComST" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrICMSSobreVendaComST" runat="server" Text="(-) ICMS-ST Sobre Venda" />
                                </div>
                                <div class="tb-body-right  ">
                                    <%--txtPrecoObjetivo.Text--%>
                                    <asp:Literal ID="ltrICMSSobreVendaValorComST" runat="server" />
                                </div>
                            </div>


                            <div id="dvPrecoVenda" runat="server" style="display: table; width: 100%; margin: 0 0 0 0; background-color: #8DB4E3;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPrecoVenda" runat="server" Text="Preço Venda" />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrPrecoVendaValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                    GetPrecoVenda(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()))) %>' />
                                </div>
                            </div>


                            <div id="dvICMSSobreVenda" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrICMSSobreVenda" runat="server" Text="(-) ICMS Sobre Venda" />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrICMSSobreVendaValor" runat="server" />
                                </div>
                            </div>


                            <div id="dvBase" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrBaseLabel" runat="server" />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrBaseValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvAjusteRegimeFiscal" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrAjusteRegimeFiscal" runat="server" Text='<%$ Resources:Resource, lblAjusteRegimeFiscal %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrAjusteRegimeFiscalValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPISCofinsSobreVenda" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPISCofinsSobreVenda" runat="server" Text='<%$ Resources:Resource, lblPISCofinsSobreVenda %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrPISCofinsSobreVendaValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvPrecoVendaLiquido" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrPrecoVendaLiquido" runat="server" Text='<%$ Resources:Resource, lblPrecoVendaLiquido %>' />
                                </div>
                                <div style="font-size: 1vw; font-size: 1vw; vertical-align: middle; text-align: right;">
                                    <asp:Literal ID="ltrPrecoVendaLiquidoValor" runat="server" />
                                </div>
                            </div>

                            <div id="dvCustoPadraoVenda" runat="server" style="display: table; width: 100%;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrCustoPadraoVenda" runat="server" Text="(-) Custo Padrão" />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrCustoPadraoVendaValor" runat="server" />
                                </div>
                            </div>
                            <div id="dvMargemValor" runat="server" style="display: table; width: 100%; background-color: #8DB4E3;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrMargemVlr" runat="server" Text='<%$ Resources:Resource, lblMargemVlr %>' />
                                </div>
                                <div class="tb-body-right">
                                    <asp:Literal ID="ltrMargemVlrValor" runat="server"  />
                                </div>
                            </div>
                            <div id="dvMargemPercentual" runat="server" style="display: table; width: 100%; background-color: #8DB4E3;">
                                <div class="tb-body-left ">
                                    <asp:Literal ID="ltrMargemPrc" runat="server" Text='<%$ Resources:Resource, lblMargemPrc %>' />
                                </div>
                                <div class="tb-body-right  ">
                                    <asp:Literal ID="ltrMargemPrcValor" runat="server" />
                                </div>
                            </div>

                        </div>

                        </div>

                    </ItemTemplate>
                    <HeaderTemplate>
                        <div style="margin: 0 0 0 0; padding: 10px 0 0 0; font-size: 1vw; font: bold">
                            <asp:GridView ID="gvSumarizacao" runat="server" Font-Bold="true" SkinID="gvUniquePage" Width="30%">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Estabelecimento">
                                        <ItemTemplate>
                                            <asp:Label ID="estabelecimentoId" runat="server" Font-Bold="true" Style="font-size: 0.79vw; float: left; margin: 0 0 0 5px" Text='<%# Eval("estabelecimentoId") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Preço Venda">
                                        <ItemTemplate>
                                            <asp:Label ID="precoVenda" runat="server" Font-Bold="false" Style="font-size: 0.79vw;" Text='<%#  String.Format("{0:C}",Eval("precoVenda") )%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Margem %">
                                        <ItemTemplate>
                                            <asp:Label ID="ltrMrg" runat="server" Font-Bold="false" Style="font-size: 0.79vw;" Text='<%# string.Format("{0:n2}%", Eval("valorMargem")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </HeaderTemplate>
                </asp:DataList>
            </asp:Panel>



            <asp:RequiredFieldValidator ID="rfvItem" runat="server" ControlToValidate="txtItem" Display="None" SetFocusOnError="true" Text="*" ValidationGroup="ValidaItem" />
            <asp:ValidationSummary ID="vsmItem" runat="server" ShowMessageBox="true" ShowSummary="false" ValidationGroup="ValidaItem" />

        </ContentTemplate>
    </asp:UpdatePanel>

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

        $("#ContentPlaceHolder1_txtCalculaPrePrecoObjetivo").keydown(function (event) {


            if (event.shiftKey == true) {
                event.preventDefault();
            }

            if ((event.keyCode >= 48 && event.keyCode <= 57) ||
                (event.keyCode >= 96 && event.keyCode <= 105) ||
                event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
                event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

            } else {
                event.preventDefault();
            }

            if ($(this).val().indexOf('.') !== -1 && event.keyCode == 190)
                event.preventDefault();
            //if a decimal has been added, disable the "."-button

        });


    </script>


</asp:Content>
