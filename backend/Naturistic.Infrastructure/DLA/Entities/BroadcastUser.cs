using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Entities
{
	public class BroadcastUser : Naturistic.Core.Entities.BroadcastUser
	{
        public ApplicationUser User { get; set; }
    }
}
