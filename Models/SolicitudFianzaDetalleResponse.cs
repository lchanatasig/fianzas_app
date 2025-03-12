namespace fianzas_app.Models
{
    public class SolicitudFianzaDetalleResponse
    {
        // Datos de la solicitud
        public int SfId { get; set; }
        public int SfEmpId { get; set; }
        public string EmpresaNombre { get; set; }
        public int SfTposId { get; set; }
        public string TipoSolicitudNombre { get; set; }
        public int SfEstfId { get; set; }
        public string EstadoFianzaNombre { get; set; }
        public int SfBenId { get; set; }
        public string BenNombre { get; set; }
        public string BenDireccion { get; set; }
        public string BenCiRuc { get; set; }
        public string BenEmail { get; set; }
        public string BenTelefono { get; set; }
        public DateTime SfFechaSolicitud { get; set; }
        public DateTime SfInicioVigencia { get; set; }
        public DateTime? SfFinVigencia { get; set; }
        public int SfPlazoGarantiaDias { get; set; }
        public string SfSectorFianza { get; set; }
        public decimal? SfMontoFianza { get; set; }
        public decimal? SfMontoContrato { get; set; }
        public string SfObjetoContrato { get; set; }
        public int? SfAprobacionLegal { get; set; }
        public int? SfAprobacionTecnica { get; set; }

        // Documentos
        public int SfdId { get; set; }
        public DateTime? SfdFechaSubida { get; set; }
        public DateTime? SfdFechaVencimiento { get; set; }
        public string SfdPoliza { get; set; }

        // Prendas asociadas
        public List<PrendaResponse> Prendas { get; set; } = new List<PrendaResponse>();

        // Historial de la solicitud
        public List<HistorialSolicitudResponse> Historial { get; set; } = new List<HistorialSolicitudResponse>();
    }

    public class PrendaResponse
    {
        public int PrenId { get; set; }
        public DateTime PrenFechaCreacion { get; set; }
        public string PrenTipo { get; set; }
        public string PrenBien { get; set; }
        public string PrenDescripcion { get; set; }
        public decimal PrenValor { get; set; }
        public string PrenUbicacion { get; set; }
        public string PrenCustodio { get; set; }
        public DateTime PrenFechaConstatacion { get; set; }
        public string PrenResponsableConstatacion { get; set; }
    }

    public class HistorialSolicitudResponse
    {
        public int SfhId { get; set; }
        public int SfhSfId { get; set; }
        public int SfhEsfId { get; set; }
        public string EstadoNombre { get; set; }
        public int SfhUsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public DateTime SfhFechaCambio { get; set; }
        public string SfhObservacion { get; set; }
    }

}
