using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class CardDocument
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        
        [Required(ErrorMessage = "Введите наименование")]
        [StringLength(300)]
        public string Name { get; set; }
    }
}
