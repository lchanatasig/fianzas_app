using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class Empresa
{
    public int EmpId { get; set; }

    public int EmpTipoEmpresaId { get; set; }

    public DateTime? EmpFechaCreacion { get; set; }

    public DateTime? EmpFechaActualizacion { get; set; }

    public string EmpNombre { get; set; } = null!;

    public string? EmpCiudad { get; set; }

    public string? EmpUbicacion { get; set; }

    public string EmpRuc { get; set; } = null!;

    public string EmpTelefono { get; set; } = null!;

    public string EmpEmail { get; set; } = null!;

    public bool? EmpEstado { get; set; }

    public virtual TipoEmpresa EmpTipoEmpresa { get; set; } = null!;

    public virtual ICollection<EmpresaFinanza> EmpresaFinanzas { get; set; } = new List<EmpresaFinanza>();
}
