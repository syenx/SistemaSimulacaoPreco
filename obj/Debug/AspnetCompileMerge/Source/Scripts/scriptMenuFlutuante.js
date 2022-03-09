
function abrirPopUp() {
    document.getElementById('ContentPlaceHolder1_upLoading').style.display = 'block';
}


$(document).ready(function () {
    $("#TravaMVA").addClass("active");
    $("#PmcReduzido").removeClass("active");
    $("#convenio").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");

    $("#divTrava").css("display", "block");
    $("#divPMCReduzido").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasGerais").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");
    $("#divConvenio").css("display", "none");
});


$("#PmcReduzido").click(function () {
    $("#PmcReduzido").addClass("active");
    $("#TravaMVA").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");
    $("#convenio").removeClass("active");

    $("#divPMCReduzido").css("display", "block");
    $("#divTrava").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasGerais").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");
    $("#divConvenio").css("display", "none");
});



$("#convenio").click(function () {

    $("#convenio").addClass("active");
    $("#TravaMVA").removeClass("active");
    $("#PmcReduzido").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");

    $("#divConvenio").css("display", "block");
    $("#divTrava").css("display", "none");
    $("#divPMCReduzido").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasGerais").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");

});

$("#TravaMVA").click(function () {

    $("#TravaMVA").addClass("active");
    $("#convenio").removeClass("active");
    $("#PmcReduzido").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");

    $("#divTrava").css("display", "block");
    $("#divPMCReduzido").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasGerais").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");
    $("#divConvenio").css("display", "none");
});

$("#CargaCusto").click(function () {
    $("#CargaCusto").addClass("active");
    $("#TravaMVA").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");
    $("#convenio").removeClass("active");

    $("#divTrava").css("display", "none");
    $("#divCargaCusto").css("display", "block");
    $("#divRegrasGerais").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");
    $("#divConvenio").css("display", "none");
});

$("#RegrasGerais").click(function () {
    $("#RegrasGerais").addClass("active");
    $("#TravaMVA").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");
    $("#convenio").removeClass("active");

    $("#divTrava").css("display", "none");
    $("#divRegrasGerais").css("display", "block");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");
    $("#divConvenio").css("display", "none");
});

$("#RegrasST").click(function () {
    $("#RegrasST").addClass("active");
    $("#TravaMVA").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasPE").removeClass("active");
    $("#convenio").removeClass("active");

    $("#divTrava").css("display", "none");
    $("#divRegrasST").css("display", "block");
    $("#divRegrasGerais").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasPE").css("display", "none");
    $("#divConvenio").css("display", "none");
});

$("#RegrasPE").click(function () {
    $("#RegrasPE").addClass("active");
    $("#TravaMVA").removeClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasGerais").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#convenio").removeClass("active");

    $("#divTrava").css("display", "none");
    $("#divRegrasPE").css("display", "block");
    $("#divRegrasGerais").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divConvenio").css("display", "none");
});

function bs_input_file() {
    $(".input-file").before(
        function () {
            if (!$(this).prev().hasClass('input-ghost')) {
                var element = $("<input type='file' class='input-ghost' style='visibility:hidden; height:0'>");
                element.attr("name", $(this).attr("name"));
                element.change(function () {
                    element.next(element).find('input').val((element.val()).split('\\').pop());
                });
                $(this).find("button.btn-choose").click(function () {
                    element.click();
                });
                $(this).find("button.btn-reset").click(function () {
                    element.val(null);
                    $(this).parents(".input-file").find('input').val('');
                });
                $(this).find('input').css("cursor", "pointer");
                $(this).find('input').mousedown(function () {
                    $(this).parents('.input-file').prev().click();
                    return false;
                });
                return element;
            }
        }
    );
}

$(function () {
    bs_input_file();
});


$("#imagemMenuSuperior").click(function () {
    var username = '<%= Session["TipoPerfil"].ToString() %>'.toUpperCase();

    if (username === "TRUE") {
        openNav();
    }
});

function openNav() {
    document.getElementById("mySidenav").style.width = "150px";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}



function manterSelecionadoRegrasGerais() {
    $("#convenio").addClass("active");
    $("#RegrasGerais").addClass("active");
    $("#CargaCusto").removeClass("active");
    $("#RegrasST").removeClass("active");
    $("#RegrasPE").removeClass("active");

    $("#divRegrasGerais").css("display", "block");
    $("#divConvenio").css("display", "none");
    $("#divCargaCusto").css("display", "none");
    $("#divRegrasST").css("display", "none");
    $("#divRegrasPE").css("display", "none");
};