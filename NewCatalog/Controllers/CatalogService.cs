using NewCatalog.Data;

namespace NewCatalog;

public class CatalogService(CatalogContext context,ILogger<CatalogService> logger)
{
     public ILogger<CatalogService> Logger { get; } = logger;
    public CatalogContext Context { get; } = context;

    // ICatalogAI catalogAI,
    // IOptions<CatalogOptions> options,
    // ICatalogIntegrationEventService eventService)
    // public ICatalogAI CatalogAI { get; } = catalogAI;
    // public IOptions<CatalogOptions> Options { get; } = options;
    // public ICatalogIntegrationEventService EventService { get; } = eventService;
};
