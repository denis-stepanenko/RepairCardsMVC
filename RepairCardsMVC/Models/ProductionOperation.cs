namespace RepairCardsMVC.Models
{
    public class ProductionOperation
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public List<Operation> Operations { get; set; }
    }
}
