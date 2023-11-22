using System.ComponentModel.DataAnnotations.Schema;

namespace RepairCardsMVC.Models
{
    public class Material
    {
        [Column("MaterialId")]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }
        public decimal? ThicknessDiameter { get; set; }
        public bool? IsPrecious { get; set; }
        public decimal? Price { get; set; }

        [NotMapped]
        public bool IsChecked { get; set; }
    }
}
