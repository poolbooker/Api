using System.ComponentModel.DataAnnotations;

namespace Pb.Api.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}