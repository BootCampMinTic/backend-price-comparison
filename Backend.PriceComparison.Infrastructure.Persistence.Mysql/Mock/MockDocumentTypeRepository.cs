using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;

public class MockDocumentTypeRepository : IDocumentTypeRepository
{
    public Task<Result<IEnumerable<DocumentTypeEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var data = new List<DocumentTypeEntity>
        {
            new() { Id = 1, Name = "Cedula de ciudadania", DocumentType = "CC", HelpTextHeader = "Ingrese su cedula", HelpText = "Numero de identificacion", Regex = "^[0-9]{6,10}$", Fields = "numero" },
            new() { Id = 2, Name = "Cedula de extranjeria", DocumentType = "CE", HelpTextHeader = "Ingrese su cedula de extranjeria", HelpText = "Numero de identificacion extranjera", Regex = "^[0-9]{6,10}$", Fields = "numero" },
            new() { Id = 3, Name = "NIT", DocumentType = "NIT", HelpTextHeader = "Ingrese el NIT", HelpText = "Numero de identificacion tributaria", Regex = "^[0-9]{9,10}$", Fields = "numero,dv" }
        };

        return Task.FromResult<Result<IEnumerable<DocumentTypeEntity>, Error>>(data);
    }
}
