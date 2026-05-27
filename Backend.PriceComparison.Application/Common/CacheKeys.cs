namespace Backend.PriceComparison.Application.Common;

public static class CacheKeys
{
    public const string ProductsPrefix = "products";

    public static string ProductById(int id) => $"product:{id}";
    public static string ProductsPage(int pageNumber, int pageSize) => $"{ProductsPrefix}:page:{pageNumber}:size:{pageSize}";
    public static string ProductsByStore(int storeId, int pageNumber, int pageSize) => $"products:store:{storeId}:page:{pageNumber}:size:{pageSize}";


    public const string CategoryProductsPrefix = "categoryproducts";
    public static string CategoryProductById(int id) => $"categoryproduct:{id}";
}
