// Función debounce: retrasa la ejecución de una función hasta que haya pasado 'delay' ms desde el último llamado
function debounce(fn, delay) {
    let timer = null;
    return function () {
        clearTimeout(timer);
        timer = setTimeout(fn, delay);
    };
}

document.addEventListener('DOMContentLoaded', function () {
    let porcentajeMaximoContrato = 0;
    let hotInstance;

    // ---------- MOSTRAR FORMULARIOS DE PRENDA Y ASIGNAR TIPO A LA TABLA ----------
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
            // Asignar el mismo tipo de prenda a todas las filas
            if (hotInstance) {
                let selectedType = prendaSeleccionada.value;
                let rowCount = hotInstance.countRows();
                for (let i = 0; i < rowCount; i++) {
                    hotInstance.setDataAtRowProp(i, 'PrenTipo', selectedType);
                }
            }
        }
    }
    document.querySelectorAll('input[name="prenda"]').forEach(radio => {
        radio.addEventListener("change", mostrarFormularioPrenda);
    });

    // ---------- FUNCIONES PARA MANEJO DE EMPRESA Y FIANZA ----------
    function cargarDatosEmpresa(empresaId) {
        const selectedId = parseInt(empresaId, 10);
        let cupo = 0;
        if (selectedId) {
            const emp = empresasData.find(e => e.EmpId === selectedId);
            if (emp && emp.Historial && emp.Historial.CupoRestante !== null) {
                cupo = emp.Historial.CupoRestante;
            }
            // Sección FCC/BUA
            document.getElementById("solicitante").value = emp.EmpNombre || "";
            document.getElementById("direccionSolicitante").value = emp.EmpUbicacion || "";
            document.getElementById("rucSolicitante").value = emp.EmpRUC || "";
            document.getElementById("emailSolicitante").value = emp.EmpEmail || "";
            document.getElementById("telefonoSolicitante").value = emp.EmpTelefono || "";
            // Sección GA
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
    }
    function cargarTipoFianza(tipoSeleccionado) {
        tipoSeleccionado = parseInt(tipoSeleccionado, 10);
        document.getElementById("datosContrato").classList.add("d-none");
        document.getElementById("datosAduanera").classList.add("d-none");
        porcentajeMaximoContrato = 0;
        if (tipoSeleccionado === 1) {
            porcentajeMaximoContrato = 15;
            document.getElementById("datosContrato").classList.remove("d-none");
        } else if (tipoSeleccionado === 2) {
            porcentajeMaximoContrato = 100;
            document.getElementById("datosAduanera").classList.remove("d-none");
        } else if (tipoSeleccionado === 3) {
            porcentajeMaximoContrato = 50;
            document.getElementById("datosContrato").classList.remove("d-none");
        }
        document.getElementById("montocontrato").value = "";
        document.getElementById("montoFianza").value = "";
        document.getElementById("montoFianzaGA").value = "";
        validarMontoContrato();
        validarMontoFianza();
    }
    const tipoFianzaSelect = document.getElementById("tipoFianza");
    if (tipoFianzaSelect) {
        tipoFianzaSelect.addEventListener("change", function () {
            cargarTipoFianza(this.value);
        });
    }
    if (typeof empresasData !== 'undefined') {
        const empresaSelect = document.getElementById("empresaSelect");
        if (empresaSelect) {
            empresaSelect.addEventListener("change", function () {
                cargarDatosEmpresa(this.value);
            });
        }
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
    }
    function validarMontoFianza() {
        let montoFianzaElem, errorMontoFianza;
        if (!document.getElementById("datosAduanera").classList.contains("d-none")) {
            montoFianzaElem = document.getElementById("montoFianzaGA");
            errorMontoFianza = document.getElementById("errorMontoFianzaGA") || document.getElementById("errorMontoFianza");
        } else {
            montoFianzaElem = document.getElementById("montoFianza");
            errorMontoFianza = document.getElementById("errorMontoFianza");
        }
        const cupoDisponibleElem = document.getElementById("cupoDisponible");
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
        const documentosSecundarios = document.getElementById("documentosSecundarios");
        if (montoFianza > 416000) {
            documentosSecundarios.classList.remove("d-none");
        } else {
            documentosSecundarios.classList.add("d-none");
        }
    }
    function calcularPlazoEnDias() {
        let inicio, fin, plazoElem;
        if (!document.getElementById("datosAduanera").classList.contains("d-none")) {
            inicio = document.getElementById("inicioVigenciaGA").value;
            fin = document.getElementById("finVigenciaGA").value;
            plazoElem = document.getElementById("plazoGarantiaDiasGA");
        } else {
            inicio = document.getElementById("inicioVigencia").value;
            fin = document.getElementById("finVigencia").value;
            plazoElem = document.getElementById("plazoGarantiaDias");
        }
        if (!inicio || !fin) {
            plazoElem.value = "";
            return;
        }
        const inicioDate = new Date(inicio);
        const finDate = new Date(fin);
        const diferenciaMs = finDate - inicioDate;
        const diferenciaDias = Math.ceil(diferenciaMs / (1000 * 60 * 60 * 24));
        if (diferenciaDias < 0) {
            plazoElem.value = "";
            alert("La fecha de fin debe ser posterior a la de inicio.");
            return;
        }
        plazoElem.value = diferenciaDias;
    }

    const inicioVigenciaElem = document.getElementById("inicioVigencia");
    const finVigenciaElem = document.getElementById("finVigencia");
    const inicioVigenciaGAElem = document.getElementById("inicioVigenciaGA");
    const finVigenciaGAElem = document.getElementById("finVigenciaGA");
    if (inicioVigenciaElem) { inicioVigenciaElem.addEventListener("change", calcularPlazoEnDias); }
    if (finVigenciaElem) { finVigenciaElem.addEventListener("change", calcularPlazoEnDias); }
    if (inicioVigenciaGAElem) { inicioVigenciaGAElem.addEventListener("change", calcularPlazoEnDias); }
    if (finVigenciaGAElem) { finVigenciaGAElem.addEventListener("change", calcularPlazoEnDias); }

    const montocontratoElem = document.getElementById("montocontrato");
    const montoFianzaElem = document.getElementById("montoFianza");
    const montoFianzaGAElem = document.getElementById("montoFianzaGA");
    if (montocontratoElem) { montocontratoElem.addEventListener("input", validarMontoContrato); }
    if (montoFianzaElem) { montoFianzaElem.addEventListener("input", validarMontoFianza); }
    if (montoFianzaGAElem) { montoFianzaGAElem.addEventListener("input", validarMontoFianza); }

    if (typeof empresasData !== 'undefined') {
        const empresaSelect = document.getElementById("empresaSelect");
        if (empresaSelect && empresaSelect.value) { cargarDatosEmpresa(empresaSelect.value); }
    }
    if (tipoFianzaSelect && tipoFianzaSelect.value) { cargarTipoFianza(tipoFianzaSelect.value); }

    // ---------- INICIALIZACIÓN DE HANDSONTABLE PARA PRENDA COMERCIAL ----------
    var container = document.getElementById('prendasExcel');
    if (container) {
        hotInstance = new Handsontable(container, {
            data: [],
            dataSchema: {
                PrenFechaCreacion: new Date(),
                PrenTipo: "",
                PrenBien: "",
                PrenDescripcion: "",
                PrenValor: null,
                PrenUbicacion: "",
                PrenCustodio: "",
                PrenFechaConstatacion: null,
                PrenResponsableConstatacion: "",
                PrenArchivo: null,      // Se maneja aparte
                PrenNumeroItem: null,
                PrenValorTotal: null
            },
            minSpareRows: 1,
            columns: [
                { data: 'PrenTipo', type: 'text' },
                { data: 'PrenNumeroItem', type: 'numeric' },
                { data: 'PrenBien', type: 'text' },
                { data: 'PrenDescripcion', type: 'text' },
                { data: 'PrenValor', type: 'numeric', numericFormat: { pattern: '0,0.00', culture: 'en-US' } },
                { data: 'PrenUbicacion', type: 'text' },
                { data: 'PrenCustodio', type: 'text' },
                { data: 'PrenFechaConstatacion', type: 'date', dateFormat: 'YYYY-MM-DD' },
                { data: 'PrenResponsableConstatacion', type: 'text' }
            ],
            colHeaders: [
                'Tipo',
                'Número Item',
                'Bien',
                'Contenido/Descripción',
                'Valor',
                'Ubicación',
                'Custodio',
                'Fecha Constatación',
                'Responsable Constatación'
            ],
            rowHeaders: true,
            stretchH: 'all',
            licenseKey: 'non-commercial-and-evaluation',
            afterChange: debounce(function (changes, source) {
                if (source === 'loadData' || !changes) return;
                let total = 0;
                let data = this.getSourceData();
                // Recorremos cada fila para sumar PrenValor (si existe)
                for (let i = 0; i < data.length; i++) {
                    let cellValue = data[i].PrenValor;
                    if (cellValue != null) {
                        let numVal = parseFloat(cellValue);
                        if (!isNaN(numVal)) {
                            total += numVal;
                        }
                    }
                    // Asignar fecha de creación si no existe
                    if (!data[i].PrenFechaCreacion) {
                        data[i].PrenFechaCreacion = new Date();
                    }
                }
                // Actualizar el input total (asegúrate de tener un elemento con id "totalPrendaValor")
                let totalInput = document.getElementById("totalPrendaValor");
                if (totalInput) {
                    totalInput.value = total.toFixed(2);
                }
                // Actualizar PrenValorTotal en cada fila con el total calculado
                for (let i = 0; i < data.length; i++) {
                    this.setDataAtRowProp(i, 'PrenValorTotal', total);
                }
            }, 300)
        });

        document.getElementById("formPrendaComercial").addEventListener('transitionend', function () {
            hotInstance.render();
        });
    }

    // ---------- ENVIAR LOS DATOS DE LA TABLA AL INPUT OCULTO AL SUBMIT DEL FORMULARIO ----------
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function () {
            // Filtrar filas vacías (se consideran vacías si PrenTipo, PrenBien y PrenDescripcion están vacíos)
            var allData = hotInstance.getSourceData();
            var validData = allData.filter(function (row) {
                return (row.PrenTipo && row.PrenTipo.trim() !== "") ||
                    (row.PrenBien && row.PrenBien.trim() !== "") ||
                    (row.PrenDescripcion && row.PrenDescripcion.trim() !== "");
            });
            document.getElementById('PrendasJson').value = JSON.stringify(validData);
        });
    }
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
