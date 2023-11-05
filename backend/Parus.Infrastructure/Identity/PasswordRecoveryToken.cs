using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parus.Core.Interfaces.Repositories;

namespace Parus.Infrastructure.Identity
{
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