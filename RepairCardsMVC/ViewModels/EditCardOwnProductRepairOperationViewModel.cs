using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditCardOwnProductRepairOperationViewModel
    {
        [Required(ErrorMessage = "Укажите исполнителя")]
        public int? ExecutorId { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }
    }
}
