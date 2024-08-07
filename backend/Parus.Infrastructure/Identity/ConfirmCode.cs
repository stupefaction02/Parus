using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Parus.Common.Utils;
using Parus.Core.Entities;
using Parus.Infrastructure;

namespace Parus.Infrastructure.Identity
{
    public class ConfirmCode : IVerificationCode
    {
        [Key]
        public int ConfirmCodeId { get; set; }

        public ApplicationUser User { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        public int Code { get; set; } 
    }

    public class TwoFactoryEmailVerificationCode : IVerificationCode
    {
        [Key]
        public int ConfirmCodeId { get; set; }

        public ApplicationUser User { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        public int Code { get; set; }
    }

    public class TwoFactoryCustomerKey
    {
        [Key]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Column(TypeName = "nvarchar(72)")]
        public string Key { get; set; }
    }

    public class RefreshSession
    {
        [NonSerialized]
        [NotMapped]
        public static TimeSpan LifeTime = new TimeSpan(365000);


        [Key]
        public int RefreshTokenId { get; set; }

        public ApplicationUser User { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        public string Token { get; set; }

        public string Fingerprint { get; set; }

        public int ExpiresAt { get; set; }

        public static RefreshSession CreateDefault(string fingerPrint, ApplicationUser user)
        {
            return new RefreshSession
            {
                // TODO: Replace with something more efficeint
                Token = GenerateToken(),
                Fingerprint = fingerPrint,
                ExpiresAt = DateTimeUtils.ToUnixTimeSeconds(DateTime.Now.Add(RefreshSession.LifeTime)),
                User = user
            };
        }

        public static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
