namespace fianzas_app.Models
{
    public class SolicitudCompletaResponse
    {
        public int SfId { get; set; }
        public int SfEmpId { get; set; }
        public string EmpresaNombre { get; set; }

        public int SfTposId { get; set; }
        public string TipoSolicitudNombre { get; set; }

        public int SfEstfId { get; set; }
        public string EstadoFianzaNombre { get; set; }

        public int? SfBenId { get; set; }
        public string BeneficiarioNombre { get; set; }

        public DateTime SfFechaSolicitud { get; set; }
        public DateTime SfInicioVigencia { get; set; }
        public DateTime? SfFinVigencia { get; set; }
        public int SfPlazoGarantiaDias { get; set; }

        public string SfSectorFianza { get; set; }
        public decimal SfMontoFianza { get; set; }
        public decimal SfMontoContrato { get; set; }
        public string SfObjetoContrato { get; set; }
        public int? SfAprobacionLegal { get; set; }
        public int? SfAprobacionTecnica { get; set; }

        public string? SfhObservacion { get; set; }


        // Documentos
        public int? SfdId { get; set; }
        public DateTime? SfdFechaSubida { get; set; }
        public DateTime? SfdFechaVencimiento { get; set; }
        public string SfdPoliza { get; set; }
    }

}
