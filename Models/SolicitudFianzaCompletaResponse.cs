namespace fianzas_app.Models
{
    public class SolicitudFianzaCompletaResponse
    {
        public SolicitudFianzaDto Solicitud { get; set; }
        public List<PrendaDto> Prendas { get; set; }
        public List<HistorialFianzaDto> Historial { get; set; }
    }

    public class SolicitudFianzaDto
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
        public bool SfAprobacionLegal { get; set; }
        public bool SfAprobacionTecnica { get; set; }
        public SolicitudFianzaDocumentoDto Documento { get; set; }
    }

    public class SolicitudFianzaDocumentoDto
    {
        public int SfdId { get; set; }
        public byte[] Solicitud { get; set; }
        public byte[] Convenio { get; set; }
        public byte[] Pagare { get; set; }
        public byte[] Prenda { get; set; }
        public DateTime? FechaSubida { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Poliza { get; set; }
    }


    public class HistorialFianzaDto
    {
        public int SfhId { get; set; }
        public int SfId { get; set; }
        public int EstadoFianzaId { get; set; }
        public string EstadoFianzaNombre { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public DateTime FechaCambio { get; set; }
        public string Observacion { get; set; }
    }

}
