namespace GossipBook.Services.Models
{
    using System.ComponentModel.DataAnnotations;

    public class GroupDataModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
    }
}