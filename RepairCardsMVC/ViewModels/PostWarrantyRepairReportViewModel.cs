namespace RepairCardsMVC.ViewModels
{
    public class PostWarrantyRepairReportViewModel
    {
        public string ParentCardProductCode { get; set; }
        public string ParentCardProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string FactoryNumber { get; set; }
        public string ParentFactoryNumber { get; set; }

        public string ExternalDefects { get; set; }
        public string InternalDefects { get; set; }
        public string Malfunctions { get; set; }
        public string CauseOfProductFailure { get; set; }
        public string ScopeOfRepair { get; set; }
        public string CommissionReport { get; set; }

        public string MainProductCode { get; set; }
        public string MainProductName { get; set; }
        public string MainProductFactoryNumber { get; set; }
    }
}
