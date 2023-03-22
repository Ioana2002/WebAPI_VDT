using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_VDT.Models
{
    public class ProfilePicture
    {
        [Key]
        public string ProfilePictureId { get; set; }
        [ForeignKey("UserId")]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Picture_URL { get; set; }
    }
}
