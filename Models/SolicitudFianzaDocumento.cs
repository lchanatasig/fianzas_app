using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class SolicitudFianzaDocumento
{
    public int SfdId { get; set; }

    public int? SfdSfId { get; set; }

    public byte[]? SfdSolicitud { get; set; }

    public byte[]? SfdConvenio { get; set; }

    public byte[]? SfdPagare { get; set; }

    public byte[]? SfdPrenda { get; set; }

    public DateTime? SfdFechaSubida { get; set; }

    public DateTime? SfdFechaVencimiento { get; set; }

    public string? SfdPoliza { get; set; }

    public virtual SolicitudFianza? SfdSf { get; set; }
}
