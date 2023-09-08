using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Naturistic.Core.Entities
{
    public class Chat
    {
        public Chat()
        {
            Channels = new HashSet<ViewerUser>();
            Messages = new HashSet<ChatMessage>();
        }

        //[ForeignKey("BroadcastUser")]
        public int ChatId { get; set; }

        public int BroadcastUserRefId { get; set; }

        public virtual BroadcastUser BroadcastUser { get; set; }   

        public ICollection<ViewerUser> Channels { get; set; }

        public ICollection<ChatMessage> Messages { get; set; }
    }
}