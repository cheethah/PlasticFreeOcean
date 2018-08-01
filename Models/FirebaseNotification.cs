using System;
namespace PlasticFreeOcean.Models
{
    public class FirebaseNotification : BaseModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Device Device { get; set; }
    }
}
