using System;
using System.Collections.Generic;

namespace fianzas_app.Models;

public partial class SolicitudFianzaPrendum
{
    public int SfpId { get; set; }

    public int? SfpSfId { get; set; }

    public int? SfpPrenId { get; set; }

    public virtual Prendum? SfpPren { get; set; }

    public virtual SolicitudFianza? SfpSf { get; set; }
}
