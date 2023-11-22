using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Введите наименование")]
        public string Name { get; set; }
    }
}
