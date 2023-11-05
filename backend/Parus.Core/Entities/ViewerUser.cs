namespace Parus.Core.Entities
{
    // can watch broadcast, can chat in chats
    public class ViewerUser
    {   
        public long Id { get; set; }
        public string IdentityUserId { get; set; }
    }

    public enum Gender : sbyte
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
}