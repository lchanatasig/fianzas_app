using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace fianzas_app.Models;

public partial class AppFianzasContext : DbContext
{
    public AppFianzasContext()
    {
    }

    public AppFianzasContext(DbContextOptions<AppFianzasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnalisisFinanciero> AnalisisFinancieros { get; set; }

    public virtual DbSet<Beneficiario> Beneficiarios { get; set; }

    public virtual DbSet<ClasificacionEmpresa> ClasificacionEmpresas { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EmpresaFinanza> EmpresaFinanzas { get; set; }

    public virtual DbSet<EstadoFianza> EstadoFianzas { get; set; }

    public virtual DbSet<HistorialEmpresa> HistorialEmpresas { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Prendum> Prenda { get; set; }

    public virtual DbSet<SolicitudFianza> SolicitudFianzas { get; set; }

    public virtual DbSet<SolicitudFianzaDocumento> SolicitudFianzaDocumentos { get; set; }

    public virtual DbSet<SolicitudFianzaPrendum> SolicitudFianzaPrenda { get; set; }

    public virtual DbSet<SolicitudHistorialFianza> SolicitudHistorialFianzas { get; set; }

    public virtual DbSet<TipoEmpresa> TipoEmpresas { get; set; }

    public virtual DbSet<TipoSolicitud> TipoSolicituds { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=app_fianzas;User Id=sa;Password=Sur2o22--;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalisisFinanciero>(entity =>
        {
            entity.HasKey(e => e.AnfId).HasName("PK__analisis__B2AD49D17A05BF45");

            entity.ToTable("analisis_financiero");

            entity.Property(e => e.AnfId).HasColumnName("anf_id");
            entity.Property(e => e.AnfAnalisisSf)
                .HasMaxLength(1000)
                .HasColumnName("anf_analisis_sf");
            entity.Property(e => e.AnfCapCobertura)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_cap_cobertura");
            entity.Property(e => e.AnfCapitalPropio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_capital_propio");
            entity.Property(e => e.AnfEmpresaId).HasColumnName("anf_empresa_id");
            entity.Property(e => e.AnfEndeudamiento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_endeudamiento");
            entity.Property(e => e.AnfLiquidez)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_liquidez");
            entity.Property(e => e.AnfRoa)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_ROA");
            entity.Property(e => e.AnfRoe)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_ROE");
            entity.Property(e => e.AnfSolvencia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anf_solvencia");

            entity.HasOne(d => d.AnfEmpresa).WithMany(p => p.AnalisisFinancieros)
                .HasForeignKey(d => d.AnfEmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_analisis_financiero_empresa");
        });

        modelBuilder.Entity<Beneficiario>(entity =>
        {
            entity.HasKey(e => e.BenId).HasName("PK__benefici__A2C8E98DCAD4F8A8");

            entity.ToTable("beneficiario");

            entity.Property(e => e.BenId).HasColumnName("ben_id");
            entity.Property(e => e.BenCiRuc)
                .HasMaxLength(13)
                .HasColumnName("ben_ci_ruc");
            entity.Property(e => e.BenDireccion)
                .HasMaxLength(255)
                .HasColumnName("ben_direccion");
            entity.Property(e => e.BenEmail)
                .HasMaxLength(255)
                .HasColumnName("ben_email");
            entity.Property(e => e.BenEstado)
                .HasDefaultValue(1)
                .HasColumnName("ben_estado");
            entity.Property(e => e.BenFechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("ben_fecha_creacion");
            entity.Property(e => e.BenNombre)
                .HasMaxLength(255)
                .HasColumnName("ben_nombre");
            entity.Property(e => e.BenTelefono)
                .HasMaxLength(10)
                .HasColumnName("ben_telefono");
        });

        modelBuilder.Entity<ClasificacionEmpresa>(entity =>
        {
            entity.HasKey(e => e.ClempId).HasName("PK__clasific__BB50B9E9321C54CF");

            entity.ToTable("clasificacion_empresa");

            entity.Property(e => e.ClempId).HasColumnName("clemp_id");
            entity.Property(e => e.ClempArchivoSoporte).HasColumnName("clemp_archivo_soporte");
            entity.Property(e => e.ClempClasificacion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("clemp_clasificacion");
            entity.Property(e => e.ClempClienteC)
                .HasMaxLength(100)
                .HasColumnName("clemp_cliente_c");
            entity.Property(e => e.ClempEmpresaId).HasColumnName("clemp_empresa_id");
            entity.Property(e => e.ClempRango)
                .HasMaxLength(255)
                .HasColumnName("clemp_rango");

            entity.HasOne(d => d.ClempEmpresa).WithMany(p => p.ClasificacionEmpresas)
                .HasForeignKey(d => d.ClempEmpresaId)
                .HasConstraintName("FK_clasificacion_empresa_empresa");
        });

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

        modelBuilder.Entity<EstadoFianza>(entity =>
        {
            entity.HasKey(e => e.EstfId).HasName("PK__estado_f__972294E521953608");

            entity.ToTable("estado_fianza");

            entity.Property(e => e.EstfId).HasColumnName("estf_id");
            entity.Property(e => e.EstfDescripcion)
                .HasMaxLength(255)
                .HasColumnName("estf_descripcion");
            entity.Property(e => e.EstfEstado)
                .HasDefaultValue(1)
                .HasColumnName("estf_estado");
            entity.Property(e => e.EstfFechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("estf_fecha_creacion");
            entity.Property(e => e.EstfNombre)
                .HasMaxLength(255)
                .HasColumnName("estf_nombre");
        });

        modelBuilder.Entity<HistorialEmpresa>(entity =>
        {
            entity.HasKey(e => e.HistId).HasName("PK__historia__D478390DE3F2A7A0");

            entity.ToTable("historial_empresas");

            entity.Property(e => e.HistId).HasColumnName("hist_id");
            entity.Property(e => e.HistActivoC)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_activoC");
            entity.Property(e => e.HistActivoD)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_activoD");
            entity.Property(e => e.HistActivoF)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_activoF");
            entity.Property(e => e.HistActivoO)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_activoO");
            entity.Property(e => e.HistActivoT)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_activoT");
            entity.Property(e => e.HistAnfAnalisisSf)
                .HasMaxLength(1000)
                .HasColumnName("hist_anf_analisis_sf");
            entity.Property(e => e.HistAnfCapCobertura)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_cap_cobertura");
            entity.Property(e => e.HistAnfCapitalPropio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_capital_propio");
            entity.Property(e => e.HistAnfEndeudamiento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_endeudamiento");
            entity.Property(e => e.HistAnfLiquidez)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_liquidez");
            entity.Property(e => e.HistAnfRoa)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_ROA");
            entity.Property(e => e.HistAnfRoe)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_ROE");
            entity.Property(e => e.HistAnfSolvencia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_anf_solvencia");
            entity.Property(e => e.HistCapital)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_capital");
            entity.Property(e => e.HistClempClasificacion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("hist_clemp_clasificacion");
            entity.Property(e => e.HistClempClienteC)
                .HasMaxLength(100)
                .HasColumnName("hist_clemp_cliente_c");
            entity.Property(e => e.HistClempRango)
                .HasMaxLength(255)
                .HasColumnName("hist_clemp_rango");
            entity.Property(e => e.HistCupoRestante)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_cupo_restante");
            entity.Property(e => e.HistEmpresaId).HasColumnName("hist_empresa_id");
            entity.Property(e => e.HistFechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("hist_fecha_actualizacion");
            entity.Property(e => e.HistLempArchivoSoporte).HasColumnName("hist_lemp_archivo_soporte");
            entity.Property(e => e.HistObservacion)
                .HasMaxLength(1000)
                .HasColumnName("hist_observacion");
            entity.Property(e => e.HistOtrasCp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_otrasCP");
            entity.Property(e => e.HistOtrasPer)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_otrasPer");
            entity.Property(e => e.HistPasivoC)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_pasivoC");
            entity.Property(e => e.HistPasivoD)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_pasivoD");
            entity.Property(e => e.HistPasivoLp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_pasivoLP");
            entity.Property(e => e.HistPasivoT)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_pasivoT");
            entity.Property(e => e.HistPatrimonioPasivo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_patrimonio_pasivo");
            entity.Property(e => e.HistPatrimonioT)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_patrimonioT");
            entity.Property(e => e.HistPerdidas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_perdidas");
            entity.Property(e => e.HistReserva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_reserva");
            entity.Property(e => e.HistUsuarioId).HasColumnName("hist_usuario_id");
            entity.Property(e => e.HistUtilidad)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_utilidad");
            entity.Property(e => e.HistUtilidadE)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_utilidadE");
            entity.Property(e => e.HistUtilidadesA)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_utilidadesA");
            entity.Property(e => e.HistVentas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hist_ventas");
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

        modelBuilder.Entity<Prendum>(entity =>
        {
            entity.HasKey(e => e.PrenId).HasName("PK__prenda__A454E9592757E938");

            entity.ToTable("prenda");

            entity.Property(e => e.PrenId).HasColumnName("pren_id");
            entity.Property(e => e.PrenArchivo).HasColumnName("pren_archivo");
            entity.Property(e => e.PrenBien)
                .HasMaxLength(255)
                .HasColumnName("pren_bien");
            entity.Property(e => e.PrenCustodio)
                .HasMaxLength(255)
                .HasColumnName("pren_custodio");
            entity.Property(e => e.PrenDescripcion)
                .HasMaxLength(255)
                .HasColumnName("pren_descripcion");
            entity.Property(e => e.PrenEstado)
                .HasDefaultValue(1)
                .HasColumnName("pren_estado");
            entity.Property(e => e.PrenFechaConstatacion)
                .HasColumnType("datetime")
                .HasColumnName("pren_fecha_constatacion");
            entity.Property(e => e.PrenFechaCreacino)
                .HasColumnType("datetime")
                .HasColumnName("pren_fecha_creacino");
            entity.Property(e => e.PrenNumeroItem).HasColumnName("pren_numero_item");
            entity.Property(e => e.PrenResponsableConstatacion)
                .HasMaxLength(255)
                .HasColumnName("pren_responsable_constatacion");
            entity.Property(e => e.PrenTipo)
                .HasMaxLength(255)
                .HasColumnName("pren_tipo");
            entity.Property(e => e.PrenUbicacion)
                .HasMaxLength(255)
                .HasColumnName("pren_ubicacion");
            entity.Property(e => e.PrenValor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pren_valor");
            entity.Property(e => e.PrenValorTotal)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pren_valor_total");
        });

        modelBuilder.Entity<SolicitudFianza>(entity =>
        {
            entity.HasKey(e => e.SfId).HasName("PK__solicitu__D5759B0D667B4B42");

            entity.ToTable("solicitud_fianza");

            entity.Property(e => e.SfId).HasColumnName("sf_id");
            entity.Property(e => e.SfAprobacionLegal).HasColumnName("sf_aprobacion_legal");
            entity.Property(e => e.SfAprobacionTecnica).HasColumnName("sf_aprobacion_tecnica");
            entity.Property(e => e.SfBenId).HasColumnName("sf_ben_id");
            entity.Property(e => e.SfEmpId).HasColumnName("sf_emp_id");
            entity.Property(e => e.SfEstfId).HasColumnName("sf_estf_id");
            entity.Property(e => e.SfFechaSolicitud)
                .HasColumnType("datetime")
                .HasColumnName("sf_fecha_solicitud");
            entity.Property(e => e.SfFinVigencia)
                .HasColumnType("datetime")
                .HasColumnName("sf_fin_vigencia");
            entity.Property(e => e.SfInicioVigencia)
                .HasColumnType("datetime")
                .HasColumnName("sf_inicio_vigencia");
            entity.Property(e => e.SfMontoContrato)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sf_monto_contrato");
            entity.Property(e => e.SfMontoFianza)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sf_monto_fianza");
            entity.Property(e => e.SfObjetoContrato)
                .HasMaxLength(255)
                .HasColumnName("sf_objeto_contrato");
            entity.Property(e => e.SfPlazoGarantiaDias).HasColumnName("sf_plazo_garantia_dias");
            entity.Property(e => e.SfSectorFianza)
                .HasMaxLength(255)
                .HasColumnName("sf_sector_fianza");
            entity.Property(e => e.SfTposId).HasColumnName("sf_tpos_id");

            entity.HasOne(d => d.SfBen).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.SfBenId)
                .HasConstraintName("FK_solicitud_beneficiario");

            entity.HasOne(d => d.SfEmp).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.SfEmpId)
                .HasConstraintName("FK_solicitud_empresa");

            entity.HasOne(d => d.SfEstf).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.SfEstfId)
                .HasConstraintName("FK_solicitud_estado");

            entity.HasOne(d => d.SfTpos).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.SfTposId)
                .HasConstraintName("FK_solicitud_tipo_fianza");
        });

        modelBuilder.Entity<SolicitudFianzaDocumento>(entity =>
        {
            entity.HasKey(e => e.SfdId).HasName("PK__solicitu__FDC8ED4C3AF9FC5A");

            entity.ToTable("solicitud_fianza_documento");

            entity.Property(e => e.SfdId).HasColumnName("sfd_id");
            entity.Property(e => e.SfdConvenio).HasColumnName("sfd_convenio");
            entity.Property(e => e.SfdFechaSubida)
                .HasColumnType("datetime")
                .HasColumnName("sfd_fecha_subida");
            entity.Property(e => e.SfdFechaVencimiento)
                .HasColumnType("datetime")
                .HasColumnName("sfd_fecha_vencimiento");
            entity.Property(e => e.SfdPagare).HasColumnName("sfd_pagare");
            entity.Property(e => e.SfdPoliza)
                .HasMaxLength(255)
                .HasColumnName("sfd_poliza");
            entity.Property(e => e.SfdPrenda).HasColumnName("sfd_prenda");
            entity.Property(e => e.SfdSfId).HasColumnName("sfd_sf_id");
            entity.Property(e => e.SfdSolicitud).HasColumnName("sfd_solicitud");

            entity.HasOne(d => d.SfdSf).WithMany(p => p.SolicitudFianzaDocumentos)
                .HasForeignKey(d => d.SfdSfId)
                .HasConstraintName("FK_solicitud_documento");
        });

        modelBuilder.Entity<SolicitudFianzaPrendum>(entity =>
        {
            entity.HasKey(e => e.SfpId).HasName("PK__solicitu__139C6936BD28D284");

            entity.ToTable("solicitud_fianza_prenda");

            entity.Property(e => e.SfpId).HasColumnName("sfp_id");
            entity.Property(e => e.SfpPrenId).HasColumnName("sfp_pren_id");
            entity.Property(e => e.SfpSfId).HasColumnName("sfp_sf_id");

            entity.HasOne(d => d.SfpPren).WithMany(p => p.SolicitudFianzaPrenda)
                .HasForeignKey(d => d.SfpPrenId)
                .HasConstraintName("FK__solicitud__sfp_p__04E4BC85");

            entity.HasOne(d => d.SfpSf).WithMany(p => p.SolicitudFianzaPrenda)
                .HasForeignKey(d => d.SfpSfId)
                .HasConstraintName("FK__solicitud__sfp_s__03F0984C");
        });

        modelBuilder.Entity<SolicitudHistorialFianza>(entity =>
        {
            entity.HasKey(e => e.SfhId).HasName("PK__solicitu__B0E755DF7A577414");

            entity.ToTable("solicitud_historial_fianza");

            entity.Property(e => e.SfhId).HasColumnName("sfh_id");
            entity.Property(e => e.SfhEsfId).HasColumnName("sfh_esf_id");
            entity.Property(e => e.SfhFechaCambio)
                .HasColumnType("datetime")
                .HasColumnName("sfh_fecha_cambio");
            entity.Property(e => e.SfhFechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("sfh_fecha_creacion");
            entity.Property(e => e.SfhObservacion)
                .HasMaxLength(255)
                .HasColumnName("sfh_observacion");
            entity.Property(e => e.SfhSfId).HasColumnName("sfh_sf_id");
            entity.Property(e => e.SfhUsuarioId).HasColumnName("sfh_usuario_id");

            entity.HasOne(d => d.SfhEsf).WithMany(p => p.SolicitudHistorialFianzas)
                .HasForeignKey(d => d.SfhEsfId)
                .HasConstraintName("FK_historial_estado");

            entity.HasOne(d => d.SfhSf).WithMany(p => p.SolicitudHistorialFianzas)
                .HasForeignKey(d => d.SfhSfId)
                .HasConstraintName("FK_historial_fianza");

            entity.HasOne(d => d.SfhUsuario).WithMany(p => p.SolicitudHistorialFianzas)
                .HasForeignKey(d => d.SfhUsuarioId)
                .HasConstraintName("FK_historial_usuario");
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

        modelBuilder.Entity<TipoSolicitud>(entity =>
        {
            entity.HasKey(e => e.TposId).HasName("PK__tipo_sol__58C73FCF367A4E23");

            entity.ToTable("tipo_solicitud");

            entity.Property(e => e.TposId).HasColumnName("tpos_id");
            entity.Property(e => e.TposEstado)
                .HasDefaultValue(1)
                .HasColumnName("tpos_estado");
            entity.Property(e => e.TposNombre)
                .HasMaxLength(255)
                .HasColumnName("tpos_nombre");
            entity.Property(e => e.TposSiglas)
                .HasMaxLength(255)
                .HasColumnName("tpos_siglas");
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
