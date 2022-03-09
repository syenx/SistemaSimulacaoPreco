<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster/KraftMaster.Master" AutoEventWireup="true" CodeBehind="CargasSimulador.aspx.cs" Inherits="KS.SimuladorPrecos.AppPaginas.Consulta.CargasSimulador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../../Styles/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>

    <script type="text/javascript" src="../../Scripts/bootstrap.min.js"></script>
    <link href="../../Styles/bootstrap.css" rel="stylesheet" />

    <script type="text/javascript" src="https://cdn.jsdelivr.net/jquery/latest/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/momentjs/latest/moment.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css" />
    <link type="text/css" href="../../Styles/MenuFlutuante.css" rel="stylesheet" />

    <script type="text/javascript" src="../../Scripts/popper.min.js"></script>
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />

    <style type="text/css">
        .GridView th, .GridView th:hover, .GridViewDif th, .GridViewDif th:hover {
            background-color: #a5a9b1;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #D9DEE4">

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

    <ul class="nav nav-tabs">
        <li id="CargaCusto" class="active"><a>Carga Custos</a></li>
        <li id="TravaMVA"><a style="cursor: pointer">Trava MVA</a></li>
        <li id="RegrasGerais"><a style="cursor: pointer">Carga Regras Gerais</a></li>
        <li id="RegrasST"><a style="cursor: pointer">Regras ST</a></li>
        <li id="RegrasPE"><a style="cursor: pointer">Regras ICMS(PE)</a></li>
        <li id="convenio"><a style="cursor: pointer">Convênio</a></li>
    </ul>

    <div id="divTrava" class="divStylles ajustePainelDesabilitado" style="display: block;">
        <div class="card-header-tabs ">
            <asp:UpdatePanel ID="uppTravaPrincipal" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCargaTrava" />
                </Triggers>
                <ContentTemplate>
                    <asp:UpdateProgress ID="UpdateProgressTravaPrincipal" runat="server" AssociatedUpdatePanelID="uppTravaPrincipal">
                        <ProgressTemplate>
                            <div class="ProgressBackGround"></div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <div class="container card shadow p-3 mb-5 bg-white rounded">
                        <h3>Trava</h3>
                        <div style="float: right">
                            <asp:Button runat="server" SkinID="buttonCargas" ID="btnCargaTrava" OnClientClick="return abrirPopUp()" OnClick="btnCargaTrava_Click" Text="Carregar" />
                            <asp:FileUpload ID="fullTrava" CssClass="custom-file-input" text="Trava" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="container card-deck shadow p-3 mb-5 bg-white rounded" style="display: none">
                <div class="container card p-1">
                    <h5>CATEGORIA: </h5>
                    <input name="txtCategoria" class="form-control" />
                </div>
                <div class="container card p-1">
                    <h5>LISTA: </h5>
                    <input name="txtLista" class="form-control" />
                </div>

                <asp:Button runat="server" SkinID="ButtonBootStrap" Text="Pesquisar" ID="btnFiltroTravaPrincipal" BorderStyle="Solid" OnClick="btnFiltroTravaPrincipal_Click" />
            </div>
            <div class="container card shadow p-3 mb-5 bg-white rounded">
                <asp:UpdatePanel ID="upTravaPrincipal" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnFiltroTravaPrincipal" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:GridView ID="gvTrava" runat="server"
                            CssClass="table table-borderless table-striped"
                            DataKeyNames="categoria ,lista ,trava ,mva_st,usuarioId,dataAtualizacao"
                            OnRowDataBound="gvTrava_RowDataBound"
                            OnRowCommand="gvTrava_RowCommand"
                            OnPageIndexChanging="gvTrava_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="categoria" HeaderText="CATEGORIA" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="lista" HeaderText="LISTA" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="trava" HeaderText="TRAVA" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="mva_st" HeaderText="MVA - ST" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="mva_ts_reduzido" HeaderText="REDUTOR" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="dataAtualizacao" HeaderText="DATA ATUALIZAÇÃO" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="usuarioId" HeaderText="Usuario" ItemStyle-HorizontalAlign="Center" />

                            </Columns>
                            <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div id="divCargaCusto" class="divStylles ajustePainelDesabilitado">
        <div class="card-header-tabs ">
            <asp:UpdatePanel ID="uppSimuladorImportacaoCusto" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCarregarCustos" />
                </Triggers>
                <ContentTemplate>
                    <asp:UpdateProgress ID="upLoading" runat="server" AssociatedUpdatePanelID="uppSimuladorImportacaoCusto">
                        <ProgressTemplate>
                            <div class="ProgressBackGround"></div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <div class="container card shadow p-3 mb-5 bg-white rounded">
                        <h3>CARGA CUSTO</h3>
                        <div style="float: right">
                            <asp:Button runat="server" SkinID="buttonCargas" ID="btnCarregarCustos" OnClientClick="return abrirPopUp()" OnClick="btnCarregarCustos_Click" Text="Carregar" />
                            <asp:FileUpload ID="fulArquivoCustos" CssClass="custom-file-input" text="Custos" runat="server" />
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="container card-deck shadow p-3 mb-5 bg-white rounded" style="display: none">
                <div class="container card p-1">
                    <h5>Usuario: </h5>
                    <input name="txtUsuarioCusto" class="form-control" />
                </div>
                <div class="container card p-1">
                    <h5>De - Até : </h5>
                    <input name="txtDeAteCusto" readonly="readonly" class="form-control" />
                </div>
                <asp:Button runat="server" SkinID="ButtonBootStrap" Text="Pesquisar" ID="btnFiltroCusto" BorderStyle="Solid" OnClick="btnFiltroCusto_Click" />
            </div>
            <div class="container card shadow p-3 mb-5 bg-white rounded">
                <asp:UpdatePanel ID="updCusto" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnFiltroCusto" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:GridView ID="gvTabelaCusto" runat="server"
                            CssClass="table table-borderless table-striped"
                            DataKeyNames="DataAlteracao,usuarioId, pathArquivo,NomeArquivo"
                            OnRowDataBound="gvTabelaCusto_RowDataBound"
                            OnRowCommand="gvTabelaCusto_RowCommand"
                            OnPageIndexChanging="gvTabelaCusto_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="NomeArquivo" HeaderText="Nome Arquivo" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="DataAlteracao" HeaderText="Data Alteração" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="usuarioId" HeaderText="Usuario" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                            <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div id="divRegrasGerais" class="divStylles ajustePainelDesabilitado">
        <div class="card-header-tabs">
            <asp:UpdatePanel ID="uppSimuladorImportacaoGerais" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCarregaRegrasGerais" />
                </Triggers>
                <ContentTemplate>
                    <div class="container card shadow p-3 mb-5 bg-white rounded">
                        <h3>REGRAS GERAIS</h3>
                        <div style="float: right">
                            <asp:Button runat="server" SkinID="buttonCargas" ID="btnCarregaRegrasGerais" OnClientClick="return abrirPopUp()" OnClick="btnCarregaRegrasGerais_Click" Text="Carregar" />
                            <asp:FileUpload ID="fullArquivosRegrasGerais" CssClass="input-ghost" text="Custos" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>


            <div class="card-deck m-md-5  shadow p-3 mb-5 bg-white rounded" style="display: none">
                <div class="container card p-1">
                    <h5>De - Até : </h5>
                    <input name="txtDeAteRegrasGerais" readonly="readonly" class="form-control" />
                </div>
                <div class="container card  p-1">
                    <h5>Usuario : </h5>
                    <asp:TextBox runat="server" ID="txtUsuarioRegrasGerais" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <div class="container card  p-1">
                    <h5>Nome Arquivo : </h5>
                    <asp:TextBox runat="server" ID="txtNomeArquivoRegras" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <div class="container card  p-1">
                    <h5>Estabelecimento : </h5>
                    <asp:TextBox runat="server" ID="txtEstabelecimentoRegras" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <asp:Button runat="server" SkinID="ButtonBootStrap" Text="Pesquisar" ID="btnFiltroRegras" BorderStyle="Solid" OnClick="btnFiltroRegras_Click" />
            </div>


            <asp:UpdatePanel ID="updRegras" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnFiltroRegras" />
                </Triggers>
                <ContentTemplate>

                    <div class="container card shadow p-2 mb-5 bg-white rounded">
                        <asp:GridView ID="gvTabelaRegra" runat="server"
                            CssClass="table table-hover table-striped"
                            DataKeyNames="usuarioId,pathArquivo,NomeArquivo,dataAtualizacao"
                            OnRowDataBound="gvTabelaRegra_RowDataBound"
                            OnRowCommand="gvTabelaRegra_RowCommand"
                            OnPageIndexChanging="gvTabelaRegra_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="NomeArquivo" HeaderText="Nome Arquivo" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="pathArquivo" HeaderText="Caminho Arquivo" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="dataAtualizacao" HeaderText="Data Atualização" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="usuarioId" HeaderText="Usuario" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                            <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div id="divRegrasST" class="divStylles ajustePainelDesabilitado">
        <div class="card-header-tabs">

            <asp:UpdatePanel ID="uppSimuladorImportacaoRegrasST" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCarregarRegrasST" />
                </Triggers>
                <ContentTemplate>

                    <asp:UpdateProgress ID="UpdateProgressBarST" runat="server" AssociatedUpdatePanelID="uppSimuladorImportacaoRegrasST">
                        <ProgressTemplate>
                            <div class="ProgressBackGround"></div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <div class="container card shadow p-3 mb-5 bg-white rounded">
                        <h3>CARGA REGRAS ST</h3>
                        <div style="float: right">
                            <asp:Button runat="server" SkinID="buttonCargas" ID="btnCarregarRegrasST" OnClientClick="return abrirPopUp()" OnClick="btnCarregarRegrasST_Click" Text="Carregar" />
                            <asp:FileUpload ID="fullRegrasST" CssClass="input-ghost" text="Custos" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="card-deck m-md-5  shadow p-3 mb-5 bg-white rounded" style="display: none">
                <div class="container card p-1">
                    <h5>De - Até : </h5>
                    <input name="txtDeAteRegrasST" readonly="readonly" class="form-control" />
                </div>
                <div class="container card  p-1">
                    <h5>ItemID : </h5>
                    <asp:TextBox runat="server" ID="txtItemIDoRegrasST" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <div class="container card  p-1">
                    <h5>Estabelecimento : </h5>
                    <asp:TextBox runat="server" ID="txtEstabelecimentooRegrasST" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <asp:Button runat="server" SkinID="ButtonBootStrap" Text="Pesquisar" ID="btnFiltroRegrasST" BorderStyle="Solid" OnClick="btnFiltroRegrasST_Click" />
            </div>



            <div class="container card shadow p-2 mb-5 bg-white rounded">
                <asp:GridView ID="gridRegrasST" runat="server"
                    CssClass="table table-bordered table-striped table-hover"
                    DataKeyNames="estabelecimentoId,itemId,classeFiscal,perfilCliente,estadoDestino,aliquota,PMC,icmsInterno,dataImportacao,usuarioId,PMC_CHEIO"
                    OnRowDataBound="gridRegrasST_RowDataBound"
                    OnRowCommand="gridRegrasST_RowCommand"
                    OnPageIndexChanging="gridRegrasST_PageIndexChanging"
                    AutoGenerateColumns="False"
                    AllowPaging="True">
                    <Columns>
                        <asp:BoundField HeaderText="Estabelecimento" DataField="estabelecimentoId" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px" />
                        <asp:BoundField HeaderText="ItemId" DataField="itemId" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Classe Fiscal" DataField="classeFiscal" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Perfil Cliente" DataField="perfilCliente" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Estado Destino" DataField="estadoDestino" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="PMC" DataField="PMC" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="ICMS S/ Venda" DataField="icmsInterno" ItemStyle-HorizontalAlign="Center" />

                        <asp:BoundField HeaderText="PMC-CHEIO" DataField="PMC_CHEIO" ItemStyle-HorizontalAlign="Center" />

                        <asp:BoundField HeaderText="Data Importacão" DataField="dataImportacao" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Usuario" DataField="usuarioId" ItemStyle-HorizontalAlign="Center" />

                    </Columns>
                    <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <div id="divRegrasPE" class="divStylles ajustePainelDesabilitado">
        <div class="card-header-tabs">
            <asp:UpdatePanel ID="uppSimuladorImportacaoRegrasPE" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCarregarRegrasST" />
                    <asp:PostBackTrigger ControlID="btnCarregarRegrasPE" />
                </Triggers>
                <ContentTemplate>

                    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="uppSimuladorImportacaoRegrasPE">
                        <ProgressTemplate>
                            <div class="ProgressBackGround"></div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <div class="container card shadow p-3 mb-5 bg-white rounded">
                        <h3>CARGA REGRAS PE</h3>
                        <div style="float: right">
                            <asp:Button runat="server" SkinID="buttonCargas" ID="btnCarregarRegrasPE" OnClientClick="return abrirPopUp()" OnClick="btnCarregarRegrasPE_Click" Text="Carregar" />
                            <asp:FileUpload ID="fullRegrasPE" CssClass="input-ghost" text="Custos" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="card-deck m-md-5  shadow p-3 mb-5 bg-white rounded" style="display: none">
                <div class="container card p-1">
                    <h5>De - Até : </h5>
                    <input name="txtDeAteRegrasPE" readonly="readonly" class="form-control" />
                </div>
                <div class="container card  p-1">
                    <h5>ItemID : </h5>
                    <asp:TextBox runat="server" ID="txtxItemIDRegrasPE" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <div class="container card  p-1">
                    <h5>Estabelecimento : </h5>
                    <asp:TextBox runat="server" ID="txtEstabelecimentoRegrasPe" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <div class="container card  p-1">
                    <h5>Usuario : </h5>
                    <asp:TextBox runat="server" ID="txtUsuarioRegrasPE" SkinID="InputBootstrap"></asp:TextBox>
                </div>
                <asp:Button runat="server" SkinID="ButtonBootStrap" Text="Pesquisar" ID="btnFiltroRegrasPE" BorderStyle="Solid" OnClick="btnFiltroRegrasPE_Click" />
            </div>

            <div class="container card shadow p-2 mb-5 bg-white rounded">
                <asp:GridView ID="gvEncargos" runat="server"
                    CssClass="table table-bordered table-striped table-hover"
                    DataKeyNames="estabelecimentoId,codigoItem,ufOrigemFornec,ufDestinoCliente,contribuinte,encargos,dataImportacao,usuarioId"
                    OnRowDataBound="gvEncargos_RowDataBound"
                    OnRowCommand="gvEncargos_RowCommand"
                    OnPageIndexChanging="gvEncargos_PageIndexChanging"
                    AutoGenerateColumns="False"
                    AllowPaging="True">
                    <Columns>
                        <asp:BoundField HeaderText="Estabelecimento" DataField="estabelecimentoId" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px" />
                        <asp:BoundField HeaderText="ItemId" DataField="codigoItem" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Uf Origem Fornec" DataField="ufOrigemFornec" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Uf Destino Fornec" DataField="ufDestinoCliente" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Contribuinte" DataField="contribuinte" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Encargos" DataField="encargos" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Data Importacão" DataField="dataImportacao" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Usuario" DataField="usuarioId" ItemStyle-HorizontalAlign="Center" />

                    </Columns>
                    <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <div id="divConvenio" class="divStylles ajustePainelDesabilitado" style="display: block;">
        <div class="card-header-tabs ">
            <asp:UpdatePanel ID="uppConvenio" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnConvenio" />
                </Triggers>
                <ContentTemplate>
                    <asp:UpdateProgress ID="UpdateConvenio" runat="server" AssociatedUpdatePanelID="uppConvenio">
                        <ProgressTemplate>
                            <div class="ProgressBackGround"></div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <div class="container card shadow p-3 mb-5 bg-white rounded">
                        <h3>Convênio</h3>
                        <div style="float: right">
                            <asp:Button runat="server" SkinID="buttonCargas" ID="btnConvenio" OnClientClick="return abrirPopUp()" OnClick="btnConvenio_Click" Text="Carregar" />
                            <asp:FileUpload ID="fullConvenio" CssClass="custom-file-input" text="Convenio" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="container card-deck shadow p-3 mb-5 bg-white rounded" style="display: none">
                <div class="container card p-1">
                    <h5>CATEGORIA: </h5>
                    <input name="txtCategoria" class="form-control" />
                </div>
                <div class="container card p-1">
                    <h5>LISTA: </h5>
                    <input name="txtLista" class="form-control" />
                </div>

                <asp:Button runat="server" SkinID="ButtonBootStrap" Text="Pesquisar" ID="btnFiltroConvenio" BorderStyle="Solid" OnClick="btnFiltroConvenio_Click" />
            </div>
            <div class="container card shadow p-3 mb-5 bg-white rounded">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnFiltroConvenio" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:GridView ID="gvConvenio" runat="server"
                            CssClass="table table-borderless table-striped"
                            DataKeyNames="usuario ,dataAtualizacao,nomeArquivo"
                            OnRowDataBound="gvConvenio_RowDataBound"
                            OnRowCommand="gvConvenio_RowCommand"
                            OnPageIndexChanging="gvConvenio_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="usuario" HeaderText="usuario" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="dataAtualizacao" HeaderText="dataAtualizacao" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="nomeArquivo" HeaderText="nomeArquivo" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                            <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <script type="text/javascript" src="../../Scripts/KSSimuladorPrecosScripts.js"></script>
    <script type="text/javascript" src="../../Scripts/scriptMenuFlutuante.js"></script>
    <script type="text/javascript" src="../../Scripts/dataPikers.js"></script>




</asp:Content>

