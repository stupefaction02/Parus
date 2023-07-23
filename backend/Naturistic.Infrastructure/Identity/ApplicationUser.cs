using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Naturistic.Core.Entities;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string ChatColor { get; set; }

        [NotMapped]
        public string Username { get => base.UserName; set => base.UserName = value; }

		[NotMapped]
		public new string Email { get => base.Email; set => base.Email = value; }

		public new bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        public override string ToString()
        {
            return $"{this.UserName} {this.Email} {this.Id}";
        }
    }
}