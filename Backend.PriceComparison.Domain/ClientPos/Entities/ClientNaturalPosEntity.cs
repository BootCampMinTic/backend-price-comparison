namespace Backend.PriceComparison.Domain.ClientPos.Entities
{
    public class ClientNaturalPosEntity: ClientEntity
    {
        public string? Name { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? SecondSurname { get; set; }
        public string? DocumentCountry { get; set; }
    }
}
