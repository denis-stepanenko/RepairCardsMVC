using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не меньше 6 символов")]
        public string NewPassword { get; set; }
    }
}
