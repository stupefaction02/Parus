using System;
using System.ComponentModel.DataAnnotations;

namespace Naturistic.Core.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        public string Body { get; set; }

        public int ChatId { get; set; }
    }
}