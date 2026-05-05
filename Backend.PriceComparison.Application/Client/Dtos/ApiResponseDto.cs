namespace Backend.PriceComparison.Application.Client.Dtos
{
    public class ApiResponseDto<T>
    {
        public string Status { get; set; } = "success";
        public string Message { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public T? Data { get; set; }

        public ApiResponseDto() { }

        public ApiResponseDto(T data, string message = "Request successful")
        {
            Status = "success";
            Message = message;
            Data = data;
        }

        public ApiResponseDto(string message, string status = "error")
        {
            Status = status;
            Message = message;
        }
    }
}
