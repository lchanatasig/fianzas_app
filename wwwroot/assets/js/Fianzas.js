document.addEventListener('DOMContentLoaded', function () {

    // Función que oculta todos los formularios de prenda y muestra el correspondiente al valor seleccionado
    function mostrarFormularioPrenda() {
        // Ocultar todos los formularios
        document.getElementById("formPrendaComercial").classList.add("d-none");
        document.getElementById("formPrendaIndustrial").classList.add("d-none");
        document.getElementById("formPrendaHipotecaria").classList.add("d-none");

        // Verificar cuál radio está seleccionado
        const prendaSeleccionada = document.querySelector('input[name="prenda"]:checked');
        if (prendaSeleccionada) {
            // Según el valor del radio, mostramos el formulario correspondiente
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
                default:
                    break;
            }
        }
    }

    // Asignar el event listener a cada radio button con nombre "prenda"
    const radiosPrenda = document.querySelectorAll('input[name="prenda"]');
    radiosPrenda.forEach(radio => {
        radio.addEventListener("change", mostrarFormularioPrenda);
    });

    // Función para mostrar/ocultar el apartado de documentos secundarios (prendas)
    function validarMontoFianza() {
        const monto = parseFloat(document.getElementById("montoFianza").value) || 0;
        const documentosSecundarios = document.getElementById("documentosSecundarios");

        if (monto > 416000) {
            documentosSecundarios.classList.remove("d-none");
        } else {
            documentosSecundarios.classList.add("d-none");
        }
    }

    // Escuchar el evento "input" en el campo montoFianza
    const montoFianzaElem = document.getElementById("montoFianza");
    if (montoFianzaElem) {
        montoFianzaElem.addEventListener("input", validarMontoFianza);
    }

    // Event listener para el select tipoFianza para mostrar el formulario correspondiente
    document.getElementById("tipoFianza").addEventListener("change", function () {
        // Ocultar ambas secciones inicialmente
        document.getElementById("datosContrato").classList.add("d-none");
        document.getElementById("datosAduanera").classList.add("d-none");

        // Obtener y convertir el valor seleccionado a número
        var tipoSeleccionado = parseInt(this.value, 10);

        // Suponiendo que para FCC (1) y BUA (3) se muestra "datosContrato" y para GA (2) "datosAduanera"
        if (tipoSeleccionado === 1 || tipoSeleccionado === 3) {
            document.getElementById("datosContrato").classList.remove("d-none");
        } else if (tipoSeleccionado === 2) {
            document.getElementById("datosAduanera").classList.remove("d-none");
        }
    });


    // Variable empresasData definida globalmente en la vista (si no, la puedes declarar aquí)
    // Por ejemplo: var empresasData = window.empresasData;
    if (typeof empresasData !== 'undefined') {
        // Al cambiar la selección de empresa, se actualizan el cupo disponible y los datos del solicitante
        // Al cambiar la selección de empresa, se actualizan el cupo disponible, los datos del solicitante y se activa el campo montoFianza
        const empresaSelect = document.getElementById("empresaSelect");
        if (empresaSelect) {
            empresaSelect.addEventListener("change", function () {
                var selectedId = this.value;
                var cupo = 0;
                if (selectedId) {
                    var emp = empresasData.find(e => e.EmpresaId == parseInt(selectedId));
                    if (emp) {
                        // Si hay historial, obtenemos el cupo restante
                        if (emp.HistorialEmpresas && emp.HistorialEmpresas.length > 0) {
                            cupo = emp.HistorialEmpresas[0].HistorialCupoRestante;
                        }
                        // Asignamos los datos de la empresa a los campos del solicitante
                        document.getElementById("solicitante").value = emp.NombreEmpresa || "";
                        document.getElementById("direccionSolicitante").value = emp.DireccionEmpresa || "";
                        document.getElementById("rucSolicitante").value = emp.CiEmpresa || "";
                        document.getElementById("emailSolicitante").value = emp.EmailEmpresa || "";
                        document.getElementById("telefonoSolicitante").value = emp.TelefonoEmpresa || "";

                        // Asignamos los datos de la empresa a los campos del solicitante
                        document.getElementById("solicitanteGA").value = emp.NombreEmpresa || "";
                        document.getElementById("direccionSolicitanteGA").value = emp.DireccionEmpresa || "";
                        document.getElementById("rucSolicitanteGA").value = emp.CiEmpresa || "";
                        document.getElementById("emailSolicitanteGA").value = emp.EmailEmpresa || "";
                        document.getElementById("telefonoSolicitanteGA").value = emp.TelefonoEmpresa || "";
                        // Activar el campo montoFianza
                        document.getElementById("montoFianza").disabled = false;
                        document.getElementById("montoFianzaGA").disabled = false;
                    }
                } else {
                    // Si no se selecciona empresa, limpiamos los campos
                    document.getElementById("solicitante").value = "";
                    document.getElementById("direccionSolicitante").value = "";
                    document.getElementById("rucSolicitante").value = "";
                    document.getElementById("emailSolicitante").value = "";
                    document.getElementById("telefonoSolicitante").value = "";

                    // Desactivar el campo montoFianza si no hay empresa seleccionada
                    document.getElementById("montoFianza").disabled = true;
                }
                // Actualizamos el campo de cupo disponible con formato 'es-ES'
                document.getElementById("cupoDisponible").value = parseFloat(cupo).toLocaleString('es-ES', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });
            });
        }

    }


});
(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Obtener todos los formularios que tienen la clase "needs-validation"
        var forms = document.getElementsByClassName('needs-validation');

        Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                // Deshabilitar los controles que están en secciones ocultas para que no se validen
                var hiddenControls = form.querySelectorAll('.d-none input, .d-none select, .d-none textarea');
                hiddenControls.forEach(function (control) {
                    control.disabled = true;
                });

                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }

                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();
document.addEventListener("DOMContentLoaded", function () {

    function abrirRevision(solicitudId, revisionTipo) {
        var url = window.urlDetalleSolicitud + "?solicitudFianzaId=" + solicitudId;

        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error al obtener la solicitud.");
                }
                return response.json();
            })
            .then(data => {
                document.getElementById("solicitudIdRevision").value = data.solicitudFianzaId;
                document.getElementById("solicitudIdLabel").innerText = data.solicitudFianzaId;
                document.getElementById("empresaRevision").innerText = data.empresa;
                let tipoId = data.tipoFianzaId;
                if (tipoId === 1) {
                    document.getElementById("tipoFianzaRevision").innerText = "Fiel Cumplimiento del Contrato";
                } else if (tipoId === 2) {
                    document.getElementById("tipoFianzaRevision").innerText = "Garantía Aduanera";
                } else if (tipoId === 3) {
                    document.getElementById("tipoFianzaRevision").innerText = "Buen Uso del Anticipo";
                } else {
                    document.getElementById("tipoFianzaRevision").innerText = "Sin datos";
                }
                document.getElementById("montoRevision").innerText = "$" + data.montoFianza.toLocaleString();

                let documentosLista = document.getElementById("documentosListaRevision");
                // En este caso, para pdfFCCSolicitud, concatenamos el parámetro de forma dinámica:
                if (tipoId === 1) {
                    documentosLista.innerHTML = `
                        <li><a href="${window.pdfFCCSolicitud}?solicitudFianzaId=${solicitudId}" download>Solicitud de Fianza (FCC PDF)</a></li>
                        <li><a href="${window.pdfFCCConvenio}?solicitudFianzaId=${solicitudId}"" download>Convenio de Fianza (FCC PDF)</a></li>
                        <li><a href="${window.pdfFCCPagare}" download>Pagaré (FCC PDF)</a></li>
                    `;
                } else if (tipoId === 2) {
                    documentosLista.innerHTML = `
                        <li><a href="${window.pdfGASolicitud}" download>Solicitud de Fianza (GA PDF)</a></li>
                        <li><a href="${window.pdfGAConvenio}" download>Convenio de Fianza (GA PDF)</a></li>
                        <li><a href="${window.pdfGAPagare}" download>Pagaré (GA PDF)</a></li>
                    `;
                } else if (tipoId === 3) {
                    documentosLista.innerHTML = `
                        <li><a href="${window.pdfBUASolicitud}" download>Solicitud de Fianza (BUA PDF)</a></li>
                        <li><a href="${window.pdfBUAConvenio}" download>Convenio de Fianza (BUA PDF)</a></li>
                        <li><a href="${window.pdfBUAPagare}" download>Pagaré (BUA PDF)</a></li>
                    `;
                } else {
                    documentosLista.innerHTML = `
                        <li><a href="#" download>Solicitud de Fianza (PDF)</a></li>
                        <li><a href="#" download>Convenio de Fianza (PDF)</a></li>
                        <li><a href="#" download>Pagaré (PDF)</a></li>
                    `;
                }

                if (data.montoFianza > 416000) {
                    documentosLista.innerHTML += `<li><a href="${window.pdfDocumentoAdicional}" download>Documento Adicional (PDF)</a></li>`;
                }

                if (revisionTipo === 'Tecnico') {
                    document.getElementById("revisionLabel").innerText = "Revisión Técnica de Solicitud";
                } else if (revisionTipo === 'Legal') {
                    document.getElementById("revisionLabel").innerText = "Revisión Legal de Solicitud";
                }
            })
            .catch(error => {
                console.error("Error al cargar la solicitud:", error);
                alert("No se pudieron cargar los datos de la solicitud.");
            });
    }

    window.abrirRevision = abrirRevision;

    // Las funciones de aprobación y rechazo siguen igual...
});

// Función para validar monto de fianza y mostrar error si supera el cupo disponible
function validarMontoFianza() {
    const montoFianzaElem = document.getElementById("montoFianza");
    const cupoDisponibleElem = document.getElementById("cupoDisponible");
    const documentosSecundarios = document.getElementById("documentosSecundarios");
    const errorMontoFianza = document.getElementById("errorMontoFianza"); // Mensaje de error

    const montoFianza = parseFloat(montoFianzaElem.value) || 0;
    const cupoDisponible = parseFloat(cupoDisponibleElem.value.replace(/\./g, '').replace(',', '.')) || 0; // Convertir formato europeo

    // Si el monto de la fianza es mayor que el cupo disponible, mostrar error
    if (montoFianza > cupoDisponible) {
        errorMontoFianza.textContent = "El monto de la fianza supera el cupo disponible.";
        errorMontoFianza.classList.remove("d-none"); // Mostrar el mensaje de error
        montoFianzaElem.classList.add("is-invalid"); // Resaltar el campo con error
    } else {
        errorMontoFianza.textContent = "";
        errorMontoFianza.classList.add("d-none"); // Ocultar el mensaje de error
        montoFianzaElem.classList.remove("is-invalid"); // Remover el error del campo
    }

    // Si el monto es mayor a 416000, mostrar documentos secundarios
    if (montoFianza > 416000) {
        documentosSecundarios.classList.remove("d-none");
    } else {
        documentosSecundarios.classList.add("d-none");
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Llamar a la función automáticamente al cargar la página
    validarMontoFianza();

    // Asignar el evento para que se ejecute en cada cambio del monto
    const montoFianzaElem = document.getElementById("montoFianza");
    if (montoFianzaElem) {
        montoFianzaElem.addEventListener("input", validarMontoFianza);
    }
});
