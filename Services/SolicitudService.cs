using fianzas_app.Models;

namespace fianzas_app.Services
{
    public class SolicitudService
    {
           private readonly AppFianzasContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SolicitudService> _logger;

        private readonly ListaService _listaService;

        public SolicitudService(AppFianzasContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<SolicitudService> logger, ListaService listaService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _listaService = listaService;
        }
    }
}
