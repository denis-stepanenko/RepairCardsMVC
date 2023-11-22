using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditCardPurchasedProductViewModel
    {
        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }
    }
}
