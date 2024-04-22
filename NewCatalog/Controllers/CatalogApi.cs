namespace NewCatalog.Controllers;
public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {

        // Routes for querying catalog items.
        app.MapGet("/items", GetAllItems);
        app.MapGet("/items/by", GetItemsByIds);
        app.MapGet("/items/{id:int}", GetItemById);
        app.MapGet("/items/by/{name:minlength(1)}", GetItemsByName);
        app.MapGet("/items/{catalogItemId:int}/images", GetItemPictureById);

        // Routes for resolving catalog items by type and brand.
        app.MapGet("/items/type/{typeId}/brand/{brandId?}", GetItemsByBrandAndTypeId);
        app.MapGet("/items/type/all/brand/{brandId:int?}", GetItemsByBrandId);
        app.MapGet("/catalogtype", async (CatalogContext context) => await context.CatalogType.OrderBy(x => x.Type).ToListAsync());
        app.MapGet("/catalogbrand", async (CatalogContext context) => await context.CatalogBrand.OrderBy(x => x.Brand).ToListAsync());

        // Routes for modifying catalog items.
        app.MapPut("/items", UpdateItem);
        app.MapPost("/items", CreateItem);
        app.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static async Task<Results<NotFound, PhysicalFileHttpResult>> GetItemPictureById(CatalogContext context, IWebHostEnvironment environment, int catalogItemId)
    {
        var item = await context.CatalogItem.FindAsync(catalogItemId);

        if (item is null)
        {
            return TypedResults.NotFound();
        }

        var path = GetFullPath(environment.ContentRootPath, item.PictureFileName);

        return TypedResults.PhysicalFile(path, "image/jpeg");
    }

    public static async Task<Results<Ok<PaginatedItems<CatalogItem>>, BadRequest<string>>> GetAllItems(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogService services)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = await services.Context.CatalogItem
            .LongCountAsync();

        var itemsOnPage = await services.Context.CatalogItem
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds(
        [AsParameters] CatalogService services,
        int[] ids)
    {
        var items = await services.Context.CatalogItem.Where(item => ids.Contains(item.Id)).ToListAsync();
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok<CatalogItem>, NotFound, BadRequest<string>>> GetItemById(
        [AsParameters] CatalogService services,
        int id)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var item = await services.Context.CatalogItem.Include(ci => ci.CatalogBrand).SingleOrDefaultAsync(ci => ci.Id == id);

        if (item == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(item);
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogService services,
        string name)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = await services.Context.CatalogItem
            .Where(c => c.Name.StartsWith(name))
            .LongCountAsync();

        var itemsOnPage = await services.Context.CatalogItem
            .Where(c => c.Name.StartsWith(name))
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogService services,
        int typeId,
        int? brandId)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var root = (IQueryable<CatalogItem>)services.Context.CatalogItem;
        root = root.Where(c => c.CatalogTypeId == typeId);
        if (brandId is not null)
        {
            root = root.Where(c => c.CatalogBrandId == brandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandId(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogService services,
        int? brandId)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var root = (IQueryable<CatalogItem>)services.Context.CatalogItem;

        if (brandId is not null)
        {
            root = root.Where(ci => ci.CatalogBrandId == brandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Results<Created, NotFound<string>>> UpdateItem(
        [AsParameters] CatalogService services,
        CatalogItem productToUpdate)
    {
        var catalogItem = await services.Context.CatalogItem.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

        if (catalogItem == null)
        {
            return TypedResults.NotFound($"Item with id {productToUpdate.Id} not found.");
        }

        // Update current product
        var catalogEntry = services.Context.Entry(catalogItem);
        catalogEntry.CurrentValues.SetValues(productToUpdate);
        return TypedResults.Created($"/api/v1/catalog/items/{productToUpdate.Id}");
    }

    public static async Task<Created> CreateItem(
        [AsParameters] CatalogService services,
        CatalogItem product)
    {
        var item = new CatalogItem
        {
            Id = product.Id,
            CatalogBrandId = product.CatalogBrandId,
            CatalogTypeId = product.CatalogTypeId,
            Description = product.Description,
            Name = product.Name,
            PictureFileName = product.PictureFileName,
            Price = product.Price,
            AvailableStock = product.AvailableStock,
            RestockThreshold = product.RestockThreshold,
            MaxStockThreshold = product.MaxStockThreshold
        };
        // item.Embedding = await services.CatalogAI.GetEmbeddingAsync(item);

        services.Context.CatalogItem.Add(item);
        await services.Context.SaveChangesAsync();

        return TypedResults.Created($"/api/v1/catalog/items/{item.Id}");
    }

    public static async Task<Results<NoContent, NotFound>> DeleteItemById(
        [AsParameters] CatalogService services,
        int id)
    {
        var item = services.Context.CatalogItem.SingleOrDefault(x => x.Id == id);

        if (item is null)
        {
            return TypedResults.NotFound();
        }

        services.Context.CatalogItem.Remove(item);
        await services.Context.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static string GetFullPath(string contentRootPath, string pictureFileName)
    {
        if (string.IsNullOrEmpty(contentRootPath) || string.IsNullOrEmpty(pictureFileName))
            // Handle null or empty values gracefully, e.g., throw an exception or return an error response.
            throw new ArgumentNullException("contentRootPath or pictureFileName is null or empty.");


        return Path.Combine(contentRootPath, "Images", pictureFileName);
    }
}