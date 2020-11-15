using System.Threading.Tasks;

namespace SportClassLibrary.Services
{
    public interface IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage) 
        {
            await Task.CompletedTask;
        }
    }
}