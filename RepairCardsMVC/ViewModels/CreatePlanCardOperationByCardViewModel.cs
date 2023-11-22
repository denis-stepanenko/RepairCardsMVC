using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreatePlanCardOperationByCardViewModel
    {
        [Required(ErrorMessage = "Выберите операции")]
        public List<string>? Ids { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }

        [Required(ErrorMessage = "Укажите исполнителя")]
        public int? ExecutorId { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime Date { get; set; }
    }
}
