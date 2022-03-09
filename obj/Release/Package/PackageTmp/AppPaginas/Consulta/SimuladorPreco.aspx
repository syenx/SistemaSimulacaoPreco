<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster/KraftMaster.Master" AutoEventWireup="true" CodeBehind="SimuladorPreco.aspx.cs" Inherits="KS.SimuladorPrecos.AppPaginas.Consulta.SimuladorPreco" %>

<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ucc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .Centro {
            width: 910px;
            padding-left: 200px;
        }

        .BordaArredondada {
            float: left;
            border-radius: 10px;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            width: 910px;
            height: 50px;
            background-color: #F5F5F5;
            border: 1px solid #D3D3D3;
            left: 200px;
            position: absolute;
        }

        .Titulo {
            text-align: center;
            color: Red;
        }

        .Rotulo {
            float: left;
            text-align: center;
            width: 100px;
            color: White;
            background-color: #000066;
            border-right: 1px solid;
        }

        .Detalhes {
            float: left;
            width: 99px;
            border: 1px solid;
            height: 23px;
            font-family: Calibri;
        }

        .CampoFixo {
            background-color: Yellow;
            font-weight: bold;
        }

        .CampoUsuario {
            background-color: #3399FF;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="uppSimulacao" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnItem" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSimular" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <%-- PROGRESS BACKGROUND --%>
            <asp:UpdateProgress ID="upLoading" runat="server" AssociatedUpdatePanelID="uppSimulacao">
                <ProgressTemplate>
                    <div class="ProgressBackGround"></div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div id="Menu" class="Centro" style="margin-top: 50px;">
                <h1 class="Titulo">
                    <asp:Literal ID="ltrHeader" runat="server" Text="<%$ Resources:Resource, lblSimulacaoVenda %>" /></h1>
                <div id="subTitulo" style="text-align: center">
                    <h3>
                        <asp:Literal ID="ltrHeaderSubTitle" runat="server" Text="<%$ Resources:Resource, lblInformacoesBasicas %>" /></h3>
                </div>
                <fieldset>
                    <div id="Div1" style="text-align: center">
                        <h3>
                            <asp:Literal ID="ltrDataAtualizacao" runat="server" /></h3>
                    </div>
                </fieldset>
                <asp:Panel ID="pnlPrincipal" runat="server" DefaultButton="btnSimular">
                    <hr />
                    <div id="linha">
                        <div class="Rotulo">
                            <asp:Literal ID="ltrItem" runat="server" Text="<%$ Resources:Resource, lblItem %>" />
                        </div>
                        <div class="Rotulo" style="width: 575px">
                            <asp:Literal ID="ltrDescricao" runat="server" Text="<%$ Resources:Resource, lblDescricao %>" />
                        </div>
                        <div class="Rotulo" style="width: 220px; margin-left: 9px;">
                            <asp:Literal ID="ltrUfOrigem" runat="server" Text="<%$ Resources:Resource, lblUfOrigem %>" />
                        </div>
                        <div class="Rotulo" style="width: 150px; display: none;">
                            <asp:Literal ID="ltrDescontoPadrao" runat="server" Text="<%$ Resources:Resource, lblDescontoPadrao %>" />
                        </div>
                        <div class="Detalhes" style="text-align: center">
                            <asp:Panel ID="pnlItem" runat="server" DefaultButton="btnItem">
                                <asp:TextBox runat="server" ID="txtItem" Width="90px" MaxLength="6"
                                    EnableTheming="True" TabIndex="1" AutoPostBack="true"
                                    OnTextChanged="txtItem_TextChanged" />
                                <ucc:AutoCompleteExtender ID="aceItens" runat="server"
                                    CompletionListCssClass="autocomplete_list"
                                    CompletionListItemCssClass="autocomplete_listitem"
                                    CompletionListHighlightedItemCssClass="autocomplete_highlighted_listitem"
                                    CompletionInterval="100"
                                    EnableCaching="false"
                                    MinimumPrefixLength="1"
                                    TargetControlID="txtItem"
                                    ServiceMethod="GetItens"
                                    ServicePath="~/AppWS/WsItens.asmx" />
                                <asp:Button ID="btnItem" runat="server" OnClick="btnItem_Click" />
                            </asp:Panel>
                        </div>
                        <div class="Detalhes CampoFixo" style="width: 573px">
                            &nbsp
                            <asp:Label runat="server" ID="lblDescricao"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 218px; margin-left: 10px;">
                            <asp:Label runat="server" ID="lblUfOrigem"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 149px; display: none;">
                            <asp:Label runat="server" ID="lblDesconto"></asp:Label>
                        </div>
                    </div>
                    <div id="Linha2">
                        <div class="Rotulo" style="width: 218px; margin-top: 10px">
                            <asp:Literal ID="ltrFornecedor" runat="server" Text="<%$ Resources:Resource, lblFornecedor %>" />
                        </div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">
                            <asp:Literal ID="ltrLista" runat="server" Text="<%$ Resources:Resource, lblLista %>" />
                        </div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">
                            <asp:Literal ID="ltrCategoria" runat="server" Text="<%$ Resources:Resource, lblCategoria %>" />
                        </div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">
                            <asp:Literal ID="ltrClasseFiscal" runat="server" Text="<%$ Resources:Resource, lblClasseFiscal %>" />
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px">
                            <asp:Label runat="server" ID="lblFornecedor"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                            <asp:Label runat="server" ID="lblLista"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                            <asp:Label runat="server" ID="lblCategoria"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                            <asp:Label runat="server" ID="lblClasseFiscal"></asp:Label>
                        </div>
                    </div>
                    <div id="Linha3">
                        <div class="Rotulo" style="width: 218px; margin-top: 10px">NCM</div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">Medicamento Controlado</div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">Uso Exclusivo Hospitalar</div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">Resolução 13</div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px">
                            <asp:Label runat="server" ID="lblNCM"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                            <asp:Label runat="server" ID="lblMedicamento"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                            <asp:Label runat="server" ID="lblUsoExclusivo"></asp:Label>
                        </div>
                        <div class="Detalhes CampoFixo" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                            <asp:Label runat="server" ID="lblResolucao13"></asp:Label>
                        </div>
                    </div>
                    <div id="linha4">
                        <div class="Rotulo" style="width: 218px; margin-top: 10px">Perfil do Cliente</div>
                        <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">Estado de Destino</div>
                        <div style="width: 455px; float: right; height: 115px; margin-top: 5px; border-bottom: 1px solid">
                            <hr style="color: Black" />
                            <div class="Rotulo" style="width: 218px">Preço Objetivo ($)</div>
                            <div class="Rotulo" style="width: 218px; margin: 0 0 0 10px">CAP Aplicado</div>
                            <div class="Detalhes" style="text-align: center; width: 216px;">
                                <asp:TextBox runat="server" ID="txtPrecoObjetivo" Width="210px" MaxLength="18" TabIndex="4" />
                            </div>
                            <div class="Detalhes" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                                <asp:TextBox runat="server" ID="txtCapAplicado" Width="210px" MaxLength="18" TabIndex="5" Enabled="false" />
                            </div>
                            <div class="Rotulo" style="width: 218px; margin: 10px 0 0 0">Margem Objetivo (%)</div>
                            <div class="Rotulo" style="width: 218px; margin: 10px 0 0 10px">Desconto Adicional (%)</div>
                            <div class="Detalhes" style="text-align: center; width: 216px;">
                                <asp:TextBox runat="server" ID="txtMargemObjetivo" Width="210px" MaxLength="18" TabIndex="6" />
                            </div>
                            <div class="Detalhes" style="text-align: center; width: 216px; margin: 0 0 0 11px">
                                <asp:TextBox runat="server" ID="txtDescontoAdicional" Width="210px" MaxLength="18" TabIndex="7" />
                            </div>
                        </div>
                        <div class="Detalhes" style="text-align: left; width: 216px; height: 95px; font-size: 12px">
                            <asp:RadioButtonList ID="rblPerfilCliente" runat="server" RepeatColumns="1" RepeatDirection="Vertical" TabIndex="2">
                                <asp:ListItem Value="0" Text="Mercado Público" />
                                <asp:ListItem Value="C" Text="Mercado Privado Contribuinte" />
                                <asp:ListItem Value="1" Text="Mercado Privado Não Contribuinte" />
                                <asp:ListItem Value="2" Text="Pessoa Física" />
                            </asp:RadioButtonList>
                        </div>
                        <div class="Detalhes" style="text-align: center; width: 216px; margin: 0 0 0 11px; height: 31px">
                            <asp:DropDownList ID="ddlEstadoDestino" runat="server" Width="215px" TabIndex="3">
                                <asp:ListItem Value="" Text="Selecione..." />
                                <asp:ListItem Value="AC" Text="AC - Acre" />
                                <asp:ListItem Value="AL" Text="AL - Alagoas" />
                                <asp:ListItem Value="AP" Text="AP - Amapá" />
                                <asp:ListItem Value="AM" Text="AM - Amazonas" />
                                <asp:ListItem Value="BA" Text="BA - Bahia" />
                                <asp:ListItem Value="CE" Text="CE - Ceará" />
                                <asp:ListItem Value="DF" Text="DF - Distrito Federal" />
                                <asp:ListItem Value="ES" Text="ES - Espírito Santo" />
                                <asp:ListItem Value="GO" Text="GO - Goiás" />
                                <asp:ListItem Value="MA" Text="MA - Maranhão" />
                                <asp:ListItem Value="MT" Text="MT - Mato Grosso" />
                                <asp:ListItem Value="MS" Text="MS - Mato Grosso do Sul" />
                                <asp:ListItem Value="MG" Text="MG - Minas Gerais" />
                                <asp:ListItem Value="PR" Text="PR - Paraná" />
                                <asp:ListItem Value="PB" Text="PB - Paraíba" />
                                <asp:ListItem Value="PA" Text="PA - Pará" />
                                <asp:ListItem Value="PE" Text="PE - Pernambuco" />
                                <asp:ListItem Value="PI" Text="PI - Piauí" />
                                <asp:ListItem Value="RJ" Text="RJ - Rio de Janeiro" />
                                <asp:ListItem Value="RN" Text="RN - Rio Grande do Norte" />
                                <asp:ListItem Value="RS" Text="RS - Rio Grande do Sul" />
                                <asp:ListItem Value="RO" Text="RO - Rondonia" />
                                <asp:ListItem Value="RR" Text="RR - Roraima" />
                                <asp:ListItem Value="SC" Text="SC - Santa Catarina" />
                                <asp:ListItem Value="SE" Text="SE - Sergipe" />
                                <asp:ListItem Value="SP" Text="SP - São Paulo" />
                                <asp:ListItem Value="TO" Text="TO - Tocantins" />
                            </asp:DropDownList>
                        </div>
                        <div style="width: 227px; padding-bottom: 14px; margin-left: 10px; float: left; border-bottom: 1px solid">
                            <div class="Rotulo" style="width: 218px; margin-top: 10px;">Desconto Objetivo ($)</div>
                            <div class="Detalhes" style="text-align: center; width: 216px;">
                                <asp:TextBox runat="server" ID="txtDescontoObjetivo" Width="210px" MaxLength="18" TabIndex="4" />
                            </div>
                        </div>
                    </div>
                    <div id="Botao" style="text-align: right; width: 100%; float: left; margin-top: 10px">
                        <asp:Button runat="server" ID="btnSimular" Text="Simular" Width="100px" TabIndex="8" ValidationGroup="ValidaItem" OnClick="btnSimular_Click" />
                    </div>
                </asp:Panel>
                <div style="width: 100%; float: left;">
                    <div style="text-align: center; margin-top: 20px;">
                        <center />
                        <asp:Panel runat="server" ID="pnlSimulacao">
                            <h3>Simulações</h3>
                            <div>
                                <center />
                                <asp:DataList ID="dlSimulacoes" runat="server" TabIndex="9"
                                    HorizontalAlign="Left" RepeatDirection="Horizontal"
                                    RepeatLayout="Table" RepeatColumns="4"
                                    OnItemDataBound="dlSimulacoes_ItemDataBound">
                                    <ItemTemplate>
                                        <div style="width: 275px; margin-left: 2px;">
                                            <div style="display: table; width: 100%; background-color: #13213C;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: center;">
                                                    <asp:Label ID="lblHeader" runat="server" Font-Bold="true" ForeColor="White" Text='<%# Eval("estabelecimentoNome") %>' />
                                                </div>
                                            </div>
                                            <div id="dvPrecoFabricaValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrPrecoFabrica" runat="server" Text='<%$ Resources:Resource, lblPrecoFabrica %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrPrecoFabricaValor" runat="server" Text='<%# Eval("precoFabrica", "{0:n2}") %>' />
                                                </div>
                                            </div>
                                            <div id="dvDescontoComercial" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrDescontoComercial" runat="server" Text='<%$ Resources:Resource, lblDescontoComercial %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrDescontoComercialValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                                    GetDescontoComercial
                                                                                                                                        (
                                                                                                                                        decimal.Parse(Eval("descontoComercial").ToString()) < 1 ?
                                                                                                                                        (decimal.Parse(Eval("descontoComercial").ToString()) * 100) :
                                                                                                                                        decimal.Parse(Eval("descontoComercial").ToString())
                                                                                                                                        )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvDescontoAdicionalValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrDescontoAdicional" runat="server" Text='<%$ Resources:Resource, lblDescontoAdicional %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrDescontoAdicionalValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                            GetDescontoAdicional(decimal.Parse(Eval("descontoAdicional").ToString())) < 1 ? 
                                                                                                                                (GetDescontoAdicional(decimal.Parse(Eval("descontoAdicional").ToString())) * 100) : 
                                                                                                                                    GetDescontoAdicional(decimal.Parse(Eval("descontoAdicional").ToString()))) %>' />
                                                </div>
                                            </div>
                                            <div id="dvRepasseValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrRepasse" runat="server" Text='<%$ Resources:Resource, lblRepasse %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrRepasseValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                          decimal.Parse(Eval("percRepasse").ToString()) < 1 ? 
                                                                                                                          (decimal.Parse(Eval("percRepasse").ToString()) * 100) : 
                                                                                                                          decimal.Parse(Eval("percRepasse").ToString()
                                                                                                                        )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvPrecoAquisicao" runat="server" style="display: table; width: 100%; border: solid 1px #000000;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrPrecoAquisicao" runat="server" Text='<%$ Resources:Resource, lblPrecoAquisicao %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrPrecoAquisicaoValor" runat="server"
                                                        Text='<%# string.Format("{0:n2}", 
                                                                       GetPrecoAquisicao(decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                         GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                         decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                         decimal.Parse(Eval("percRepasse").ToString())
                                                                                        )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvReducaoBaseValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrReducaoBase" runat="server" Text='<%$ Resources:Resource, lblReducaoBase %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrReducaoBaseValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                              decimal.Parse(Eval("percReducaoBase").ToString()) < 1 ? 
                                                                                                                              (decimal.Parse(Eval("percReducaoBase").ToString()) * 100) : 
                                                                                                                              decimal.Parse(Eval("percReducaoBase").ToString()
                                                                                                                            )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvICMSSEValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrICMSe" runat="server" Text='<%$ Resources:Resource, lblICMSe %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrICMSeValor" runat="server" Text='<%# string.Format("{0:n2}%",GetPercICMSe( decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                                         Eval("estabelecimentoId").ToString(),  
                                                                                                                                         Eval("ufIdOrigem").ToString(),
                                                                                                                                         Eval("resolucao13").ToString()
                                                                                                                      )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvCreditoICMSValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrCreditoICMS" runat="server" Text='<%$ Resources:Resource, lblICMSCreditoVlr %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrCreditoICMSValor" runat="server" Text='<%# string.Format("{0:n2}",     
                                                                                                               GetCreditoICMS(decimal.Parse(Eval("percReducaoBase").ToString()),
                                                                                                                              decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                              decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                              GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                              decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                              decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                int.Parse(Eval("itemCodigoOrigem").ToString()),
                                                                                                                                Eval("tratamentoICMSEstab").ToString(),
                                                                                                                                Eval("estabelecimentoId").ToString(),
                                                                                                                                Eval("ufIdOrigem").ToString(),
                                                                                                                                Eval("resolucao13").ToString()
                                                                                                                              ))    %>' />
                                                </div>
                                            </div>
                                            <div id="dvIPIPrc" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrIPIPrc" runat="server" Text='<%$ Resources:Resource, lblIPIPrc %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrIPIPrcValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                         decimal.Parse(Eval("percIPI").ToString()) < 1 ? 
                                                                                                                         (decimal.Parse(Eval("percIPI").ToString()) * 100) : 
                                                                                                                         decimal.Parse(Eval("percIPI").ToString()
                                                                                                                       )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvIPIValor" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrIPIVlr" runat="server" Text='<%$ Resources:Resource, lblIPIVlr %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrIPIVlrValor" runat="server" Text='<%# string.Format("{0:n2}",
                                                                                                            GetValorIPI(decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                        decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                        GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                        decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                        decimal.Parse(Eval("percRepasse").ToString())
                                                                                                                       )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvPISCofinsPrc" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrPISCofinsPrc" runat="server" Text='<%$ Resources:Resource, lblPISCofinsPrc %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrPISCofinsPrcValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                               decimal.Parse(Eval("percPisCofins").ToString()) < 1 ? 
                                                                                                                               (decimal.Parse(Eval("percPisCofins").ToString()) * 100) : 
                                                                                                                               decimal.Parse(Eval("percPisCofins").ToString()
                                                                                                                             )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvPISCofins" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrPISCofinsVlr" runat="server" Text='<%$ Resources:Resource, lblPISCofinsVlr %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrPISCofinsVlrValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                    GetValorPISCofins(decimal.Parse(Eval("percPisCofins").ToString()),
                                                                                                                                      decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                                      GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                                      decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                                      decimal.Parse(Eval("percRepasse").ToString())
                                                                                                                                     )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvPMC" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrPMC" runat="server" Text='<%# string.Format(GetResourceValue("lblPMC18"), decimal.Parse(Eval("percPmc").ToString()) < 1 ? (decimal.Parse(Eval("percPmc").ToString()) * 100) : decimal.Parse(Eval("percPmc").ToString()))  %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrPMCValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                      decimal.Parse(Eval("pmc17").ToString()) < 1 ? 
                                                                                                                      (decimal.Parse(Eval("pmc17").ToString()) * 100) : 
                                                                                                                      decimal.Parse(Eval("pmc17").ToString()
                                                                                                                    )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvDescParaSTValor" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrDescParaST" runat="server" Text='<%$ Resources:Resource, lblDscST %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrDescParaSTValor" runat="server" Text='<%# Eval("descST").ToString().Equals("-") ? 
                                                                                                                Eval("descST") : 
                                                                                                                string.Format("{0:n2}%", 
                                                                                                                               decimal.Parse(Eval("descST").ToString()) < 1 ?
                                                                                                                               (decimal.Parse(Eval("descST").ToString()) * 100) :
                                                                                                                               decimal.Parse(Eval("descST").ToString()
                                                                                                                             )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvMVA" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrMVA" runat="server" Text='<%$ Resources:Resource, lblMVA %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrMVAValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                      decimal.Parse(Eval("mva").ToString()) < 1 ? 
                                                                                                                      (decimal.Parse(Eval("mva").ToString()) * 100) : 
                                                                                                                      decimal.Parse(Eval("mva").ToString()
                                                                                                                    )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvICMSST" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrICMSST" runat="server" Text='<%$ Resources:Resource, lblISMSSTVlr %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrICMSSTValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                            GetValorICMS_ST(Eval("descST").ToString(),
                                                                                                                            decimal.Parse(Eval("mva").ToString()),
                                                                                                                            decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                            decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                            decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                            GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                            decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                            decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                            decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                            decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                            decimal.Parse(Eval("reducaoST_MVA").ToString())
                                                                                                                           )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvICMSAliquota" runat="server" style="display: table; width: 100%; background-color: Blue;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrAliquotaICMS" runat="server" Text='<%$ Resources:Resource, lblAliquotaInternaICMS %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrAliquotaICMSValor" runat="server" Text='<%# string.Format("{0:n2}%", 
                                                                                                                    decimal.Parse(Eval("aliquotaInternaICMS").ToString()) < 1 ? 
                                                                                                                        (decimal.Parse(Eval("aliquotaInternaICMS").ToString()) * 100) : 
                                                                                                                            Eval("aliquotaInternaICMS")) %>' />
                                                </div>
                                            </div>
                                            <div id="dvCustoPadrao" runat="server" style="display: table; width: 100%; border: solid 1px #000000;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrCustoPadrao" runat="server" Text='<%$ Resources:Resource, lblCustoPadrao %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #DBE5F1;">
                                                    <asp:Literal ID="ltrCustoPadraoValor" runat="server" Text='<%# string.Format("{0:n2}",
                                                                                                                    GetValorCustoPadrao(
                                                                                                                                        decimal.Parse(Eval("percReducaoBase").ToString()), 
                                                                                                                                        decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                                        decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                                        decimal.Parse(Eval("percPisCofins").ToString()),
                                                                                                                                        Eval("descST").ToString(),
                                                                                                                                        decimal.Parse(Eval("mva").ToString()),
                                                                                                                                        decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                                        decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                        decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                                        GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                                        decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                                        decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                        decimal.Parse(Eval("valorICMSST").ToString()),    
                                                                                                                                        decimal.Parse(Eval("reducaoST_MVA").ToString()),
                                                                                                                                        Eval("estabelecimentoId").ToString(),
                                                                                                                                        Eval("tipo").ToString(),
                                                                                                                                        int.Parse(Eval("itemCodigoOrigem").ToString()),
                                                                                                                                        Eval("tratamentoICMSEstab").ToString(),
                                                                                                                                        Eval("ufIdOrigem").ToString(),
                                                                                                                                        Eval("resolucao13").ToString()
                                                                                                                                       )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvLinhaVaga" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; background-color: transparent; color: transparent;">
                                                    <asp:Literal ID="ltr" runat="server" Text="T" />
                                                </div>
                                            </div>
                                            <div id="dvPrecoVenda" runat="server" style="display: table; width: 100%; border: solid 1px #000000;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #8DB4E3;">
                                                    <asp:Literal ID="ltrPrecoVenda" runat="server" Text='<%$ Resources:Resource, lblPrecoVenda %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #8DB4E3;">
                                                    <asp:Literal ID="ltrPrecoVendaValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                    GetPrecoVenda(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()))) %>' />
                                                    <%--GetPrecoVenda(txtPrecoObjetivo.Text)--%>
                                                </div>
                                            </div>
                                            <div id="dvICMSSobreVenda" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrICMSSobreVenda" runat="server" Text='<%$ Resources:Resource, lblICMSSobreVenda %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <%--txtPrecoObjetivo.Text--%>
                                                    <asp:Literal ID="ltrICMSSobreVendaValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                        GetICMSSobreVenda(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()),
                                                                                                                                          Eval("estabelecimentoId").ToString(),
                                                                                                                                          Eval("tipo").ToString(),
                                                                                                                                          rblPerfilCliente.SelectedValue,
                                                                                                                                          Eval("resolucao13").ToString(),
                                                                                                                                          Eval("exclusivoHospitalar").ToString(),
                                                                                                                                          Eval("descST").ToString(),
                                                                                                                                          decimal.Parse(Eval("mva").ToString()),
                                                                                                                                          decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                                          decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                          decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                                          GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                                          decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                                          decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                          decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                                          decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                                          decimal.Parse(Eval("reducaoST_MVA").ToString())
                                                                                                                                         )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvICMSSTSobreVenda" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrICMSSTSobreVenda" runat="server" Text='<%$ Resources:Resource, lblICMSSobreVendaST %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrICMSSTSobreVendaValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                        GetICMSSTSobreVenda(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()),
                                                                                                                                            Eval("estabelecimentoId").ToString(),
                                                                                                                                            Eval("tipo").ToString(),                                                                                                                                            
                                                                                                                                            rblPerfilCliente.SelectedValue,
                                                                                                                                            Eval("resolucao13").ToString(),
                                                                                                                                            Eval("listaDescricao").ToString(),
                                                                                                                                            Eval("categoria").ToString(),
                                                                                                                                            Eval("ufIdOrigem").ToString(),
                                                                                                                                            decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                            decimal.Parse(Eval("percReducaoBase").ToString()), 
                                                                                                                                            decimal.Parse(Eval("percICMSe").ToString()), 
                                                                                                                                            decimal.Parse(Eval("percIPI").ToString()), 
                                                                                                                                            decimal.Parse(Eval("percPisCofins").ToString()), 
                                                                                                                                            Eval("descST").ToString(), 
                                                                                                                                            decimal.Parse(Eval("mva").ToString()), 
                                                                                                                                            decimal.Parse(Eval("aliquotaInternaICMS").ToString()), 
                                                                                                                                            decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                                            GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                                            decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                                            decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                            decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                                            decimal.Parse(Eval("reducaoST_MVA").ToString()),
                                                                                                                                            Eval("exclusivoHospitalar").ToString(),
                                                                                                                                            false,
                                                                                                                                            null,true
                                                                                                                                           )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvAjusteRegimeFiscal" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrAjusteRegimeFiscal" runat="server" Text='<%$ Resources:Resource, lblAjusteRegimeFiscal %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrAjusteRegimeFiscalValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                        GetAjusteRegimeFiscalSobreVenda(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()),
                                                                                                                                                        Eval("estabelecimentoId").ToString(),
                                                                                                                                                        Eval("tipo").ToString(),
                                                                                                                                                        rblPerfilCliente.SelectedValue,
                                                                                                                                                        Eval("resolucao13").ToString(),
                                                                                                                                                        decimal.Parse(Eval("percReducaoBase").ToString()),
                                                                                                                                                        decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                                                        decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                                                        decimal.Parse(Eval("percPisCofins").ToString()),
                                                                                                                                                        Eval("descST").ToString(),
                                                                                                                                                        decimal.Parse(Eval("mva").ToString()),
                                                                                                                                                        decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                                                        decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                                        decimal.Parse(Eval("precoFabrica").ToString()),
                                                                                                                                                        GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())),
                                                                                                                                                        decimal.Parse(Eval("descontoAdicional").ToString()),
                                                                                                                                                        decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                                        Eval("ufIdOrigem").ToString(),
                                                                                                                                                        Eval("exclusivoHospitalar").ToString(),
                                                                                                                                                        decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                                                        decimal.Parse(Eval("reducaoST_MVA").ToString())
                                                                                                                                                       )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvPISCofinsSobreVenda" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrPISCofinsSobreVenda" runat="server" Text='<%$ Resources:Resource, lblPISCofinsSobreVenda %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrPISCofinsSobreVendaValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                            GetPISCofinsSobreVenda(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()),
                                                                                                                                                   decimal.Parse(Eval("percPisCofins").ToString())
                                                                                                                                                  )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvPrecoVendaLiquido" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrPrecoVendaLiquido" runat="server" Text='<%$ Resources:Resource, lblPrecoVendaLiquido %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrPrecoVendaLiquidoValor" runat="server" Text='<%# string.Format("{0:n2}", 
                                                                                                                        GetPrecoVendaLiquido(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()),
                                                                                                                                             decimal.Parse(Eval("percPisCofins").ToString()),
                                                                                                                                             Eval("estabelecimentoId").ToString(),
                                                                                                                                             Eval("tipo").ToString(),
                                                                                                                                             rblPerfilCliente.SelectedValue,
                                                                                                                                             Eval("resolucao13").ToString(),
                                                                                                                                             decimal.Parse(Eval("percReducaoBase").ToString()),
                                                                                                                                             decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                                             decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                                             Eval("descST").ToString(),
                                                                                                                                             decimal.Parse(Eval("mva").ToString()),
                                                                                                                                             decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                                             decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                             decimal.Parse(Eval("precoFabrica").ToString()),
                                                                                                                                             GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())),
                                                                                                                                             decimal.Parse(Eval("descontoAdicional").ToString()),
                                                                                                                                             decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                             Eval("ufIdOrigem").ToString(),
                                                                                                                                             Eval("exclusivoHospitalar").ToString(),
                                                                                                                                             decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                                             decimal.Parse(Eval("reducaoST_MVA").ToString()),
                                                                                                                                             Eval("listaDescricao").ToString(),
                                                                                                                                             Eval("categoria").ToString()
                                                                                                                                            )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvCustoPadraoVenda" runat="server" style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%;">
                                                    <asp:Literal ID="ltrCustoPadraoVenda" runat="server" Text='<%$ Resources:Resource, lblCustoPadraoCalculo %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%;">
                                                    <asp:Literal ID="ltrCustoPadraoVendaValor" runat="server" Text='<%# string.Format("{0:n2}",
                                                                                                                        GetValorCustoPadrao(
                                                                                                                                            decimal.Parse(Eval("percReducaoBase").ToString()), 
                                                                                                                                            decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                                            decimal.Parse(Eval("percIPI").ToString()),
                                                                                                                                            decimal.Parse(Eval("percPisCofins").ToString()),
                                                                                                                                            Eval("descST").ToString(),
                                                                                                                                            decimal.Parse(Eval("mva").ToString()),
                                                                                                                                            decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                                            decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                            decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                                            GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                                            decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                                            decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                            decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                                            decimal.Parse(Eval("reducaoST_MVA").ToString()),
                                                                                                                                            Eval("estabelecimentoId").ToString(),
                                                                                                                                            Eval("tipo").ToString(),
                                                                                                                                            int.Parse(Eval("itemCodigoOrigem").ToString()),
                                                                                                                                             Eval("tratamentoICMSEstab").ToString(),
                                                                                                                                             Eval("ufIdOrigem").ToString(),
                                                                                                                                              Eval("resolucao13").ToString()
                                                                                                                                           )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvMargemValor" runat="server" style="display: table; width: 100%; border-left: solid 1px #000000; border-top: solid 1px #000000; border-right: solid 1px #000000;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #8DB4E3;">
                                                    <asp:Literal ID="ltrMargemVlr" runat="server" Text='<%$ Resources:Resource, lblMargemVlr %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #8DB4E3;">
                                                    <asp:Literal ID="ltrMargemVlrValor" runat="server" Text='<%# string.Format("{0:n2}",
                                                                                                                    GetValorMagem(GetPrecoVendaDesconto(Eval("estabelecimentoId").ToString()),
                                                                                                                                  decimal.Parse(Eval("percPisCofins").ToString()),
                                                                                                                                  Eval("estabelecimentoId").ToString(),
                                                                                                                                  Eval("tipo").ToString(),
                                                                                                                                  rblPerfilCliente.SelectedValue,
                                                                                                                                  Eval("resolucao13").ToString(),
                                                                                                                                  decimal.Parse(Eval("percReducaoBase").ToString()), 
                                                                                                                                  decimal.Parse(Eval("percICMSe").ToString()),
                                                                                                                                  decimal.Parse(Eval("percIPI").ToString()),                                                                                                                                  
                                                                                                                                  Eval("descST").ToString(),
                                                                                                                                  decimal.Parse(Eval("mva").ToString()),
                                                                                                                                  decimal.Parse(Eval("aliquotaInternaICMS").ToString()),
                                                                                                                                  decimal.Parse(Eval("pmc17").ToString()),
                                                                                                                                  decimal.Parse(Eval("precoFabrica").ToString()), 
                                                                                                                                  GetDescontoComercial(decimal.Parse(Eval("descontoComercial").ToString())), 
                                                                                                                                  decimal.Parse(Eval("descontoAdicional").ToString()), 
                                                                                                                                  decimal.Parse(Eval("percRepasse").ToString()),
                                                                                                                                  decimal.Parse(Eval("valorICMSST").ToString()),
                                                                                                                                  decimal.Parse(Eval("reducaoST_MVA").ToString()),
                                                                                                                                  Eval("ufIdOrigem").ToString(),
                                                                                                                                  Eval("exclusivoHospitalar").ToString(),
                                                                                                                                  Eval("listaDescricao").ToString(),
                                                                                                                                  Eval("categoria").ToString(),
                                                                                                                                  int.Parse(Eval("itemCodigoOrigem").ToString()),
                                                                                                                                   Eval("tratamentoICMSEstab").ToString()
                                                                                                                                 )) %>' />
                                                </div>
                                            </div>
                                            <div id="dvMargemPercentual" runat="server" style="display: table; width: 100%; border-left: solid 1px #000000; border-bottom: solid 1px #000000; border-right: solid 1px #000000;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; width: 50%; background-color: #8DB4E3;">
                                                    <asp:Literal ID="ltrMargemPrc" runat="server" Text='<%$ Resources:Resource, lblMargemPrc %>' />
                                                </div>
                                                <div style="display: table-cell; vertical-align: middle; text-align: right; width: 50%; background-color: #8DB4E3;">
                                                    <asp:Literal ID="ltrMargemPrcValor" runat="server" />
                                                </div>
                                            </div>
                                            <div style="display: table; width: 100%;">
                                                <div style="display: table-cell; vertical-align: middle; text-align: left; background-color: transparent; color: transparent;">
                                                    <asp:Literal ID="Literal1" runat="server" Text="T" />
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                        <div style="width: 400px; margin-left: 2px; margin-bottom: 10px;">
                                            <asp:GridView ID="gvSumarizacao" runat="server"
                                                SkinID="gvUniquePage"
                                                Width="378px">
                                                <Columns>
                                                    <asp:BoundField DataField="estabelecimentoId" HeaderText="Estabelecimento" />
                                                    <asp:BoundField DataField="precoVenda" DataFormatString="{0:n2}" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" HeaderText="Preço Venda" />
                                                    <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderText="Margem %">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltrMrg" runat="server" Text='<%# string.Format("{0:n2}%", Eval("valorMargem")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </HeaderTemplate>
                                </asp:DataList>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
            <asp:RequiredFieldValidator ID="rfvItem" runat="server" ControlToValidate="txtItem" Display="None" SetFocusOnError="true" Text="*" ValidationGroup="ValidaItem" />
            <asp:ValidationSummary ID="vsmItem" runat="server" ShowMessageBox="true" ShowSummary="false" ValidationGroup="ValidaItem" />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
