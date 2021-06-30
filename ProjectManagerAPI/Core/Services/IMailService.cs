
using System.Threading.Tasks;
using ProjectManagerAPI.Core.ServiceResource;

namespace ProjectManagerAPI.Core.Services
{
    public interface IMailService
    {
        public Task SendEmailAsync(MailRequest mailRequest);
    }
}
