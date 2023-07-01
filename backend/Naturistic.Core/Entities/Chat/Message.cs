using System;

namespace Naturistic.Core.Entities
{
    public class Message
    {   
        public int MessageId { get; set; }

        public string Body { get; set; }

        public DateTime TimeStamp { get; set; }

        public string SenderName { get; set; }

        public string SenderNameColor { get; set; }

        public long SenderId { get; set; }

        public ViewerUser Sender { get; set; }
    }
}