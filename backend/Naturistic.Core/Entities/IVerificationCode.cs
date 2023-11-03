namespace Parus.Core.Entities
{
	public interface IVerificationCode
    {
        int Code { get; set; }
        string UserId { get; set; }
    }
}