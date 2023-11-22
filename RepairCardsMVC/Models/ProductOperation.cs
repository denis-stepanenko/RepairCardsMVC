using Microsoft.EntityFrameworkCore;

namespace RepairCardsMVC.Models
{
    public class ProductOperation
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Labor { get; set; }
        public int Department { get; set; }
    }
}
