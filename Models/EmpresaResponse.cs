namespace fianzas_app.Models
{
    public class EmpresaResponse
    {
        public int EmpId { get; set; }
        public int EmpTipoEmpresaId { get; set; }
        public string TipoEmpresaNombre { get; set; }
        public string EmpNombre { get; set; }
        public string EmpCiudad { get; set; }
        public string EmpUbicacion { get; set; }
        public string EmpRUC { get; set; }
        public string EmpTelefono { get; set; }
        public string EmpEmail { get; set; }
        public bool EmpEstado { get; set; }
        public DateTime EmpFechaCreacion { get; set; }
        public DateTime? EmpFechaActualizacion { get; set; }

        public EmpresaFinanzasDto Finanzas { get; set; }
        public AnalisisFinancieroDto AnalisisFinanciero { get; set; }
        public ClasificacionEmpresaDto Clasificacion { get; set; }
        public HistorialEmpresaDto Historial { get; set; }  // Solo el campo cupo_restante
    }

    public class EmpresaFinanzasDto
    {
        public int EmpfId { get; set; }
        public decimal ActivoCorriente { get; set; }
        public decimal ActivoFijo { get; set; }
        public decimal? ActivoDiferido { get; set; }
        public decimal? OtrosActivos { get; set; }
        public decimal? TotalActivos { get; set; }
        public decimal? PasivoCorriente { get; set; }
        public decimal? PasivoLargoPlazo { get; set; }
        public decimal? PasivoDiferido { get; set; }
        public decimal? TotalPasivo { get; set; }
        public decimal Capital { get; set; }
        public decimal Reserva { get; set; }
        public decimal? OtrasCuentasPatrimonio { get; set; }
        public decimal? UtilidadesAcumuladas { get; set; }
        public decimal? UtilidadEjercicio { get; set; }
        public decimal Perdida { get; set; }
        public decimal? OtrasPerdidas { get; set; }
        public decimal? PatrimonioNeto { get; set; }
        public decimal? PasivoPatrimonio { get; set; }
        public decimal Ventas { get; set; }
        public decimal Utilidad { get; set; }
        public decimal CupoAsignado { get; set; }
    }

    public class AnalisisFinancieroDto
    {
        public decimal? Liquidez { get; set; }
        public decimal? Solvencia { get; set; }
        public decimal? CapCobertura { get; set; }
        public decimal? Endeudamiento { get; set; }
        public decimal? CapitalPropio { get; set; }
        public decimal? ROA { get; set; }
        public decimal? ROE { get; set; }
        public string AnalisisSf { get; set; }
    }

    public class ClasificacionEmpresaDto
    {
        public string ClienteC { get; set; }
        public string Clasificacion { get; set; }
        public string Rango { get; set; }
        public IFormFile ClempArchivoSoporte { get; set; }

    }

    public class HistorialEmpresaDto
    {
        public decimal? CupoRestante { get; set; }
        public DateTime? HistFechaActualizacion { get; set; }

    }

}
