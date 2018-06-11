namespace PlasticFreeOcean.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string password { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsNeedtoReset { get; set; }
    }
}