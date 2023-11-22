using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class CardMaterial
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public decimal Count { get; set; }

        public decimal? Price { get; set; }

        public int? Department { get; set; }

        public int UnitId { get; set; }

        public Card Card { get; set; }
    }
}
