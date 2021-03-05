using System.ComponentModel.DataAnnotations;

namespace Pb.Api.Models.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}