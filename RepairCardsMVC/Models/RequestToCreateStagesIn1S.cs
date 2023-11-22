namespace RepairCardsMVC.Models
{
    public class RequestToCreateStagesIn1S
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public int Department { get; set; }

        public string? CustomerName { get; set; }
        public string? ExecutorName { get; set; }

        public DateTime Date { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}
