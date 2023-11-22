using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int Department { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Укажите карту")]
        public string CardNumber { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? ProductFactoryNumber { get; set; }

        public int RepairTypeId { get; set; }
        public RepairType? RepairType { get; set; }

        public string? ShortOrder { get; set; }

        public string? ContractNumber { get; set; }

        public string? RepairCode { get; set; }
        public string? RepairName { get; set; }
        public string? RepairOrder { get; set; }
        public string? RepairDirection { get; set; }
        public string? RepairCipher { get; set; }
        public string? RepairClientOrder { get; set; }

        public bool Constructor { get; set; }
        public DateTime? ConstructorConfirmDate { get; set; }
    }
}
