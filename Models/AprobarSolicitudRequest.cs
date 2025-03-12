namespace fianzas_app.Models
{
    public class AprobarSolicitudRequest
    {
        public int SfId { get; set; }                // ID de la solicitud de fianza
        public string TipoAprobacion { get; set; }   // 'LEGAL' o 'TECNICA'
        public bool Aprobado { get; set; }           // true (1) = aprobado, false (0) = rechazado
        public int UsuarioId { get; set; }           // ID del usuario que realiza la acción
        public string Observacion { get; set; }      // Observación opcional
    }

}
