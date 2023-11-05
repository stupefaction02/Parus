using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
}
