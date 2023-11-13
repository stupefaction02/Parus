using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Parus.Core.Entities;

namespace Parus.Infrastructure.Identity
{
	public class ApplicationUser : IdentityUser, IUser
    {
		#region Broadcast/Viewer fields

		[MaxLength(128)]
		public string AvatarPath { get; set; }

		#endregion

		public string ChatColor { get; set; }

        public string GetUsername() { return base.UserName; }

        public string GetEmail() { return base.Email; }

        public string GetId() { return base.Id; }

		public new bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        public ConfirmCode ConfirmCode { get; set; }

        public PasswordRecoveryToken PasswordRecoveryToken { get; set; }

        [NotMapped]
        public IVerificationCode ConfirmCodeCore { get => ConfirmCode; }

        public RefreshSession RefreshSession { get; set; }

        public override string ToString()
        {
            return $"{this.UserName} {this.Email} {this.Id}";
        }
    }
}