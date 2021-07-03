
using ProjectManagerAPI.Core.ServiceResource;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Services
{
    public interface IMailService
    {
        public Task SendEmailAsync(MailRequest mailRequest);
    }
}
