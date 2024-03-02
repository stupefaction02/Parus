namespace Parus.Core.Entities
{
	public interface IUser
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