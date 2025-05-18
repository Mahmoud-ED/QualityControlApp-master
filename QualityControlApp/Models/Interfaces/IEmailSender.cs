//using Microsoft.DotNet.Scaffolding.Shared.Messaging;  // الانتباه من اضافة هذه المكتبة بدلا من النيم سبيس 

using QualityControlApp.Classes;

namespace QualityControlApp.Models.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
