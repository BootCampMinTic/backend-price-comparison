namespace Backend.PriceComparison.Application.Common;

public static class CacheKeys
{
    public const string ProductsPrefix = "products";
    public const string SalesPrefix = "sales";

    public static string ProductById(int id) => $"product:{id}";
    public static string ProductsPage(int pageNumber, int pageSize) => $"{ProductsPrefix}:page:{pageNumber}:size:{pageSize}";
    public static string ProductsByStore(int storeId, int pageNumber, int pageSize) => $"products:store:{storeId}:page:{pageNumber}:size:{pageSize}";

    public static string SaleById(int id) => $"sale:{id}";
    public static string SalesPage(int pageNumber, int pageSize) => $"{SalesPrefix}:page:{pageNumber}:size:{pageSize}";

    public const string CategoryProductPrefix = "categories:product";
    public const string CategoryStorePrefix = "categories:store";

    public static string CategoryProductById(int id) => $"category:product:{id}";
    public static string CategoryProductPage(int pageNumber, int pageSize) => $"{CategoryProductPrefix}:page:{pageNumber}:size:{pageSize}";

    public static string CategoryStoreById(int id) => $"category:store:{id}";
    public static string CategoryStorePage(int pageNumber, int pageSize) => $"{CategoryStorePrefix}:page:{pageNumber}:size:{pageSize}";
}
