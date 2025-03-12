using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class SolicitudHistorialFianza
{
    public int SfhId { get; set; }

    public int? SfhSfId { get; set; }

    public int? SfhEsfId { get; set; }

    public int? SfhUsuarioId { get; set; }

    public DateTime? SfhFechaCambio { get; set; }

    public string? SfhObservacion { get; set; }

    public DateTime? SfhFechaCreacion { get; set; }

    public virtual EstadoFianza? SfhEsf { get; set; }

    public virtual SolicitudFianza? SfhSf { get; set; }

    public virtual Usuario? SfhUsuario { get; set; }
}
