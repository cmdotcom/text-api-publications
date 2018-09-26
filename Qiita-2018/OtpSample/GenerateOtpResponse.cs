using System;

namespace OtpSample
{
    public class GenerateOtpResponse
    {
        public Guid id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime expireAt { get; set; }
    }
}