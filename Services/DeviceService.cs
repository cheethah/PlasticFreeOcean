using System;
using Microsoft.EntityFrameworkCore;
using PlasticFreeOcean.Models;

namespace PlasticFreeOcean.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork<PlasticFreeOceanContext> _unitOfWork;
        public DeviceService(IUnitOfWork<PlasticFreeOceanContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(string DeviceId, Guid UserId)
        {
            _unitOfWork.GetRepository<Device>().Insert(new Device
            {
                Id = Guid.NewGuid(),
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                DeviceId = DeviceId,
                IsActive = true,
                UserId = UserId,

            });
            _unitOfWork.SaveChanges();
        }
    }
}
