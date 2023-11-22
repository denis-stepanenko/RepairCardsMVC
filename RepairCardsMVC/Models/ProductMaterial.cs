using Microsoft.EntityFrameworkCore;

namespace RepairCardsMVC.Models
{
    public class ProductMaterial
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }
        public decimal Count { get; set; }
        public decimal? Price { get; set; }

        public int UnitId { get; set; }
    }
}
