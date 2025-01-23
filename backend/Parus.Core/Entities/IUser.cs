using System.Text.Json.Serialization;

namespace Parus.Core.Entities
{
    public interface IUserSearchResult
    {
        string Username { get; set; }
        string Avapath { get; set; }
        string Description { get; set; }
        int SubCountsStr { get; set; }
    }

    public class UserElasticDto : IUserSearchResult
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("avapath")]
        public string Avapath { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("subCountsStr")]
        public int SubCountsStr { get; set; }
    }

    public interface IUser// : IUserSearchResult
    {
        public string GetUsername();

        public string GetEmail();

		string GetId();

		public bool EmailConfirmed { get; set; }

        public IVerificationCode ConfirmCodeCore { get; }

        public bool GetTwoFactorEnabled();

        byte GetIndexingStatus();

        string GetAvatarPath();
        void SetIndexingRule(IndexingRule rule);
    }

    public enum IndexingRule
    {
        DoNothing = 0,
        AddToIndex = 1,
        Update = 2
    }
}