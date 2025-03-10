using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class ClasificacionEmpresa
{
    public int ClempId { get; set; }

    public int? ClempEmpresaId { get; set; }

    public string? ClempClienteC { get; set; }

    public string? ClempClasificacion { get; set; }

    public string? ClempRango { get; set; }

    public byte[]? ClempArchivoSoporte { get; set; }

    public virtual Empresa? ClempEmpresa { get; set; }
}
