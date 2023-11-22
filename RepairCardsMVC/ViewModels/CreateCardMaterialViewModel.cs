using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.ViewModels
{
    public class CreateCardMaterialViewModel
    {
        [Required(ErrorMessage = "Выберите материалы")]
        public List<string> SelectedItems { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }

        public int Department { get; set; }
    }
}
