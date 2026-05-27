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
}
