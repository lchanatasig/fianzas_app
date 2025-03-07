using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class Perfil
{
    public int PerfilId { get; set; }

    public string PerfilNombre { get; set; } = null!;

    public int PerfilEstado { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
