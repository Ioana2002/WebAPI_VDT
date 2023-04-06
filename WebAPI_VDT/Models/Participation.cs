using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_VDT.Models
{
    public class Participation
    {

        [Key]
        public int ID { get; set; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid? ParticipareId { get; set; }
        [ForeignKey("EvenimentId")]
        [Column(TypeName = "int")]
        public int? EvenimentId { get; set; }
        [ForeignKey("ParticipantId")]
        [Column(TypeName = "int")]
        public int? ParticipantId { get; set; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid? EvenimentGuid { get; set; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid? ParticipantGuid { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string ParticipantNume { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string Taxa { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataInscriere { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public string Telefon { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Email { get; set; }
    }
}
