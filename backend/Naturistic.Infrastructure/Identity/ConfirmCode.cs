using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Naturistic.Core.Entities;
using Naturistic.Infrastructure;

namespace Naturistic.Infrastructure.Identity
{
    public class ConfirmCode : IConfirmCode
    {
        [Key]
        public int ConfirmCodeId { get; set; }

        public ApplicationUser User { get; set; }

        [Column(TypeName = "varchar(256)")]
        public string UserId { get; set; }

        public int Code { get; set; } 
    }
}
