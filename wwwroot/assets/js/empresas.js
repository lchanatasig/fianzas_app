
// Función que calcula el cupo según los valores ingresados
function calcularCupo() {
    // Obtener valores de los inputs y convertirlos a número. Si no hay valor se toma 0.
    const activoCorriente = parseFloat(document.getElementById("EmpfActivoCorriente").value) || 0;
    const activoFijo = parseFloat(document.getElementById("EmpfActivoFijo").value) || 0;
    const activoDiferido = parseFloat(document.getElementById("EmpfActivoDiferido").value) || 0;
    const otrosActivos = parseFloat(document.getElementById("EmpfOtrosActivos").value) || 0;
    const pasivoCorriente = parseFloat(document.getElementById("EmpfPasivoCorriente").value) || 0;
    const pasivoLargoPlazo = parseFloat(document.getElementById("EmpfPasivoLargoPlazo").value) || 0;
    const pasivoDiferido = parseFloat(document.getElementById("EmpfPasivoDiferido").value) || 0;
    const capital = parseFloat(document.getElementById("EmpfCapital").value) || 0;
    const reserva = parseFloat(document.getElementById("EmpfReserva").value) || 0;
    const ocuentasPatrimonio = parseFloat(document.getElementById("EmpfOtrasCuentasPatrimonio").value) || 0;
    const utilidadesAcumuladas = parseFloat(document.getElementById("EmpfUtilidadesAcumuladas").value) || 0;
    const utilidadEjercicio = parseFloat(document.getElementById("EmpfUtilidadEjercicio").value) || 0;
    const perdida = parseFloat(document.getElementById("EmpfPerdida").value) || 0;
    const otrasPerdidas = parseFloat(document.getElementById("EmpfOtrasPerdidas").value) || 0;
    const ventas = parseFloat(document.getElementById("EmpfVentas").value) || 0;
    const utilidad = parseFloat(document.getElementById("EmpfUtilidad").value) || 0;

    // Calculo de los activos
    const totalActivo = activoCorriente + activoFijo + activoDiferido + otrosActivos;
    document.getElementById("EmpfTotalActivos").value = totalActivo;

    // Calculo de los pasivos
    const totalPasivo = pasivoCorriente + pasivoLargoPlazo + pasivoDiferido;
    document.getElementById("EmpfTotalPasivo").value = totalPasivo;

    // Calcular patrimonio neto
    const patrimonioNeto = capital + reserva + ocuentasPatrimonio + utilidadesAcumuladas + utilidadEjercicio - perdida - otrasPerdidas;
    document.getElementById("EmpfPatrimonioNeto").value = patrimonioNeto;

    const patrimonioPasivo = patrimonioNeto + totalPasivo;
    document.getElementById("EmpfPasivoPatrimonio").value = patrimonioPasivo;

    // Calcular base de cupo según la fórmula
    const baseCupo = (activoCorriente * 0.1) + (activoFijo * 0.05) + (ventas * 0.03) + (utilidad * 0.01) + (patrimonioNeto * 0.2);

    // Calcular cupo final multiplicando la base de cupo por el 10%
    const cupo10 = baseCupo * 0.10;

    const cupoFinal = parseFloat(cupo10) || 0;

    // Asignar el resultado al input de "Cupo Asignado" y al campo oculto
    const cupoRedondeado = cupoFinal.toFixed(2); // 2 es el número de decimales
    document.getElementById("EmpfCupoAsignado").value = cupoRedondeado;

    // ===============================
    // Validaciones por números con colores
    // ===============================

    // 1️⃣ Liquidez: debe ser igual o mayor a 1
    const liquidez = activoCorriente / pasivoCorriente || 0;
    validarCampo("AnfLiquidez", liquidez, (val) => val >= 1);

    // 2️⃣ Solvencia: debe ser igual o mayor a 1
    const solvencia = totalActivo / totalPasivo || 0;
    validarCampo("AnfSolvencia", solvencia, (val) => val >= 1);

    // 3️⃣ Capacidad de Cobertura: debe ser igual o mayor a 1
    const capCobertura = activoFijo / totalPasivo;
    validarCampo("AnfCapCobertura", capCobertura, (val) => val >= 1);

    // 4️⃣ Endeudamiento: debe ser menor a 1
    const endeudamiento = totalPasivo / totalActivo || 0; // ratio recomendado
    validarCampo("AnfEndeudamiento", endeudamiento, (val) => val < 1);

    // 5️⃣ Capital Propio: debe ser igual o mayor a 1
    const capitalPropio = patrimonioNeto / totalActivo;
    validarCampo("AnfCapitalPropio", capitalPropio, (val) => val >= 1);

    // 6️⃣ ROA: Utilidad / Total Activo
    const roa = (utilidad / totalActivo)*100 || 0;
    document.getElementById("AnfROA").value = roa.toFixed(2); // Solo mostrar el valor, sin validación de color

    // 7️⃣ ROE: Utilidad / Patrimonio Neto
    const roe = (utilidad / patrimonioNeto) * 100 || 0;
    document.getElementById("AnfROE").value = roe.toFixed(2); // Solo mostrar el valor, sin validación de color

}


// Función para validar los valores y cambiar color de los inputs
function validarCampo(inputId, valor, condicion) {
    const input = document.getElementById(inputId);

    // Asignar el valor al campo (con 2 decimales)
    input.value = valor.toFixed(2);

    // Evaluar la condición pasada y asignar clase correspondiente
    if (condicion(valor)) {
        input.classList.add("valido");
        input.classList.remove("invalido");
    } else {
        input.classList.add("invalido");
        input.classList.remove("valido");
    }
}


// Esperar a que el DOM se cargue y agregar el evento "input" a todos los campos numéricos
document.addEventListener('DOMContentLoaded', () => {
    const inputs = document.querySelectorAll("input[type='number']");
    inputs.forEach(input => {
        input.addEventListener("input", calcularCupo);
    });
});

document.addEventListener('DOMContentLoaded', () => {
    const switches = document.querySelectorAll('.form-check-input[type="checkbox"][role="switch"]');

    switches.forEach((checkbox) => {
        checkbox.addEventListener('change', function () {
            if (this.checked) {
                // Desmarcar los otros
                switches.forEach((otherCheckbox) => {
                    if (otherCheckbox !== this) {
                        otherCheckbox.checked = false;
                    }
                });
            }
        });
    });
});
