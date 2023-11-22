namespace RepairCardsMVC.ViewModels
{
    public class ReportProductOperationViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Labor { get; set; }
        public int Type { get; set; }
        public DateTime Date { get; set; }
        public int Department { get; set; }
        public string Executor { get; set; }
        public int OperationCount { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int ProductCount { get; set; }
    }
}
