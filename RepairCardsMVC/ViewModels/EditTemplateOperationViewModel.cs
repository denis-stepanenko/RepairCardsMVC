using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditTemplateOperationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }
    }
}
