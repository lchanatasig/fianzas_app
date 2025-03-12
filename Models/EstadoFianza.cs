using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class EstadoFianza
{
    public int EstfId { get; set; }

    public string EstfNombre { get; set; } = null!;

    public string? EstfDescripcion { get; set; }

    public DateTime? EstfFechaCreacion { get; set; }

    public int? EstfEstado { get; set; }

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();

    public virtual ICollection<SolicitudHistorialFianza> SolicitudHistorialFianzas { get; set; } = new List<SolicitudHistorialFianza>();
}
