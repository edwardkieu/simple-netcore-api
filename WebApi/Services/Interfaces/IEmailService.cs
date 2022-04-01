using System.Threading.Tasks;
using WebApi.ViewModels.Email;

namespace WebApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
