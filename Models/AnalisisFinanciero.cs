using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class AnalisisFinanciero
{
    public int AnfId { get; set; }

    public int AnfEmpresaId { get; set; }

    public decimal? AnfLiquidez { get; set; }

    public decimal? AnfSolvencia { get; set; }

    public decimal? AnfCapCobertura { get; set; }

    public decimal? AnfEndeudamiento { get; set; }

    public decimal? AnfCapitalPropio { get; set; }

    public decimal? AnfRoa { get; set; }

    public decimal? AnfRoe { get; set; }

    public string? AnfAnalisisSf { get; set; }

    public virtual Empresa AnfEmpresa { get; set; } = null!;
}
