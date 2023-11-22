using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class EditCardMaterialViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public decimal Count { get; set; }
    }
}
