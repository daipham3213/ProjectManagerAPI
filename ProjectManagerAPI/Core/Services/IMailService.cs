
using ProjectManagerAPI.Core.ServiceResource;

namespace ProjectManagerAPI.Core.Services
{
    public interface IMailService
    {
        public System.Threading.Tasks.Task SendEmailAsync(MailRequest mailRequest);
    }
}
