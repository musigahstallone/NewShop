namespace NewShop.Web.Services.CatalogServices;

public record CatalogItem
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public string? PictureUri { get; init; }
    public int CatalogBrandId { get; init; }
    public required string CatalogBrand { get; init; }
    public int CatalogTypeId { get; init; }
    public required string CatalogType { get; init; }
}