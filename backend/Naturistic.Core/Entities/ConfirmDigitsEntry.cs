using System;
using System.Collections.Generic;
using System.Text;

namespace Naturistic.Core.Entities
{
    public class ConfirmDigitsEntry
    {
        public int ConfirmDigitsEntryId { get; set; }

        public string IdentityUserId { get; set; }

        public int ConfirmDigit { get; set; } 
    }
}
