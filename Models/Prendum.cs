using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class Prendum
{
    public int PrenId { get; set; }

    public DateTime? PrenFechaCreacino { get; set; }

    public string? PrenTipo { get; set; }

    public string? PrenBien { get; set; }

    public string? PrenDescripcion { get; set; }

    public decimal? PrenValor { get; set; }

    public string? PrenUbicacion { get; set; }

    public string? PrenCustodio { get; set; }

    public DateTime? PrenFechaConstatacion { get; set; }

    public string? PrenResponsableConstatacion { get; set; }

    public byte[]? PrenArchivo { get; set; }

    public int? PrenEstado { get; set; }

    public virtual ICollection<SolicitudFianzaPrendum> SolicitudFianzaPrenda { get; set; } = new List<SolicitudFianzaPrendum>();
}
