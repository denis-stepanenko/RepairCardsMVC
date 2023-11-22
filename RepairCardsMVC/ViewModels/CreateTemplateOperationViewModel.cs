using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreateTemplateOperationViewModel
    {
        [Required(ErrorMessage = "Выберите операции")]
        public List<string> Ids { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }
    }
}
