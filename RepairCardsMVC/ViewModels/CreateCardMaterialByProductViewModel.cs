using RepairCardsMVC.Models;

namespace RepairCardsMVC.ViewModels
{
    public class CreateCardMaterialByProductViewModel
    {
        public List<ProductMaterial> Items { get; set; }
        public int Department { get; set; }
        public int CardId { get; set; }
    }
}
