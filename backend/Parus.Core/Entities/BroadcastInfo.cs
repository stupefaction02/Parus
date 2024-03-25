using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

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

    // For all search results, for ElasticDto and normal MSSQL output (our model BroadcastInfo)
    public interface IBroadcastsInfoSearchResult
    {
        int Id { get; set; }
        string Username { get; set; }
        string CategoryName { get; set; }
        string AvatarPic { get; set; }
        string Preview { get; set; }
        string Title { get; set; }
        List<Tag> Tags { get; set; }
    }

    public class BroadcastInfoElasticDto : IBroadcastsInfoSearchResult
    {
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("catId")]
        public string CategoryName { get; set; }

        [JsonPropertyName("ava")]
        public string AvatarPic { get; set; }

        [JsonPropertyName("preview")]
        public string Preview { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }

    public class BroadcastInfo : IBroadcastsInfoSearchResult
    {
        /// <summary>
        /// ref: <see cref="Parus.Core.Entities.IndexingRule"/>
        /// </summary>
        public byte IndexingStatus { get; set; }

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

        [NotMapped]
        public string CategoryName { get; set; }

        //public byte IndexingStatus { get; set; }

        public override string ToString()
        {
            return $"{Username}";
        }

        public BroadcastInfoElasticDto ElasticDto()
        {
            return new BroadcastInfoElasticDto 
            {
                Title = Title,
                AvatarPic = AvatarPic,
                Id = Id,
                Username = Username,
                Preview = Preview,
                Tags = Tags,
                CategoryName = Category == null ? "" : Category.Name
            };
        }
    }

    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public List<BroadcastInfo> Broadcasts { get; set; }
    }

    public class BroadcastCategory
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("name")]
		public string Name { get; set; }

        [JsonIgnore]
        public List<BroadcastInfo> Broadcasts { get; set; }
        
        public string GetRef() { return Name.ToLower(); }

        internal object ElasticDto()
        {
            return new BroadcastCategory 
            {
                
            };
        }

        [JsonPropertyName("views_count")]
        public int ViewsCount { get; set; } = 0;

        [JsonIgnore]
        public byte IndexingStatus { get; set; }
    }
}
