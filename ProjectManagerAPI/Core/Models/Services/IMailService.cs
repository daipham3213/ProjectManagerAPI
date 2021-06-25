using ProjectManagerAPI.Core.Models.ServiceResource;

namespace ProjectManagerAPI.Core.Models.Services
{
    public interface IMailService
    {
        public System.Threading.Tasks.Task SendEmailAsync(MailRequest mailRequest);
    }
}
