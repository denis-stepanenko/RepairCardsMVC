using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairCardsMVC.Models
{
    [Keyless]
    [Table("v_compacted_orders2")]
    public class Order
    {
        public string? Number { get; set; }
        public string? Direction { get; set; }
        public string? Cipher { get; set; }
        public string? ClientOrder { get; set; }
    }
}
