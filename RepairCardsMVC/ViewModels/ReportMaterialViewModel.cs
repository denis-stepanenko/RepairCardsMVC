namespace RepairCardsMVC.ViewModels
{
    public class ReportMaterialViewModel
    {
        public string CardNumber { get; set; }
        public string Direction { get; set; }
        public string Cipher { get; set; }
        public int Department { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Count { get; set; }
        public decimal Price { get; set; }
        public string ArrivalUnitName { get; set; }
        public string CurrentUnitName { get; set; }
    }
}
