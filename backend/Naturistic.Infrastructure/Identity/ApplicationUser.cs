using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string ChatColor { get; set; }

        public string GetUsername() { return base.UserName; }

        public string GetEmail() { return base.Email; }

		public new bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        public ConfirmCode ConfirmCode { get; set; }

        public PasswordRecoveryToken PasswordRecoveryToken { get; set; }

        [NotMapped]
        public IConfirmCode ConfirmCodeCore { get => ConfirmCode; }

        public override string ToString()
        {
            return $"{this.UserName} {this.Email} {this.Id}";
        }
    }

    public class PasswordRecoveryToken : IPasswordRecoveryToken
    {
        [NotMapped]
        public static int LifetimeHours => 6;

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Token { get; set; }

        [Required]
        public long ExpireAt { get; set; }
    }
}