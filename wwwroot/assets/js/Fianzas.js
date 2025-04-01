document.addEventListener('DOMContentLoaded', function () {
    // Declaración global y variables
    let porcentajeMaximoContrato = 0;
    // Declarar la instancia de Handsontable en ámbito global dentro del DOMContentLoaded
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

            // Actualizar la columna 'pren_tipo' en todas las filas de la tabla
            if (hotInstance) {
                let selectedType = prendaSeleccionada.value;
                let rowCount = hotInstance.countRows();
                for (let i = 0; i < rowCount; i++) {
                    hotInstance.setDataAtRowProp(i, 'pren_tipo', selectedType);
                }
            }
        }
    }

    const radiosPrenda = document.querySelectorAll('input[name="prenda"]');
    radiosPrenda.forEach(radio => {
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

        // Ejecutar validaciones después de cargar los datos
        validarMontoContrato();
        validarMontoFianza();
    }

    function cargarTipoFianza(tipoSeleccionado) {
        tipoSeleccionado = parseInt(tipoSeleccionado, 10);

        // Ocultar ambas secciones inicialmente
        document.getElementById("datosContrato").classList.add("d-none");
        document.getElementById("datosAduanera").classList.add("d-none");

        // Reiniciar la variable
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

        document.getElementById("montocontrato").value = "";
        document.getElementById("montoFianza").value = "";
        document.getElementById("montoFianzaGA").value = "";

        // Ejecutar validaciones después de cambiar el tipo
        validarMontoContrato();
        validarMontoFianza();
    }

    // ---------- EVENTO CAMBIO DE TIPO DE FIANZA ----------
    const tipoFianzaSelect = document.getElementById("tipoFianza");
    if (tipoFianzaSelect) {
        tipoFianzaSelect.addEventListener("change", function () {
            cargarTipoFianza(this.value);
        });
    }

    // ---------- EVENTO CAMBIO DE EMPRESA ----------
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
    }

    // ---------- VALIDACIÓN DEL MONTO DE FIANZA ----------
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

    // ---------- CÁLCULO AUTOMÁTICO DEL PLAZO DE GARANTÍA EN DÍAS ----------
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

    // Añadir event listeners para inputs de fechas
    const inicioVigenciaElem = document.getElementById("inicioVigencia");
    const finVigenciaElem = document.getElementById("finVigencia");
    const inicioVigenciaGAElem = document.getElementById("inicioVigenciaGA");
    const finVigenciaGAElem = document.getElementById("finVigenciaGA");

    if (inicioVigenciaElem) {
        inicioVigenciaElem.addEventListener("change", calcularPlazoEnDias);
    }
    if (finVigenciaElem) {
        finVigenciaElem.addEventListener("change", calcularPlazoEnDias);
    }
    if (inicioVigenciaGAElem) {
        inicioVigenciaGAElem.addEventListener("change", calcularPlazoEnDias);
    }
    if (finVigenciaGAElem) {
        finVigenciaGAElem.addEventListener("change", calcularPlazoEnDias);
    }

    // ---------- AGREGAR EVENT LISTENERS A LOS INPUTS DE MONTO ----------
    const montocontratoElem = document.getElementById("montocontrato");
    const montoFianzaElem = document.getElementById("montoFianza");
    const montoFianzaGAElem = document.getElementById("montoFianzaGA");

    if (montocontratoElem) {
        montocontratoElem.addEventListener("input", validarMontoContrato);
    }
    if (montoFianzaElem) {
        montoFianzaElem.addEventListener("input", validarMontoFianza);
    }
    if (montoFianzaGAElem) {
        montoFianzaGAElem.addEventListener("input", validarMontoFianza);
    }

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

    // Disparar eventos 'change' al cargar la vista
    if (typeof empresasData !== 'undefined') {
        const empresaSelect = document.getElementById("empresaSelect");
        if (empresaSelect && empresaSelect.value) {
            cargarDatosEmpresa(empresaSelect.value);
        }
    }
    if (tipoFianzaSelect && tipoFianzaSelect.value) {
        cargarTipoFianza(tipoFianzaSelect.value);
    }

    // ---------- INICIALIZACIÓN DE HANDSONTABLE PARA PRENDA COMERCIAL ----------
    var container = document.getElementById('prendasExcel');
    if (container) {
        // Se define dataSchema para que cada fila se cree como objeto
        hotInstance = new Handsontable(container, {
            data: [], // Array vacío inicial
            dataSchema: {
                pren_tipo: "",
                pren_numero_item: null,
                pren_bien: "",
                pren_descripcion: "",
                pren_valor: null,
                pren_ubicacion: "",
                pren_custodio: "",
                pren_fecha_constatacion: null,
                pren_responsable_constatacion: ""
            },
            minSpareRows: 1, // Siempre una fila extra para nuevos datos
            columns: [
                { data: 'pren_tipo', type: 'text' },
                { data: 'pren_numero_item', type: 'numeric' },
                { data: 'pren_bien', type: 'text' },
                { data: 'pren_descripcion', type: 'text' },
                { data: 'pren_valor', type: 'numeric', numericFormat: { pattern: '0,0.00', culture: 'en-US' } },
                { data: 'pren_ubicacion', type: 'text' },
                { data: 'pren_custodio', type: 'text' },
                { data: 'pren_fecha_constatacion', type: 'date', dateFormat: 'YYYY-MM-DD' },
                { data: 'pren_responsable_constatacion', type: 'text' }
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
            // Hook para recalcular el total de la columna pren_valor
            afterChange: function (changes, source) {
                if (source !== 'loadData' && changes) {
                    let total = 0;
                    let data = this.getSourceData();
                    data.forEach(row => {
                        if (row.pren_valor && !isNaN(row.pren_valor)) {
                            total += parseFloat(row.pren_valor);
                        }
                    });
                    // Actualizar el campo con id "totalPrendaValor"
                    let totalInput = document.getElementById("totalPrendaValor");
                    if (totalInput) {
                        totalInput.value = total.toFixed(2);
                    }
                }
            }
        });

        // Si el formulario se muestra con una transición, refrescamos la tabla
        document.getElementById("formPrendaComercial").addEventListener('transitionend', function () {
            hotInstance.render();
        });
    }

    // ---------- ENVIAR LOS DATOS DE LA TABLA AL INPUT OCULTO AL SUBMIT DEL FORMULARIO ----------
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function () {
            // Usamos getSourceData() para obtener los objetos y no arrays
            document.getElementById('PrendasJson').value = JSON.stringify(hotInstance.getSourceData());
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
