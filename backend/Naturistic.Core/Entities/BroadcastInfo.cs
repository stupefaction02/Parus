using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Naturistic.Core.Entities
{
    public class BroadcastInfo
    {
        public string Ref { get; set; }

        [Key]
        public int Id { get; set; }
		
		public string Username { get; set; }

		public string AvatarPic { get; set; }

        public string Preview { get; set; }

        public string Title { get; set; }

        public List<Tag> Tags { get; set; }

        public BroadcastCategory Category { get; set; }
    }

    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<BroadcastInfo> Broadcasts { get; set; }
    }

    public class BroadcastCategory
    {
        [Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public List<BroadcastInfo> Broadcasts { get; set; }
	}
}
