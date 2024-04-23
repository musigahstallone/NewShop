namespace NewShop.Web.Utilities.Item;
public static class ItemHelper
{
    public static string Url(CatalogItem item)
        => $"item/{item.Id}";
}