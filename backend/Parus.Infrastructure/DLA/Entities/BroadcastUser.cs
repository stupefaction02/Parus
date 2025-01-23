using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DLA.Entities
{
	public class BroadcastUser : Parus.Core.Entities.BroadcastUser
	{
        public ParusUser User { get; set; }
    }
}
