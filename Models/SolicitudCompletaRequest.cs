namespace fianzas_app.Models
{
    public class SolicitudCompletaRequest
    {
        // Beneficiario (opcional)
        public int? BenId { get; set; }
        public string BenNombre { get; set; }
        public string BenDireccion { get; set; }
        public string BenCiRuc { get; set; }
        public string BenEmail { get; set; }
        public string BenTelefono { get; set; }

        // Datos Solicitud Fianza
        public int SfEmpId { get; set; }
        public int SfUsuarioId { get; set; }
        public int SfTposId { get; set; }
        public int SfEstfId { get; set; }
        public DateTime SfFechaSolicitud { get; set; }
        public DateTime SfInicioVigencia { get; set; }
        public DateTime SfFinVigencia { get; set; }
        public int SfPlazoGarantiaDias { get; set; }
        public string SfSectorFianza { get; set; }
        public decimal SfMontoFianza { get; set; }
        public decimal SfMontoContrato { get; set; }
        public string SfObjetoContrato { get; set; }
        public int? SfAprobacionLegal { get; set; }
        public int? SfAprobacionTecnica { get; set; }

        // Documentos
        public byte[] SfdSolicitud { get; set; }
        public byte[] SfdConvenio { get; set; }
        public byte[] SfdPagare { get; set; }
        public byte[] SfdPrenda { get; set; }
        public DateTime? SfdFechaSubida { get; set; }
        public DateTime? SfdFechaVencimiento { get; set; }
        public string SfdPoliza { get; set; }

        // Prendas (múltiples)
        public string PrendasJson { get; set; }

        public List<PrendaDto> Prendas { get; set; } = new List<PrendaDto>();
    }

    public class PrendaDto
    {
        public int PrenId { get; set; }

        public DateTime? PrenFechaCreacion { get; set; }
        public string PrenTipo { get; set; }
        public string PrenBien { get; set; }
        public string PrenDescripcion { get; set; }
        public decimal? PrenValor { get; set; }
        public string PrenUbicacion { get; set; }
        public string PrenCustodio { get; set; }
        public DateTime? PrenFechaConstatacion { get; set; }
        public string PrenResponsableConstatacion { get; set; }
        public byte[] PrenArchivo { get; set; }
        public int? PrenNumeroItem { get; set; }

        public decimal? PrenValorTotal { get; set; }

    }

}
