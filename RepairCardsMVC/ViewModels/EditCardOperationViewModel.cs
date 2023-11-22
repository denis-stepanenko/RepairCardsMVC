using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditCardOperationViewModel
    {
        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }

        [Required(ErrorMessage = "Укажите исполнителя")]
        public int? ExecutorId { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime Date { get; set; }
    }
}
