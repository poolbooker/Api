using System.ComponentModel.DataAnnotations;

namespace Pb.Api.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}