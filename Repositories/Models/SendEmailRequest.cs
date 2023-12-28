namespace Repositories.Models
{
    public class SendEmailRequest
    {
        public SendEmailRequest(string to, string subject, string body, string? code)
        {
            To = to;
            Subject = subject;
            Body = body;
            Code = code;
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string? Code { get; set; }
    }
}
