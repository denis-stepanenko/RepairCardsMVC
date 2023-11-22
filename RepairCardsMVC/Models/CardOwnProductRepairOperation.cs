namespace RepairCardsMVC.Models
{
    public class CardOwnProductRepairOperation
    {
        public int Id { get; set; }
        public int CardOwnProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Labor { get; set; }
        public decimal LaborAll { get; set; }
        public DateTime Date { get; set; }
        public int Department { get; set; }
        public string UnitName { get; set; }
        public string GroupName { get; set; }

        public int ExecutorId { get; set; }
        public Executor? Executor { get; set; }

        public int? ConfirmUserId { get; set; }
        public string? ConfirmUserName { get; set; }
    }
}
