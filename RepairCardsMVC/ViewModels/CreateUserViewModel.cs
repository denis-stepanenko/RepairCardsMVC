using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Введите имя пользователя")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите ФИО")]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите подразделение")]
        public int Department { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не меньше 6 символов")]
        public string Password { get; set; }
    }
}
