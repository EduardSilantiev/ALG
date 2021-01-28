using Newtonsoft.Json;
using System.Collections.Generic;

namespace ALG.Application.Helpers.Exceptions
{
    public class ExceptionMessage
    {
        public string Message { get; set; }

        public ExceptionMessage() { }

        public ExceptionMessage(string message)
        {
            Message = message;
        }

        public ExceptionMessage(IEnumerable<string> messages)
        {
            Message = string.Join("; ", messages);
        }

        public override string ToString() => JsonConvert.SerializeObject(new { message = new string(Message) });
    }
}
