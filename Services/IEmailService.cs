using System.Threading.Tasks;

namespace PlasticFreeOcean.Services
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
        string CreateEmailBodyKonfirmasi(string userName, string link);
    }
}
