using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class CardOperation
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int? Number { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }
        public decimal Labor { get; set; }
        public int Type { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime Date { get; set; }

        public decimal LaborAll { get; set; }
        public string? UnitName { get; set; }
        public string? GroupName { get; set; }
        public int Department { get; set; }

        //[Required(ErrorMessage = "Укажите исполнителя")]
        public int? ExecutorId { get; set; }
        public Executor? Executor { get; set; }

        public string? TemplateCode { get; set; }
        public string? TemplateName { get; set; }

        public string? Comment { get; set; }

        public string? ConfirmUserName { get; set; }
        public int? ConfirmUserId { get; set; }

        public bool IsConfirmed => ConfirmUserId != null;

        public Card? Card { get; set; }
    }
}
