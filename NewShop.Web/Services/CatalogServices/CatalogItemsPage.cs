namespace NewShop.Web.Services.CatalogServices;

public record CatalogItemsPage(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);