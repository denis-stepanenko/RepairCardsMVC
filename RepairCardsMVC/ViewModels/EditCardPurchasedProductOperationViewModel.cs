using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditCardPurchasedProductOperationViewModel
    {
        [Required(ErrorMessage = "Укажите исполнителя")]
        public int? ExecutorId { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime? Date { get; set; }
    }
}
