namespace Backend.PriceComparison.Domain.ClientPos.Entities
{
    public class ClientLegalPosEntity: ClientEntity
    {
        public string? CompanyName { get; set; }
        public int VerificationDigit { get; set; }
        public bool VATResponsibleParty { get; set; }
        public bool LargeTaxpayer { get; set; }
        public bool SelfRetainer { get; set; }
        public bool WithholdingAgent { get; set; }
        public bool SimpleTaxRegime { get; set; }
        public string? DocumentCountry { get; set; }
    }
}
