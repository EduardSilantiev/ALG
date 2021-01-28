namespace ALG.Application.Helpers
{
    public class CanBeProcessedDto
    {
        public bool CanBeProcessed { get; set; }
        public string RejectionReason { get; set; }
        public int StatusCode { get; set; }
    }
}
