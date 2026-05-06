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
}
