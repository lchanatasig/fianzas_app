using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class TipoEmpresa
{
    public int TempId { get; set; }

    public string TempNombre { get; set; } = null!;

    public string? TempDescripcion { get; set; }

    public int? TempEstado { get; set; }

    public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();
}
