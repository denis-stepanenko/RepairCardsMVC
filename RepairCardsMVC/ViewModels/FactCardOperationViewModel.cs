namespace RepairCardsMVC.ViewModels
{
    public class FactCardOperationViewModel
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Labor { get; set; }
        public int Type { get; set; }
        public DateTime? Date { get; set; }

        public decimal LaborAll { get; set; }
        public string UnitName { get; set; }
        public string GroupName { get; set; }
        public int Department { get; set; }
        public string TemplateName { get; set; }

        public string Executor { get; set; }
    }
}
