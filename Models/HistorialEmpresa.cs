using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class HistorialEmpresa
{
    public int HistId { get; set; }

    public int HistEmpresaId { get; set; }

    public int HistUsuarioId { get; set; }

    public DateTime? HistFechaActualizacion { get; set; }

    public decimal? HistActivoC { get; set; }

    public decimal? HistActivoF { get; set; }

    public decimal? HistActivoD { get; set; }

    public decimal? HistActivoO { get; set; }

    public decimal? HistActivoT { get; set; }

    public decimal? HistPasivoC { get; set; }

    public decimal? HistPasivoLp { get; set; }

    public decimal? HistPasivoD { get; set; }

    public decimal? HistPasivoT { get; set; }

    public decimal? HistCapital { get; set; }

    public decimal? HistReserva { get; set; }

    public decimal? HistOtrasCp { get; set; }

    public decimal? HistUtilidadesA { get; set; }

    public decimal? HistUtilidadE { get; set; }

    public decimal? HistPerdidas { get; set; }

    public decimal? HistOtrasPer { get; set; }

    public decimal? HistPatrimonioT { get; set; }

    public decimal? HistPatrimonioPasivo { get; set; }

    public decimal? HistVentas { get; set; }

    public decimal? HistUtilidad { get; set; }

    public decimal? HistCupoRestante { get; set; }

    public string? HistObservacion { get; set; }

    public decimal? HistAnfLiquidez { get; set; }

    public decimal? HistAnfSolvencia { get; set; }

    public decimal? HistAnfCapCobertura { get; set; }

    public decimal? HistAnfEndeudamiento { get; set; }

    public decimal? HistAnfCapitalPropio { get; set; }

    public decimal? HistAnfRoa { get; set; }

    public decimal? HistAnfRoe { get; set; }

    public string? HistAnfAnalisisSf { get; set; }

    public string? HistClempClienteC { get; set; }

    public string? HistClempClasificacion { get; set; }

    public string? HistClempRango { get; set; }

    public byte[]? HistLempArchivoSoporte { get; set; }
}
