namespace Parus.Core.Entities
{
	public interface IUser
    {
        public string GetUsername();

        public string GetEmail();

		string GetId();

		public bool EmailConfirmed { get; set; }

        public IVerificationCode ConfirmCodeCore { get; }
    }
}