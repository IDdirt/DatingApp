using System;
using DatingAppMvc.Models;

namespace DatingAppMvc.Dtos
{
    public class MessageToRetrunDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        // Auto mapper is smart enought to map SenderKnowAs to KnowAs / same for PhotoUrl
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientKnownAs{ get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }

    }
}