using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_VDT.Models
{
    public class Profile
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string ProfileId { get; set; }
        [ForeignKey("UserId")]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string Nume { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string Prenume { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Telefon { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Gen { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Adresa { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string Oras { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string Judet { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string Tara { get; set; }
        [Column(TypeName = "int")]
        public int Varsta { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Poza_CI { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string CNP { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Serie_CI { get; set; }
        [Column(TypeName = "int")]
        public int? Numar_CI { get; set; }
        [Column(TypeName = "varchar(2000)")]
        public string DespreMine { get; set; }

    }
}
