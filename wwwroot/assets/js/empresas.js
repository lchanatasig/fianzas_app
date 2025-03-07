// Función que calcula el cupo según los valores ingresados
function calcularCupo() {
    // Obtener valores de los inputs y convertirlos a número. Si no hay valor se toma 0.
    const activoCorriente = parseFloat(document.getElementById("ActivoCorriente").value) || 0;
    const activoFijo = parseFloat(document.getElementById("ActivoFijo").value) || 0;
    const capital = parseFloat(document.getElementById("Capital").value) || 0;
    const reserva = parseFloat(document.getElementById("Reserva").value) || 0;
    const perdida = parseFloat(document.getElementById("Perdida").value) || 0;
    const ventas = parseFloat(document.getElementById("Ventas").value) || 0;
    const utilidad = parseFloat(document.getElementById("Utilidad").value) || 0;

    // Calcular patrimonio neto
    const patrimonioNeto = capital + reserva - perdida;

    // Calcular base de cupo según la fórmula
    const baseCupo = (activoCorriente * 0.1) + (activoFijo * 0.05) + (ventas * 0.03) + (utilidad * 0.01) + (patrimonioNeto * 0.2);

    // Calcular cupo final multiplicando la base de cupo por el 10%
    const cupoFinal = baseCupo * 0.10;

    // Asignar el resultado al input de "Cupo Asignado" y al campo oculto
    const formattedCupo = cupoFinal.toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    document.getElementById("cupoAsignado").value = formattedCupo;
    document.getElementById("cupoAsignadoHidden").value = formattedCupo;

}

// Esperar a que el DOM se cargue y agregar el evento "input" a todos los campos numéricos
document.addEventListener('DOMContentLoaded', () => {
    const inputs = document.querySelectorAll("input[type='number']");
    inputs.forEach(input => {
        input.addEventListener("input", calcularCupo);
    });
});