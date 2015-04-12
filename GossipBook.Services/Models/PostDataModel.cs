namespace GossipBook.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PostDataModel
    {
        [Required]
        public string Content { get; set; }

        public DateTime PostedAt { get; set; }
    }
}