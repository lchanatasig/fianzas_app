using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class EmpresaFinanza
{
    public int EmpfId { get; set; }

    public int EmpfEmpresaId { get; set; }

    public DateTime? EmpfFechaCreacion { get; set; }

    public DateTime? EmpfFechaActualizacion { get; set; }

    public decimal EmpfActivoCorriente { get; set; }

    public decimal EmpfActivoFijo { get; set; }

    public decimal? EmpfActivoDiferido { get; set; }

    public decimal? EmpfOtrosActivos { get; set; }

    public decimal? EmpfTotalActivos { get; set; }

    public decimal? EmpfPasivoCorriente { get; set; }

    public decimal? EmpfPasivoLargoPlazo { get; set; }

    public decimal? EmpfPasivoDiferido { get; set; }

    public decimal? EmpfTotalPasivo { get; set; }

    public decimal EmpfCapital { get; set; }

    public decimal EmpfReserva { get; set; }

    public decimal? EmpfOtrasCuentasPatrimonio { get; set; }

    public decimal? EmpfUtilidadesAcumuladas { get; set; }

    public decimal? EmpfUtilidadEjercicio { get; set; }

    public decimal EmpfPerdida { get; set; }

    public decimal? EmpfOtrasPerdidas { get; set; }

    public decimal? EmpfPatrimonioNeto { get; set; }

    public decimal? EmpfPasivoPatrimonio { get; set; }

    public decimal EmpfVentas { get; set; }

    public decimal EmpfUtilidad { get; set; }

    public decimal EmpfCupoAsignado { get; set; }

    public virtual Empresa EmpfEmpresa { get; set; } = null!;
}
