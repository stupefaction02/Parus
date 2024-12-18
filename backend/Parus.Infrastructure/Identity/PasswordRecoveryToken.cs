using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parus.Common.Utils;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;

namespace Parus.Infrastructure.Identity
{
	public class PasswordRecoveryToken : IPasswordRecoveryToken
    {
        [NotMapped]
        public static int LifetimeHours => 6;

        [Required]
        public string UserId { get; set; }
        public ParusUser User { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Token { get; set; }

        [Required]
        public long ExpireAt { get; set; }

        public string GetToken()
        {
            return Token;
        }

        public long GetExpiresAt()
        {
            return ExpireAt;
        }

        public IUser GetUser()
        {
            return User;
        }

        public bool Validate(out string errorMessage)
        {
            int expires = (int)ExpireAt;

            DateTime now = DateTime.UtcNow;

            if (DateTimeUtils.ToUnixTimeSeconds(now) > expires)
            {
                errorMessage = "Token is expired";
                return false;
            }

            errorMessage = "";

            return true;
        }
    }
}