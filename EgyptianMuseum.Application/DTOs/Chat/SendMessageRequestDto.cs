namespace EgyptianMuseum.Application.DTOs.Chat
{
    public class SendMessageRequestDto
    {
        public string SenderType { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
