namespace PIM.Api.DTOs.Responses
{
    public class FeedbackDto
    {
        public int IdFeedback { get; set; }
        public int Nota { get; set; }
        public string? Comentario { get; set; }
        public DateTime DataFeedback { get; set; }
    }
}
