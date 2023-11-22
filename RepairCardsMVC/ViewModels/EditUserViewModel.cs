using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Введите имя пользователя")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите ФИО")]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите подразделение")]
        public int Department { get; set; }

        public List<string>? Roles { get; set; }
    }
}
