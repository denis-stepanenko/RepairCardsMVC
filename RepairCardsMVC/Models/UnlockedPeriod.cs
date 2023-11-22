using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class UnlockedPeriod
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите год")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Введите месяц")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Укажите карту")]
        public int? CardId { get; set; }

        public Card? Card { get; set; }

        public string? CreatorName { get; set; }
    }
}
