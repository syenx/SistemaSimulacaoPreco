
//Apresenta um ToolTip com o Texto Sugerido
function fnShowToolTip(strText, obj)
{	
    if(strText != "")
    {
	    //spToolTip.style.display = "";
	    document.getElementById(obj).style.display = "";
	    var xHtml = "<p class='toolTip'>" + strText + "</p>";
	    //window.spToolTip.innerHTML = xHtml;
	    document.getElementById(obj).innerHTML = xHtml;
    			
	    //spToolTip.style.left = document.body.scrollLeft + event.x + 2;
	    //spToolTip.style.top = document.body.scrollTop + event.y + 2;
	    document.getElementById(obj).style.left = document.body.scrollLeft + event.x + 2;
	    document.getElementById(obj).style.top  = document.body.scrollTop + event.y + 2;
    }
    else{
	    //spToolTip.style.display = "none";
	    document.getElementById(obj).style.display = "none";
	    }
}

//Desabilita a Visualização do ToolTip
function fnCloseToolTip(obj)
{
	//spToolTip.style.display = "none";
	document.getElementById(obj).style.display = "none";
} 

//Formata Borda das Caixas de Texto
function TextBoxBorder(obj, opc)
{
    if(opc == "1")
        obj.style.borderColor = "#2F8C46";
    else
        obj.style.borderColor = "#3CB453";
}

//TextBox Recebe Somente Números - funciona nos Browsers IE, FireFox e Chrome
function OnlyNumber(e) {
    var rValue = new RegExp(/^[0-9]/);
    
    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if(window.event){ event.returnValue = false }
        else return false;
}

//Mascara de Data
function DateMask(obj, e) 
{
    var rValue = new RegExp(/^[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if (window.event) { event.returnValue = false }
    else return false;

    if (obj.value.length < obj.maxLength) {
        if (rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
            if (obj.value.length == 2) {
                obj.value += '/' + String.fromCharCode(window.event ? e.keyCode : e.which);
            }
            else {
                if (obj.value.length == 5) {
                    obj.value += '/' + String.fromCharCode(window.event ? e.keyCode : e.which);
                }
                else {
                    obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                }
            }
        }
    }

    if (window.event) { event.returnValue = false }
    else return false;
}

//Valida a Data Informada
function fnValidaData(obj)
{
    var rValue = new RegExp(/^((0?[1-9]|[12]\d)\/(0?[1-9]|1[0-2])|30\/(0?[13-9]|1[0-2])|31\/(0?[13578]|1[02]))\/(19|20)?\d{2}$/);
    
    if(obj.value != "")
    {    
        if(!(rValue.test(obj.value)))
        {
            obj.value = "";
            event.returnValue = false;
        }
    }    
}

//Formatação de CPF
function FormataCpf(campo, teclapres)
{
    var rValue = new RegExp(/^[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? teclapres.keyCode : teclapres.which))) &&
	        (window.event ? teclapres.keyCode : teclapres.which) != 8)
        if (window.event) { teclapres.returnValue = false }
    else return false;

	var tecla = teclapres.keyCode;
	var vr = new String(campo.value);
	vr = vr.replace(".", "");
	vr = vr.replace("/", "");
	vr = vr.replace("-", "");
	tam = vr.length + 1;
	if (tecla != 14)
	{
		if (tam == 4)
			campo.value = vr.substr(0, 3) + '.';
		if (tam == 7)
			campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 6) + '.';
		if (tam == 11)
			campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(7, 3) + '-' + vr.substr(11, 2);
	}
}

//Formata a Digitação de CNPJ
function CNPJFormat(campo, teclapres) 
{
    var rValue = new RegExp(/^[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? teclapres.keyCode : teclapres.which))) &&
	        (window.event ? teclapres.keyCode : teclapres.which) != 8)
        if (window.event) { teclapres.returnValue = false }
    else return false;
    
	var tecla = teclapres.keyCode;
	var vr = new String(campo.value);
	vr = vr.replace(".", "");
	vr = vr.replace("/", "");
	vr = vr.replace("-", "");
	tam = vr.length + 1;
	if (tecla != 14)
	{
		if (tam == 3)
			campo.value = vr.substr(0, 2) + '.';
		if (tam == 6)
			campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 5) + '.';
		if (tam == 10)
			campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(6, 3) + '/';
		if (tam == 15)
			campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(6, 3) + '/' + vr.substr(9, 4) + '-' + vr.substr(13, 2);
	}
}

//Primeira Letra Sempre Maiúscula
function FirstLetter(obj, e)
{
    var rValue = new RegExp(/[a-zA-Z0-9\s]/);

    var sVlr1 = obj.value + String.fromCharCode(window.event ? e.keyCode : e.which);

    if (sVlr1.length > obj.maxLength) {
        if (window.event) { event.returnValue = false }
        else return false;
        return false;
    }

    if (rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
        if (sVlr1.length == 1) {
            var sVlr = String.fromCharCode(window.event ? e.keyCode : e.which);
            obj.value += sVlr.toUpperCase();
            if (window.event) { event.returnValue = false }
            else return false;
        }

        var valor = sVlr1.charAt(sVlr1.length - 2);
        var rTst = new RegExp(/\s/);

        if (rTst.test(valor)) {
            var s = String.fromCharCode(window.event ? e.keyCode : e.which);

            if (!(rTst.test(s))) {
                obj.value += s.toUpperCase();
                if (window.event) { event.returnValue = false }
                else return false;                
            }
        }
        else {
            if (sVlr1.length > 1) {
                var sV = String.fromCharCode(window.event ? e.keyCode : e.which);
                obj.value += sV.toLowerCase();
                if (window.event) { event.returnValue = false }
                else return false;
            }
        }
    }
} 

//Formata Telefone
function formatTelefone(obj, e) 
{
    var rValue = new RegExp(/^[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if (window.event) { event.returnValue = false }
        else return false;

    if (obj.value.length == 4)
        obj.value += "-";
}

///Formata CEP
function FormataCEP(obj, e) 
{
    var rValue = new RegExp(/^[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if (window.event) { event.returnValue = false }
        else return false;
    
    if (obj.value.length == 5)
        obj.value += "-";  
}

//Verifica Maxlength
function CheckMaxLength(obj, MaxLength) {
    if (obj.value.length >= MaxLength)
        if (window.event) { event.returnValue = false }
    else return false;
}

///Abre um PopUp
function fnOpenPopUp(url, name)
{
    window.open(url, name, "menubar=no, toolbar=yes, status=yes, location=no, resizable=yes, scrollbars=yes, dependent=yes, titlebar=no").focus();
}

///Abre um PopUp para Exportar
function fnOpenPopUpToExport(url, name) {
    window.open(url, name, "width=100, height=100, menubar=no, toolbar=yes, status=yes, location=no, resizable=yes, scrollbars=yes, dependent=yes, titlebar=no").focus();
}

//BLOQUEAR BOTÃO DIREITO DO MOUSE
/*function verfonte(e)
{
    var strcopy = "Copyright (c) ";
	var lngano1 = "2011";
	var lngano2 = "";
	var mydate = new Date();
	lngano2 = mydate.getFullYear();
	strcopy = strcopy + lngano1;
	if (lngano2 > lngano1)
		{
			strcopy = strcopy + "-" + lngano2;
		}
	strcopy = strcopy + " :: SEAL Tecnologia - Departamento de Software.\nTodos os Direitos Reservados.";
	if(!document.all)	//NS
		{
			if (e.button==2){window.alert(strcopy);}
		}
	else				//IE
		{
			if (event.button==2){window.alert(strcopy);}
		}
}*/
	
//EVENTO PARA BLOQUEAR O BOTAO DIREITO DO MOUSE
//document.onmousedown=verfonte

//Desmarca Check Pai
function CheckUncheckFather(id, gvId, coluna) {
    //get reference of GridView control
    var grid = document.getElementById(gvId);
    //variable to contain the cell of the grid
    var cell;
    var counter = 0;

    if (grid.rows.length > 0) {
        //loop starts from 1. rows[0] points to the header.
        for (i = 1; i < grid.rows.length; i++) {
            //get the reference of column
            cell = grid.rows[i].cells[coluna];

            if (cell == undefined)
                continue;

            //loop according to the number of childNodes in the cell
            for (j = 0; j < cell.childNodes.length; j++) {
                //if childNode type is CheckBox                 
                if (cell.childNodes[j].type == "checkbox") {
                    //assign the status of the Select All checkbox to the cell checkbox within the grid
                    if (!(cell.childNodes[j].checked)) {
                        ++counter;
                    }
                }
            }
        }
    }

    document.getElementById(id).checked = counter > 0 ? false : true;
}

// Verifica se foram selecionados checks na Grid
function VerifyIfCheckInGrid(gvId, coluna, msg) {
    var grid = document.getElementById(gvId);
    var cell;
    var counter = 0;

    if (grid.rows.length > 0) {
        for (i = 1; i < grid.rows.length; i++) {
            cell = grid.rows[i].cells[coluna];

            for (j = 0; j < cell.childNodes.length; j++) {
                if (cell.childNodes[j].type == "checkbox") {
                    if (cell.childNodes[j].checked) {
                        ++counter;
                    }
                }
            }
        }
    }

    if (counter == 0) {
        window.alert(msg);

        if (window.event) {
            event.returnValue = false;
        } else return false;
    }
}

//Seleciona Todos os Checks da Grid
function SelectAll(id, gvId, coluna) {
    //get reference of GridView control
    var grid = document.getElementById(gvId);
    //variable to contain the cell of the grid
    var cell;

    if (grid.rows.length > 0) {
        //loop starts from 1. rows[0] points to the header.
        for (i = 1; i < grid.rows.length; i++) {
            //get the reference of column
            cell = grid.rows[i].cells[coluna];

            if (cell == undefined)
                continue;

            //loop according to the number of childNodes in the cell
            for (j = 0; j < cell.childNodes.length; j++) {
                //if childNode type is CheckBox                 
                if (cell.childNodes[j].type == "checkbox") {
                    //assign the status of the Select All checkbox to the cell checkbox within the grid
                    cell.childNodes[j].checked = document.getElementById(id).checked;
                }
            }
        }
    }
}

//Seleciona todos os checkboxes do CheckBoxList
function CheckBoxListCheckAll(chkTodos, cbControl) {

    var chkBoxList = document.getElementById(cbControl);
    var chkBoxCount = chkBoxList.getElementsByTagName("input");

    for (var i = 0; i < chkBoxCount.length; i++) {
        chkBoxCount[i].checked = document.getElementById(chkTodos).checked;
    }

    return false;
}

//Desmarca o CheckBox Pai
function CheckBoxListUnCheckFather(chkTodos, cbControl) {

    var chkBoxList = document.getElementById(cbControl);
    var chkBoxCount = chkBoxList.getElementsByTagName("input");
    var counter = 0;

    for (var i = 0; i < chkBoxCount.length; i++) {
        if (!(chkBoxCount[i].checked))
            ++counter;
    }
    
    document.getElementById(chkTodos).checked = counter > 0 ? false : true;
    return false;
}

//Seleciona todos os checkboxes do CheckBoxList
function CheckBoxListCheckAllWithGrid(chkTodos, cbControl, gvId) {

    var chkBoxList = document.getElementById(cbControl);
    var chkBoxCount = chkBoxList.getElementsByTagName("input");
    var counter = 0;

    for (var i = 0; i < chkBoxCount.length; i++) {
        chkBoxCount[i].checked = document.getElementById(chkTodos).checked;
        if (!(chkBoxCount[i].checked))
            ++counter;
    }

    if (counter == chkBoxCount.length) {
        chkBoxCount[0].checked = true; //Coluna Produtor
        chkBoxCount[1].checked = true; //Coluna Propriedade
        chkBoxCount[6].checked = true; //Coluna Sexo
        chkBoxCount[7].checked = true; //Coluna Raça
    }

    if (navigator.appName == "Microsoft Internet Explorer") {
        //Funciona corretamente só no IE
        //get reference of GridView control
        var grid = document.getElementById(gvId);
        //variable to contain the cell of the grid
        var cell;

        var colunas = new Array();
        colunas[0] = "";
        colunas[1] = chkBoxCount[0].id; //Coluna Produtor
        colunas[2] = chkBoxCount[1].id; //Coluna Propriedade
        colunas[3] = chkBoxCount[2].id; //Coluna Data de Entrada na Propriedade
        colunas[4] = chkBoxCount[3].id; //Coluna Propriedade Anterior
        colunas[5] = chkBoxCount[4].id; //Coluna Curral de Manejo
        colunas[6] = chkBoxCount[5].id; //Coluna Número Monitor
        colunas[7] = chkBoxCount[6].id; //Coluna Sexo
        colunas[8] = chkBoxCount[7].id; //Coluna Raça
        colunas[9] = chkBoxCount[8].id; //Coluna Data de Nascimento
        colunas[10] = chkBoxCount[9].id; //Coluna Data da Última Pesagem
        colunas[11] = chkBoxCount[10].id; //Coluna Peso Atual

        if (grid.rows.length > 0) {
            //loop starts from 1. rows[0] points to the header.
            for (i = 0; i < grid.rows.length; i++) {
                for (y = 0; y < grid.rows[i].cells.length; y++) {
                    for (z = 0; z < chkBoxCount.length; z++) {
                        //for (a = 0; a < colunas.length; a++) {
                        if (chkBoxCount[z].id == colunas[y])
                            grid.rows[i].cells[y].style.display = chkBoxCount[z].checked ? "block" : "none";
                        //}
                    }
                }
            }
        }
    }

    return false;
}

//Desmarca o CheckBox Pai
function CheckBoxListUnCheckFatherWithGrid(chkTodos, cbControl, gvId) {

    var chkBoxList = document.getElementById(cbControl);
    var chkBoxCount = chkBoxList.getElementsByTagName("input");
    var counter = 0;

    for (var i = 0; i < chkBoxCount.length; i++) {
        if (!(chkBoxCount[i].checked))
            ++counter;
    }

    document.getElementById(chkTodos).checked = counter > 0 ? false : true;

    if (counter == chkBoxCount.length) {
        chkBoxCount[0].checked = true; //Coluna Produtor
        chkBoxCount[1].checked = true; //Coluna Propriedade
        chkBoxCount[6].checked = true; //Coluna Sexo
        chkBoxCount[7].checked = true; //Coluna Raça
    }

    if (navigator.appName == "Microsoft Internet Explorer") {
        // Funciona Corretamente só no IE
        //get reference of GridView control
        var grid = document.getElementById(gvId);
        //variable to contain the cell of the grid
        var cell;

        var colunas = new Array();
        colunas[0] = "";
        colunas[1] = chkBoxCount[0].id; //Coluna Produtor
        colunas[2] = chkBoxCount[1].id; //Coluna Propriedade
        colunas[3] = chkBoxCount[2].id; //Coluna Data de Entrada na Propriedade
        colunas[4] = chkBoxCount[3].id; //Coluna Propriedade Anterior
        colunas[5] = chkBoxCount[4].id; //Coluna Curral de Manejo
        colunas[6] = chkBoxCount[5].id; //Coluna Número Monitor
        colunas[7] = chkBoxCount[6].id; //Coluna Sexo
        colunas[8] = chkBoxCount[7].id; //Coluna Raça
        colunas[9] = chkBoxCount[8].id; //Coluna Data de Nascimento
        colunas[10] = chkBoxCount[9].id; //Coluna Data da Última Pesagem
        colunas[11] = chkBoxCount[10].id; //Coluna Peso Atual

        if (grid.rows.length > 0) {
            //loop starts from 1. rows[0] points to the header.
            for (i = 0; i < grid.rows.length; i++) {
                for (y = 0; y < grid.rows[i].cells.length; y++) {
                    for (z = 0; z < chkBoxCount.length; z++) {
                        //for (a = 0; a < colunas.length; a++) {
                        if (chkBoxCount[z].id == colunas[y])
                            grid.rows[i].cells[y].style.display = chkBoxCount[z].checked ? "block" : "none";
                        //}
                    }
                }
            }
        }
    }

    return false;
}

//Envia o foco para o próximo elemento do formulário
function JumpNext(maxLength, value, tabIndex, form) {
    if (document.getElementById(value).value.length == maxLength) {
        tabIndex++;
        for (var i = 0; i < document.getElementById(form).length; i++) {
            if (document.getElementById(form).elements[i].tabIndex == tabIndex) {
                if (document.getElementById(form).elements[i].disabled == true ||
                    document.getElementById(form).elements[i].style.display == "none") {
                    ++tabIndex;
                    continue;
                }
                else
                    document.getElementById(form).elements[i].focus();
            }
        }
    }
}

//Mascara de Hora
function HourMask(obj, e) {
    var rValue = new RegExp(/^[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if (window.event) { event.returnValue = false }
        else return false;                 

        if (obj.value.length < obj.maxLength) {
            if (rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
                if (obj.value.length == 0) {
                    var v1 = new RegExp(/^[0-2]/);

                    if (!v1.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
                        if (window.event) { event.returnValue = false }
                        else return false;
                    }
                    else {
                        obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                    }
                }
                else if (obj.value.length == 1) {
                    var v2 = new RegExp(/^[0-3]/);
                    var vOne = new RegExp(/^[0-1]/);
                    var vTwo = new RegExp(/^[0-9]/);

                    if (vOne.test(obj.value)) {
                        if (!vTwo.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
                            if (window.event) { event.returnValue = false }
                            else return false;
                        }
                        else {
                            obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                        }
                    }
                    else if (!v2.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
                        if (window.event) { event.returnValue = false }
                        else return false;
                    }
                    else {
                        obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                    }
                }
                else if (obj.value.length == 2) {
                    var v3 = new RegExp(/^[0-5]/);
                                
                    if (!v3.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
                        if (window.event) { event.returnValue = false }
                        else return false;
                    }
                    else {
                        obj.value += ':' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    }
                }
                else if (obj.value.length == 3) {
                    var v3 = new RegExp(/^[0-5]/);

                    if (!v3.test(String.fromCharCode(window.event ? e.keyCode : e.which))) {
                        if (window.event) { event.returnValue = false }
                        else return false;
                    }
                    else {
                        obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                    }
                }
                else {
                    obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                }
            }
        }

    if (window.event) { event.returnValue = false }
    else return false;
}

//Valida Hora
function fnValidaHora(obj, e) {
    var rValue = new RegExp(/^([0-2]{1})([0-9]{1}):([0-5]{1})([0-9]{1})/);

    if (!(rValue.test(obj.value))) {
        obj.value = "";
        event.returnValue = false;
    }
}

//Set Scripts a Todos os TextBoxes
function SetScriptsForAllTextBoxesInGrid(id, gvId, column) {
    //get reference of GridView control
    var grid = document.getElementById(gvId);
    //variable to contain the cell of the grid
    var cell, cell2;

    if (grid.rows.length > 0) {
        //loop starts from 1. rows[0] points to the header.
        for (i = 1; i < grid.rows.length - 1; i++) {
            //get the reference of first column
            cell = grid.rows[i].cells[column];
            cell2 = grid.rows[i].cells[0];

            var check = false;
            //loop according to the number of childNodes in the cell
            for (j = 0; j < cell2.childNodes.length; j++) {
                //if childNode type is TextBox
                if (cell2.childNodes[j].type == "checkbox") {
                    check = cell2.childNodes[j].checked;
                }
            }

            //loop according to the number of childNodes in the cell
            for (j = 0; j < cell.childNodes.length; j++) {
                //if childNode type is TextBox
                if (cell.childNodes[j].type == "text") {
                    if (check) {
                        cell.childNodes[j].value = document.getElementById(id).value;
                    }
                }
            }
        }
    }
}

//Set Scripts a Todos os DropDownLists na Grid
function SetScriptsForAllDropDownListsInGrid(id, gvId, column) {
    //get reference of GridView control
    var grid = document.getElementById(gvId);
    //variable to contain the cell of the grid
    var cell, cell2;

    if (grid.rows.length > 0) {
        //loop starts from 1. rows[0] points to the header.
//        for (i = 1; i < grid.rows.length - 1; i++) {
          for (i = 1; i < grid.rows.length; i++) {
            //get the reference of first column
            cell = grid.rows[i].cells[column];
            cell2 = grid.rows[i].cells[0];

            var check = false;
            //loop according to the number of childNodes in the cell
            for (j = 0; j < cell2.childNodes.length; j++) {
                //if childNode type is TextBox
                if (cell2.childNodes[j].type == "checkbox") {
                    check = cell2.childNodes[j].checked;
                }
            }

            //loop according to the number of childNodes in the cell
            for (j = 0; j < cell.childNodes.length; j++) {
                //if childNode type is TextBox
                if (cell.childNodes[j].type == "select-one") {
                    if (check) {
                        var ddl = document.getElementById(id);
                        cell.childNodes[j].value = ddl.options[ddl.selectedIndex].value;
                    }
                }
            }
        }
    }
}

//Bloquear BackSpace
function BlockBackSpace(e) {
    //event.returnValue = event.keyCode == 8 ? false : true;
    if ((window.event ? e.keyCode : e.which) == 8) {
        if (window.event) {
            //event.returnValue = (window.event ? e.keyCode : e.which) != 8 ? false : true;
            event.returnValue = false;
        } else return false;
    } else if ((window.event ? e.keyCode : e.which) == 9) {
        if (window.event) {
            event.returnValue = false;
        } else return false;
    }
}

//Formata Telefone com DDD
function formatDDDCelular(obj, e) {
    var rValue = new RegExp(/[0-9+]/);
    var teste;

    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if (window.event) { event.returnValue = false }
        else return false;

        if (obj.value.length == 0) {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                }
                else return false;
            }
            else {
                obj.value += '(' + String.fromCharCode(window.event ? e.keyCode : e.which);
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
        }
        else if (obj.value.length == 3) {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {

                teste = obj.value;

                if (teste == "(11") {
                    obj.value += ') 9' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {
                    obj.value += ') ' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }
        }
        else if (obj.value.length == 4) {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {
                if (obj.value.substr(0, 4) == "(11)") {
                    obj.value += ' 9' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {
                    obj.value += ' ' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }
        }
        else if (obj.value.length == 5) {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {
                if (obj.value.substr(0, 4) == "(11)") {
                    obj.value += '9' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }
        }
        else if (obj.value.length == 9) {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {
                if (!(obj.value.substr(0, 4) == "(11)")) {
                    obj.value += '-' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }
        }
        else if (obj.value.length == 10) {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {
                if (obj.value.substr(0, 4) == "(11)") {
                    obj.value += '-' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }
        }
        else {
            if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {
                if (obj.value.length >= obj.maxLength) {
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {
                    obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }
        }
    }

    //Formata Celular com DDD
    function formatDDDCelularNovo(obj, e) {
        var rValue = new RegExp(/[0-9+]/);
        var teste;

        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9 && (window.event ? e.keyCode : e.which) != 32)
            if (window.event) { event.returnValue = false }
            else return false;

            if (obj.value.length == 0) {
                if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                    if (window.event) {
                        event.returnValue = false;
                    }
                    else return false;
                }
                else {
                    obj.value += '(' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }

            else if (obj.value.length == 3) {
                if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) && (window.event ? e.keyCode : e.which) != 32) {
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {

                    teste = obj.value;

                    obj.value += ') ' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }

            else if (obj.value.length == 4) {
                if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) && (window.event ? e.keyCode : e.which) != 32) {
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {

                    obj.value += ' ' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }

            else if (obj.value.length == 5) {
                if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) && (window.event ? e.keyCode : e.which) != 32) {
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {

                    obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }

            else if (obj.value.length == 10) {
                if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {

                    obj.value += '-' + String.fromCharCode(window.event ? e.keyCode : e.which);
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
            }

            else {
                if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
                    if (window.event) {
                        event.returnValue = false;
                    } else return false;
                }
                else {
                    if (obj.value.length >= obj.maxLength) {
                        if (window.event) {
                            event.returnValue = false;
                        } else return false;
                    }
                    else {
                        obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                        if (window.event) {
                            event.returnValue = false;
                        } else return false;
                    }
                }
            }
    }

//Formata Telefone com DDD
function formatDDDTelefone(obj, e) {
    var rValue = new RegExp(/[0-9+]/);

    if (obj.value.length == 0) {
        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
        else {
            obj.value += '(' + String.fromCharCode(window.event ? e.keyCode : e.which);
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
    }
    else if (obj.value.length == 3) {
        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
        else {
            obj.value += ') ' + String.fromCharCode(window.event ? e.keyCode : e.which);
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
    }
    else if (obj.value.length == 4) {
        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
        else {
            obj.value += ' ' + String.fromCharCode(window.event ? e.keyCode : e.which);
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
    }
    else if (obj.value.length == 9) {
        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
        else {
            obj.value += '-' + String.fromCharCode(window.event ? e.keyCode : e.which);
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
    }
    else {
        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which)))) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
        else {
            if (obj.value.length >= obj.maxLength) {
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
            else {
                obj.value += String.fromCharCode(window.event ? e.keyCode : e.which);
                if (window.event) {
                    event.returnValue = false;
                } else return false;
            }
        }
    }
}

//Formata Placa do Veículo
function fnFrmPlaca(obj, e) {
    var rValue = new RegExp(/[0-9+]/);
    var rValueStr = new RegExp(/[a-zA-Z+]/);

    if (obj.value.length < 3) {
        if (!(rValueStr.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
        else {
            obj.value += String.fromCharCode(window.event ? e.keyCode : e.which).toUpperCase();

            if (window.event) {
                event.returnValue = false;
            } else return false;

            if (obj.value.length >= 3)
                obj.value += " ";
        }
    }
    else {
        if (obj.value.length == 3)
            obj.value += " ";

        if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9) {
            if (window.event) {
                event.returnValue = false;
            } else return false;
        }
    }
}

// Mostra/Esconde painel
function ShowHidePanel(ddl, pnl, vle) {
    var ddl1 = document.getElementById(ddl);
    var pnl1 = document.getElementById(pnl);
    var vlr = vle.split('|');

    for (var i = 0; i < vlr.length; i++) {
        if (ddl1.type == "select-one") {
            if (ddl1.options[ddl1.selectedIndex].value == vlr[i]) {
                pnl1.style.display = "block";
                break;
            }
            else
                pnl1.style.display = "none";
        }
    }
}

// Verifica se algum valor foi selecionado
function ValidaDrop(msg, ddl) {
    var msglst = msg.split('|');
    var ddllst = ddl.split('|');
    var msgAlt = "";
    var id = 0;

    for (var i = 0; i < ddllst.length; i++) {
        var ddl1 = document.getElementById(ddllst[i]);

        if (ddl1.type == "select-one") {
            if (ddl1.options[ddl1.selectedIndex] == undefined || 
                    ddl1.options[ddl1.selectedIndex].value == "") {
                msgAlt += msglst[i] + "\n";

                if (id == 0) {
                    id = i;
                }
            }
        }
    }

    if (msgAlt != "") {
        window.alert(msgAlt);

        document.getElementById(ddllst[id]).focus();

        if (window.event) {
            event.returnValue = false;
        } else return false;
    }
}

// Verifica se algum checkbox foi selecionado
function GetCheckBoxListValues(chkBoxID, msg) {
    var chkBox = document.getElementById(chkBoxID);
    var options = chkBox.getElementsByTagName('input');
    var listOfSpans = chkBox.getElementsByTagName('span');
    var count = 0;

    for (var i = 0; i < options.length; i++) {
        if (options[i].checked) {
            count++;
        }
    }

    if (count == "0") {
        window.alert(msg);
        
        if (window.event) {
            event.returnValue = false;
        } else return false;
    }
}

// Recupera o texto selecionado e apresenta na caixa de texto
function GetSelectedText(txb) {
    var txt = txb.value.substr(txb.selectionStart, (txb.selectionEnd - txb.selectionStart));

    if (txt != "")
        txb.value = "";

    if (navigator.appName == "Microsoft Internet Explorer") {
        document.selection.empty();
    }
    else {
        window.getSelection().removeAllRanges();
    }

    if (txb != null) {
        if (txb.createTextRange) {
            var FieldRange = txb.createTextRange();
            FieldRange.moveStart('character', txb.value.length);
            FieldRange.collapse();
            FieldRange.select();
        }
    }
}

//Adciona script para formato monetário
//function moeda(campo, e) {
function CurrencyFormat(campo, e) {

    var rValue = new RegExp(/^\-[0-9]/);

    if (!(rValue.test(String.fromCharCode(window.event ? e.keyCode : e.which))) &&
	        (window.event ? e.keyCode : e.which) != 8 && (window.event ? e.keyCode : e.which) != 9)
        if (window.event) { event.returnValue = false }
        else return false;

   var SeparadorDecimal = ","  
   var SeparadorMilesimo = "."  
   var sep = 0;  
   var key = '';  
   var i = j = 0;  
   var len = len2 = 0;  
   //var strCheck = '0123456789';  
   var strCheck = '-0123456789';
   var aux = aux2 = '';  
   var whichCode = (window.Event) ? e.which : e.keyCode;  
    
   if (whichCode == 13) return true;  
   key = String.fromCharCode(whichCode); // Valor para o código da Chave  
    
   if (strCheck.indexOf(key) == -1) return true; // Chave inválida  
   len = campo.value.length;  
   for(i = 0; i < len; i++)  
  
       if ((campo.value.charAt(i) != '0') && (campo.value.charAt(i) != SeparadorDecimal)) break;  
   aux = '';  
   for(; i < len; i++)  
  
       if (strCheck.indexOf(campo.value.charAt(i))!=-1) aux += campo.value.charAt(i);  
   aux += key;  
   len = aux.length;  
  
   if (len == 0) campo.value = '';  
   if (len == 1) campo.value = '0'+ SeparadorDecimal + '0' + aux;  
    //if (len == 2) campo.value = '0'+ SeparadorDecimal + aux;  
   if (len == 2) {
       if (aux.charAt(0) != "-")
           campo.value = '0' + SeparadorDecimal + aux;
       else
           campo.value = aux;
   }

   if (len > 2) {  
       aux2 = '';  
       for (j = 0, i = len - 3; i >= 0; i--) {  
           if (j == 3) {  
               aux2 += SeparadorMilesimo;  
               j = 0;  
           }  
           aux2 += aux.charAt(i);  
           j++;  
       }  
       campo.value = '';  
       len2 = aux2.length;

       for (i = len2 - 1; i >= 0; i--)
           if (aux2.charAt(i) == ".") {
               if (aux2.charAt(i + 1) != "-")
                   campo.value += aux2.charAt(i);
           }
           else
               campo.value += aux2.charAt(i);

       if (campo.value != "-")
           campo.value += SeparadorDecimal + aux.substr(len - 2, len);
       else
           campo.value += aux.substr(len - 2, len);

       //for (i = len2 - 1; i >= 0; i--)  
       //campo.value += aux2.charAt(i);  
       //campo.value += SeparadorDecimal + aux.substr(len - 2, len);  
}  
   return false;

}

//Formata a saida do valor monetário em Reais
function FormataSaidaCurrency(amount) {
    var i = parseFloat(amount);
    if (isNaN(i)) { i = 0.00; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    i = parseInt((i + .005) * 100);
    i = i / 100;
    s = new String(i);
    if (s.indexOf('.') < 0) { s += '.00'; }
    if (s.indexOf('.') == (s.length - 2)) { s += '0'; }
    s = s.replace('.', ',');
    s = s.replace(/./g, function (c, i, a) {
        return i > 0 && c !== "," && (a.length - i) % 3 === 0 ? "." + c : c;
    });
    s = minus + s;
    return s;
}

//Formata a saida do valor monetário em Dólar
function FormataSaidaCurrencyDolar(amount) {
    var i = parseFloat(amount);
    if (isNaN(i)) { i = 0.00; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    i = parseInt((i + .005) * 100);
    i = i / 100;
    s = new String(i);
    if (s.indexOf('.') < 0) { s += '.00'; }
    if (s.indexOf('.') == (s.length - 2)) { s += '0'; }
    s = s.replace(/./g, function (c, i, a) {
        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? "," + c : c;
    });
    s = minus + s;
    return s;
}

// Recupera novos parâmetros para o AutoCompleteExtender
function SetContextKey(ace, params) {
    var vParams = params.split('|');
    var vlr = "";

    for (var i = 0; i < vParams.length; i++) {
        vlr += $get(vParams[i]).value;
        vlr += ((i + 1) == (vParams.length)) ? "" : "|";
    }

    $find(ace).set_contextKey(vlr);
}

/***** Métodos Referentes ao Pedido *****/

//Realiza o cálculo do valor limite de crédito da tela de pedido
function CalculosDoPedido(vlrLC, vlrLCAberto, vlrDisponivel, vlrTotalItens ,vlrTotal, qtdTotal, gvId, colQdt, colItem, cd, hfFrete, sofreAlteracao, supervisao) {
    var vlrLimiteCredito = parseFloat("0.00").toFixed(cd);
    var vlrLimiteCreditoEmAberto = parseFloat("0.00").toFixed(cd);
    var calculo = parseFloat("0.00").toFixed(cd);

    if (document.getElementById(vlrLC) != null) {
        if (document.getElementById(vlrLC).innerHTML != "") {
            var vlr = document.getElementById(vlrLC).innerHTML;
            vlr = vlr.replace(/\./g, "");
            vlr = vlr.replace(",", ".");
            vlrLimiteCredito = parseFloat(vlr).toFixed(cd);
        } 
    }

    if (document.getElementById(vlrLCAberto) != null) {
        if (document.getElementById(vlrLCAberto).innerHTML != "") {
            var vlr = document.getElementById(vlrLCAberto).innerHTML;
            vlr = vlr.replace(/\./g, "");
            vlr = vlr.replace(",", ".");
            vlrLimiteCreditoEmAberto = parseFloat(vlr).toFixed(cd);
        } 
    }

    var grid = document.getElementById(gvId);
    var valorGridItem, qtdGridItm;
    var totalItensPedido = 0;
    var somaTotalItens = parseFloat("0").toFixed(cd);

    if (grid != null) {
        if (grid.rows.length > 0) {
            for (i = 1; i < grid.rows.length; i++) {
                valorGridItem = grid.rows[i].cells[colItem];
                qtdGridItm = grid.rows[i].cells[colQdt];
                var qtditm = 0;
                var vlritm = parseFloat("0.00").toFixed(cd);

                for (j = 0; j < qtdGridItm.childNodes.length; j++) {
                    if (qtdGridItm.childNodes[j].type == "text") {
                        if (qtdGridItm.childNodes[j].value != "") {
                            qtditm = parseInt(qtdGridItm.childNodes[j].value);
                        }
                    }
                }

                for (j = 0; j < valorGridItem.childNodes.length; j++) {
                    if (valorGridItem.childNodes[j].type == "text") {
                        if (valorGridItem.childNodes[j].value != "") {
                            var vlr = valorGridItem.childNodes[j].value;
                            vlr = vlr.replace(/\./g, "");
                            vlr = vlr.replace(",", ".");
                            var vlritm = parseFloat(vlr).toFixed(cd);
                        }
                    }
                }

                totalItensPedido += qtditm;
                somaTotalItens = parseFloat(somaTotalItens) + parseFloat(qtditm) * parseFloat(vlritm);
            }
        } 
    }

    if (document.getElementById(hfFrete) != null) {
        if (document.getElementById(hfFrete).value != "") {
            var vlr = document.getElementById(hfFrete).value;
            vlr = vlr.replace(/\./g, "");
            vlr = vlr.replace(",", ".");
            var vlrFrete = parseFloat(vlr).toFixed(cd);
            somaTotalItens = parseFloat(somaTotalItens) + parseFloat(vlrFrete);
        }
    }

    if (document.getElementById(vlrDisponivel) != null) {
        calculo = (((vlrLimiteCredito) - (vlrLimiteCreditoEmAberto)) - somaTotalItens);
        calculo = parseFloat(calculo).toFixed(cd);
        document.getElementById(vlrDisponivel).innerHTML = FormataSaidaCurrency(calculo);
        document.getElementById(vlrDisponivel).style.color = parseFloat(calculo).toFixed(cd) < 0 ? "Red" : "Blue";
    }

    document.getElementById(vlrTotal).value         = FormataSaidaCurrency(somaTotalItens);
    document.getElementById(qtdTotal).value         = totalItensPedido;
    document.getElementById(vlrTotalItens).value    = FormataSaidaCurrency(somaTotalItens - parseFloat(vlrFrete));
}

/* Controla quando há divergência de valores dos itens com a tabela de preço  */
function ControlaValores(gvId, colQdt, colItem, cd) {
    var grid = document.getElementById(gvId);
    var valorGridItem, valorGridQtd;

    if (grid.rows.length > 0) {
        for (i = 1; i < grid.rows.length; i++) {
            valorGridItem = grid.rows[i].cells[colItem];
            valorGridQtd = grid.rows[i].cells[colQdt];
            var vlritm = parseFloat("0.00").toFixed(cd);
            var vlrMin = parseFloat("0.00").toFixed(cd);
            var vlrMax = parseFloat("0.00").toFixed(cd);
            var ped = "0";

            for (j = 0; j < valorGridQtd.childNodes.length; j++) {
                if (valorGridQtd.childNodes[j].type == "text") {
                    if (valorGridQtd.childNodes[j].value != "") {
                        if (parseFloat(valorGridQtd.childNodes[j].value) == 0) {
                            valorGridQtd.childNodes[j].style.border = "1px solid Red";
                            valorGridQtd.childNodes[j].style.borderLeft = "4px solid Red";
                            valorGridQtd.childNodes[j].style.color = "Red";
                        }
                        else {
                            valorGridQtd.childNodes[j].style.border = "1px solid #AcA899";
                            valorGridQtd.childNodes[j].style.borderLeft = "4px solid #AcA899";
                            valorGridQtd.childNodes[j].style.color = "Black";
                        }
                    }
                    else {
                        valorGridQtd.childNodes[j].style.border = "1px solid Red";
                        valorGridQtd.childNodes[j].style.borderLeft = "4px solid Red";
                        valorGridQtd.childNodes[j].style.color = "Red";
                    }
                }
            }

            for (j = 0; j < valorGridItem.childNodes.length; j++) {
                if (valorGridItem.childNodes[j].type == "text") {
                    if (valorGridItem.childNodes[j].value != "") {
                        if (parseFloat(valorGridItem.childNodes[j].value) == 0) {
                            valorGridItem.childNodes[j].style.border = "1px solid Red";
                            valorGridItem.childNodes[j].style.borderLeft = "4px solid Red";
                            valorGridItem.childNodes[j].style.color = "Red";
                        }
                        else {
                            valorGridItem.childNodes[j].style.border = "1px solid #AcA899";
                            valorGridItem.childNodes[j].style.borderLeft = "4px solid #AcA899";
                            valorGridItem.childNodes[j].style.color = "Black";
                        }

                        var vlr = valorGridItem.childNodes[j].value;
                        vlr = vlr.replace(/\./g, "");
                        vlr = vlr.replace(",", ".");
                        vlritm = parseFloat(vlr).toFixed(cd);
                    }
                    else {
                        valorGridItem.childNodes[j].style.border = "1px solid Red";
                        valorGridItem.childNodes[j].style.borderLeft = "4px solid Red";
                        valorGridItem.childNodes[j].style.color = "Red";
                    }
                }

                if (valorGridItem.childNodes[j].type == "hidden") {
                    if (valorGridItem.childNodes[j].name.substring(valorGridItem.childNodes[j].name.lastIndexOf("$") + 1, valorGridItem.childNodes[j].name.length) == "hfValorMinimo") {
                        if (valorGridItem.childNodes[j].value != "") {
                            var vlr = valorGridItem.childNodes[j].value;
                            vlr = vlr.replace(/\./g, "");
                            vlr = vlr.replace(",", ".");
                            vlrMin = parseFloat(vlr).toFixed(cd);
                        }
                    }

                    if (valorGridItem.childNodes[j].name.substring(valorGridItem.childNodes[j].name.lastIndexOf("$") + 1, valorGridItem.childNodes[j].name.length) == "hfValorMaximo") {
                        if (valorGridItem.childNodes[j].value != "") {
                            var vlr = valorGridItem.childNodes[j].value;
                            vlr = vlr.replace(/\./g, "");
                            vlr = vlr.replace(",", ".");
                            vlrMax = parseFloat(vlr).toFixed(cd);
                        }
                    }

                    if (valorGridItem.childNodes[j].name.substring(valorGridItem.childNodes[j].name.lastIndexOf("$") + 1, valorGridItem.childNodes[j].name.length) == "hfPedidoId") {
                        if (valorGridItem.childNodes[j].value != "") {
                            ped = valorGridItem.childNodes[j].value;
                        }
                    }
                }

                if (parseFloat((vlritm)) < parseFloat((vlrMin)) || parseFloat((vlritm)) > parseFloat((vlrMax))) {
                    grid.rows[i].style.color = "Red";
                    grid.rows[i].style.fontWeight = "bold";
                }
                else {
                    grid.rows[i].style.color = ped == "0" ? "Gray" : "Black";
                    grid.rows[i].style.fontWeight = "normal";
                }
            }
        }
    }
}

// Realiza o controle do valor do frete
function ControlaFrete(txtFrete, hfFrete, cd) {
    var vlrFreteDefault = parseFloat("0.00").toFixed(cd);
    var vlrFreteNovo = parseFloat("0.00").toFixed(cd);

    if (document.getElementById(hfFrete) != null) {
        if (document.getElementById(hfFrete).value != "") {
            var vlr = document.getElementById(hfFrete).value;
            vlr = vlr.replace(/\./g, "");
            vlr = vlr.replace(",", ".");
            vlrFreteDefault = parseFloat(vlr).toFixed(cd);
        } 
    }

    if (document.getElementById(txtFrete) != null) {
        if (document.getElementById(txtFrete).value != "") {
            var vlr = document.getElementById(txtFrete).value;
            vlr = vlr.replace(/\./g, "");
            vlr = vlr.replace(",", ".");
            vlrFreteNovo = parseFloat(vlr).toFixed(cd);
        } 
    }

    if (vlrFreteNovo != vlrFreteDefault ||
            vlrFreteNovo == "0.00" ||
                document.getElementById(txtFrete).value == "") {
        document.getElementById(txtFrete).style.border = "1px solid Red";
        document.getElementById(txtFrete).style.borderLeft = "4px solid Red";

        if (vlrFreteNovo != vlrFreteDefault)
            document.getElementById(txtFrete).style.color = "Red";
    }
    else {
        document.getElementById(txtFrete).style.border = "1px solid #AcA899";
        document.getElementById(txtFrete).style.borderLeft = "4px solid #AcA899";
        document.getElementById(txtFrete).style.color = "Black";
    }

    document.getElementById(txtFrete).style.width = "150px";
}

// Valida quando cliente triangular
function ClienteTriangular(rfvTriangular, dropTpPedido, btn, msg, drop, msg1, drop1, cpvTriangular, rfvCondPgto, txtCondPgto, UN, imbCondPgto) {
    var ddl = document.getElementById(dropTpPedido);

    for (var i = 0; i < ddl.length; i++) {
        if (ddl.type == "select-one") {
            if (ddl.options[ddl.selectedIndex] == undefined ||
                            ddl.options[ddl.selectedIndex].value != "") {

                /* Valida transferência */
                if (ddl.options[ddl.selectedIndex].value.split('-')[1] == "507" ||
                            ddl.options[ddl.selectedIndex].value.split('-')[1] == "607") {
                    document.getElementById(rfvCondPgto).enabled = false;
                    document.getElementById(imbCondPgto).disabled = true;
                    document.getElementById(txtCondPgto).value = "0 | " + ddl.options[ddl.selectedIndex].text;
                    document.getElementById(txtCondPgto).setAttribute('title', '0 | ' + ddl.options[ddl.selectedIndex].text);
                }
                else {
                    document.getElementById(rfvCondPgto).enabled = true;
                    document.getElementById(imbCondPgto).disabled = false;
                    document.getElementById(txtCondPgto).value = "";

                    /* Valida consignação */
                    if (UN == "PJD" || UN == "PJP") {
                        if (ddl.options[ddl.selectedIndex].value.split('-')[1] == "504" ||
                            ddl.options[ddl.selectedIndex].value.split('-')[1] == "604") {
                            document.getElementById(rfvCondPgto).enabled = false;
                            document.getElementById(imbCondPgto).disabled = true;
                            document.getElementById(txtCondPgto).value = "0 | " + ddl.options[ddl.selectedIndex].text;
                            document.getElementById(txtCondPgto).setAttribute('title', '0 | ' + ddl.options[ddl.selectedIndex].text);
                        }
                        else {
                            document.getElementById(rfvCondPgto).enabled = true;
                            document.getElementById(imbCondPgto).disabled = false;
                            document.getElementById(txtCondPgto).value = "";
                        }
                    }
                }

                if (ddl.options[ddl.selectedIndex].value.split('-')[0] == "True") {
                    document.getElementById(btn).onclick = function () { ValidaDrop(msg, drop), WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("btnSalvar", "", true, "Validacao", "", false, false)) };
                    document.getElementById(rfvTriangular).enabled = true;
                    document.getElementById(cpvTriangular).enabled = true;
                    break;
                }
                else {
                    document.getElementById(btn).onclick = function () { ValidaDrop(msg1, drop1), WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("btnSalvar", "", true, 'Validacao', "", false, false)) };
                    document.getElementById(rfvTriangular).enabled = false;
                    document.getElementById(cpvTriangular).enabled = false;
                }                
            }
        }
    }
}

// Valida mudança do id do cliente
function ValidaMudancaIdCliente(txtId, hfId, lblNome, cpv) {
    var hfId_ = document.getElementById(hfId);
    var lblNome_ = document.getElementById(lblNome);
    var lblNomeValue = lblNome_.innerHTML;

    if (hfId_.value != "") {
        if (txtId.value != hfId_.value) {
            txtId.style.border = "1px solid Red";
            txtId.style.borderLeft = "4px solid Red";
            lblNome_.style.color = "Red";
            document.getElementById(cpv).enabled = true;
        }
        else {
            txtId.style.border = "1px solid #AcA899";
            txtId.style.borderLeft = "4px solid #AcA899";
            lblNome_.style.color = "#13213C";
            document.getElementById(cpv).enabled = false;
        }

        lblNome_.innerHTML = lblNomeValue;
    }
}

// Valida os valores informados nas caixas de texto da grid de itens
function ValidaValoresInformados(gvId, colQdt, colItem, cd, msg) {
    var grid = document.getElementById(gvId);
    var valorGridItem, valorGridQtd;

    if (grid != null) {
        if (grid.rows.length > 0) {
            for (i = 1; i < grid.rows.length; i++) {
                valorGridItem = grid.rows[i].cells[colItem];
                valorGridQtd = grid.rows[i].cells[colQdt];

                for (j = 0; j < valorGridQtd.childNodes.length; j++) {
                    if (valorGridQtd.childNodes[j].type == "text") {
                        if (valorGridQtd.childNodes[j].value != "") {
                            if (parseFloat(valorGridQtd.childNodes[j].value) == 0) {
                                valorGridQtd.childNodes[j].focus();
                                valorGridQtd.childNodes[j].style.border = "1px solid Red";
                                valorGridQtd.childNodes[j].style.borderLeft = "4px solid Red";
                                valorGridQtd.childNodes[j].style.color = "Red";

                                window.al(msg);

                                if (window.event) {
                                    event.returnValue = false;
                                } else return false;
                            }
                            else {
                                valorGridQtd.childNodes[j].style.border = "1px solid #AcA899";
                                valorGridQtd.childNodes[j].style.borderLeft = "4px solid #AcA899";
                                valorGridQtd.childNodes[j].style.color = "Black";
                            }
                        }
                        else {
                            valorGridQtd.childNodes[j].focus();
                            valorGridQtd.childNodes[j].style.border = "1px solid Red";
                            valorGridQtd.childNodes[j].style.borderLeft = "4px solid Red";
                            valorGridQtd.childNodes[j].style.color = "Red";

                            window.al(msg);

                            if (window.event) {
                                event.returnValue = false;
                            } else return false;
                        }
                    }
                }

                for (j = 0; j < valorGridItem.childNodes.length; j++) {
                    if (valorGridItem.childNodes[j].type == "text") {
                        if (valorGridItem.childNodes[j].value != "") {
                            if (parseFloat(valorGridItem.childNodes[j].value) == 0) {
                                valorGridItem.childNodes[j].focus();
                                valorGridItem.childNodes[j].style.border = "1px solid Red";
                                valorGridItem.childNodes[j].style.borderLeft = "4px solid Red";
                                valorGridItem.childNodes[j].style.color = "Red";

                                window.alert(msg);

                                if (window.event) {
                                    event.returnValue = false;
                                } else return false;
                            }
                            else {
                                valorGridItem.childNodes[j].style.border = "1px solid #AcA899";
                                valorGridItem.childNodes[j].style.borderLeft = "4px solid #AcA899";
                                valorGridItem.childNodes[j].style.color = "Black";
                            }
                        }
                        else {
                            valorGridItem.childNodes[j].focus();
                            valorGridItem.childNodes[j].style.border = "1px solid Red";
                            valorGridItem.childNodes[j].style.borderLeft = "4px solid Red";
                            valorGridItem.childNodes[j].style.color = "Red";

                            window.alert(msg);

                            if (window.event) {
                                event.returnValue = false;
                            } else return false;
                        }
                    }
                }
            }
        } 
    }
}

// Calcula valores no contrato
function CalculosDoContrato(gvId, colQdt, colVlr, colVlrFab, colDesc, colSaldo, qtdTotal, vlrTotal, cd, gravado) {
    var grid = document.getElementById(gvId);
    var valorGridItem, qtdGridItm, vlrFab, vlrGridDesc, qtdSaldoAtual;
    var totalItensContrato = 0;
    var somaTotalItens = parseFloat("0").toFixed(cd);
    var vlrDesconto = parseFloat("0").toFixed(cd);

    if (grid != null) {
        if (grid.rows.length > 0) {
            for (i = 1; i < grid.rows.length; i++) {
                valorGridItem = grid.rows[i].cells[colVlr];
                qtdGridItm = grid.rows[i].cells[colQdt];
                vlrFab = grid.rows[i].cells[colVlrFab];
                vlrGridDesc = grid.rows[i].cells[colDesc];
                qtdSaldoAtual = grid.rows[i].cells[colSaldo];
                qdtVinculada = grid.rows[i].cells[colSaldo];
                var qtditm = 0;
                var vlritm = parseFloat("0.00").toFixed(cd);
                var vlrFabrica = parseFloat("0.00").toFixed(cd);
                var _vlrSaldo = "";

                for (j = 0; j < qdtVinculada.childNodes.length; j++) {
                    if (qdtVinculada.childNodes[j].type == "hidden") {
                        if (qdtVinculada.childNodes[j].name.substring(qdtVinculada.childNodes[j].name.lastIndexOf("$") + 1, qdtVinculada.childNodes[j].name.length) == "hdfContratoSaldo") {
                            if (qdtVinculada.childNodes[j].value != "") {
                                _vlrSaldo = qdtVinculada.childNodes[j].value;
                            }
                        }
                    }
                }

                for (j = 0; j < qtdGridItm.childNodes.length; j++) {
                    if (qtdGridItm.childNodes[j].type == "text") {
                        if (qtdGridItm.childNodes[j].value != "") {
                            qtditm = parseInt(qtdGridItm.childNodes[j].value);

                            /* Verifica se a quantidade informada é menor do que a quantidade já vinculada a empenho(s) */
                            if (qtdGridItm.childNodes[j].value < _vlrSaldo) {
                                qtdGridItm.childNodes[j].style.color = "Red";
                            }
                            else {
                                qtdGridItm.childNodes[j].style.color = "Black";
                            }

                            /* Apresenta o saldo quando item sendo informado */
                            for (z = 0; z < qtdSaldoAtual.childNodes.length; z++) {
                                if (qtdSaldoAtual.childNodes[z] != undefined) {

                                    if (qtdGridItm.childNodes[j].value != "") {
                                        //qtdSaldoAtual.childNodes[z].innerHTML = qtdGridItm.childNodes[j].value;
                                        qtdSaldoAtual.childNodes[z].innerHTML = qtdGridItm.childNodes[j].value - _vlrSaldo;
                                    }
                                    else
                                        qtdSaldoAtual.childNodes[z].innerHTML = "0";

                                    if (!gravado) {
                                        if (z == 1)
                                            qtdSaldoAtual.childNodes[z].style.color = "Gray";
                                    }
                                    else
                                        if (z == 1)
                                            qtdSaldoAtual.childNodes[z].style.color = "Black";
                                }
                            }
                        }
                        else {
                            for (z = 0; z < qtdSaldoAtual.childNodes.length; z++) {
                                if (qtdSaldoAtual.childNodes[z] != undefined) {
                                    qtdSaldoAtual.childNodes[z].innerHTML = "0";
                                }
                            }
                        }
                    }
                }

                for (j = 0; j < valorGridItem.childNodes.length; j++) {
                    if (valorGridItem.childNodes[j].type == "text") {
                        if (valorGridItem.childNodes[j].value != "") {
                            var vlr = valorGridItem.childNodes[j].value;
                            vlr = vlr.replace(/\./g, "");
                            vlr = vlr.replace(",", ".");
                            vlritm = parseFloat(vlr).toFixed(cd);
                        }
                    }
                }

                for (j = 0; j < vlrFab.childNodes.length; j++) {
                    if (vlrFab.childNodes[j] != undefined) {
                        if (vlrFab.childNodes[j].wholeText != "") {
                            var vlr = vlrFab.childNodes[j].wholeText;
                            vlr = vlr.replace(/\./g, "");
                            vlr = vlr.replace(",", ".");
                            vlrFabrica = parseFloat(vlr).toFixed(cd);
                        }
                    }
                }

                totalItensContrato += qtditm;
                somaTotalItens = parseFloat(somaTotalItens) + parseFloat(qtditm) * parseFloat(vlritm);

                for (x = 0; x < vlrGridDesc.childNodes.length; x++) {
                    if (vlrGridDesc.childNodes[x] != undefined) {

                        if (parseFloat(vlrFabrica).toFixed(cd) != "0.00")
                            vlrGridDesc.childNodes[x].innerHTML = FormataSaidaCurrency(parseFloat(((((vlrFabrica) - (vlritm)) * 100) / (vlrFabrica))).toFixed(cd));
                        else
                            vlrGridDesc.childNodes[x].innerHTML = "0,00";

                        if (parseFloat(vlrGridDesc.childNodes[x].innerHTML) < 0) {
                            if (x == 1)
                                vlrGridDesc.childNodes[x].style.color = "Red";
                        }
                        else {
                            if (x == 1) {
                                if (gravado)
                                    vlrGridDesc.childNodes[x].style.color = "Black";
                                else
                                    vlrGridDesc.childNodes[x].style.color = "Gray";
                            }
                        }
                    }
                }
            }            
        }
    }

    document.getElementById(vlrTotal).value = FormataSaidaCurrency(somaTotalItens);
    document.getElementById(qtdTotal).value = totalItensContrato;
}

// Valida quantidade caracter de textBox multline
function CountMaxLength(text, long, msg) {
    var maxlength = new Number(long); 
    if (text.value.length > maxlength) {
        text.value = text.value.substring(0, maxlength);
        alert(msg + long);
    }
}


// Check CapsLock
function isCapslock(e) {

    e = (e) ? e : window.event;

    var charCode = false;
    if (e.which) {
        charCode = e.which;
    } else if (e.keyCode) {
        charCode = e.keyCode;
    }

    var shifton = false;
    if (e.shiftKey) {
        shifton = e.shiftKey;
    } else if (e.modifiers) {
        shifton = !!(e.modifiers & 4);
    }

    if (charCode >= 97 && charCode <= 122 && shifton) {
        return true;
    }

    if (charCode >= 65 && charCode <= 90 && !shifton) {
        return true;
    }

    return false;

}