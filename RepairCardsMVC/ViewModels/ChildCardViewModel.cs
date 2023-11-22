namespace RepairCardsMVC.ViewModels
{
    public class ChildCardViewModel
    {
        public int Id { get; set; }
        public string? ParentId { get; set; }
        public string Number { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Direction { get; set; }
        public string Cipher { get; set; }
        public bool HasNotBeenRepaired { get; set; }
    }
}
