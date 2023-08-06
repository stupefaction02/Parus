using System;
using System.Collections.Generic;
using System.Text;

namespace Naturistic.Core.Entities
{
    public class ConfirmCodeEntity
    {
        public int ConfirmCodeEntityId { get; set; }

        public IUser User { get; set; }

        public int UserId { get; set; }

        public int Code { get; set; } 
    }
}
