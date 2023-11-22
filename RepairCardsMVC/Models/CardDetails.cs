using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class CardDetails
    {
        public int Id { get; set; }
        [StringLength(8000)]
        public string? ExternalDefects { get; set; }
        [StringLength(8000)]
        public string? InternalDefects { get; set; }
        [StringLength(8000)]
        public string? Malfunctions { get; set; }
        [StringLength(8000)]
        public string? CauseOfProductFailure { get; set; }
        [StringLength(8000)]
        public string? ScopeOfRepair { get; set; }
        [StringLength(8000)]
        public string? CommissionReport { get; set; }
    }
}
