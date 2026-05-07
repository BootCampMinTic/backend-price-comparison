namespace Backend.PriceComparison.Application.Client;

public static class CacheKeys
{
    public const string DocumentTypesAll = "documenttypes:all";

    public const string ClientsNaturalPrefix = "clients:natural";
    public const string ClientsLegalPrefix = "clients:legal";

    public static string ClientById(int id, string type) => $"client:{type}:{id}";
    public static string ClientByDocumentNumber(string documentNumber, string type) => $"client:{type}:document:{documentNumber}";
    public static string ClientsNaturalPage(int pageNumber, int pageSize) => $"{ClientsNaturalPrefix}:page:{pageNumber}:size:{pageSize}";
    public static string ClientsLegalPage(int pageNumber, int pageSize) => $"{ClientsLegalPrefix}:page:{pageNumber}:size:{pageSize}";

    public const string StatesAll = "states:all";
    public const string TypeUsersAll = "typeusers:all";
    public const string CategoryProductsAll = "categoryproducts:all";
    public const string CategoryStoresAll = "categorystores:all";

    public const string StoresPrefix = "stores";
    public const string ProductsPrefix = "products";
    public const string UsersPrefix = "users";
    public const string SalesPrefix = "sales";

    public static string StoreById(int id) => $"store:{id}";
    public static string StoresPage(int pageNumber, int pageSize) => $"{StoresPrefix}:page:{pageNumber}:size:{pageSize}";
    public static string ProductById(int id) => $"product:{id}";
    public static string ProductsPage(int pageNumber, int pageSize) => $"{ProductsPrefix}:page:{pageNumber}:size:{pageSize}";
    public static string ProductsByStore(int storeId, int pageNumber, int pageSize) => $"products:store:{storeId}:page:{pageNumber}:size:{pageSize}";
    public static string UserById(int id) => $"user:{id}";
    public static string UsersAll => "users:all";
    public static string SaleById(int id) => $"sale:{id}";
    public static string SalesPage(int pageNumber, int pageSize) => $"{SalesPrefix}:page:{pageNumber}:size:{pageSize}";
}
