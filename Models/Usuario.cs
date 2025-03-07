using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public int? UsuarioPerfilId { get; set; }

    public string UsuarioNombres { get; set; } = null!;

    public string? UsuarioDireccion { get; set; }

    public string? UsuarioPassword { get; set; }

    public string? UsuarioCi { get; set; }

    public int? UsuarioEstado { get; set; }

    public virtual Perfil? UsuarioPerfil { get; set; }
}
