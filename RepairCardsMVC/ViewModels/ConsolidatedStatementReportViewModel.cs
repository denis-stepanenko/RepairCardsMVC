namespace RepairCardsMVC.ViewModels
{
    public class ConsolidatedStatementReportViewModel
    {
        public string CardNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Direction { get; set; }
        public string Order { get; set; }
        public string FactoryNumber { get; set; }

        public int Department { get; set; }
        public string DepartmentName { get; set; }

        public decimal Labor { get; set; }
        public decimal LaborWithCoefficient { get; set; }

        public decimal Cost { get; set; }
        public decimal CostWithCoefficient { get; set; }
    }
}
