using System.ComponentModel.DataAnnotations.Schema;

namespace RepairCardsMVC.Models
{
    public class PurchasedProduct
    {
        public int Id { get; set; }
        [Column("decnum")]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
