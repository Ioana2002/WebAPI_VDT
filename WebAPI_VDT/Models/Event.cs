using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_VDT.Models
{
    public class Event
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid? EvenimentId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string Denumire { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Data_Inceput { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Data_Sfarsit { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Locatia { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Judet { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Descriere { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Poster { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Ora { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string TipEveniment { get; set; }
    }
}
