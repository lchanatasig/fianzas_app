namespace fianzas_app.Models
{
    public class EmpresaRequest
    {
        public int? EmpresaId { get; set; }  // ID para actualizaciones
        public int EmpTipoEmpresaId { get; set; }
        public string EmpNombre { get; set; }
        public string EmpCiudad { get; set; }
        public string EmpUbicacion { get; set; }
        public string EmpRUC { get; set; }
        public string EmpTelefono { get; set; }
        public string EmpEmail { get; set; }
        public bool? EmpEstado { get; set; } = true;

        // Datos financieros - Activos
        public decimal EmpfActivoCorriente { get; set; }
        public decimal EmpfActivoFijo { get; set; }
        public decimal? EmpfActivoDiferido { get; set; }
        public decimal? EmpfOtrosActivos { get; set; }
        public decimal? EmpfTotalActivos { get; set; }

        // Datos financieros - Pasivos
        public decimal? EmpfPasivoCorriente { get; set; }
        public decimal? EmpfPasivoLargoPlazo { get; set; }
        public decimal? EmpfPasivoDiferido { get; set; }
        public decimal? EmpfTotalPasivo { get; set; }

        // Datos financieros - Patrimonio
        public decimal EmpfCapital { get; set; }
        public decimal EmpfReserva { get; set; }
        public decimal? EmpfOtrasCuentasPatrimonio { get; set; }
        public decimal? EmpfUtilidadesAcumuladas { get; set; }
        public decimal? EmpfUtilidadEjercicio { get; set; }
        public decimal EmpfPerdida { get; set; }
        public decimal? EmpfOtrasPerdidas { get; set; }
        public decimal? EmpfPatrimonioNeto { get; set; }

        // Patrimonio y Pasivo
        public decimal? EmpfPasivoPatrimonio { get; set; }

        // Resultados financieros
        public decimal EmpfVentas { get; set; }
        public decimal EmpfUtilidad { get; set; }
        public decimal EmpfCupoAsignado { get; set; }

        // Datos de análisis financiero
        public decimal? AnfLiquidez { get; set; }
        public decimal? AnfSolvencia { get; set; }
        public decimal? AnfCapCobertura { get; set; }
        public decimal? AnfEndeudamiento { get; set; }
        public decimal? AnfCapitalPropio { get; set; }
        public decimal? AnfROA { get; set; }
        public decimal? AnfROE { get; set; }
        public string AnfAnalisisSf { get; set; }

        // Clasificación de la empresa
        public string ClempClienteC { get; set; }
        public string ClempClasificacion { get; set; }
        public string ClempRango { get; set; }
        public byte[] ClempArchivoSoporte { get; set; }

        // Observación inicial
        public string HistObservacion { get; set; }
    }
}
