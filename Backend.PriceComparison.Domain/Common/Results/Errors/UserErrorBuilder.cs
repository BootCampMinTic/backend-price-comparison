using System.Net;

namespace Backend.PriceComparison.Domain.Common.Results.Errors;

public static class UserErrorBuilder
{
    private const string NotFoundCode = "UserRecordNotFound";
    private const string CreationErrorCode = "UserCreationError";

    public static Error NotFound(int id, string entity) => Error.CreateInstance(
        NotFoundCode,
        $"{entity} with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error NoRecordsFound(string entity) => Error.CreateInstance(
        NotFoundCode,
        $"No {entity} records were found.",
        HttpStatusCode.NotFound);

    public static Error CreationFailed(string entity) => Error.CreateInstance(
        CreationErrorCode,
        $"Failed to create {entity} due to an internal error.",
        HttpStatusCode.InternalServerError);
}
