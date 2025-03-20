// Función helper para obtener el valor numérico de un input, devolviendo 0 si está vacío o no existe.
function getNumberValue(id) {
    const element = document.getElementById(id);
    return element ? parseFloat(element.value) || 0 : 0;
}

function calcularCupo() {
    // Obtener valores de los inputs usando la función helper.
    const activoCorriente = getNumberValue("EmpfActivoCorriente");
    const activoFijo = getNumberValue("EmpfActivoFijo");
    const activoDiferido = getNumberValue("EmpfActivoDiferido");
    const otrosActivos = getNumberValue("EmpfOtrosActivos");
    const pasivoCorriente = getNumberValue("EmpfPasivoCorriente");
    const pasivoLargoPlazo = getNumberValue("EmpfPasivoLargoPlazo");
    const pasivoDiferido = getNumberValue("EmpfPasivoDiferido");
    const capital = getNumberValue("EmpfCapital");
    const reserva = getNumberValue("EmpfReserva");
    const otrasCuentasPatrimonio = getNumberValue("EmpfOtrasCuentasPatrimonio");
    const utilidadesAcumuladas = getNumberValue("EmpfUtilidadesAcumuladas");
    const utilidadEjercicio = getNumberValue("EmpfUtilidadEjercicio");
    const perdida = getNumberValue("EmpfPerdida");
    const otrasPerdidas = getNumberValue("EmpfOtrasPerdidas");
    const ventas = getNumberValue("EmpfVentas");
    const utilidad = getNumberValue("EmpfUtilidad");

    // Cálculo de totales.
    const totalActivo = activoCorriente + activoFijo + activoDiferido + otrosActivos;
    document.getElementById("EmpfTotalActivos").value = totalActivo.toFixed(2);

    const totalPasivo = pasivoCorriente + pasivoLargoPlazo + pasivoDiferido;
    document.getElementById("EmpfTotalPasivo").value = totalPasivo.toFixed(2);

    const patrimonioNeto = capital + reserva + otrasCuentasPatrimonio + utilidadesAcumuladas + utilidadEjercicio - perdida - otrasPerdidas;
    document.getElementById("EmpfPatrimonioNeto").value = patrimonioNeto.toFixed(2);

    const patrimonioPasivo = patrimonioNeto + totalPasivo;
    document.getElementById("EmpfPasivoPatrimonio").value = patrimonioPasivo.toFixed(2);

    // Calcular base de cupo y cupo final.
    const baseCupo = (activoCorriente * 0.1) + (activoFijo * 0.05) + (ventas * 0.03) + (utilidad * 0.01) + (patrimonioNeto * 0.2);
    const cupoFinal = baseCupo * 0.10;

    // Depuración: Mostrar en consola el valor calculado.
    console.log("Cupo Final calculado:", cupoFinal);

    // Asignar el valor redondeado al input.
    const cupoRedondeado = cupoFinal.toFixed(2);
    document.getElementById("EmpfCupoAsignado").value = cupoRedondeado;

    // Cálculo de ratios financieros con control de división por cero.
    const liquidez = pasivoCorriente !== 0 ? activoCorriente / pasivoCorriente : 0;
    validarCampo("AnfLiquidez", liquidez, (val) => val >= 1);

    const solvencia = totalPasivo !== 0 ? totalActivo / totalPasivo : 0;
    validarCampo("AnfSolvencia", solvencia, (val) => val >= 1);

    const capCobertura = totalPasivo !== 0 ? activoFijo / totalPasivo : 0;
    validarCampo("AnfCapCobertura", capCobertura, (val) => val >= 1);

    const endeudamiento = totalActivo !== 0 ? totalPasivo / totalActivo : 0;
    validarCampo("AnfEndeudamiento", endeudamiento, (val) => val < 1);

    const capitalPropio = totalActivo !== 0 ? patrimonioNeto / totalActivo : 0;
    validarCampo("AnfCapitalPropio", capitalPropio, (val) => val >= 1);

    // Calcular ROA y ROE (conversión a porcentaje).
    const roa = totalActivo !== 0 ? (utilidad / totalActivo) * 100 : 0;
    document.getElementById("AnfROA").value = roa.toFixed(2);

    const roe = patrimonioNeto !== 0 ? (utilidad / patrimonioNeto) * 100 : 0;
    document.getElementById("AnfROE").value = roe.toFixed(2);
}

// Función para validar los valores y cambiar el color de los inputs según la condición.
function validarCampo(inputId, valor, condicion) {
    const input = document.getElementById(inputId);
    if (!input) return;
    input.value = valor.toFixed(2);
    if (condicion(valor)) {
        input.classList.add("valido");
        input.classList.remove("invalido");
    } else {
        input.classList.add("invalido");
        input.classList.remove("valido");
    }
}

// Unificar los eventos DOMContentLoaded.
document.addEventListener('DOMContentLoaded', () => {
    // Asignar el evento "input" a todos los campos numéricos.
    const numericInputs = document.querySelectorAll("input[type='number']");
    numericInputs.forEach(input => {
        input.addEventListener("input", calcularCupo);
    });

    // Configurar los switches para que solo uno se active.
    const switches = document.querySelectorAll('.form-check-input[type="checkbox"][role="switch"]');
    switches.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            if (this.checked) {
                switches.forEach(other => {
                    if (other !== this) {
                        other.checked = false;
                    }
                });
            }
        });
    });

    // Ejecutar el cálculo al cargar la página para establecer valores iniciales.
    calcularCupo();
});
