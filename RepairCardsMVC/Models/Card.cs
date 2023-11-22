using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class Card
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Номер\".")]
        [RegularExpression(@"^[0-9]+/[0-9]+\.[0-9]{2}$", ErrorMessage = "Поле \"Номер\" должно иметь формат \"любые цифры/номер цеха.две цифры года\" (например \"290/17.20\").")]
        [Remote(action: "VerifyCardNumber", controller: "Card", AdditionalFields = nameof(Id), ErrorMessage = "Карта ремонта с таким номером уже существует.")]
        [StringLength(50)]
        public string Number { get; set; }

        [RegularExpression(@"^[0-9]+/[0-9]+\.[0-9]{2}$", ErrorMessage = "Поле \"Номер акта\" должно иметь формат \"любые цифры/номер цеха.две цифры года\" (например \"290/17.20\").")]
        [Remote(action: "VerifyActNumber", controller: "Card", AdditionalFields = nameof(Id), ErrorMessage = "Карта с таким номером акта уже существует")]
        [StringLength(50)]
        public string? ActNumber { get; set; }

        [RegularExpression(@"^[0-9]+/[0-9]+\.[0-9]{2}$", ErrorMessage = "Поле \"Номер карты разрешения\" должно иметь формат \"любые цифры/номер цеха.две цифры года\" (например \"290/17.20\").")]
        [StringLength(50)]
        public string? PermissionCardNumber { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Цех\".")]
        public int Department { get; set; }

        [RegularExpression(@"^0000-[0-9]+$", ErrorMessage = "Поле \"Заказ производства\" должно иметь формат \"0000-любые цифры\" (например \"0000-150\").")]
        [StringLength(50)]
        public string? Stage { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Заводской номер\".")]
        [StringLength(50)]
        public string FactoryNumber { get; set; }
        public string? Order { get; set; }
        public string? Direction { get; set; }
        public string? Cipher { get; set; }
        public string? ClientOrder { get; set; }

        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Продукт\".")]
        public string ProductCode { get; set; }

        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Номер накладной\".")]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Дата поступления\".")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Не указано поле \"Откуда поступил\".")]
        public int Source { get; set; }

        [StringLength(200)]
        public string? ReasonForRepair { get; set; }

        public int? RepairTypeId { get; set; }
        public RepairType? RepairType { get; set; }

        public int? CardStatusId { get; set; }
        public CardStatus? CardStatus { get; set; }

        public int? ParentId { get; set; }
        public Card? Parent { get; set; }

        public int? ParentId2 { get; set; }
        public Card? Parent2 { get; set; }

        public string? CreatorName { get; set; }

        public bool HasNotBeenRepaired { get; set; }

        public bool IsDepartment4Confirmed { get; set; }
        public bool IsDepartment5Confirmed { get; set; }
        public bool IsDepartment6Confirmed { get; set; }
        public bool IsDepartment13Confirmed { get; set; }
        public bool IsDepartment17Confirmed { get; set; }
        public bool IsDepartment80Confirmed { get; set; }
        public bool IsDepartment82Confirmed { get; set; }
        public bool IsDepartment90Confirmed { get; set; }

        public List<CardOperation>? Operations { get; set; }
    }
}
