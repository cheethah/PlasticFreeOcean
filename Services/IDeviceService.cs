using System;
namespace PlasticFreeOcean.Services
{
    public interface IDeviceService
    {
        void Create(string DeviceId, Guid UserId);
    }
}
