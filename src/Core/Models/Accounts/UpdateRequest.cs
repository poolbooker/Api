using System.ComponentModel.DataAnnotations;
using Pb.Api.Entities;

namespace Pb.Api.Models.Accounts
{
    public class UpdateRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _email;
        private string _phone;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Role? Role { get; set; }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }

        public string Phone
        {
            get => _phone;
            set => _phone = ReplaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = ReplaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = ReplaceEmptyWithNull(value);
        }

        public CustomerType CustomerType { get; set; }

        private string ReplaceEmptyWithNull(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}