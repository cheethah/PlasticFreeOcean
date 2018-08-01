using System;
namespace PlasticFreeOcean.Models
{
    public class BaseModel
    {
        public Guid Id {get;set;}
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
