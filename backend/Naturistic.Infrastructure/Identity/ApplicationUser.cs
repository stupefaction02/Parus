using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Naturistic.Core.Entities;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string ChatColor { get; set; }

        public string GetUsername() { return base.UserName; }

        public string GetEmail() { return base.Email; }

		public new bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        public ConfirmCodeEntity ConfirmCode { get; set; }

        public override string ToString()
        {
            return $"{this.UserName} {this.Email} {this.Id}";
        }
    }
}