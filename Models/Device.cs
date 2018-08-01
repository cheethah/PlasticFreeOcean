using System;
namespace PlasticFreeOcean.Models
{
    public class Device : BaseModel
    {
        public string DeviceId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
