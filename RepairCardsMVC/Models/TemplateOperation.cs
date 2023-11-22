namespace RepairCardsMVC.Models
{
    public class TemplateOperation
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int Number { get; set; }

        public int OperationId { get; set; }
        public Operation Operation { get; set; }

        public int Count { get; set; }
    }
}
