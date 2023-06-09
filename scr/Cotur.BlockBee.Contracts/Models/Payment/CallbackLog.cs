using System;

namespace Cotur.BlockBee.Contracts.Models.Payment
{
    public class CallbackLog
    {
        public string RequestUrl { get; set; }
        public string Response { get; set; }
        public string ResponseStatus { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime NextTry { get; set; }
        public bool Pending { get; set; }
        public bool Success { get; set; }
    }
}