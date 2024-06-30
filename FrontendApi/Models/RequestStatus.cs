namespace FrontendApi.Models
{
    public class RequestStatus
    {
        public required string Id { get; set; }
        public required bool IsCompleted { get; set; }
        public required string RedirectUrl { get; set; }
    }
}
