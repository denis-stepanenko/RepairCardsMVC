using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreateCardPurchasedProductByCardViewModel
    {
        [Required(ErrorMessage = "Выберите ПКИ")]
        public List<string> Ids { get; set; }
    }
}
