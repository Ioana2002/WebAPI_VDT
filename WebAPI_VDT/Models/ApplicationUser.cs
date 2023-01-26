using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_VDT.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "NVARCHAR(150)")]
        public string FullName { get; set; }
    }
}
