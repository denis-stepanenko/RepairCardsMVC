using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class RepairType
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите наименование")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
