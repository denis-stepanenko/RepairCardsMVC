using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditCardOwnProductViewModel
    {
        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }
    }
}
