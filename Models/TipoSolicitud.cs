using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class TipoSolicitud
{
    public int TposId { get; set; }

    public string TposSiglas { get; set; } = null!;

    public string TposNombre { get; set; } = null!;

    public int? TposEstado { get; set; }

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();
}
