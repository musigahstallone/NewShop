﻿namespace NewShop.Web.Utilities.Services.CatalogServe;

public record CatalogItem(
        int Id,
        string Name,
        string Description,
        decimal Price,
        string PictureUrl,
        int CatalogBrandId,
        CatalogBrand CatalogBrand,
        int CatalogTypeId,
        CatalogItemType CatalogType);

public record CatalogResult(int PageIndex, int PageSize, int Count, List<CatalogItem> Data);
public record CatalogBrand(int Id, string Brand);
public record CatalogItemType(int Id, string Type);