using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class Template
    {
        public int Id { get; set; }
        public int Department { get; set; }

        [Required(ErrorMessage = "Введите наименование")]
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }
    }
}
