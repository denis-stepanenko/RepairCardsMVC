using RepairCardsMVC.Models;

namespace RepairCardsMVC.ViewModels
{
    public class CardReportViewModel
    {
        public List<SalariedEmployeeLaborCoefficient> Coefficients { get; set; }
        public int CardId { get; set; }
        public int CardsPageNumber { get; set; }
        public string? CardsFilter { get; set; }
    }
}
