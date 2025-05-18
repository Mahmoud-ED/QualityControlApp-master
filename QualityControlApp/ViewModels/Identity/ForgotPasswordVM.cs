using System.ComponentModel.DataAnnotations;

namespace QualityControlApp.ViewModels.Identity
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public required string Email { get; set; }

    }
}
