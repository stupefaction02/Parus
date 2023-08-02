using System;
using System.Collections.Generic;
using System.Text;

namespace Naturistic.Core.Entities
{
    public class ConfirmCodeEntity
    {
        public int ConfirmCodeEntityId { get; set; }

        public string Username { get; set; }

        public int Code { get; set; } 
    }
}
