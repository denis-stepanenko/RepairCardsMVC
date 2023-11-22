using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class CardStatus
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите наименование")]
        public string Name { get; set; }
    }
}
