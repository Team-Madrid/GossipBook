namespace GossipBook.Services.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CommentDataModel
    {
        [Required]
        public string Content { get; set; }
    }
}