using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class Executor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите наименование")]
        public string Name { get; set; }
        public int Department { get; set; }
    }
}
