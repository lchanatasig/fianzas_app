using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace fianzas_app.Models;

public partial class AppFianzasContext : DbContext
{
    public AppFianzasContext(DbContextOptions<AppFianzasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EmpresaFinanza> EmpresaFinanzas { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<TipoEmpresa> TipoEmpresas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.EmpId).HasName("PK__empresa__1299A8617D71E496");

            entity.ToTable("empresa");

            entity.HasIndex(e => e.EmpRuc, "UQ__empresa__3159555080385E10").IsUnique();

            entity.HasIndex(e => e.EmpEmail, "UQ__empresa__3D542B08C7FE255B").IsUnique();

            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.EmpCiudad)
                .HasMaxLength(100)
                .HasColumnName("emp_ciudad");
            entity.Property(e => e.EmpEmail)
                .HasMaxLength(255)
                .HasColumnName("emp_email");
            entity.Property(e => e.EmpEstado)
                .HasDefaultValue(true)
                .HasColumnName("emp_estado");
            entity.Property(e => e.EmpFechaActualizacion).HasColumnName("emp_fecha_actualizacion");
            entity.Property(e => e.EmpFechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("emp_fecha_creacion");
            entity.Property(e => e.EmpNombre)
                .HasMaxLength(255)
                .HasColumnName("emp_nombre");
            entity.Property(e => e.EmpRuc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("emp_RUC");
            entity.Property(e => e.EmpTelefono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("emp_telefono");
            entity.Property(e => e.EmpTipoEmpresaId).HasColumnName("emp_tipo_empresa_id");
            entity.Property(e => e.EmpUbicacion)
                .HasMaxLength(500)
                .HasColumnName("emp_ubicacion");

            entity.HasOne(d => d.EmpTipoEmpresa).WithMany(p => p.Empresas)
                .HasForeignKey(d => d.EmpTipoEmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_empresa_tipo_id");
        });

        modelBuilder.Entity<EmpresaFinanza>(entity =>
        {
            entity.HasKey(e => e.EmpfId).HasName("PK__empresa___A7F416085014D71E");

            entity.ToTable("empresa_finanzas");

            entity.Property(e => e.EmpfId).HasColumnName("empf_id");
            entity.Property(e => e.EmpfActivoCorriente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_activo_corriente");
            entity.Property(e => e.EmpfActivoDiferido)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_activo_diferido");
            entity.Property(e => e.EmpfActivoFijo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_activo_fijo");
            entity.Property(e => e.EmpfCapital)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_capital");
            entity.Property(e => e.EmpfCupoAsignado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_cupo_asignado");
            entity.Property(e => e.EmpfEmpresaId).HasColumnName("empf_empresa_id");
            entity.Property(e => e.EmpfFechaActualizacion).HasColumnName("empf_fecha_actualizacion");
            entity.Property(e => e.EmpfFechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("empf_fecha_creacion");
            entity.Property(e => e.EmpfOtrasCuentasPatrimonio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_otras_cuentas_patrimonio");
            entity.Property(e => e.EmpfOtrasPerdidas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_otras_perdidas");
            entity.Property(e => e.EmpfOtrosActivos)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_otros_activos");
            entity.Property(e => e.EmpfPasivoCorriente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_pasivo_corriente");
            entity.Property(e => e.EmpfPasivoDiferido)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_pasivo_diferido");
            entity.Property(e => e.EmpfPasivoLargoPlazo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_pasivo_largo_plazo");
            entity.Property(e => e.EmpfPasivoPatrimonio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_pasivo_patrimonio");
            entity.Property(e => e.EmpfPatrimonioNeto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_patrimonio_neto");
            entity.Property(e => e.EmpfPerdida)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_perdida");
            entity.Property(e => e.EmpfReserva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_reserva");
            entity.Property(e => e.EmpfTotalActivos)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_total_activos");
            entity.Property(e => e.EmpfTotalPasivo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_total_pasivo");
            entity.Property(e => e.EmpfUtilidad)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_utilidad");
            entity.Property(e => e.EmpfUtilidadEjercicio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_utilidad_ejercicio");
            entity.Property(e => e.EmpfUtilidadesAcumuladas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_utilidades_acumuladas");
            entity.Property(e => e.EmpfVentas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("empf_ventas");

            entity.HasOne(d => d.EmpfEmpresa).WithMany(p => p.EmpresaFinanzas)
                .HasForeignKey(d => d.EmpfEmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_empresa_fianza_id");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.PerfilId).HasName("PK__perfil__638DD32C543E1E4A");

            entity.ToTable("perfil");

            entity.Property(e => e.PerfilId).HasColumnName("perfil_id");
            entity.Property(e => e.PerfilEstado)
                .HasDefaultValue(1)
                .HasColumnName("perfil_estado");
            entity.Property(e => e.PerfilNombre)
                .HasMaxLength(255)
                .HasColumnName("perfil_nombre");
        });

        modelBuilder.Entity<TipoEmpresa>(entity =>
        {
            entity.HasKey(e => e.TempId).HasName("PK__tipo_emp__FEEC6BDB199AE6BE");

            entity.ToTable("tipo_empresa");

            entity.Property(e => e.TempId).HasColumnName("temp_id");
            entity.Property(e => e.TempDescripcion)
                .HasMaxLength(255)
                .HasColumnName("temp_descripcion");
            entity.Property(e => e.TempEstado)
                .HasDefaultValue(1)
                .HasColumnName("temp_estado");
            entity.Property(e => e.TempNombre)
                .HasMaxLength(100)
                .HasColumnName("temp_nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__usuario__2ED7D2AF55F91784");

            entity.ToTable("usuario");

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.UsuarioCi)
                .HasMaxLength(13)
                .HasColumnName("usuario_ci");
            entity.Property(e => e.UsuarioDireccion)
                .HasMaxLength(255)
                .HasColumnName("usuario_direccion");
            entity.Property(e => e.UsuarioEstado)
                .HasDefaultValue(1)
                .HasColumnName("usuario_estado");
            entity.Property(e => e.UsuarioNombres)
                .HasMaxLength(255)
                .HasColumnName("usuario_nombres");
            entity.Property(e => e.UsuarioPassword)
                .HasMaxLength(255)
                .HasColumnName("usuario_password");
            entity.Property(e => e.UsuarioPerfilId).HasColumnName("usuario_perfil_id");

            entity.HasOne(d => d.UsuarioPerfil).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.UsuarioPerfilId)
                .HasConstraintName("FK_usuario_perfil_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
