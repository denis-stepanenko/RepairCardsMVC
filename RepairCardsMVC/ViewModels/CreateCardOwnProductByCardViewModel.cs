using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreateCardOwnProductByCardViewModel
    {
        [Required(ErrorMessage = "Выберите ДСЕ")]
        public List<string>? Ids { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime Date { get; set; }
    }
}
