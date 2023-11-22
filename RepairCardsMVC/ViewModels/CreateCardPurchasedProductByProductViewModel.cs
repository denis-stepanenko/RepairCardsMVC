using System.ComponentModel.DataAnnotations;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.ViewModels
{
    public class CreateCardPurchasedProductByProductViewModel
    {
        public List<Relation> Items { get; set; }
        public int CardId { get; set; }
    }
}
