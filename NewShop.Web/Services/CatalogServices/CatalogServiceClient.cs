using System.Globalization;

namespace NewShop.Web.Services.CatalogServices;

public class CatalogServiceClient(HttpClient client)
{
    public Task<CatalogItemsPage?> GetItemsAsync(int? before = null, int? after = null)
    {
        // Make the query string with encoded parameters
        var query = (before, after) switch
        {
            (null, null) => default,
            (int b, null) => QueryString.Create("before", b.ToString(CultureInfo.InvariantCulture)),
            (null, int a) => QueryString.Create("after", a.ToString(CultureInfo.InvariantCulture)),
            _ => throw new InvalidOperationException(),
        };

        return client.GetFromJsonAsync<CatalogItemsPage>($"api/v1/catalog/items/type/all/brand{query}");
    }
}

/*

public record CatalogItemsPage(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);

public record CatalogItem {
public int Id { get; init; }
public required string Name { get; init; }
public required string Description { get; init; }
public decimal Price { get; init; }
public string? PictureUri { get; init; }
public int CatalogBrandId { get; init; }
public required string CatalogBrand { get; init; }
public int CatalogTypeId { get; init; }
public required string CatalogType { get; init; } }

public class CatalogServiceClient(HttpClient client)
{
private readonly HttpClient _client = client;

public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync()
{
    return await _client.GetFromJsonAsync<IEnumerable<CatalogItem>>("api/v1/catalog") ?? [];
}

public async Task<CatalogItem> GetCatalogItemByIdAsync(int id)
{
    return await _client.GetFromJsonAsync<CatalogItem>($"api/v1/catalog/{id}");
}

public async Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync()
{
    return await _client.GetFromJsonAsync<IEnumerable<CatalogBrand>>("api/v1/catalog/brands");
}

public async Task<CatalogBrand> GetCatalogBrandByIdAsync(int id)
{
    return await _client.GetFromJsonAsync<CatalogBrand>($"api/v1/catalog/brands/{id}");
}

public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync()
{
    return await _client.GetFromJsonAsync<IEnumerable<CatalogType>>("api/v1/catalog/types");
}

public async Task<CatalogType> GetCatalogTypeByIdAsync(int id)
{
    return await _client.GetFromJsonAsync<CatalogType>($"api/v1/catalog/types/{id}");
}

public async Task<CatalogItemsPage?> GetItemsAsync(int? before = null, int? after = null)
{
    // Make the query string with encoded parameters
    var query = (before, after) switch
    {
        (null, null) => default,
        (int b, null) => $"?before={b}",
        (null, int a) => $"?after={a}",
        _ => throw new InvalidOperationException(),
    };

    return await _client.GetFromJsonAsync<CatalogItemsPage>($"api/v1/catalog/items{query}");
}
}

public class CatalogBrand
{
}

public class CatalogType
{
}
*/