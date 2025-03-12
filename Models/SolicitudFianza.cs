using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class SolicitudFianza
{
    public int SfId { get; set; }

    public int? SfEmpId { get; set; }

    public int? SfTposId { get; set; }

    public int? SfEstfId { get; set; }

    public int? SfBenId { get; set; }

    public DateTime? SfFechaSolicitud { get; set; }

    public DateTime? SfInicioVigencia { get; set; }

    public int? SfPlazoGarantiaDias { get; set; }

    public string SfSectorFianza { get; set; } = null!;

    public decimal SfMontoFianza { get; set; }

    public decimal SfMontoContrato { get; set; }

    public string SfObjetoContrato { get; set; } = null!;

    public int? SfAprobacionLegal { get; set; }

    public int? SfAprobacionTecnica { get; set; }

    public DateTime? SfFinVigencia { get; set; }

    public virtual Beneficiario? SfBen { get; set; }

    public virtual Empresa? SfEmp { get; set; }

    public virtual EstadoFianza? SfEstf { get; set; }

    public virtual TipoSolicitud? SfTpos { get; set; }

    public virtual ICollection<SolicitudFianzaDocumento> SolicitudFianzaDocumentos { get; set; } = new List<SolicitudFianzaDocumento>();

    public virtual ICollection<SolicitudFianzaPrendum> SolicitudFianzaPrenda { get; set; } = new List<SolicitudFianzaPrendum>();

    public virtual ICollection<SolicitudHistorialFianza> SolicitudHistorialFianzas { get; set; } = new List<SolicitudHistorialFianza>();
}
