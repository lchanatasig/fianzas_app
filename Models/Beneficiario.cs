using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class Beneficiario
{
    public int BenId { get; set; }

    public DateTime? BenFechaCreacion { get; set; }

    public string BenNombre { get; set; } = null!;

    public string BenDireccion { get; set; } = null!;

    public string BenCiRuc { get; set; } = null!;

    public string BenEmail { get; set; } = null!;

    public string BenTelefono { get; set; } = null!;

    public int? BenEstado { get; set; }

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();
}
