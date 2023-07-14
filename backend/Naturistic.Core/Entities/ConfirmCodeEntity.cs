using System;
using System.Collections.Generic;
using System.Text;

namespace Naturistic.Core.Entities
{
    public class ConfirmCodeEntity
    {
        public int ConfirmCodeEntityId { get; set; }

        public string UserEmail { get; set; }

        public int Code { get; set; } 
    }
}
