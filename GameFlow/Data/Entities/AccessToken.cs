namespace GameFlow.Data.Entities
{
    public class AccessToken
    {
        public Guid Jti { get; set; }
        public Guid Sub { get; set; }
        public Guid Aud { get; set; }
        public DateTime Iat { get; set; } = DateTime.Now;
        public DateTime? Nbf { get; set; }
        public DateTime Exp { get; set; }
        public String? Iss { get; set; }
    }
}
