using System.ComponentModel.DataAnnotations.Schema;

namespace RepairCardsMVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Column("type")]
        public int TableId { get; set; }
        [Column("decnum")]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
