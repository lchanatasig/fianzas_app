using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class Corredor
{
    public int CorredorId { get; set; }

    public DateTime? CorredorFechaCreacion { get; set; }

    public string CorredorNombre { get; set; } = null!;

    public string? CorredorEmail { get; set; }

    public string? CorredorNumero { get; set; }

    public int? CorredorEstado { get; set; }

    public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();
}
