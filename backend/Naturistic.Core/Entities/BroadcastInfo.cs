using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Parus.Core.Entities
{
    public class BroadcastInfoKeyword
    {
        public string Keyword { get; set; }

        public BroadcastInfo BroadcastInfo { get; set; }
        public int BroadcastInfoId { get; set; }

        [Key]
        public int Id { get; set; }
    }

    public class BroadcastInfo
    {
        public string HostUserId { get; set; }

        public string Ref { get; set; }

        [Key]
        public int Id { get; set; }
		
		public string Username { get; set; }

		public string AvatarPic { get; set; }

        public string Preview { get; set; }

        public string Title { get; set; }

        public List<Tag> Tags { get; set; }

        public BroadcastCategory Category { get; set; }

        public override string ToString()
        {
            return $"{Username}";
        }
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
        
        public string GetRef() { return Name.ToLower(); }

        public int ViewsCount { get; set; } = 0;
    }
}
