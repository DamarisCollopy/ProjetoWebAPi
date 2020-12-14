using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage) 
        {
            await Task.CompletedTask;
        }
    }
}