namespace RepairCardsMVC.ViewModels
{
    public class ReportCardViewModel
    {
        public int Id { get; set; }
        public string Number { get; set; }

        public int Department { get; set; }
        public string Order { get; set; }
        public string Stage { get; set; }
        public string FactoryNumber { get; set; }
        public string Direction { get; set; }
        public string Cipher { get; set; }
        public string ClientOrder { get; set; }
        public int? SpecificationType { get; set; }

        public int? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string InvoiceNumber { get; set; }
        public DateTime? Date { get; set; }
        public int Source { get; set; }
        public string ReasonForRepair { get; set; }

        public string RepairType { get; set; }

        public string ParentNumber { get; set; }
        public string ParentProductCode { get; set; }
    }
}
