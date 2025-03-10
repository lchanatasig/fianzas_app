// Porcentaje máximo de contrato según el tipo seleccionado
let porcentajeMaximoContrato = 0;
let contadorPrendas = 0;

document.addEventListener('DOMContentLoaded', function () {

    // ---------- MOSTRAR FORMULARIOS DE PRENDA ----------
    function mostrarFormularioPrenda() {
        document.getElementById("formPrendaComercial").classList.add("d-none");
        document.getElementById("formPrendaIndustrial").classList.add("d-none");
        document.getElementById("formPrendaHipotecaria").classList.add("d-none");
        document.getElementById("formPrendaCartaAval").classList.add("d-none");

        const prendaSeleccionada = document.querySelector('input[name="prenda"]:checked');

        if (prendaSeleccionada) {
            switch (prendaSeleccionada.value) {
                case "Prenda Comercial":
                    document.getElementById("formPrendaComercial").classList.remove("d-none");
                    break;
                case "Prenda Industrial":
                    document.getElementById("formPrendaIndustrial").classList.remove("d-none");
                    break;
                case "Prenda Hipotecaria":
                    document.getElementById("formPrendaHipotecaria").classList.remove("d-none");
                    break;
                case "Prenda Carta Aval":
                    document.getElementById("formPrendaCartaAval").classList.remove("d-none");
                    break;
                default:
                    break;
            }
        }
    }

    const radiosPrenda = document.querySelectorAll('input[name="prenda"]');
    radiosPrenda.forEach(radio => {
        radio.addEventListener("change", mostrarFormularioPrenda);
    });

    // ---------- EVENTO CAMBIO DE TIPO DE FIANZA ----------
    const tipoFianzaSelect = document.getElementById("tipoFianza");
    tipoFianzaSelect.addEventListener("change", function () {
        document.getElementById("datosContrato").classList.add("d-none");
        document.getElementById("datosAduanera").classList.add("d-none");

        const tipoSeleccionado = parseInt(this.value, 10);
        porcentajeMaximoContrato = 0;

        if (tipoSeleccionado === 1) {
            porcentajeMaximoContrato = 15; // FCC
            document.getElementById("datosContrato").classList.remove("d-none");
        } else if (tipoSeleccionado === 2) {
            porcentajeMaximoContrato = 100; // GA
            document.getElementById("datosAduanera").classList.remove("d-none");
        } else if (tipoSeleccionado === 3) {
            porcentajeMaximoContrato = 50; // BUA
            document.getElementById("datosContrato").classList.remove("d-none");
        }

        console.log("Tipo Fianza Seleccionado:", tipoSeleccionado);
        console.log("Porcentaje máximo permitido para el contrato:", porcentajeMaximoContrato);

        document.getElementById("montocontrato").value = "";
        document.getElementById("montoFianza").value = "";

        validarMontoContrato();
        validarMontoFianza();
    });

    // ---------- EVENTO CAMBIO DE EMPRESA ----------
    if (typeof empresasData !== 'undefined') {
        const empresaSelect = document.getElementById("empresaSelect");

        empresaSelect.addEventListener("change", function () {
            const selectedId = parseInt(this.value, 10);
            let cupo = 0;

            if (selectedId) {
                const emp = empresasData.find(e => e.EmpId === selectedId);

                if (emp && emp.Historial && emp.Historial.CupoRestante !== null) {
                    cupo = emp.Historial.CupoRestante;
                }

                document.getElementById("solicitante").value = emp.EmpNombre || "";
                document.getElementById("direccionSolicitante").value = emp.EmpUbicacion || "";
                document.getElementById("rucSolicitante").value = emp.EmpRUC || "";
                document.getElementById("emailSolicitante").value = emp.EmpEmail || "";
                document.getElementById("telefonoSolicitante").value = emp.EmpTelefono || "";

                document.getElementById("solicitanteGA").value = emp.EmpNombre || "";
                document.getElementById("direccionSolicitanteGA").value = emp.EmpUbicacion || "";
                document.getElementById("rucSolicitanteGA").value = emp.EmpRUC || "";
                document.getElementById("emailSolicitanteGA").value = emp.EmpEmail || "";
                document.getElementById("telefonoSolicitanteGA").value = emp.EmpTelefono || "";

                document.getElementById("montoFianza").disabled = false;
                document.getElementById("montoFianzaGA").disabled = false;

                const cupoInput = document.getElementById("cupoDisponible");
                cupoInput.value = parseFloat(cupo).toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                cupoInput.dataset.valorNumerico = cupo;

            } else {
                limpiarCamposEmpresa();
            }

            validarMontoContrato();
            validarMontoFianza();
        });
    }

    function limpiarCamposEmpresa() {
        const campos = ["solicitante", "direccionSolicitante", "rucSolicitante", "emailSolicitante", "telefonoSolicitante"];
        const camposGA = ["solicitanteGA", "direccionSolicitanteGA", "rucSolicitanteGA", "emailSolicitanteGA", "telefonoSolicitanteGA"];

        campos.forEach(id => document.getElementById(id).value = "");
        camposGA.forEach(id => document.getElementById(id).value = "");

        document.getElementById("montoFianza").disabled = true;
        document.getElementById("montoFianzaGA").disabled = true;
        document.getElementById("cupoDisponible").value = "";
        document.getElementById("cupoDisponible").dataset.valorNumerico = 0;
    }

    // ---------- VALIDACIÓN DEL MONTO DEL CONTRATO ----------
    function validarMontoContrato() {
        const montocontratoElem = document.getElementById("montocontrato");
        const cupoDisponibleElem = document.getElementById("cupoDisponible");
        const errorMontoContrato = document.getElementById("errorMontoContrato");

        const cupoDisponible = parseFloat(cupoDisponibleElem.dataset.valorNumerico) || 0;
        const montoContrato = parseFloat(montocontratoElem.value) || 0;

        if (porcentajeMaximoContrato === 0 || cupoDisponible === 0) {
            mostrarError(errorMontoContrato, montocontratoElem, "Debe seleccionar una empresa y un tipo de fianza.");
            return;
        }

        const maxPermitido = (cupoDisponible * porcentajeMaximoContrato) / 100;

        if (montoContrato > maxPermitido) {
            mostrarError(errorMontoContrato, montocontratoElem, `El monto del contrato supera el ${porcentajeMaximoContrato}% del cupo disponible ($${maxPermitido.toFixed(2)}).`);
        } else {
            ocultarError(errorMontoContrato, montocontratoElem);
        }

        console.log("Monto de Contrato:", montoContrato);
        console.log("Máximo permitido:", maxPermitido);
    }

    // ---------- VALIDACIÓN DEL MONTO DE FIANZA ----------
    function validarMontoFianza() {
        const montoFianzaElem = document.getElementById("montoFianza");
        const cupoDisponibleElem = document.getElementById("cupoDisponible");
        const documentosSecundarios = document.getElementById("documentosSecundarios");
        const errorMontoFianza = document.getElementById("errorMontoFianza");

        const cupoDisponible = parseFloat(cupoDisponibleElem.dataset.valorNumerico) || 0;
        const montoFianza = parseFloat(montoFianzaElem.value) || 0;

        if (cupoDisponible === 0) {
            mostrarError(errorMontoFianza, montoFianzaElem, "Debe seleccionar una empresa válida.");
            return;
        }

        if (montoFianza > cupoDisponible) {
            mostrarError(errorMontoFianza, montoFianzaElem, `El monto de la fianza no puede superar el cupo disponible ($${cupoDisponible.toFixed(2)}).`);
        } else {
            ocultarError(errorMontoFianza, montoFianzaElem);
        }

        if (montoFianza > 416000) {
            documentosSecundarios.classList.remove("d-none");
        } else {
            documentosSecundarios.classList.add("d-none");
        }

        console.log("Monto de Fianza:", montoFianza);
        console.log("Cupo disponible:", cupoDisponible);
    }

    // ---------- CÁLCULO AUTOMÁTICO DEL PLAZO DE GARANTÍA EN DÍAS ----------
    const inicioVigenciaElem = document.getElementById("inicioVigencia");
    const finVigenciaElem = document.getElementById("finVigencia");
    const plazoGarantiaDiasElem = document.getElementById("plazoGarantiaDias");

    function calcularPlazoEnDias() {
        const inicioVigencia = inicioVigenciaElem.value;
        const finVigencia = finVigenciaElem.value;

        if (!inicioVigencia || !finVigencia) {
            plazoGarantiaDiasElem.value = "";
            return;
        }

        const inicioDate = new Date(inicioVigencia);
        const finDate = new Date(finVigencia);

        const diferenciaMs = finDate - inicioDate;
        const diferenciaDias = Math.ceil(diferenciaMs / (1000 * 60 * 60 * 24));

        if (diferenciaDias < 0) {
            plazoGarantiaDiasElem.value = "";
            alert("La fecha de fin debe ser posterior a la de inicio.");
            return;
        }

        plazoGarantiaDiasElem.value = diferenciaDias;

        console.log("Inicio Vigencia:", inicioVigencia);
        console.log("Fin Vigencia:", finVigencia);
        console.log("Plazo de Garantía en días:", diferenciaDias);
    }

    inicioVigenciaElem.addEventListener("change", calcularPlazoEnDias);
    finVigenciaElem.addEventListener("change", calcularPlazoEnDias);

    // ---------- FUNCIONES AUXILIARES ----------
    function mostrarError(errorElem, inputElem, mensaje) {
        errorElem.textContent = mensaje;
        errorElem.classList.remove("d-none");
        inputElem.classList.add("is-invalid");
    }

    function ocultarError(errorElem, inputElem) {
        errorElem.textContent = "";
        errorElem.classList.add("d-none");
        inputElem.classList.remove("is-invalid");
    }

    // ---------- EVENTOS INPUT PARA VALIDACIÓN EN TIEMPO REAL ----------
    const montocontratoElem = document.getElementById("montocontrato");
    if (montocontratoElem) {
        montocontratoElem.addEventListener("input", validarMontoContrato);
    }

    const montoFianzaElem = document.getElementById("montoFianza");
    if (montoFianzaElem) {
        montoFianzaElem.addEventListener("input", validarMontoFianza);
    }

    // ============= HANDSONTABLE PARA LAS PRENDAS =============
    const container = document.getElementById('prendasExcel');
    const hot = new Handsontable(container, {
        data: [],
        colHeaders: [
            'Tipo',
            'Bien',
            'Descripción',
            'Valor',
            'Ubicación',
            'Custodio',
            'Fecha de Constatación',
            'Responsable de Constatación'
        ],
        columns: [
            { data: 'PrenTipo', type: 'text' },
            { data: 'PrenBien', type: 'text' },
            { data: 'PrenDescripcion', type: 'text' },
            { data: 'PrenValor', type: 'numeric', numericFormat: { pattern: '0,0.00' } },
            { data: 'PrenUbicacion', type: 'text' },
            { data: 'PrenCustodio', type: 'text' },
            { data: 'PrenFechaConstatacion', type: 'date', dateFormat: 'YYYY-MM-DD' },
            { data: 'PrenResponsableConstatacion', type: 'text' }
        ],
        rowHeaders: true,
        minSpareRows: 1,
        height: 'auto',
        licenseKey: 'non-commercial-and-evaluation'
    });

    const form = document.getElementById('solicitudFianzaForm');

    form.addEventListener('submit', function (event) {
        // Cambia getData() a getSourceData()
        const prendasData = hot.getSourceData();

        // Opcionalmente filtra filas vacías
        const prendasFiltradas = prendasData.filter(prenda =>
            Object.values(prenda).some(x => x !== null && x !== '')
        );

        const prendasJsonField = document.getElementById('PrendasJson');

        prendasJsonField.value = JSON.stringify(prendasFiltradas);

        console.log("Prendas JSON a enviar:", prendasJsonField.value);
    });


});

// ---------- BOOTSTRAP VALIDACIÓN GENERAL ----------
(function () {
    'use strict';
    window.addEventListener('load', function () {
        const forms = document.getElementsByClassName('needs-validation');

        Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                const hiddenControls = form.querySelectorAll('.d-none input, .d-none select, .d-none textarea');
                hiddenControls.forEach(control => control.disabled = true);

                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }

                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();
