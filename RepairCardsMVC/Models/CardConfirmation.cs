namespace RepairCardsMVC.Models
{
    public class CardConfirmation
    {
        public int Id { get; set; }
        public int CardId { get; set; }

        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public Role UserRole { get; set; }
        public string UserName { get; set; }

        public int CardConfirmationObjectId { get; set; }
        public CardConfirmationObject CardConfirmationObject { get; set; }

        public DateTime Date { get; set; }
    }
}
